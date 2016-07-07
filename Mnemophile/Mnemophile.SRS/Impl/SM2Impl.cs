using Mnemophile.SRS.Models;
using Mnemophile.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mnemophile.Const.SRS;

namespace Mnemophile.SRS.Impl
{
  internal static class SM2Impl
  {
    public static void Answer(this Card card, int ease)
    {
      card.CurrentReviewTime = DateTime.Now.UnixTimestamp();

      // New card
      if (card.IsNew())
        card.UpdateLearningStep(true);

      // Handle card learning
      if (card.IsLearning())
      {

      }

      // Handle card review (graduated card)
      else
      {
        
      }

      // Update card properties
      card.Reviews++;
      card.LastModified = card.CurrentReviewTime;

      // TODO: Save
    }
  }
}
