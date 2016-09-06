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
using PCLStorage;

namespace SQLite.Net.Bridge.Tests
{
  public class TestBaseDb : SQLiteConnectionWithLockBridge
  {
    #region Constructors

    public TestBaseDb(
      IColumnInformationProvider columnInformationProvider = null,
      IContractResolver contractResolver = null)
      : base(
        new SQLitePlatformTest(),
        CreateTemporaryDatabase(),
        columnInformationProvider,
        resolver: contractResolver)
    {
    }

    #endregion

    #region Methods

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

    #endregion
  }
}