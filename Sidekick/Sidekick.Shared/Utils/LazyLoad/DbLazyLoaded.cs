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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sidekick.Shared.Attributes.LazyLoad;
using Sidekick.Shared.Interfaces.Database;

namespace Sidekick.Shared.Utils.LazyLoad
{
  public class DbLazyLoad<T> where T : class
  {
    #region Fields

    protected readonly string[] LazyLoadedProperties;
    protected readonly string[] LazyUnloadedProperties;
    protected readonly string[] PermanentProperties;
    protected readonly string PrimaryKeyPropName;
    protected readonly Dictionary<string, PropertyInfo> PropertyInfoMap;

    #endregion

    #region Constructors

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

    #endregion

    #region Properties

    protected static DbLazyLoad<T> Instance { get; set; }

    #endregion

    #region Methods

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

    #endregion
  }

  public static class DbLazyLoadExtensions
  {
    #region Methods

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

    #endregion
  }
}