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

namespace Sidekick.Windows.ViewModels.Settings
{
  using System.Collections.Generic;
  using System.Threading.Tasks;
  using System.Windows;

  using Catel.MVVM;

  using MahApps.Metro;

  using Sidekick.Windows.Services.Interfaces;

  /// <summary>
  ///   App-appearance related settings panel View Model
  /// </summary>
  /// <seealso cref="Catel.MVVM.ViewModelBase" />
  public class SettingsThemeViewModel : ViewModelBase
  {
    #region Fields

    private readonly IApplicationConfigurationService _applicationConfigService;
    private bool _isInitializing = false;

    #endregion



    #region Constructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="SettingsThemeViewModel" /> class.
    /// </summary>
    /// <param name="applicationConfigService">The application configuration service.</param>
    public SettingsThemeViewModel(IApplicationConfigurationService applicationConfigService)
      : base(false)
    {
      _applicationConfigService = applicationConfigService;
    }

    #endregion



    #region Properties

    public IEnumerable<AppTheme> Themes { get; private set; }
    public IEnumerable<Accent> Accents { get; private set; }

    public AppTheme SelectedTheme { get; set; }
    public Accent SelectedAccent { get; set; }

    #endregion



    #region Methods

    protected override Task InitializeAsync()
    {
      _isInitializing = true;

      Themes = ThemeManager.AppThemes;
      Accents = ThemeManager.Accents;

      SelectedTheme = _applicationConfigService.Theme;
      SelectedAccent = _applicationConfigService.Accent;

      _isInitializing = false;

      return base.InitializeAsync();
    }

    private void OnSelectedThemeChanged()
    {
      if (_isInitializing)
        return;

      ThemeManager.ChangeAppStyle(Application.Current, SelectedAccent, SelectedTheme);

      _applicationConfigService.Theme = SelectedTheme;
    }

    private void OnSelectedAccentChanged()
    {
      if (_isInitializing)
        return;

      ThemeManager.ChangeAppStyle(Application.Current, SelectedAccent, SelectedTheme);

      _applicationConfigService.Accent = SelectedAccent;
    }

    #endregion
  }
}