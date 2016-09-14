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
  using System.Threading.Tasks;

  using Catel;
  using Catel.Collections;
  using Catel.MVVM;
  using Catel.Services;

  using Sidekick.MVVM.Models;
  using Sidekick.Shared.Interfaces.Database;
  using Sidekick.SpacedRepetition.Models;
  using Sidekick.Windows.Models;

  /// <summary>
  ///   Displays card collection in a DataGrid.
  /// </summary>
  /// <seealso cref="Catel.MVVM.ViewModelBase" />
  public class CollectionDataGridViewModel : ViewModelBase
  {
    #region Fields

    private readonly IDatabaseAsync _db;
    private readonly IPleaseWaitService _pleaseWaitService;

    #endregion



    #region Constructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="CollectionDataGridViewModel" /> class.
    /// </summary>
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

    #endregion



    #region Methods

    /// <inheritdoc />
    protected override async Task InitializeAsync()
    {
      _pleaseWaitService.Push("Loading collection");

      await PageableCollection.UpdateCurrentPageItemsAsync().ConfigureAwait(true);

      _pleaseWaitService.Pop();
    }

    private ITableQueryAsync<Card> FilterCollection(ITableQueryAsync<Card> query)
    {
      return Query != null && Query.IsExpressionValid ? Query.Apply(query) : query;
    }

    private void OnQueryChanged()
    {
      if (PageableCollection != null)
        PageableCollection.GoToFirstPage(true);
    }

    #endregion
  }
}