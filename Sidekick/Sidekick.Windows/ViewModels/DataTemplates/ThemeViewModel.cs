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

namespace Sidekick.Windows.ViewModels.DataTemplates
{
  using System.Windows.Media;

  using Catel;
  using Catel.Fody;
  using Catel.MVVM;

  using MahApps.Metro;

  /// <summary>
  ///   ViewModel for MahApps theme view
  /// </summary>
  /// <seealso cref="Catel.MVVM.ViewModelBase" />
  public class ThemeViewModel : ViewModelBase
  {
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ThemeViewModel"/> class.
    /// </summary>
    /// <param name="accent">The accent.</param>
    public ThemeViewModel(Accent accent)
      : base(false)
    {
      Argument.IsNotNull(() => accent);

      ColorName = accent.Name;
      ColorBrush = accent.Resources["AccentColorBrush"] as Brush;
      BorderColorBrush = ColorBrush;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ThemeViewModel"/> class.
    /// </summary>
    /// <param name="theme">The theme.</param>
    public ThemeViewModel(AppTheme theme)
      : base(false)
    {
      Argument.IsNotNull(() => theme);

      ColorName = theme.Name;
      ColorBrush = theme.Resources["WhiteColorBrush"] as Brush;
      BorderColorBrush = theme.Resources["BlackColorBrush"] as Brush;
    }

    #endregion



    #region Properties

    public string ColorName { get; set; }
    public Brush ColorBrush { get; set; }
    public Brush BorderColorBrush { get; set; }

    #endregion
  }
}