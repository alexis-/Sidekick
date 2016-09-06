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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Catel.Collections;
using Catel.Data;

namespace Sidekick.MVVM.Models
{
  public abstract class PageableCollectionBase<T> : ModelBase
  {
    #region Fields

    protected const int DefaultPageSize = 25;

    #endregion

    #region Constructors

    protected PageableCollectionBase()
      : this(DefaultPageSize)
    {
    }

    protected PageableCollectionBase(int pageSize)
    {
      HandlePropertyAndCollectionChanges = false;

      PageSize = pageSize;
      CurrentPage = 1;

      HandlePropertyAndCollectionChanges = true;
    }

    #endregion

    #region Properties

    public int PageSize { get; set; }
    public int CurrentPage { get; set; }
    public abstract int TotalPageCount { get; }

    public FastObservableCollection<T> CurrentPageItems { get; set; }

    #endregion

    #region Methods

    public void GoToNextPage()
    {
      if (CurrentPage < TotalPageCount)
        CurrentPage++;
    }

    public void GoToPreviousPage()
    {
      if (CurrentPage > 1)
        CurrentPage--;
    }

    public void Reset()
    {
      CurrentPage = 1;
    }

    protected Task OnCurrentPageChanged()
    {
      return UpdateCurrentPageItemsAsync();
    }

    protected Task OnPageSizeChanged()
    {
      return UpdateCurrentPageItemsAsync();
    }

    public abstract Task<bool> RemoveAsync(T item);
    public abstract Task<bool> AddAsync(T item);

    public abstract Task<bool> RemoveAsync(IEnumerable<T> items);
    public abstract Task<bool> AddAsync(IEnumerable<T> items);

    public abstract Task UpdateCurrentPageItemsAsync();

    protected virtual async Task<bool> DoRemoveAsync(Func<bool> removeAction)
    {
      int lastTotalPageNumber = TotalPageCount;

      if (!removeAction())
        return false;

      // if the last item on the last page is removed
      if (CurrentPage > TotalPageCount)
        CurrentPage--;

      // Update the total number of pages
      if (lastTotalPageNumber != TotalPageCount)
        RaisePropertyChanged(() => TotalPageCount);

      await UpdateCurrentPageItemsAsync();

      return true;
    }

    protected virtual async Task<bool> DoAddAsync(Func<bool> addAction)
    {
      int lastTotalPageNumber = TotalPageCount;

      if (!addAction())
        return false;

      // Update the total number of pages
      if (lastTotalPageNumber != TotalPageCount)
        RaisePropertyChanged(() => TotalPageCount);

      await UpdateCurrentPageItemsAsync();

      return true;
    }

    #endregion
  }
}