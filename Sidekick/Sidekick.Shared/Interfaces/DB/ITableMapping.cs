using System.Collections.Generic;
using System.Reflection;

namespace Sidekick.Shared.Interfaces.DB
{
  public interface ITableMapping
  {
    /// <summary>
    ///     Retrieve database table name for given type.
    /// </summary>
    /// <returns>Database table name</returns>
    string GetTableName();

    /// <summary>
    ///     Retrieve properties mapped to a database column.
    /// </summary>
    /// <returns>Properties name</returns>
    IEnumerable<string> GetColumnMarkedProperties();

    /// <summary>
    ///     Retrieve <see cref="PropertyInfo"/> for given propertyName.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns><see cref="PropertyInfo"/> for given property name</returns>
    PropertyInfo GetPropertyInfo(string propertyName);

    /// <summary>
    /// Gets the primary key property name.
    /// </summary>
    /// <returns>PK property name</returns>
    string GetPrimaryKeyProp();

    /// <summary>
    /// Gets column's name for given property.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns>Column's name for </returns>
    string GetColumnName(string propertyName);

    /// <summary>
    /// Determines whether given property is mapped to a column.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns>Whether given property is a column</returns>
    bool IsDatabaseColumn(string propertyName);
  }
}
