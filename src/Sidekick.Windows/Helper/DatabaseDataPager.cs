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

namespace Sidekick.Windows.Helper
{
  using System;
  using System.Collections.Generic;
  using System.Threading.Tasks;

  using AgnosticDatabase.Interfaces;

  using Catel.Data;

  /// <summary>
  ///   Data paging helper.
  /// </summary>
  /// <typeparam name="T">Collection data type</typeparam>
  /// <seealso cref="Catel.Data.ModelBase" />
  public class DatabaseDataPager<T> : ModelBase
    where T : class
  {
    #region Fields

    private readonly Func<ITableQueryAsync<T>, ITableQueryAsync<T>> _queryFunc;
    private readonly IDatabaseAsync _db;

    #endregion



    #region Constructors

    /// <summary>Initializes a new instance of the <see cref="DatabaseDataPager{T}" /> class.</summary>
    /// <param name="queryFunc">The query function.</param>
    /// <param name="db">The database.</param>
    public DatabaseDataPager(
      Func<ITableQueryAsync<T>, ITableQueryAsync<T>> queryFunc, IDatabaseAsync db)
      : this(queryFunc, 0, db)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseDataPager{T}"/> class.
    /// </summary>
    /// <param name="queryFunc">The query function.</param>
    /// <param name="pageSize">The page count.</param>
    /// <param name="db">The database.</param>
    public DatabaseDataPager(
      Func<ITableQueryAsync<T>, ITableQueryAsync<T>> queryFunc, int pageSize, IDatabaseAsync db)
    {
      _queryFunc = queryFunc;
      _db = db;

      HandlePropertyAndCollectionChanges = false;
      ItemCount = 0;
      PageSize = pageSize;
      HandlePropertyAndCollectionChanges = true;
    }

    #endregion



    #region Properties

    /// <summary>Gets or sets the item count.</summary>
    public int ItemCount { get; set; }

    /// <summary>Gets or sets the size of the page.</summary>
    public int PageSize { get; set; }

    /// <summary>Gets or sets the total page count.</summary>
    public int PageCount => PageSize > 0 ? (int)Math.Ceiling(ItemCount / (float)PageSize) : 0;

    #endregion



    /// <summary>Computes <see cref="PageSize"/> and <see cref="PageCount"/></summary>
    /// <returns></returns>
    public async Task ResetAsync()
    {
      // Create base query
      ITableQueryAsync<T> baseQuery = _db.Table<T>();

      if (_queryFunc != null)
        baseQuery = _queryFunc(baseQuery);
      
      // Count items
      ItemCount = await baseQuery.CountAsync().ConfigureAwait(false);

      RaisePropertyChanged(() => PageCount);
    }

    /// <summary>Loads data for given page.</summary>
    /// <param name="pageIndex">Index of the page.</param>
    /// <returns></returns>
    public Task<IEnumerable<T>> LoadPageAsync(int pageIndex)
    {
      return LoadPageAsync(PageSize * pageIndex, PageSize);
    }

    /// <summary>Loads data for given page.</summary>
    /// <param name="start">Start item index.</param>
    /// <param name="size">Page size.</param>
    /// <returns></returns>
    public async Task<IEnumerable<T>> LoadPageAsync(int start, int size)
    {
      // Create base query
      ITableQueryAsync<T> baseQuery = _db.Table<T>();

      if (_queryFunc != null)
        baseQuery = _queryFunc(baseQuery);

      ITableQueryAsync<T> fetchQuery = baseQuery.Skip(start).Take(size);

      return await fetchQuery.ToListAsync().ConfigureAwait(false);
    }
  }
}