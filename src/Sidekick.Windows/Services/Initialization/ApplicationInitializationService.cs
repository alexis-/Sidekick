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

namespace Sidekick.Windows.Services.Initialization
{
  using System.Linq;
  using System.Threading.Tasks;
  using System.Windows;

  using AgnosticDatabase.Interfaces;

  using Anotar.Catel;

  using Catel;
  using Catel.IoC;
  using Catel.MVVM;
  using Catel.Services;
  using Catel.Threading;
  using Catel.Windows.Controls;

  using MahApps.Metro;

  using MethodTimer;

  using Orchestra.Services;

  using Sidekick.Windows.Services.Database;
  using Sidekick.Windows.Services.Interfaces;
  using Sidekick.Windows.ViewModels.SpacedRepetition;
  using Sidekick.Windows.Views.SpacedRepetition;

  /// <summary>Core initialization methods.</summary>
  /// <seealso cref="Orchestra.Services.ApplicationInitializationServiceBase" />
  public class ApplicationInitializationService : ApplicationInitializationServiceBase
  {
    #region Fields

    private readonly IServiceLocator _serviceLocator;
    private readonly ICommandManager _commandManager;
    private readonly IUIVisualizerService _uiVisualizer;

    #endregion



    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationInitializationService" />
    /// class.
    /// </summary>
    /// <param name="serviceLocator">The service locator.</param>
    /// <param name="commandManager">The command manager.</param>
    /// <param name="uiVisualizer">The UI visualizer.</param>
    public ApplicationInitializationService(
      IServiceLocator serviceLocator, ICommandManager commandManager,
      IUIVisualizerService uiVisualizer)
    {
      Argument.IsNotNull(() => serviceLocator);
      Argument.IsNotNull(() => commandManager);

      _serviceLocator = serviceLocator;
      _commandManager = commandManager;
      _uiVisualizer = uiVisualizer;
    }

    #endregion



    #region Methods

    /// <summary>Initializes the before showing splash screen asynchronous.</summary>
    /// <returns></returns>
    public override Task InitializeBeforeShowingSplashScreenAsync()
    {
      RegisterTypes();
      InitializeTheme();

      return base.InitializeBeforeShowingSplashScreenAsync();
    }

    /// <summary>Initializes the before creating shell asynchronous.</summary>
    /// <returns></returns>
    public override async Task InitializeBeforeCreatingShellAsync()
    {
      // Non-async first
      InitializeConfiguration();
      InitializeCommands();
      InitializeViewPaths();
      InitializeViewModelPaths();
      InitializeWindows();
      InitializePerformance();

      // First initialize database
      await InitializeDatabaseAsync().ConfigureAwait(false);

      await
        TaskHelper.RunAndWaitAsync(InitializeCollectionFilterManagerAsync).ConfigureAwait(false);
    }

    /// <summary>Initializes the after creating shell asynchronous.</summary>
    /// <returns></returns>
    public override async Task InitializeAfterCreatingShellAsync()
    {
      await base.InitializeAfterCreatingShellAsync().ConfigureAwait(false);
    }


    //
    // Sync init

    private void RegisterTypes()
    {
      _serviceLocator
        .RegisterType<IConfigurationInitializationService, ConfigurationInitializationService>();
      _serviceLocator
        .RegisterType<IApplicationConfigurationService, ApplicationConfigurationService>();

      _serviceLocator.RegisterType<IDatabaseAsync, DatabaseAsyncService>();
      _serviceLocator.RegisterType<CollectionQueryManagerService>();
    }

    private void InitializeConfiguration()
    {
      var configInitService = _serviceLocator.ResolveType<IConfigurationInitializationService>();

      configInitService.Initialize();
    }

    private void InitializeCommands()
    {
      var commandManager = _serviceLocator.ResolveType<ICommandManager>();

      // Spaced Repetition collection review shortcuts (1 - 5 answer keys)
      foreach (var answerGesture in Commands.SpacedRepetition.CollectionReview.AnswerGestures)
        commandManager.CreateCommand(
          answerGesture.Item1, answerGesture.Item2,
          throwExceptionWhenCommandIsAlreadyCreated: false);

      // Search command (search toolbar icon)
      commandManager.CreateCommand(Commands.General.Search, Commands.General.SearchInputGesture);

      // commandManager.CreateCommand("File.Refresh", new InputGesture(Key.R, ModifierKeys.Control),
      //   throwExceptionWhenCommandIsAlreadyCreated: false);
      // commandManager.CreateCommand("File.Save", new InputGesture(Key.S, ModifierKeys.Control),
      //   throwExceptionWhenCommandIsAlreadyCreated: false);
      // commandManager.CreateCommand("File.Exit", throwExceptionWhenCommandIsAlreadyCreated: false);
    }

    private void InitializeViewPaths()
    {
      //
      // ViewModels
      IViewLocator viewLocator = _serviceLocator.ResolveType<IViewLocator>();

      viewLocator.NamingConventions.Add("Sidekick.Windows.Views.DataTemplates.[VM]View");
      viewLocator.NamingConventions.Add("Sidekick.Windows.Views.Settings.[VM]View");
      viewLocator.NamingConventions.Add("Sidekick.Windows.Views.SpacedRepetition.[VM]View");
      viewLocator.NamingConventions.Add("Sidekick.WPF.Views.DataTemplates.[VM]View");
    }

    private void InitializeViewModelPaths()
    {
      //
      // ViewModels
      IViewModelLocator viewModelLocator = _serviceLocator.ResolveType<IViewModelLocator>();

      viewModelLocator.NamingConventions.Add(
                        "Sidekick.Windows.ViewModels.DataTemplates.[VW]ViewModel");
      viewModelLocator.NamingConventions.Add(
                        "Sidekick.Windows.ViewModels.Settings.[VW]ViewModel");
      viewModelLocator.NamingConventions.Add(
                        "Sidekick.Windows.ViewModels.SpacedRepetition.[VW]ViewModel");

      viewModelLocator.NamingConventions.Add("Sidekick.MVVM.ViewModels.[VW]ViewModel");
      viewModelLocator.NamingConventions.Add(
                        "Sidekick.MVVM.ViewModels.SpacedRepetition.[VW]ViewModel");
    }

    private void InitializeWindows()
    {
      _uiVisualizer.Register<BrowserViewModel, BrowserWindow>();
    }

    private void InitializePerformance()
    {
      // Catel.Data.ModelBase.DefaultSuspendValidationValue = true;
      // UserControl.DefaultCreateWarningAndErrorValidatorForViewModelValue = false;
      UserControl.DefaultSkipSearchingForInfoBarMessageControlValue = true;
    }

    private void InitializeTheme()
    {
      var appConfigService = _serviceLocator.ResolveType<IApplicationConfigurationService>();

      SidekickThemeManager.Initialize();

      AppTheme appTheme = appConfigService.Theme;
      Accent accent = appConfigService.Accent;

      if (appTheme == null)
      {
        appTheme = SidekickThemeManager.AppThemes.FirstOrDefault();
        LogTo.Error("AppTheme is NULL, reverting to default.");
      }

      if (accent == null)
      {
        accent = SidekickThemeManager.Accents.FirstOrDefault();
        LogTo.Error("Accent is NULL, reverting to default.");
      }

      ThemeManager.ChangeAppStyle(Application.Current, accent, appTheme);
    }

#if false
    private void InitializeNavigationMenu()
    {
      var languageService = _serviceLocator.ResolveType<ILanguageService>();
      NavigationMenuService menuService =
        _serviceLocator.ResolveType<NavigationMenuService>();

      menuService.Add(
        languageService.GetString("Main_Navigation_Collection_Button"),
        "appbar_card", 0, typeof(ReviewViewModel));
      menuService.Add(
        languageService.GetString("Main_Navigation_KnowledgeNetwork_Button"),
        "appbar_share", 10, null);
    }
#endif


    //
    // Async init

    [Time]
    private async Task InitializeDatabaseAsync()
    {
      await Task.Run(() => _serviceLocator.ResolveType<IDatabaseAsync>()).ConfigureAwait(false);
    }

    [Time]
    private async Task InitializeCollectionFilterManagerAsync()
    {
      var filterManager = _serviceLocator.ResolveType<CollectionQueryManagerService>();

      await filterManager.InitializeAsync().ConfigureAwait(false);
    }

    #endregion
  }
}