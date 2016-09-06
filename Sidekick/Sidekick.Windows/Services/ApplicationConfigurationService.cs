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

using Catel.Configuration;
using Catel.Fody;
using MahApps.Metro;
using Sidekick.Windows.Services.Interfaces;

namespace Sidekick.Windows.Services
{
  internal class ApplicationConfigurationService :
    IApplicationConfigurationService
  {
    #region Fields

    private readonly IConfigurationService _configurationService;

    #endregion

    #region Constructors

    public ApplicationConfigurationService(
      [NotNull] IConfigurationService configurationService)
    {
      _configurationService = configurationService;
    }

    #endregion

    #region Properties

    //
    // Theme

    public AppTheme Theme
    {
      get
      {
        string themeName = GetValue(
          Settings.Application.Theme.AppTheme,
          Settings.Application.Theme.AppThemeDefaultValue);

        return ThemeManager.GetAppTheme(themeName);
      }
      set
      {
        SetValue(
          Settings.Application.Theme.AppTheme,
          value.Name);
      }
    }

    public Accent Accent
    {
      get
      {
        string accentName = GetValue(
          Settings.Application.Theme.Accent,
          Settings.Application.Theme.AccentDefaultValue);

        return ThemeManager.GetAccent(accentName);
      }
      set
      {
        SetValue(
          Settings.Application.Theme.Accent,
          value.Name);
      }
    }

    #endregion

    #region Methods

    //
    // Core

    private T GetValue<T>(string key, T defaultValue)
    {
      return _configurationService.GetValue(
        ConfigurationContainer.Local,
        key, defaultValue);
    }

    private void SetValue<T>(string key, T value)
    {
      _configurationService.SetValue(
        ConfigurationContainer.Local,
        key, value);
    }

    #endregion
  }
}