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
            (int)((lastInterval + delay * 0.25f) * 1.2f));
        case ConstSRS.Grade.Good:
          return (lastInterval, delay, eFactor) => Math.Max(lastInterval + 1,
            (int)((lastInterval + delay * 0.5f) * eFactor));
        case ConstSRS.Grade.Easy:
          return (lastInterval, delay, eFactor) => Math.Max(lastInterval + 1,
            (int)((lastInterval + delay) * eFactor * config.ReviewEasyBonus));
      }

      throw new ArgumentException("Invalid grade option", nameof(grade));
    }

    internal static ConstSRS.GradingInfo GradeFailSevere =
      new ConstSRS.GradingInfo
    {
      Grade = ConstSRS.Grade.FailSevere,
      Label = "Fail (Severe)",
      Description = ""
    };

    internal static ConstSRS.GradingInfo GradeFailMedium =
      new ConstSRS.GradingInfo
    {
      Grade = ConstSRS.Grade.FailMedium,
      Label = "Fail (Medium)",
      Description = ""
    };

    internal static ConstSRS.GradingInfo GradeFail =
      new ConstSRS.GradingInfo
    {
      Grade = ConstSRS.Grade.Fail,
      Label = "Fail",
      Description = ""
    };

    internal static ConstSRS.GradingInfo GradeHard =
      new ConstSRS.GradingInfo
    {
      Grade = ConstSRS.Grade.Hard,
      Label = "Hard",
      Description = ""
    };

    internal static ConstSRS.GradingInfo GradeGood =
      new ConstSRS.GradingInfo
    {
      Grade = ConstSRS.Grade.Good,
      Label = "Good",
      Description = ""
    };

    internal static ConstSRS.GradingInfo GradeEasy =
      new ConstSRS.GradingInfo
    {
      Grade = ConstSRS.Grade.Easy,
      Label = "Easy",
      Description = ""
    };
  }
}
