using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mnemophile.SRS.Models
{
  public partial class Card
  {
    /// <summary>
    /// Configuration inherited by current Card
    /// </summary>
    internal CollectionConfig Config => CollectionConfig.Default;

    /// <summary>
    /// Used in reviewing card process. Store time of review, from which
    /// other values will be computed (Due, Steps, ...)
    /// </summary>
    internal int CurrentReviewTime { get; set; }

    /// <summary>
    /// Either return Config's Lapsing or Learning steps, depending on Card
    /// Lapse value.
    /// </summary>
    public int[] LearningOrLapsingSteps => Lapses > 0
      ? Config.LapseSteps
      : Config.LearningSteps;

    public bool IsNew()
    {
      return (State & CardStateFlag.New) == CardStateFlag.New;
    }

    public bool IsLearning()
    {
      return (State & CardStateFlag.Learning) == CardStateFlag.Learning;
    }

    public bool IsDue()
    {
      return (State & CardStateFlag.Due) == CardStateFlag.Due;
    }

    public bool IsBuried()
    {
      return (State & CardStateFlag.Buried) == CardStateFlag.Buried;
    }

    public bool IsSuspended()
    {
      return (State & CardStateFlag.Suspended) == CardStateFlag.Suspended;
    }
  }
}
