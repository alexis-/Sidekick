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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sidekick.Shared.Attributes.Database;

namespace SQLite.Net.Bridge
{
  /// <summary>
  /// Bridges database-generic attributes with SQLite attributes.
  /// OrderAttribute is skipped, SQLite using types affinity rather than
  /// standard typed columns.
  /// </summary>
  /// <seealso cref="SQLite.Net.IColumnInformationProvider" />
  public class ColumnInformationProviderBridge : IColumnInformationProvider
  {
    #region Fields

    private readonly DefaultColumnInformationProvider _defaultProvider =
      new DefaultColumnInformationProvider();

    #endregion

    #region Methods

    public virtual string GetColumnName(PropertyInfo p)
    {
      var colAttr = p
        .GetCustomAttributes<ColumnAttribute>(true)
        .FirstOrDefault();

      return colAttr == null
               ? _defaultProvider.GetColumnName(p)
               : colAttr.Name;
    }

    public string GetTableName(TypeInfo t)
    {
      var colAttr = t
        .GetCustomAttributes<ColumnAttribute>(true)
        .FirstOrDefault();

      return colAttr == null
               ? _defaultProvider.GetTableName(t)
               : colAttr.Name;
    }

    public virtual bool IsIgnored(PropertyInfo p)
    {
      return p.IsDefined(typeof(IgnoreAttribute), true)
             || _defaultProvider.IsIgnored(p);
    }

    public virtual IEnumerable<Attributes.IndexedAttribute> GetIndices(
      MemberInfo p)
    {
      List<Attributes.IndexedAttribute> ret =
        new List<Attributes.IndexedAttribute>(
          _defaultProvider.GetIndices(p));

      ret.AddRange(p.GetCustomAttributes<IndexedAttribute>().Select(
        ia => new Attributes.IndexedAttribute(ia.Name, ia.Order)));

      return ret;
    }

    public virtual bool IsPK(MemberInfo m)
    {
      return m.GetCustomAttributes<PrimaryKeyAttribute>().Any()
             || _defaultProvider.IsPK(m);
    }

    public virtual string Collation(MemberInfo m)
    {
      return m.GetCustomAttributes<CollationAttribute>().Any()
               ? m.GetCustomAttributes<CollationAttribute>().First().Value
               : _defaultProvider.Collation(m);
    }

    public virtual bool IsAutoInc(MemberInfo m)
    {
      return m.GetCustomAttributes<AutoIncrementAttribute>().Any()
             || _defaultProvider.IsAutoInc(m);
    }

    public virtual int? MaxStringLength(PropertyInfo p)
    {
      return p.GetCustomAttributes<MaxLengthAttribute>().Any()
               ? p.GetCustomAttributes<MaxLengthAttribute>().First().Value
               : _defaultProvider.MaxStringLength(p);
    }

    public virtual object GetDefaultValue(PropertyInfo p)
    {
      foreach (var attribute in p.GetCustomAttributes<DefaultAttribute>())
      {
        try
        {
          if (!attribute.UseProperty)
          {
            return Convert.ChangeType(attribute.Value, p.PropertyType);
          }

          var obj = Activator.CreateInstance(p.DeclaringType);
          return p.GetValue(obj);
        }
        catch (Exception exception)
        {
          throw new Exception("Unable to convert " + attribute.Value +
                              " to type " + p.PropertyType, exception);
        }
      }

      return _defaultProvider.GetDefaultValue(p);
    }

    public virtual bool IsMarkedNotNull(MemberInfo p)
    {
      return p.GetCustomAttributes<NotNullAttribute>(true).Any()
             || _defaultProvider.IsMarkedNotNull(p);
    }

    #endregion
  }
}