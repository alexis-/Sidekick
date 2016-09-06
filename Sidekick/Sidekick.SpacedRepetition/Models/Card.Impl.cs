// 
// The MIT License (MIT)
// 
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the 
// Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using Sidekick.Shared.Extensions;
using Sidekick.Shared.Interfaces.Database;
using Sidekick.SpacedRepetition.Const;

namespace Sidekick.SpacedRepetition.Models
{
  public partial class Card
  {
    #region Methods

    /// <summary>
    /// Dismisses card and postpone review to next day.
    /// </summary>
    /// <param name="db">The database.</param>
    public void Dismiss(IDatabase db)
    {
      MiscState = MiscState | CardMiscStateFlag.Dismissed;

      Due = Math.Max(Due, DateTimeExtensions.Tomorrow.ToUnixTimestamp());
      // TODO: If card is overdue, interval bonus is lost

      using (db.Lock())
        db.Update(this);
    }

    /// <summary>
    /// Answers current card with specified grade and calculate outcomes.
    /// No modification saved.
    /// </summary>
    /// <param name="grade">The grade.</param>
    public void Answer(Grade grade)
    {
      Answer(grade, null);
    }

    /// <summary>
    /// Answers current card with specified grade and calculate outcomes.
    /// Modifications are saved to database.
    /// </summary>
    /// <param name="grade">The grade.</param>
    /// <param name="db">Database instance</param>
    public void Answer(Grade grade, IDatabase db)
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
          case Grade.FailSevere:
          case Grade.FailMedium:
          case Grade.Fail:
            // TODO: If relearning, further decrease ease and interval ?
            cardAction = UpdateLearningStep(true);
            break;

          case Grade.Hard:
          case Grade.Good:
            cardAction = UpdateLearningStep();
            break;

          case Grade.Easy:
            cardAction = Graduate(true);
            break;
        }

      // Handle card review (graduated card)
      else
        switch (grade)
        {
          case Grade.FailSevere:
          case Grade.FailMedium:
          case Grade.Fail:
            cardAction = Lapse(grade);
            break;

          case Grade.Hard:
          case Grade.Good:
          case Grade.Easy:
            cardAction = Review(grade);
            break;
        }

      // Update card properties
      MiscState = CardMiscStateFlag.None;
      Reviews++;
      LastModified = CurrentReviewTime;


      if (db != null)
        switch (cardAction)
        {
          case CardAction.Delete:
            PracticeState = CardPracticeState.Deleted;

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
    public GradeInfo[] ComputeGrades()
    {
      int currentReviewTime = DateTime.Now.UnixTimestamp();
      GradeInfo[] gradeInfos;

      // Learning grades
      if (IsNew() || IsLearning())
        gradeInfos = new[]
        {
          GradeInfo.GradeFail,
          GradeInfo.GradeGood, GradeInfo.GradeEasy
        };

      // Due grades
      else
        gradeInfos = new[]
        {
          GradeInfo.GradeFail,
          GradeInfo.GradeHard, GradeInfo.GradeGood,
          GradeInfo.GradeEasy
        };

      // Compute outcomes of each grade option
      for (int i = 0; i < gradeInfos.Length; i++)
      {
        GradeInfo gradeInfo = gradeInfos[i];
        Card cardClone = Clone();

        cardClone.CurrentReviewTime = currentReviewTime;
        cardClone.Answer(gradeInfo.Grade, null);

        gradeInfo.NextReview = cardClone.DueDateTime;
        // TODO: Set GradeInfo.CardValueAftermath
      }

      return gradeInfos;
    }

    #endregion
  }
}