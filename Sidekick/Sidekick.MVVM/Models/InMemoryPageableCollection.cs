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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Catel.Collections;
using Sidekick.Shared.Utils;

namespace Sidekick.MVVM.Models
{
  public class InMemoryPageableCollection<T> : PageableCollectionBase<T>
  {
    #region Constructors

    public InMemoryPageableCollection(
      IEnumerable<T> allObjects = null)
    {
      AllObjects = new FastObservableCollection<T>(allObjects);
    }

    public InMemoryPageableCollection(
      IEnumerable<T> allObjects, int pageSize)
      : base(pageSize)
    {
      AllObjects = new FastObservableCollection<T>(allObjects);
    }

    #endregion

    #region Properties

    protected FastObservableCollection<T> AllObjects { get; set; }

    public override int TotalPageCount =>
      AllObjects != null && PageSize > 0
        ? (int)Math.Ceiling(AllObjects.Count / (float)PageSize)
        : 0;

    #endregion

    #region Methods

    public override Task<bool> RemoveAsync(T item)
    {
      return DoRemoveAsync(() => RemoveFromList(item));
    }

    public override Task<bool> RemoveAsync(IEnumerable<T> items)
    {
      return DoRemoveAsync(() => RemoveFromList(items));
    }

    public override Task<bool> AddAsync(T item)
    {
      return DoAddAsync(() => AddToList(item));
    }

    public override Task<bool> AddAsync(IEnumerable<T> items)
    {
      return DoAddAsync(() => AddToList(items));
    }

    public override Task UpdateCurrentPageItemsAsync()
    {
      int skip = (CurrentPage - 1) * PageSize;

      ((ICollection<T>)CurrentPageItems).ReplaceRange(
        AllObjects.Skip(skip)
                  .Take(PageSize));

      return TaskConstants.Completed;
    }

    // 
    // Allows to override Add/Remove behavior to add features (such as DB)
    protected bool RemoveFromList(T item)
    {
      AllObjects.Remove(item);
      return true;
    }

    protected bool RemoveFromList(IEnumerable<T> items)
    {
      AllObjects.RemoveItems(items);
      return true;
    }

    protected bool AddToList(T item)
    {
      int insertPosition = (CurrentPage - 1) * PageSize;

      AllObjects.Insert(insertPosition, item);

      return true;
    }

    protected bool AddToList(IEnumerable<T> items)
    {
      int insertPosition = (CurrentPage - 1) * PageSize;

      AllObjects.InsertItems(items, insertPosition);

      return true;
    }

    #endregion
  }
}