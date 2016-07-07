using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mnemophile.Const.SRS
{
  public static class ConstSRS
  {
    public enum Grade
    {
      FailSevere = 0,
      FailMedium = 1,
      Fail = 2,
      Hard = 3,
      Good = 4,
      Easy = 5,
    }

    public enum CardInsertionOption
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
