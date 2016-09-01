// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IReflectionService.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using System;
using System.Threading.Tasks;
using Sidekick.FilterBuilder.Models.Interfaces;

namespace Sidekick.FilterBuilder.Services.Interfaces
{
  public interface IReflectionService
  {
    Task<IPropertyCollection> GetInstancePropertiesAsync(Type targetType);

    void ClearCache();
  }
}