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

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Sidekick.Shared.Attributes.Database;
using Sidekick.Shared.Attributes.LazyLoad;
using Sidekick.Shared.Utils;
using Sidekick.Shared.Utils.LazyLoad;
using SQLite.Net.Bridge.Tests;
using Xunit;

namespace Sidekick.Shared.Tests
{
  public class DbLazyLoadedTest
  {
    public class LazyObject
    {
      #region Fields

      private static Random _random = new Random();

      #endregion

      #region Constructors

      public LazyObject() {}

      #endregion

      #region Properties

      [PrimaryKey, AutoIncrement]
      public int Id { get; set; }

      [Indexed]
      public int Idx { get; set; }

      [LazyLoaded, LazyUnloaded]
      public string Value { get; set; }

      #endregion

      public static LazyObject Random()
      {
        return new LazyObject()
        {
          Idx = _random.Next(),
          Value = Faker.RandomString(20)
        };
      }
    }

    private class LazyObjectDb : TestBaseDb
    {
      #region Constructors

      public LazyObjectDb()
      {
        CreateTable<LazyObject>();
      }

      #endregion
    }

    [Fact]
    public void LazyLoadCollection()
    {
      IEnumerable<LazyObject> paramObjs = Faker.RandomCollection(
        LazyObject.Random);

      // Setup DB & LazyLoader
      LazyObjectDb db = new LazyObjectDb();
      DbLazyLoad<LazyObject> lazyLoader =
        DbLazyLoad<LazyObject>.GetOrCreateInstance(db);

      db.InsertAll(paramObjs);

      // Test shallow loading
      IEnumerable<LazyObject> fetchedObjs = db.Table<LazyObject>()
                                              .ShallowLoad(lazyLoader)
                                              .ToList();
      // ToList() is necessary, as db.Table<T> return a custom Enumerable
      // implementation which apparently doesn't allow for true references

      fetchedObjs.Should().OnlyContain(lo =>
                                       lo.Value == null
                                       && lo.Id > 0);
      fetchedObjs.ShouldAllBeEquivalentTo(paramObjs,
        config => config
                    .Including(ctx => ctx.SelectedMemberPath.EndsWith(".Id"))
                    .Including(ctx => ctx.SelectedMemberPath.EndsWith(".Idx"))
        );

      // Test further loading
      IEnumerable<LazyObject> furtherObjs = db.Table<LazyObject>()
                                              .FurtherLoad(lazyLoader);

      furtherObjs.Should().OnlyContain(lo => lo.Value != null);

      lazyLoader.UpdateFromFurtherLoad(fetchedObjs, furtherObjs, lo => lo.Id);

      fetchedObjs.ShouldAllBeEquivalentTo(furtherObjs,
        config =>
        config.Including(ctx => ctx.SelectedMemberPath.EndsWith(".Value"))
        );
      fetchedObjs.ShouldAllBeEquivalentTo(paramObjs,
        config => config
                    .Including(ctx => ctx.SelectedMemberPath.EndsWith(".Id"))
                    .Including(ctx => ctx.SelectedMemberPath.EndsWith(".Idx"))
        );

      // Test further unloading
      foreach (LazyObject fetchedObj in fetchedObjs)
        lazyLoader.LazyUnload(fetchedObj);

      fetchedObjs.Should().OnlyContain(lo =>
                                       lo.Value == null
                                       && lo.Id > 0);
    }
  }
}