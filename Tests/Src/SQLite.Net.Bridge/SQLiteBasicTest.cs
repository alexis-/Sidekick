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

namespace SQLite.Net.Bridge.Tests
{
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading.Tasks;

  using AgnosticDatabase.Attributes;

  using FluentAssertions;
  
  using Sidekick.Shared.Utils;

  using Xunit;

  /// <summary>
  ///   SQLite.Net.Bridge-related tests
  /// </summary>
  public class SQLiteBasicTest
  {
    /// <summary>
    ///   Initializes a new instance of the <see cref="SQLiteBasicTest" /> class.
    /// </summary>
    public SQLiteBasicTest() { }

    /// <summary>
    ///   Basic object implementation for testing purposes
    /// </summary>
    public class BasicObject
    {
      #region Constructors

      /// <summary>
      /// Initializes a new instance of the <see cref="BasicObject"/> class.
      /// </summary>
      public BasicObject()
      {
        Idx = Faker.RandomInt();
        Value = Faker.RandomString(20);
      }

      #endregion



      #region Properties

      /// <summary>
      ///   Auto-generated ID
      /// </summary>
      [PrimaryKey, AutoIncrement]
      public int Id { get; set; }

      /// <summary>
      ///   Random indexed value
      /// </summary>
      [Indexed]
      public int Idx { get; set; }

      /// <summary>
      ///   Random string
      /// </summary>
      public string Value { get; set; }

      #endregion
    }

    private class BasicTestDb : TestBaseDb
    {
      #region Constructors

      public BasicTestDb()
      {
        CreateTable<BasicObject>();
      }

      #endregion
    }

    /// <summary>
    ///   Simple asynchronous inserting and concurrent querying test.
    /// </summary>
    [Fact]
    public async void AsyncOperations()
    {
      const int Count = 500;

      BasicTestDb db = new BasicTestDb();
      List<Task<bool>> tasks = new List<Task<bool>>();

      await
        Task.Run(
              () => db.InsertAll(Faker.RandomCollection(() => new BasicObject(), Count, Count)))
            .ConfigureAwait(true);

      for (int i = 0; i < 20; i++)
        tasks.Add(
          Task.Run(
            () =>
            {
              using (db.Lock())
                return db.Table<BasicObject>().ToList().Count == Count;
            }));

      await Task.WhenAll(tasks.ToArray()).ConfigureAwait(true);

      tasks.Should().OnlyContain(f => f.Result);
    }
    
    [Fact]
    public void CreateAndFetchObjects()
    {
      IEnumerable<BasicObject> paramObjs = Faker.RandomCollection(() => new BasicObject());

      BasicTestDb db = new BasicTestDb();

      // Create DB objects
      db.InsertAll(paramObjs);

      // Fetch created objects without Id
      IEnumerable<BasicObject> fetchedObjs =
        db.Table<BasicObject>()
          .SelectColumns(new[] { nameof(BasicObject.Idx), nameof(BasicObject.Value) });

      paramObjs.ShouldAllBeEquivalentTo(
        fetchedObjs, config => config.Excluding(ctx => ctx.SelectedMemberPath.EndsWith(".Id")));

      fetchedObjs.Should().OnlyContain(bo => bo.Id == 0);


      // Fetch created objects with Id
      fetchedObjs = db.Table<BasicObject>();

      fetchedObjs.Should().OnlyContain(bo => bo.Id != 0 && bo.Id <= paramObjs.Count());
    }


    [Fact]
    public void SelectExpr()
    {
      const int Count = 500;

      BasicTestDb db = new BasicTestDb();

      var paramObjs = Faker.RandomCollection(() => new BasicObject(), Count, Count);

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
  }
}