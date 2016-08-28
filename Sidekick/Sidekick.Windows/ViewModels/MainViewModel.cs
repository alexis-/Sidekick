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

using System.Threading.Tasks;
using Catel.IoC;
using Catel.MVVM;
using Catel.Services;
using Sidekick.MVVM.ViewModels;
using Sidekick.MVVM.ViewModels.SpacedRepetition;
using Sidekick.Shared.Utils;

namespace Sidekick.Windows.ViewModels
{
  public class MainViewModel : ViewModelBase
  {
    #region Fields

    private CollectionViewModel _collectionViewModel;
    private SettingsViewModel _settingsViewModel;

    #endregion

    #region Constructors

    //
    // Constructors
    public MainViewModel(ILanguageService languageService)
    {
      Title = languageService.GetString("App_Title");

      // Commands
      ShowCollection = new Command(OnShowCollectionExecute);
      ShowSettings = new Command(OnShowSettingsExecute);
    }

    #endregion

    #region Properties

    //
    // Properties

    public MainContentViewModelBase CurrentModel { get; set; }

    #endregion

    #region Methods

    //
    // Methods

    private void SetCurrentModel(MainContentViewModelBase viewModel)
    {
      if (CurrentModel == viewModel)
        return;

      Task<bool> allowChangeTask =
        CurrentModel != null
          ? CurrentModel.OnContentChange()
          : TaskConstants.BooleanTrue;

      if (allowChangeTask.IsCompleted)
        CurrentModel = viewModel;

      else
        SetCurrentModelAsync(viewModel, allowChangeTask);
    }

    private async void SetCurrentModelAsync(
      MainContentViewModelBase viewModel,
      Task<bool> allowChangeTask)
    {
      if (await allowChangeTask)
        CurrentModel = viewModel;
    }


    //
    // Commands

    public Command ShowSettings { get; set; }

    private void OnShowSettingsExecute()
    {
      SetCurrentModel(
        _settingsViewModel
        ?? (_settingsViewModel = new SettingsViewModel()));
    }

    public Command ShowCollection { get; set; }

    private void OnShowCollectionExecute()
    {
      SetCurrentModel(
        _collectionViewModel
        ?? (_collectionViewModel =
            TypeFactory.Default.CreateInstance<CollectionViewModel>()));
    }

    #endregion
  }
}