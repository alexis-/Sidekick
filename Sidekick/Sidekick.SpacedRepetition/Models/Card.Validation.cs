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

namespace Sidekick.SpacedRepetition.Models
{
  public partial class Card
  {
    #region Methods

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

    #endregion
  }
}