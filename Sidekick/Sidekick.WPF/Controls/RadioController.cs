﻿// 
// The MIT License (MIT)
// Copyright (c) 2016 Incogito
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Catel.Collections;
using Catel.Fody;

namespace Sidekick.WPF.Controls
{
  /// <summary>
  /// Act as a controller for toggleable elements.
  /// Ensure only a single time is checked at any time, like radio buttons.
  /// </summary>
  /// <seealso cref="System.Windows.DependencyObject" />
  public class RadioController : DependencyObject
  {
    public delegate Task<bool> SelectionChangeHandler(
      object selectedItem, object parameter);

    #region Fields

    public static readonly DependencyProperty ActiveProperty =
      DependencyProperty.Register(
        "Active", typeof(bool), typeof(RadioController),
        new PropertyMetadata(false));

    private readonly Dictionary<ToggleButton, object> _elements =
      new Dictionary<ToggleButton, object>();

    private bool _isSettingValue = false;

    #endregion

    #region Constructors

    public RadioController()
      : this(true)
    {
    }

    public RadioController(bool active)
    {
      Active = active;
    }

    #endregion

    #region Properties

    public bool Active
    {
      get { return (bool)GetValue(ActiveProperty); }
      set { SetValue(ActiveProperty, value); }
    }

    #endregion

    #region Methods

    public event SelectionChangeHandler OnSelectionChanged;

    //
    // Core methods

    public void AddElement(
      [NotNull] ToggleButton toggleButton, object parameter)
    {
      if (_elements.ContainsKey(toggleButton))
        return;

      toggleButton.Loaded += OnElementLoaded;
      toggleButton.Unloaded += OnElementUnloaded;
      toggleButton.Checked += OnElementCheckChange;
      toggleButton.Unchecked += OnElementCheckChange;

      _elements.Add(toggleButton, parameter);
    }

    public void RemoveElement([NotNull] ToggleButton toggleButton)
    {
      toggleButton.Loaded -= OnElementLoaded;
      toggleButton.Unloaded -= OnElementUnloaded;
      toggleButton.Checked -= OnElementCheckChange;
      toggleButton.Unchecked -= OnElementCheckChange;

      _elements.Remove(toggleButton);
    }

    private void EnsureCheckState([NotNull] ToggleButton toggleButton)
    {
      if (IsTrue(toggleButton.IsChecked))
      {
        Task<bool> allowChangeTask =
          OnSelectionChanged?.Invoke(toggleButton, _elements[toggleButton]);

        if (allowChangeTask == null)
          throw new InvalidOperationException("Task is NULL.");

        if (allowChangeTask.IsCompleted)
          OnToggledChecked(toggleButton, allowChangeTask.Result);

        else
#pragma warning disable 4014
          EnsureCheckStateAsync(toggleButton, allowChangeTask);
#pragma warning restore 4014
      }

      else if (!_elements.Keys.Any(e => IsTrue(e.IsChecked)))
        SetCheckedValue(toggleButton, true);
    }

    private async Task EnsureCheckStateAsync(
      [NotNull] ToggleButton toggleButton, Task<bool> allowChangeTask)
    {
      OnToggledChecked(toggleButton, await allowChangeTask);
    }

    private void OnToggledChecked(
      [NotNull] ToggleButton toggleButton, bool allowChange)
    {
      if (allowChange)
        _elements
          .Keys
          .Where(e => !Equals(e, toggleButton) && IsTrue(e.IsChecked))
          .ForEach(e => SetCheckedValue(e, false));

      else
        SetCheckedValue(toggleButton, false);
    }

    private void SetCheckedValue(ToggleButton toggleButton, bool isChecked)
    {
      _isSettingValue = true;
      toggleButton.IsChecked = isChecked;
      _isSettingValue = false;
    }

    private bool IsTrue(bool? value)
    {
      return value != null && value.Value;
    }


    //
    // Elements events handler

    private void OnElementCheckChange(
      object sender, RoutedEventArgs routedEventArgs)
    {
      if (_isSettingValue)
        return;

      EnsureCheckState(sender as ToggleButton);
    }

    private void OnElementUnloaded(
      object sender, RoutedEventArgs routedEventArgs)
    {
      RemoveElement(sender as ToggleButton);
    }

    private void OnElementLoaded(
      object sender, RoutedEventArgs routedEventArgs)
    {
      EnsureCheckState(sender as ToggleButton);
    }

    #endregion
  }


  /// <summary>
  /// RadioController attached property and TypeConverter.
  /// </summary>
  public static class RadioControllerHelper
  {
    #region Fields

    public static readonly DependencyProperty RadioControllerProperty =
      DependencyProperty.RegisterAttached(
        "RadioController", typeof(RadioController),
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
      DependencyObject obj, RadioController value)
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
    /// Convert string bool, or boolean values to RadioController
    /// </summary>
    /// <seealso cref="System.ComponentModel.TypeConverter" />
    private class RadioControllerConverter : TypeConverter
    {
      #region Methods

      public override bool CanConvertFrom(
        ITypeDescriptorContext context, Type sourceType)
      {
        return sourceType == typeof(string)
               || sourceType == typeof(bool);
      }

      public override object ConvertFrom(
        ITypeDescriptorContext context, CultureInfo culture, object value)
      {
        bool active = false;

        if (value is string)
        {
          if (!Boolean.TryParse((string)value, out active))
            return null;
        }

        else if (value is bool)
          active = (bool)value;

        return new RadioController(active);
      }

      #endregion
    }
  }
}