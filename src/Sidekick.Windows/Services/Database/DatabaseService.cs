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

namespace Sidekick.Windows.Services.Database
{
  using System;
  using System.Linq;
  using System.Reflection;

  using Catel.Data;
  using Catel.IO;
  using Catel.IoC;

  using Orc.FilterBuilder.Models;

  using Sidekick.SpacedRepetition.Models;
  using Sidekick.Windows.Models;

  using SQLite.Net;
  using SQLite.Net.Bridge;
  using SQLite.Net.Platform.Win32;

  /// <summary>Database class.</summary>
  /// <seealso cref="SQLite.Net.Bridge.SQLiteConnectionWithLockBridge" />
  public class DatabaseService : SQLiteConnectionWithLockBridge
  {
    #region Fields

    private const string DbFilename = "database.db";

    #endregion



    #region Constructors

    /// <summary>Initializes a new instance of the <see cref="DatabaseService" /> class.</summary>
    public DatabaseService()
      : base(
        new SQLitePlatformWin32(), CreateOrOpenDatabase(), new SidekickColumnProvider(),
        resolver: new ContractResolver(t => true, CreateInstance))
    {
      using (Lock())
      {
        CreateTable<Note>();
        CreateTable<Card>();
        CreateTable<ReviewLog>();
        CreateTable<CollectionQuery>();
      }
    }

    #endregion



    #region Methods

    private static string CreateOrOpenDatabase()
    {
      // Use local storage. It may be too voluminous to sync, use server sync
      // for that purpose instead.
      var storageTarget = ApplicationDataTarget.UserLocal;

      return Path.Combine(Path.GetApplicationDataDirectory(storageTarget), DbFilename);
    }

    private static object CreateInstance(Type type, params object[] args)
    {
      if (type == typeof(Card))
      {
        CollectionConfig config = null; // TODO: Load config
        // ServiceLocator.Default.ResolveType<CollectionConfig>();

        return new Card(config ?? CollectionConfig.Default);
      }

      return TypeFactory.Default.CreateInstanceWithParametersAndAutoCompletion(type, args);
    }

    #endregion



    private class CatelColumnProvider : ColumnInformationProviderBridge
    {
      #region Methods

      public override bool IsIgnored(PropertyInfo p)
      {
        if (p.DeclaringType == typeof(ModelBase))
          return true;

        return base.IsIgnored(p);
      }

      #endregion
    }

    private class SidekickColumnProvider : CatelColumnProvider
    {
      #region Fields

      private static readonly string[] FilterSchemeColumns = { "Title" };

      #endregion



      #region Methods

      public override bool IsIgnored(PropertyInfo p)
      {
        if (p.DeclaringType == typeof(FilterScheme))
          return !FilterSchemeColumns.Contains(p.Name);

        return base.IsIgnored(p);
      }

      #endregion
    }
  }
}