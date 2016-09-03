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
using System.Linq;
using System.Reflection;
using Catel.Data;
using FluentAssertions;
using Sidekick.SpacedRepetition.Models;
using SQLite.Net;
using SQLite.Net.Bridge;
using SQLite.Net.Bridge.Tests;

namespace Sidekick.SpacedRepetition.Tests
{
  public class CardDb : TestBaseDb
  {
    #region Fields

    // Catel ModelBase public properties
    public static readonly string[] ModelBaseProps =
    { "IsDirty", "IsReadOnly" };

    #endregion

    #region Constructors

    public CardDb(CollectionConfig config = null)
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

    #endregion

    #region Methods

    private static object CreateInstance(
      Type type, CollectionConfig config, params object[] args)
    {
      if (type == typeof(Card))
        return new Card(config ?? CollectionConfig.Default);

      return Activator.CreateInstance(type, args);
    }

    #endregion

    private class CatelColumnProvider : ColumnInformationProviderBridge
    {
      #region Methods

      public override bool IsIgnored(PropertyInfo p)
      {
        if (p.DeclaringType == typeof(ModelBase))
          return true;

        return base.IsIgnored(p);
      }

      #endregion
    }

    //  {
    //  const string[] ModelBaseExcludedProps =
    //{

    //public void CreateCardMapping()
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