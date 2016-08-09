using Mnemophile.Utils;
using System;
using Mnemophile.Const.SRS;
using Mnemophile.Interfaces.DB;
using Mnemophile.SRS.Impl;

namespace Mnemophile.SRS.Models
{
  public partial class Card
  {
    internal void Dismiss(IDatabase db)
    {
      MiscState = MiscState | ConstSRS.CardMiscStateFlag.Dismissed;

      Due = Math.Max(Due, DateTime.Today.AddDays(1).ToUnixTimestamp());
      // TODO: If card is overdue, interval bonus is lost
      
      using (db.Lock())
        db.Update(this);
    }

    internal void Answer(ConstSRS.Grade grade)
    {
      Answer(grade, null);
    }

    internal void Answer(ConstSRS.Grade grade, IDatabase db)
    {
      CardAction cardAction = CardAction.Invalid;
      CurrentReviewTime = DateTime.Now.UnixTimestamp();

      // New card
      if (IsNew())
        UpdateLearningStep(true);

      // Handle card learning
      if (IsLearning())
        switch (grade)
        {
          case ConstSRS.Grade.FailSevere:
          case ConstSRS.Grade.FailMedium:
          case ConstSRS.Grade.Fail:
            // TODO: If relearning, further decrease ease and interval ?
            cardAction = UpdateLearningStep(true);
            break;

          case ConstSRS.Grade.Hard:
          case ConstSRS.Grade.Good:
            cardAction = UpdateLearningStep();
            break;

          case ConstSRS.Grade.Easy:
            cardAction = Graduate(true);
            break;
        }

      // Handle card review (graduated card)
      else
        switch (grade)
        {
          case ConstSRS.Grade.FailSevere:
          case ConstSRS.Grade.FailMedium:
          case ConstSRS.Grade.Fail:
            cardAction = Lapse(grade);
            break;

          case ConstSRS.Grade.Hard:
          case ConstSRS.Grade.Good:
          case ConstSRS.Grade.Easy:
            cardAction = Review(grade);
            break;
        }

      // Update card properties
      MiscState = ConstSRS.CardMiscStateFlag.None;
      Reviews++;
      LastModified = CurrentReviewTime;

      if (db != null)
        switch (cardAction)
        {
          case CardAction.Delete:
            PracticeState = ConstSRS.CardPracticeState.Deleted;

            using (db.Lock())
              db.Delete<Card>(Id);

            break;

          case CardAction.Update:
            using (db.Lock())
              db.Update(this);

            break;

          default:
            throw new InvalidOperationException(
              "Card.Answer ended up in an invalid Card Action state.");
        }
    }

    /// <summary>
    /// Computes all grading options for given card, according to its State.
    /// Card values are unaffected.
    /// </summary>
    /// <returns>Description of all grading options outcomes</returns>
    public ConstSRS.GradingInfo[] ComputeGrades()
    {
      int currentReviewTime = DateTime.Now.UnixTimestamp();
      ConstSRS.GradingInfo[] gradingInfos;

      // Learning grades
      if (IsNew() || IsLearning())
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
        Card cardClone = Clone();

        cardClone.CurrentReviewTime = currentReviewTime;
        cardClone.Answer(gradingInfo.Grade, null);

        // TODO: Set gradingInfo.CardValueAftermath
      }

      return gradingInfos;
    }
  }
}
