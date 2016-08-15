using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mnemophile.Const.SRS;
using Mnemophile.Utils;

namespace Mnemophile.SRS.Models
{
  partial class Card
  {
    public int ReviewLeftToday()
    {
      if (Due >= DateTime.Today.AddDays(1).ToUnixTimestamp())
        return 0;

      switch (PracticeState)
      {
        case ConstSRS.CardPracticeState.New:
          return 1;

        case ConstSRS.CardPracticeState.Due:
          return 1;

        case ConstSRS.CardPracticeState.Deleted: // Leech option
          return 0;
      }

      if (IsLearning())
        return GetLearningStepsLeft();

      throw new InvalidOperationException("Invalid card state");
    }
  }
}
