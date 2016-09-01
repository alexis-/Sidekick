// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyInfoExtensions.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using System.Reflection;
using Catel.ComponentModel;

namespace Sidekick.FilterBuilder.Extensions
{
  internal static class PropertyInfoExtensions
  {
    #region Methods

    public static string GetDisplayName(this PropertyInfo propertyInfo)
    {
      if (propertyInfo != null)
      {
#if false
        DisplayNameAttribute catelDispNameAttr;
                if (propertyInfo.TryGetAttribute(out catelDispNameAttr))
                {
                    return catelDispNameAttr.DisplayName;
                }

                DisplayNameAttribute dispNameAttr;
                if (propertyInfo.TryGetAttribute(out dispNameAttr))
                {
                    return dispNameAttr.DisplayName;
                }

                // TODO: Check what to do with PCL profile here - System.ComponentModel.DataAnnotations
                //DisplayAttribute dispAttr;
                //if (propertyInfo.TryGetAttribute(out dispAttr))
                //{
                //    return dispAttr.GetName();
                //}
#endif
      }

      return null;
    }

    #endregion
  }
}