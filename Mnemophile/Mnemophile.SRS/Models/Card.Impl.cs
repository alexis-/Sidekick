using Mnemophile.Utils;
using System;
using Mnemophile.Const.SpacedRepetition;
using Mnemophile.Interfaces.DB;
using Mnemophile.SpacedRepetition.Impl;

namespace Mnemophile.SpacedRepetition.Models
{
  public partial class Card
  {
    public void Dismiss(IDatabase db)
    {
      MiscState = MiscState | ConstSpacedRepetition.CardMiscStateFlag.Dismissed;

      Due = Math.Max(Due, DateTime.Today.AddDays(1).ToUnixTimestamp());
      // TODO: If card is overdue, interval bonus is lost
      
      using (db.Lock())
        db.Update(this);
    }

    public void Answer(ConstSpacedRepetition.Grade grade)
    {
      Answer(grade, null);
    }

    public void Answer(ConstSpacedRepetition.Grade grade, IDatabase db)
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
          case ConstSpacedRepetition.Grade.FailSevere:
          case ConstSpacedRepetition.Grade.FailMedium:
          case ConstSpacedRepetition.Grade.Fail:
            // TODO: If relearning, further decrease ease and interval ?
            cardAction = UpdateLearningStep(true);
            break;

          case ConstSpacedRepetition.Grade.Hard:
          case ConstSpacedRepetition.Grade.Good:
            cardAction = UpdateLearningStep();
            break;

          case ConstSpacedRepetition.Grade.Easy:
            cardAction = Graduate(true);
            break;
        }

      // Handle card review (graduated card)
      else
        switch (grade)
        {
          case ConstSpacedRepetition.Grade.FailSevere:
          case ConstSpacedRepetition.Grade.FailMedium:
          case ConstSpacedRepetition.Grade.Fail:
            cardAction = Lapse(grade);
            break;

          case ConstSpacedRepetition.Grade.Hard:
          case ConstSpacedRepetition.Grade.Good:
          case ConstSpacedRepetition.Grade.Easy:
            cardAction = Review(grade);
            break;
        }

      // Update card properties
      MiscState = ConstSpacedRepetition.CardMiscStateFlag.None;
      Reviews++;
      LastModified = CurrentReviewTime;



      if (db != null)
        switch (cardAction)
        {
          case CardAction.Delete:
            PracticeState = ConstSpacedRepetition.CardPracticeState.Deleted;

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
    public override ConstSpacedRepetition.GradingInfo[] ComputeGrades()
    {
      int currentReviewTime = DateTime.Now.UnixTimestamp();
      ConstSpacedRepetition.GradingInfo[] gradingInfos;

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
      for (int i = 0; i < gradingInfos.Length; i++)
      {
        ConstSpacedRepetition.GradingInfo gradingInfo = gradingInfos[i];
        Card cardClone = Clone();

        cardClone.CurrentReviewTime = currentReviewTime;
        cardClone.Answer(gradingInfo.Grade, null);

        gradingInfo.NextReview = DateTimeEx.FromUnixTimestamp(cardClone.Due);
        // TODO: Set gradingInfo.CardValueAftermath
      }

      return gradingInfos;
    }
  }
}
