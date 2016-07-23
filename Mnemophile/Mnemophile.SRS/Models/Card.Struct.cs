using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Mnemophile.Const.SRS;

namespace Mnemophile.SRS.Models
{
  public partial class Card
  {
    /// <summary>
    /// Bitwise mask to retain misc states (Suspended, Dismissed) while
    /// updating main state (New, Learning, Due).
    /// </summary>
    //const ConstSRS.CardStateFlag CardMiscStateMask =
    //  (ConstSRS.CardStateFlag)(int.MaxValue
    //                   ^ (int)ConstSRS.CardStateFlag.New
    //                   ^ (int)ConstSRS.CardStateFlag.Learning
    //                   ^ (int)ConstSRS.CardStateFlag.Due);

    /// <summary>
    /// Bitwise mask to retain main state (New, Learning, Due) while updating
    /// misc states (Suspended, Dismissed).
    /// </summary>
    //const ConstSRS.CardStateFlag CardMainStateMask = ~CardMiscStateMask;

    internal enum CardAction
    {
      Invalid = -1,
      Update,
      Delete
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
