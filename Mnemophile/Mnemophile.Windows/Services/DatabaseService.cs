using System;
using System.Reflection;
using Catel.Data;
using Catel.IoC;
using Mnemophile.SRS.Impl;
using Mnemophile.SRS.Models;
using PCLStorage;
using SQLite.Net;
using SQLite.Net.Bridge;
using SQLite.Net.Platform.Win32;

namespace Mnemophile.Windows.Services
{
  public class DatabaseService : SQLiteConnectionWithLockBridge
  {
    private const string DbFilename = "database.db";

    public DatabaseService()
      : base(
        new SQLitePlatformWin32(),
        CreateOrOpenDatabase(),
        new CatelColumnProvider(),
        resolver: new ContractResolver(
          t => true,
          CreateInstance))
    {
      CreateTable<Note>();
      CreateTable<Card>();
      CreateTable<ReviewLog>();
    }

    private static string CreateOrOpenDatabase()
    {
      // Use local storage. It may be too voluminous to sync, use server sync
      // for that purpose instead.
      var storage = FileSystem.Current.LocalStorage;

      return storage.CreateFileAsync(
        DbFilename,
        CreationCollisionOption.OpenIfExists).Result.Path;
    }

    private static object CreateInstance(
      Type type, params object[] args)
    {
      if (type == typeof(Card))
      {
        CollectionConfig config =
          ServiceLocator.Default.ResolveType<CollectionConfig>();

        return new Card(config ?? CollectionConfig.Default);
      }

      return Activator.CreateInstance(type, args);
    }

    private class CatelColumnProvider : ColumnInformationProviderBridge
    {
      public override bool IsIgnored(PropertyInfo p)
      {
        if (p.DeclaringType == typeof(ModelBase))
          return true;

        return base.IsIgnored(p);
      }
    }
  }
}
