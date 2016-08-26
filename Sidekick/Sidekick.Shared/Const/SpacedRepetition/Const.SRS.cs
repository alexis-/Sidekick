using System;

namespace Sidekick.Shared.Const.SpacedRepetition
{
  public static class ConstSpacedRepetition
  {
    //
    // Card states

    public struct CardPracticeState
    {
      //
      // Attribute & Constructor
      private readonly short _value;

      private CardPracticeState(short state)
      {
        _value = state;
      }


      //
      // States

      public const short Deleted = -1,
                         Due = 0,
                         New = 1,
                         Learning = 2;


      //
      // Core methods

      public override bool Equals(object obj)
      {
        CardPracticeState otherObj = (CardPracticeState)obj;

        return otherObj._value == this._value;
      }

      public bool Equals(CardPracticeState other)
      {
        return _value == other._value;
      }

      public override int GetHashCode()
      {
        return _value.GetHashCode();
      }

      public static implicit operator short(CardPracticeState state)
      {
        return state._value;
      }

      public static implicit operator CardPracticeState(short state)
      {
        return new CardPracticeState(state);
      }
    }

    [Flags]
    public enum CardPracticeStateFilterFlag
    {
      New = 1,
      Learning = 2,
      Due = 4,

      All = New | Learning | Due
    }

    [Flags]
    public enum CardMiscStateFlag : short
    {
      None = 0,
      Suspended = 1,
      Dismissed = 2,
    }



    //
    // Answer grades

    public struct Grade
    {
      //
      // Attribute & Constructor
      private readonly int _value;

      private Grade(int grade)
      {
        _value = grade;
      }


      //
      // Grades

      public const int FailSevere = 0,
                       FailMedium = 1,
                       Fail = 2,
                       Hard = 3,
                       Good = 4,
                       Easy = 5;


      //
      // Core methods

      public override bool Equals(object obj)
      {
        Grade otherObj = (Grade)obj;

        return otherObj._value == this._value;
      }

      public bool Equals(Grade other)
      {
        return _value == other._value;
      }

      public override int GetHashCode()
      {
        return _value.GetHashCode();
      }

      public static implicit operator int(Grade grade)
      {
        return grade._value;
      }

      public static implicit operator Grade(int grade)
      {
        return new Grade(grade);
      }
    }

    public struct GradingInfo
    {
      public Grade Grade { get; set; }
      public string LocalizableText { get; set; }
      public DateTime NextReview { get; set; }
      public string[] CardValuesAftermath { get; set; }
    }



    //
    // Misc

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
  }
}
