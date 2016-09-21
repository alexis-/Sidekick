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

namespace AgnosticDatabase.Interfaces
{
  using System;
  using System.Collections.Generic;
  using System.Linq.Expressions;
  using System.Threading;
  using System.Threading.Tasks;

  /// <summary>
  ///   Builds query on a given table.
  /// </summary>
  /// <typeparam name="T">Table type</typeparam>
  /// <seealso cref="System.Collections.Generic.IEnumerable{T}" />
  public interface ITableQueryAsync<T>
  {
    #region Methods

    ITableQueryAsync<T> AddOrderBy<TValue>(Expression<Func<T, TValue>> orderExpr, bool asc);

    ITableQueryAsync<T> AddOrderBy(string propertyName, bool asc);

    object Clone();

    Task<int> CountAsync(CancellationToken cancellationToken = default(CancellationToken));

    Task<int> DeleteAsync(CancellationToken cancellationToken = default(CancellationToken));

    Task<T> ElementAtAsync(
      int index, CancellationToken cancellationToken = default(CancellationToken));

    Task<T> FirstAsync(CancellationToken cancellationToken = default(CancellationToken));

    Task<T> FirstOrDefaultAsync(
      CancellationToken cancellationToken = default(CancellationToken));

    Task<IEnumerable<TMapped>> MapToAsync<TMapped>(
      bool selectFromAvailableProperties = true,
      CancellationToken cancellationToken = default(CancellationToken));

    ITableQueryAsync<T> OrderBy<TValue>(Expression<Func<T, TValue>> orderExpr);

    ITableQueryAsync<T> OrderBy(string propertyName);

    ITableQueryAsync<T> OrderByDescending<TValue>(Expression<Func<T, TValue>> orderExpr);

    ITableQueryAsync<T> OrderByDescending(string propertyName);

    ITableQueryAsync<T> OrderByRand();

    ITableQueryAsync<T> SelectColumns(params string[] propertiesName);

    ITableQueryAsync<T> SelectColumns(string selectSqlStatement, params string[] propertiesName);

    ITableQueryAsync<T> Skip(int n);

    ITableQueryAsync<T> Take(int n);

    ITableQueryAsync<T> ThenBy<TValue>(Expression<Func<T, TValue>> orderExpr);

    ITableQueryAsync<T> ThenBy(string propertyName);

    ITableQueryAsync<T> ThenByDescending<TValue>(Expression<Func<T, TValue>> orderExpr);

    ITableQueryAsync<T> ThenByDescending(string propertyName);

    ITableQueryAsync<T> Where(Expression<Func<T, bool>> predExpr);

    Task<List<T>> ToListAsync(CancellationToken cancellationToken = default(CancellationToken));

    #endregion
  }
}