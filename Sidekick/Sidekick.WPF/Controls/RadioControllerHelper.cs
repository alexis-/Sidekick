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

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace Sidekick.WPF.Controls
{
  /// <summary>
  /// RadioController attached property and TypeConverter.
  /// </summary>
  public static class RadioControllerHelper
  {
    #region Fields

    public static readonly DependencyProperty RadioControllerProperty =
      DependencyProperty.RegisterAttached(
        "RadioController",
        typeof(RadioController),
        typeof(RadioControllerHelper),
        new PropertyMetadata(null, RadioControllerPropertyChangedCallback));

    #endregion

    #region Methods

    [Category("Sidekick")]
    [AttachedPropertyBrowsableForType(typeof(Panel))]
    [AttachedPropertyBrowsableForChildren]
    [TypeConverter(typeof(RadioControllerConverter))]
    public static RadioController GetRadioController(DependencyObject obj)
    {
      return (RadioController)obj.GetValue(RadioControllerProperty);
    }

    public static void SetRadioController(
      DependencyObject obj,
      RadioController value)
    {
      obj.SetValue(RadioControllerProperty, value);
    }

    private static void RadioControllerPropertyChangedCallback(
      DependencyObject dependencyObject,
      DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
    {
    }

    #endregion

    /// <summary>
    /// Convert bound <see cref="IRadioControllerMonitor"/> to RadioController
    /// </summary>
    /// <seealso cref="System.ComponentModel.TypeConverter" />
    private class RadioControllerConverter : TypeConverter
    {
      #region Methods

      public override bool CanConvertFrom(
        ITypeDescriptorContext context,
        Type sourceType)
      {
        return sourceType == typeof(IRadioControllerMonitor);
      }

      public override object ConvertFrom(
        ITypeDescriptorContext context,
        CultureInfo culture,
        object value)
      {
        return new RadioController(value as IRadioControllerMonitor);
      }

      #endregion
    }
  }
}