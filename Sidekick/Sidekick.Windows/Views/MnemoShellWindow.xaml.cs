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

namespace Sidekick.Windows.Views
{
  using System.Windows;
  using System.Windows.Data;

  using Catel.IoC;
  using Catel.MVVM;

  using MahApps.Metro;
  using MahApps.Metro.Controls;

  using Orchestra;
  using Orchestra.Services;
  using Orchestra.Views;
  using Orchestra.Windows;

  using Sidekick.Windows.Services.Interfaces;

  /// <summary>
  ///   Interaction logic for MnemoShellWindow.xaml
  /// </summary>
  public partial class MnemoShellWindow : MetroDataWindow, IShell
  {
    #region Constructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="MnemoShellWindow" /> class.
    /// </summary>
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
      SetBinding(TitleProperty, new Binding("ViewModel.Title") { Source = mainView });

      // Border
      SetupBorder();
    }

    #endregion



    #region Methods

    private void SetupStatusBar(IMahAppsService mahAppsService, IStatusService statusService)
    {
      var statusBarContent = mahAppsService.GetStatusBar();

      statusService.Initialize(statusTextBlock);
      if (statusBarContent != null)
        customStatusBarItem.Content = statusBarContent;
    }

    private void SetupRightCommands(
      IServiceLocator serviceLocator, IMahAppsService mahAppsService,
      ICommandManager commandManager)
    {
      var windowCommands = mahAppsService.GetRightWindowCommands();

      if (mahAppsService.GetAboutInfo() != null)
      {
        var aboutWindowCommand =
          WindowCommandHelper.CreateWindowCommandButton("appbar_information", "about");

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
        flyouts.Items.Add(flyout);

      Flyouts = flyouts;
    }

    private void SetupTheme(IApplicationConfigurationService applicationConfigService)
    {
      AppTheme appTheme = applicationConfigService.Theme;
      Accent accent = applicationConfigService.Accent;

      ThemeManager.ChangeAppStyle(Application.Current, accent, appTheme);

      // var themeService = serviceLocator.ResolveType<IThemeService>();
      // ThemeHelper.EnsureApplicationThemes(GetType().Assembly, themeService.ShouldCreateStyleForwarders());
      // var application = Application.Current;
      // var applicationResources = application.Resources;
      // var resourceDictionary = applicationResources.MergedDictionaries[0];
      // MahAppsHelper.ApplyTheme();
    }

    private void SetupBorder()
    {
      // var accentColorBrush = ThemeHelper.GetAccentColorBrush();
      // border.BorderBrush = accentColorBrush;
    }

    #endregion
  }
}