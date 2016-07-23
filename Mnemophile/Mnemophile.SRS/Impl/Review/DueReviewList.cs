using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mnemophile.Const.SRS;
using Mnemophile.Interfaces.DB;
using Mnemophile.SRS.Models;
using Mnemophile.Utils;
using Mnemophile.Utils.Collections;
using Mnemophile.Utils.LazyLoad;

namespace Mnemophile.SRS.Impl.Review
{
  internal class DueReviewList : ReviewAsyncDbListBase
  {
    //
    // Const

    private const int IncrementalLoadMax = 10;
    private const int IncrementalLoadMin = 5;



    //
    // Properties

    private int DueCardPerDay { get; }
    private IComparer<Card> Comparer { get; }



    //
    // Constructor

    public DueReviewList(IDatabase db, CollectionConfig config) : base(db)
    {
      DueCardPerDay = config.DueCardPerDay;
      Comparer = ReviewComparers.DueComparer;
    }



    // 
    // Core methods

    /// <summary>
    ///     Available number of cards (capped by user's configured number of
    ///     due card per day), excluding dismissed ones.
    /// </summary>
    public int AvailableCount => Objects
      .Where(c => !DismissedIds.Contains(c.Id))
      .Take(DueCardPerDay)
      .Count();

    /// <summary>
    ///     Computes total review count
    /// </summary>
    public int ReviewCount => Objects
      .Where(c => !DismissedIds.Contains(c.Id))
      .Take(DueCardPerDay)
      .Sum(c => c.ReviewLeftToday());

    private int ReserveSize => Objects.Count - DueCardPerDay - Dismissed;



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
      int totalLoadCount = DueCardPerDay + IncrementalLoadMax;
      int fullLoadCount = Math.Min(totalLoadCount, IncrementalFurtherLoadMax);
      int shallowLoadCount = totalLoadCount - fullLoadCount;

      ITableQuery<Card> tableQuery =
        Db.Table<Card>()
          .Take(fullLoadCount)
          .Where(c => c.PracticeState == ConstSRS.CardPracticeState.Due);

      Objects.AddRange(tableQuery.OrderBy(c => c.Due));

      FurtherLoadedIndex = Objects.Count - 1;

      if (shallowLoadCount > 0 && Objects.Count == fullLoadCount)
      {
        tableQuery =
          Db.Table<Card>()
            .Take(shallowLoadCount)
            .ShallowLoad(LazyLoader)
            .Where(c =>
                   c.PracticeState == ConstSRS.CardPracticeState.Due
                   && !Objects.Select(o => o.Id).Contains(c.Id));

        Objects.AddRange(tableQuery.OrderBy(c => c.Due));
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
      int loadCount = IncrementalLoadMax - ReserveSize;

      ITableQuery<Card> tableQuery =
        Db.Table<Card>()
          .Take(loadCount)
          .Where(c =>
                 c.PracticeState == ConstSRS.CardPracticeState.Due
                 && !Objects.Select(o => o.Id).Contains(c.Id));

      if (!fullLoad)
        tableQuery = tableQuery.ShallowLoad(LazyLoader);

      IEnumerable<Card> dueCards = tableQuery.OrderBy(c => c.Due);
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
    ///     Keep at least <see cref="DueCardPerDay" /> active cards in list
    /// </summary>
    /// <returns></returns>
    protected override int GetMaxIndexLoadThreshold()
    {
      return Index + 1 + Objects.Count - DueCardPerDay - Dismissed;
    }

    protected override int GetFurtherLoadedIndex()
    {
      return FurtherLoadedIndex;
    }

    protected override int GetNextLoadThreshold()
    {
      return Index + 1 + Objects.Count - DueCardPerDay - Dismissed
             - IncrementalLoadMin;
    }
  }
}