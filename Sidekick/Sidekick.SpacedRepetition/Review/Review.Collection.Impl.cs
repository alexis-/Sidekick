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

namespace Sidekick.SpacedRepetition.Review
{
  using System;
  using System.Collections.Generic;
  using System.Threading.Tasks;

  using MethodTimer;

  using Sidekick.Shared.Extensions;
  using Sidekick.Shared.Interfaces.Database;
  using Sidekick.Shared.Utils;
  using Sidekick.SpacedRepetition.Extensions;
  using Sidekick.SpacedRepetition.Interfaces;
  using Sidekick.SpacedRepetition.Models;

  /// <summary>Loads and schedule cards to be displayed for review.</summary>
  /// <seealso cref="Sidekick.SpacedRepetition.Interfaces.IReviewCollection" />
  public class ReviewCollectionImpl : IReviewCollection
  {
    #region Fields

    private const int MaxEvalTime = 60;
    private readonly IDatabaseAsync _db;
    private readonly bool _fakeLog;
    private readonly Random _random;

    private string _cardTableName;

    #endregion



    #region Constructors

    //
    // Constructor

    /// <summary>Initializes a new instance of the <see cref="ReviewCollectionImpl" /> class.</summary>
    /// <param name="db">Database instance</param>
    /// <param name="config">Configuration for reviewed collection</param>
    /// <param name="fakeLog">
    ///   For testing purpose, fakes <see cref="ReviewLog" /> times. ReviewLog Id
    ///   field is based on current timestamp, and may lead to unique constraint conflict.
    /// </param>
    public ReviewCollectionImpl(
      IDatabaseAsync db, CollectionConfig config, bool fakeLog = false)
    {
      _db = db;
      _fakeLog = fakeLog;
      _random = new Random();

      LastEval = -1;
      CurrentList = null;
      NextAction = new Dictionary<ReviewCollectionAsyncBase, Func<Task<bool>>>();

      Initialized = InitializeAsync(config);
    }

    #endregion



    #region Properties

    //
    // IReviewCollection implementation

    /// <summary>
    ///   Waitable Task to check whether ReviewCollection is ready. ReviewCollection should be
    ///   initialized before calling other any other method.
    /// </summary>
    /// <value>
    ///   True if ReviewCollection is initialized and ready. False if no card is available for
    ///   review.
    /// </value>
    public Task<bool> Initialized { get; }

    /// <summary>Last fetched card.</summary>
    public Card Current => CurrentList?.Current;

    //
    // Card types lists

    private ReviewCollectionNew ReviewCollectionNew { get; set; }
    private ReviewCollectionLearn ReviewCollectionLearn { get; set; }
    private ReviewCollectionDue ReviewCollectionDue { get; set; }

    private ReviewCollectionAsyncBase CurrentList { get; set; }

    private Dictionary<ReviewCollectionAsyncBase, Func<Task<bool>>> NextAction { get; }

    //
    // Misc

    private int EvalStartTime { get; set; }
    private int LastEval { get; set; }

    #endregion



    #region Methods

    /// <summary>Answer current card and fetch next one.</summary>
    /// <param name="grade">Answer grade</param>
    /// <returns>Whether any cards are available</returns>
    /// <exception cref="System.InvalidOperationException">No card available (Current is null).</exception>
    public Task<bool> AnswerAsync(Grade grade)
    {
      // Card sanity check
      Card card = Current;

      if (card == null)
        throw new InvalidOperationException("Card unavailable");

      // Compute evaluation time & create review log
      ReviewLog log = CreateLog(card);

      // Answer
      NextAction[CurrentList] = () => CurrentList.MoveNextAsync();
      CurrentList = null;

      CardAction cardAction = card.Answer(grade);

      // Complete log with updated values
      CompleteLog(log, card, grade);

      // If this was a new card, add to learning list
      if (log.LastState == PracticeState.New)
        ReviewCollectionLearn.AddManual(card);
      
#pragma warning disable 4014
      // Save changes to Database
      UpdateCardAsync(log, card, cardAction);
#pragma warning restore 4014

      ReviewCollectionNew.DismissSiblings(card);
      ReviewCollectionLearn.DismissSiblings(card);
      ReviewCollectionDue.DismissSiblings(card);

      return DoNextAsync();
    }

    /// <summary>Dismiss current card and fetch next one.</summary>
    /// <returns>Whether any cards are available</returns>
    /// <exception cref="System.InvalidOperationException">No card available (Current is null).</exception>
    public Task<bool> DismissAsync()
    {
      Card card = Current;

      if (card == null)
        throw new InvalidOperationException("Card unavailable");

      NextAction[CurrentList] = () => CurrentList.DismissAsync();
      CurrentList = null;

      // Create review log before dismiss
      ReviewLog log = CreateLog(card);

      // Actually dismiss card
      CardAction cardAction = card.Dismiss();

      // Complete log with updated values
      CompleteLog(log, card, Grade.Dismiss);

#pragma warning disable 4014
      // Save changes to Database
      UpdateCardAsync(log, card, cardAction);
#pragma warning restore 4014

      return DoNextAsync();
    }

    /// <summary>State-dependent counts of cards to be reviewed.</summary>
    /// <param name="state">The state.</param>
    /// <returns>State-dependent card count.</returns>
    public int CountByState(CardPracticeStateFilterFlag state)
    {
      int ret = 0;

      if ((state & CardPracticeStateFilterFlag.Due) == CardPracticeStateFilterFlag.Due)
        ret += ReviewCollectionDue.ReviewCount();

      if ((state & CardPracticeStateFilterFlag.Learning) == CardPracticeStateFilterFlag.Learning)
        ret += ReviewCollectionLearn.ReviewCount();

      if ((state & CardPracticeStateFilterFlag.New) == CardPracticeStateFilterFlag.New)
        ret += ReviewCollectionNew.ReviewCount();

      return ret;
    }

    //
    // Internal

    private ReviewLog CreateLog(Card card)
    {
      int evalTime = Math.Min(DateTime.Now.ToUnixTimestamp() - EvalStartTime, MaxEvalTime);

      ReviewLog log = new ReviewLog(
        EvalStartTime, card.Id, card.Due, card.PracticeState, card.Interval, card.EFactor,
        evalTime);

      if (_fakeLog && LastEval > 0)
        log.Id = Math.Max(LastEval + 1, EvalStartTime);

      LastEval = log.Id;

      return log;
    }

    private void CompleteLog(ReviewLog log, Card card, Grade grade)
    {
      log.CompleteReview(
        grade, card.Due, card.PracticeState, card.Interval, card.EFactor);
    }

    private async Task UpdateCardAsync(ReviewLog log, Card card, CardAction cardAction)
    {
      Task[] tasks;
      Task logTask = _db.InsertAsync(log);
      Task cardUpdateTask = _db.UpdateAsync(card);

      if (cardAction == CardAction.Dismiss || cardAction == CardAction.Delete)
        tasks = new[] { cardUpdateTask, logTask };

      else
      {
        Task siblingsTask =
          _db.QueryAsync<Card>(
            @"UPDATE """ + _cardTableName + @""" SET ""Due"" = ? WHERE ""Due"" < ?"
            + @" AND ""NoteId"" = ? AND ""Id"" <> ?",
            DateTimeExtensions.Tomorrow.ToUnixTimestamp(),
            DateTimeExtensions.Tomorrow.ToUnixTimestamp(), card.NoteId, card.Id);

        tasks = new[] { cardUpdateTask, logTask, siblingsTask };
      }

      await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    [Time]
    private async Task<bool> InitializeAsync(CollectionConfig config)
    {
      _cardTableName =
        (await _db.GetTableMappingAsync<Card>().ConfigureAwait(false)).GetTableName();

      ReviewSession reviewSession =
        await ReviewSession.ComputeSessionAsync(_db, config).ConfigureAwait(false);

      ReviewCollectionNew = new ReviewCollectionNew(_db, config, reviewSession.New);
      ReviewCollectionLearn = new ReviewCollectionLearn(_db);
      ReviewCollectionDue = new ReviewCollectionDue(_db, reviewSession.Due);

      NextAction[ReviewCollectionNew] = () => ReviewCollectionNew.MoveNextAsync();
      NextAction[ReviewCollectionLearn] = () => ReviewCollectionLearn.MoveNextAsync();
      NextAction[ReviewCollectionDue] = () => ReviewCollectionDue.MoveNextAsync();

      await
        Task.WhenAll(
              ReviewCollectionNew.IsInitializedAsync(), ReviewCollectionLearn.IsInitializedAsync(),
              ReviewCollectionDue.IsInitializedAsync()).ConfigureAwait(false);

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
    private ReviewCollectionAsyncBase GetNextCardSource()
    {
      int newReviewCount = ReviewCollectionNew.ReviewCount();
      int learnReviewCount = ReviewCollectionLearn.ReviewCount();
      int dueReviewCount = ReviewCollectionDue.ReviewCount();

      int totalReviewCount = newReviewCount + learnReviewCount + dueReviewCount;

      int rnd = _random.Next(0, totalReviewCount);

      if (rnd < newReviewCount)
        return ReviewCollectionNew;

      if (rnd < newReviewCount + learnReviewCount)
        return ReviewCollectionLearn;

      if (rnd < newReviewCount + learnReviewCount + dueReviewCount)
        return ReviewCollectionDue;

      return null;
    }

    #endregion
  }
}