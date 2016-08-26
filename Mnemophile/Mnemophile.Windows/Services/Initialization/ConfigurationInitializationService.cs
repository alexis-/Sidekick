using Catel.Configuration;
using JetBrains.Annotations;
using Mnemophile.Windows.Services.Interfaces;

namespace Mnemophile.Windows.Services
{

  internal class ConfigurationInitializationService : IConfigurationInitializationService
  {
    private readonly IConfigurationService _configurationService;

    public ConfigurationInitializationService([NotNull]IConfigurationService configurationService)
    {
      _configurationService = configurationService;
    }

    public void Initialize()
    {
      // Theme
      InitializeConfigurationKey(Settings.Application.Theme.AppTheme, Settings.Application.Theme.AppThemeDefaultValue);
      InitializeConfigurationKey(Settings.Application.Theme.Accent, Settings.Application.Theme.AccentDefaultValue);
    }

    private void InitializeConfigurationKey(string key, object defaultValue)
    {
      _configurationService.InitializeValue(ConfigurationContainer.Local, key, defaultValue);
    }
  }
}
