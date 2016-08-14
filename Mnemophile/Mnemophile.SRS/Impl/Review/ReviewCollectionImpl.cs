using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Mnemophile.Attributes.DB;
using Mnemophile.Const.SRS;
using Mnemophile.Interfaces.DB;
using Mnemophile.Interfaces.SRS;
using Mnemophile.SRS.Models;
using Mnemophile.Utils;

namespace Mnemophile.SRS.Impl.Review
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

    public Task<bool> Answer(ConstSRS.Grade grade)
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
      if (log.LastState == ConstSRS.CardPracticeState.New
        && log.NewState == ConstSRS.CardPracticeState.Learning)
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
                     + @" AND ""NoteId"" = ?",
                     DateTime.Today.AddDays(1).ToUnixTimestamp(),
                     DateTime.Today.AddDays(1).ToUnixTimestamp(),
                     noteId);
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

    public int CountByState(ConstSRS.CardPracticeStateFilterFlag state)
    {
      return -1;
    }



    //
    // Internal

    private async Task<bool> Initialize(IDatabase db, CollectionConfig config)
    {
      int newToday, dueToday;
      ComputeSessionInfos(config, out newToday, out dueToday);

      NewReviewList = new NewReviewList(db, config, newToday);
      LearnReviewList = new LearnReviewList(db);
      DueReviewList = new DueReviewList(db, dueToday);

      NextAction[NewReviewList] = () => NewReviewList.MoveNext();
      NextAction[LearnReviewList] = () => LearnReviewList.MoveNext();
      NextAction[DueReviewList] = () => DueReviewList.MoveNext();

      return await DoNext();
    }

    private void ComputeSessionInfos(
      CollectionConfig config, out int newToday, out int dueToday)
    {
      int todayStart = DateTime.Today.ToUnixTimestamp();
      int todayEnd = DateTime.Today.AddDays(1).ToUnixTimestamp();

      IEnumerable<ReviewLog> logs =
        Db.Table<ReviewLog>()
          .Where(l =>
                 l.Id >= todayStart && l.Id < todayEnd
                 && l.LastState != ConstSRS.CardPracticeState.Learning)
          .SelectColumns(nameof(ReviewLog.Id), nameof(ReviewLog.LastState));

      int newReviewedToday = logs.Count(l =>
                            l.LastState == ConstSRS.CardPracticeState.New);
      int dueReviewedToday = logs.Count() - newReviewedToday;

      newToday = config.NewCardPerDay - newReviewedToday;
      dueToday = config.DueCardPerDay - dueReviewedToday;
    }

    private async Task<bool> DoNext()
    {
      CurrentList = await GetNextCardSource();

      var ret = CurrentList != null && await NextAction[CurrentList]();

      EvalStartTime = DateTime.Now.ToUnixTimestamp();

      return ret;
    }

    private async Task<ReviewAsyncDbListBase> GetNextCardSource()
    {
      // Fix await - makes pre-loading useless
      int newReviewCount = await NewReviewList.ReviewCount();
      int learnReviewCount = await LearnReviewList.ReviewCount();
      int dueReviewCount = await DueReviewList.ReviewCount();

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