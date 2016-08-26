using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Sidekick.Shared.Interfaces.DB;
using SQLite.Net.Interop;

namespace SQLite.Net.Bridge
{
  public class TableMappingBridge : ITableMapping
  {
    private TableMapping TableMapping { get; }
    public IEnumerable<PropertyInfo> Properties { get; }


    //
    // Constructor

    public TableMappingBridge(TableMapping tableMapping,
      IReflectionService reflectionService)
    {
      TableMapping = tableMapping;

      Properties = reflectionService.GetPublicInstanceProperties(
        tableMapping.MappedType);
    }


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
  }
}