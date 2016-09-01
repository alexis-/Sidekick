﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InstanceProperties.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Catel;
using Catel.Data;
using Catel.Reflection;
using Sidekick.FilterBuilder.Helpers;
using Sidekick.FilterBuilder.Models.Interfaces;

namespace Sidekick.FilterBuilder.Models
{
  public class InstanceProperties : IPropertyCollection
  {
    public InstanceProperties(Type type)
    {
      Argument.IsNotNull(() => type);

      var finalProperties = new Dictionary<string, IPropertyMetadata>();

      var regularProperties = new List<PropertyInfo>();
      regularProperties.AddRange(
        type.GetProperties()
            .Where(m => m.CanRead && m.IsSupportedType()));

      foreach (var property in regularProperties.Distinct())
      {
        finalProperties[property.Name] = new PropertyMetadata(type, property);
      }

      var catelProperties = new List<PropertyData>();
      if (typeof(ModelBase).IsAssignableFromEx(type))
      {
        var propertyDataManager = PropertyDataManager.Default;
        var catelTypeInfo = propertyDataManager.GetCatelTypeInfo(type);
        catelProperties.AddRange(
          catelTypeInfo.GetCatelProperties()
                       .Values.Where(x => !x.IsModelBaseProperty));
      }

      foreach (var property in catelProperties.Distinct())
      {
        finalProperties[property.Name] = new PropertyMetadata(type, property);
      }

      Properties =
        new List<IPropertyMetadata>(finalProperties.Values.OrderBy(m => m.Name));
    }

    #region Properties

    public List<IPropertyMetadata> Properties { get; private set; }

    #endregion

    #region Methods

    public IPropertyMetadata GetProperty(string propertyName)
    {
      return (from property in Properties
        where string.Equals(property.Name, propertyName)
        select property).FirstOrDefault();
    }

    #endregion
  }
}