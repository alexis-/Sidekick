using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sidekick.Shared.Const.SpacedRepetition;
using Sidekick.Shared.Interfaces.DB;
using Sidekick.Shared.Interfaces.SpacedRepetition;
using Sidekick.Shared.Utils;
using Sidekick.SpacedRepetition.Models;

namespace Sidekick.SpacedRepetition.Impl.Review
{
  public class ReviewCollectionImpl : IReviewCollection
  {
    const int MaxEvalTime = 60;

    //
    // Card types lists

    private NewReviewList NewReviewList { get; set; }
    private LearnReviewList LearnReviewList { get; set; }
    private DueReviewList DueReviewList { get; set; }

    private ReviewAsyncDbListBase CurrentList { get; set; }


    //
    // Misc

    public IDatabase Db { get; set; }
    private string CardTableName { get; }
    private Random Random { get; }
    private Dictionary<ReviewAsyncDbListBase,
      Func<Task<bool>>> NextAction { get; }
    
    private int EvalStartTime { get; set; }
    private int LastEval { get; set; }
    private bool FakeLog { get; }



    //
    // Constructor

    public ReviewCollectionImpl(IDatabase db, CollectionConfig config,
      bool fakeLog = false)
    {
      Db = db;
      FakeLog = fakeLog;
      LastEval = -1;
      Random = new Random();

      CardTableName = Db.GetTableMapping<Card>().GetTableName();

      CurrentList = null;
      NextAction =
        new Dictionary<ReviewAsyncDbListBase, Func<Task<bool>>>();
      
      Initialized = Initialize(db, config);
    }



    //
    // IReviewCollection implementation

    public Task<bool> Initialized { get; set; }

    public ICard Current => CurrentList?.Current;

    public Task<bool> Answer(ConstSpacedRepetition.Grade grade)
    {
      // Card sanity check
      Card card = (Card)Current;

      if (card == null)
        throw new InvalidOperationException("Card unavailable");


      //
      // Compute evaluation time & create review log

      int evalTime = Math.Min(
        DateTime.Now.ToUnixTimestamp() - EvalStartTime,
        MaxEvalTime);
      
      ReviewLog log = new ReviewLog(
        EvalStartTime, card.Id, card.Due, card.PracticeState,
        card.Interval, card.EFactor);

      if (FakeLog && LastEval > 0)
        log.Id = Math.Max(LastEval + 1, EvalStartTime);

      LastEval = log.Id;


      //
      // Answer

      int noteId = card.NoteId;

      NextAction[CurrentList] = () => CurrentList.MoveNext();
      CurrentList = null;

      card.Answer(grade, Db);

      log.CompleteReview(
        grade, card.Due, card.PracticeState,
        card.Interval, card.EFactor, evalTime);

      // If this was a new card, add to learning list
      if (log.LastState == ConstSpacedRepetition.CardPracticeState.New)
        LearnReviewList.AddManual(card);


      //
      // Dismiss sibling cards & insert review log

      Task.Run(() =>
               {
                 using (Db.Lock())
                 {
                   Db.Insert(log);

                   Db.Query<Card>(
                     @"UPDATE """ + CardTableName
                     + @""" SET ""Due"" = ? WHERE ""Due"" < ?"
                     + @" AND ""NoteId"" = ? AND ""Id"" <> ?",
                     DateTime.Today.AddDays(1).ToUnixTimestamp(),
                     DateTime.Today.AddDays(1).ToUnixTimestamp(),
                     noteId, card.Id);
                 }
               }
        );

      NewReviewList.DismissSiblings(card);
      LearnReviewList.DismissSiblings(card);
      DueReviewList.DismissSiblings(card);

      return DoNext();
    }

    public Task<bool> Dismiss()
    {
      Card card = (Card)Current;

      if (card == null)
        throw new InvalidOperationException("Card unavailable");

      NextAction[CurrentList] = () => CurrentList.Dismiss();
      CurrentList = null;

      card.Dismiss(Db);

      return DoNext();
    }

    public int CountByState(ConstSpacedRepetition.CardPracticeStateFilterFlag state)
    {
      int ret = 0;

      if ((state & ConstSpacedRepetition.CardPracticeStateFilterFlag.Due) ==
          ConstSpacedRepetition.CardPracticeStateFilterFlag.Due)
        ret += DueReviewList.ReviewCount();

      if ((state & ConstSpacedRepetition.CardPracticeStateFilterFlag.Learning) ==
          ConstSpacedRepetition.CardPracticeStateFilterFlag.Learning)
        ret += LearnReviewList.ReviewCount();

      if ((state & ConstSpacedRepetition.CardPracticeStateFilterFlag.New) ==
          ConstSpacedRepetition.CardPracticeStateFilterFlag.New)
        ret += NewReviewList.ReviewCount();

      return ret;
    }



    //
    // Internal

    private async Task<bool> Initialize(IDatabase db, CollectionConfig config)
    {
      ReviewSession reviewSession = new ReviewSession(Db, config);

      NewReviewList = new NewReviewList(db, config, reviewSession.New);
      LearnReviewList = new LearnReviewList(db);
      DueReviewList = new DueReviewList(db, reviewSession.Due);

      NextAction[NewReviewList] = () => NewReviewList.MoveNext();
      NextAction[LearnReviewList] = () => LearnReviewList.MoveNext();
      NextAction[DueReviewList] = () => DueReviewList.MoveNext();

      await Task.WhenAll(
        NewReviewList.Initialized(),
        LearnReviewList.Initialized(),
        DueReviewList.Initialized());

      //await NewReviewList.Initialized();
      //await LearnReviewList.Initialized();
      //await DueReviewList.Initialized();

      return await DoNext();
    }
    

    private Task<bool> DoNext()
    {
      CurrentList = GetNextCardSource();

      if (CurrentList == null)
        return TaskConstants.BooleanFalse;

      Task<bool> nextAction = NextAction[CurrentList]();

      if (nextAction.IsCompleted)
      {
        EvalStartTime = DateTime.Now.ToUnixTimestamp();
        return nextAction;
      }

      return WaitNextAction(nextAction);
    }

    private async Task<bool> WaitNextAction(Task<bool> nextAction)
    {
      bool ret = await nextAction;

      EvalStartTime = DateTime.Now.ToUnixTimestamp();

      return ret;
    }

    private ReviewAsyncDbListBase GetNextCardSource()
    {
      // TODO: Fix await - makes pre-loading useless
      int newReviewCount = NewReviewList.ReviewCount();
      int learnReviewCount = LearnReviewList.ReviewCount();
      int dueReviewCount = DueReviewList.ReviewCount();

      int totalReviewCount =
        newReviewCount + learnReviewCount + dueReviewCount;

      int rnd = Random.Next(0, totalReviewCount);

      if (rnd < newReviewCount)
        return NewReviewList;

      if (rnd < newReviewCount + learnReviewCount)
        return LearnReviewList;

      if (rnd < newReviewCount + learnReviewCount + dueReviewCount)
        return DueReviewList;
      
      return null;
    }
  }
}