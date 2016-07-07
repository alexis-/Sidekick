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
    public static void Answer(this Card card, ConstSRS.Grade grade,
      bool save = true)
    {
      card.CurrentReviewTime = DateTime.Now.UnixTimestamp();

      // New card
      if (card.IsNew())
        card.UpdateLearningStep(true);

      // Handle card learning
      if (card.IsLearning())
        switch (grade)
        {
          case ConstSRS.Grade.FailSevere:
          case ConstSRS.Grade.FailMedium:
          case ConstSRS.Grade.Fail:
            // TODO: If relearning, further decrease ease and interval ?
            card.UpdateLearningStep(true);
            break;

          case ConstSRS.Grade.Hard:
          case ConstSRS.Grade.Good:
            card.UpdateLearningStep();
            break;

          case ConstSRS.Grade.Easy:
            card.Graduate(true);
            break;
        }

      // Handle card review (graduated card)
      else
        switch (grade)
        {
          case ConstSRS.Grade.FailSevere:
          case ConstSRS.Grade.FailMedium:
          case ConstSRS.Grade.Fail:
            card.Lapse(grade);
            break;

          case ConstSRS.Grade.Hard:
          case ConstSRS.Grade.Good:
          case ConstSRS.Grade.Easy:
            card.Review(grade);
            break;
        }

      // Update card properties
      card.Reviews++;
      card.LastModified = card.CurrentReviewTime;

      if (save)
        ; // TODO: Save
    }

    /// <summary>
    /// Computes all grading options for given card, according to its State.
    /// Card values are unaffected.
    /// </summary>
    /// <param name="card">Card instance</param>
    /// <returns>Description of all grading options outcomes</returns>
    public static ConstSRS.GradingInfo[] ComputeGrades(this Card card)
    {
      int currentReviewTime = DateTime.Now.UnixTimestamp();
      ConstSRS.GradingInfo[] gradingInfos;

      // Learning grades
      if (card.IsNew() || card.IsLearning())
        gradingInfos = new[] { GradingOptions.GradeFail,
          GradingOptions.GradeGood, GradingOptions.GradeEasy };

      // Due grades
      else
        gradingInfos = new[] { GradingOptions.GradeFail,
          GradingOptions.GradeHard, GradingOptions.GradeGood,
          GradingOptions.GradeEasy };

      // Compute outcomes of each grade option
      foreach (var gradingInfo in gradingInfos)
      {
        Card cardClone = card.Clone();

        cardClone.CurrentReviewTime = currentReviewTime;
        cardClone.Answer(gradingInfo.Grade, false);

        // TODO: Set gradingInfo.CardValueAftermath
      }

      return gradingInfos;
    }
  }
}
