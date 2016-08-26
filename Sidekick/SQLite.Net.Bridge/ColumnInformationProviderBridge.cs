using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Sidekick.Shared.Attributes.DB;
using SQLite.Net;

namespace SQLite.Net.Bridge
{
  /// <summary>
  /// Bridges Sidekick solution-generic attributes with SQLite attributes.
  /// OrderAttribute is skipped, SQLite using a types affinity rather than
  /// standard typed columns.
  /// </summary>
  /// <seealso cref="SQLite.Net.IColumnInformationProvider" />
  public class ColumnInformationProviderBridge : IColumnInformationProvider
  {
    private readonly DefaultColumnInformationProvider _defaultProvider =
      new DefaultColumnInformationProvider();

    public virtual string GetColumnName(PropertyInfo p)
    {
      var colAttr = p
        .GetCustomAttributes<ColumnAttribute>(true)
        .FirstOrDefault();

      return colAttr == null
        ? _defaultProvider.GetColumnName(p)
        : colAttr.Name;
    }

    public virtual bool IsIgnored(PropertyInfo p)
    {
      return p.IsDefined(typeof(IgnoreAttribute), true)
             || _defaultProvider.IsIgnored(p);
    }

    public virtual IEnumerable<Attributes.IndexedAttribute> GetIndices(MemberInfo p)
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
  }
}
