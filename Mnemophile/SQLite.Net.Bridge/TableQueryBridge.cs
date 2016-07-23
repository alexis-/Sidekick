using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Mnemophile.Interfaces.DB;
using SQLite.Net.Interop;

namespace SQLite.Net.Bridge
{
  public class TableQueryBridge<T> : TableQuery<T>, ITableQuery<T>
  {
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
  }
}
