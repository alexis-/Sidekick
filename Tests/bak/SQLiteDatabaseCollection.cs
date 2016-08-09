using Xunit;

namespace Mnemophile.Tests
{
  [CollectionDefinition("SQLiteDatabaseCollection")]
  public class SQLiteDatabaseCollection
    : ICollectionFixture<SQLiteDatabaseFixture>
  {
  }
}