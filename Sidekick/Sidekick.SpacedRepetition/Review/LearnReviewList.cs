// 
// The MIT License (MIT)
// Copyright (c) 2016 Incogito
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
using System.Linq;
using System.Threading.Tasks;
using Sidekick.Shared.Extensions;
using Sidekick.Shared.Interfaces.Database;
using Sidekick.Shared.Utils;
using Sidekick.Shared.Utils.LazyLoad;
using Sidekick.SpacedRepetition.Const;
using Sidekick.SpacedRepetition.Models;

namespace Sidekick.SpacedRepetition.Review
{
  internal class LearnReviewList : ReviewAsyncDbListBase
  {
    #region Constructors

    //
    // Constructor

    public LearnReviewList(IDatabase db) : base(db)
    {
      Comparer = ReviewComparers.DueComparer;
    }

    #endregion

    #region Properties

    //
    // Properties

    private IComparer<Card> Comparer { get; }

    #endregion

    #region Methods

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
                   c.PracticeState >= CardPracticeState.Learning
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
                     c.PracticeState >= CardPracticeState.Learning
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
      int shift = Current == null || Current.ReviewLeftToday() == 0
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

    #endregion
  }
}