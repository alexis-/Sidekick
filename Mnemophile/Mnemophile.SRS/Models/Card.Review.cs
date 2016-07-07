using System;

namespace Mnemophile.SRS.Models
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
    /// <param name="easy">whether selected ease was 'easy'</param>
    internal void Graduate(bool easy = false)
    {
      SetDueState();

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
    }

    /// <summary>
    /// Either setup lapsing/learning steps (if reset is set to true), move
    /// on to next step, or graduate.
    /// </summary>
    /// <param name="reset">if true, set learning state and lapsing or
    /// learning first step.</param>
    internal void UpdateLearningStep(bool reset = false)
    {
      // Set learning mode and first step delay
      if (reset)
      {
        SetLearningState();
        SetDueInMinutes(LearningOrLapsingSteps[0]);
      }

      // Graduate
      else if (IsGraduating())
          Graduate(); // Graduate

      // Move on to next step
      else
        SetDueInMinutes(LearningOrLapsingSteps[GetCurrentLearningStep() + 1]);
    }

    /// <summary>
    /// Determines whether current lapsing or learning step is graduating.
    /// </summary>
    /// <returns>Whether current step is graduating</returns>
    internal bool IsGraduating()
    {
      if (IsDue())
        // TODO: Log an error here
        return false;

      return GetCurrentLearningStep() >= LearningOrLapsingSteps.Length - 1;
    }

    /// <summary>
    /// Get current lapsing or learning step index.
    /// In the occurence steps settings changed, closest inferior value
    /// index is returned.
    /// </summary>
    /// <returns>Current lapsing or learning index</returns>
    internal int GetCurrentLearningStep()
    {
      int lastStepValue = Due - LastModified;
      int lastStep = 0;

      for (;
        lastStep < LearningOrLapsingSteps.Length
        && lastStepValue > LearningOrLapsingSteps[lastStep] * 60; // in second
        lastStep++)
        ;

      return lastStep;
    }

    /// <summary>
    /// Switch card State to Learning, while retaining other flags
    /// </summary>
    internal void SetLearningState()
    {
      State = (State & CardMiscStateMask) | CardStateFlag.Learning;
    }
    #endregion



    //
    //
    // Due

    #region Due

    /// <summary>
    /// Switch card State to Due, while retaining other flags
    /// </summary>
    internal void SetDueState()
    {
      State = (State & CardMiscStateMask) | CardStateFlag.Due;
    }
    #endregion



    //
    //
    // Misc

    #region Misc

    internal void SetDueFromInterval()
    {
      SetDueInDays(Interval);
    }

    internal void SetDueInMinutes(int minutes)
    {
      SetDue(minutes, 0);
    }

    internal void SetDueInDays(int days)
    {
      SetDue(0, days);
    }

    internal void SetDue(int minutes, int days)
    {
      Due = CurrentReviewTime
            + minutes * 60
            + days * 24 * 3600;
    }
    #endregion
  }
}