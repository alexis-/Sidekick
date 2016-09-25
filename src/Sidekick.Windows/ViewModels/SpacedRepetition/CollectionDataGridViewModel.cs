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

// ReSharper disable PossibleNullReferenceException

namespace Sidekick.Windows.ViewModels.SpacedRepetition
{
  using System.ComponentModel;
  using System.Threading.Tasks;
  using System.Windows.Input;

  using AgnosticDatabase.Interfaces;

  using Catel;
  using Catel.MVVM;
  using Catel.Services;

  using Sidekick.SpacedRepetition.Models;
  using Sidekick.Windows.Helper;
  using Sidekick.Windows.Models;

  using Syncfusion.UI.Xaml.Controls.DataPager;

  /// <summary>Displays card collection in a DataGrid.</summary>
  /// <seealso cref="Catel.MVVM.ViewModelBase" />
  public class CollectionDataGridViewModel : ViewModelBase
  {
    #region Fields

    /// <summary>
    ///   Gets or sets the sf data pager. Terrible hack to circumvent some limitations of
    ///   SfDataPager
    /// </summary>
    // ReSharper disable once StyleCop.SA1401
    internal SfDataPager SfDataPager;

    private readonly IDatabaseAsync _db;
    private readonly IPleaseWaitService _pleaseWaitService;
    private SortDescriptionCollection _sortDescriptions;

    #endregion



    #region Constructors

    /// <summary>Initializes a new instance of the <see cref="CollectionDataGridViewModel" /> class.</summary>
    /// <param name="query">Optional collection query</param>
    /// <param name="db">Database instance.</param>
    /// <param name="pleaseWaitService">The please wait service.</param>
    /// <param name="commandManager">The command manager.</param>
    public CollectionDataGridViewModel(
      CollectionQuery query, IDatabaseAsync db, IPleaseWaitService pleaseWaitService,
      ICommandManager commandManager) : base(false)
    {
      Argument.IsNotNull(() => db);
      Argument.IsNotNull(() => pleaseWaitService);

      _db = db;
      _pleaseWaitService = pleaseWaitService;
      
      Query = query;
      DataPager = new DatabaseDataPager<Card>(FilterCollection, 100, db);

      // Search command
      SearchCommand = new Command(() => IsDataGridSearchVisible = !IsDataGridSearchVisible);
      commandManager.RegisterCommand(Commands.General.Search, SearchCommand, this);
    }

    #endregion



    #region Properties

    /// <summary>Gets or sets the data pager.</summary>
    [Model]
    public DatabaseDataPager<Card> DataPager { get; set; }

    /// <summary>Gets or sets the size of the page.</summary>
    [ViewModelToModel("DataPager", "PageSize")]
    public int PageSize { get; set; }

    /// <summary>Gets or sets the total page count.</summary>
    [ViewModelToModel("DataPager", "PageCount")]
    public int PageCount { get; set; }

    /// <summary>Gets or sets the query.</summary>
    [Model]
    public CollectionQuery Query { get; set; }

    /// <summary>Gets or sets a value indicating whether the data grid search control is visible.</summary>
    public bool IsDataGridSearchVisible { get; set; } = false;

    /// <summary>Gets or sets the search command.</summary>
    public ICommand SearchCommand { get; set; }

    #endregion



    #region Methods

    /// <summary>Called when sorting columns.</summary>
    /// <param name="sfDataPager">The sf data pager.</param>
    public void OnSortColumnsExecute(SfDataPager sfDataPager)
    {
      sfDataPager.PagedSource.ResetCache();
      sfDataPager.PagedSource.ResetCacheForPage(SfDataPager.PageIndex);

      _sortDescriptions = sfDataPager.PagedSource.SortDescriptions;

      sfDataPager.MoveToPage(SfDataPager.PageIndex);
    }

    /// <summary>Called when loading is required.</summary>
    /// <param name="sfDataPager">The sf data pager.</param>
    /// <param name="args">
    ///   The
    ///   <see cref="Syncfusion.UI.Xaml.Controls.DataPager.OnDemandLoadingEventArgs" /> instance
    ///   containing the event data.
    /// </param>
    public async void OnDemandLoadExecuteAsync(
      SfDataPager sfDataPager, OnDemandLoadingEventArgs args)
    {
      // Fix for SfDataPager calling on close...
      if (IsClosed)
        return;

      var items = await DataPager.LoadPageAsync(args.StartIndex, PageSize).ConfigureAwait(true);

      sfDataPager.LoadDynamicItems(args.StartIndex, items);
      sfDataPager.PagedSource.Refresh();
    }

    /// <inheritdoc />
    protected override async Task InitializeAsync()
    {
      try
      {
        _pleaseWaitService.Push("Loading collection");

        await DataPager.ResetAsync().ConfigureAwait(true);

        // Fix inconsistent SfDataPager behavior....
        if (SfDataPager.PageSize != 0)
        {
          SfDataPager.DataContext = this;
          SfDataPager.GetBindingExpression(SfDataPager.PageCountProperty).UpdateTarget();
          SfDataPager.GetBindingExpression(SfDataPager.PageSizeProperty).UpdateTarget();

          SfDataPager.MoveToFirstPage();
        }
      }
      finally
      {
        _pleaseWaitService.Pop();
      }
    }

    /// <inheritdoc />
    protected override Task CloseAsync()
    {
      SfDataPager = null;

      return base.CloseAsync();
    }

    private ITableQueryAsync<Card> FilterCollection(ITableQueryAsync<Card> query)
    {
      if (Query != null && Query.IsExpressionValid)
        query = Query.Apply(query);

      if (_sortDescriptions != null)
        foreach (var sortDescription in _sortDescriptions)
          query = query.AddOrderBy(
            sortDescription.PropertyName,
            sortDescription.Direction == ListSortDirection.Ascending);

      return query;
    }

    #endregion
  }
}