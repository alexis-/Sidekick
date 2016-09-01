﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeExtensions.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using System;
using System.Linq;
using System.Reflection;

namespace Sidekick.FilterBuilder.Extensions
{
  /// <summary>
  /// Type class extensions methods
  /// </summary>
  internal static class TypeExtensions
  {
    #region Methods

    /// <summary>
    /// Gets non nullable type used to create nullable type.
    /// </summary>
    /// <param name="type">Nullable Type to retrieve non nullable parameter</param>
    /// <returns></returns>
    internal static Type GetNonNullable(this Type type)
    {
      var genericArguments = type.GetGenericArguments();
      return type.IsNullable() ? genericArguments.Single() : type;
    }

    /// <summary>
    /// Checks if type is instance of nullable struct
    /// </summary>
    /// <param name="type">Type to check</param>
    /// <returns></returns>
    internal static bool IsNullable(this Type type)
    {
      return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
    }

    #endregion
  }
}