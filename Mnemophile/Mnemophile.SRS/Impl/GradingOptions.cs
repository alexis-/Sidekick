using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mnemophile.Const.SRS;
using Mnemophile.SRS.Models;

namespace Mnemophile.SRS.Impl
{
  internal static class GradingOptions
  {
    internal static float GradeReviewEaseModifiers(ConstSRS.Grade grade,
      CollectionConfig config)
    {
      switch (grade)
      {
        case ConstSRS.Grade.FailSevere:
        case ConstSRS.Grade.FailMedium:
        case ConstSRS.Grade.Fail:
          return config.LapseEaseMalus;
        case ConstSRS.Grade.Hard:
          return config.ReviewHardEaseModifier;
        case ConstSRS.Grade.Good:
          return config.ReviewGoodEaseModifier;
        case ConstSRS.Grade.Easy:
          return config.ReviewEasyEaseModifier;
      }

      throw new ArgumentException("Invalid grade option", nameof(grade));
    }

    internal static Func<int, int, float, int> GradeReviewIntervalFormulas(
      ConstSRS.Grade grade, CollectionConfig config)
    {
      switch (grade)
      {
        case ConstSRS.Grade.Hard:
          return (lastInterval, delay, eFactor) => Math.Max(lastInterval + 1,
            (int)Math.Floor((lastInterval + delay * 0.25f) * 1.2f));
        case ConstSRS.Grade.Good:
          return (lastInterval, delay, eFactor) => Math.Max(lastInterval + 1,
            (int)Math.Floor((lastInterval + delay * 0.5f) * eFactor));
        case ConstSRS.Grade.Easy:
          return (lastInterval, delay, eFactor) => Math.Max(lastInterval + 1,
            (int)((lastInterval + delay) * eFactor * config.ReviewEasyBonus));
      }

      throw new ArgumentException("Invalid grade option", nameof(grade));
    }

    internal static ConstSRS.GradingInfo GradeFailSevere =>
      new ConstSRS.GradingInfo
    {
      Grade = ConstSRS.Grade.FailSevere,
      LocalizableText = "SpacedRepetition_Grade_FailSevere"
    };

    internal static ConstSRS.GradingInfo GradeFailMedium =>
      new ConstSRS.GradingInfo
    {
      Grade = ConstSRS.Grade.FailMedium,
      LocalizableText = "SpacedRepetition_Grade_FailMedium"
    };

    internal static ConstSRS.GradingInfo GradeFail =>
      new ConstSRS.GradingInfo
    {
      Grade = ConstSRS.Grade.Fail,
      LocalizableText = "SpacedRepetition_Grade_Fail"
    };

    internal static ConstSRS.GradingInfo GradeHard =>
      new ConstSRS.GradingInfo
    {
      Grade = ConstSRS.Grade.Hard,
      LocalizableText = "SpacedRepetition_Grade_Hard"
    };

    internal static ConstSRS.GradingInfo GradeGood =>
      new ConstSRS.GradingInfo
    {
      Grade = ConstSRS.Grade.Good,
      LocalizableText = "SpacedRepetition_Grade_Good"
    };

    internal static ConstSRS.GradingInfo GradeEasy =>
      new ConstSRS.GradingInfo
    {
      Grade = ConstSRS.Grade.Easy,
      LocalizableText = "SpacedRepetition_Grade_Easy"
    };
  }
}
