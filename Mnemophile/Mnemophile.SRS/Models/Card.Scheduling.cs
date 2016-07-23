using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mnemophile.Const.SRS;

namespace Mnemophile.SRS.Models
{
  partial class Card
  {
    public int ReviewLeftToday()
    {
      switch (PracticeState)
      {
        case ConstSRS.CardPracticeState.New:
          return LearningOrLapsingSteps.Length;
          
        case ConstSRS.CardPracticeState.Learning:
          return GetLearningStepsLeft();

        case ConstSRS.CardPracticeState.Due:
          return 1;
      }

      throw new InvalidOperationException("Invalid card state");
    }
  }
}
