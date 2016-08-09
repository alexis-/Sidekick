using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using JetBrains.Annotations;
using Ploeh.AutoFixture.Xunit2;
using SQLite.Net.Attributes;
using SQLite.Net.Interop;
using Xunit;


namespace SQLite.Net.Bridge.Tests
{
  public class SQLiteNetBasicTest
  {
    public class BasicObject
    {
      [PrimaryKey]
      public int Id { get; set; }

      [Indexed]
      public int Idx { get; set; }

      public string Value { get; set; }
    }


    public SQLiteNetBasicTest()
    {
    }

  }
}
