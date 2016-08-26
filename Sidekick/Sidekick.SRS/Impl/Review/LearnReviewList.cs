using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sidekick.Shared.Const.SpacedRepetition;
using Sidekick.Shared.Interfaces.DB;
using Sidekick.Shared.Utils;
using Sidekick.Shared.Utils.LazyLoad;
using Sidekick.SpacedRepetition.Models;

namespace Sidekick.SpacedRepetition.Impl.Review
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
    ///     Computes total review count left
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
          .Sum(c => c.ReviewLeftToday());
      }
    }



    //
    // AsyncDbListBase core methods implementation

    /// <summary>
    ///     Learning cards may be reviewed more than once per day, overrides
    ///     default behaviour to account for multiple reviews.
    /// </summary>
    /// <returns>
    ///     Whether any item is available
    /// </returns>
    public override Task<bool> MoveNext()
    {
      if (Current == null || Current.ReviewLeftToday() == 0)
        return base.MoveNext();

      lock (LockObject)
        Sort();

      Current = Objects[Index];

      return TaskConstants.BooleanTrue;
    }

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
        return firstLoad && DoFirstLoad();
    }

    /// <summary>
    ///     Fill the item list with its first items.
    /// </summary>
    /// <returns>Whether any item was loaded.</returns>
    protected bool DoFirstLoad()
    {
      int fullLoadCount = IncrementalFurtherLoadMax;

      int tomorrow = DateTime.Today.AddDays(1).ToUnixTimestamp();

      using (Db.Lock())
      {
        ITableQuery<Card> tableQuery =
          Db.Table<Card>()
            .Where(c =>
                   c.PracticeState >= ConstSpacedRepetition.CardPracticeState.Learning
                   && c.Due < tomorrow)
            .Take(fullLoadCount);

        Objects.AddRange(tableQuery.OrderBy(c => c.Due));
      }

      FurtherLoadedIndex = Objects.Count - 1;

      if (Objects.Count == fullLoadCount)
      {
        using (Db.Lock())
        {
          ITableQuery<Card> tableQuery =
            Db.Table<Card>()
              .ShallowLoad(LazyLoader)
              .Where(c =>
                     c.PracticeState >= ConstSpacedRepetition.CardPracticeState.Learning
                     && c.Due < tomorrow
                     && !Objects.Select(o => o.Id).Contains(c.Id));

          Objects.AddRange(tableQuery.OrderBy(c => c.Due));
        }
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
      int shift = (Current == null || Current.ReviewLeftToday() == 0)
                    ? 1
                    : 0;
      Objects.Sort(
        Index + shift,
        Objects.Count - Index - shift,
        Comparer);
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