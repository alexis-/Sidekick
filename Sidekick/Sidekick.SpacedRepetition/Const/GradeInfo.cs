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

namespace Sidekick.SpacedRepetition.Const
{
  public struct GradeInfo
  {
    public Grade Grade { get; set; }
    public string LocalizableText { get; set; }
    public DateTime NextReview { get; set; }
    public string[] CardValuesAftermath { get; set; }


    //
    // Pre-defined GradeInfo

    public static GradeInfo GradeFailSevere =>
      new GradeInfo
      {
        Grade = Grade.FailSevere,
        LocalizableText = "SpacedRepetition_Grade_FailSevere"
      };

    public static GradeInfo GradeFailMedium =>
      new GradeInfo
      {
        Grade = Grade.FailMedium,
        LocalizableText = "SpacedRepetition_Grade_FailMedium"
      };

    public static GradeInfo GradeFail =>
      new GradeInfo
      {
        Grade = Grade.Fail,
        LocalizableText = "SpacedRepetition_Grade_Fail"
      };

    public static GradeInfo GradeHard =>
      new GradeInfo
      {
        Grade = Grade.Hard,
        LocalizableText = "SpacedRepetition_Grade_Hard"
      };

    public static GradeInfo GradeGood =>
      new GradeInfo
      {
        Grade = Grade.Good,
        LocalizableText = "SpacedRepetition_Grade_Good"
      };

    public static GradeInfo GradeEasy =>
      new GradeInfo
      {
        Grade = Grade.Easy,
        LocalizableText = "SpacedRepetition_Grade_Easy"
      };
  }
}