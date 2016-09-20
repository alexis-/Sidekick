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

namespace Sidekick.Windows.Models
{
  using System;
  using System.IO;

  using Catel;
  using Catel.Fody;
  using Catel.Runtime.Serialization.Json;

  using Orc.FilterBuilder;
  using Orc.FilterBuilder.Models;

  using Sidekick.Shared.Interfaces.Database;
  using Sidekick.SpacedRepetition.Models;

  using SQLite.Net.Attributes;

  /// <summary>
  ///   FilterScheme for Cards. Created for conveniance and table name.
  /// </summary>
  /// <seealso cref="FilterScheme" />
  [Table("CollectionQueries")]
  public class CollectionQuery : FilterScheme
  {
    #region Fields

    private readonly IJsonSerializer _serializer;

    #endregion



    #region Constructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="CollectionQuery" /> class.
    /// </summary>
    /// <param name="serializer">The serializer.</param>
    public CollectionQuery(IJsonSerializer serializer) : this(string.Empty, serializer) { }

    /// <summary>
    ///   Initializes a new instance of the <see cref="CollectionQuery" /> class.
    /// </summary>
    /// <param name="title">Filter title</param>
    /// <param name="serializer">The serializer.</param>
    public CollectionQuery(string title, IJsonSerializer serializer)
      : this(title, false, serializer)
    {
      _serializer = serializer;
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="CollectionQuery" /> class.
    /// </summary>
    /// <param name="title">Filter title</param>
    /// <param name="builtIn">Whether this is a built in query.</param>
    /// <param name="serializer">The serializer.</param>
    public CollectionQuery(string title, bool builtIn, IJsonSerializer serializer)
      : base(typeof(Card), title)
    {
      BuiltIn = builtIn;
      _serializer = serializer;
    }

    #endregion



    #region Properties

    /// <summary>
    ///   Gets or sets a value indicating whether this is a built in query.
    /// </summary>
    public bool BuiltIn { get; set; }

    /// <summary>
    ///   Database identifier.
    /// </summary>
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    /// <summary>
    ///   Serialized filter datas. DB Item.
    /// </summary>
    [NoWeaving]
    public string Data { get { return SerializeData(); } set { DeserializeData(value); } }

    /// <summary>
    ///   Assembly qualified name of target type for current filter. DB Item.
    /// </summary>
    [NoWeaving]
    public string TargetTypeName
    {
      get { return TargetType.AssemblyQualifiedName; }
      set { TargetType = Type.GetType(value); }
    }

    #endregion



    #region Methods

    /// <summary>
    ///   Applies filter conditions to database query.
    /// </summary>
    /// <param name="query">The database query</param>
    /// <returns></returns>
    public ITableQueryAsync<Card> Apply(ITableQueryAsync<Card> query)
    {
      Argument.IsNotNull(() => query);

      return query.Where(this.ToLinqExpression<Card>());
    }

    private string SerializeData()
    {
      Argument.IsNotNull(() => Root);

      using (var stream = new MemoryStream())
      {
        _serializer.Serialize(Root, stream, null);
        stream.Position = 0L;

        using (var streamReader = new StreamReader(stream))
          return streamReader.ReadToEnd();
      }
    }

    private void DeserializeData(string json)
    {
      Argument.IsNotNullOrWhitespace(() => json);

      using (var stream = new MemoryStream())
      using (var streamWriter = new StreamWriter(stream))
      {
        streamWriter.Write(json);
        streamWriter.Flush();
        stream.Position = 0L;

        var root =
          (ConditionTreeItem)_serializer.Deserialize(typeof(ConditionGroup), stream, null);

        ConditionItems.Clear();
        ConditionItems.Add(root);
      }
    }

    #endregion
  }
}