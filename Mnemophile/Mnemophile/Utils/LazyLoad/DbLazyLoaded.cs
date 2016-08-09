using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Mnemophile.Interfaces.DB;
using Mnemophile.Utils.LazyLoad.Attributes;

namespace Mnemophile.Utils.LazyLoad
{
  public class DbLazyLoad<T> where T : class
  {
    protected string[] PermanentProperties;
    protected string[] LazyLoadedProperties;
    protected string[] LazyUnloadedProperties;
    protected Dictionary<string, PropertyInfo> PropertyInfoMap;
    protected string PrimaryKeyPropName;

    protected DbLazyLoad(IDatabase db)
    {
      ITableMapping tableMapping = db.GetTableMapping<T>();
      PrimaryKeyPropName = tableMapping.GetPrimaryKeyProp();

      PropertyInfoMap = new Dictionary<string, PropertyInfo>();

      List<string> permanentPropertiesList = new List<string>();
      List<string> lazyLoadedPropertiesList = new List<string>();
      List<string> lazyUnloadedPropertiesList = new List<string>();

      foreach (string propName in tableMapping.GetColumnMarkedProperties())
      {
        PropertyInfo propInfo = tableMapping.GetPropertyInfo(propName);

        var lazyLoadedAttribute =
          propInfo.GetCustomAttribute<LazyLoadedAttribute>();
        var lazyUnloadedAttribute =
          propInfo.GetCustomAttribute<LazyUnloadedAttribute>();

        if (propName == PrimaryKeyPropName)
        {
          permanentPropertiesList.Add(propName);
          lazyLoadedPropertiesList.Add(propName);
          lazyUnloadedPropertiesList.Add(propName);
        }

        else if (lazyUnloadedAttribute != null && lazyLoadedAttribute != null)
        {
          lazyLoadedPropertiesList.Add(propName);
          lazyUnloadedPropertiesList.Add(propName);
        }

        else if (lazyLoadedAttribute != null)
          lazyLoadedPropertiesList.Add(propName);

        else
          permanentPropertiesList.Add(propName);

        PropertyInfoMap.Add(propName, propInfo);
      }

      PermanentProperties = permanentPropertiesList.ToArray();
      LazyLoadedProperties = lazyLoadedPropertiesList.ToArray();
      LazyUnloadedProperties = lazyUnloadedPropertiesList.ToArray();
    }

    protected static DbLazyLoad<T> Instance { get; set; }
    public static DbLazyLoad<T> GetOrCreateInstance(IDatabase db)
    {
      return Instance ?? (Instance = new DbLazyLoad<T>(db));
    }

    public ITableQuery<T> ShallowLoad(ITableQuery<T> query)
    {
      return query.SelectColumns(PermanentProperties);
    }

    public ITableQuery<T> FurtherLoad(ITableQuery<T> query)
    {
      return query.SelectColumns(LazyLoadedProperties);
    }

    public void UpdateFromFurtherLoad<TPk>(
      IEnumerable<T> toUpdateList,
      IEnumerable<T> furtherLoadedList,
      Func<T, TPk> keySelector)
    {
      Dictionary<TPk, T> toUpdateDictionary =
        toUpdateList.ToDictionary(keySelector);
      Dictionary<TPk, T> furtherLoadedDictionary =
        furtherLoadedList.ToDictionary(keySelector);

      foreach (TPk key in toUpdateDictionary.Keys)
        UpdateFromFurtherLoad(
          toUpdateDictionary[key],
          furtherLoadedDictionary[key]);
    }

    public void UpdateFromFurtherLoad(T toUpdate, T furtherLoaded)
    {
      foreach (string propName in LazyLoadedProperties)
      {
        if (propName == PrimaryKeyPropName)
          continue;

        PropertyInfo propertyInfo = PropertyInfoMap[propName];

        propertyInfo.SetValue(toUpdate, propertyInfo.GetValue(furtherLoaded));
      }
    }

    public void LazyUnload(T instance)
    {
      foreach (string propName in LazyUnloadedProperties)
      {
        if (propName == PrimaryKeyPropName)
          continue;

        PropertyInfo propInfo = PropertyInfoMap[propName];
        //Type propType = propInfo.PropertyType;

        propInfo.SetValue(instance, null);
      }
    }
  }

  public static class DbLazyLoadExtensions
  {
    public static ITableQuery<T> ShallowLoad<T>(
      this ITableQuery<T> query, DbLazyLoad<T> lazyLoad)
      where T : class
    {
      return lazyLoad.ShallowLoad(query);
    }
    public static ITableQuery<T> FurtherLoad<T>(
      this ITableQuery<T> query, DbLazyLoad<T> lazyLoad)
      where T : class
    {
      return lazyLoad.FurtherLoad(query);
    }
  }
}
