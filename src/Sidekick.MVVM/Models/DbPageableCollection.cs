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

  using AgnosticDatabase.Interfaces;

  using Anotar.Catel;

  using Catel.Collections;

  /// <summary>
  ///   Pageable collection for IDatabaseAsync store.
  /// </summary>
  /// <typeparam name="T">Collection objects type</typeparam>
  /// <seealso cref="Sidekick.MVVM.Models.PageableCollectionBase{T}" />
  public class DbPageableCollection<T> : PageableCollectionBase<T>
    where T : class
  {
    #region Fields

    private readonly IDatabaseAsync _db;
    private readonly Func<ITableQueryAsync<T>, ITableQueryAsync<T>> _queryFunc;

    private int _itemCount = 0;

    #endregion



    #region Constructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="DbPageableCollection{T}" /> class.
    /// </summary>
    /// <param name="db">Database instance;</param>
    /// <param name="queryFunc">(Optional) query function.</param>
    public DbPageableCollection(
      IDatabaseAsync db, Func<ITableQueryAsync<T>, ITableQueryAsync<T>> queryFunc = null)
    {
      _db = db;
      _queryFunc = queryFunc;

      DisablePropertyChangeNotifications = true;
      CurrentPageItems = new FastObservableCollection<T>();
      DisablePropertyChangeNotifications = false;
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="DbPageableCollection{T}" /> class.
    /// </summary>
    /// <param name="db">Database instance;</param>
    /// <param name="queryFunc">(Optional) query function.</param>
    /// <param name="pageSize">Item count per page.</param>
    public DbPageableCollection(
      IDatabaseAsync db, Func<ITableQueryAsync<T>, ITableQueryAsync<T>> queryFunc, int pageSize)
      : base(pageSize)
    {
      _db = db;
      _queryFunc = queryFunc;

      DisablePropertyChangeNotifications = true;
      CurrentPageItems = new FastObservableCollection<T>();
      DisablePropertyChangeNotifications = false;
    }

    #endregion



    #region Properties

    /// <summary>
    ///   Total page count (total item count / page size).
    /// </summary>
    public override int TotalPageCount
      => PageSize > 0 ? (int)Math.Ceiling(_itemCount / (float)PageSize) : 0;

    #endregion



    #region Methods

    /// <summary>
    ///   Removes an item from collection/store.
    /// </summary>
    /// <param name="item">Item to remove</param>
    /// <returns></returns>
    public override Task<bool> RemoveAsync(T item)
    {
      return DoRemoveAsync(() => RemoveFromDbAsync(item));
    }

    /// <summary>
    ///   Removes items from collection/store.
    /// </summary>
    /// <param name="items">Items to remove</param>
    /// <returns></returns>
    public override Task<bool> RemoveAsync(IEnumerable<T> items)
    {
      return DoRemoveAsync(() => RemoveFromDbAsync(items));
    }

    /// <summary>
    ///   Add an item to collection/store.
    /// </summary>
    /// <param name="item">Item to add</param>
    /// <returns></returns>
    public override Task<bool> AddAsync(T item)
    {
      return DoAddAsync(() => AddToDbAsync(item));
    }

    /// <summary>
    ///   Add items to collection/store.
    /// </summary>
    /// <param name="items">Items to add</param>
    /// <returns></returns>
    public override Task<bool> AddAsync(IEnumerable<T> items)
    {
      return DoAddAsync(() => AddToDbAsync(items));
    }

    /// <summary>
    ///   Update page item according to current page.
    /// </summary>
    /// <returns></returns>
    public override async Task UpdateCurrentPageItemsAsync()
    {
      // Create base query
      ITableQueryAsync<T> baseQuery = _db.Table<T>();

      if (_queryFunc != null)
        baseQuery = _queryFunc(baseQuery);


      // Count items
      _itemCount = await baseQuery.CountAsync().ConfigureAwait(false);


      // Fetch and add items
      int skip = (CurrentPage - 1) * PageSize;
      ITableQueryAsync<T> fetchQuery = baseQuery.Skip(skip).Take(PageSize);

      IEnumerable<T> newItems = await fetchQuery.ToListAsync().ConfigureAwait(false);

      using (CurrentPageItems.SuspendChangeNotifications())
        ((ICollection<T>)CurrentPageItems).ReplaceRange(newItems);
    }


    // 
    // Allows to override Add/Remove behavior to add features

    /// <summary>
    ///   Removes from database.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns></returns>
    protected async Task<bool> RemoveFromDbAsync(T item)
    {
      return await _db.DeleteAsync(item).ConfigureAwait(false) > 1;
    }

    /// <summary>
    ///   Removes from database.
    /// </summary>
    /// <param name="items">The items.</param>
    /// <returns></returns>
    protected async Task<bool> RemoveFromDbAsync(IEnumerable<T> items)
    {
      try
      {
        await _db.RunInTransactionAsync(
                   dbSync =>
                   {
                     foreach (T item in items)
                       dbSync.Delete(item);
                   }).ConfigureAwait(false);
      }
      catch (Exception ex)
      {
        LogTo.Error(ex, "RemoveFromDb");

        return false;
      }

      return true;
    }

    /// <summary>
    ///   Adds to database.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns></returns>
    protected async Task<bool> AddToDbAsync(T item)
    {
      return await _db.InsertAsync(item).ConfigureAwait(false) > 0;
    }

    /// <summary>
    ///   Adds to database.
    /// </summary>
    /// <param name="items">The items.</param>
    /// <returns></returns>
    protected async Task<bool> AddToDbAsync(IEnumerable<T> items)
    {
      return await _db.InsertAllAsync(items).ConfigureAwait(false) > 0;
    }

    #endregion
  }
}