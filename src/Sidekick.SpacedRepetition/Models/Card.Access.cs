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

namespace Sidekick.SpacedRepetition.Models
{
  using System;

  using AgnosticDatabase.Attributes;

  using Sidekick.Shared.Extensions;
  using Sidekick.Shared.Utils;

  /// <summary>
  ///   Card model for SpacedRepetition system. Keeps track of individual cards progress
  ///   (ease, interval, ...) and datas.
  /// </summary>
  public partial class Card
  {
    #region Properties

    /// <summary>
    ///   Used in reviewing card process. Store time of review, from which other values will be
    ///   computed (Due, Steps, ...)
    /// </summary>
    [Ignore]
    public int CurrentReviewTime { get; set; }

    /// <summary>Either return Config's Lapsing or Learning steps, depending on Card Lapse value.</summary>
    [Ignore]
    public Delay[] LearningOrLapsingSteps
      => Lapses > 0 ? Config.LapseSteps : Config.LearningSteps;

    /// <summary>Gets due value in DateTime format.</summary>
    [Ignore]
    public DateTime DueDateTime => DateTimeExtensions.FromUnixTimestamp(Due);

    /// <summary>Configuration inherited by current Card</summary>
    [Ignore]
    internal CollectionConfig Config { get; set; }

    #endregion
  }
}