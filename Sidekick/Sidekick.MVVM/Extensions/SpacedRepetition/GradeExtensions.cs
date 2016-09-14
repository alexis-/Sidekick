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

namespace Sidekick.MVVM.Extensions.SpacedRepetition
{
  using System;

  using Sidekick.SpacedRepetition.Const;

  using Xamarin.Forms;

  /// <summary>Extension methods for <see cref="Grade" />
  /// </summary>
  public static class GradeExtensions
  {
    #region Methods

    /// <summary>Returns the <see cref="Color" /> associated with given grade.</summary>
    /// <param name="grade">The grade.</param>
    /// <returns></returns>
    /// <exception cref="System.InvalidOperationException">Invalid grade</exception>
    public static Color GetColor(this Grade grade)
    {
      switch (grade)
      {
        case Grade.FailSevere:
          return Color.Black;

        case Grade.FailMedium:
          return Color.Black;

        case Grade.Fail:
          return Color.FromHex("#FFDC143C"); // Crimson

        case Grade.Hard:
          return Color.FromHex("#FF9ACD32"); // YellowGreen

        case Grade.Good:
          return Color.FromHex("#FF1E90FF"); // DodgerBlue

        case Grade.Easy:
          return Color.FromHex("FFA500"); // Orange
      }

      throw new InvalidOperationException("Invalid grade");
    }

    #endregion
  }
}