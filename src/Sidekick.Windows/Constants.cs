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

namespace Sidekick.Windows
{
  using System;
  using System.Windows.Input;

  using InputGesture = Catel.Windows.Input.InputGesture;

  // ReSharper disable StyleCop.SA1600

  public static class Settings
  {
    public static class Application
    {
      public static class Theme
      {
        #region Fields

        public const string AppTheme = "Theme.AppTheme";
        public const string AppThemeDefaultValue = "LightSide";

        public const string Accent = "Theme.Accent";
        public const string AccentDefaultValue = "Blue";

        #endregion
      }
    }
  }

  public static class Commands
  {
    public static class General
    {
      #region Fields

      public const string Search = "General.Search";
      public static readonly InputGesture SearchInputGesture = new InputGesture(
        Key.F, ModifierKeys.Control);

      #endregion
    }

    public static class SpacedRepetition
    {
      public static class CollectionReview
      {
        #region Fields

        public const string Answer1 = "SpacedRepetition.CollectionReview.Answer1";
        public const string Answer2 = "SpacedRepetition.CollectionReview.Answer2";
        public const string Answer3 = "SpacedRepetition.CollectionReview.Answer3";
        public const string Answer4 = "SpacedRepetition.CollectionReview.Answer4";
        public const string Answer5 = "SpacedRepetition.CollectionReview.Answer5";
        public static readonly InputGesture AnswerD1InputGesture = new InputGesture(Key.D1);
        public static readonly InputGesture AnswerNumPad1InputGesture =
          new InputGesture(Key.NumPad1);
        public static readonly InputGesture AnswerD2InputGesture = new InputGesture(Key.D2);
        public static readonly InputGesture AnswerNumPad2InputGesture =
          new InputGesture(Key.NumPad2);
        public static readonly InputGesture AnswerD3InputGesture = new InputGesture(Key.D3);
        public static readonly InputGesture AnswerNumPad3InputGesture =
          new InputGesture(Key.NumPad3);
        public static readonly InputGesture AnswerD4InputGesture = new InputGesture(Key.D4);
        public static readonly InputGesture AnswerNumPad4InputGesture =
          new InputGesture(Key.NumPad4);
        public static readonly InputGesture AnswerD5InputGesture = new InputGesture(Key.D5);
        public static readonly InputGesture AnswerNumPad5InputGesture =
          new InputGesture(Key.NumPad5);

        public static readonly Tuple<string, InputGesture, int>[] AnswerGestures =
        {
          new Tuple<string, InputGesture, int>(Answer1, AnswerD1InputGesture, 1),
          new Tuple<string, InputGesture, int>(Answer2, AnswerD2InputGesture, 2),
          new Tuple<string, InputGesture, int>(Answer3, AnswerD3InputGesture, 3),
          new Tuple<string, InputGesture, int>(Answer4, AnswerD4InputGesture, 4),
          new Tuple<string, InputGesture, int>(Answer5, AnswerD5InputGesture, 5),
          new Tuple<string, InputGesture, int>(Answer1, AnswerNumPad1InputGesture, 1),
          new Tuple<string, InputGesture, int>(Answer2, AnswerNumPad2InputGesture, 2),
          new Tuple<string, InputGesture, int>(Answer3, AnswerNumPad3InputGesture, 3),
          new Tuple<string, InputGesture, int>(Answer4, AnswerNumPad4InputGesture, 4),
          new Tuple<string, InputGesture, int>(Answer5, AnswerNumPad5InputGesture, 5)
        };

        #endregion
      }
    }
  }
}