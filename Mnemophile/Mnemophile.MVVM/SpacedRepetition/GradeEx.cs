using System;
using Mnemophile.Const.SRS;
using Xamarin.Forms;

namespace Mnemophile.MVVM.SpacedRepetition
{
  public static class GradeEx
  {
    public static Color GetColor(this ConstSRS.Grade grade)
    {
      switch (grade)
      {
        case ConstSRS.Grade.FailSevere:
          return Color.Black;

        case ConstSRS.Grade.FailMedium:
          return Color.Black;

        case ConstSRS.Grade.Fail:
          return Color.FromHex("#FFDC143C"); // Crimson

        case ConstSRS.Grade.Hard:
          return Color.FromHex("#FF9ACD32"); // YellowGreen

        case ConstSRS.Grade.Good:
          return Color.FromHex("#FF1E90FF"); // DodgerBlue

        case ConstSRS.Grade.Easy:
          return Color.FromHex("FFA500"); // Orange
      }

      throw new InvalidOperationException("Invalid grade");
    }

    //public static string GetLocalizableText(this ConstSRS.Grade grade)
    //{
    //  switch (grade)
    //  {
    //    case ConstSRS.Grade.FailSevere:
    //      return "SpacedRepetition_Grade_FailSevere";

    //    case ConstSRS.Grade.FailMedium:
    //      return "SpacedRepetition_Grade_FailMedium";

    //    case ConstSRS.Grade.Fail:
    //      return "SpacedRepetition_Grade_Fail";

    //    case ConstSRS.Grade.Hard:
    //      return "SpacedRepetition_Grade_Hard";

    //    case ConstSRS.Grade.Good:
    //      return "SpacedRepetition_Grade_Good";

    //    case ConstSRS.Grade.Easy:
    //      return "SpacedRepetition_Grade_Easy";
    //  }

    //  throw new InvalidOperationException("Invalid grade");
    //}
  }
}
