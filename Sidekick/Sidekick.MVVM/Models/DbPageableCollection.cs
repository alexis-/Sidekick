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
using System.Linq;
using System.Threading.Tasks;
using Catel.Collections;
using Catel.Logging;
using Sidekick.Shared.Interfaces.Database;

namespace Sidekick.MVVM.Models
{
  public class DbPageableCollection<T> : PageableCollectionBase<T>
    where T : class
  {
    #region Fields

    private readonly IDatabase _db;
    private readonly Func<ITableQuery<T>, ITableQuery<T>> _queryFunc;

    private int _itemCount = 0;

    #endregion

    #region Constructors

    public DbPageableCollection(
      IDatabase db, Func<ITableQuery<T>, ITableQuery<T>> queryFunc = null)
    {
      _db = db;
      _queryFunc = queryFunc;

      DisablePropertyChangeNotifications = true;
      CurrentPageItems = new FastObservableCollection<T>();
      DisablePropertyChangeNotifications = false;
    }

    public DbPageableCollection(
      IDatabase db, Func<ITableQuery<T>, ITableQuery<T>> queryFunc,
      int pageSize)
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

    public override int TotalPageCount =>
      PageSize > 0
        ? (int)Math.Ceiling(_itemCount / (float)PageSize)
        : 0;

    #endregion

    #region Methods

    public override Task<bool> RemoveAsync(T item)
    {
      return DoRemoveAsync(() => RemoveFromDb(item));
    }

    public override Task<bool> RemoveAsync(IEnumerable<T> items)
    {
      return DoRemoveAsync(() => RemoveFromDb(items));
    }

    public override Task<bool> AddAsync(T item)
    {
      return DoAddAsync(() => AddToDb(item));
    }

    public override Task<bool> AddAsync(IEnumerable<T> items)
    {
      return DoAddAsync(() => AddToDb(items));
    }

    public override async Task UpdateCurrentPageItemsAsync()
    {
      // Create base query
      ITableQuery<T> baseQuery = _db.Table<T>();

      if (_queryFunc != null)
        baseQuery = _queryFunc(baseQuery);


      // Count items
      baseQuery.Count();


      // Fetch and add items
      int skip = (CurrentPage - 1) * PageSize;
      ITableQuery<T> fetchQuery = baseQuery.Skip(skip)
                                           .Take(PageSize);

      IEnumerable<T> newItems;

      using (_db.Lock())
      {
        newItems = await Task.Run(() => fetchQuery.ToList());
      }

      ((ICollection<T>)CurrentPageItems).ReplaceRange(newItems);
    }

    // 
    // Allows to override Add/Remove behavior to add features
    protected bool RemoveFromDb(T item)
    {
      return _db.Delete(item) > 1;
    }

    protected bool RemoveFromDb(IEnumerable<T> items)
    {
      try
      {
        _db.BeginTransaction();

        foreach (T item in items)
        {
          _db.Delete(item);
        }

        _db.CommitTransaction();
      }
      catch (Exception ex)
      {
        LogManager.GetCurrentClassLogger().Error(ex);
        return false;
      }

      return true;
    }

    protected bool AddToDb(T item)
    {
      return _db.Insert(item) > 0;
    }

    protected bool AddToDb(IEnumerable<T> items)
    {
      return _db.InsertAll(items) > 0;
    }

    #endregion
  }
}