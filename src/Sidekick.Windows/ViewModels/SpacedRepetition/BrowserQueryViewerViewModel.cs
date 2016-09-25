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

namespace Sidekick.Windows.ViewModels.SpacedRepetition
{
  using System.Collections.ObjectModel;
  using System.Linq;
  using System.Threading.Tasks;
  using System.Windows.Input;

  using Catel;
  using Catel.IoC;
  using Catel.MVVM;
  using Catel.Services;

  using Sidekick.Windows.Models;
  using Sidekick.Windows.Services;

  /// <summary>Link CollectionDataGridView[Model], queries and other controls</summary>
  /// <seealso cref="Catel.MVVM.ViewModelBase" />
  public class BrowserQueryViewerViewModel : ViewModelBase
  {
    #region Fields

    private readonly CollectionQueryManagerService _collectionQueryManagerService;

    private readonly IPleaseWaitService _pleaseWaitService;

    #endregion



    #region Constructors

    /// <summary>Initializes a new instance of the <see cref="BrowserQueryViewerViewModel" /> class.</summary>
    /// <param name="pleaseWaitService">The please wait service.</param>
    /// <param name="collectionQueryManagerService">The collection query manager service.</param>
    public BrowserQueryViewerViewModel(
      IPleaseWaitService pleaseWaitService,
      CollectionQueryManagerService collectionQueryManagerService) : base(false)
    {
      Argument.IsNotNull(() => pleaseWaitService);
      Argument.IsNotNull(() => collectionQueryManagerService);

      _pleaseWaitService = pleaseWaitService;
      _collectionQueryManagerService = collectionQueryManagerService;

      CollectionQueries = _collectionQueryManagerService.CollectionQueries;
      SelectedQuery = Queries.FirstOrDefault();

      AddQueryCommand = new Command(OnAddQueryCommandExecute, tag: "AddQuery");
    }

    #endregion



    #region Properties

    /// <summary>All created queries.</summary>
    [Model]
    public CollectionQueries CollectionQueries { get; set; }

    /// <summary>Available filter queries.</summary>
    [ViewModelToModel("CollectionQueries", "Queries")]
    public ObservableCollection<CollectionQuery> Queries { get; set; }

    /// <summary>Current filter query used to filter content displayed in the DataGrid.</summary>
    public CollectionQuery SelectedQuery { get; set; }

    /// <summary>Gets or sets the collection view model.</summary>
    public CollectionDataGridViewModel CollectionViewModel { get; set; }

    /// <summary>Displays query builder view on Add Query button press.</summary>
    public ICommand AddQueryCommand { get; set; }

    #endregion



    #region Methods

    /// <summary>
    /// Closes this instance. Always called after the <see cref="M:Catel.MVVM.ViewModelBase.Cancel" /> of <see cref="M:Catel.MVVM.ViewModelBase.Save" /> method.
    /// </summary>
    /// <returns></returns>
    protected override Task CloseAsync()
    {
      return CollectionViewModel.CloseViewModelAsync(null);
    }

    private void OnAddQueryCommandExecute() { }

    private async void OnSelectedQueryChanged()
    {
      if (CollectionViewModel != null)
        await CollectionViewModel.CloseViewModelAsync(null).ConfigureAwait(true);
      
      CollectionViewModel =
        TypeFactory.Default.CreateInstanceWithParametersAndAutoCompletion(
                     typeof(CollectionDataGridViewModel), SelectedQuery) as
          CollectionDataGridViewModel;
    }

    #endregion
  }
}