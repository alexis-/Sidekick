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

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sidekick.Shared.Interfaces.Database;
using SQLite.Net.Interop;

namespace SQLite.Net.Bridge
{
  /// <summary>
  ///   Gather informations from given table and its columns.
  ///   Bridges database-generic interface with SQLite.NET implementation.
  /// </summary>
  /// <seealso cref="Sidekick.Shared.Interfaces.Database.ITableMapping" />
  public class TableMappingBridge : ITableMapping
  {
    #region Constructors

    //
    // Constructor

    public TableMappingBridge(TableMapping tableMapping,
      IReflectionService reflectionService)
    {
      TableMapping = tableMapping;

      Properties = reflectionService.GetPublicInstanceProperties(
        tableMapping.MappedType);
    }

    #endregion

    #region Properties

    private TableMapping TableMapping { get; }
    public IEnumerable<PropertyInfo> Properties { get; }

    #endregion

    #region Methods

    //
    // Implementation

    public string GetTableName()
    {
      return TableMapping.TableName;
    }

    public IEnumerable<string> GetColumnMarkedProperties()
    {
      return TableMapping.Columns.Select(c => c.PropertyName);
    }

    public PropertyInfo GetPropertyInfo(string propertyName)
    {
      return Properties.FirstOrDefault(pi => pi.Name == propertyName);
    }

    public string GetPrimaryKeyProp()
    {
      return TableMapping.PK.Name;
    }

    public string GetColumnName(string propertyName)
    {
      return TableMapping.FindColumnWithPropertyName(propertyName).Name;
    }

    public bool IsDatabaseColumn(string propertyName)
    {
      return TableMapping.FindColumnWithPropertyName(propertyName) != null;
    }

    #endregion
  }
}