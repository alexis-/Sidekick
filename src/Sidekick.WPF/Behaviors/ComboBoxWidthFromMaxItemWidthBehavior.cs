using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Sidekick.WPF.Extensions;

namespace Sidekick.WPF.Behaviors
{
  public static class ComboBoxWidthFromMaxItemWidthBehavior
  {
    public static readonly DependencyProperty
      ComboBoxWidthFromMaxItemWidthProperty =
        DependencyProperty.RegisterAttached(
          "ComboBoxWidthFromMaxItemWidth", typeof(bool),
          typeof(ComboBoxWidthFromMaxItemWidthBehavior),
          new UIPropertyMetadata(
            false, OnComboBoxWidthFromMaxItemWidthPropertyChanged));

    public static bool GetComboBoxWidthFromMaxItemWidth(DependencyObject obj)
    {
      return (bool)obj.GetValue(ComboBoxWidthFromMaxItemWidthProperty);
    }

    public static void SetComboBoxWidthFromMaxItemWidth(
      DependencyObject obj, bool value)
    {
      obj.SetValue(ComboBoxWidthFromMaxItemWidthProperty, value);
    }

    private static void OnComboBoxWidthFromMaxItemWidthPropertyChanged(
      DependencyObject dpo, DependencyPropertyChangedEventArgs e)
    {
      ComboBox comboBox = dpo as ComboBox;

      if (comboBox != null)
      {
        if ((bool)e.NewValue)
          comboBox.Loaded += OnComboBoxLoaded;

        else
          comboBox.Loaded -= OnComboBoxLoaded;
      }
    }

    private static void OnComboBoxLoaded(object sender, RoutedEventArgs e)
    {
      ComboBox comboBox = sender as ComboBox;
      Action action = () => { comboBox.SetWidthFromItems(); };
      comboBox.Dispatcher.BeginInvoke(action, DispatcherPriority.ContextIdle);
    }
  }
}
