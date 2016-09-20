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

  using Sidekick.Shared.Extensions;
  using Sidekick.Shared.Interfaces.Database;
  using Sidekick.SpacedRepetition.Models;

  /// <summary>Defines all reviews on a given day (session)</summary>
  public class ReviewSession
  {
    #region Constructors

    /// <summary>Initializes a new instance of the <see cref="ReviewSession" /> class.</summary>
    /// <param name="newCount">The new count.</param>
    /// <param name="dueCount">The due count.</param>
    protected ReviewSession(int newCount, int dueCount)
    {
      New = newCount;
      Due = dueCount;
    }

    #endregion



    #region Properties

    /// <summary>New card count for session.</summary>
    public int New { get; }

    /// <summary>Due card count for session.</summary>
    public int Due { get; }

    #endregion



    #region Methods

    /// <summary>
    ///   Computes the session card review count values asynchronously.
    /// </summary>
    /// <param name="db">Database instance.</param>
    /// <param name="config">The configuration.</param>
    /// <returns></returns>
    public static async Task<ReviewSession> ComputeSessionAsync(
      IDatabaseAsync db, CollectionConfig config)
    {
      int todayStart = DateTime.Today.ToUnixTimestamp();
      int todayEnd = DateTimeExtensions.Tomorrow.ToUnixTimestamp();

      IEnumerable<ReviewLog> logs =
        await
          db.Table<ReviewLog>()
            .Where(
              l =>
                l.Id >= todayStart && l.Id < todayEnd
                && (l.LastState == PracticeState.New || l.LastState == PracticeState.Due))
            .SelectColumns(nameof(ReviewLog.LastState))
            .ToListAsync()
            .ConfigureAwait(false);

      int newReviewedToday = logs.Count(l => l.LastState == PracticeState.New);
      int dueReviewedToday = logs.Count() - newReviewedToday;

      int newCount = config.NewCardPerDay - newReviewedToday;
      int dueCount = config.DueCardPerDay - dueReviewedToday;

      return new ReviewSession(newCount, dueCount);
    }

    #endregion
  }
}