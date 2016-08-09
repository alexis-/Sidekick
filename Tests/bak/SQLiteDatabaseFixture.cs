using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net.Interop;

namespace Mnemophile.Tests
{
  public class SQLiteDatabaseFixture : IDisposable
  {
    public SQLiteDatabaseFixture()
    {
      Assembly.GetExecutingAssembly()
      Platform = (ISQLitePlatform)Activator
        .CreateInstance(Type.GetType("Mnemophile.Tests.SQLitePlatformTest"));
    }

    public void Dispose()
    {
    }
    
    public ISQLitePlatform Platform { get; }
  }
}
