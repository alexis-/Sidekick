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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Sidekick.WPF.Controls
{
  public abstract class FlatIconizableButtonBase<T> : FlatIconizableButtonBase
    where T : ButtonBase, new()
  {
    #region Fields

    protected T ButtonBase;

    #endregion

    #region Methods

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      ButtonBase = EnforceInstance<T>("PART_Button");
      InitializeVisualElementsContainer();
    }

    /// <summary>
    /// Get element from name. If it exist then element instance return,
    /// if not, new will be created.
    /// </summary>
    /// <typeparam name="TElement">Type of the element.</typeparam>
    /// <param name="partName">Name of template part.</param>
    /// <returns>Element</returns>
    protected TElement EnforceInstance<TElement>(string partName)
      where TElement : FrameworkElement, new()
    {
      return GetTemplateChild(partName) as TElement
             ?? new TElement();
    }

    protected virtual void InitializeVisualElementsContainer()
    {
      ButtonBase.Click -= ButtonClick;
      ButtonBase.Click += ButtonClick;
    }

    #endregion
  }

  [TemplatePart(Name = "PART_ButtonContent", Type = typeof(ContentControl))]
  public abstract class FlatIconizableButtonBase : ContentControl
  {
    #region Fields

    //
    // Fields

    public static readonly DependencyProperty OrientationProperty =
      DependencyProperty.Register(
        "Orientation", typeof(Orientation), typeof(FlatIconizableButtonBase),
        new FrameworkPropertyMetadata(
          Orientation.Horizontal,
          FrameworkPropertyMetadataOptions.AffectsMeasure));

    public static readonly DependencyProperty IconProperty =
      DependencyProperty.Register(
        "Icon", typeof(Object), typeof(FlatIconizableButtonBase));

    public static readonly DependencyProperty IconSizeProperty =
      DependencyProperty.Register(
        "IconSize", typeof(Size), typeof(FlatIconizableButtonBase));

    public static readonly DependencyProperty IconTemplateProperty =
      DependencyProperty.Register(
        "IconTemplate", typeof(DataTemplate),
        typeof(FlatIconizableButtonBase));

    public static readonly DependencyProperty CommandProperty =
      DependencyProperty.Register(
        "Command", typeof(ICommand), typeof(FlatIconizableButtonBase));

    public static readonly DependencyProperty CommandTargetProperty =
      DependencyProperty.Register(
        "CommandTarget", typeof(IInputElement),
        typeof(FlatIconizableButtonBase));

    public static readonly DependencyProperty CommandParameterProperty =
      DependencyProperty.Register(
        "CommandParameter", typeof(Object), typeof(FlatIconizableButtonBase));

    public static readonly DependencyProperty MouseOverBrushProperty =
        DependencyProperty.Register(
          "MouseOverBrush", typeof(Brush), typeof(FlatIconizableButtonBase),
          new FrameworkPropertyMetadata(
            default(Brush), FrameworkPropertyMetadataOptions.AffectsRender));
    
    public static readonly DependencyProperty PressedBrushProperty =
        DependencyProperty.Register(
          "PressedBrush", typeof(Brush), typeof(FlatIconizableButtonBase),
          new FrameworkPropertyMetadata(
            default(Brush), FrameworkPropertyMetadataOptions.AffectsRender));
    
    public static readonly DependencyProperty ButtonPaddingProperty =
      DependencyProperty.Register(
        "ButtonPadding", typeof(Thickness), typeof(FlatIconizableButtonBase));

    public static readonly DependencyProperty ButtonStyleProperty =
      DependencyProperty.Register(
        "ButtonStyle", typeof(Style), typeof(FlatIconizableButtonBase),
        new FrameworkPropertyMetadata(
          default(Style), FrameworkPropertyMetadataOptions.Inherits
                          | FrameworkPropertyMetadataOptions.AffectsArrange
                          | FrameworkPropertyMetadataOptions.AffectsMeasure));

    public static readonly RoutedEvent ClickEvent =
      EventManager.RegisterRoutedEvent(
        "Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler),
        typeof(FlatIconizableButtonBase));

    #endregion

    #region Properties

    //
    // Properties

    /// <summary>
    /// Reflects the parameter to pass to the CommandProperty upon execution. 
    /// </summary>
    public Object CommandParameter
    {
      get { return GetValue(CommandParameterProperty); }
      set { SetValue(CommandParameterProperty, value); }
    }

    /// <summary>
    /// Gets or sets the target element on which to fire the command.
    /// </summary>
    public IInputElement CommandTarget
    {
      get { return (IInputElement)GetValue(CommandTargetProperty); }
      set { SetValue(CommandTargetProperty, value); }
    }

    /// <summary>
    /// Get or sets the Command property. 
    /// </summary>
    public ICommand Command
    {
      get { return (ICommand)GetValue(CommandProperty); }
      set { SetValue(CommandProperty, value); }
    }

    /// <summary>
    /// Gets or sets the dimension of children stacking.
    /// </summary>
    public Orientation Orientation
    {
      get { return (Orientation)GetValue(OrientationProperty); }
      set { SetValue(OrientationProperty, value); }
    }

    /// <summary>
    ///  Gets or sets the Content used to generate the icon part.
    /// </summary>
    [Bindable(true)]
    public Object Icon
    {
      get { return GetValue(IconProperty); }
      set { SetValue(IconProperty, value); }
    }

    /// <summary>
    /// Gets or sets the size of the Content used to display the icon.
    /// </summary>
    [Bindable(true)]
    public Size IconSize
    {
      get { return (Size)GetValue(IconSizeProperty); }
      set { SetValue(IconSizeProperty, value); }
    }

    /// <summary> 
    /// Gets or sets the ContentTemplate used to display the content of the icon part. 
    /// </summary>
    [Bindable(true)]
    public DataTemplate IconTemplate
    {
      get { return (DataTemplate)GetValue(IconTemplateProperty); }
      set { SetValue(IconTemplateProperty, value); }
    }

    public Brush MouseOverBrush
    {
      get { return (Brush)GetValue(MouseOverBrushProperty); }
      set { SetValue(MouseOverBrushProperty, value); }
    }
    public Brush PressedBrush
    {
      get { return (Brush)GetValue(PressedBrushProperty); }
      set { SetValue(PressedBrushProperty, value); }
    }

    /// <summary>
    /// Gets or sets the button padding.
    /// </summary>
    [Bindable(true)]
    public Thickness ButtonPadding
    {
      get { return (Thickness)GetValue(ButtonPaddingProperty); }
      set { SetValue(ButtonPaddingProperty, value); }
    }

    /// <summary>
    /// Gets/sets the button style.
    /// </summary>
    public Style ButtonStyle
    {
      get { return (Style)GetValue(ButtonStyleProperty); }
      set { SetValue(ButtonStyleProperty, value); }
    }

    #endregion

    #region Methods

    //
    // Methods

    public event RoutedEventHandler Click
    {
      add { AddHandler(ClickEvent, value); }
      remove { RemoveHandler(ClickEvent, value); }
    }

    protected void ButtonClick(object sender, RoutedEventArgs e)
    {
      e.RoutedEvent = ClickEvent;

      RaiseEvent(e);
    }

    #endregion
  }
}