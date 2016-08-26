using Catel.Configuration;
using JetBrains.Annotations;
using MahApps.Metro;
using Sidekick.Windows.Services.Interfaces;

namespace Sidekick.Windows.Services
{

  internal class ApplicationConfigurationService :
    IApplicationConfigurationService
  {
    private readonly IConfigurationService _configurationService;

    public ApplicationConfigurationService(
      [NotNull]IConfigurationService configurationService)
    {
      _configurationService = configurationService;
    }

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


    //
    // Theme

    public AppTheme Theme {
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
  }
}
