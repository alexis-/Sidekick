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

namespace Sidekick.SpacedRepetition.Extensions
{
  using System;
  using System.Threading.Tasks;

  using AgnosticDatabase.Interfaces;

  using Sidekick.Shared.Extensions;
  using Sidekick.SpacedRepetition.Models;

  /// <summary>Card review-actions-related extension methods.</summary>
  public static class CardActionExtensions
  {
    #region Methods

    /// <summary>Dismisses card and postpone review to next day.</summary>
    /// <param name="card">Card instance.</param>
    public static CardAction Dismiss(this Card card)
    {
      card.MiscState = card.MiscState | CardMiscStateFlag.Dismissed;

      // TODO: If card is overdue, interval bonus is lost
      card.Due = Math.Max(card.Due, DateTimeExtensions.Tomorrow.ToUnixTimestamp());

      return CardAction.Dismiss;
    }

    /// <summary>
    ///   Answers current card with specified grade and calculate outcomes. No modification
    ///   saved.
    /// </summary>
    /// <param name="card">Card instance.</param>
    /// <param name="grade">The grade.</param>
    public static CardAction Answer(this Card card, Grade grade)
    {
      CardAction cardAction = CardAction.Invalid;
      card.CurrentReviewTime = DateTime.Now.UnixTimestamp();

      // New card
      if (card.IsNew())
        card.UpdateLearningStep(true);

      // Handle card learning
      if (card.IsLearning())
        switch (grade)
        {
          case Grade.FailSevere:
          case Grade.FailMedium:
          case Grade.Fail:
            // TODO: If relearning, further decrease ease and interval ?
            cardAction = card.UpdateLearningStep(true);
            break;

          case Grade.Hard:
          case Grade.Good:
            cardAction = card.UpdateLearningStep();
            break;

          case Grade.Easy:
            cardAction = card.Graduate(true);
            break;
        }

      // Handle card review (graduated card)
      else
        switch (grade)
        {
          case Grade.FailSevere:
          case Grade.FailMedium:
          case Grade.Fail:
            cardAction = card.Lapse(grade);
            break;

          case Grade.Hard:
          case Grade.Good:
          case Grade.Easy:
            cardAction = card.Review(grade);
            break;
        }

      // Update card properties
      card.MiscState = CardMiscStateFlag.None;
      card.Reviews++;
      card.LastModified = card.CurrentReviewTime;

      if (cardAction == CardAction.Delete)
        card.PracticeState = PracticeState.Deleted;

      return cardAction;
    }

    /// <summary>Dismisses the asynchronous.</summary>
    /// <param name="card">The card.</param>
    /// <param name="db">Database instance.</param>
    /// <returns></returns>
    public static async Task DismissAsync(this Card card, IDatabaseAsync db)
    {
      card.Dismiss();

      await db.UpdateAsync(card).ConfigureAwait(false);
    }

    /// <summary>
    ///   Answers current card with specified grade and calculate outcomes. Modifications are
    ///   saved to database.
    /// </summary>
    /// <param name="card">Card instance</param>
    /// <param name="grade">The grade.</param>
    /// <param name="db">Database instance</param>
    public static async Task AnswerAsync(this Card card, Grade grade, IDatabaseAsync db)
    {
      CardAction cardAction = card.Answer(grade);

      if (db != null)
        switch (cardAction)
        {
          case CardAction.Delete:
            await db.DeleteAsync<Card>(card.Id).ConfigureAwait(false);
            break;

          case CardAction.Update:
            await db.UpdateAsync(card).ConfigureAwait(false);
            break;

          default:
            throw new InvalidOperationException(
              "Card.Answer ended up in an invalid Card Action state.");
        }
    }

    /// <summary>Save card modifications to database according to CardAction.</summary>
    /// <param name="card">Card instance</param>
    /// <param name="cardAction">The resulting card action.</param>
    /// <param name="db">Database instance</param>
    public static async Task AnswerAsync(
      this Card card, CardAction cardAction, IDatabaseAsync db)
    {
      if (db != null)
        switch (cardAction)
        {
          case CardAction.Delete:
            await db.DeleteAsync<Card>(card.Id).ConfigureAwait(false);
            break;

          case CardAction.Update:
            await db.UpdateAsync(card).ConfigureAwait(false);
            break;

          default:
            throw new InvalidOperationException(
              "Card.Answer ended up in an invalid Card Action state.");
        }
    }

    /// <summary>
    ///   Computes all grading options for given card, according to its State. Card values are
    ///   unaffected.
    /// </summary>
    /// <param name="card">Card instance.</param>
    /// <returns>Description of all grading options outcomes</returns>
    public static ReviewAnswerInfo[] ComputeGrades(this Card card)
    {
      int currentReviewTime = DateTime.Now.UnixTimestamp();
      ReviewAnswerInfo[] reviewAnswerInfos;

      // Learning grades
      if (card.IsNew() || card.IsLearning())
        reviewAnswerInfos = new[] { ReviewAnswerInfo.Fail, ReviewAnswerInfo.Good, ReviewAnswerInfo.Easy };

      // Due grades
      else
        reviewAnswerInfos = new[]
          {
              ReviewAnswerInfo.Fail, ReviewAnswerInfo.Hard, ReviewAnswerInfo.Good, ReviewAnswerInfo.Easy 
          };

      // Compute outcomes of each grade option
      for (int i = 0; i < reviewAnswerInfos.Length; i++)
      {
        ReviewAnswerInfo reviewAnswerInfo = reviewAnswerInfos[i];
        Card cardClone = card.Clone();

        cardClone.CurrentReviewTime = currentReviewTime;
        cardClone.Answer(reviewAnswerInfo.Grade);

        reviewAnswerInfo.NextReview = cardClone.DueDateTime;
        //// TODO: Set ReviewAnswerInfo.CardValueAftermath
      }

      return reviewAnswerInfos;
    }

    #endregion
  }
}