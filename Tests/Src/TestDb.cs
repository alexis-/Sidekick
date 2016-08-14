using System;
using Mnemophile.Tests;
using PCLStorage;
using SQLite.Net;
using SQLite.Net.Bridge;

namespace Mnemophile.Tests
{
  public class TestDb : SQLiteConnectionWithLockBridge
  {
    public TestDb(IContractResolver contractResolver = null) : base(
      new SQLitePlatformTest(),
      CreateTemporaryDatabase(),
      resolver: contractResolver)
    {
    }

    public static string CreateTemporaryDatabase(string fileName = null)
    {
      var desiredName = fileName ?? CreateDefaultTempFilename() + ".db";
      var localStorage = FileSystem.Current.LocalStorage;

      if (localStorage.CheckExistsAsync("temp").Result !=
          ExistenceCheckResult.FolderExists)
        localStorage.CreateFolderAsync(
          "temp",
          CreationCollisionOption.OpenIfExists)
                    .Wait();

      IFolder tempFolder = localStorage.GetFolderAsync("temp").Result;
      return tempFolder.CreateFileAsync(
        desiredName,
        CreationCollisionOption.FailIfExists)
                       .Result.Path;
    }

    public static Guid CreateDefaultTempFilename()
    {
      return Guid.NewGuid();
    }
  }
}