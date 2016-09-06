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

using System.Threading.Tasks;
using Catel;
using Catel.IoC;
using Catel.Logging;
using Catel.MVVM;
using Catel.Runtime.Serialization.Json;
using Catel.Threading;
using Catel.Windows.Controls;
using MethodTimer;
using Orchestra.Services;
using Sidekick.Shared.Interfaces.Database;
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