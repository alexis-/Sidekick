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
  internal class LearnReviewList : ReviewAsyncDbListBase
  {
    //
    // Properties
    
    private IComparer<Card> Comparer { get; }



    //
    // Constructor

    public LearnReviewList(IDatabase db) : base(db)
    {
      Comparer = ReviewComparers.DueComparer;
    }



    // 
    // Core methods

    /// <summary>
    ///     Available number of cards (capped by user's configured number of
    ///     learning card per day), excluding dismissed ones.
    /// </summary>
    public int AvailableCount => Objects
      .Count(c => !DismissedIds.Contains(c.Id));

    /// <summary>
    ///     Computes total review count
    /// </summary>
    public int ReviewCount => Objects
      .Where(c => !DismissedIds.Contains(c.Id))
      .Sum(c => c.ReviewLeftToday());



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

      return firstLoad && DoFirstLoad();
    }

    /// <summary>
    ///     Fill the item list with its first items.
    /// </summary>
    /// <returns>Whether any item was loaded.</returns>
    protected bool DoFirstLoad()
    {
      int fullLoadCount = IncrementalFurtherLoadMax;

      ITableQuery<Card> tableQuery =
        Db.Table<Card>()
          .Take(fullLoadCount)
          .Where(c => c.PracticeState == ConstSRS.CardPracticeState.Due);

      Objects.AddRange(tableQuery.OrderBy(c => c.Due));

      FurtherLoadedIndex = Objects.Count - 1;

      if (Objects.Count == fullLoadCount)
      {
        tableQuery =
          Db.Table<Card>()
            .ShallowLoad(LazyLoader)
            .Where(c =>
                   c.PracticeState == ConstSRS.CardPracticeState.Due
                   && !Objects.Select(o => o.Id).Contains(c.Id));

        Objects.AddRange(tableQuery.OrderBy(c => c.Due));
      }
      
      Status = ReviewStatus.MoveNextEndOfStore;

      return Objects.Count > 0;
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
    
    protected override int GetMaxIndexLoadThreshold()
    {
      return -1;
    }

    protected override int GetFurtherLoadedIndex()
    {
      return FurtherLoadedIndex;
    }

    protected override int GetNextLoadThreshold()
    {
      return -1;
    }
  }
}