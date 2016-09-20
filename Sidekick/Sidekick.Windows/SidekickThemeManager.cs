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
  using System.Collections.Generic;
  using System.Linq;

  using MahApps.Metro;

  /// <summary>Sidekick-specific theme management, based on MahApps themes</summary>
  public static class SidekickThemeManager
  {
    #region Fields

    private static readonly string[] SidekickThemes = { "LightSide", "DarkSide" };

    #endregion



    #region Properties

    /// <summary>Gets the application themes.</summary>
    public static IEnumerable<AppTheme> AppThemes => ThemeManager.AppThemes
      .Where(t => SidekickThemes.Contains(t.Name));

    /// <summary>Gets the application accents.</summary>
    public static IEnumerable<Accent> Accents => ThemeManager.Accents;

    #endregion



    #region Methods

    /// <summary>Setup Sidekick themes.</summary>
    /// <exception cref="System.InvalidOperationException">
    ///   Sidekick ThemeManager is already
    ///   initialized.
    /// </exception>
    public static void Initialize()
    {
      if (ThemeManager.AppThemes.Any(t => SidekickThemes.Contains(t.Name)))
        throw new InvalidOperationException("Sidekick ThemeManager is already initialized.");

      foreach (var theme in SidekickThemes)
      {
        var resourceAddress =
          new Uri(
            $"pack://application:,,,/Sidekick.Windows;component/Styles/Themes/{theme}.xaml");

        ThemeManager.AddAppTheme(theme, resourceAddress);
      }
    }

    /// <summary>Find a specific accent.</summary>
    /// <param name="name">Theme name.</param>
    public static Accent GetAccent(string name)
    {
      return
        Accents.FirstOrDefault(
          a => a.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
    }

    /// <summary>Find a specific AppTheme.</summary>
    /// <param name="name">Theme name.</param>
    public static AppTheme GetAppTheme(string name)
    {
      return
        AppThemes.FirstOrDefault(
          t => t.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
    }

    #endregion
  }
}