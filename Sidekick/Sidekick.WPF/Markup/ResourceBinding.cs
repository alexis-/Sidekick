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
using System.Windows.Data;
using System.Windows.Markup;

namespace Sidekick.WPF.Markup
{
  //
  // This one is borrowed from following thread :
  // http://stackoverflow.com/a/28647821/596579
  //
  namespace Mersoft.Mvvm.MarkupExtensions
  {
    public class ResourceBinding : MarkupExtension
    {
      #region Constructors

      public ResourceBinding()
      {
      }

      public ResourceBinding(string path)
      {
        this.Path = new PropertyPath(path);
      }

      #endregion

      #region Methods

      public override object ProvideValue(IServiceProvider serviceProvider)
      {
        var provideValueTargetService =
          (IProvideValueTarget)
          serviceProvider.GetService(typeof(IProvideValueTarget));
        if (provideValueTargetService == null)
          return null;

        if (provideValueTargetService.TargetObject != null &&
            provideValueTargetService.TargetObject.GetType().FullName ==
            "System.Windows.SharedDp")
          return this;


        var targetObject =
          provideValueTargetService.TargetObject as FrameworkElement;
        var targetProperty =
          provideValueTargetService.TargetProperty as DependencyProperty;
        if (targetObject == null || targetProperty == null)
          return null;

        //
        // Binding
        var binding = new Binding
        {
          Path = this.Path,
          XPath = this.XPath,
          Mode = this.Mode,
          UpdateSourceTrigger = this.UpdateSourceTrigger,
          Converter = this.Converter,
          ConverterParameter = this.ConverterParameter,
          ConverterCulture = this.ConverterCulture
        };

        if (this.RelativeSource != null)
          binding.RelativeSource = this.RelativeSource;

        if (this.ElementName != null)
          binding.ElementName = this.ElementName;

        if (this.Source != null)
          binding.Source = this.Source;

        binding.FallbackValue = this.FallbackValue;

        //
        // Multi-bind & converter
        var multiBinding = new MultiBinding
        {
          Converter = HelperConverter.Current,
          ConverterParameter = targetProperty,
          NotifyOnSourceUpdated = true
        };

        multiBinding.Bindings.Add(binding);

        targetObject.SetBinding(
          ResourceBindingKeyHelperProperty, multiBinding);

        return null;
      }

      #endregion

      #region Helper properties

      public static object GetResourceBindingKeyHelper(DependencyObject obj)
      {
        return (object)obj.GetValue(ResourceBindingKeyHelperProperty);
      }

      public static void SetResourceBindingKeyHelper(DependencyObject obj,
        object value)
      {
        obj.SetValue(ResourceBindingKeyHelperProperty, value);
      }
      
      public static readonly DependencyProperty
        ResourceBindingKeyHelperProperty =
          DependencyProperty.RegisterAttached("ResourceBindingKeyHelper",
            typeof(object), typeof(ResourceBinding),
            new PropertyMetadata(null, ResourceKeyChanged));

      static void ResourceKeyChanged(DependencyObject d,
        DependencyPropertyChangedEventArgs e)
      {
        var target = d as FrameworkElement;
        var newVal = e.NewValue as Tuple<object, DependencyProperty>;

        if (target == null || newVal == null)
          return;

        var dp = newVal.Item2;

        if (newVal.Item1 == null)
        {
          target.SetValue(dp, dp.GetMetadata(target).DefaultValue);
          return;
        }

        target.SetResourceReference(dp, newVal.Item1);
      }

      #endregion

      #region BindingBase Members

      /// <summary> Value to use when source cannot provide a value </summary>
      /// <remarks>
      ///   Initialized to DependencyProperty.UnsetValue; if FallbackValue
      ///   is not set, BindingExpression will return target property's
      ///   default when Binding cannot get a real value.
      /// </remarks>
      public object FallbackValue { get; set; }

      #endregion

      #region Binding Members

      /// <summary> The source path (for CLR bindings).</summary>
      public object Source { get; set; }

      /// <summary> The source path (for CLR bindings).</summary>
      public PropertyPath Path { get; set; }

      /// <summary> The XPath path (for XML bindings).</summary>
      [DefaultValue(null)]
      public string XPath { get; set; }

      /// <summary> Binding mode </summary>
      [DefaultValue(BindingMode.Default)]
      public BindingMode Mode { get; set; }

      /// <summary> Update type </summary>
      [DefaultValue(UpdateSourceTrigger.Default)]
      public UpdateSourceTrigger UpdateSourceTrigger { get; set; }

      /// <summary> The Converter to apply </summary>
      [DefaultValue(null)]
      public IValueConverter Converter { get; set; }

      /// <summary>
      /// The parameter to pass to converter.
      /// </summary>
      /// <value></value>
      [DefaultValue(null)]
      public object ConverterParameter { get; set; }

      /// <summary> Culture in which to evaluate the converter </summary>
      [DefaultValue(null)]
      [TypeConverter(typeof(System.Windows.CultureInfoIetfLanguageTagConverter)
        )]
      public CultureInfo ConverterCulture { get; set; }

      /// <summary>
      /// Description of the object to use as the source, relative to the target element.
      /// </summary>
      [DefaultValue(null)]
      public RelativeSource RelativeSource { get; set; }

      /// <summary> Name of the element to use as the source </summary>
      [DefaultValue(null)]
      public string ElementName { get; set; }

      #endregion

      #region Nested types

      private class HelperConverter : IMultiValueConverter
      {
        #region Fields

        public static readonly HelperConverter Current =
          new HelperConverter();

        #endregion

        #region Methods

        public object Convert(object[] values, Type targetType,
          object parameter, CultureInfo culture)
        {
          return Tuple.Create(values[0], (DependencyProperty)parameter);
        }

        public object[] ConvertBack(object value, Type[] targetTypes,
          object parameter, CultureInfo culture)
        {
          throw new NotImplementedException();
        }

        #endregion
      }

      #endregion
    }
  }
}