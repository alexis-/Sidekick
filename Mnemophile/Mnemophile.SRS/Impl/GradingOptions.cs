using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mnemophile.Const.SpacedRepetition;
using Mnemophile.SpacedRepetition.Models;

namespace Mnemophile.SpacedRepetition.Impl
{
  internal static class GradingOptions
  {
    internal static float GradeReviewEaseModifiers(ConstSpacedRepetition.Grade grade,
      CollectionConfig config)
    {
      switch (grade)
      {
        case ConstSpacedRepetition.Grade.FailSevere:
        case ConstSpacedRepetition.Grade.FailMedium:
        case ConstSpacedRepetition.Grade.Fail:
          return config.LapseEaseMalus;
        case ConstSpacedRepetition.Grade.Hard:
          return config.ReviewHardEaseModifier;
        case ConstSpacedRepetition.Grade.Good:
          return config.ReviewGoodEaseModifier;
        case ConstSpacedRepetition.Grade.Easy:
          return config.ReviewEasyEaseModifier;
      }

      throw new ArgumentException("Invalid grade option", nameof(grade));
    }

    internal static Func<int, int, float, int> GradeReviewIntervalFormulas(
      ConstSpacedRepetition.Grade grade, CollectionConfig config)
    {
      switch (grade)
      {
        case ConstSpacedRepetition.Grade.Hard:
          return (lastInterval, delay, eFactor) => Math.Max(lastInterval + 1,
            (int)Math.Floor((lastInterval + delay * 0.25f) * 1.2f));
        case ConstSpacedRepetition.Grade.Good:
          return (lastInterval, delay, eFactor) => Math.Max(lastInterval + 1,
            (int)Math.Floor((lastInterval + delay * 0.5f) * eFactor));
        case ConstSpacedRepetition.Grade.Easy:
          return (lastInterval, delay, eFactor) => Math.Max(lastInterval + 1,
            (int)((lastInterval + delay) * eFactor * config.ReviewEasyBonus));
      }

      throw new ArgumentException("Invalid grade option", nameof(grade));
    }

    internal static ConstSpacedRepetition.GradingInfo GradeFailSevere =>
      new ConstSpacedRepetition.GradingInfo
    {
      Grade = ConstSpacedRepetition.Grade.FailSevere,
      LocalizableText = "SpacedRepetition_Grade_FailSevere"
    };

    internal static ConstSpacedRepetition.GradingInfo GradeFailMedium =>
      new ConstSpacedRepetition.GradingInfo
    {
      Grade = ConstSpacedRepetition.Grade.FailMedium,
      LocalizableText = "SpacedRepetition_Grade_FailMedium"
    };

    internal static ConstSpacedRepetition.GradingInfo GradeFail =>
      new ConstSpacedRepetition.GradingInfo
    {
      Grade = ConstSpacedRepetition.Grade.Fail,
      LocalizableText = "SpacedRepetition_Grade_Fail"
    };

    internal static ConstSpacedRepetition.GradingInfo GradeHard =>
      new ConstSpacedRepetition.GradingInfo
    {
      Grade = ConstSpacedRepetition.Grade.Hard,
      LocalizableText = "SpacedRepetition_Grade_Hard"
    };

    internal static ConstSpacedRepetition.GradingInfo GradeGood =>
      new ConstSpacedRepetition.GradingInfo
    {
      Grade = ConstSpacedRepetition.Grade.Good,
      LocalizableText = "SpacedRepetition_Grade_Good"
    };

    internal static ConstSpacedRepetition.GradingInfo GradeEasy =>
      new ConstSpacedRepetition.GradingInfo
    {
      Grade = ConstSpacedRepetition.Grade.Easy,
      LocalizableText = "SpacedRepetition_Grade_Easy"
    };
  }
}
