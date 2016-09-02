using System;
using System.Threading.Tasks;
using Catel;
using Catel.IoC;
using Catel.Logging;
using Catel.MVVM;
using Catel.Runtime.Serialization.Json;
using Catel.Services;
using Catel.Threading;
using Catel.Windows.Controls;
using MethodTimer;
using Orchestra.Services;
using Sidekick.MVVM.ViewModels.SpacedRepetition;
using Sidekick.Shared.Interfaces.Database;
using Sidekick.SpacedRepetition;
using Sidekick.SpacedRepetition.Interfaces;
using Sidekick.Windows.Services.Interfaces;

namespace Sidekick.Windows.Services.Initialization
{

  public class ApplicationInitializationService
    : ApplicationInitializationServiceBase
  {
    #region Fields

    private static readonly ILog Log = LogManager.GetCurrentClassLogger();
    private readonly IServiceLocator _serviceLocator;
    private readonly ICommandManager _commandManager;

    #endregion


    //
    // Constructors
    #region Constructors

    public ApplicationInitializationService(
      IServiceLocator serviceLocator,
      ICommandManager commandManager)
    {
      Argument.IsNotNull(() => serviceLocator);
      Argument.IsNotNull(() => commandManager);
      
      _serviceLocator = serviceLocator;
      _commandManager = commandManager;
    }

    #endregion


    //
    // Core methods
    #region Methods

    //
    // States override

    public override async Task InitializeBeforeCreatingShellAsync()
    {
      // Non-async first
      RegisterTypes();
      PreloadTypes();
      InitializeConfiguration();
      InitializeCommands();
      InitializeViewPaths();
      InitializeViewModelPaths();
      //InitializeNavigationMenu();

      await TaskHelper.RunAndWaitAsync(
        InitializeDatabase,
        InitializePerformance);
    }

    public override async Task InitializeAfterCreatingShellAsync()
    {
      await base.InitializeAfterCreatingShellAsync();
    }



    //
    // Sync init

    private void RegisterTypes()
    {
      _serviceLocator.RegisterType<
        IConfigurationInitializationService,
        ConfigurationInitializationService>();
      _serviceLocator.RegisterType<
        IApplicationConfigurationService,
        ApplicationConfigurationService>();

      //_serviceLocator.RegisterTypeAndInstantiate<NavigationMenuService>();
      
      _serviceLocator.RegisterType<IDatabase>(
        slr => new DatabaseService());
    }

    private void PreloadTypes()
    {
      var tmp = ServiceLocator.Default.ResolveType<IJsonSerializer>();
    }

    private void InitializeConfiguration()
    {
      var configInitService =
        _serviceLocator.ResolveType<IConfigurationInitializationService>();

      configInitService.Initialize();
    }

    private void InitializeCommands()
    {
      var commandManager = _serviceLocator.ResolveType<ICommandManager>();
      
      //commandManager.CreateCommand("File.Refresh", new InputGesture(Key.R, ModifierKeys.Control),
      //  throwExceptionWhenCommandIsAlreadyCreated: false);
      //commandManager.CreateCommand("File.Save", new InputGesture(Key.S, ModifierKeys.Control),
      //  throwExceptionWhenCommandIsAlreadyCreated: false);
      //commandManager.CreateCommand("File.Exit", throwExceptionWhenCommandIsAlreadyCreated: false);
    }

    private void InitializeViewPaths()
    {
      //
      // ViewModels
      IViewLocator viewLocator =
        _serviceLocator.ResolveType<IViewLocator>();
      
      viewLocator.NamingConventions.Add(
        "Sidekick.Windows.Views.DataTemplates.[VM]View");
      viewLocator.NamingConventions.Add(
        "Sidekick.Windows.Views.Settings.[VM]View");
      viewLocator.NamingConventions.Add(
        "Sidekick.Windows.Views.SpacedRepetition.[VM]View");
      viewLocator.NamingConventions.Add(
        "Sidekick.WPF.Views.DataTemplates.[VM]View");
    }

    private void InitializeViewModelPaths()
    {
      //
      // ViewModels
      IViewModelLocator viewModelLocator =
        _serviceLocator.ResolveType<IViewModelLocator>();
      
      viewModelLocator.NamingConventions.Add(
        "Sidekick.Windows.ViewModels.DataTemplates.[VW]ViewModel");
      viewModelLocator.NamingConventions.Add(
        "Sidekick.Windows.ViewModels.Settings.[VW]ViewModel");
      viewModelLocator.NamingConventions.Add(
        "Sidekick.Windows.ViewModels.SpacedRepetition.[VW]ViewModel");

      viewModelLocator.NamingConventions.Add(
        "Sidekick.MVVM.ViewModels.[VW]ViewModel");
      viewModelLocator.NamingConventions.Add(
        "Sidekick.MVVM.ViewModels.SpacedRepetition.[VW]ViewModel");
    }

#if false
    private void InitializeNavigationMenu()
    {
      var languageService = _serviceLocator.ResolveType<ILanguageService>();
      NavigationMenuService menuService =
        _serviceLocator.ResolveType<NavigationMenuService>();

      menuService.Add(
        languageService.GetString("Main_Navigation_Collection_Button"),
        "appbar_card", 0, typeof(CollectionViewModel));
      menuService.Add(
        languageService.GetString("Main_Navigation_KnowledgeNetwork_Button"),
        "appbar_share", 10, null);
    }
#endif


    //
    // Async init

    [Time]
    private async Task InitializeDatabase()
    {
      await Task.Run(() => _serviceLocator.ResolveType<IDatabase>());
    }

    [Time]
    private async Task InitializePerformance()
    {
      //Catel.Data.ModelBase.DefaultSuspendValidationValue = true;
      //UserControl.DefaultCreateWarningAndErrorValidatorForViewModelValue = false;
      UserControl.DefaultSkipSearchingForInfoBarMessageControlValue = true;
    }

#endregion
  }
}