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



    //
    // Constructor

    public ReviewCollectionImpl(IDatabase db, CollectionConfig config)
    {
      Db = db;
      Random = new Random();

      CardTableName = Db.GetTableMapping<Card>().GetTableName();
      
      CurrentList = null;
      NextAction =
        new Dictionary<ReviewAsyncDbListBase, Func<Task<bool>>>();

      Initialize(db, config);
    }



    //
    // IReviewCollection implementation

    public Task<bool> Initialized { get; set; }

    public ICard Current => CurrentList?.Current;

    public Task<bool> Answer(ConstSRS.Grade grade)
    {
      Card card = (Card)Current;

      if (card == null)
        throw new InvalidOperationException("Card unavailable");

      bool isNewCard = card.IsNew();
      int noteId = card.NoteId;

      NextAction[CurrentList] = () => CurrentList.MoveNext();
      CurrentList = null;

      card.Answer(grade, Db);

      // If this was a new card, add to learning list
      if (isNewCard && card.IsLearning())
        LearnReviewList.AddManual(card);

      // Dismiss sibling cards
      Task.Run(() =>
               {
                 using (Db.Lock())
                 {
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

    private void Initialize(IDatabase db, CollectionConfig config)
    {
      NewReviewList = new NewReviewList(db, config);
      LearnReviewList = new LearnReviewList(db);
      DueReviewList = new DueReviewList(db, config);

      NextAction[NewReviewList] = () => NewReviewList.MoveNext();
      NextAction[LearnReviewList] = () => LearnReviewList.MoveNext();
      NextAction[DueReviewList] = () => DueReviewList.MoveNext();

      Initialized = DoNext();
    }

    private async Task<bool> DoNext()
    {
      CurrentList = await GetNextCardSource();

      return CurrentList != null && await NextAction[CurrentList]();
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