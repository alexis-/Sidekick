using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mnemophile.Const.SpacedRepetition;
using Mnemophile.Interfaces.DB;
using Mnemophile.SpacedRepetition.Models;
using Mnemophile.Utils;
using Mnemophile.Utils.Collections;
using Mnemophile.Utils.LazyLoad;

namespace Mnemophile.SpacedRepetition.Impl.Review
{
  internal class DueReviewList : ReviewAsyncDbListBase
  {
    //
    // Const

    private const int IncrementalLoadMax = 10;
    private const int IncrementalLoadMin = 5;



    //
    // Properties

    private int DueCardsLeft { get; }
    private IComparer<Card> Comparer { get; }



    //
    // Constructor

    public DueReviewList(IDatabase db, int dueCardsLeft) : base(db)
    {
      DueCardsLeft = dueCardsLeft;
      Comparer = ReviewComparers.DueComparer;
    }



    // 
    // Core methods

    /// <summary>
    ///     Number of cards available for iteration
    ///     TODO: Check if loading ?
    /// </summary>
    public override int AvailableCount()
    {
      lock (LockObject)
      {
        if (Objects.Count == 0 && LoadCompletionSource != null
          && (Status == ReviewStatus.New || Status == ReviewStatus.MoveNext))
          throw new InvalidOperationException();

        return Objects
          .Skip(Index + 1)
          .Count();
      }
    }

    /// <summary>
    ///     Computes total review count
    /// </summary>
    public override int ReviewCount()
    {
      lock (LockObject)
      {
        if (Objects.Count == 0 && LoadCompletionSource != null
          && (Status == ReviewStatus.New || Status == ReviewStatus.MoveNext))
          throw new InvalidOperationException();

        return Objects
          .Where(c => !DismissedIds.Contains(c.Id))
          .Take(DueCardsLeft)
          .Sum(c => c.ReviewLeftToday());
      }
    }

    private int ReserveSize => Objects.Count - DueCardsLeft - Dismissed;



    //
    // AsyncDbListBase core methods implementation

    /// <summary>
    ///     Load items.
    /// </summary>
    /// <param name="fullLoad">
    ///     If true, full objects should be loaded.
    ///     Only relevant when using lazy loading.
    /// </param>
    /// <returns>Whether any item was loaded.</returns>
    protected override bool DoLoadMore(bool fullLoad)
    {
      bool firstLoad = Objects.Count == 0;

      lock (LockObject)
        return firstLoad
               ? DoFirstLoad()
               : DoRegularLoad(fullLoad);
    }

    /// <summary>
    ///     Fill the item list with its first items.
    /// </summary>
    /// <returns>Whether any item was loaded.</returns>
    protected bool DoFirstLoad()
    {
      int totalLoadCount = DueCardsLeft + IncrementalLoadMax;
      int fullLoadCount = Math.Min(totalLoadCount, IncrementalFurtherLoadMax);
      int shallowLoadCount = totalLoadCount - fullLoadCount;

      int tomorrow = DateTime.Today.AddDays(1).ToUnixTimestamp();

      using (Db.Lock())
      {
        ITableQuery<Card> tableQuery =
          Db.Table<Card>()
            .Where(c =>
                   c.PracticeState == ConstSpacedRepetition.CardPracticeState.Due
                   && c.Due < tomorrow)
            .Take(fullLoadCount);

        Objects.AddRange(tableQuery.OrderBy(c => c.Due));
      }

      FurtherLoadedIndex = Objects.Count - 1;

      if (shallowLoadCount > 0 && Objects.Count == fullLoadCount)
      {
        using (Db.Lock())
        {
          ITableQuery<Card> tableQuery =
            Db.Table<Card>()
              .ShallowLoad(LazyLoader)
              .Where(c =>
                     c.PracticeState == ConstSpacedRepetition.CardPracticeState.Due
                     && c.Due < tomorrow
                     && !Objects.Select(o => o.Id).Contains(c.Id))
              .Take(shallowLoadCount);

          Objects.AddRange(tableQuery.OrderBy(c => c.Due));
        }
      }

      if (Objects.Count < totalLoadCount)
        Status = ReviewStatus.MoveNextEndOfStore;

      return Objects.Count > 0;
    }

    /// <summary>
    ///     Load more items in an already initialized item list.
    /// </summary>
    /// <param name="fullLoad">
    ///     If true, full objects should be loaded.
    ///     Only relevant when using lazy loading.
    /// </param>
    /// <returns>Whether more items were loaded.</returns>
    protected bool DoRegularLoad(bool fullLoad)
    {
      IEnumerable<Card> dueCards;
      int loadCount = IncrementalLoadMax - ReserveSize;

      int tomorrow = DateTime.Today.AddDays(1).ToUnixTimestamp();

      using (Db.Lock())
      {
        ITableQuery<Card> tableQuery =
          Db.Table<Card>()
            .Where(c =>
                   c.PracticeState == ConstSpacedRepetition.CardPracticeState.Due
                   && c.Due < tomorrow
                   && !Objects.Select(o => o.Id).Contains(c.Id))
            .Take(loadCount);

        if (!fullLoad)
          tableQuery = tableQuery.ShallowLoad(LazyLoader);

        dueCards = tableQuery.OrderBy(c => c.Due);
      }

      int loadedCount = dueCards.Count();

      if (loadedCount < loadCount)
        Status = ReviewStatus.MoveNextEndOfStore;

      else
      {
        Objects.AddRange(dueCards);

        if (fullLoad)
          FurtherLoadedIndex = Objects.Count - 1;
      }

      return loadCount > 0;
    }

    /// <summary>
    ///     Sort cards according to provided comparer.
    ///     TODO: Use a more efficient backing type for sorting
    ///     TODO: Should this be in implementing class
    /// </summary>
    protected override void Sort()
    {
      Objects.Sort(Index + 1, Objects.Count - Index - 1, Comparer);
    }



    //
    // AsyncDbListBase load indicators implementation
    
    protected override int GetNextFurtherLoadThreshold()
    {
      return GetFurtherLoadedIndex() - IncrementalFurtherLoadMin;
    }

    /// <summary>
    ///     Keep at least <see cref="DueCardsLeft" /> active cards in list
    /// </summary>
    /// <returns></returns>
    protected override int GetMaxIndexLoadThreshold()
    {
      return Index + 1 + Objects.Count - DueCardsLeft - Dismissed;
    }

    protected override int GetFurtherLoadedIndex()
    {
      return FurtherLoadedIndex;
    }

    protected override int GetNextLoadThreshold()
    {
      return Index + 1 + Objects.Count - DueCardsLeft - Dismissed
             - IncrementalLoadMin;
    }
  }
}