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
using System.Reflection;
using Catel.Data;
using Catel.IO;
using Sidekick.SpacedRepetition;
using Sidekick.SpacedRepetition.Models;
using Sidekick.Windows.Models;
using SQLite.Net;
using SQLite.Net.Bridge;
using SQLite.Net.Platform.Win32;

namespace Sidekick.Windows.Services
{
  public class DatabaseService : SQLiteConnectionWithLockBridge
  {
    #region Fields

    private const string DbFilename = "database.db";

    #endregion

    #region Constructors

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
      CreateTable<CollectionFilter>();
    }

    #endregion

    #region Methods

    private static string CreateOrOpenDatabase()
    {
      // Use local storage. It may be too voluminous to sync, use server sync
      // for that purpose instead.
      var storageTarget = ApplicationDataTarget.UserLocal;

      return Path.Combine(
        Path.GetApplicationDataDirectory(storageTarget),
        DbFilename);
    }

    private static object CreateInstance(
      Type type, params object[] args)
    {
      if (type == typeof(Card))
      {
        CollectionConfig config = null; // TODO: Load config
        //ServiceLocator.Default.ResolveType<CollectionConfig>();

        return new Card(config ?? CollectionConfig.Default);
      }

      return Activator.CreateInstance(type, args);
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
  }
}