// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFilterCustomizationService.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using Sidekick.FilterBuilder.Models.Interfaces;

namespace Sidekick.FilterBuilder.Services.Interfaces
{
  public interface IFilterCustomizationService
    {
        void CustomizeInstanceProperties(IPropertyCollection instanceProperties);
    }
}