using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Mnemophile.Attributes.DB;
using Mnemophile.Const.SRS;
using Mnemophile.Interfaces.DB;
using Mnemophile.Interfaces.SRS;
using Mnemophile.SRS.Models;
using static System.String;

namespace Mnemophile.SRS.Impl.Review
{
  internal class ReviewCollectionImpl : IReviewCollection
  {
    private ReviewEnumerator Enumerator { get; }

    //
    // Constructor

    internal ReviewCollectionImpl(IDatabase db, CollectionConfig config)
    {
      Enumerator = new ReviewEnumerator(db, config);
    }


    //
    // IReviewCollection implementation

    public IEnumerator<ICard> GetEnumerator()
    {
      return Enumerator;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    public int Count => CountByState(
      ConstSRS.CardPracticeStateFilterFlag.All);

    public int CountByState(ConstSRS.CardPracticeStateFilterFlag state)
    {
      return -1;
    }
  }

  internal class ReviewEnumerator : IEnumerator<ICard>
  {
    private IDatabase Db { get; }
    private CollectionConfig Config { get; }

    internal ReviewEnumerator(IDatabase db, CollectionConfig config)
    {
      Db = db;
      Config = config;
    }

    public bool MoveNext()
    {
      throw new NotImplementedException();
    }

    public void Reset()
    {
      throw new NotImplementedException();
    }

    public ICard Current { get; }

    object IEnumerator.Current => Current;

    public void Dispose()
    {
      throw new NotImplementedException();
    }
  }
}