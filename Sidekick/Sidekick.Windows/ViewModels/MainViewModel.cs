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

namespace Sidekick.Windows.ViewModels
{
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
  using Sidekick.Windows.ViewModels.SpacedRepetition;
  using Sidekick.WPF.Controls;

  /// <summary>
  ///   ViewModel for Main view directly under MainWindow.
  ///   Controls left menu and displaying main navigation.
  /// </summary>
  /// <seealso cref="Catel.MVVM.ViewModelBase" />
  /// <seealso cref="Sidekick.WPF.Controls.IRadioControllerMonitor" />
  public sealed class MainViewModel : ViewModelBase, IRadioControllerMonitor
  {
    #region Fields

    private readonly Dictionary<string, MainContentViewModelBase> _navViewModels;

    #endregion



    #region Constructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="MainViewModel" /> class.
    ///   Setup Title and navigation control.
    /// </summary>
    /// <param name="languageService">
    ///   Language service used to setup Title
    /// </param>
    public MainViewModel(ILanguageService languageService)
    {
      _navViewModels = new Dictionary<string, MainContentViewModelBase>();

      Title = languageService.GetString("App_Title");
      MenuController = new RadioController(this);
    }

    #endregion



    #region Properties

    /// <summary>
    ///   Displayed ViewModel.
    /// </summary>
    public MainContentViewModelBase CurrentModel { get; set; }

    /// <summary>
    ///   Navigation menu controller.
    /// </summary>
    public RadioController MenuController { get; set; }

    #endregion



    #region Methods

    /// <summary>
    ///   Notify on selection changes and allows to asynchronously validate whether to endorse it
    ///   or not.
    /// </summary>
    /// <param name="selectedItem">The selected item.</param>
    /// <param name="parameter">Item context (e.g. binding).</param>
    /// <returns>Waitable task for validation result.</returns>
    public Task<bool> RadioControllerOnSelectionChangedAsync(
      object selectedItem, object parameter)
    {
      Argument.IsNotNull(() => parameter);
      Argument.IsOfType("parameter", parameter, typeof(string));

      string buttonName = (string)parameter;

      switch (buttonName)
      {
        case "NavigationCollectionButton":
          return
            SetCurrentModelAsync(
              _navViewModels.GetOrAdd(
                buttonName, () => TypeFactory.Default.CreateInstance<CollectionViewModel>()));

        case "NavigationBrowseButton":
          return
            SetCurrentModelAsync(
              _navViewModels.GetOrAdd(
                buttonName, () => TypeFactory.Default.CreateInstance<BrowserViewModel>()));

        case "NavigationKnowledgeNetworkButton":
          // return SetCurrentModelAsync(_navViewModels.GetOrAdd(
          // buttonName,
          // () => TypeFactory.Default.CreateInstanceWithParameters<QueryBuilderViewModel>(typeof(Card))));
          return TaskConstants.BooleanFalse;

        case "SettingsButton":
          return
            SetCurrentModelAsync(
              _navViewModels.GetOrAdd(
                buttonName, () => TypeFactory.Default.CreateInstance<SettingsViewModel>()));
      }

      return TaskConstants.BooleanFalse;
    }

    /// <inheritdoc />
    protected override Task InitializeAsync()
    {
      RadioControllerOnSelectionChangedAsync(null, "NavigationCollectionButton");

      return base.InitializeAsync();
    }

    private Task<bool> SetCurrentModelAsync(MainContentViewModelBase viewModel)
    {
      if (CurrentModel == viewModel)
        return TaskConstants.BooleanFalse;

      Task<bool> allowChangeTask = CurrentModel != null
                                     ? CurrentModel.OnContentChange()
                                     : TaskConstants.BooleanTrue;

      if (allowChangeTask.IsCompleted && allowChangeTask.Result)
        CurrentModel = viewModel;
      else
        SetCurrentModelAsync(viewModel, allowChangeTask);

      return allowChangeTask;
    }

    private async void SetCurrentModelAsync(
      MainContentViewModelBase viewModel, Task<bool> allowChangeTask)
    {
      if (await allowChangeTask.ConfigureAwait(true))
        CurrentModel = viewModel;
    }

    #endregion
  }
}