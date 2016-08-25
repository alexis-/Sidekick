using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mnemophile.Const.SRS;
using Mnemophile.Interfaces.DB;
using Mnemophile.SRS.Models;
using Mnemophile.Utils;

namespace Mnemophile.SRS.Impl.Review
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
                 && (l.LastState == ConstSRS.CardPracticeState.New
                     || l.LastState == ConstSRS.CardPracticeState.Due))
          .SelectColumns(nameof(ReviewLog.LastState))
          .ToList();

      int newReviewedToday = logs.Count(l =>
                            l.LastState == ConstSRS.CardPracticeState.New);
      int dueReviewedToday = logs.Count() - newReviewedToday;

      New = config.NewCardPerDay - newReviewedToday;
      Due = config.DueCardPerDay - dueReviewedToday;
    }
  }
}
