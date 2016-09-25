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

namespace Sidekick.Windows.Services
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Runtime.CompilerServices;
  using System.Windows;
  using System.Windows.Input;

  using Catel;
  using Catel.Logging;
  using Catel.MVVM;

  using Sidekick.Shared.Extensions;
  using Sidekick.Windows.Services.Interfaces;

  using InputGesture = Catel.Windows.Input.InputGesture;

  /// <summary>
  ///   Manager that takes care of application-wide commands and can dynamically forward them
  ///   to the right view models.
  /// </summary>
  public class CommandManagerService : ICommandManagerEx
  {
    #region Fields

    private static readonly ILog Log = LogManager.GetCurrentClassLogger();
    private readonly Dictionary<string, List<InputGesture>> _commandGestures =
      new Dictionary<string, List<InputGesture>>();
    private readonly Dictionary<string, ICompositeCommand> _commands =
      new Dictionary<string, ICompositeCommand>();
    private readonly Dictionary<string, object> _commandsParameter =
      new Dictionary<string, object>();

    private readonly object _lockObject = new object();

    private readonly Dictionary<string, InputGesture> _originalCommandGestures =
      new Dictionary<string, InputGesture>();
    private readonly ConditionalWeakTable<FrameworkElement, CommandManagerWrapper>
      _subscribedViews = new ConditionalWeakTable<FrameworkElement, CommandManagerWrapper>();

    private bool _subscribedToApplicationActivedEvent;

    private bool _suspendedKeyboardEvents;

    #endregion



    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandManagerService" /> class.
    /// </summary>
    public CommandManagerService()
    {
      SubscribeToKeyboardEvents();
    }

    #endregion



    #region Events

    /// <summary>Occurs when a command has been created.</summary>
    public event EventHandler<CommandCreatedEventArgs> CommandCreated;

    #endregion



    #region Properties

    /// <summary>Gets or sets a value indicating whether the keyboard events are suspended.</summary>
    /// <value><c>true</c> if the keyboard events are suspended; otherwise, <c>false</c>.</value>
    public bool IsKeyboardEventsSuspended
    {
      get { return _suspendedKeyboardEvents; }
      set
      {
        if (_suspendedKeyboardEvents == value)
          return;

        _suspendedKeyboardEvents = value;

        Log.Debug(value ? "Suspended keyboard events" : "Resumed keyboard events");
      }
    }

    #endregion



    #region Methods

    /// <summary>
    ///   Creates the command inside the command manager.
    ///   <para />
    ///   If the <paramref name="throwExceptionWhenCommandIsAlreadyCreated" /> is <c>false</c> and the
    ///   command is already created, only the input gesture is updated for the existing command.
    /// </summary>
    /// <param name="commandName">Name of the command.</param>
    /// <param name="inputGesture">The input gesture.</param>
    /// <param name="compositeCommand">
    ///   The composite command. If <c>null</c>, this will default to a
    ///   new instance of <see cref="CompositeCommand" />.
    /// </param>
    /// <param name="throwExceptionWhenCommandIsAlreadyCreated">
    ///   if set to <c>true</c>, this method
    ///   will throw an exception when the command is already created.
    /// </param>
    /// <exception cref="ArgumentException">
    ///   The <paramref name="commandName" /> is <c>null</c> or
    ///   whitespace.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///   The specified command is already created using the
    ///   <see cref="CreateCommand" /> method.
    /// </exception>
    public void CreateCommand(
      string commandName, InputGesture inputGesture = null,
      ICompositeCommand compositeCommand = null,
      bool throwExceptionWhenCommandIsAlreadyCreated = true)
    {
      Argument.IsNotNullOrWhitespace("commandName", commandName);

      lock (_lockObject)
      {
        Log.Debug(
          "Creating command '{0}' with input gesture '{1}'", commandName,
          ObjectToStringHelper.ToString(inputGesture));

        if (_commands.ContainsKey(commandName))
        {
          if (throwExceptionWhenCommandIsAlreadyCreated)
          {
            var error =
              $"Command '{commandName}' is already created using the CreateCommand method";
            Log.Error(error);

            throw new InvalidOperationException(error);
          }

          _commandGestures[commandName].Add(inputGesture);
          return;
        }

        if (compositeCommand == null)
          compositeCommand = new CompositeCommand();

        _commands.Add(commandName, compositeCommand);
        _originalCommandGestures.Add(commandName, inputGesture);
        _commandGestures.Add(commandName, new List<InputGesture>(new[] { inputGesture }));

        CommandCreated.SafeInvoke(
          this, () => new CommandCreatedEventArgs(compositeCommand, commandName));
      }
    }

    /// <summary>Invalidates the all the currently registered commands.</summary>
    public void InvalidateCommands()
    {
      lock (_lockObject)
        foreach (var commandName in _commands.Keys)
        {
          var command = _commands[commandName];

          command?.RaiseCanExecuteChanged();
        }
    }

    /// <summary>Gets all the registered commands.</summary>
    /// <returns>The names of the commands.</returns>
    public IEnumerable<string> GetCommands()
    {
      lock (_lockObject)
        return _commands.Keys.ToList();
    }

    /// <summary>Gets the command created with the command name.</summary>
    /// <param name="commandName">Name of the command.</param>
    /// <returns>The <see cref="ICommand" /> or <c>null</c> if the command is not created.</returns>
    /// <exception cref="ArgumentException">
    ///   The <paramref name="commandName" /> is <c>null</c> or
    ///   whitespace.
    /// </exception>
    public ICommand GetCommand(string commandName)
    {
      Argument.IsNotNullOrWhitespace("commandName", commandName);

      lock (_lockObject)
      {
        if (_commands.ContainsKey(commandName))
          return _commands[commandName];

        return null;
      }
    }

    /// <summary>Determines whether the specified command name is created.</summary>
    /// <param name="commandName">Name of the command.</param>
    /// <returns><c>true</c> if the specified command name is created; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentException">
    ///   The <paramref name="commandName" /> is <c>null</c> or
    ///   whitespace.
    /// </exception>
    public bool IsCommandCreated(string commandName)
    {
      Argument.IsNotNullOrWhitespace("commandName", commandName);

      lock (_lockObject)
        return _commands.ContainsKey(commandName);
    }

    /// <summary>Executes the command.</summary>
    /// <param name="commandName">Name of the command.</param>
    /// <exception cref="ArgumentException">
    ///   The <paramref name="commandName" /> is <c>null</c> or
    ///   whitespace.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///   The specified command is not created using the
    ///   <see cref="CreateCommand" /> method.
    /// </exception>
    public void ExecuteCommand(string commandName)
    {
      Argument.IsNotNullOrWhitespace("commandName", commandName);

      lock (_lockObject)
      {
        Log.Debug("Executing command '{0}'", commandName);

        if (!_commands.ContainsKey(commandName))
          throw Log.ErrorAndCreateException<InvalidOperationException>(
            "Command '{0}' is not yet created using the CreateCommand method", commandName);

        _commands[commandName].Execute(_commandsParameter.SafeGet(commandName));
      }
    }

    /// <summary>Registers a command with the specified command name.</summary>
    /// <param name="commandName">Name of the command.</param>
    /// <param name="command">The command.</param>
    /// <param name="viewModel">The view model.</param>
    /// <exception cref="ArgumentException">
    ///   The <paramref name="commandName" /> is <c>null</c> or
    ///   whitespace.
    /// </exception>
    /// <exception cref="ArgumentNullException">The <paramref name="command" /> is <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException">
    ///   The specified command is not created using the
    ///   <see cref="CreateCommand" /> method.
    /// </exception>
    public void RegisterCommand(
      string commandName, ICommand command, IViewModel viewModel = null)
    {
      Argument.IsNotNullOrWhitespace("commandName", commandName);
      Argument.IsNotNull("command", command);

      if (CatelEnvironment.IsInDesignMode)
        return;

      lock (_lockObject)
      {
        Log.Debug("Registering command to '{0}'", commandName);

        if (!_commands.ContainsKey(commandName))
          throw Log.ErrorAndCreateException<InvalidOperationException>(
            "Command '{0}' is not yet created using the CreateCommand method", commandName);

        _commands[commandName].RegisterCommand(command, viewModel);

        InvalidateCommands();
      }
    }

    /// <summary>Registers a command with the specified command name.</summary>
    /// <param name="commandName">Name of the command.</param>
    /// <param name="command">The command.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="viewModel">The view model.</param>
    public void RegisterCommand(
      string commandName, ICommand command, object parameter, IViewModel viewModel = null)
    {
      Argument.IsNotNull(() => parameter);

      if (CatelEnvironment.IsInDesignMode)
        return;
      
      RegisterCommand(commandName, command, viewModel);

      lock (_lockObject)
      {
        _commandsParameter[commandName] = parameter;
      }
    }

    /// <summary>Registers the action with the specified command name.</summary>
    /// <param name="commandName">Name of the command.</param>
    /// <param name="action">The action.</param>
    /// <exception cref="ArgumentException">
    ///   The <paramref name="commandName" /> is <c>null</c> or
    ///   whitespace.
    /// </exception>
    /// <exception cref="ArgumentNullException">The <paramref name="action" /> is <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException">
    ///   The specified command is not created using the
    ///   <see cref="CreateCommand" /> method.
    /// </exception>
    public void RegisterAction(string commandName, Action action)
    {
      Argument.IsNotNullOrWhitespace("commandName", commandName);
      Argument.IsNotNull("action", action);

      if (CatelEnvironment.IsInDesignMode)
        return;

      lock (_lockObject)
      {
        Log.Debug("Registering action to '{0}'", commandName);

        if (!_commands.ContainsKey(commandName))
          throw Log.ErrorAndCreateException<InvalidOperationException>(
            "Command '{0}' is not yet created using the CreateCommand method", commandName);

        _commands[commandName].RegisterAction(action);

        InvalidateCommands();
      }
    }

    /// <summary>Registers the action with the specified command name.</summary>
    /// <param name="commandName">Name of the command.</param>
    /// <param name="action">The action.</param>
    /// <exception cref="ArgumentException">
    ///   The <paramref name="commandName" /> is <c>null</c> or
    ///   whitespace.
    /// </exception>
    /// <exception cref="ArgumentNullException">The <paramref name="action" /> is <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException">
    ///   The specified command is not created using the
    ///   <see cref="CreateCommand" /> method.
    /// </exception>
    public void RegisterAction(string commandName, Action<object> action)
    {
      Argument.IsNotNullOrWhitespace("commandName", commandName);
      Argument.IsNotNull("action", action);

      if (CatelEnvironment.IsInDesignMode)
        return;

      lock (_lockObject)
      {
        Log.Debug("Registering action to '{0}'", commandName);

        if (!_commands.ContainsKey(commandName))
          throw Log.ErrorAndCreateException<InvalidOperationException>(
            "Command '{0}' is not yet created using the CreateCommand method", commandName);

        _commands[commandName].RegisterAction(action);

        InvalidateCommands();
      }
    }

    /// <summary>Unregisters a command with the specified command name.</summary>
    /// <param name="commandName">Name of the command.</param>
    /// <param name="command">The command.</param>
    /// <exception cref="ArgumentException">
    ///   The <paramref name="commandName" /> is <c>null</c> or
    ///   whitespace.
    /// </exception>
    /// <exception cref="ArgumentNullException">The <paramref name="command" /> is <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException">
    ///   The specified command is not created using the
    ///   <see cref="CreateCommand" /> method.
    /// </exception>
    public void UnregisterCommand(string commandName, ICommand command)
    {
      Argument.IsNotNullOrWhitespace("commandName", commandName);
      Argument.IsNotNull("command", command);

      if (CatelEnvironment.IsInDesignMode)
        return;

      lock (_lockObject)
      {
        Log.Debug("Unregistering command from '{0}'", commandName);

        if (!_commands.ContainsKey(commandName))
          throw Log.ErrorAndCreateException<InvalidOperationException>(
            "Command '{0}' is not yet created using the CreateCommand method", commandName);

        _commands[commandName].UnregisterCommand(command);
        _commandsParameter.Remove(commandName);

        InvalidateCommands();
      }
    }

    /// <summary>Unregisters the action with the specified command name.</summary>
    /// <param name="commandName">Name of the command.</param>
    /// <param name="action">The action.</param>
    /// <exception cref="ArgumentException">
    ///   The <paramref name="commandName" /> is <c>null</c> or
    ///   whitespace.
    /// </exception>
    /// <exception cref="ArgumentNullException">The <paramref name="action" /> is <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException">
    ///   The specified command is not created using the
    ///   <see cref="CreateCommand" /> method.
    /// </exception>
    public void UnregisterAction(string commandName, Action action)
    {
      Argument.IsNotNullOrWhitespace("commandName", commandName);
      Argument.IsNotNull("action", action);

      if (CatelEnvironment.IsInDesignMode)
        return;

      lock (_lockObject)
      {
        Log.Debug("Unregistering action from '{0}'", commandName);

        if (!_commands.ContainsKey(commandName))
          throw Log.ErrorAndCreateException<InvalidOperationException>(
            "Command '{0}' is not yet created using the CreateCommand method", commandName);

        _commands[commandName].UnregisterAction(action);

        InvalidateCommands();
      }
    }

    /// <summary>Unregisters the action with the specified command name.</summary>
    /// <param name="commandName">Name of the command.</param>
    /// <param name="action">The action.</param>
    /// <exception cref="ArgumentException">
    ///   The <paramref name="commandName" /> is <c>null</c> or
    ///   whitespace.
    /// </exception>
    /// <exception cref="ArgumentNullException">The <paramref name="action" /> is <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException">
    ///   The specified command is not created using the
    ///   <see cref="CreateCommand" /> method.
    /// </exception>
    public void UnregisterAction(string commandName, Action<object> action)
    {
      Argument.IsNotNullOrWhitespace("commandName", commandName);
      Argument.IsNotNull("action", action);

      if (CatelEnvironment.IsInDesignMode)
        return;

      lock (_lockObject)
      {
        Log.Debug("Unregistering action from '{0}'", commandName);

        if (!_commands.ContainsKey(commandName))
          throw Log.ErrorAndCreateException<InvalidOperationException>(
            "Command '{0}' is not yet created using the CreateCommand method", commandName);

        _commands[commandName].UnregisterAction(action);

        InvalidateCommands();
      }
    }

    /// <summary>Gets the original input gesture with which the command was initially created.</summary>
    /// <param name="commandName">Name of the command.</param>
    /// <returns>
    ///   The input gesture or <c>null</c> if there is no input gesture for the specified
    ///   command.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///   The specified command is not created using the
    ///   <see cref="CreateCommand" /> method.
    /// </exception>
    public InputGesture GetOriginalInputGesture(string commandName)
    {
      Argument.IsNotNullOrWhitespace("commandName", commandName);

      lock (_lockObject)
      {
        if (!_commands.ContainsKey(commandName))
          throw Log.ErrorAndCreateException<InvalidOperationException>(
            "Command '{0}' is not yet created using the CreateCommand method", commandName);

        return _originalCommandGestures[commandName];
      }
    }

    /// <summary>Gets the input gesture for the specified command.</summary>
    /// <param name="commandName">Name of the command.</param>
    /// <returns>
    ///   The input gesture or <c>null</c> if there is no input gesture for the specified
    ///   command.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///   The specified command is not created using the
    ///   <see cref="CreateCommand" /> method.
    /// </exception>
    public InputGesture GetInputGesture(string commandName)
    {
      Argument.IsNotNullOrWhitespace("commandName", commandName);

      lock (_lockObject)
      {
        if (!_commands.ContainsKey(commandName))
          throw Log.ErrorAndCreateException<InvalidOperationException>(
            "Command '{0}' is not yet created using the CreateCommand method", commandName);

        return _commandGestures[commandName].FirstOrDefault();
      }
    }

    /// <summary>Updates the input gesture for the specified command.</summary>
    /// <param name="commandName">Name of the command.</param>
    /// <param name="inputGesture">The new input gesture.</param>
    /// <exception cref="ArgumentException">
    ///   The <paramref name="commandName" /> is <c>null</c> or
    ///   whitespace.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///   The specified command is not created using the
    ///   <see cref="CreateCommand" /> method.
    /// </exception>
    public void UpdateInputGesture(string commandName, InputGesture inputGesture = null)
    {
      Argument.IsNotNullOrWhitespace("commandName", commandName);

      lock (_lockObject)
      {
        Log.Debug(
          "Updating input gesture of command '{0}' to '{1}'", commandName,
          ObjectToStringHelper.ToString(inputGesture));

        if (!_commands.ContainsKey(commandName))
          throw Log.ErrorAndCreateException<InvalidOperationException>(
            "Command '{0}' is not yet created using the CreateCommand method", commandName);

        if (!_commandGestures[commandName].Contains(inputGesture))
          _commandGestures[commandName].Add(inputGesture);
      }
    }

    /// <summary>
    ///   Resets the input gestures to the original input gestures with which the commands were
    ///   registered.
    /// </summary>
    public void ResetInputGestures()
    {
      lock (_lockObject)
      {
        Log.Info("Resetting input gestures");

        foreach (var command in _commands)
        {
          Log.Debug(
            "Resetting input gesture for command '{0}' to '{1}'", command.Key,
            _originalCommandGestures[command.Key]);

          var originalGesture = _originalCommandGestures[command.Key];

          _commandGestures[command.Key].Clear();
          _commandGestures[command.Key].Add(originalGesture);
        }
      }
    }

    /// <summary>Subscribes to keyboard events.</summary>
    public void SubscribeToKeyboardEvents()
    {
      var application = Application.Current;
      if (application == null)
      {
        Log.Warning("Application.Current is null, cannot subscribe to keyboard events");
        return;
      }

      FrameworkElement mainView = application.MainWindow;
      if (mainView == null)
      {
        if (!_subscribedToApplicationActivedEvent)
        {
          application.Activated += (sender, e) => SubscribeToKeyboardEvents();
          _subscribedToApplicationActivedEvent = true;
          Log.Info(
            "Application.MainWindow is null, cannot subscribe to keyboard events, subscribed to Application.Activated event");
        }

        return;
      }

      SubscribeToKeyboardEvents(mainView);
    }

    /// <summary>Subscribes to keyboard events.</summary>
    /// <param name="view">The view.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="view" /> is <c>null</c>.</exception>
    public void SubscribeToKeyboardEvents(FrameworkElement view)
    {
      Argument.IsNotNull("view", view);

      CommandManagerWrapper commandManagerWrapper;

      if (!_subscribedViews.TryGetValue(view, out commandManagerWrapper))
      {
        _subscribedViews.Add(view, new CommandManagerWrapper(view, this));

        var app = Application.Current;
        if (app != null)
        {
          var mainWindow = app.MainWindow;
          if (ReferenceEquals(mainWindow, view))
            EventManager.RegisterClassHandler(
              typeof(Window), FrameworkElement.LoadedEvent,
              new RoutedEventHandler(OnWindowLoaded));
        }
      }
    }

    private void OnWindowLoaded(object sender, RoutedEventArgs e)
    {
      var view = sender as FrameworkElement;
      if (view != null)
        SubscribeToKeyboardEvents(view);
    }

    #endregion
  }
}