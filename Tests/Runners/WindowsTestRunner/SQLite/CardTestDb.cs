using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mnemophile.SRS.Impl;
using Mnemophile.SRS.Models;
using Mnemophile.Tests;
using SQLite.Net;

namespace Mnemophile.Tests
{
  public class CardTestDb : TestDb
  {
    public CardTestDb(CollectionConfig config = null)
      : base(new ContractResolver(
        t => true, (t, a) => CreateInstance(t, config, a)))
    {
      CreateTable<Note>();
      CreateTable<Card>();
      CreateTable<ReviewLog>();
    }

    public static object CreateInstance(
      Type type, CollectionConfig config, params object[] args)
    {
      if (type == typeof(Card))
        return new Card(config ?? CollectionConfig.Default);

      return Activator.CreateInstance(type, args);
    }
  }
}
