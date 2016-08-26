using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Ploeh.AutoFixture.Xunit2;
using Sidekick.Shared.Attributes.DB;
using Sidekick.Shared.Utils.LazyLoad;
using Sidekick.Shared.Utils.LazyLoad.Attributes;
using Xunit;

namespace Sidekick.Tests
{
  public class DbLazyLoadedTest
  {
    public class LazyObject
    {
      [PrimaryKey, AutoIncrement]
      public int Id { get; set; }

      [Indexed]
      public int Idx { get; set; }

      [LazyLoaded, LazyUnloaded]
      public string Value { get; set; }
    }

    private class LazyObjectDb : TestDb
    {
      public LazyObjectDb()
      {
        CreateTable<LazyObject>();
      }
    }

    [Theory, AutoData]
    public void LazyLoadCollection(IEnumerable<LazyObject> paramObjs)
    {
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
