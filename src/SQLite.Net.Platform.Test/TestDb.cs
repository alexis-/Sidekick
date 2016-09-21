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

namespace SQLite.Net.Platform.Test
{
  using System;
  using System.IO;
  using System.Reflection;

  using PCLStorage;

  using SQLite.Net.Bridge;
  using SQLite.Net.Interop;

  /// <summary>
  ///   Base for test database.
  /// </summary>
  /// <seealso cref="SQLite.Net.Bridge.SQLiteConnectionWithLockBridge" />
  public abstract class TestBaseDb : SQLiteConnectionWithLockBridge
  {
    #region Constructors

    /// <summary>Initializes a new instance of the <see cref="TestBaseDb"/> class.</summary>
    /// <param name="platform">The test platform.</param>
    /// <param name="columnInformationProvider">The column information provider.</param>
    /// <param name="contractResolver">The contract resolver.</param>
    public TestBaseDb(
      ISQLitePlatform platform,
      IColumnInformationProvider columnInformationProvider = null,
      IContractResolver contractResolver = null)
      : base(
        platform, CreateTemporaryDatabase(), columnInformationProvider,
        resolver: contractResolver) { }

    #endregion



    #region Methods

    private static string CreateTemporaryDatabase(string fileName = null)
    {
      try
      {
        var desiredName = fileName ?? CreateDefaultTempFilename() + ".db";
        var localStorage = FileSystem.Current.LocalStorage;

        if (localStorage.CheckExistsAsync("temp").Result != ExistenceCheckResult.FolderExists)
          localStorage.CreateFolderAsync("temp", CreationCollisionOption.OpenIfExists).Wait();
      
        IFolder tempFolder = localStorage.GetFolderAsync("temp").Result;
        return
          tempFolder.CreateFileAsync(desiredName, CreationCollisionOption.FailIfExists)
                    .Result.Path;
      }
      catch (Exception)
      {
        return (string)Type.GetType("System.IO.Path").GetRuntimeMethod("GetTempFileName", new Type[] {}).Invoke(null, null);
      }
    }

    private static Guid CreateDefaultTempFilename()
    {
      return Guid.NewGuid();
    }

    #endregion
  }
}