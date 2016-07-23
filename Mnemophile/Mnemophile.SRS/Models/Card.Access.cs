using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mnemophile.Const.SRS;
using Mnemophile.SRS.Impl;
using Mnemophile.Utils;

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
    public Delay[] LearningOrLapsingSteps => Lapses > 0
      ? Config.LapseSteps
      : Config.LearningSteps;

    /// <summary>
    /// Get current lapsing or learning step index.
    /// In the occurence steps settings changed, closest inferior value
    /// index is returned.
    /// Accounts for CardPracticeState offset.
    /// </summary>
    /// <returns>Current lapsing or learning index</returns>
    internal int GetCurrentLearningStep()
    {
      if (!IsLearning())
        throw new InvalidOperationException(
          "Invalid call for State " + PracticeState);

      return Math.Min(
        PracticeState - ConstSRS.CardPracticeState.Learning,
        LearningOrLapsingSteps.Length - 1);

      //int lastStepValue = Due - LastModified;
      //int lastStep = 0;

      //for (;
      //  lastStep < LearningOrLapsingSteps.Length
      //  && lastStepValue > LearningOrLapsingSteps[lastStep];
      //  lastStep++)
      //  ;

      //return lastStep;
    }

    /// <summary>
    /// Computes review count to graduation.
    /// Accounts for CardPracticeState offset.
    /// </summary>
    /// <returns>Review count to graduation</returns>
    /// <exception cref="System.InvalidOperationException">Invalid call for State  + PracticeState</exception>
    internal int GetLearningStepsLeft()
    {
      if (!IsLearning())
        throw new InvalidOperationException(
          "Invalid call for State " + PracticeState);

      return LearningOrLapsingSteps.Length - GetCurrentLearningStep();
    }

    public bool IsNew()
    {
      return PracticeState == ConstSRS.CardPracticeState.New;
    }

    public bool IsLearning()
    {
      return PracticeState >= ConstSRS.CardPracticeState.Learning;
    }

    public bool IsDue()
    {
      return PracticeState == ConstSRS.CardPracticeState.Due;
    }

    public bool IsDismissed()
    {
      return (MiscState & ConstSRS.CardMiscStateFlag.Dismissed) ==
        ConstSRS.CardMiscStateFlag.Dismissed;
    }

    public bool IsSuspended()
    {
      return (MiscState & ConstSRS.CardMiscStateFlag.Suspended) ==
        ConstSRS.CardMiscStateFlag.Suspended;
    }
  }
}
