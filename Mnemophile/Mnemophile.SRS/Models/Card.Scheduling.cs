using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mnemophile.Const.SpacedRepetition;
using Mnemophile.Utils;

namespace Mnemophile.SpacedRepetition.Models
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
