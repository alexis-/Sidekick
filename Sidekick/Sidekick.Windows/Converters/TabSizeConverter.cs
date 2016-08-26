using System;
using System.Windows.Data;
using Catel.Windows.Controls;

namespace Sidekick.Windows.Converters
{
  public class TabSizeConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter,
        System.Globalization.CultureInfo culture)
    {
      TabControl tabControl = values[0] as TabControl;

      if (tabControl == null)
        throw new InvalidOperationException("Invalid control, "
          + "TabControl is required.");

      double width = tabControl.ActualWidth / tabControl.Items.Count;
      //Subtract 1, otherwise we could overflow to two rows.
      return (width <= 1) ? 0 : (width - 1);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter,
        System.Globalization.CultureInfo culture)
    {
      throw new NotSupportedException();
    }
  }
}
