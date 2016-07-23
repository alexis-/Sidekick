using System;
using System.Collections;
using System.Collections.Generic;

namespace Mnemophile.Interfaces.DB
{
  public interface IDatabase
  {
    /// <summary>
    ///     Begins a new transaction. Call <see cref="CommitTransaction" /> to
    ///     end the transaction.
    /// </summary>
    /// <example cref="System.InvalidOperationException">
    ///     Throws if a transaction has already begun.
    /// </example>
    void BeginTransaction();

    /// <summary>
    ///     Commits the transaction that was begun by
    ///     <see cref="BeginTransaction" />.
    /// </summary>
    void CommitTransaction();

    /// <summary>
    ///     Executes a "create table if not exists" on the database. It also
    ///     creates any specified indexes on the columns of the table. It uses
    ///     a schema automatically generated from the specified type. You can
    ///     later access this schema by calling GetMapping.
    /// </summary>
    /// <returns>
    ///     The number of entries added to the database schema.
    /// </returns>
    int CreateTable<T>();

    /// <summary>
    ///     Deletes the given object from the database using its primary key.
    /// </summary>
    /// <param name="objectToDelete">
    ///     The object to delete. It must have a primary key designated using
    ///     the PrimaryKeyAttribute.
    /// </param>
    /// <returns>
    ///     The number of rows deleted.
    /// </returns>
    int Delete(object objectToDelete);

    /// <summary>
    ///     Deletes the object with the specified primary key.
    /// </summary>
    /// <param name="primaryKey">
    ///     The primary key of the object to delete.
    /// </param>
    /// <returns>
    ///     The number of objects deleted.
    /// </returns>
    /// <typeparam name='T'>
    ///     The type of object.
    /// </typeparam>
    int Delete<T>(object primaryKey);

    /// <summary>
    ///     Deletes all the objects from the specified table.
    ///     WARNING WARNING: Let me repeat. It deletes ALL the objects from 
    ///     the specified table. Do you really want to do that?
    /// </summary>
    /// <returns>
    ///     The number of objects deleted.
    /// </returns>
    /// <typeparam name='T'>
    ///     The type of objects to delete.
    /// </typeparam>
    int DeleteAll<T>();

    /// <summary>
    ///     Executes a "drop table" on the database. This is non-recoverable.
    /// </summary>
    /// <typeparam name='T'>
    ///     The type of table to drop.
    /// </typeparam>
    int DropTable<T>();

    /// <summary>
    /// Computes or retrieve mapping for given table type
    /// </summary>
    /// <typeparam name="T">The type of DB's table</typeparam>
    /// <returns>Table mapping</returns>
    ITableMapping GetTableMapping<T>() where T : class;

    /// <summary>
    ///     Inserts the given object and retrieves its auto incremented
    ///     primary key if it has one.
    /// </summary>
    /// <param name="obj">
    ///     The object to insert.
    /// </param>
    /// <typeparam name='T'>
    ///     The type of object to insert.
    /// </typeparam>
    /// <returns>
    ///     The number of rows added to the table.
    /// </returns>
    int Insert<T>(T obj);

    /// <summary>
    ///     Inserts all specified objects.
    /// </summary>
    /// <param name="objects">
    ///     An <see cref="IEnumerable" /> of the objects to insert.
    /// </param>
    /// <param name="runInTransaction">
    ///     A boolean indicating if the inserts should be wrapped in a
    ///     transaction.
    /// </param>
    /// <typeparam name='T'>
    ///     The type of object to insert.
    /// </typeparam>
    /// <returns>
    ///     The number of rows added to the table.
    /// </returns>
    int InsertAll<T>(IEnumerable<T> objects, bool runInTransaction = true);

    int InsertAllOrIgnore<T>(IEnumerable<T> objects);

    /// <summary>
    ///     Inserts all specified objects.
    ///     For each insertion, if a UNIQUE constraint violation occurs with
    ///     some pre-existing object, this function deletes the old object.
    /// </summary>
    /// <param name="objects">
    ///     An <see cref="IEnumerable" /> of the objects to insert or replace.
    /// </param>
    /// <typeparam name='T'>
    ///     The type of object to insert or replace.
    /// </typeparam>
    /// <returns>
    ///     The total number of rows modified.
    /// </returns>
    int InsertAllOrReplace<T>(IEnumerable<T> objects);

    int InsertOrIgnore<T>(T obj);

    /// <summary>
    ///     Inserts the given object and retrieves its
    ///     auto incremented primary key if it has one.
    ///     If a UNIQUE constraint violation occurs with
    ///     some pre-existing object, this function deletes
    ///     the old object.
    /// </summary>
    /// <param name="obj">
    ///     The object to insert.
    /// </param>
    /// <typeparam name='T'>
    ///     The type of object to insert or replace.
    /// </typeparam>
    /// <returns>
    ///     The number of rows modified.
    /// </returns>
    int InsertOrReplace<T>(T obj);

    /// <summary>
    ///     Creates a SQLiteCommand given the command text (SQL) with
    ///     arguments. Place a '?' in the command text for each of the
    ///     arguments and then executes that command. It returns each row of
    ///     the result using the mapping automatically generated for the
    ///     given type.
    /// </summary>
    /// <param name="query">
    ///     The fully escaped SQL.
    /// </param>
    /// <param name="args">
    ///     Arguments to substitute for the occurences of '?' in the query.
    /// </param>
    /// <typeparam name='T'>
    ///     The type of object to query for.
    /// </typeparam>
    /// <returns>
    ///     An enumerable with one result for each row returned by the query.
    /// </returns>
    List<T> Query<T>(string query, params object[] args) where T : class;

    /// <summary>
    ///     Releases a savepoint returned from
    ///     <see cref="SaveTransactionPoint" />. Releasing a savepoint makes
    ///     changes since that savepoint permanent if the savepoint began the
    ///     transaction, or otherwise the changes are permanent pending a call
    ///     to <see cref="CommitTransaction" />. The RELEASE command is like a
    ///     COMMIT for a SAVEPOINT.
    /// </summary>
    /// <param name="savepoint">
    ///     The name of the savepoint to release.  The string should be the
    ///     result of a call to <see cref="SaveTransactionPoint" />
    /// </param>
    void ReleaseTransaction(string savepoint);

    /// <summary>
    ///     Rolls back the transaction that was begun by
    ///     <see cref="BeginTransaction" /> or
    ///     <see cref="SaveTransactionPoint" />.
    /// </summary>
    void RollbackTransaction();

    /// <summary>
    ///     Rolls back the savepoint created by
    ///     <see cref="BeginTransaction" /> or 
    ///     <see cref="SaveTransactionPoint" />.
    /// </summary>
    /// <param name="savepoint">
    ///     The name of the savepoint to roll back to, as returned by
    ///     <see cref="SaveTransactionPoint" />. If savepoint is null or
    ///     empty, this method is equivalent to a call to
    ///     <see cref="RollbackTransaction" />
    /// </param>
    void RollbackTransactionTo(string savepoint);

    /// <summary>
    ///     Executes <paramref name="action" /> within a (possibly nested)
    ///     transaction by wrapping it in a SAVEPOINT. If an exception occurs
    ///     the whole transaction is rolled back, not just the current
    ///     savepoint. The exception is rethrown.
    /// </summary>
    /// <param name="action">
    ///     The <see cref="Action" /> to perform within a transaction.
    ///     <paramref name="action" /> can contain any number of operations
    ///     on the connection but should never call
    ///     <see cref="BeginTransaction" /> or
    ///     <see cref="CommitTransaction" />.
    /// </param>
    void RunInTransaction(Action action);

    /// <summary>
    ///     Creates a savepoint in the database at the current point in the
    ///     transaction timeline. Begins a new transaction if one is not in
    ///     progress.
    ///     Call <see cref="RollbackTransactionTo" /> to undo transactions
    ///     since the returned savepoint.
    ///     Call <see cref="ReleaseTransaction" /> to commit transactions
    ///     after the savepoint returned here.
    ///     Call <see cref="CommitTransaction" /> to end the transaction,
    ///     committing all changes.
    /// </summary>
    /// <returns>
    ///     A string naming the savepoint.
    /// </returns>
    string SaveTransactionPoint();
    
    /// <summary>
    ///     Returns a queryable interface to the table represented by the
    ///     given type.
    /// </summary>
    /// <returns>
    ///     A queryable object that is able to translate Where, OrderBy, and
    ///     Take queries into native SQL.
    /// </returns>
    ITableQuery<T> Table<T>() where T : class;

    /// <summary>
    ///     Updates all of the columns of a table using the specified object
    ///     except for its primary key.
    ///     The object is required to have a primary key.
    /// </summary>
    /// <param name="obj">
    ///     The object to update. It must have a primary key designated using
    ///     the PrimaryKeyAttribute.
    /// </param>
    /// <typeparam name='T'>
    ///     The type of object to update.
    /// </typeparam>
    /// <returns>
    ///     The number of rows updated.
    /// </returns>
    int Update<T>(T obj);

    /// <summary>
    ///     Updates all specified objects.
    /// </summary>
    /// <param name="objects">
    ///     An <see cref="IEnumerable" /> of the objects to insert.
    /// </param>
    /// <param name="runInTransaction">
    ///     A boolean indicating if the inserts should be wrapped in a
    ///     transaction.
    /// </param>
    /// <returns>
    ///     The number of rows modified.
    /// </returns>
    int UpdateAll<T>(IEnumerable<T> objects, bool runInTransaction = true);
  }
}
