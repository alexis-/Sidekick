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
using Catel.Collections;
using Catel.MVVM;
using Catel.Services;
using Sidekick.MVVM.Models;
using Sidekick.Shared.Interfaces.Database;
using Sidekick.SpacedRepetition.Models;
using Sidekick.Windows.Models;

namespace Sidekick.Windows.ViewModels.SpacedRepetition
{
  public class CollectionDataGridViewModel : ViewModelBase
  {
    #region Fields

    private readonly IDatabase _db;
    private readonly IPleaseWaitService _pleaseWaitService;
    private readonly CollectionFilter _filter;

    #endregion

    #region Constructors

    public CollectionDataGridViewModel(
      IDatabase db, IPleaseWaitService pleaseWaitService)
      : this(db, pleaseWaitService, null)
    {
    }

    public CollectionDataGridViewModel(
      IDatabase db, IPleaseWaitService pleaseWaitService,
      CollectionFilter filter)
      : base(false)
    {
      _db = db;
      _pleaseWaitService = pleaseWaitService;
      _filter = filter;

      PageableCollection = new DbPageableCollection<Card>(
        _db, FilterCollection);
    }

    #endregion

    #region Properties

    [Model]
    public PageableCollectionBase<Card> PageableCollection { get; set; }

    [ViewModelToModel("PageableCollection", "CurrentPageItems")]
    public FastObservableCollection<Card> FilteredCollection { get; set; }

    [ViewModelToModel("PageableCollection", "PageSize")]
    public int PageSize { get; set; }

    [ViewModelToModel("PageableCollection", "CurrentPage")]
    public int SelectedPage { get; set; }

    [ViewModelToModel("PageableCollection", "TotalPageCount")]
    public int TotalPageCount { get; set; }

    #endregion

    #region Methods

    protected override async Task InitializeAsync()
    {
      _pleaseWaitService.Push("Loading collection");

      await PageableCollection.UpdateCurrentPageItemsAsync();

      _pleaseWaitService.Pop();
    }

    private ITableQuery<Card> FilterCollection(ITableQuery<Card> query)
    {
      return _filter != null
               ? _filter.Apply(query)
               : query;
    }

    #endregion
  }
}