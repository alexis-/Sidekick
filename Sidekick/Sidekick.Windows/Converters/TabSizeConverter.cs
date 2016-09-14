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

namespace Sidekick.Windows.Converters
{
  using System;
  using System.Windows.Data;

  using Catel.Windows.Controls;

  /// <summary>Gives equal tab header size to all tab headers.</summary>
  /// <seealso cref="System.Windows.Data.IMultiValueConverter" />
  public class TabSizeConverter : IMultiValueConverter
  {
    #region Methods

    /// <inheritdoc />
    public object Convert(
      object[] values, Type targetType, object parameter,
      System.Globalization.CultureInfo culture)
    {
      TabControl tabControl = values[0] as TabControl;

      if (tabControl == null)
        throw new InvalidOperationException("Invalid control, " + "TabControl is required.");

      double width = tabControl.ActualWidth / tabControl.Items.Count;

      // Subtract 1, otherwise we could overflow to two rows.
      return width <= 1 ? 0 : width - 1;
    }

    /// <inheritdoc />
    public object[] ConvertBack(
      object value, Type[] targetTypes, object parameter,
      System.Globalization.CultureInfo culture)
    {
      throw new NotSupportedException();
    }

    #endregion
  }
}