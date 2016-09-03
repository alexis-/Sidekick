// 
// The MIT License (MIT)
// Copyright (c) 2016 Incogito
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

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Sidekick.Shared.Attributes.Database;
using Sidekick.Shared.Utils;
using Xunit;

namespace SQLite.Net.Bridge.Tests
{
  public class SQLiteBasicTest
  {
    public SQLiteBasicTest()
    {
    }

    public class BasicObject
    {
      #region Constructors

      public BasicObject()
      {
        Idx = Faker.RandomInt();
        Value = Faker.RandomString(20);
      }

      #endregion

      #region Properties

      [PrimaryKey, AutoIncrement]
      public int Id { get; set; }

      [Indexed]
      public int Idx { get; set; }

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

    [Fact]
    public void CreateAndFetchObjects()
    {
      IEnumerable<BasicObject> paramObjs = Faker.RandomCollection(
        () => new BasicObject());

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
    public async void AsyncOperations()
    {
      const int count = 500;

      BasicTestDb db = new BasicTestDb();
      List<Task<bool>> tasks = new List<Task<bool>>();

      db.InsertAll(Faker.RandomCollection(
        () => new BasicObject(), count, count));

      for (int i = 0; i < 20; i++)
      {
        Task<bool> task = new Task<bool>(
          () =>
          {
            using (db.Lock())
              return db.Table<BasicObject>().ToList().Count == count;
          });

        tasks.Add(task);
        task.Start();
      }

      await Task.WhenAll(tasks.ToArray());

      tasks.Should().OnlyContain(f => f.Result);
    }


    [Fact]
    public void SelectExpr()
    {
      const int count = 500;

      BasicTestDb db = new BasicTestDb();

      var paramObjs = Faker.RandomCollection(
        () => new BasicObject(), count, count);

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