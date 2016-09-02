// 
// The MIT License (MIT)
// Copyright (c) 2016 Incogito
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
using Sidekick.Shared.Attributes.Database;
using Sidekick.Shared.Extensions;
using Sidekick.Shared.Utils;
using Sidekick.SpacedRepetition.Const;

namespace Sidekick.SpacedRepetition.Models
{
  public partial class Card
  {
    #region Properties

    /// <summary>
    /// Configuration inherited by current Card
    /// </summary>
    [Ignore]
    internal CollectionConfig Config { get; set; }

    /// <summary>
    /// Used in reviewing card process. Store time of review, from which
    /// other values will be computed (Due, Steps, ...)
    /// </summary>
    [Ignore]
    public int CurrentReviewTime { get; set; }

    /// <summary>
    /// Either return Config's Lapsing or Learning steps, depending on Card
    /// Lapse value.
    /// </summary>
    [Ignore]
    public Delay[] LearningOrLapsingSteps => Lapses > 0
                                               ? Config.LapseSteps
                                               : Config.LearningSteps;

    /// <summary>
    /// Gets due value in DateTime format.
    /// </summary>
    [Ignore]
    public DateTime DueDateTime =>
      DateTimeExtensions.FromUnixTimestamp(Due);

    #endregion

    #region Methods

    /// <summary>
    /// Get current lapsing or learning step index.
    /// In the occurence steps settings changed, closest inferior value
    /// index is returned.
    /// Accounts for CardPracticeState offset.
    /// </summary>
    /// <returns>Current lapsing or learning index</returns>
    public int GetCurrentLearningIndex()
    {
      if (!IsLearning())
        throw new InvalidOperationException(
          "Invalid call for State " + PracticeState);

      // Account for possible config changes
      return Math.Min(
        PracticeState - CardPracticeState.Learning,
        LearningOrLapsingSteps.Length - 1);
    }

    /// <summary>
    /// Computes review count to graduation.
    /// Accounts for CardPracticeState offset.
    /// </summary>
    /// <returns>Review count to graduation</returns>
    /// <exception cref="System.InvalidOperationException">Invalid call for State  + PracticeState</exception>
    public int GetLearningStepsLeft()
    {
      if (!IsLearning())
        throw new InvalidOperationException(
          "Invalid call for State " + PracticeState);

      return LearningOrLapsingSteps.Length - GetCurrentLearningIndex();
    }

    public bool IsNew()
    {
      return PracticeState == CardPracticeState.New;
    }

    public bool IsLearning()
    {
      return PracticeState >= CardPracticeState.Learning;
    }

    public bool IsDue()
    {
      return PracticeState == CardPracticeState.Due;
    }

    public bool IsDismissed()
    {
      return (MiscState & CardMiscStateFlag.Dismissed) ==
             CardMiscStateFlag.Dismissed;
    }

    public bool IsSuspended()
    {
      return (MiscState & CardMiscStateFlag.Suspended) ==
             CardMiscStateFlag.Suspended;
    }

    #endregion
  }
}