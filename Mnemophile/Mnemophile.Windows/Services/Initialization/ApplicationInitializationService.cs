using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Catel;
using Catel.IoC;
using Catel.Logging;
using Catel.MVVM;
using Catel.Threading;
using Catel.Windows.Controls;
using global::MahApps.Metro.Controls;
using MethodTimer;
using Mnemophile.Interfaces.DB;
using Mnemophile.Interfaces.SRS;
using Mnemophile.SRS.Impl;
using Mnemophile.Windows.Services.Interfaces;
using Orchestra.Services;
using InputGesture = Catel.Windows.Input.InputGesture;

namespace Mnemophile.Windows.Services
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
      InitializeConfiguration();
      InitializeCommands();
      InitializeViewPaths();
      InitializeViewModelPaths();

      await TaskHelper.RunAndWaitAsync(new Func<Task>[] {
        InitializeDatabase,
        InitializePerformance
      });
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

      // TODO: Load SRS implementation dynamically
      _serviceLocator.RegisterType<ISRS>(
        slr => new SM2Impl());
      _serviceLocator.RegisterType<IDatabase>(
        slr => new DatabaseService());
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
        "[UP].Views.DataTemplates.[VM]View");
      viewLocator.NamingConventions.Add(
        "[UP].Views.Settings.[VM]View");
      viewLocator.NamingConventions.Add(
        "[UP].Views.SpacedRepetition.[VM]View");
    }

    private void InitializeViewModelPaths()
    {
      //
      // ViewModels
      IViewModelLocator viewModelLocator =
        _serviceLocator.ResolveType<IViewModelLocator>();
      
      viewModelLocator.NamingConventions.Add(
        "Mnemophile.Windows.ViewModels.DataTemplates.[VW]ViewModel");
      viewModelLocator.NamingConventions.Add(
        "Mnemophile.Windows.ViewModels.Settings.[VW]ViewModel");
      viewModelLocator.NamingConventions.Add(
        "Mnemophile.Windows.ViewModels.SpacedRepetition.[VW]ViewModel");

      viewModelLocator.NamingConventions.Add(
        "Mnemophile.MVVM.ViewModels.[VW]ViewModel");
      viewModelLocator.NamingConventions.Add(
        "Mnemophile.MVVM.ViewModels.SpacedRepetition.[VW]ViewModel");
    }



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