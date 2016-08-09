using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mnemophile.Const.SRS
{
  public static class ConstSRS
  {
    public enum CardPracticeState : short
    {
      Deleted = -1,
      Due = 0,
      New = 1,
      Learning = 2,
    }

    [Flags]
    public enum CardPracticeStateFilterFlag
    {
      New = 1,
      Learning = 2,
      Due = 4,

      All = New | Learning | Due
    }

    public static IEnumerable<CardPracticeState> GetPracticeStates(
      this CardPracticeStateFilterFlag flag)
    {
      List<CardPracticeState> states = new List<CardPracticeState>();

      if ((flag & CardPracticeStateFilterFlag.New) ==
          CardPracticeStateFilterFlag.New)
        states.Add(CardPracticeState.New);
      if ((flag & CardPracticeStateFilterFlag.Learning) ==
          CardPracticeStateFilterFlag.Learning)
        states.Add(CardPracticeState.Learning);
      if ((flag & CardPracticeStateFilterFlag.Due) ==
          CardPracticeStateFilterFlag.Due)
        states.Add(CardPracticeState.Due);

      return states;
    }

    [Flags]
    public enum CardMiscStateFlag : short
    {
      None = 0,
      Suspended = 1,
      Dismissed = 2,
    }

    public enum Grade
    {
      FailSevere = 0,
      FailMedium = 1,
      Fail = 2,
      Hard = 3,
      Good = 4,
      Easy = 5,
    }

    public enum CardOrderingOption
    {
      Linear,
      Random,
    }

    public enum CardLeechAction
    {
      Suspend,
      Delete,
    }

    public struct GradingInfo
    {
      public Grade Grade { get; set; }
      public string Label { get; set; }
      public string Description { get; set; }
      public string[] CardValuesAftermath { get; set; }
    }
  }
}
