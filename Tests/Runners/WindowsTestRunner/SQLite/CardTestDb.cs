using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Catel.Data;
using FluentAssertions;
using Mnemophile.SpacedRepetition.Impl;
using Mnemophile.SpacedRepetition.Models;
using Mnemophile.Tests;
using SQLite.Net;
using SQLite.Net.Bridge;

namespace Mnemophile.Tests
{
  public class CardTestDb : TestDb
  {
    // Catel ModelBase public properties
    public static readonly string[] ModelBaseProps =
      { "IsDirty", "IsReadOnly" };

    public CardTestDb(CollectionConfig config = null)
      : base(
        new CatelColumnProvider(),
        new ContractResolver(
          t => true,
          (t, a) => CreateInstance(t, config, a)))
    {
      CreateTable<Note>();
      CreateTable<Card>();
      CreateTable<ReviewLog>();

      TableMapping tableMapping = GetMapping<Card>();
      TableMapping.Column[] columns = tableMapping.Columns;

      columns.Should().NotContain(c => ModelBaseProps.Contains(c.Name));
    }

    private static object CreateInstance(
      Type type, CollectionConfig config, params object[] args)
    {
      if (type == typeof(Card))
        return new Card(config ?? CollectionConfig.Default);

      return Activator.CreateInstance(type, args);
    }

    private class CatelColumnProvider : ColumnInformationProviderBridge
    {
      public override bool IsIgnored(PropertyInfo p)
      {
        if (p.DeclaringType == typeof(ModelBase))
          return true;

        return base.IsIgnored(p);
      }
    }

    //public void CreateCardMapping()
    //{
    //  const string[] ModelBaseExcludedProps =
    //  {
    //    nameof(ModelBase.)
    //  }

    //  IEnumerable<PropertyInfo> cardProps =
    //    Platform.ReflectionService.GetPublicInstanceProperties(typeof(Card));
    //  props.Where(pi => pi.Name)
    //  TableMapping cardMapping = GetMapping<Card>();

    //  cardMapping.Columns
    //}
  }
}
