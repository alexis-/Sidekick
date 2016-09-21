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
  using System.Threading;
  using System.Threading.Tasks;

  using AgnosticDatabase.Interfaces;

  /// <summary>
  ///   Provides an async wrapper around <see cref="SQLiteConnectionWithLockBridge" />. Not
  ///   truly asynchronous.
  /// </summary>
  public class SQLiteConnectionAsync : IDatabaseAsync
  {
    #region Fields

    private readonly SQLiteConnectionWithLockBridge _sqliteConnection;
    private readonly TaskCreationOptions _taskCreationOptions;

    private readonly TaskScheduler _taskScheduler;

    #endregion



    #region Constructors

    /// <summary>Initializes a new instance of the <see cref="SQLiteConnectionAsync" /> class.</summary>
    /// <param name="sqliteConnection">Connection instance</param>
    /// <param name="taskScheduler">
    ///   If null this parameter will be TaskScheduler.Default (evaluated
    ///   when used in each method, not in ctor)
    /// </param>
    /// <param name="taskCreationOptions">Defaults to None</param>
    public SQLiteConnectionAsync(
      SQLiteConnectionWithLockBridge sqliteConnection, TaskScheduler taskScheduler = null,
      TaskCreationOptions taskCreationOptions = TaskCreationOptions.None)
    {
      _sqliteConnection = sqliteConnection;
      _taskCreationOptions = taskCreationOptions;
      _taskScheduler = taskScheduler;
    }

    #endregion



    #region Methods

    /// <summary>
    ///   Executes a "create table if not exists" on the database. It also creates any
    ///   specified indexes on the columns of the table. It uses a schema automatically generated from
    ///   the specified type. You can later access this schema by calling GetMapping.
    /// </summary>
    /// <typeparam name="T">Type of the table to create</typeparam>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of entries added to the database schema.</returns>
    public Task<int> CreateTableAsync<T>(
      CancellationToken cancellationToken = default(CancellationToken)) where T : class
    {
      return CreateTableAsync(typeof(T), cancellationToken);
    }


    /// <summary>
    ///   Executes a "create table if not exists" on the database. It also creates any
    ///   specified indexes on the columns of the table. It uses a schema automatically generated from
    ///   the specified type. You can later access this schema by calling GetMapping.
    /// </summary>
    /// <param name="type">Type of the table to create</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of entries added to the database schema.</returns>
    /// <exception cref="System.ArgumentNullException">Type is null.</exception>
    public Task<int> CreateTableAsync(
      Type type, CancellationToken cancellationToken = default(CancellationToken))
    {
      if (type == null)
        throw new ArgumentNullException("type");

      return Task.Factory.StartNew(
                   () =>
                   {
                     var conn = GetConnection();

                     using (conn.Lock())
                       return conn.CreateTable(type);
                   }, cancellationToken, _taskCreationOptions,
                   _taskScheduler ?? TaskScheduler.Default);
    }


    /// <summary>Deletes the given object from the database using its primary key.</summary>
    /// <param name="objectToDelete">
    ///   The object to delete. It must have a primary key designated using
    ///   the PrimaryKeyAttribute.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of rows deleted.</returns>
    /// <exception cref="System.ArgumentNullException">objectToDelete is null.</exception>
    public Task<int> DeleteAsync(
      object objectToDelete, CancellationToken cancellationToken = default(CancellationToken))
    {
      if (objectToDelete == null)
        throw new ArgumentNullException("objectToDelete");

      return Task.Factory.StartNew(
                   () =>
                   {
                     var conn = GetConnection();
                     using (conn.Lock())
                       return conn.Delete(objectToDelete);
                   }, cancellationToken, _taskCreationOptions,
                   _taskScheduler ?? TaskScheduler.Default);
    }


    /// <summary>Deletes the object with the specified primary key.</summary>
    /// <typeparam name="T">The type of object.</typeparam>
    /// <param name="primaryKey">The primary key of the object to delete.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of objects deleted.</returns>
    /// <exception cref="System.ArgumentNullException">primaryKey is null.</exception>
    public Task<int> DeleteAsync<T>(
      object primaryKey, CancellationToken cancellationToken = default(CancellationToken))
    {
      if (primaryKey == null)
        throw new ArgumentNullException("primaryKey");

      return Task.Factory.StartNew(
                   () =>
                   {
                     var conn = GetConnection();
                     using (conn.Lock())
                       return conn.Delete<T>(primaryKey);
                   }, cancellationToken, _taskCreationOptions,
                   _taskScheduler ?? TaskScheduler.Default);
    }


    /// <summary>
    ///   Deletes all the objects from the specified table. WARNING WARNING: Let me repeat. It
    ///   deletes ALL the objects from the specified table. Do you really want to do that?
    /// </summary>
    /// <param name="type">The type of objects to delete.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of objects deleted.</returns>
    public Task<int> DeleteAllAsync(
      Type type, CancellationToken cancellationToken = default(CancellationToken))
    {
      return Task.Factory.StartNew(
                   () =>
                   {
                     var conn = GetConnection();
                     using (conn.Lock())
                       return conn.DeleteAll(type);
                   }, cancellationToken, _taskCreationOptions,
                   _taskScheduler ?? TaskScheduler.Default);
    }


    /// <summary>
    ///   Deletes all the objects from the specified table. WARNING WARNING: Let me repeat. It
    ///   deletes ALL the objects from the specified table. Do you really want to do that?
    /// </summary>
    /// <typeparam name="T">The type of objects to delete.</typeparam>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of objects deleted.</returns>
    public Task<int> DeleteAllAsync<T>(
      CancellationToken cancellationToken = default(CancellationToken))
    {
      return DeleteAllAsync(typeof(T), cancellationToken);
    }


    /// <summary>Executes a "drop table" on the database. This is non-recoverable.</summary>
    /// <param name="type">The type of table to drop.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public Task<int> DropTableAsync(
      Type type, CancellationToken cancellationToken = default(CancellationToken))
    {
      return Task.Factory.StartNew(
                   () =>
                   {
                     var conn = GetConnection();
                     using (conn.Lock())
                       return conn.DropTable(type);
                   }, cancellationToken, _taskCreationOptions,
                   _taskScheduler ?? TaskScheduler.Default);
    }


    /// <summary>Executes a "drop table" on the database. This is non-recoverable.</summary>
    /// <typeparam name="T">The type of table to drop.</typeparam>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public Task<int> DropTableAsync<T>(
      CancellationToken cancellationToken = default(CancellationToken)) where T : class
    {
      return DropTableAsync(typeof(T), cancellationToken);
    }


    /// <summary>Computes or retrieve mapping for given table type</summary>
    /// <typeparam name="T">The type of DB's table</typeparam>
    /// <returns>Table mapping</returns>
    public Task<ITableMapping> GetTableMappingAsync<T>() where T : class
    {
      return Task.Factory.StartNew(
                   () =>
                   {
                     SQLiteConnectionWithLockBridge conn = GetConnection();

                     using (conn.Lock())
                       return conn.GetTableMapping<T>();
                   }, CancellationToken.None, _taskCreationOptions,
                   _taskScheduler ?? TaskScheduler.Default);
    }


    /// <summary>
    ///   Inserts the given object and retrieves its auto incremented primary key if it has
    ///   one.
    /// </summary>
    /// <param name="obj">The object to insert.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of rows added to the table.</returns>
    /// <exception cref="System.ArgumentNullException">obj is NULL.</exception>
    public Task<int> InsertAsync(
      object obj, CancellationToken cancellationToken = default(CancellationToken))
    {
      if (obj == null)
        throw new ArgumentNullException("obj");

      return Task.Factory.StartNew(
                   () =>
                   {
                     var conn = GetConnection();
                     using (conn.Lock())
                       return conn.Insert(obj);
                   }, cancellationToken, _taskCreationOptions,
                   _taskScheduler ?? TaskScheduler.Default);
    }


    /// <summary>
    ///   Inserts the given object and retrieves its auto incremented primary key if it has
    ///   one.
    /// </summary>
    /// <typeparam name="T">The type of object to insert.</typeparam>
    /// <param name="obj">The object to insert.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of rows added to the table.</returns>
    /// <exception cref="System.ArgumentNullException">obj is NULL.</exception>
    public Task<int> InsertAsync<T>(
      T obj, CancellationToken cancellationToken = default(CancellationToken))
    {
      if (obj == null)
        throw new ArgumentNullException("obj");

      return Task.Factory.StartNew(
                   () =>
                   {
                     var conn = GetConnection();
                     using (conn.Lock())
                       return conn.Insert(obj);
                   }, cancellationToken, _taskCreationOptions,
                   _taskScheduler ?? TaskScheduler.Default);
    }


    /// <summary>Inserts all specified objects.</summary>
    /// <typeparam name="T">The type of object to insert.</typeparam>
    /// <param name="objects">
    ///   An <see cref="T:System.Collections.IEnumerable" /> of the objects to
    ///   insert.
    /// </param>
    /// <param name="runInTransaction">
    ///   A boolean indicating if the inserts should be wrapped in a
    ///   transaction.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of rows added to the table.</returns>
    /// <exception cref="System.ArgumentNullException">objects is NULL.</exception>
    public Task<int> InsertAllAsync<T>(
      IEnumerable<T> objects, bool runInTransaction = true,
      CancellationToken cancellationToken = default(CancellationToken))
    {
      if (objects == null)
        throw new ArgumentNullException("objects");

      return Task.Factory.StartNew(
                   () =>
                   {
                     var conn = GetConnection();
                     using (conn.Lock())
                       return conn.InsertAll(objects);
                   }, cancellationToken, _taskCreationOptions,
                   _taskScheduler ?? TaskScheduler.Default);
    }


    /// <summary>
    ///   Inserts all specified objects. For each insertion, if a UNIQUE constraint violation
    ///   occurs with some pre-existing object, this function ignore the new object.
    /// </summary>
    /// <typeparam name="T">The type of object to insert or replace.</typeparam>
    /// <param name="objects">
    ///   An <see cref="T:System.Collections.IEnumerable" /> of the objects to
    ///   insert or replace.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The total number of rows modified.</returns>
    /// <exception cref="System.ArgumentNullException">objects is NULL.</exception>
    public Task<int> InsertAllOrIgnoreAsync<T>(
      IEnumerable<T> objects, CancellationToken cancellationToken = default(CancellationToken))
    {
      if (objects == null)
        throw new ArgumentNullException("objects");

      return Task.Factory.StartNew(
                   () =>
                   {
                     SQLiteConnectionWithLock conn = GetConnection();
                     using (conn.Lock())
                       return conn.InsertOrIgnoreAll(objects);
                   }, cancellationToken, _taskCreationOptions,
                   _taskScheduler ?? TaskScheduler.Default);
    }


    /// <summary>
    ///   Inserts all specified objects. For each insertion, if a UNIQUE constraint violation
    ///   occurs with some pre-existing object, this function deletes the old object.
    /// </summary>
    /// <typeparam name="T">The type of object to insert or replace.</typeparam>
    /// <param name="objects">
    ///   An <see cref="T:System.Collections.IEnumerable" /> of the objects to
    ///   insert or replace.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The total number of rows modified.</returns>
    /// <exception cref="System.ArgumentNullException">objects is NULL.</exception>
    public Task<int> InsertAllOrReplaceAsync<T>(
      IEnumerable<T> objects, CancellationToken cancellationToken = default(CancellationToken))
    {
      if (objects == null)
        throw new ArgumentNullException("objects");

      return Task.Factory.StartNew(
                   () =>
                   {
                     var conn = GetConnection();
                     using (conn.Lock())
                       return conn.InsertOrReplaceAll(objects);
                   }, cancellationToken, _taskCreationOptions,
                   _taskScheduler ?? TaskScheduler.Default);
    }


    /// <summary>
    ///   Inserts the given object and retrieves its auto incremented primary key if it has
    ///   one. If a UNIQUE constraint violation occurs with some pre-existing object, this function
    ///   ignore the new object.
    /// </summary>
    /// <typeparam name="T">The type of object to insert or replace.</typeparam>
    /// <param name="obj">The object to insert.</param>
    /// <returns>The number of rows modified.</returns>
    public Task<int> InsertOrIgnoreAsync<T>(T obj)
    {
      return Task.Factory.StartNew(
                   () =>
                   {
                     SQLiteConnectionWithLock conn = GetConnection();
                     using (conn.Lock())
                       return conn.InsertOrIgnore(obj);
                   });
    }


    /// <summary>
    ///   Inserts the given object and retrieves its auto incremented primary key if it has
    ///   one. If a UNIQUE constraint violation occurs with some pre-existing object, this function
    ///   deletes the old object.
    /// </summary>
    /// <typeparam name="T">The type of object to insert or replace.</typeparam>
    /// <param name="obj">The object to insert.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of rows modified.</returns>
    /// <exception cref="System.ArgumentNullException">obj is NULL.</exception>
    public Task<int> InsertOrReplaceAsync<T>(
      T obj, CancellationToken cancellationToken = default(CancellationToken))
    {
      if (obj == null)
        throw new ArgumentNullException("obj");

      return Task.Factory.StartNew(
                   () =>
                   {
                     var conn = GetConnection();
                     using (conn.Lock())
                       return conn.InsertOrReplace(obj);
                   }, cancellationToken, _taskCreationOptions,
                   _taskScheduler ?? TaskScheduler.Default);
    }


    /// <summary>
    ///   Creates a SQLiteCommand given the command text (SQL) with arguments. Place a '?' in
    ///   the command text for each of the arguments and then executes that command. It returns each
    ///   row of the result using the mapping automatically generated for the given type.
    /// </summary>
    /// <typeparam name="T">The type of object to query for.</typeparam>
    /// <param name="query">The fully escaped SQL.</param>
    /// <param name="args">Arguments to substitute for the occurences of '?' in the query.</param>
    /// <returns>An enumerable with one result for each row returned by the query.</returns>
    public Task<List<T>> QueryAsync<T>(string query, params object[] args) where T : class
    {
      return QueryAsync<T>(CancellationToken.None, query, args);
    }


    /// <summary>
    ///   Creates a SQLiteCommand given the command text (SQL) with arguments. Place a '?' in
    ///   the command text for each of the arguments and then executes that command. It returns each
    ///   row of the result using the mapping automatically generated for the given type.
    /// </summary>
    /// <typeparam name="T">The type of object to query for.</typeparam>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <param name="query">The fully escaped SQL.</param>
    /// <param name="args">Arguments to substitute for the occurences of '?' in the query.</param>
    /// <returns>An enumerable with one result for each row returned by the query.</returns>
    /// <exception cref="System.ArgumentNullException">sql or args is NULL.</exception>
    public Task<List<T>> QueryAsync<T>(
      CancellationToken cancellationToken, string query, params object[] args) where T : class
    {
      if (query == null)
        throw new ArgumentNullException("query");
      if (args == null)
        throw new ArgumentNullException("args");

      return Task.Factory.StartNew(
                   () =>
                   {
                     cancellationToken.ThrowIfCancellationRequested();
                     var conn = GetConnection();
                     using (conn.Lock())
                     {
                       cancellationToken.ThrowIfCancellationRequested();
                       return conn.Query<T>(query, args);
                     }
                   }, cancellationToken, _taskCreationOptions,
                   _taskScheduler ?? TaskScheduler.Default);
    }


    /// <summary>
    ///   Asynchronously runs specified actions in a transaction, rollback if an exception is
    ///   thrown.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">action is NULL.</exception>
    public Task RunInTransactionAsync(
      Action<IDatabase> action, CancellationToken cancellationToken = default(CancellationToken))
    {
      if (action == null)
        throw new ArgumentNullException("action");

      return Task.Factory.StartNew(
                   () =>
                   {
                     cancellationToken.ThrowIfCancellationRequested();
                     var conn = GetConnection();
                     using (conn.Lock())
                     {
                       cancellationToken.ThrowIfCancellationRequested();
                       conn.BeginTransaction();
                       try
                       {
                         action(conn);
                         conn.Commit();
                       }
                       catch (Exception)
                       {
                         conn.Rollback();
                         throw;
                       }
                     }
                   }, cancellationToken, _taskCreationOptions,
                   _taskScheduler ?? TaskScheduler.Default);
    }


    /// <summary>Returns a queryable interface to the table represented by the given type.</summary>
    /// <typeparam name="T">Type of the table</typeparam>
    /// <returns>
    ///   A queryable object that is able to translate Where, OrderBy, and Take queries into
    ///   native SQL.
    /// </returns>
    public ITableQueryAsync<T> Table<T>() where T : class
    {
      //
      // This isn't async as the underlying connection doesn't go out to the database
      // until the query is performed. The Async methods are on the query iteself.
      //
      var conn = GetConnection();
      return new TableQueryAsync<T>(conn.Table<T>(), conn, _taskScheduler, _taskCreationOptions);
    }


    /// <summary>
    ///   Updates all of the columns of a table using the specified object except for its
    ///   primary key. The object is required to have a primary key.
    /// </summary>
    /// <typeparam name="T">The type of object to update.</typeparam>
    /// <param name="obj">
    ///   The object to update. It must have a primary key designated using the
    ///   PrimaryKeyAttribute.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of rows updated.</returns>
    /// <exception cref="System.ArgumentNullException">obj is NULL.</exception>
    public Task<int> UpdateAsync<T>(
      T obj, CancellationToken cancellationToken = default(CancellationToken))
    {
      if (obj == null)
        throw new ArgumentNullException("obj");

      return Task.Factory.StartNew(
                   () =>
                   {
                     var conn = GetConnection();
                     using (conn.Lock())
                       return conn.Update(obj);
                   }, cancellationToken, _taskCreationOptions,
                   _taskScheduler ?? TaskScheduler.Default);
    }


    /// <summary>Updates all asynchronous.</summary>
    /// <typeparam name="T">The type of objects to update.</typeparam>
    /// <param name="objects">The objects.</param>
    /// <param name="runInTransaction">if set to <c>true</c> [run in transaction].</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of rows modified.</returns>
    /// <exception cref="System.ArgumentNullException">objects is NULL.</exception>
    public Task<int> UpdateAllAsync<T>(
      IEnumerable<T> objects, bool runInTransaction = true,
      CancellationToken cancellationToken = default(CancellationToken))
    {
      if (objects == null)
        throw new ArgumentNullException("objects");

      return Task.Factory.StartNew(
                   () =>
                   {
                     var conn = GetConnection();
                     using (conn.Lock())
                       return conn.UpdateAll(objects);
                   }, cancellationToken, _taskCreationOptions,
                   _taskScheduler ?? TaskScheduler.Default);
    }

    /// <summary>Returns connection instance.</summary>
    protected SQLiteConnectionWithLockBridge GetConnection()
    {
      return _sqliteConnection;
    }

    #endregion
  }
}