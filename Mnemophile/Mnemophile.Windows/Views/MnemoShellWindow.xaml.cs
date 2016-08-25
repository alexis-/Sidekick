using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Catel.IoC;
using Catel.MVVM;
using MahApps.Metro;
using MahApps.Metro.Controls;
using Mnemophile.Windows.Services.Interfaces;
using Orchestra;
using Orchestra.Services;
using Orchestra.Views;
using Orchestra.Windows;

namespace Mnemophile.Windows.Views
{
  /// <summary>
  /// Interaction logic for MnemoShellWindow.xaml
  /// </summary>
  public partial class MnemoShellWindow : MetroDataWindow, IShell
  {
    public MnemoShellWindow()
    {
      IServiceLocator serviceLocator = ServiceLocator.Default;

      InitializeComponent();


      //
      // Services

      // Get services
      var statusService = serviceLocator.ResolveType<IStatusService>();
      var commandManager = serviceLocator.ResolveType<ICommandManager>();
      var flyoutService = serviceLocator.ResolveType<IFlyoutService>();
      var mahAppsService = serviceLocator.ResolveType<IMahAppsService>();
      var appConfigService = serviceLocator.ResolveType<IApplicationConfigurationService>();

      // Register
      serviceLocator.RegisterInstance(pleaseWaitProgressBar, "pleaseWaitService");
      serviceLocator.RegisterInstance<IAboutInfoService>(mahAppsService);

      // Theme
      SetupTheme(appConfigService);

      // Flyouts
      SetupFlyouts(flyoutService);

      // Setup window command bar right container
      SetupRightCommands(serviceLocator, mahAppsService, commandManager);

      // Setup status bar
      SetupStatusBar(mahAppsService, statusService);

      // Main content
      var mainView = mahAppsService.GetMainView();
      contentControl.Content = mainView;

      // Title
      SetBinding(TitleProperty,
        new Binding("ViewModel.Title") { Source = mainView });

      // Border
      SetupBorder();
    }

    private void SetupStatusBar(
      IMahAppsService mahAppsService, IStatusService statusService)
    {
      var statusBarContent = mahAppsService.GetStatusBar();

      statusService.Initialize(statusTextBlock);
      if (statusBarContent != null)
      {
        customStatusBarItem.Content = statusBarContent;
      }
    }

    private void SetupRightCommands(
      IServiceLocator serviceLocator, IMahAppsService mahAppsService,
      ICommandManager commandManager)
    {
      var windowCommands = mahAppsService.GetRightWindowCommands();

      if (mahAppsService.GetAboutInfo() != null)
      {
        var aboutWindowCommand = WindowCommandHelper.CreateWindowCommandButton("appbar_information", "about");

        var aboutService = serviceLocator.ResolveType<IAboutService>();
        commandManager.RegisterAction("Help.About", aboutService.ShowAbout);
        aboutWindowCommand.Command = commandManager.GetCommand("Help.About");

        windowCommands.Items.Add(aboutWindowCommand);
      }

      RightWindowCommands = windowCommands;
    }

    private void SetupFlyouts(IFlyoutService flyoutService)
    {
      var flyouts = new FlyoutsControl();
      foreach (var flyout in flyoutService.GetFlyouts())
      {
        flyouts.Items.Add(flyout);
      }

      Flyouts = flyouts;
    }

    private void SetupTheme(IApplicationConfigurationService applicationConfigService)
    {
      AppTheme appTheme = applicationConfigService.Theme;
      Accent accent = applicationConfigService.Accent;

      ThemeManager.ChangeAppStyle(Application.Current, accent, appTheme);

      //var themeService = serviceLocator.ResolveType<IThemeService>();
      //ThemeHelper.EnsureApplicationThemes(GetType().Assembly, themeService.ShouldCreateStyleForwarders());
      //var application = Application.Current;
      //var applicationResources = application.Resources;
      //var resourceDictionary = applicationResources.MergedDictionaries[0];
      //MahAppsHelper.ApplyTheme();
    }

    private void SetupBorder()
    {
      //var accentColorBrush = ThemeHelper.GetAccentColorBrush();
      //border.BorderBrush = accentColorBrush;
    }
  }
}