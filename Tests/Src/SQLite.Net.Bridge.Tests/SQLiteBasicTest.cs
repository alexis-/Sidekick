using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Sidekick.Tests;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit2;
using Sidekick.Shared.Attributes.Database;
using Xunit;


namespace SQLite.Net.Bridge.Tests
{
  public class SQLiteBasicTest
  {
    public class BasicObject
    {
      [PrimaryKey, AutoIncrement]
      public int Id { get; set; }

      [Indexed]
      public int Idx { get; set; }

      public string Value { get; set; }
    }

    private class BasicTestDb : TestDb
    {
      public BasicTestDb()
      {
        CreateTable<BasicObject>();
      }
    }

    public SQLiteBasicTest()
    {
    }

    [Theory, AutoData]
    public void CreateAndFetchObjects(IEnumerable<BasicObject> paramObjs)
    {
      BasicTestDb db = new BasicTestDb();

      // Create DB objects
      db.InsertAll(paramObjs);

      // Fetch created objects without Id
      IEnumerable<BasicObject> fetchedObjs =
        db.Table<BasicObject>()
          .SelectColumns(new[]
          {
            nameof(BasicObject.Idx),
            nameof(BasicObject.Value)
          });

      paramObjs.ShouldAllBeEquivalentTo(
        fetchedObjs,
        config => config
                    .Excluding(ctx => ctx.SelectedMemberPath.EndsWith(".Id"))
        );
      
      fetchedObjs.Should().OnlyContain(bo => bo.Id == 0);


      // Fetch created objects with Id
      fetchedObjs = db.Table<BasicObject>();

      fetchedObjs.Should().OnlyContain(
        bo =>
        bo.Id != 0
        && bo.Id <= paramObjs.Count());
    }


    [Fact]
    public void SelectExpr()
    {
      BasicTestDb db = new BasicTestDb();
      Fixture fixture = new Fixture();

      IEnumerable<BasicObject> paramObjs =
        fixture.CreateMany<BasicObject>(500);

      // Create DB objects
      db.InsertAll(paramObjs);

      IEnumerable<BasicObject> fetchedObjs;

      // Fetch created objects
      using (db.Lock())
        fetchedObjs =
          db.Table<BasicObject>()
            .Where(bo => paramObjs.Select(po => po.Idx).Contains(bo.Idx))
            .ToList();

      fetchedObjs.Count().Should().Be(paramObjs.Count());
    }

    [Fact]
    public void AsyncOperations()
    {
      BasicTestDb db = new BasicTestDb();
      Fixture fixture = new Fixture();

      List<Task<bool>> tasks = new List<Task<bool>>();
      const int count = 500;

      db.InsertAll(fixture.CreateMany<BasicObject>(count));

      for (int i = 0; i < 20; i++)
      {
        Task<bool> task = new Task<bool>(() => DoAsyncOperation(db, count));

        tasks.Add(task);
        task.Start();
      }

      Task.WaitAll(tasks.ToArray());

      tasks.Should().OnlyContain(f => f.Result);
    }

    private bool DoAsyncOperation(BasicTestDb db, int count)
    {
      using (db.Lock())
      {
        return db.Table<BasicObject>().ToList().Count == count;
      }
    }
  }
}
