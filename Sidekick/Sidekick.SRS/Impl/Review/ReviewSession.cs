using System;
using System.Collections.Generic;
using System.Linq;
using Sidekick.Shared.Const.SpacedRepetition;
using Sidekick.Shared.Interfaces.DB;
using Sidekick.Shared.Utils;
using Sidekick.SpacedRepetition.Models;

namespace Sidekick.SpacedRepetition.Impl.Review
{
  public class ReviewSession
  {
    public int New { get; set; }
    public int Due { get; set; }

    public ReviewSession(IDatabase db, CollectionConfig config)
    {
      ComputeSessionInfos(db, config);
    }
    
    public void ComputeSessionInfos(
      IDatabase db, CollectionConfig config)
    {
      int todayStart = DateTime.Today.ToUnixTimestamp();
      int todayEnd = DateTime.Today.AddDays(1).ToUnixTimestamp();

      IEnumerable<ReviewLog> logs =
        db.Table<ReviewLog>()
          .Where(l =>
                 l.Id >= todayStart && l.Id < todayEnd
                 && (l.LastState == ConstSpacedRepetition.CardPracticeState.New
                     || l.LastState == ConstSpacedRepetition.CardPracticeState.Due))
          .SelectColumns(nameof(ReviewLog.LastState))
          .ToList();

      int newReviewedToday = logs.Count(l =>
                            l.LastState == ConstSpacedRepetition.CardPracticeState.New);
      int dueReviewedToday = logs.Count() - newReviewedToday;

      New = config.NewCardPerDay - newReviewedToday;
      Due = config.DueCardPerDay - dueReviewedToday;
    }
  }
}
