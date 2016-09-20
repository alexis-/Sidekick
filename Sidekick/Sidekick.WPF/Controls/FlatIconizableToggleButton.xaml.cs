// 
// Microsoft Public License (Ms-PL)
// 
// This license governs use of the accompanying software. If you use the
// software, you accept this license. If you do not accept the license, do not
// use the software.
// 
// 1. Definitions
// The terms "reproduce," "reproduction," "derivative works," and
// "distribution" have the same meaning here as under U.S.copyright law.
// A "contribution" is the original software, or any additions or changes to
// the software.
// A "contributor" is any person that distributes its contribution under this
// license.
// "Licensed patents" are a contributor's patent claims that read directly on
// its contribution.
// 
// 2. Grant of Rights
// (A) Copyright Grant- Subject to the terms of this license, including the
// license conditions and limitations in section 3, each contributor grants
// you a non-exclusive, worldwide, royalty-free copyright license to reproduce
// its contribution, prepare derivative works of its contribution, and
// distribute its contribution or any derivative works that you create.
// (B) Patent Grant- Subject to the terms of this license, including the
// license conditions and limitations in section 3, each contributor grants
// you a non-exclusive, worldwide, royalty-free license under its licensed
// patents to make, have made, use, sell, offer for sale, import, and/or
// otherwise dispose of its contribution in the software or derivative works
// of the contribution in the software.
// 
// 3. Conditions and Limitations
// (A) No Trademark License- This license does not grant you rights to use any
// contributors' name, logo, or trademarks.
// (B) If you bring a patent claim against any contributor over patents that
// you claim are infringed by the software, your patent license from such
// contributor to the software ends automatically.
// (C) If you distribute any portion of the software, you must retain all
// copyright, patent, trademark, and attribution notices that are present in
// the software.
// (D) If you distribute any portion of the software in source code form, you
// may do so only under this license by including a complete copy of this
// license with your distribution. If you distribute any portion of the
// software in compiled or object code form, you may only do so under a
// license that complies with this license.
// (E) The software is licensed "as-is." You bear the risk of using it. The
// contributors give no express warranties, guarantees or conditions.You may
// have additional consumer rights under your local laws which this license
// cannot change.To the extent permitted under your local laws, the
// contributors exclude the implied warranties of merchantability, fitness for
// a particular purpose and non-infringement.
//
// Copious parts of this code from https://github.com/MahApps/MahApps.Metro/

namespace Sidekick.WPF.Controls
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Input;

    [TemplateVisualState(Name = NormalState, GroupName = CommonStates)]
    [TemplateVisualState(Name = DisabledState, GroupName = CommonStates)]
    [TemplatePart(Name = "PART_Button", Type = typeof(ToggleButton))]
    public class FlatIconizableToggleButton : FlatIconizableButtonBase<ToggleButton>
    {
        #region Fields

        //
        // Fields
        private const string CommonStates = "CommonStates";
        private const string NormalState = "Normal";
        private const string DisabledState = "Disabled";

        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register(
                "IsChecked", typeof(bool?), typeof(FlatIconizableToggleButton),
                new FrameworkPropertyMetadata(
                    false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnIsCheckedChanged));

        public static readonly DependencyProperty CheckChangedCommandProperty =
            DependencyProperty.Register(
                "CheckChangedCommand", typeof(ICommand), typeof(FlatIconizableToggleButton),
                new PropertyMetadata(null));

        public static readonly DependencyProperty CheckedCommandProperty =
            DependencyProperty.Register(
                "CheckedCommand", typeof(ICommand), typeof(FlatIconizableToggleButton),
                new PropertyMetadata(null));

        public static readonly DependencyProperty UnCheckedCommandProperty =
            DependencyProperty.Register(
                "UnCheckedCommand", typeof(ICommand), typeof(FlatIconizableToggleButton),
                new PropertyMetadata(null));

        public static readonly DependencyProperty CheckChangedCommandParameterProperty =
            DependencyProperty.Register(
                "CheckChangedCommandParameter", typeof(object),
                typeof(FlatIconizableToggleButton), new PropertyMetadata(null));

        public static readonly DependencyProperty CheckedCommandParameterProperty =
            DependencyProperty.Register(
                "CheckedCommandParameter", typeof(object), typeof(FlatIconizableToggleButton),
                new PropertyMetadata(null));

        public static readonly DependencyProperty UnCheckedCommandParameterProperty =
            DependencyProperty.Register(
                "UnCheckedCommandParameter", typeof(object), typeof(FlatIconizableToggleButton),
                new PropertyMetadata(null));

        public static readonly DependencyProperty RadioControllerProperty =
            DependencyProperty.Register(
                "RadioController", typeof(RadioController), typeof(FlatIconizableToggleButton),
                new PropertyMetadata(null, OnRadioControllerPropertyChanged));

        public static readonly DependencyProperty RadioControllerParameterProperty =
            DependencyProperty.Register(
                "RadioControllerParameter", typeof(object), typeof(FlatIconizableToggleButton),
                new PropertyMetadata(null));

        #endregion



        #region Constructors

        //
        // Constructors

        static FlatIconizableToggleButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(FlatIconizableToggleButton),
                new FrameworkPropertyMetadata(typeof(FlatIconizableToggleButton)));
        }

        #endregion



        #region Events

        public event EventHandler<RoutedEventArgs> Checked;
        public event EventHandler<RoutedEventArgs> Unchecked;
        public event EventHandler<RoutedEventArgs> Indeterminate;

        /// <summary>An event that is raised when the value of IsChecked changes.</summary>
        public event EventHandler IsCheckedChanged;

        #endregion



        #region Properties

        /// <summary>Gets/sets whether the control is Checked (On) or not (Off).</summary>
        [TypeConverter(typeof(NullableBoolConverter))]
        public bool? IsChecked
        {
            get { return (bool?)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        /// <summary>Gets/sets the command which will be executed if the IsChecked property was changed.</summary>
        public ICommand CheckChangedCommand
        {
            get { return (ICommand)GetValue(CheckChangedCommandProperty); }
            set { SetValue(CheckChangedCommandProperty, value); }
        }

        /// <summary>
        ///     Gets/sets the command which will be executed if the checked event of the control is
        ///     fired.
        /// </summary>
        public ICommand CheckedCommand
        {
            get { return (ICommand)GetValue(CheckedCommandProperty); }
            set { SetValue(CheckedCommandProperty, value); }
        }

        /// <summary>
        ///     Gets/sets the command which will be executed if the checked event of the control is
        ///     fired.
        /// </summary>
        public ICommand UnCheckedCommand
        {
            get { return (ICommand)GetValue(UnCheckedCommandProperty); }
            set { SetValue(UnCheckedCommandProperty, value); }
        }

        /// <summary>Gets/sets the command parameter which will be passed by the CheckChangedCommand.</summary>
        public object CheckChangedCommandParameter
        {
            get { return GetValue(CheckChangedCommandParameterProperty); }
            set { SetValue(CheckChangedCommandParameterProperty, value); }
        }

        /// <summary>Gets/sets the command parameter which will be passed by the CheckedCommand.</summary>
        public object CheckedCommandParameter
        {
            get { return GetValue(CheckedCommandParameterProperty); }
            set { SetValue(CheckedCommandParameterProperty, value); }
        }

        /// <summary>Gets/sets the command parameter which will be passed by the UnCheckedCommand.</summary>
        public object UnCheckedCommandParameter
        {
            get { return GetValue(UnCheckedCommandParameterProperty); }
            set { SetValue(UnCheckedCommandParameterProperty, value); }
        }

        /// <summary>Gets or sets the radio control panel to handle checking/unchecking.</summary>
        [Bindable(true)]
        public RadioController RadioController
        {
            get { return (RadioController)GetValue(RadioControllerProperty); }
            set { SetValue(RadioControllerProperty, value); }
        }

        /// <summary>Gets or sets the radio control parameter to be passed along with selection callbacks.</summary>
        [Bindable(true)]
        public object RadioControllerParameter
        {
            get { return GetValue(RadioControllerParameterProperty); }
            set { SetValue(RadioControllerParameterProperty, value); }
        }

        #endregion



        #region Methods

        //
        // Methods


        private static void OnIsCheckedChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var thisButton = (FlatIconizableToggleButton)d;

            if (thisButton.ButtonBase != null)
            {
                var oldValue = (bool?)e.OldValue;
                var newValue = (bool?)e.NewValue;

                if (oldValue != newValue)
                {
                    var command = thisButton.CheckChangedCommand;
                    var commandParameter = thisButton.CheckChangedCommandParameter ?? thisButton;

                    if (command != null && command.CanExecute(commandParameter))
                        command.Execute(commandParameter);

                    thisButton.IsCheckedChanged?.Invoke(thisButton, EventArgs.Empty);
                }
            }
        }

        private static void OnRadioControllerPropertyChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var thisButton = (FlatIconizableToggleButton)dependencyObject;

            if (thisButton.RadioController != null && thisButton.ButtonBase != null)
            {
                ToggleButton toggleButton = thisButton.ButtonBase as ToggleButton;

                thisButton.RadioController.AddElement(
                              toggleButton,
                              thisButton.RadioControllerParameter ?? thisButton.Name);
            }
        }

        private void ChangeVisualState(bool useTransitions)
        {
            VisualStateManager.GoToState(
                this, IsEnabled ? NormalState : DisabledState, useTransitions);
        }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (ButtonBase != null)
            {
                ButtonBase.Checked -= CheckedHandler;
                ButtonBase.Unchecked -= UncheckedHandler;
                ButtonBase.Indeterminate -= IndeterminateHandler;

                BindingOperations.ClearBinding(ButtonBase, ToggleButton.IsCheckedProperty);

                ButtonBase.IsEnabledChanged -= IsEnabledHandler;
            }

            ButtonBase = EnforceInstance<ToggleButton>("PART_Button");

            if (ButtonBase != null)
            {
                ButtonBase.Checked += CheckedHandler;
                ButtonBase.Unchecked += UncheckedHandler;
                ButtonBase.Indeterminate += IndeterminateHandler;

                var binding = new Binding("IsChecked") { Source = this };
                ButtonBase.SetBinding(ToggleButton.IsCheckedProperty, binding);

                ButtonBase.IsEnabledChanged += IsEnabledHandler;

                RadioController?.AddElement(ButtonBase, RadioControllerParameter);
            }

            ChangeVisualState(false);
        }

        private void IsEnabledHandler(object sender, DependencyPropertyChangedEventArgs e)
        {
            ChangeVisualState(false);
        }

        private void CheckedHandler(object sender, RoutedEventArgs e)
        {
            var command = CheckedCommand;
            var commandParameter = CheckedCommandParameter ?? this;
            if (command != null && command.CanExecute(commandParameter))
                command.Execute(commandParameter);

            SafeRaise.Raise(Checked, this, e);
        }

        private void UncheckedHandler(object sender, RoutedEventArgs e)
        {
            var command = UnCheckedCommand;
            var commandParameter = UnCheckedCommandParameter ?? this;
            if (command != null && command.CanExecute(commandParameter))
                command.Execute(commandParameter);

            SafeRaise.Raise(Unchecked, this, e);
        }

        private void IndeterminateHandler(object sender, RoutedEventArgs e)
        {
            SafeRaise.Raise(Indeterminate, this, e);
        }

        #endregion
    }
}