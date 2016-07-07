using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mnemophile.Const.SRS
{
  public static class ConstSRS
  {
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
  }
}
