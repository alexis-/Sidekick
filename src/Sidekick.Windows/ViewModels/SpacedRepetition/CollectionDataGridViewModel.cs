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
  using System.ComponentModel;
  using System.Threading.Tasks;

  using AgnosticDatabase.Interfaces;

  using Catel;
  using Catel.Collections;
  using Catel.MVVM;
  using Catel.Services;

  using Sidekick.MVVM.Models;
  using Sidekick.SpacedRepetition.Models;
  using Sidekick.Windows.Models;
  using Sidekick.WPF.Controls;

  /// <summary>Displays card collection in a DataGrid.</summary>
  /// <seealso cref="Catel.MVVM.ViewModelBase" />
  public class CollectionDataGridViewModel : ViewModelBase, ISortController
  {
    #region Fields

    private readonly IDatabaseAsync _db;
    private readonly IPleaseWaitService _pleaseWaitService;
    private SortDescriptionCollection _sortDescriptions;
    private Task _sortTask;

    #endregion



    #region Constructors

    /// <summary>Initializes a new instance of the <see cref="CollectionDataGridViewModel" /> class.</summary>
    /// <param name="query">Optional collection query</param>
    /// <param name="db">Database instance.</param>
    /// <param name="pleaseWaitService">The please wait service.</param>
    public CollectionDataGridViewModel(
      CollectionQuery query, IDatabaseAsync db, IPleaseWaitService pleaseWaitService)
      : base(false)
    {
      Argument.IsNotNull(() => db);
      Argument.IsNotNull(() => pleaseWaitService);

      _db = db;
      _pleaseWaitService = pleaseWaitService;

      Query = query;
      PageableCollection = new DbPageableCollection<Card>(_db, FilterCollection);
    }

    #endregion



    #region Properties

    /// <summary>Gets or sets the pageable collection.</summary>
    [Model]
    public PageableCollectionBase<Card> PageableCollection { get; set; }

    /// <summary>Gets or sets the filtered collection.</summary>
    [ViewModelToModel("PageableCollection", "CurrentPageItems")]
    public FastObservableCollection<Card> FilteredCollection { get; set; }

    /// <summary>Gets or sets the size of the page.</summary>
    /// <value>The size of the page.</value>
    [ViewModelToModel("PageableCollection", "PageSize")]
    public int PageSize { get; set; }

    /// <summary>Gets or sets the selected page.</summary>
    [ViewModelToModel("PageableCollection", "CurrentPage")]
    public int SelectedPage { get; set; }

    /// <summary>Gets or sets the total page count.</summary>
    [ViewModelToModel("PageableCollection", "TotalPageCount")]
    public int TotalPageCount { get; set; }

    /// <summary>Gets or sets the query.</summary>
    public CollectionQuery Query { get; set; }

    /// <summary>Gets or sets the DataGrid sort controller.</summary>
    /// <summary>Called when control sorting is updated.</summary>
    /// <param name="sortDescriptions">The sort descriptions.</param>
    public bool OnSorting(SortDescriptionCollection sortDescriptions)
    {
      _sortDescriptions = sortDescriptions;

#pragma warning disable 4014
      _sortTask = PageableCollection.UpdateCurrentPageItemsAsync()
        .ContinueWith(ret => _sortTask = null);
#pragma warning restore 4014

      return false;
    }

    /// <summary>Determines whether sort is enabled.</summary>
    public bool CanSort()
    {
      return _sortTask == null;
    }

    /// <inheritdoc />
    protected override async Task InitializeAsync()
    {
      try
      {
        _pleaseWaitService.Push("Loading collection");

        await PageableCollection.UpdateCurrentPageItemsAsync().ConfigureAwait(true);
      }
      finally
      {
        _pleaseWaitService.Pop();
      }
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

    private void OnQueryChanged()
    {
      if (PageableCollection != null)
        PageableCollection.GoToFirstPage(true);
    }

    #endregion
  }
}