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
  using System.Linq;
  using System.Threading.Tasks;

  using MethodTimer;

  using Sidekick.Shared.Interfaces.Database;
  using Sidekick.Shared.Utils.Collections;
  using Sidekick.Shared.Utils.LazyLoad;
  using Sidekick.SpacedRepetition.Const;
  using Sidekick.SpacedRepetition.Models;

  /// <summary>
  ///   ReviewList implementation for New cards.
  ///   Doesn't use any comparer, new cards are either loaded randomly or in order from DB.
  ///   <see cref="ReviewCount" /> returns the sum of all cards <see cref="Card.ReviewLeftToday" />
  ///   within <see cref="NewCardsLeft" /> limits.
  /// </summary>
  /// <seealso cref="Sidekick.SpacedRepetition.Review.ReviewAsyncDbListBase" />
  internal class NewReviewList : ReviewAsyncDbListBase
  {
    #region Fields

    private const int IncrementalLoadMax = 10;
    private const int IncrementalLoadMin = 5;

    #endregion



    #region Constructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="NewReviewList" /> class.
    /// </summary>
    /// <param name="db">Database instance</param>
    /// <param name="config">Current session collection configuration</param>
    /// <param name="newCardsLeft">New cards count for current session</param>
    public NewReviewList(IDatabaseAsync db, CollectionConfig config, int newCardsLeft)
      : base(db)
    {
      NewCardsLeft = newCardsLeft;
      Random = config.InsertionOption == CardOrderingOption.Random;

      Initialize(true);
    }

    #endregion



    #region Properties

    private bool Random { get; }
    private int NewCardsLeft { get; }

    private int ReserveSize => Objects.Count - NewCardsLeft - Dismissed;

    #endregion



    #region Methods

    // 
    // Core methods

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

        return
          Objects.Where(c => !DismissedIds.Contains(c.Id))
                 .Take(NewCardsLeft)
                 .Sum(c => c.IsNew() ? c.ReviewLeftToday() : 0);
      }
    }

    //
    // AsyncDbListBase core methods implementation

    /// <summary>
    ///   Load items.
    /// </summary>
    /// <param name="fullLoad">
    ///   If true, full objects should be loaded.
    ///   Only relevant when using lazy loading.
    /// </param>
    /// <returns>Whether any item was loaded.</returns>
    protected override Task<bool> DoLoadMoreAsync(bool fullLoad)
    {
      bool firstLoad = Objects.Count == 0;

      return firstLoad ? DoFirstLoadAsync() : DoRegularLoadAsync(fullLoad);
    }

    /// <summary>
    ///   Fill the item list with its first items.
    /// </summary>
    /// <returns>Whether any item was loaded.</returns>
    [Time]
    protected async Task<bool> DoFirstLoadAsync()
    {
      int totalLoadCount = NewCardsLeft + IncrementalLoadMax;
      int fullLoadCount = Math.Min(totalLoadCount, IncrementalFurtherLoadMax);
      int shallowLoadCount = totalLoadCount - fullLoadCount;

      ITableQueryAsync<Card> tableQuery =
        Db.Table<Card>()
          .Where(c => c.PracticeState == CardPracticeState.New)
          .Take(fullLoadCount);

      tableQuery = Random ? tableQuery.OrderByRand() : tableQuery.OrderBy(c => c.Due);

      // Fully load up to IncrementalFurtherLoadMax items
      int loadedCount = await AddItemsAsync(tableQuery).ConfigureAwait(false);

      FurtherLoadedIndex = loadedCount - 1;

      if (shallowLoadCount > 0 && loadedCount == fullLoadCount)
      {
        tableQuery =
          Db.Table<Card>()
            .ShallowLoad(LazyLoader)
            .Where(
              c =>
                c.PracticeState == CardPracticeState.New
                && !Objects.Select(o => o.Id).Contains(c.Id))
            .Take(shallowLoadCount);

        tableQuery = Random ? tableQuery.OrderByRand() : tableQuery.OrderBy(c => c.Due);

        loadedCount += await AddItemsAsync(tableQuery).ConfigureAwait(false);
      }

      if (loadedCount < totalLoadCount)
        Status = ReviewStatus.MoveNextEndOfStore;

      return loadedCount > 0;
    }

    /// <summary>
    ///   Load more items in an already initialized item list.
    /// </summary>
    /// <param name="fullLoad">
    ///   If true, full objects should be loaded.
    ///   Only relevant when using lazy loading.
    /// </param>
    /// <returns>Whether more items were loaded.</returns>
    [Time]
    protected async Task<bool> DoRegularLoadAsync(bool fullLoad)
    {
      int loadCount = IncrementalLoadMax - ReserveSize;

      ITableQueryAsync<Card> tableQuery =
        Db.Table<Card>()
          .Where(
            c =>
              c.PracticeState == CardPracticeState.New
              && !Objects.Select(o => o.Id).Contains(c.Id))
          .Take(loadCount);

      if (!fullLoad)
        tableQuery = tableQuery.ShallowLoad(LazyLoader);

      tableQuery = Random ? tableQuery.OrderByRand() : tableQuery.OrderBy(c => c.Due);

      int loadedCount = await AddItemsAsync(tableQuery).ConfigureAwait(false);

      if (loadedCount < loadCount)
        Status = ReviewStatus.MoveNextEndOfStore;

      else if (fullLoad)
        FurtherLoadedIndex = loadedCount - 1;

      return loadedCount > 0;
    }

    /// <summary>
    ///   Sort cards according to provided comparer.
    ///   TODO: Use a more efficient backing type for sorting
    ///   TODO: Should this be in implementing class
    /// </summary>
    protected override void Sort()
    {
      // Do nothing
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
    ///   Keep at least <see cref="NewCardsLeft" /> active cards in list
    /// </summary>
    /// <returns>Maximum reachable index.</returns>
    protected override int GetMaxIndexLoadThreshold()
    {
      return Index + 1 + Objects.Count - NewCardsLeft - Dismissed;
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
      return Index + 1 + Objects.Count - NewCardsLeft - Dismissed - IncrementalLoadMin;
    }

    #endregion
  }
}