using System;
using SQLite.Net.Interop;

namespace Mnemophile.Tests
{
  public interface ISQLiteDatabaseFixture : IDisposable
  {
    ISQLitePlatform Platform { get; }
  }
}