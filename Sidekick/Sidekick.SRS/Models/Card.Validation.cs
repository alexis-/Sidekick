using System;

namespace Sidekick.SpacedRepetition.Models
{
  public partial class Card
  {
    // TODO: Assert
    // - LearningSteps in ascending order
    // - LearningSteps not empty

    /// <summary>
    /// Called each time Interval value is set.
    /// Ensure Interval is greater than 0, less than maximum Interval and
    /// apply Interval modifer (default 100%).
    /// </summary>
    /// <param name="interval">new interval value</param>
    /// <returns>Sanitized Interval</returns>
    public int SanitizeInterval(int interval)
    {
      return Math.Max(1, Math.Min(Config.ReviewMaxInterval,
        (int)(interval * Config.ReviewIntervalModifier)));
    }

    /// <summary>
    /// Called each time EFactor value is set.
    /// Ensure EFactor does not fall below minimum Ease (default 130%).
    /// </summary>
    /// <param name="eFactor">new EFactor value</param>
    /// <returns>Sanitized EFactor</returns>
    public float SanitizeEFactor(float eFactor)
    {
      return Math.Max(Config.ReviewMinEase, eFactor);
    }
  }
}
