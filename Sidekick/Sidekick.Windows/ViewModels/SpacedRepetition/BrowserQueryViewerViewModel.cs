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
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Linq;
  using System.Threading.Tasks;
  using System.Windows.Input;

  using Catel;
  using Catel.MVVM;
  using Catel.Services;

  using Orc.FilterBuilder.Models;

  using Sidekick.Shared.Interfaces.Database;
  using Sidekick.Windows.Models;
  using Sidekick.Windows.Services;

  /// <summary>
  ///   Link CollectionDataGridView[Model], queries and other controls
  /// </summary>
  /// <seealso cref="Catel.MVVM.ViewModelBase" />
  public class BrowserQueryViewerViewModel : ViewModelBase
  {
    #region Fields

    private readonly IDatabaseAsync _db;
    private readonly IPleaseWaitService _pleaseWaitService;
    private readonly CollectionQueryManagerService _collectionQueryManagerService;

    #endregion



    #region Constructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="BrowserQueryViewerViewModel" /> class.
    /// </summary>
    /// <param name="db">The database.</param>
    /// <param name="pleaseWaitService">The please wait service.</param>
    /// <param name="collectionQueryManagerService">Collection query manager.</param>
    public BrowserQueryViewerViewModel(
      IPleaseWaitService pleaseWaitService,
      CollectionQueryManagerService collectionQueryManagerService)
      : base(false)
    {
      Argument.IsNotNull(() => pleaseWaitService);
      Argument.IsNotNull(() => collectionQueryManagerService);
      
      _pleaseWaitService = pleaseWaitService;
      _collectionQueryManagerService = collectionQueryManagerService;

      AddQueryCommand = new Command(OnAddQueryCommandExecute, tag: "AddQuery");
    }

    #endregion



    #region Properties

    /// <summary>
    ///   All created queries.
    /// </summary>
    [Model]
    public CollectionQueries CollectionQueries { get; set; }

    /// <summary>
    ///   Available filter queries.
    /// </summary>
    [ViewModelToModel("CollectionQueries", "Queries")]
    public ObservableCollection<CollectionQuery> Queries { get; set; }

    /// <summary>
    ///   Current filter query used to filter content displayed in the DataGrid.
    /// </summary>
    public FilterScheme SelectedQuery { get; set; }

    /// <summary>
    ///   Displays query builder view on Add Query button press.
    /// </summary>
    public ICommand AddQueryCommand { get; set; }

    #endregion



    #region Methods

    /// <inheritdoc />
    protected override async Task InitializeAsync()
    {
      _pleaseWaitService.Push("Loading");

      await _collectionQueryManagerService.InitializeAsync().ConfigureAwait(false);

      CollectionQueries = _collectionQueryManagerService.CollectionQueries;
      SelectedQuery = Queries.FirstOrDefault();

      _pleaseWaitService.Pop();
    }

    private void OnAddQueryCommandExecute() { }

    #endregion
  }
}