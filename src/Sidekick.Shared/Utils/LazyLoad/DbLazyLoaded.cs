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

namespace Sidekick.Shared.Utils.LazyLoad
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Reflection;
  using System.Threading.Tasks;

  using AgnosticDatabase.Interfaces;

  using Sidekick.Shared.Attributes.LazyLoad;

  /// <summary>
  ///   Helps handling lazy operations in combination with a <see cref="AgnosticDatabase.Interfaces.IDatabaseAsync" /> provider.
  /// </summary>
  /// <typeparam name="T">Table type</typeparam>
  public class DbLazyLoad<T>
    where T : class
  {
    #region Constructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="DbLazyLoad{T}" /> class.
    /// </summary>
    protected DbLazyLoad()
    {
      Instance = Task.FromResult(this);
    }

    #endregion



    #region Properties

    protected static Task<DbLazyLoad<T>> Instance { get; set; }

    protected string[] LazyLoadedProperties { get; set; }
    protected string[] LazyUnloadedProperties { get; set; }
    protected string[] PermanentProperties { get; set; }
    protected string PrimaryKeyPropName { get; set; }
    protected Dictionary<string, PropertyInfo> PropertyInfoMap { get; set; }

    #endregion



    #region Methods

    public static Task<DbLazyLoad<T>> GetOrCreateInstanceAsync(IDatabaseAsync db)
    {
      return Instance ?? new DbLazyLoad<T>().InitializeAsync(db);
    }

    public ITableQueryAsync<T> ShallowLoad(ITableQueryAsync<T> query)
    {
      return query.SelectColumns(PermanentProperties);
    }

    public ITableQueryAsync<T> FurtherLoad(ITableQueryAsync<T> query)
    {
      return query.SelectColumns(LazyLoadedProperties);
    }

    public void UpdateFromFurtherLoad<TPk>(
      IEnumerable<T> toUpdateList, IEnumerable<T> furtherLoadedList, Func<T, TPk> keySelector)
    {
      Dictionary<TPk, T> toUpdateDictionary = toUpdateList.ToDictionary(keySelector);
      Dictionary<TPk, T> furtherLoadedDictionary = furtherLoadedList.ToDictionary(keySelector);

      foreach (TPk key in toUpdateDictionary.Keys)
        UpdateFromFurtherLoad(toUpdateDictionary[key], furtherLoadedDictionary[key]);
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

    protected async Task<DbLazyLoad<T>> InitializeAsync(IDatabaseAsync db)
    {
      ITableMapping tableMapping = await db.GetTableMappingAsync<T>().ConfigureAwait(false);
      PrimaryKeyPropName = tableMapping.GetPrimaryKeyProp();

      PropertyInfoMap = new Dictionary<string, PropertyInfo>();

      List<string> permanentPropertiesList = new List<string>();
      List<string> lazyLoadedPropertiesList = new List<string>();
      List<string> lazyUnloadedPropertiesList = new List<string>();

      foreach (string propName in tableMapping.GetColumnMarkedProperties())
      {
        PropertyInfo propInfo = tableMapping.GetPropertyInfo(propName);

        var lazyLoadedAttribute = propInfo.GetCustomAttribute<LazyLoadedAttribute>();
        var lazyUnloadedAttribute = propInfo.GetCustomAttribute<LazyUnloadedAttribute>();

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

      return this;
    }

    #endregion
  }

  public static class DbLazyLoadExtensions
  {
    #region Methods

    public static ITableQueryAsync<T> ShallowLoad<T>(
      this ITableQueryAsync<T> query, DbLazyLoad<T> lazyLoad) where T : class
    {
      return lazyLoad.ShallowLoad(query);
    }

    public static ITableQueryAsync<T> FurtherLoad<T>(
      this ITableQueryAsync<T> query, DbLazyLoad<T> lazyLoad) where T : class
    {
      return lazyLoad.FurtherLoad(query);
    }

    #endregion
  }
}