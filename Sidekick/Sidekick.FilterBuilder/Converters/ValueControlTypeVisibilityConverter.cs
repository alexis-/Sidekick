// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueControlTypeVisibilityConverter.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using System;
using System.Linq;
using System.Windows;
using Catel.MVVM.Converters;
using Catel.Reflection;

namespace Sidekick.FilterBuilder.Converters
{
  class ValueControlTypeVisibilityConverter : ValueConverterBase
  {
    #region Methods

    protected override object Convert(object value, Type targetType,
      object parameter)
    {
      var controlType = (ValueControlType)value;
      if (parameter != null && parameter.GetType().IsArrayEx())
      {
        var parameterType = (ValueControlType[])parameter;
        return parameterType.Contains(controlType)
                 ? Visibility.Visible
                 : Visibility.Collapsed;
      }
      else
      {
        var parameterType = (ValueControlType)parameter;
        return controlType == parameterType
                 ? Visibility.Visible
                 : Visibility.Collapsed;
      }
    }

    #endregion
  }
}