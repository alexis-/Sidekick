using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mnemophile.Attributes.DB;
using Mnemophile.Const.SpacedRepetition;
using Mnemophile.SpacedRepetition.Impl;
using Mnemophile.Utils;

namespace Mnemophile.SpacedRepetition.Models
{
  public partial class Card
  {
    /// <summary>
    /// Configuration inherited by current Card
    /// </summary>
    [Ignore]
    internal CollectionConfig Config { get; set; }

    /// <summary>
    /// Used in reviewing card process. Store time of review, from which
    /// other values will be computed (Due, Steps, ...)
    /// </summary>
    [Ignore]
    public int CurrentReviewTime { get; set; }

    /// <summary>
    /// Either return Config's Lapsing or Learning steps, depending on Card
    /// Lapse value.
    /// </summary>
    [Ignore]
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
    public int GetCurrentLearningIndex()
    {
      if (!IsLearning())
        throw new InvalidOperationException(
          "Invalid call for State " + PracticeState);

      // Account for possible config changes
      return Math.Min(
        PracticeState - ConstSpacedRepetition.CardPracticeState.Learning,
        LearningOrLapsingSteps.Length - 1);
    }

    /// <summary>
    /// Computes review count to graduation.
    /// Accounts for CardPracticeState offset.
    /// </summary>
    /// <returns>Review count to graduation</returns>
    /// <exception cref="System.InvalidOperationException">Invalid call for State  + PracticeState</exception>
    public int GetLearningStepsLeft()
    {
      if (!IsLearning())
        throw new InvalidOperationException(
          "Invalid call for State " + PracticeState);

      return LearningOrLapsingSteps.Length - GetCurrentLearningIndex();
    }

    public override bool IsNew()
    {
      return PracticeState == ConstSpacedRepetition.CardPracticeState.New;
    }

    public override bool IsLearning()
    {
      return PracticeState >= ConstSpacedRepetition.CardPracticeState.Learning;
    }

    public override bool IsDue()
    {
      return PracticeState == ConstSpacedRepetition.CardPracticeState.Due;
    }

    public override bool IsDismissed()
    {
      return (MiscState & ConstSpacedRepetition.CardMiscStateFlag.Dismissed) ==
        ConstSpacedRepetition.CardMiscStateFlag.Dismissed;
    }

    public override bool IsSuspended()
    {
      return (MiscState & ConstSpacedRepetition.CardMiscStateFlag.Suspended) ==
        ConstSpacedRepetition.CardMiscStateFlag.Suspended;
    }
  }
}
