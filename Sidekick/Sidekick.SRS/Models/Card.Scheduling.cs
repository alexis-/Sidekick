using System;
using Sidekick.Shared.Const.SpacedRepetition;
using Sidekick.Shared.Utils;

namespace Sidekick.SpacedRepetition.Models
{
  partial class Card
  {
    public int ReviewLeftToday()
    {
      if (Due >= DateTime.Today.AddDays(1).ToUnixTimestamp())
        return 0;

      switch (PracticeState)
      {
        case ConstSpacedRepetition.CardPracticeState.New:
          return 1;

        case ConstSpacedRepetition.CardPracticeState.Due:
          return 1;

        case ConstSpacedRepetition.CardPracticeState.Deleted: // Leech option
          return 0;
      }

      if (IsLearning())
        return GetLearningStepsLeft();

      throw new InvalidOperationException("Invalid card state");
    }
  }
}
