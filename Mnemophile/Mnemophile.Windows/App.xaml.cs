using System;
using System.Globalization;
using System.Windows;
using Catel.ApiCop;
using Catel.ApiCop.Listeners;
using Catel.IoC;
using Catel.Logging;
using Catel.Services;
using Catel.Services.Models;
using Mnemophile.Windows.Views;
using Orchestra.Services;
using Orchestra.Views;

namespace Mnemophile.Windows
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    public App()
    {
    }

    protected override void OnStartup(StartupEventArgs e)
    {
      var dependencyResolver = this.GetDependencyResolver();

      //
      // Logging

#if DEBUG
      LogManager.AddDebugListener(true);
#endif


      //
      // Localization

      var languageService = dependencyResolver.Resolve<ILanguageService>();
      languageService.FallbackCulture = new CultureInfo("en-US");

      languageService.RegisterLanguageSource(
        new LanguageResourceSource(
          "Mnemophile.Windows",
          "Mnemophile.Windows.Properties",
          "Resources"));


      //
      // Shell creation

      var serviceLocator = ServiceLocator.Default;
      var shellService = serviceLocator.ResolveType<IShellService>();

      shellService.CreateWithSplashAsync<MnemoShellWindow>();
    }

    protected override void OnExit(ExitEventArgs e)
    {
      //
      // API Cop output

#if DEBUG
      var apiCopListener = new ConsoleApiCopListener();
      ApiCopManager.AddListener(apiCopListener);
      ApiCopManager.WriteResults();
#endif

      base.OnExit(e);
    }
  }
}
