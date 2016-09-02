// 
// The MIT License (MIT)
// Copyright (c) 2016 Incogito
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

using System.Collections.Generic;
using System.Threading.Tasks;
using Catel;
using Catel.IoC;
using Catel.MVVM;
using Catel.Services;
using Sidekick.MVVM.ViewModels;
using Sidekick.MVVM.ViewModels.SpacedRepetition;
using Sidekick.Shared.Extensions;
using Sidekick.Shared.Utils;
using Sidekick.SpacedRepetition.Models;
using Sidekick.Windows.Models;
using Sidekick.Windows.Services;
using Sidekick.Windows.ViewModels.SpacedRepetition;
using Sidekick.WPF.Controls;

namespace Sidekick.Windows.ViewModels
{
  public class MainViewModel : ViewModelBase
  {
    #region Fields

    private readonly NavigationMenuService _menuService;

    private readonly Dictionary<string,
      MainContentViewModelBase> _navViewModels;

    #endregion

    #region Constructors

    //
    // Constructors
    public MainViewModel(
      ILanguageService languageService, NavigationMenuService menuService)
    {
      _menuService = menuService;
      _navViewModels =
        new Dictionary<string, MainContentViewModelBase>();

      Title = languageService.GetString("App_Title");
      MenuController = new RadioController();
    }

    #endregion

    #region Properties

    //
    // Properties

    public MainContentViewModelBase CurrentModel { get; set; }

    public RadioController MenuController { get; set; }

    #endregion

    #region Methods

    //
    // Methods

    protected override Task InitializeAsync()
    {
      MenuController.OnSelectionChanged += MenuControllerOnOnSelectionChanged;
      MenuControllerOnOnSelectionChanged(null, "NavigationBrowseButton");

      return base.InitializeAsync();
    }

    private Task<bool> MenuControllerOnOnSelectionChanged(
      object selectedItem, object parameter)
    {
      Argument.IsNotNull(() => parameter);
      Argument.IsOfType("parameter", parameter, typeof(string));

      string buttonName = (string)parameter;

      switch (buttonName)
      {
        case "NavigationCollectionButton":
          return SetCurrentModel(_navViewModels.GetOrAdd(
            buttonName,
            () => TypeFactory.Default.CreateInstance<CollectionViewModel>()));

        case "NavigationBrowseButton":
          return SetCurrentModel(_navViewModels.GetOrAdd(
            buttonName,
            () => TypeFactory.Default.CreateInstance<BrowserViewModel>()));

        case "NavigationKnowledgeNetworkButton":
          //return SetCurrentModel(_navViewModels.GetOrAdd(
          //  buttonName,
          //  () => TypeFactory.Default.CreateInstanceWithParameters<QueryBuilderViewModel>(typeof(Card))));
          return TaskConstants.BooleanFalse;

        case "SettingsButton":
          return SetCurrentModel(_navViewModels.GetOrAdd(
            buttonName,
            () => TypeFactory.Default.CreateInstance<SettingsViewModel>()));
      }

      return TaskConstants.BooleanFalse;
    }

    private Task<bool> SetCurrentModel(MainContentViewModelBase viewModel)
    {
      if (CurrentModel == viewModel)
        return TaskConstants.BooleanFalse;

      Task<bool> allowChangeTask =
        CurrentModel != null
          ? CurrentModel.OnContentChange()
          : TaskConstants.BooleanTrue;

      if (allowChangeTask.IsCompleted && allowChangeTask.Result)
        CurrentModel = viewModel;

      else
        SetCurrentModelAsync(viewModel, allowChangeTask);

      return allowChangeTask;
    }

    private async void SetCurrentModelAsync(
      MainContentViewModelBase viewModel,
      Task<bool> allowChangeTask)
    {
      if (await allowChangeTask)
        CurrentModel = viewModel;
    }

    #endregion
  }
}