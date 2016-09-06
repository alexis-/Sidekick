// 
// The MIT License (MIT)
// 
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the 
// Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MethodTimer;
using Sidekick.Shared.Extensions;
using Sidekick.Shared.Interfaces.Database;
using Sidekick.Shared.Utils;
using Sidekick.SpacedRepetition.Const;
using Sidekick.SpacedRepetition.Interfaces;
using Sidekick.SpacedRepetition.Models;

namespace Sidekick.SpacedRepetition.Review
{
  /// <summary>
  ///   Loads and schedule cards to be displayed for review.
  /// </summary>
  /// <seealso cref="Sidekick.SpacedRepetition.Interfaces.IReviewCollection" />
  public class ReviewCollectionImpl : IReviewCollection
  {
    #region Fields

    private const int MaxEvalTime = 60;

    private readonly string _cardTableName;
    private readonly IDatabase _db;
    private readonly bool _fakeLog;
    private readonly Random _random;

    #endregion



    #region Constructors

    //
    // Constructor

    /// <summary>
    ///   Initializes a new instance of the <see cref="ReviewCollectionImpl" /> class.
    /// </summary>
    /// <param name="db">Database instance</param>
    /// <param name="config">Configuration for reviewed collection</param>
    /// <param name="fakeLog">
    ///   For testing purpose, fakes <see cref="ReviewLog" /> times.
    ///   ReviewLog Id field is based on current timestamp, and may lead to unique
    ///   constraint conflict.
    /// </param>
    public ReviewCollectionImpl(IDatabase db, CollectionConfig config, bool fakeLog = false)
    {
      _db = db;
      _fakeLog = fakeLog;
      LastEval = -1;
      _random = new Random();

      _cardTableName = _db.GetTableMapping<Card>().GetTableName();

      CurrentList = null;
      NextAction = new Dictionary<ReviewAsyncDbListBase, Func<Task<bool>>>();

      Initialized = InitializeAsync(db, config);
    }

    #endregion



    #region Properties

    //
    // IReviewCollection implementation

    /// <summary>
    ///   Waitable Task to check whether ReviewCollection is ready.
    ///   ReviewCollection should be initialized before calling other any
    ///   other method.
    /// </summary>
    /// <value>
    ///   True if ReviewCollection is initialized and ready.
    ///   False if no card is available for review.
    /// </value>
    public Task<bool> Initialized { get; }

    /// <summary>
    ///   Last fetched card.
    /// </summary>
    public Card Current => CurrentList?.Current;

    //
    // Card types lists

    private NewReviewList NewReviewList { get; set; }
    private LearnReviewList LearnReviewList { get; set; }
    private DueReviewList DueReviewList { get; set; }

    private ReviewAsyncDbListBase CurrentList { get; set; }

    private Dictionary<ReviewAsyncDbListBase, Func<Task<bool>>> NextAction { get; }

    //
    // Misc

    private int EvalStartTime { get; set; }
    private int LastEval { get; set; }

    #endregion



    #region Methods

    /// <summary>
    ///   Answer current card and fetch next one.
    /// </summary>
    /// <param name="grade">Answer grade</param>
    /// <returns>
    ///   Whether any cards are available
    /// </returns>
    /// <exception cref="System.InvalidOperationException">
    ///   No card available (Current is null).
    /// </exception>
    public Task<bool> AnswerAsync(Grade grade)
    {
      // Card sanity check
      Card card = Current;

      if (card == null)
        throw new InvalidOperationException("Card unavailable");

      // Compute evaluation time & create review log
      int evalTime = Math.Min(DateTime.Now.ToUnixTimestamp() - EvalStartTime, MaxEvalTime);

      ReviewLog log = new ReviewLog(
        EvalStartTime, card.Id, card.Due, card.PracticeState, card.Interval, card.EFactor);

      if (_fakeLog && LastEval > 0)
        log.Id = Math.Max(LastEval + 1, EvalStartTime);

      LastEval = log.Id;

      // Answer
      int noteId = card.NoteId;

      NextAction[CurrentList] = () => CurrentList.MoveNextAsync();
      CurrentList = null;

      card.Answer(grade, _db);

      log.CompleteReview(
        grade, card.Due, card.PracticeState, card.Interval, card.EFactor, evalTime);

      // If this was a new card, add to learning list
      if (log.LastState == CardPracticeState.New)
        LearnReviewList.AddManual(card);

      // Dismiss sibling cards & insert review log
      Task.Run(
        () =>
        {
          using (_db.Lock())
          {
            _db.Insert(log);

            _db.Query<Card>(
              @"UPDATE """ + _cardTableName + @""" SET ""Due"" = ? WHERE ""Due"" < ?"
              + @" AND ""NoteId"" = ? AND ""Id"" <> ?",
              DateTimeExtensions.Tomorrow.ToUnixTimestamp(),
              DateTimeExtensions.Tomorrow.ToUnixTimestamp(), noteId, card.Id);
          }
        });

      NewReviewList.DismissSiblings(card);
      LearnReviewList.DismissSiblings(card);
      DueReviewList.DismissSiblings(card);

      return DoNextAsync();
    }

    /// <summary>
    ///   Dismiss current card and fetch next one.
    /// </summary>
    /// <returns>
    ///   Whether any cards are available
    /// </returns>
    /// <exception cref="System.InvalidOperationException">
    ///   No card available (Current is null).
    /// </exception>
    public Task<bool> DismissAsync()
    {
      Card card = Current;

      if (card == null)
        throw new InvalidOperationException("Card unavailable");

      NextAction[CurrentList] = () => CurrentList.DismissAsync();
      CurrentList = null;

      card.Dismiss(_db);

      return DoNextAsync();
    }

    /// <summary>
    ///   State-dependent counts of cards to be reviewed.
    /// </summary>
    /// <param name="state">The state.</param>
    /// <returns>
    ///   State-dependent card count.
    /// </returns>
    public int CountByState(CardPracticeStateFilterFlag state)
    {
      int ret = 0;

      if ((state & CardPracticeStateFilterFlag.Due) == CardPracticeStateFilterFlag.Due)
        ret += DueReviewList.ReviewCount();

      if ((state & CardPracticeStateFilterFlag.Learning) == CardPracticeStateFilterFlag.Learning)
        ret += LearnReviewList.ReviewCount();

      if ((state & CardPracticeStateFilterFlag.New) == CardPracticeStateFilterFlag.New)
        ret += NewReviewList.ReviewCount();

      return ret;
    }

    //
    // Internal

    [Time]
    private async Task<bool> InitializeAsync(IDatabase db, CollectionConfig config)
    {
      ReviewSession reviewSession = new ReviewSession(_db, config);

      NewReviewList = new NewReviewList(db, config, reviewSession.New);
      LearnReviewList = new LearnReviewList(db);
      DueReviewList = new DueReviewList(db, reviewSession.Due);

      NextAction[NewReviewList] = () => NewReviewList.MoveNextAsync();
      NextAction[LearnReviewList] = () => LearnReviewList.MoveNextAsync();
      NextAction[DueReviewList] = () => DueReviewList.MoveNextAsync();

      await Task.WhenAll(
                  NewReviewList.IsInitializedAsync(), LearnReviewList.IsInitializedAsync(),
                  DueReviewList.IsInitializedAsync()).ConfigureAwait(false);

      return await DoNextAsync().ConfigureAwait(false);
    }

    [Time]
    private Task<bool> DoNextAsync()
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

      return WaitNextActionAsync(nextAction);
    }

    [Time]
    private async Task<bool> WaitNextActionAsync(Task<bool> nextAction)
    {
      bool ret = await nextAction.ConfigureAwait(false);

      EvalStartTime = DateTime.Now.ToUnixTimestamp();

      return ret;
    }

    [Time]
    private ReviewAsyncDbListBase GetNextCardSource()
    {
      int newReviewCount = NewReviewList.ReviewCount();
      int learnReviewCount = LearnReviewList.ReviewCount();
      int dueReviewCount = DueReviewList.ReviewCount();

      int totalReviewCount = newReviewCount + learnReviewCount + dueReviewCount;

      int rnd = _random.Next(0, totalReviewCount);

      if (rnd < newReviewCount)
        return NewReviewList;

      if (rnd < newReviewCount + learnReviewCount)
        return LearnReviewList;

      if (rnd < newReviewCount + learnReviewCount + dueReviewCount)
        return DueReviewList;

      return null;
    }

    #endregion
  }
}