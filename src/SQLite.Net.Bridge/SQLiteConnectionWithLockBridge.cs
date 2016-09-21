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
  using System.Collections;
  using System.Collections.Generic;

  using AgnosticDatabase.Interfaces;

  using SQLite.Net.Interop;

  // ReSharper disable once InconsistentNaming

  /// <summary>Database instance. Bridges database-generic interface with SQLite.NET implementation.</summary>
  /// <seealso cref="SQLite.Net.SQLiteConnectionWithLock" />
  /// <seealso cref="AgnosticDatabase.Interfaces.IDatabase" />
  public class SQLiteConnectionWithLockBridge : SQLiteConnectionWithLock, IDatabase
  {
    #region Constructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="SQLiteConnectionWithLockBridge" />
    ///   class.
    /// </summary>
    /// <param name="sqlitePlatform">The sqlite platform.</param>
    /// <param name="databasePath">The database path.</param>
    /// <param name="columnInformationProvider">The column information provider.</param>
    /// <param name="storeDateTimeAsTicks">if set to <c>true</c> [store date time as ticks].</param>
    /// <param name="serializer">The serializer.</param>
    /// <param name="tableMappings">The table mappings.</param>
    /// <param name="extraTypeMappings">The extra type mappings.</param>
    /// <param name="resolver">The resolver.</param>
    public SQLiteConnectionWithLockBridge(
      ISQLitePlatform sqlitePlatform, string databasePath,
      IColumnInformationProvider columnInformationProvider = null,
      bool storeDateTimeAsTicks = true, IBlobSerializer serializer = null,
      IDictionary<string, TableMapping> tableMappings = null,
      IDictionary<Type, string> extraTypeMappings = null, IContractResolver resolver = null)
      : base(
        sqlitePlatform,
        new SQLiteConnectionString(databasePath, storeDateTimeAsTicks, serializer, resolver),
        tableMappings, extraTypeMappings)
    {
      ColumnInformationProvider = columnInformationProvider
                                  ?? new ColumnInformationProviderBridge();
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="SQLiteConnectionWithLockBridge" />
    ///   class.
    /// </summary>
    /// <param name="sqlitePlatform">The sqlite platform.</param>
    /// <param name="databasePath">The database path.</param>
    /// <param name="openFlags">The open flags.</param>
    /// <param name="columnInformationProvider">The column information provider.</param>
    /// <param name="storeDateTimeAsTicks">if set to <c>true</c> [store date time as ticks].</param>
    /// <param name="serializer">The serializer.</param>
    /// <param name="tableMappings">The table mappings.</param>
    /// <param name="extraTypeMappings">The extra type mappings.</param>
    /// <param name="resolver">The resolver.</param>
    public SQLiteConnectionWithLockBridge(
      ISQLitePlatform sqlitePlatform, string databasePath, SQLiteOpenFlags openFlags,
      IColumnInformationProvider columnInformationProvider = null,
      bool storeDateTimeAsTicks = true, IBlobSerializer serializer = null,
      IDictionary<string, TableMapping> tableMappings = null,
      IDictionary<Type, string> extraTypeMappings = null, IContractResolver resolver = null)
      : base(
        sqlitePlatform,
        new SQLiteConnectionString(
          databasePath, storeDateTimeAsTicks, serializer, resolver, openFlags), tableMappings,
        extraTypeMappings)
    {
      ColumnInformationProvider = columnInformationProvider
                                  ?? new ColumnInformationProviderBridge();
    }

    #endregion



    #region Methods

    /// <summary>Commits the transaction that was begun by
    ///   <see cref="M:Sidekick.Shared.Interfaces.Database.IDatabase.BeginTransaction" />.</summary>
    public void CommitTransaction()
    {
      Commit();
    }

    /// <summary>
    ///   Executes a "create table if not exists" on the database. It also creates any
    ///   specified indexes on the columns of the table. It uses a schema automatically generated from
    ///   the specified type. You can later access this schema by calling GetMapping.
    /// </summary>
    /// <typeparam name="T">Type of the table to create</typeparam>
    /// <returns>The number of entries added to the database schema.</returns>
    public int CreateTable<T>()
    {
      return CreateTable(typeof(T));
    }

    /// <summary>Computes or retrieve mapping for given table type</summary>
    /// <typeparam name="T">The type of DB's table</typeparam>
    /// <returns>Table mapping</returns>
    public ITableMapping GetTableMapping<T>() where T : class
    {
      return new TableMappingBridge(GetMapping<T>(), Platform.ReflectionService);
    }

    /// <summary>
    ///   Inserts the given object and retrieves its auto incremented primary key if it has
    ///   one.
    /// </summary>
    /// <typeparam name="T">The type of object to insert.</typeparam>
    /// <param name="obj">The object to insert.</param>
    /// <returns>The number of rows added to the table.</returns>
    public int Insert<T>(T obj)
    {
      return Insert(obj, typeof(T));
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
    /// <returns>The number of rows added to the table.</returns>
    public int InsertAll<T>(IEnumerable<T> objects, bool runInTransaction = true)
    {
      return InsertAll(objects, typeof(T), runInTransaction);
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
    /// <returns>The total number of rows modified.</returns>
    public int InsertAllOrIgnore<T>(IEnumerable<T> objects)
    {
      return InsertOrIgnoreAll(objects);
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
    /// <returns>The total number of rows modified.</returns>
    public int InsertAllOrReplace<T>(IEnumerable<T> objects)
    {
      return InsertOrReplaceAll(objects, typeof(T));
    }

    /// <summary>
    ///   Inserts the given object and retrieves its auto incremented primary key if it has
    ///   one. If a UNIQUE constraint violation occurs with some pre-existing object, this function
    ///   ignore the new object.
    /// </summary>
    /// <typeparam name="T">The type of object to insert or replace.</typeparam>
    /// <param name="obj">The object to insert.</param>
    /// <returns>The number of rows modified.</returns>
    public int InsertOrIgnore<T>(T obj)
    {
      return InsertOrIgnore(obj, typeof(T));
    }

    /// <summary>
    ///   Inserts the given object and retrieves its auto incremented primary key if it has
    ///   one. If a UNIQUE constraint violation occurs with some pre-existing object, this function
    ///   deletes the old object.
    /// </summary>
    /// <typeparam name="T">The type of object to insert or replace.</typeparam>
    /// <param name="obj">The object to insert.</param>
    /// <returns>The number of rows modified.</returns>
    public int InsertOrReplace<T>(T obj)
    {
      return InsertOrReplace(obj, typeof(T));
    }

    /// <summary>
    ///   Releases a savepoint returned from
    ///   <see cref="M:Sidekick.Shared.Interfaces.Database.IDatabase.SaveTransactionPoint" />.
    ///   Releasing a savepoint makes changes since that savepoint permanent if the savepoint began
    ///   the transaction, or otherwise the changes are permanent pending a call to
    ///   <see cref="M:Sidekick.Shared.Interfaces.Database.IDatabase.CommitTransaction" />. The
    ///   RELEASE command is like a COMMIT for a SAVEPOINT.
    /// </summary>
    /// <param name="savepoint">
    ///   The name of the savepoint to release.  The string should be the result
    ///   of a call to
    ///   <see cref="M:Sidekick.Shared.Interfaces.Database.IDatabase.SaveTransactionPoint" />
    /// </param>
    public void ReleaseTransaction(string savepoint)
    {
      Release(savepoint);
    }

    /// <summary>Rolls back the transaction that was begun by
    ///   <see cref="M:Sidekick.Shared.Interfaces.Database.IDatabase.BeginTransaction" /> or
    ///   <see cref="M:Sidekick.Shared.Interfaces.Database.IDatabase.SaveTransactionPoint" />.</summary>
    public void RollbackTransaction()
    {
      Rollback();
    }

    /// <summary>
    ///   Rolls back the savepoint created by
    ///   <see cref="M:Sidekick.Shared.Interfaces.Database.IDatabase.BeginTransaction" /> or
    ///   <see cref="M:Sidekick.Shared.Interfaces.Database.IDatabase.SaveTransactionPoint" />.
    /// </summary>
    /// <param name="savepoint">
    ///   The name of the savepoint to roll back to, as returned by
    ///   <see cref="M:Sidekick.Shared.Interfaces.Database.IDatabase.SaveTransactionPoint" />. If
    ///   savepoint is null or empty, this method is equivalent to a call to
    ///   <see cref="M:Sidekick.Shared.Interfaces.Database.IDatabase.RollbackTransaction" />
    /// </param>
    public void RollbackTransactionTo(string savepoint)
    {
      RollbackTo(savepoint);
    }

    /// <summary>Returns a queryable interface to the table represented by the given type.</summary>
    /// <typeparam name="T">Type of the table</typeparam>
    /// <returns>
    ///   A queryable object that is able to translate Where, OrderBy, and Take queries into
    ///   native SQL.
    /// </returns>
    ITableQuery<T> IDatabase.Table<T>()
    {
      return Table<T>();
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
    /// <returns>The number of rows updated.</returns>
    public int Update<T>(T obj)
    {
      return Update(obj, typeof(T));
    }

    /// <summary>Updates all specified objects.</summary>
    /// <typeparam name="T">Type of the objects</typeparam>
    /// <param name="objects">
    ///   An <see cref="T:System.Collections.IEnumerable" /> of the objects to
    ///   insert.
    /// </param>
    /// <param name="runInTransaction">
    ///   A boolean indicating if the inserts should be wrapped in a
    ///   transaction.
    /// </param>
    /// <returns>The number of rows modified.</returns>
    public int UpdateAll<T>(IEnumerable<T> objects, bool runInTransaction = true)
    {
      return UpdateAll((IEnumerable)objects, runInTransaction);
    }

    /// <summary>Returns a queryable interface to the table represented by the given type.</summary>
    /// <typeparam name="T">Type of the table</typeparam>
    /// <returns>
    ///   A queryable object that is able to translate Where, OrderBy, and Take queries into
    ///   native SQL.
    /// </returns>
    public new TableQueryBridge<T> Table<T>() where T : class
    {
      return new TableQueryBridge<T>(Platform, this);
    }

    #endregion
  }
}