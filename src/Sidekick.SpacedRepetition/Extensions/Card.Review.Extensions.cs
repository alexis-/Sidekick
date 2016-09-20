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

  using Sidekick.Shared.Extensions;
  using Sidekick.Shared.Utils;
  using Sidekick.SpacedRepetition.Models;

  /// <summary>Card review-computations-related extension methods.</summary>
  public static class CardReviewExtensions
  {
    #region Learning

    /// <summary>Graduate from learning into due state.</summary>
    /// <param name="card">Card instance.</param>
    /// <param name="easy">whether answer was graded 'easy'</param>
    public static CardAction Graduate(this Card card, bool easy = false)
    {
      if (card.IsDue())
        throw new InvalidOperationException("Card.Graduate invoked with card 'Due' state");

      // Regraduating
      if (card.Lapses > 0)
        card.Interval = Math.Max(
          card.Config.LapseMinInterval, (int)(card.Config.LapseIntervalFactor * card.Interval));

      // Graduating for the first time
      else
      {
        card.Interval = easy
                          ? card.Config.GraduationEasyInterval
                          : card.Config.GraduationInterval;
        card.EFactor = card.Config.GraduationStartingEase;
      }

      card.SetDueFromInterval();
      card.SetDueState();

      return CardAction.Update;
    }

    /// <summary>
    ///   Either setup lapsing/learning steps (if reset is set to true), move on to next step,
    ///   or graduate.
    /// </summary>
    /// <param name="card">Card instance.</param>
    /// <param name="reset">if true, set learning state and lapsing or learning first step.</param>
    public static CardAction UpdateLearningStep(this Card card, bool reset = false)
    {
      // Set learning mode and first step delay
      if (reset)
      {
        card.SetDue(card.LearningOrLapsingSteps[0]);
        card.SetLearningState();
      }

      else if (card.IsDue())
        throw new InvalidOperationException(
          "Card.UpdateLearningStep invoked with " + nameof(reset)
          + " parameter 'false' and card 'Due' state");

      // Graduate
      else if (card.IsGraduating())
        card.Graduate();

      // Move on to next step
      else
        card.SetDue(card.LearningOrLapsingSteps[card.IncreaseLearningStep()]);

      return CardAction.Update;
    }

    /// <summary>Determines whether current lapsing or learning step is graduating.</summary>
    /// <param name="card">Card instance.</param>
    /// <returns>Whether current step is graduating</returns>
    public static bool IsGraduating(this Card card)
    {
      if (card.IsDue())
        return false;

      return card.GetCurrentLearningIndex() >= card.LearningOrLapsingSteps.Length - 1;
    }

    /// <summary>
    ///   Increment learning step index, and return new value. Accounts for PracticeState
    ///   offset.
    /// </summary>
    /// <param name="card">Card instance.</param>
    /// <returns>New step index</returns>
    public static int IncreaseLearningStep(this Card card)
    {
      card.PracticeState++;

      return card.GetCurrentLearningIndex();
    }

    /// <summary>
    ///   Get current lapsing or learning step index. In the occurence steps settings changed,
    ///   closest inferior value index is returned. Accounts for PracticeState offset.
    /// </summary>
    /// <param name="card">Card instance.</param>
    /// <returns>Current lapsing or learning index</returns>
    public static int GetCurrentLearningIndex(this Card card)
    {
      if (!card.IsLearning())
        throw new InvalidOperationException("Invalid call for State " + card.PracticeState);

      // Account for possible config changes
      return Math.Min(
        card.PracticeState - PracticeState.Learning, card.LearningOrLapsingSteps.Length - 1);
    }

    /// <summary>Computes review count to graduation. Accounts for PracticeState offset.</summary>
    /// <param name="card">Card instance.</param>
    /// <returns>Review count to graduation</returns>
    /// <exception cref="System.InvalidOperationException">Invalid call for State  + PracticeState</exception>
    public static int GetLearningStepsLeft(this Card card)
    {
      if (!card.IsLearning())
        throw new InvalidOperationException("Invalid call for State " + card.PracticeState);

      return card.LearningOrLapsingSteps.Length - card.GetCurrentLearningIndex();
    }

    /// <summary>Switch card State to Learning, while retaining misc states.</summary>
    /// <param name="card">Card instance.</param>
    public static void SetLearningState(this Card card)
    {
      card.PracticeState = PracticeState.Learning;
    }

    public static bool IsNew(this Card card)
    {
      return card.PracticeState == PracticeState.New;
    }

    public static bool IsLearning(this Card card)
    {
      return card.PracticeState >= PracticeState.Learning;
    }

    #endregion



    #region Due

    /// <summary>Process fail-graded answers for due cards.</summary>
    /// <param name="card">Card instance.</param>
    /// <param name="grade">Fail grade</param>
    public static CardAction Lapse(this Card card, Grade grade)
    {
      if (grade > Grade.Fail)
        throw new ArgumentException("Card.Lapse invoked with invalid grade", nameof(grade));

      card.Lapses++;

      // TODO: Handle extended grading options
      card.EFactor += grade.ReviewEaseModifiers(card.Config);
      card.UpdateLearningStep(true);

      if (card.IsLeech())
        return card.Leech();

      return CardAction.Update;
    }

    /// <summary>
    ///   Process instances where lapse threshold rules are met and card turns into Leech. Take
    ///   action accordingly to preferences (suspend or delete card).
    /// </summary>
    /// <param name="card">Card instance.</param>
    public static CardAction Leech(this Card card)
    {
      switch (card.Config.LeechAction)
      {
        case CardLeechAction.Suspend:
          card.SetSuspendedState();
          return CardAction.Update;

        case CardLeechAction.Delete:
          return CardAction.Delete;
      }

      throw new ArgumentException("Card.Leech invoked with invalid LeechAction setting");
    }

    /// <summary>Determines whether lapse threshold rules are met and card leech.</summary>
    /// <param name="card">Card instance.</param>
    /// <returns>Whether card is a leech</returns>
    public static bool IsLeech(this Card card)
    {
      return card.Config.LeechThreshold > 0 && card.Lapses >= card.Config.LeechThreshold
             && card.Lapses % (int)Math.Ceiling(card.Config.LeechThreshold / 2.0f) == 0;
    }

    /// <summary>Process pass-graded answers for due cards.</summary>
    /// <param name="card">Card instance.</param>
    /// <param name="grade">Pass grade</param>
    public static CardAction Review(this Card card, Grade grade)
    {
      if (grade < Grade.Hard)
        throw new ArgumentException("Card.Review invoked with invalid grade", nameof(grade));

      // Interval computing must happen prior to EFactor update
      card.Interval = card.ComputeReviewInterval(grade);
      card.EFactor += grade.ReviewEaseModifiers(card.Config);

      card.SetDueFromInterval();

      return CardAction.Update;
    }

    /// <summary>Computes the new interval.</summary>
    /// <param name="card">Card instance.</param>
    /// <param name="grade">The grade.</param>
    /// <returns></returns>
    public static int ComputeReviewInterval(this Card card, Grade grade)
    {
      int daysLate = Math.Max(0, (DateTime.Now - card.DueDateTime).Days);
      int newInterval = grade.GradeReviewIntervalFormulas(card.Config)(
        card.Interval, daysLate, card.EFactor);

      return Math.Max(RandomizeInterval(newInterval), card.Interval + 1);
    }

    /// <summary>Adds some randomness to interval computation.</summary>
    /// <param name="interval">The interval.</param>
    /// <returns></returns>
    public static int RandomizeInterval(int interval)
    {
      int[] rndRange;

      if (interval < 2)
        return interval;

      if (interval == 2)
        rndRange = new[] { 2, 3 };
      else
      {
        float rnd;

        if (interval < 7)
          rnd = Math.Max(1, interval * 0.25f);
        else if (interval < 30)
          rnd = Math.Max(2, interval * 0.15f);
        else
          rnd = Math.Max(4, interval * 0.05f);

        rndRange = new[] { interval - (int)rnd, interval + (int)rnd };
      }

      return new Random().Next(rndRange[0], rndRange[1] + 1);
    }

    /// <summary>Sets card State to Suspended, while retaining main state.</summary>
    /// <param name="card">Card instance.</param>
    public static void SetSuspendedState(this Card card)
    {
      card.MiscState |= CardMiscStateFlag.Suspended;
    }

    /// <summary>Switch card State to Due, while retaining misc states.</summary>
    /// <param name="card">Card instance.</param>
    public static void SetDueState(this Card card)
    {
      card.PracticeState = PracticeState.Due;
    }

    public static bool IsDue(this Card card)
    {
      return card.PracticeState == PracticeState.Due;
    }

    #endregion



    #region Misc

    public static int ReviewLeftToday(this Card card)
    {
      if (card.Due >= DateTimeExtensions.Tomorrow.ToUnixTimestamp())
        return 0;

      switch (card.PracticeState)
      {
        case PracticeState.New:
          return 1;

        case PracticeState.Due:
          return 1;

        case PracticeState.Deleted: // Leech option
          return 0;
      }

      if (card.IsLearning())
        return card.GetLearningStepsLeft();

      throw new InvalidOperationException("Invalid card state");
    }

    /// <summary>Build Due time from Interval (which unit is days).</summary>
    /// <param name="card">Card instance.</param>
    public static void SetDueFromInterval(this Card card)
    {
      card.SetDue(Delay.FromDays(card.Interval));
    }

    /// <summary>Build Due time respectively from given Delay and current review time.</summary>
    /// <param name="card">Card instance.</param>
    /// <param name="delay">The delay</param>
    public static void SetDue(this Card card, Delay delay)
    {
      card.Due = card.CurrentReviewTime + delay;
    }

    public static bool IsDismissed(this Card card)
    {
      return (card.MiscState & CardMiscStateFlag.Dismissed) == CardMiscStateFlag.Dismissed;
    }

    public static bool IsSuspended(this Card card)
    {
      return (card.MiscState & CardMiscStateFlag.Suspended) == CardMiscStateFlag.Suspended;
    }

#if false
    public static int ComputeReviewCount( // TODO: This may be useful at some point
      ConstSpacedRepetition.PracticeState state,
      CollectionConfig config, ConstSpacedRepetition.Grade grade)
    {
      switch (state)
      {
        case ConstSpacedRepetition.PracticeState.New:
          return grade == ConstSpacedRepetition.Grade.Easy
                   ? 1
                   : config.LearningSteps.Length;

        case ConstSpacedRepetition.PracticeState.Due:
          return grade == ConstSpacedRepetition.Grade.Easy
                   ? 1
                   : config.LearningSteps.Length;

        case ConstSpacedRepetition.PracticeState.Learning:
          return grade == ConstSpacedRepetition.Grade.Easy
                   ? 1
                   : config.LearningSteps.Length;
      }
      /*
        (grade == ConstSpacedRepetition.Grade.Easy
        ? newCards : newCards * config.LearningSteps.Length)
        + (grade == ConstSpacedRepetition.Grade.Easy
        ? learnCards : learnCards * config.LearningSteps.Length)
        + (grade == ConstSpacedRepetition.Grade.Easy
        ? lapsingCards : lapsingCards * config.LapseSteps.Length)
        + dueCards;
      */
    }
#endif

    #endregion
  }
}