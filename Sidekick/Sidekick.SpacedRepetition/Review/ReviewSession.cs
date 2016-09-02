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
using Sidekick.Shared.Extensions;
using Sidekick.Shared.Interfaces.Database;
using Sidekick.SpacedRepetition.Const;
using Sidekick.SpacedRepetition.Models;

namespace Sidekick.SpacedRepetition.Review
{
  /// <summary>
  ///   Defines all reviews on a given day (session)
  /// </summary>
  public class ReviewSession
  {
    #region Constructors

    public ReviewSession(IDatabase db, CollectionConfig config)
    {
      ComputeSessionInfos(db, config);
    }

    #endregion

    #region Properties

    public int New { get; set; }
    public int Due { get; set; }

    #endregion

    #region Methods

    public void ComputeSessionInfos(
      IDatabase db, CollectionConfig config)
    {
      int todayStart = DateTime.Today.ToUnixTimestamp();
      int todayEnd = DateTime.Today.AddDays(1).ToUnixTimestamp();

      IEnumerable<ReviewLog> logs =
        db.Table<ReviewLog>()
          .Where(l =>
                 l.Id >= todayStart && l.Id < todayEnd
                 && (l.LastState == CardPracticeState.New
                     || l.LastState == CardPracticeState.Due))
          .SelectColumns(nameof(ReviewLog.LastState))
          .ToList();

      int newReviewedToday = logs.Count(
        l => l.LastState == CardPracticeState.New);
      int dueReviewedToday = logs.Count() - newReviewedToday;

      New = config.NewCardPerDay - newReviewedToday;
      Due = config.DueCardPerDay - dueReviewedToday;
    }

    #endregion
  }
}