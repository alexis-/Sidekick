using System;
using Sidekick.Shared.Const.SpacedRepetition;
using Sidekick.Shared.Utils;
using Sidekick.SpacedRepetition.Impl;

namespace Sidekick.SpacedRepetition.Models
{
  public partial class Card
  {
    //
    //
    // Learning

    #region Learning
    /// <summary>
    /// Graduate from learning into due state.
    /// </summary>
    /// <param name="easy">whether answer was graded 'easy'</param>
    public CardAction Graduate(bool easy = false)
    {
      if (IsDue())
        throw new InvalidOperationException(
          "Card.Graduate invoked with card 'Due' state");

      // Regraduating
      if (Lapses > 0)
        Interval = Math.Max(Config.LapseMinInterval,
          (int)(Config.LapseIntervalFactor * Interval));

      // Graduating for the first time
      else
      {
        Interval = easy
          ? Config.GraduationEasyInterval
          : Config.GraduationInterval;
        EFactor = Config.GraduationStartingEase;
      }

      SetDueFromInterval();
      SetDueState();

      return CardAction.Update;
    }

    /// <summary>
    /// Either setup lapsing/learning steps (if reset is set to true), move
    /// on to next step, or graduate.
    /// </summary>
    /// <param name="reset">if true, set learning state and lapsing or
    /// learning first step.</param>
    public CardAction UpdateLearningStep(bool reset = false)
    {
      // Set learning mode and first step delay
      if (reset)
      {
        SetDue(LearningOrLapsingSteps[0]);
        SetLearningState();
      }

      else if (IsDue())
        throw new InvalidOperationException(
          "Card.UpdateLearningStep invoked with " + nameof(reset)
          + " parameter 'false' and card 'Due' state");

      // Graduate
      else if (IsGraduating())
          Graduate();

      // Move on to next step
      else
        SetDue(LearningOrLapsingSteps[IncreaseLearningStep()]);

      return CardAction.Update;
    }

    /// <summary>
    /// Determines whether current lapsing or learning step is graduating.
    /// </summary>
    /// <returns>Whether current step is graduating</returns>
    public bool IsGraduating()
    {
      if (IsDue())
        return false;

      return GetCurrentLearningIndex() >= LearningOrLapsingSteps.Length - 1;
    }

    /// <summary>
    /// Increment learning step index, and return new value.
    /// Accounts for CardPracticeState offset.
    /// </summary>
    /// <returns>New step index</returns>
    public int IncreaseLearningStep()
    {
      PracticeState++;

      return GetCurrentLearningIndex();
    }

    /// <summary>
    /// Switch card State to Learning, while retaining misc states.
    /// </summary>
    public void SetLearningState()
    {
      PracticeState = ConstSpacedRepetition.CardPracticeState.Learning;
    }
    #endregion



    //
    //
    // Due

    #region Due
    /// <summary>
    /// Process fail-graded answers for due cards.
    /// </summary>
    /// <param name="grade">Fail grade</param>
    public CardAction Lapse(ConstSpacedRepetition.Grade grade)
    {
      if (grade > ConstSpacedRepetition.Grade.Fail)
        throw new ArgumentException(
          "Card.Lapse invoked with invalid grade", nameof(grade));

      Lapses++;

      // TODO: Handle extended grading options
      EFactor += GradingOptions.GradeReviewEaseModifiers(grade, Config);
      UpdateLearningStep(true);

      if (IsLeech())
        return Leech();

      return CardAction.Update;
    }

    /// <summary>
    /// Process instances where lapse threshold rules are met and card turns
    /// into Leech. Take action accordingly to preferences (suspend or delete
    /// card).
    /// </summary>
    public CardAction Leech()
    {
      switch (Config.LeechAction)
      {
        case ConstSpacedRepetition.CardLeechAction.Suspend:
          SetSuspendedState();
          return CardAction.Update;

        case ConstSpacedRepetition.CardLeechAction.Delete:
          return CardAction.Delete;
      }

      throw new ArgumentException(
        "Card.Leech invoked with invalid LeechAction setting");
    }

    /// <summary>
    /// Determines whether lapse threshold rules are met and card leech.
    /// </summary>
    /// <returns>Whether card is a leech</returns>
    public bool IsLeech()
    {
      return Config.LeechThreshold > 0
        && Lapses >= Config.LeechThreshold
        && Lapses % (int)Math.Ceiling(Config.LeechThreshold / 2.0f) == 0;
    }

    /// <summary>
    /// Process pass-graded answers for due cards.
    /// </summary>
    /// <param name="grade">Pass grade</param>
    public CardAction Review(ConstSpacedRepetition.Grade grade)
    {
      if (grade < ConstSpacedRepetition.Grade.Hard)
        throw new ArgumentException(
          "Card.Review invoked with invalid grade", nameof(grade));

      // Interval computing must happen prior to EFactor update
      Interval = ComputeReviewInterval(grade);
      EFactor += GradingOptions.GradeReviewEaseModifiers(grade, Config);

      SetDueFromInterval();

      return CardAction.Update;
    }

    public int ComputeReviewInterval(ConstSpacedRepetition.Grade grade)
    {
      int daysLate = (DateTime.Now - DateTimeEx.FromUnixTimestamp(Due)).Days;
      int newInterval = GradingOptions.GradeReviewIntervalFormulas(grade,
        Config)(Interval, daysLate, EFactor);

      return Math.Max(RandomizeInterval(newInterval), Interval + 1);
    }

    public int RandomizeInterval(int interval)
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

    /// <summary>
    /// Sets card State to Suspended, while retaining main state.
    /// </summary>
    public void SetSuspendedState()
    {
      MiscState |= ConstSpacedRepetition.CardMiscStateFlag.Suspended;
    }

    /// <summary>
    /// Switch card State to Due, while retaining misc states.
    /// </summary>
    public void SetDueState()
    {
      PracticeState = ConstSpacedRepetition.CardPracticeState.Due;
    }
    #endregion



    //
    //
    // Misc

    #region Misc

#if false
    public static int ComputeReviewCount( // TODO: This may be useful at some point
      ConstSpacedRepetition.CardPracticeState state,
      CollectionConfig config, ConstSpacedRepetition.Grade grade)
    {
      switch (state)
      {
        case ConstSpacedRepetition.CardPracticeState.New:
          return grade == ConstSpacedRepetition.Grade.Easy
                   ? 1
                   : config.LearningSteps.Length;

        case ConstSpacedRepetition.CardPracticeState.Due:
          return grade == ConstSpacedRepetition.Grade.Easy
                   ? 1
                   : config.LearningSteps.Length;

        case ConstSpacedRepetition.CardPracticeState.Learning:
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

    /// <summary>
    /// Build Due time from Interval (which unit is days).
    /// </summary>
    public void SetDueFromInterval()
    {
      SetDue(Delay.FromDays(Interval));
    }

    /// <summary>
    /// Build Due time respectively from given Delay and current review time.
    /// </summary>
    /// <param name="delay">The delay</param>
    public void SetDue(Delay delay)
    {
      Due = CurrentReviewTime + delay;
    }
    #endregion
  }
}