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
using Sidekick.SpacedRepetition.Const;
using Sidekick.SpacedRepetition.Models;

namespace Sidekick.SpacedRepetition.Extensions
{
  internal static class GradeExtensions
  {
    #region Methods

    internal static float ReviewEaseModifiers(
      this Grade grade, CollectionConfig config)
    {
      switch (grade)
      {
        case Grade.FailSevere:
        case Grade.FailMedium:
        case Grade.Fail:
          return config.LapseEaseMalus;
        case Grade.Hard:
          return config.ReviewHardEaseModifier;
        case Grade.Good:
          return config.ReviewGoodEaseModifier;
        case Grade.Easy:
          return config.ReviewEasyEaseModifier;
      }

      throw new ArgumentException("Invalid grade option", nameof(grade));
    }

    internal static Func<int, int, float, int> GradeReviewIntervalFormulas(
      this Grade grade, CollectionConfig config)
    {
      switch (grade)
      {
        case Grade.Hard:
          return (lastInterval, delay, eFactor) => Math.Max(lastInterval + 1,
            (int)Math.Floor((lastInterval + delay * 0.25f) * 1.2f));
        case Grade.Good:
          return (lastInterval, delay, eFactor) => Math.Max(lastInterval + 1,
            (int)Math.Floor((lastInterval + delay * 0.5f) * eFactor));
        case Grade.Easy:
          return (lastInterval, delay, eFactor) => Math.Max(lastInterval + 1,
            (int)((lastInterval + delay) * eFactor * config.ReviewEasyBonus));
      }

      throw new ArgumentException("Invalid grade option", nameof(grade));
    }

    #endregion
  }
}