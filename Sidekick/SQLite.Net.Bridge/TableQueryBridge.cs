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
using System.Linq.Expressions;
using Sidekick.Shared.Interfaces.Database;
using SQLite.Net.Interop;

namespace SQLite.Net.Bridge
{
  /// <summary>
  ///   Builds queries on a given Table.
  ///   Bridges database-generic interface with SQLite.NET implementation.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <seealso cref="SQLite.Net.TableQuery{T}" />
  /// <seealso cref="Sidekick.Shared.Interfaces.Database.ITableQuery{T}" />
  public class TableQueryBridge<T> : TableQuery<T>, ITableQuery<T>
  {
    #region Constructors

    protected TableQueryBridge(ISQLitePlatform platformImplementation,
      SQLiteConnection conn, TableMapping table)
      : base(platformImplementation, conn, table)
    {
    }

    protected TableQueryBridge(TableQueryBridge<T> other)
      : base(other)
    {
    }

    public TableQueryBridge(ISQLitePlatform platformImplementation,
      SQLiteConnection conn) : base(platformImplementation, conn)
    {
    }

    #endregion

    #region Methods

    public override object Clone()
    {
      return new TableQueryBridge<T>(this);
    }

    int ITableQuery<T>.Count()
    {
      return Count();
    }

    int ITableQuery<T>.Count(Expression<Func<T, bool>> predExpr)
    {
      return Count(predExpr);
    }

    int ITableQuery<T>.Delete(Expression<Func<T, bool>> predExpr)
    {
      return Delete(predExpr);
    }

    T ITableQuery<T>.ElementAt(int index)
    {
      return ElementAt(index);
    }

    T ITableQuery<T>.First()
    {
      return First();
    }

    T ITableQuery<T>.FirstOrDefault()
    {
      return FirstOrDefault();
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
      return GetEnumerator();
    }

    ITableQuery<TResult> ITableQuery<T>.Join<TInner, TKey, TResult>(
      ITableQuery<TInner> inner,
      Expression<Func<T, TKey>> outerKeySelector,
      Expression<Func<TInner, TKey>> innerKeySelector,
      Expression<Func<T, TInner, TResult>> resultSelector)
    {
      return new TableQueryBridge<TResult>(_sqlitePlatform, Connection,
        Connection.GetMapping(typeof(TResult)))
      {
        _joinOuter = this,
        _joinOuterKeySelector = outerKeySelector,
        _joinInner = inner as TableQueryBridge<TInner>,
        _joinInnerKeySelector = innerKeySelector,
        _joinSelector = resultSelector
      };
    }

    IEnumerable<U> ITableQuery<T>.MapTo<U>(
      bool selectFromAvailableProperties)
    {
      return MapTo<U>(selectFromAvailableProperties);
    }

    ITableQuery<T> ITableQuery<T>.OrderBy<TValue>(
      Expression<Func<T, TValue>> orderExpr)
    {
      return (TableQueryBridge<T>)OrderBy(orderExpr);
    }

    ITableQuery<T> ITableQuery<T>.OrderByDescending<TValue>(
      Expression<Func<T, TValue>> orderExpr)
    {
      return (TableQueryBridge<T>)OrderByDescending(orderExpr);
    }

    ITableQuery<T> ITableQuery<T>.OrderByRand()
    {
      return (TableQueryBridge<T>)OrderByRand();
    }

    ITableQuery<T> ITableQuery<T>.SelectColumns(
      params string[] propertiesName)
    {
      return (TableQueryBridge<T>)SelectColumns(propertiesName);
    }

    ITableQuery<T> ITableQuery<T>.SelectColumns(
      string selectSqlStatement, params string[] propertiesName)
    {
      return (TableQueryBridge<T>)SelectColumns(selectSqlStatement,
        propertiesName);
    }

    ITableQuery<T> ITableQuery<T>.Skip(int n)
    {
      return (TableQueryBridge<T>)Skip(n);
    }

    ITableQuery<T> ITableQuery<T>.Take(int n)
    {
      return (TableQueryBridge<T>)Take(n);
    }

    ITableQuery<T> ITableQuery<T>.ThenBy<TValue>(
      Expression<Func<T, TValue>> orderExpr)
    {
      return (TableQueryBridge<T>)ThenBy(orderExpr);
    }

    ITableQuery<T> ITableQuery<T>.ThenByDescending<TValue>(
      Expression<Func<T, TValue>> orderExpr)
    {
      return (TableQueryBridge<T>)ThenByDescending(orderExpr);
    }

    ITableQuery<T> ITableQuery<T>.Where(
      Expression<Func<T, bool>> predExpr)
    {
      return (TableQueryBridge<T>)Where(predExpr);
    }

    #endregion
  }
}