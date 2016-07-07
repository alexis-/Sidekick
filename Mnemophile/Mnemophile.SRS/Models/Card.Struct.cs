using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Mnemophile.SRS.Models
{
  public partial class Card
  {
    /// <summary>
    /// Bitwise mask to retain misc states (Suspended, Buried) while updating
    /// main state (New, Learning, Due).
    /// </summary>
    const CardStateFlag CardMiscStateMask =
      (CardStateFlag)(int.MaxValue
                       ^ (int)CardStateFlag.New
                       ^ (int)CardStateFlag.Learning
                       ^ (int)CardStateFlag.Due);

    /// <summary>
    /// Bitflags to describe card state. New, Learning and Due are mutually
    /// exclusive ; but may be combined with Suspended or Buried states.
    /// </summary>
    [Flags]
    public enum CardStateFlag
    {
      New = 0,
      Learning = 1,
      Due = 2,
      Suspended = 4,
      Buried = 8,
    }

    //[StructLayout(LayoutKind.Explicit)]
    //internal struct DueUnionStruct
    //{
    //  [FieldOffset(0)]
    //  public int Due;
    //  [FieldOffset(0)]
    //  public short LearnDelay;
    //  [FieldOffset(4)]
    //  public short LearnStep;
    //}
  }
}
