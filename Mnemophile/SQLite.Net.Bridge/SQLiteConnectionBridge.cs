﻿using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Mnemophile.Interfaces.DB;
using SQLite.Net.Interop;

namespace SQLite.Net.Bridge
{
  // ReSharper disable once InconsistentNaming
  public class SQLiteConnectionBridge : SQLiteConnection, IDatabase
  {
    public SQLiteConnectionBridge([NotNull] ISQLitePlatform sqlitePlatform,
      [NotNull] string databasePath, bool storeDateTimeAsTicks = true,
      [CanBeNull] IBlobSerializer serializer = null,
      [CanBeNull] IDictionary<string, TableMapping> tableMappings = null,
      [CanBeNull] IDictionary<Type, string> extraTypeMappings = null,
      [CanBeNull] IContractResolver resolver = null)
      : base(sqlitePlatform, databasePath, storeDateTimeAsTicks, serializer,
          tableMappings, extraTypeMappings, resolver)
    {
      ColumnInformationProvider = new ColumnInformationProviderBridge();
    }

    public SQLiteConnectionBridge([NotNull] ISQLitePlatform sqlitePlatform,
      string databasePath, SQLiteOpenFlags openFlags,
      bool storeDateTimeAsTicks = true,
      [CanBeNull] IBlobSerializer serializer = null,
      [CanBeNull] IDictionary<string, TableMapping> tableMappings = null,
      [CanBeNull] IDictionary<Type, string> extraTypeMappings = null,
      IContractResolver resolver = null)
      : base(sqlitePlatform, databasePath, openFlags, storeDateTimeAsTicks,
          serializer, tableMappings, extraTypeMappings, resolver)
    {
      ColumnInformationProvider = new ColumnInformationProviderBridge();
    }

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

    public int InsertAll<T>(IEnumerable<T> objects, bool runInTransaction = true)
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

    public new TableQueryBridge<T> Table<T>() where T : class
    {
      return new TableQueryBridge<T>(Platform, this);
    }

    public int Update<T>(T obj)
    {
      return Update(obj, typeof(T));
    }

    public int UpdateAll<T>(IEnumerable<T> objects, bool runInTransaction = true)
    {
      return UpdateAll((IEnumerable)objects, runInTransaction);
    }
  }
}
