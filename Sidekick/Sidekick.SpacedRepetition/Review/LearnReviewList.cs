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
  using System.Linq;
  using System.Threading.Tasks;

  using MethodTimer;

  using Sidekick.Shared.Extensions;
  using Sidekick.Shared.Interfaces.Database;
  using Sidekick.Shared.Utils;
  using Sidekick.Shared.Utils.Collections;
  using Sidekick.Shared.Utils.LazyLoad;
  using Sidekick.SpacedRepetition.Const;
  using Sidekick.SpacedRepetition.Models;

  /// <summary>
  ///   ReviewList implementation for Learning cards.
  ///   Uses <see cref="ReviewComparers.DueComparer" /> to sort cards by due date.
  ///   <see cref="ReviewCount" /> returns the sum of all cards
  ///   <see cref="Card.ReviewLeftToday" /> without any daily-limit (unlike
  ///   <see cref="DueReviewList" /> or <see cref="NewReviewList" />).
  /// </summary>
  /// <seealso cref="Sidekick.SpacedRepetition.Review.ReviewAsyncDbListBase" />
  internal class LearnReviewList : ReviewAsyncDbListBase
  {
    #region Constructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="LearnReviewList" /> class.
    /// </summary>
    /// <param name="db">Database instance</param>
    public LearnReviewList(IDatabaseAsync db) : base(db)
    {
      Comparer = ReviewComparers.DueComparer;

      Initialize(true);
    }

    #endregion



    #region Properties

    private IComparer<Card> Comparer { get; }

    #endregion



    #region Methods

    /// <summary>
    ///   Computes the number of available cards for iteration
    /// </summary>
    /// <returns></returns>
    /// <exception cref="System.InvalidOperationException">
    ///   List is not properly initialized
    /// </exception>
    public override int AvailableCount()
    {
      lock (LockObject)
      {
        if (Objects.Count == 0 && LoadCompletionSource != null
            && (Status == ReviewStatus.New || Status == ReviewStatus.MoveNext))
          throw new InvalidOperationException("List is not properly initialized");

        return Objects.Skip(Index + 1).Count();
      }
    }

    /// <summary>
    ///   Computes total review count left
    /// </summary>
    /// <returns></returns>
    /// <exception cref="System.InvalidOperationException">
    ///   List is not properly initialized
    /// </exception>
    public override int ReviewCount()
    {
      lock (LockObject)
      {
        if (Objects.Count == 0 && LoadCompletionSource != null
            && (Status == ReviewStatus.New || Status == ReviewStatus.MoveNext))
          throw new InvalidOperationException("List is not properly initialized");

        return Objects.Where(c => !DismissedIds.Contains(c.Id)).Sum(c => c.ReviewLeftToday());
      }
    }


    //
    // AsyncDbListBase core methods implementation

    /// <summary>
    ///   Learning cards may be reviewed more than once per day, overrides
    ///   default behaviour to account for multiple reviews.
    /// </summary>
    /// <returns>
    ///   Whether any item is available
    /// </returns>
    public override Task<bool> MoveNextAsync()
    {
      if (Current == null || Current.ReviewLeftToday() == 0)
        return base.MoveNextAsync();

      lock (LockObject)
      {
        Sort();

        Current = Objects[Index];
      }

      return TaskConstants.BooleanTrue;
    }

    /// <summary>
    ///   Load items.
    /// </summary>
    /// <param name="fullLoad">
    ///   If true, full objects should be loaded.
    ///   Only relevant when using lazy loading.
    /// </param>
    /// <returns>Whether any item was loaded.</returns>
    protected override async Task<bool> DoLoadMoreAsync(bool fullLoad)
    {
      bool firstLoad = Objects.Count == 0;

      return firstLoad && await DoFirstLoadAsync().ConfigureAwait(false);
    }

    /// <summary>
    ///   Fill the item list with its first items.
    /// </summary>
    /// <returns>Whether any item was loaded.</returns>
    [Time]
    protected async Task<bool> DoFirstLoadAsync()
    {
      int fullLoadCount = IncrementalFurtherLoadMax;
      int tomorrow = DateTimeExtensions.Tomorrow.ToUnixTimestamp();

      // Fully load up to IncrementalFurtherLoadMax items
      int loadedCount =
        await
          AddItemsAsync(
            Db.Table<Card>()
              .Where(c => c.PracticeState >= CardPracticeState.Learning && c.Due < tomorrow)
              .Take(fullLoadCount)
              .OrderBy(c => c.Due)).ConfigureAwait(false);

      // Update fully loaded index accordingly
      FurtherLoadedIndex = loadedCount - 1;

      if (loadedCount == fullLoadCount)
        loadedCount +=
          await
            AddItemsAsync(
              Db.Table<Card>()
                .ShallowLoad(LazyLoader)
                .Where(
                  c =>
                    c.PracticeState >= CardPracticeState.Learning && c.Due < tomorrow
                    && !Objects.Select(o => o.Id).Contains(c.Id))
                .OrderBy(c => c.Due)).ConfigureAwait(false);

      Status = ReviewStatus.MoveNextEndOfStore;

      return loadedCount > 0;
    }

    /// <summary>
    ///   Sort cards upon adding new cards. Object lock should already be in place when
    ///   calling this.
    ///   TODO: Use a more efficient backing type for sorting
    ///   TODO: Should this be in implementing class
    /// </summary>
    protected override void Sort()
    {
      int shift = Current == null || Current.ReviewLeftToday() == 0 ? 1 : 0;

      Objects.Sort(Index + shift, Objects.Count - Index - shift, Comparer);
    }


    //
    // AsyncDbListBase load indicators implementation

    /// <summary>
    ///   Index threshold used in determining when asynchronous further item loading should be
    ///   initiated. Only required when using lazy loading.
    /// </summary>
    /// <returns>Next load index threshold.</returns>
    protected override int GetNextFurtherLoadThreshold()
    {
      return GetFurtherLoadedIndex() - IncrementalFurtherLoadMin;
    }

    /// <summary>
    ///   Gets the maximum reachable index until awaiting item load to complete. If
    ///   <see cref="AsyncDbListBase{T}.ReviewStatus.MoveNextEndOfStore" /> is set, this is
    ///   ignored.
    /// </summary>
    /// <returns>Maximum reachable index.</returns>
    protected override int GetMaxIndexLoadThreshold()
    {
      return -1;
    }

    /// <summary>
    ///   Gets the index of last fully loaded item. Only required when using lazy loading.
    /// </summary>
    /// <returns>Index of last fully loaded item.</returns>
    protected override int GetFurtherLoadedIndex()
    {
      return FurtherLoadedIndex;
    }

    /// <summary>
    ///   Index threshold used in determining when asynchronous item loading should be initiated.
    /// </summary>
    /// <returns>Next load index threshold.</returns>
    protected override int GetNextLoadThreshold()
    {
      return -1;
    }

    #endregion
  }
}