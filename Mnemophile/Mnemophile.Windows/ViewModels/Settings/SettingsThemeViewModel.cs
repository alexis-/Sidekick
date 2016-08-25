using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using Catel.Data;
using Catel.MVVM;
using MahApps.Metro;
using Mnemophile.Windows.Services.Interfaces;

namespace Mnemophile.Windows.ViewModels.Settings
{
  public class SettingsThemeViewModel : ViewModelBase
  {
    private readonly IApplicationConfigurationService _applicationConfigService;
    private bool _isInitializing = false;

    public SettingsThemeViewModel(IApplicationConfigurationService applicationConfigService)
    {
      _applicationConfigService = applicationConfigService;
    }

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

    public IEnumerable<AppTheme> Themes { get; private set; }
    public IEnumerable<Accent> Accents { get; private set; }

    public AppTheme SelectedTheme { get; set; }
    public Accent SelectedAccent { get; set; }

    protected override void OnPropertyChanged(AdvancedPropertyChangedEventArgs e)
    {
      base.OnPropertyChanged(e);

      if (!_isInitializing && e.NewValue != null)
        switch (e.PropertyName)
        {
          case "SelectedTheme":
            ThemeManager.ChangeAppStyle(
              Application.Current,
              SelectedAccent,
              SelectedTheme);

            _applicationConfigService.Theme = SelectedTheme;
            break;

          case "SelectedAccent":
            ThemeManager.ChangeAppStyle(
              Application.Current,
              SelectedAccent,
              SelectedTheme);

            _applicationConfigService.Accent = SelectedAccent;
            break;
        }
    }
  }
}