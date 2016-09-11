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

namespace SQLite.Net.Bridge
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Linq.Expressions;
  using System.Threading;
  using System.Threading.Tasks;

  using Sidekick.Shared.Interfaces.Database;

  /// <summary>
  ///   Builds query on a given table.
  /// </summary>
  /// <typeparam name="T">Table type</typeparam>
  /// <seealso cref="Sidekick.Shared.Interfaces.Database.ITableQueryAsync{T}" />
  public class TableQueryAsync<T> : ITableQueryAsync<T>
    where T : class
  {
    #region Fields

    private readonly IDatabase _db;

    private readonly ITableQuery<T> _innerQuery;
    private readonly TaskCreationOptions _taskCreationOptions;

    private readonly TaskScheduler _taskScheduler;

    #endregion



    #region Constructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="TableQueryAsync{T}" /> class.
    /// </summary>
    /// <param name="innerQuery">The inner query.</param>
    /// <param name="db">Database instance.</param>
    /// <param name="taskScheduler">
    ///   If null this parameter will be TaskScheduler.Default (evaluated when used in each method,
    ///   not in ctor)
    /// </param>
    /// <param name="taskCreationOptions">Defaults to DenyChildAttach</param>
    /// <exception cref="System.ArgumentNullException">innerQuery is NULL.</exception>
    public TableQueryAsync(
      ITableQuery<T> innerQuery, IDatabase db, TaskScheduler taskScheduler = null,
      TaskCreationOptions taskCreationOptions = TaskCreationOptions.None)
    {
      if (innerQuery == null)
        throw new ArgumentNullException("innerQuery");

      _innerQuery = innerQuery;
      _db = db;
      _taskScheduler = taskScheduler;
      _taskCreationOptions = taskCreationOptions;
    }

    #endregion



    #region Methods

    public object Clone()
    {
      return new TableQueryAsync<T>(
        _innerQuery.Clone() as TableQueryBridge<T>, _db, _taskScheduler, _taskCreationOptions);
    }

    public Task<int> CountAsync(
      CancellationToken cancellationToken = default(CancellationToken))
    {
      return Task.Factory.StartNew(
                   () =>
                   {
                     cancellationToken.ThrowIfCancellationRequested();
                     using (_db.Lock())
                       return _innerQuery.Count();
                   }, cancellationToken, _taskCreationOptions,
                   _taskScheduler ?? TaskScheduler.Default);
    }

    public Task<int> DeleteAsync(
      CancellationToken cancellationToken = default(CancellationToken))
    {
      return Task.Factory.StartNew(
                   () =>
                   {
                     cancellationToken.ThrowIfCancellationRequested();
                     using (_db.Lock())
                       return _innerQuery.Delete();
                   }, cancellationToken, _taskCreationOptions,
                   _taskScheduler ?? TaskScheduler.Default);
    }


    public Task<T> ElementAtAsync(
      int index, CancellationToken cancellationToken = default(CancellationToken))
    {
      return Task.Factory.StartNew(
                   () =>
                   {
                     cancellationToken.ThrowIfCancellationRequested();
                     using (_db.Lock())
                       return _innerQuery.ElementAt(index);
                   }, cancellationToken, _taskCreationOptions,
                   _taskScheduler ?? TaskScheduler.Default);
    }


    public Task<T> FirstAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
      return Task.Factory.StartNew(
                   () =>
                   {
                     cancellationToken.ThrowIfCancellationRequested();
                     using (_db.Lock())
                     {
                       cancellationToken.ThrowIfCancellationRequested();
                       return _innerQuery.First();
                     }
                   }, cancellationToken, _taskCreationOptions,
                   _taskScheduler ?? TaskScheduler.Default);
    }


    public Task<T> FirstOrDefaultAsync(
      CancellationToken cancellationToken = default(CancellationToken))
    {
      return Task.Factory.StartNew(
                   () =>
                   {
                     cancellationToken.ThrowIfCancellationRequested();
                     using (_db.Lock())
                     {
                       cancellationToken.ThrowIfCancellationRequested();
                       return _innerQuery.FirstOrDefault();
                     }
                   }, cancellationToken, _taskCreationOptions,
                   _taskScheduler ?? TaskScheduler.Default);
    }


    public Task<IEnumerable<TMapped>> MapToAsync<TMapped>(
      bool selectFromAvailableProperties = true,
      CancellationToken cancellationToken = default(CancellationToken))
    {
      return Task.Factory.StartNew(
                   () =>
                   {
                     cancellationToken.ThrowIfCancellationRequested();
                     using (_db.Lock())
                     {
                       cancellationToken.ThrowIfCancellationRequested();
                       return _innerQuery.MapTo<TMapped>(selectFromAvailableProperties);
                     }
                   }, cancellationToken, _taskCreationOptions,
                   _taskScheduler ?? TaskScheduler.Default);
    }


    public ITableQueryAsync<T> OrderBy<TValue>(Expression<Func<T, TValue>> orderExpr)
    {
      if (orderExpr == null)
        throw new ArgumentNullException("orderExpr");

      return new TableQueryAsync<T>(
        _innerQuery.OrderBy(orderExpr), _db, _taskScheduler ?? TaskScheduler.Default,
        _taskCreationOptions);
    }


    public ITableQueryAsync<T> OrderByDescending<TValue>(Expression<Func<T, TValue>> orderExpr)
    {
      if (orderExpr == null)
        throw new ArgumentNullException("orderExpr");

      return new TableQueryAsync<T>(
        _innerQuery.OrderByDescending(orderExpr), _db, _taskScheduler ?? TaskScheduler.Default,
        _taskCreationOptions);
    }


    public ITableQueryAsync<T> OrderByRand()
    {
      return new TableQueryAsync<T>(
        _innerQuery.OrderByRand(), _db, _taskScheduler ?? TaskScheduler.Default,
        _taskCreationOptions);
    }


    public ITableQueryAsync<T> SelectColumns(params string[] propertiesName)
    {
      return new TableQueryAsync<T>(
        _innerQuery.SelectColumns(propertiesName), _db, _taskScheduler ?? TaskScheduler.Default,
        _taskCreationOptions);
    }


    public ITableQueryAsync<T> SelectColumns(
      string selectSqlStatement, params string[] propertiesName)
    {
      return
        new TableQueryAsync<T>(
          _innerQuery.SelectColumns(selectSqlStatement, propertiesName), _db,
          _taskScheduler ?? TaskScheduler.Default, _taskCreationOptions);
    }


    public ITableQueryAsync<T> Skip(int n)
    {
      return new TableQueryAsync<T>(
        _innerQuery.Skip(n), _db, _taskScheduler ?? TaskScheduler.Default, _taskCreationOptions);
    }


    public ITableQueryAsync<T> Take(int n)
    {
      return new TableQueryAsync<T>(
        _innerQuery.Take(n), _db, _taskScheduler ?? TaskScheduler.Default, _taskCreationOptions);
    }


    public ITableQueryAsync<T> ThenBy<TValue>(Expression<Func<T, TValue>> orderExpr)
    {
      if (orderExpr == null)
        throw new ArgumentNullException("orderExpr");

      return new TableQueryAsync<T>(
        _innerQuery.ThenBy(orderExpr), _db, _taskScheduler ?? TaskScheduler.Default,
        _taskCreationOptions);
    }


    public ITableQueryAsync<T> ThenByDescending<TValue>(Expression<Func<T, TValue>> orderExpr)
    {
      if (orderExpr == null)
        throw new ArgumentNullException("orderExpr");

      return new TableQueryAsync<T>(
        _innerQuery.ThenByDescending(orderExpr), _db, _taskScheduler ?? TaskScheduler.Default,
        _taskCreationOptions);
    }


    public Task<List<T>> ToListAsync(
      CancellationToken cancellationToken = default(CancellationToken))
    {
      return Task.Factory.StartNew(
                   () =>
                   {
                     cancellationToken.ThrowIfCancellationRequested();
                     using (_db.Lock())
                       return _innerQuery.ToList();
                   }, cancellationToken, _taskCreationOptions,
                   _taskScheduler ?? TaskScheduler.Default);
    }


    public ITableQueryAsync<T> Where(Expression<Func<T, bool>> predExpr)
    {
      if (predExpr == null)
        throw new ArgumentNullException("predExpr");

      return new TableQueryAsync<T>(
        _innerQuery.Where(predExpr), _db, _taskScheduler ?? TaskScheduler.Default,
        _taskCreationOptions);
    }

    #endregion
  }
}