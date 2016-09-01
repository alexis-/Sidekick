// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflectionService.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using System;
using System.Threading.Tasks;
using Catel;
using Catel.Caching;
using Catel.Threading;
using Sidekick.FilterBuilder.Models;
using Sidekick.FilterBuilder.Models.Interfaces;
using Sidekick.FilterBuilder.Services.Interfaces;

namespace Sidekick.FilterBuilder.Services
{
  public class ReflectionService : IReflectionService
  {
    private readonly IFilterCustomizationService _filterCustomizationService;

    #region Fields

    private readonly ICacheStorage<Type, IPropertyCollection> _cache =
      new CacheStorage<Type, IPropertyCollection>();

    #endregion

    public ReflectionService(
      IFilterCustomizationService filterCustomizationService)
    {
      Argument.IsNotNull(() => filterCustomizationService);

      _filterCustomizationService = filterCustomizationService;
    }

    #region Methods

    private IPropertyCollection GetInstanceProperties(Type targetType)
    {
      Argument.IsNotNull(() => targetType);

      return _cache.GetFromCacheOrFetch(
        targetType,
        () =>
        {
          var instanceProperties = new InstanceProperties(targetType);

          _filterCustomizationService
            .CustomizeInstanceProperties
            (instanceProperties);

          return instanceProperties;
        });
    }

    public Task<IPropertyCollection> GetInstancePropertiesAsync(Type targetType)
    {
      return
        TaskHelper<IPropertyCollection>.FromResult(
          GetInstanceProperties(targetType));
    }

    public void ClearCache()
    {
      _cache.Clear();
    }

    #endregion
  }
}