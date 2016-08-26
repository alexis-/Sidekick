namespace Sidekick.SpacedRepetition.Models
{
  public partial class Card
  {
    /// <summary>
    /// Bitwise mask to retain misc states (Suspended, Dismissed) while
    /// updating main state (New, Learning, Due).
    /// </summary>
    //const ConstSpacedRepetition.CardStateFlag CardMiscStateMask =
    //  (ConstSpacedRepetition.CardStateFlag)(int.MaxValue
    //                   ^ (int)ConstSpacedRepetition.CardStateFlag.New
    //                   ^ (int)ConstSpacedRepetition.CardStateFlag.Learning
    //                   ^ (int)ConstSpacedRepetition.CardStateFlag.Due);

    /// <summary>
    /// Bitwise mask to retain main state (New, Learning, Due) while updating
    /// misc states (Suspended, Dismissed).
    /// </summary>
    //const ConstSpacedRepetition.CardStateFlag CardMainStateMask = ~CardMiscStateMask;

    public enum CardAction
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
