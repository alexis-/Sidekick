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

namespace Sidekick.MVVM.Models
{
  using System;
  using System.Collections.Generic;
  using System.Threading.Tasks;

  using Catel.Collections;
  using Catel.Data;

  /// <summary>
  ///   Base class for collection paging helper
  /// </summary>
  /// <typeparam name="T">Collection items type</typeparam>
  /// <seealso cref="Catel.Data.ModelBase" />
  public abstract class PageableCollectionBase<T> : ModelBase
  {
    #region Fields

    /// <summary>
    ///   Default number of item per page.
    /// </summary>
    protected const int DefaultPageSize = 25;

    #endregion



    #region Constructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="PageableCollectionBase{T}"/> class.
    ///   Sets page size to default value <see cref="DefaultPageSize"/>.
    /// </summary>
    protected PageableCollectionBase() : this(DefaultPageSize) { }

    /// <summary>
    ///   Initializes a new instance of the <see cref="PageableCollectionBase{T}"/> class.
    /// </summary>
    /// <param name="pageSize">Item count per page</param>
    protected PageableCollectionBase(int pageSize)
    {
      HandlePropertyAndCollectionChanges = false;
      DisablePropertyChangeNotifications = true;

      PageSize = pageSize;
      CurrentPage = 1;

      DisablePropertyChangeNotifications = false;
      HandlePropertyAndCollectionChanges = true;
    }

    #endregion



    #region Properties

    /// <summary>
    ///   Item count per page.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    ///   Current page number.
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    ///   Total page count (total item count / page size).
    /// </summary>
    public abstract int TotalPageCount { get; }

    /// <summary>
    ///   Item collection for current page.
    /// </summary>
    public FastObservableCollection<T> CurrentPageItems { get; set; }

    #endregion



    #region Methods

    /// <summary>
    ///   Removes an item from collection/store.
    /// </summary>
    /// <param name="item">Item to remove</param>
    /// <returns></returns>
    public abstract Task<bool> RemoveAsync(T item);

    /// <summary>
    ///   Add an item to collection/store.
    /// </summary>
    /// <param name="item">Item to add</param>
    /// <returns></returns>
    public abstract Task<bool> AddAsync(T item);

    /// <summary>
    ///   Removes items from collection/store.
    /// </summary>
    /// <param name="items">Items to remove</param>
    /// <returns></returns>
    public abstract Task<bool> RemoveAsync(IEnumerable<T> items);

    /// <summary>
    ///   Add items to collection/store.
    /// </summary>
    /// <param name="items">Items to add</param>
    /// <returns></returns>
    public abstract Task<bool> AddAsync(IEnumerable<T> items);

    /// <summary>
    ///   Update page item according to current page.
    /// </summary>
    /// <returns></returns>
    public abstract Task UpdateCurrentPageItemsAsync();

    /// <summary>
    ///   Load next page if available.
    /// </summary>
    public void GoToNextPage()
    {
      if (CurrentPage < TotalPageCount)
        CurrentPage++;
    }

    /// <summary>
    ///   Load previous page if available.
    /// </summary>
    public void GoToPreviousPage()
    {
      if (CurrentPage > 1)
        CurrentPage--;
    }

    /// <summary>Load first page.</summary>
    /// <param name="forceRefresh">Whether to refresh, even if already on first page.</param>
    public void GoToFirstPage(bool forceRefresh)
    {
      if (CurrentPage == 1 && forceRefresh)
        OnCurrentPageChanged();
      
      CurrentPage = 1;
    }

    /// <summary>
    ///   Load last page.
    /// </summary>
    public void GoToLastPage()
    {
      CurrentPage = TotalPageCount;
    }

    /// <summary>
    ///   Called when <see cref="CurrentPage"/> changed and updates page collection,
    ///   <see cref="UpdateCurrentPageItemsAsync"/>
    /// </summary>
    /// <returns>Waitable task</returns>
    // ReSharper disable once ConsiderUsingAsyncSuffix
    protected Task OnCurrentPageChanged()
    {
      return UpdateCurrentPageItemsAsync();
    }

    /// <summary>
    ///   Called when <see cref="PageSize"/> changed and updates page collection,
    ///   <see cref="UpdateCurrentPageItemsAsync"/>
    /// </summary>
    /// <returns>Waitable task</returns>
    // ReSharper disable once ConsiderUsingAsyncSuffix
    protected Task OnPageSizeChanged()
    {
      return UpdateCurrentPageItemsAsync();
    }

    /// <summary>
    ///   Removes item-s using <see cref="removeAction"/> and update accordingly.
    /// </summary>
    /// <param name="removeAction">The remove action.</param>
    /// <returns>Waitable task which define success state</returns>
    protected virtual async Task<bool> DoRemoveAsync(Func<Task<bool>> removeAction)
    {
      int lastTotalPageNumber = TotalPageCount;

      if (await removeAction().ConfigureAwait(false) == false)
        return false;

      // if the last item on the last page is removed
      if (CurrentPage > TotalPageCount)
        CurrentPage--;

      // Update the total number of pages
      if (lastTotalPageNumber != TotalPageCount)
        RaisePropertyChanged(() => TotalPageCount);

      await UpdateCurrentPageItemsAsync().ConfigureAwait(false);

      return true;
    }

    /// <summary>
    ///   Adds item-s using <see cref="addAction"/> and update accordingly.
    /// </summary>
    /// <param name="addAction">The add action.</param>
    /// <returns>Waitable task which define success state</returns>
    protected virtual async Task<bool> DoAddAsync(Func<Task<bool>> addAction)
    {
      int lastTotalPageNumber = TotalPageCount;

      if (await addAction().ConfigureAwait(false) == false)
        return false;

      // Update the total number of pages
      if (lastTotalPageNumber != TotalPageCount)
        RaisePropertyChanged(() => TotalPageCount);

      await UpdateCurrentPageItemsAsync().ConfigureAwait(false);

      return true;
    }

    #endregion
  }
}