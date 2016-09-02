using System;
using Sidekick.SpacedRepetition;
using Sidekick.SpacedRepetition.Const;
using Xamarin.Forms;

namespace Sidekick.MVVM.Extensions.SpacedRepetition
{
  public static class GradeExtensions
  {
    public static Color GetColor(this Grade grade)
    {
      switch (grade)
      {
        case Grade.FailSevere:
          return Color.Black;

        case Grade.FailMedium:
          return Color.Black;

        case Grade.Fail:
          return Color.FromHex("#FFDC143C"); // Crimson

        case Grade.Hard:
          return Color.FromHex("#FF9ACD32"); // YellowGreen

        case Grade.Good:
          return Color.FromHex("#FF1E90FF"); // DodgerBlue

        case Grade.Easy:
          return Color.FromHex("FFA500"); // Orange
      }

      throw new InvalidOperationException("Invalid grade");
    }

    //public static string GetLocalizableText(this ConstSpacedRepetition.Grade grade)
    //{
    //  switch (grade)
    //  {
    //    case ConstSpacedRepetition.Grade.FailSevere:
    //      return "SpacedRepetition_Grade_FailSevere";

    //    case ConstSpacedRepetition.Grade.FailMedium:
    //      return "SpacedRepetition_Grade_FailMedium";

    //    case ConstSpacedRepetition.Grade.Fail:
    //      return "SpacedRepetition_Grade_Fail";

    //    case ConstSpacedRepetition.Grade.Hard:
    //      return "SpacedRepetition_Grade_Hard";

    //    case ConstSpacedRepetition.Grade.Good:
    //      return "SpacedRepetition_Grade_Good";

    //    case ConstSpacedRepetition.Grade.Easy:
    //      return "SpacedRepetition_Grade_Easy";
    //  }

    //  throw new InvalidOperationException("Invalid grade");
    //}
  }
}
