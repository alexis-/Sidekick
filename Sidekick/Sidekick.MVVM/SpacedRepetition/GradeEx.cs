using System;
using Sidekick.Shared.Const.SpacedRepetition;
using Xamarin.Forms;

namespace Sidekick.MVVM.SpacedRepetition
{
  public static class GradeEx
  {
    public static Color GetColor(this ConstSpacedRepetition.Grade grade)
    {
      switch (grade)
      {
        case ConstSpacedRepetition.Grade.FailSevere:
          return Color.Black;

        case ConstSpacedRepetition.Grade.FailMedium:
          return Color.Black;

        case ConstSpacedRepetition.Grade.Fail:
          return Color.FromHex("#FFDC143C"); // Crimson

        case ConstSpacedRepetition.Grade.Hard:
          return Color.FromHex("#FF9ACD32"); // YellowGreen

        case ConstSpacedRepetition.Grade.Good:
          return Color.FromHex("#FF1E90FF"); // DodgerBlue

        case ConstSpacedRepetition.Grade.Easy:
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
