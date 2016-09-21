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

  /// <summary>
  ///   Builds query on a given table.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <seealso cref="System.Collections.Generic.IEnumerable{T}" />
  public interface ITableQuery<T> : IEnumerable<T>
  {
    #region Methods

    ITableQuery<T> AddOrderBy<TValue>(Expression<Func<T, TValue>> orderExpr, bool asc);

    ITableQuery<T> AddOrderBy(string propertyName, bool asc);

    object Clone();

    int Count();

    int Delete();

    T ElementAt(int index);

    T First();

    T FirstOrDefault();

    ITableQuery<TResult> Join<TInner, TKey, TResult>(
      ITableQuery<TInner> inner, Expression<Func<T, TKey>> outerKeySelector,
      Expression<Func<TInner, TKey>> innerKeySelector,
      Expression<Func<T, TInner, TResult>> resultSelector);

    IEnumerable<TMapped> MapTo<TMapped>(bool selectFromAvailableProperties = true);

    ITableQuery<T> OrderBy<TValue>(Expression<Func<T, TValue>> orderExpr);

    ITableQuery<T> OrderBy(string propertyName);

    ITableQuery<T> OrderByDescending<TValue>(Expression<Func<T, TValue>> orderExpr);

    ITableQuery<T> OrderByDescending(string propertyName);

    ITableQuery<T> OrderByRand();

    ITableQuery<T> SelectColumns(params string[] propertiesName);

    ITableQuery<T> SelectColumns(string selectSqlStatement, params string[] propertiesName);

    ITableQuery<T> Skip(int n);

    ITableQuery<T> Take(int n);

    ITableQuery<T> ThenBy<TValue>(Expression<Func<T, TValue>> orderExpr);

    ITableQuery<T> ThenBy(string propertyName);

    ITableQuery<T> ThenByDescending<TValue>(Expression<Func<T, TValue>> orderExpr);

    ITableQuery<T> ThenByDescending(string propertyName);

    ITableQuery<T> Where(Expression<Func<T, bool>> predExpr);

    #endregion
  }
}