using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace Sidekick.Shared.Interfaces.DB
{
  public interface ITableQuery<T> : IEnumerable<T>
  {
    object Clone();
    int Count();
    int Count([NotNull] Expression<Func<T, bool>> predExpr);
    int Delete([NotNull] Expression<Func<T, bool>> predExpr);
    T ElementAt(int index);
    T First();
    T FirstOrDefault();
    ITableQuery<TResult> Join<TInner, TKey, TResult>(
      ITableQuery<TInner> inner,
      Expression<Func<T, TKey>> outerKeySelector,
      Expression<Func<TInner, TKey>> innerKeySelector,
      Expression<Func<T, TInner, TResult>> resultSelector);
    IEnumerable<U> MapTo<U>(bool selectFromAvailableProperties = true);
    ITableQuery<T> OrderBy<TValue>(Expression<Func<T, TValue>> orderExpr);
    ITableQuery<T> OrderByDescending<TValue>(
      Expression<Func<T, TValue>> orderExpr);
    ITableQuery<T> OrderByRand();
    ITableQuery<T> SelectColumns(params string[] propertiesName);
    ITableQuery<T> SelectColumns(string selectSqlStatement,
      params string[] propertiesName);
    ITableQuery<T> Skip(int n);
    ITableQuery<T> Take(int n);
    ITableQuery<T> ThenBy<TValue>(Expression<Func<T, TValue>> orderExpr);
    ITableQuery<T> ThenByDescending<TValue>(
      Expression<Func<T, TValue>> orderExpr);
    ITableQuery<T> Where([NotNull] Expression<Func<T, bool>> predExpr);
  }
}
