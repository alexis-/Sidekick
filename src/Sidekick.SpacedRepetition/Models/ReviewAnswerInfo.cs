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

  /// <summary>
  ///   Answer meta-datas : Grade value, Localized description, 
  /// </summary>
  public struct ReviewAnswerInfo
  {
    #region Predefined

    /// <summary>Gets the fail severe.</summary>
    public static ReviewAnswerInfo FailSevere =>
      new ReviewAnswerInfo
      {
        Grade = Grade.FailSevere,
        LocalizableText = "SpacedRepetition_Grade_FailSevere"
      };

    /// <summary>Gets the fail medium.</summary>
    public static ReviewAnswerInfo FailMedium =>
      new ReviewAnswerInfo
      {
        Grade = Grade.FailMedium,
        LocalizableText = "SpacedRepetition_Grade_FailMedium"
      };

    /// <summary>Gets the fail.</summary>
    public static ReviewAnswerInfo Fail =>
      new ReviewAnswerInfo
      {
        Grade = Grade.Fail,
        LocalizableText = "SpacedRepetition_Grade_Fail"
      };

    /// <summary>Gets the hard.</summary>
    public static ReviewAnswerInfo Hard =>
      new ReviewAnswerInfo
      {
        Grade = Grade.Hard,
        LocalizableText = "SpacedRepetition_Grade_Hard"
      };

    /// <summary>Gets the good.</summary>
    public static ReviewAnswerInfo Good =>
      new ReviewAnswerInfo
      {
        Grade = Grade.Good,
        LocalizableText = "SpacedRepetition_Grade_Good"
      };

    /// <summary>Gets the easy.</summary>
    public static ReviewAnswerInfo Easy =>
      new ReviewAnswerInfo
      {
        Grade = Grade.Easy,
        LocalizableText = "SpacedRepetition_Grade_Easy"
      };

    #endregion



    #region Properties

    /// <summary>Answer grade.</summary>
    public Grade Grade { get; set; }

    /// <summary>Localizable text reference.</summary>
    public string LocalizableText { get; set; }

    /// <summary>Next review date time.</summary>
    public DateTime NextReview { get; set; }

    /// <summary>Aftermath values of answering with given <see cref="Grade"/>.</summary>
    public string[] CardValuesAftermath { get; set; }

    #endregion
  }
}