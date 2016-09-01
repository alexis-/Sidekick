// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPropertyCollection.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace Sidekick.FilterBuilder.Models.Interfaces
{
  public interface IPropertyCollection
    {
        List<IPropertyMetadata> Properties { get; }

        IPropertyMetadata GetProperty(string propertyName);
    }
}