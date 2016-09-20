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

namespace Sidekick.WPF.Controls
{
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading.Tasks;
  using System.Windows;
  using System.Windows.Controls.Primitives;

  using Catel.Collections;
  using Catel.Fody;

  /// <summary>
  ///   Callback interface for RadioController class.
  /// </summary>
  public interface IRadioControllerMonitor
  {
    #region Methods

    /// <summary>
    ///   Notify on selection changes and allows to asynchronously validate whether to endorse it
    ///   or not.
    /// </summary>
    /// <param name="selectedItem">The selected item.</param>
    /// <param name="parameter">Item context (e.g. binding).</param>
    /// <returns>Waitable task for validation result.</returns>
    Task<bool> RadioControllerOnSelectionChangedAsync(object selectedItem, object parameter);

    #endregion
  }

  /// <summary>
  ///   Act as a controller for toggleable elements.
  ///   Ensure only a single time is checked at any time, like radio buttons.
  /// </summary>
  /// <seealso cref="System.Windows.DependencyObject" />
  public class RadioController : DependencyObject
  {
    #region Fields

    private readonly Dictionary<ToggleButton, object> _elements =
      new Dictionary<ToggleButton, object>();

    private readonly IRadioControllerMonitor _monitor;

    private bool _isSettingValue;

    #endregion

    #region Constructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="RadioController" /> class.
    ///   Default constructor with no monitor.
    /// </summary>
    public RadioController()
      : this(null)
    {
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="RadioController" /> class.
    ///   Allow to provide a monitor implementation instance.
    /// </summary>
    /// <param name="monitor">The monitor.</param>
    public RadioController(IRadioControllerMonitor monitor)
    {
      _monitor = monitor;
    }

    #endregion

    #region Methods

    //
    // Core methods

    /// <summary>
    ///   Adds a new toggleable button to this controller.
    /// </summary>
    /// <param name="toggleButton">The toggle button.</param>
    /// <param name="parameter">The parameter.</param>
    public void AddElement([NotNull] ToggleButton toggleButton, object parameter)
    {
      if (_elements.ContainsKey(toggleButton))
        return;

      toggleButton.Loaded += OnElementLoaded;
      toggleButton.Unloaded += OnElementUnloaded;
      toggleButton.Checked += OnElementCheckChange;
      toggleButton.Unchecked += OnElementCheckChange;

      _elements.Add(toggleButton, parameter);
    }

    /// <summary>
    ///   Removes a toggleable button from list.
    /// </summary>
    /// <param name="toggleButton">The toggle button.</param>
    public void RemoveElement([NotNull] ToggleButton toggleButton)
    {
      if (!_elements.ContainsKey(toggleButton))
        throw new System.InvalidOperationException("No such toggleable button instance.");

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
        Task<bool> allowChangeTask = _monitor?.RadioControllerOnSelectionChangedAsync(
          toggleButton,
          _elements[toggleButton]);

        if (allowChangeTask == null || allowChangeTask.IsCompleted)
          OnToggledChecked(toggleButton, allowChangeTask == null || allowChangeTask.Result);

        else
#pragma warning disable 4014
          EnsureCheckStateAsync(toggleButton, allowChangeTask);
#pragma warning restore 4014
      }

      else if (!_elements.Keys.Any(e => IsTrue(e.IsChecked)))
        SetCheckedValue(toggleButton, true);
    }

    private async Task EnsureCheckStateAsync(
      [NotNull] ToggleButton toggleButton,
      Task<bool> allowChangeTask)
    {
      OnToggledChecked(toggleButton, await allowChangeTask);
    }

    private void OnToggledChecked([NotNull] ToggleButton toggleButton, bool allowChange)
    {
      if (allowChange)
        _elements.Keys.Where(e => !Equals(e, toggleButton) && IsTrue(e.IsChecked))
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

    private void OnElementCheckChange(object sender, RoutedEventArgs routedEventArgs)
    {
      if (_isSettingValue)
        return;

      EnsureCheckState(sender as ToggleButton);
    }

    private void OnElementUnloaded(object sender, RoutedEventArgs routedEventArgs)
    {
      RemoveElement(sender as ToggleButton);
    }

    private void OnElementLoaded(object sender, RoutedEventArgs routedEventArgs)
    {
      EnsureCheckState(sender as ToggleButton);
    }

    #endregion
  }
}