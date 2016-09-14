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
  using System.Linq;
  using System.Threading.Tasks;

  using Catel.Collections;

  using Sidekick.Shared.Utils;

  /// <summary>Pageable implementation which holds all items in memory.</summary>
  /// <typeparam name="T">Objects type</typeparam>
  /// <seealso cref="Sidekick.MVVM.Models.PageableCollectionBase{T}" />
  public class InMemoryPageableCollection<T> : PageableCollectionBase<T>
  {
    #region Constructors

    /// <summary>Initializes a new instance of the <see cref="InMemoryPageableCollection{T}" /> class.</summary>
    /// <param name="allObjects">All objects.</param>
    public InMemoryPageableCollection(IEnumerable<T> allObjects = null)
    {
      AllObjects = new FastObservableCollection<T>(allObjects);
    }

    /// <summary>Initializes a new instance of the <see cref="InMemoryPageableCollection{T}" /> class.</summary>
    /// <param name="allObjects">All objects.</param>
    /// <param name="pageSize">Size of the page.</param>
    public InMemoryPageableCollection(IEnumerable<T> allObjects, int pageSize) : base(pageSize)
    {
      AllObjects = new FastObservableCollection<T>(allObjects);
    }

    #endregion



    #region Properties

    /// <summary>Total page count (total item count / page size).</summary>
    public override int TotalPageCount
      =>
      AllObjects != null && PageSize > 0
        ? (int)Math.Ceiling(AllObjects.Count / (float)PageSize)
        : 0;

    /// <summary>Holds objects to page</summary>
    protected FastObservableCollection<T> AllObjects { get; set; }

    #endregion



    #region Methods

    /// <summary>Removes an item from collection/store.</summary>
    /// <param name="item">Item to remove</param>
    /// <returns></returns>
    public override Task<bool> RemoveAsync(T item)
    {
      return DoRemoveAsync(() => RemoveFromList(item));
    }

    /// <summary>Removes items from collection/store.</summary>
    /// <param name="items">Items to remove</param>
    /// <returns></returns>
    public override Task<bool> RemoveAsync(IEnumerable<T> items)
    {
      return DoRemoveAsync(() => RemoveFromList(items));
    }

    /// <summary>Add an item to collection/store.</summary>
    /// <param name="item">Item to add</param>
    /// <returns></returns>
    public override Task<bool> AddAsync(T item)
    {
      return DoAddAsync(() => AddToList(item));
    }

    /// <summary>Add items to collection/store.</summary>
    /// <param name="items">Items to add</param>
    /// <returns></returns>
    public override Task<bool> AddAsync(IEnumerable<T> items)
    {
      return DoAddAsync(() => AddToList(items));
    }

    /// <summary>Update page item according to current page.</summary>
    /// <returns></returns>
    public override Task UpdateCurrentPageItemsAsync()
    {
      int skip = (CurrentPage - 1) * PageSize;

      using (CurrentPageItems.SuspendChangeNotifications())
        ((ICollection<T>)CurrentPageItems).ReplaceRange(AllObjects.Skip(skip).Take(PageSize));

      return TaskConstants.Completed;
    }


    // 
    // Allows to override Add/Remove behavior to add features (such as DB)

    /// <summary>Removes item from list synchronously.</summary>
    /// <param name="item">The item.</param>
    /// <returns></returns>
    protected Task<bool> RemoveFromList(T item)
    {
      AllObjects.Remove(item);
      return TaskConstants.BooleanTrue;
    }

    /// <summary>Removes items from list synchronously.</summary>
    /// <param name="items">The items.</param>
    /// <returns></returns>
    protected Task<bool> RemoveFromList(IEnumerable<T> items)
    {
      AllObjects.RemoveItems(items);
      return TaskConstants.BooleanTrue;
    }

    /// <summary>Adds item to list synchronously.</summary>
    /// <param name="item">The item.</param>
    /// <returns></returns>
    protected Task<bool> AddToList(T item)
    {
      int insertPosition = (CurrentPage - 1) * PageSize;

      AllObjects.Insert(insertPosition, item);

      return TaskConstants.BooleanTrue;
    }

    /// <summary>Adds items to list synchronously.</summary>
    /// <param name="items">The items.</param>
    /// <returns></returns>
    protected Task<bool> AddToList(IEnumerable<T> items)
    {
      int insertPosition = (CurrentPage - 1) * PageSize;

      AllObjects.InsertItems(items, insertPosition);

      return TaskConstants.BooleanTrue;
    }

    #endregion
  }
}