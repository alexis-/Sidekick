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

namespace Sidekick.Shared.Interfaces.Database
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Threading;
  using System.Threading.Tasks;

  /// <summary>
  ///   Implementation-agnostic database interface for asynchronous operations.
  ///   True-asynchronicity is not guaranteed and depends on database implementation.
  /// </summary>
  public interface IDatabaseAsync
  {
    #region Methods

    /// <summary>
    ///   Executes a "create table if not exists" on the database. It also
    ///   creates any specified indexes on the columns of the table. It uses
    ///   a schema automatically generated from the specified type. You can
    ///   later access this schema by calling GetMapping.
    /// </summary>
    /// <typeparam name="T">Type of the table to create</typeparam>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///   The number of entries added to the database schema.
    /// </returns>
    Task<int> CreateTableAsync<T>(
      CancellationToken cancellationToken = default(CancellationToken)) where T : class;


    /// <summary>
    ///   Executes a "create table if not exists" on the database. It also
    ///   creates any specified indexes on the columns of the table. It uses
    ///   a schema automatically generated from the specified type. You can
    ///   later access this schema by calling GetMapping.
    /// </summary>
    /// <param name="type">Type of the table to create</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///   The number of entries added to the database schema.
    /// </returns>
    Task<int> CreateTableAsync(
      Type type, CancellationToken cancellationToken = default(CancellationToken));


    /// <summary>
    ///   Deletes the given object from the database using its primary key.
    /// </summary>
    /// <param name="objectToDelete">
    ///   The object to delete. It must have a primary key designated using
    ///   the PrimaryKeyAttribute.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///   The number of rows deleted.
    /// </returns>
    Task<int> DeleteAsync(
      object objectToDelete, CancellationToken cancellationToken = default(CancellationToken));


    /// <summary>
    ///   Deletes the object with the specified primary key.
    /// </summary>
    /// <typeparam name="T">The type of object.</typeparam>
    /// <param name="primaryKey">The primary key of the object to delete.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///   The number of objects deleted.
    /// </returns>
    Task<int> DeleteAsync<T>(
      object primaryKey, CancellationToken cancellationToken = default(CancellationToken));


    /// <summary>
    ///   Deletes all the objects from the specified table.
    ///   WARNING WARNING: Let me repeat. It deletes ALL the objects from
    ///   the specified table. Do you really want to do that?
    /// </summary>
    /// <param name="type">The type of objects to delete.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///   The number of objects deleted.
    /// </returns>
    Task<int> DeleteAllAsync(
      Type type, CancellationToken cancellationToken = default(CancellationToken));


    /// <summary>
    ///   Deletes all the objects from the specified table.
    ///   WARNING WARNING: Let me repeat. It deletes ALL the objects from
    ///   the specified table. Do you really want to do that?
    /// </summary>
    /// <typeparam name="T">The type of objects to delete.</typeparam>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///   The number of objects deleted.
    /// </returns>
    Task<int> DeleteAllAsync<T>(
      CancellationToken cancellationToken = default(CancellationToken));


    /// <summary>
    ///   Executes a "drop table" on the database. This is non-recoverable.
    /// </summary>
    /// <typeparam name="T">The type of table to drop.</typeparam>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task<int> DropTableAsync<T>(
      CancellationToken cancellationToken = default(CancellationToken)) where T : class;


    /// <summary>
    ///   Executes a "drop table" on the database. This is non-recoverable.
    /// </summary>
    /// <param name="type">The type of table to drop.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task<int> DropTableAsync(
      Type type, CancellationToken cancellationToken = default(CancellationToken));


    /// <summary>
    ///   Computes or retrieve mapping for given table type
    /// </summary>
    /// <typeparam name="T">The type of DB's table</typeparam>
    /// <returns>Table mapping</returns>
    Task<ITableMapping> GetTableMappingAsync<T>() where T : class;


    /// <summary>
    ///   Inserts the given object and retrieves its auto incremented
    ///   primary key if it has one.
    /// </summary>
    /// <param name="obj">The object to insert.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///   The number of rows added to the table.
    /// </returns>
    Task<int> InsertAsync(
      object obj, CancellationToken cancellationToken = default(CancellationToken));


    /// <summary>
    ///   Inserts the given object and retrieves its auto incremented
    ///   primary key if it has one.
    /// </summary>
    /// <typeparam name="T">The type of object to insert.</typeparam>
    /// <param name="obj">The object to insert.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///   The number of rows added to the table.
    /// </returns>
    Task<int> InsertAsync<T>(
      T obj, CancellationToken cancellationToken = default(CancellationToken));


    /// <summary>
    ///   Inserts all specified objects.
    /// </summary>
    /// <typeparam name="T">The type of object to insert.</typeparam>
    /// <param name="objects">An <see cref="IEnumerable" /> of the objects to insert.</param>
    /// <param name="runInTransaction">
    ///   A boolean indicating if the inserts should be wrapped in a
    ///   transaction.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///   The number of rows added to the table.
    /// </returns>
    Task<int> InsertAllAsync<T>(
      IEnumerable<T> objects, bool runInTransaction = true,
      CancellationToken cancellationToken = default(CancellationToken));


    /// <summary>
    ///   Inserts all specified objects.
    ///   For each insertion, if a UNIQUE constraint violation occurs with
    ///   some pre-existing object, this function ignore the new object.
    /// </summary>
    /// <typeparam name="T">The type of object to insert or replace.</typeparam>
    /// <param name="objects">An <see cref="IEnumerable" /> of the objects to insert or replace.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///   The total number of rows modified.
    /// </returns>
    Task<int> InsertAllOrIgnoreAsync<T>(
      IEnumerable<T> objects, CancellationToken cancellationToken = default(CancellationToken));


    /// <summary>
    ///   Inserts all specified objects.
    ///   For each insertion, if a UNIQUE constraint violation occurs with
    ///   some pre-existing object, this function deletes the old object.
    /// </summary>
    /// <typeparam name="T">The type of object to insert or replace.</typeparam>
    /// <param name="objects">An <see cref="IEnumerable" /> of the objects to insert or replace.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///   The total number of rows modified.
    /// </returns>
    Task<int> InsertAllOrReplaceAsync<T>(
      IEnumerable<T> objects, CancellationToken cancellationToken = default(CancellationToken));


    /// <summary>
    ///   Inserts the given object and retrieves its auto incremented primary
    ///   key if it has one. If a UNIQUE constraint violation occurs with
    ///   some pre-existing object, this function ignore the new object.
    /// </summary>
    /// <param name="obj">
    ///   The object to insert.
    /// </param>
    /// <typeparam name='T'>
    ///   The type of object to insert or replace.
    /// </typeparam>
    /// <returns>
    ///   The number of rows modified.
    /// </returns>
    Task<int> InsertOrIgnoreAsync<T>(T obj);


    /// <summary>
    ///   Inserts the given object and retrieves its auto incremented primary
    ///   key if it has one. If a UNIQUE constraint violation occurs with
    ///   some pre-existing object, this function deletes the old object.
    /// </summary>
    /// <typeparam name="T">The type of object to insert or replace.</typeparam>
    /// <param name="obj">The object to insert.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///   The number of rows modified.
    /// </returns>
    Task<int> InsertOrReplaceAsync<T>(
      T obj, CancellationToken cancellationToken = default(CancellationToken));


    /// <summary>
    ///   Creates a SQLiteCommand given the command text (SQL) with
    ///   arguments. Place a '?' in the command text for each of the
    ///   arguments and then executes that command. It returns each row of
    ///   the result using the mapping automatically generated for the
    ///   given type.
    /// </summary>
    /// <param name="query">
    ///   The fully escaped SQL.
    /// </param>
    /// <param name="args">
    ///   Arguments to substitute for the occurences of '?' in the query.
    /// </param>
    /// <typeparam name='T'>
    ///   The type of object to query for.
    /// </typeparam>
    /// <returns>
    ///   An enumerable with one result for each row returned by the query.
    /// </returns>
    Task<List<T>> QueryAsync<T>(string query, params object[] args) where T : class;


    /// <summary>
    ///   Creates a SQLiteCommand given the command text (SQL) with
    ///   arguments. Place a '?' in the command text for each of the
    ///   arguments and then executes that command. It returns each row of
    ///   the result using the mapping automatically generated for the
    ///   given type.
    /// </summary>
    /// <typeparam name="T">The type of object to query for.</typeparam>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <param name="query">The fully escaped SQL.</param>
    /// <param name="args">Arguments to substitute for the occurences of '?' in the query.</param>
    /// <returns>
    ///   An enumerable with one result for each row returned by the query.
    /// </returns>
    Task<List<T>> QueryAsync<T>(
      CancellationToken cancellationToken, string query, params object[] args) where T : class;


    /// <summary>
    ///   Asynchronously runs specified actions in a transaction, rollback if an exception is thrown.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task RunInTransactionAsync(
      Action<IDatabase> action, CancellationToken cancellationToken = default(CancellationToken));


    /// <summary>
    ///   Returns a queryable interface to the table represented by the
    ///   given type.
    /// </summary>
    /// <typeparam name="T">Type of the table</typeparam>
    /// <returns>
    ///   A queryable object that is able to translate Where, OrderBy, and
    ///   Take queries into native SQL.
    /// </returns>
    ITableQueryAsync<T> Table<T>() where T : class;


    /// <summary>
    ///   Updates all of the columns of a table using the specified object
    ///   except for its primary key. The object is required to have a primary key.
    /// </summary>
    /// <typeparam name="T">The type of object to update.</typeparam>
    /// <param name="obj">
    ///   The object to update. It must have a primary key designated using
    ///   the PrimaryKeyAttribute.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///   The number of rows updated.
    /// </returns>
    Task<int> UpdateAsync<T>(
      T obj, CancellationToken cancellationToken = default(CancellationToken));


    /// <summary>
    /// Updates all specified objects.
    /// </summary>
    /// <typeparam name="T">The type of objects to update.</typeparam>
    /// <param name="objects">An <see cref="IEnumerable" /> of the objects to insert.</param>
    /// <param name="runInTransaction">A boolean indicating if the inserts should be wrapped in a
    /// transaction.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The number of rows modified.
    /// </returns>
    Task<int> UpdateAllAsync<T>(
      IEnumerable<T> objects, bool runInTransaction = true,
      CancellationToken cancellationToken = default(CancellationToken));

    #endregion
  }
}