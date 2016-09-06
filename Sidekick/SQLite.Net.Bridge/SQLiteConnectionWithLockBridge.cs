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

using System;
using System.Collections;
using System.Collections.Generic;
using Sidekick.Shared.Interfaces.Database;
using SQLite.Net.Interop;

namespace SQLite.Net.Bridge
{
  // ReSharper disable once InconsistentNaming
  /// <summary>
  ///   Database instance.
  ///   Bridges database-generic interface with SQLite.NET implementation.
  /// </summary>
  /// <seealso cref="SQLite.Net.SQLiteConnectionWithLock" />
  /// <seealso cref="Sidekick.Shared.Interfaces.Database.IDatabase" />
  public class SQLiteConnectionWithLockBridge
    : SQLiteConnectionWithLock, IDatabase
  {
    #region Constructors

    public SQLiteConnectionWithLockBridge(
      ISQLitePlatform sqlitePlatform,
      string databasePath,
      IColumnInformationProvider columnInformationProvider = null,
      bool storeDateTimeAsTicks = true,
      IBlobSerializer serializer = null,
      IDictionary<string, TableMapping> tableMappings = null,
      IDictionary<Type, string> extraTypeMappings = null,
      IContractResolver resolver = null)
      : base(
        sqlitePlatform,
        new SQLiteConnectionString(
          databasePath, storeDateTimeAsTicks, serializer, resolver),
        tableMappings,
        extraTypeMappings)
    {
      ColumnInformationProvider =
        columnInformationProvider ?? new ColumnInformationProviderBridge();
    }

    public SQLiteConnectionWithLockBridge(
      ISQLitePlatform sqlitePlatform,
      string databasePath, SQLiteOpenFlags openFlags,
      IColumnInformationProvider columnInformationProvider = null,
      bool storeDateTimeAsTicks = true,
      IBlobSerializer serializer = null,
      IDictionary<string, TableMapping> tableMappings = null,
      IDictionary<Type, string> extraTypeMappings = null,
      IContractResolver resolver = null)
      : base(
        sqlitePlatform,
        new SQLiteConnectionString(
          databasePath, storeDateTimeAsTicks, serializer,
          resolver, openFlags),
        tableMappings,
        extraTypeMappings)
    {
      ColumnInformationProvider =
        columnInformationProvider ?? new ColumnInformationProviderBridge();
    }

    #endregion

    #region Methods

    public void CommitTransaction()
    {
      Commit();
    }

    public int CreateTable<T>()
    {
      return CreateTable(typeof(T));
    }

    public ITableMapping GetTableMapping<T>() where T : class
    {
      return new TableMappingBridge(GetMapping<T>(),
        Platform.ReflectionService);
    }

    public int Insert<T>(T obj)
    {
      return Insert(obj, typeof(T));
    }

    public int InsertAll<T>(IEnumerable<T> objects,
      bool runInTransaction = true)
    {
      return InsertAll(objects, typeof(T), runInTransaction);
    }

    public int InsertAllOrIgnore<T>(IEnumerable<T> objects)
    {
      return InsertOrIgnoreAll(objects);
    }

    public int InsertAllOrReplace<T>(IEnumerable<T> objects)
    {
      return InsertOrReplaceAll(objects, typeof(T));
    }

    public int InsertOrIgnore<T>(T obj)
    {
      return InsertOrIgnore(obj, typeof(T));
    }

    public int InsertOrReplace<T>(T obj)
    {
      return InsertOrReplace(obj, typeof(T));
    }

    public void ReleaseTransaction(string savepoint)
    {
      Release(savepoint);
    }

    public void RollbackTransaction()
    {
      Rollback();
    }

    public void RollbackTransactionTo(string savepoint)
    {
      RollbackTo(savepoint);
    }

    ITableQuery<T> IDatabase.Table<T>()
    {
      return Table<T>();
    }

    public int Update<T>(T obj)
    {
      return Update(obj, typeof(T));
    }

    public int UpdateAll<T>(IEnumerable<T> objects,
      bool runInTransaction = true)
    {
      return UpdateAll((IEnumerable)objects, runInTransaction);
    }

    public new TableQueryBridge<T> Table<T>() where T : class
    {
      return new TableQueryBridge<T>(Platform, this);
    }

    #endregion
  }
}