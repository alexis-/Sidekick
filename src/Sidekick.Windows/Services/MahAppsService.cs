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
  using System.Windows;

  using Catel;
  using Catel.MVVM;
  using Catel.Services;

  using MahApps.Metro.Controls;

  using Orchestra;
  using Orchestra.Models;
  using Orchestra.Services;

  using Sidekick.Windows.Views;

  /// <summary>MahApps-specific setup.</summary>
  /// <seealso cref="Orchestra.Services.IMahAppsService" />
  public class MahAppsService : IMahAppsService
  {
    #region Fields

    private readonly ICommandManager _commandManager;
    private readonly IMessageService _messageService;
    private readonly IUIVisualizerService _uiVisualizerService;

    #endregion



    #region Constructors

    /// <summary>Initializes a new instance of the <see cref="MahAppsService" /> class.</summary>
    /// <param name="commandManager">The command manager.</param>
    /// <param name="messageService">The message service.</param>
    /// <param name="uiVisualizerService">The UI visualizer service.</param>
    public MahAppsService(
      ICommandManager commandManager, IMessageService messageService,
      IUIVisualizerService uiVisualizerService)
    {
      Argument.IsNotNull(() => commandManager);
      Argument.IsNotNull(() => messageService);
      Argument.IsNotNull(() => uiVisualizerService);

      _commandManager = commandManager;
      _messageService = messageService;
      _uiVisualizerService = uiVisualizerService;
    }

    #endregion



    #region IMahAppsService Members

    /// <summary>Window toolbar right commands.</summary>
    public WindowCommands GetRightWindowCommands()
    {
      var windowCommands = new WindowCommands();

      ////var refreshButton = WindowCommandHelper.CreateWindowCommandButton("appbar_refresh_counterclockwise_down", "refresh");
      ////refreshButton.Command = _commandManager.GetCommand("File.Refresh");
      ////_commandManager.RegisterAction("File.Refresh", () => _messageService.ShowAsync("Refresh"));
      ////windowCommands.Items.Add(refreshButton);

      ////var saveButton = WindowCommandHelper.CreateWindowCommandButton("appbar_save", "save");
      ////saveButton.Command = _commandManager.GetCommand("File.Save");
      ////_commandManager.RegisterAction("File.Save", () => _messageService.ShowAsync("Save"));
      ////windowCommands.Items.Add(saveButton);

      ////var showWindowButton = WindowCommandHelper.CreateWindowCommandButton("appbar_new_window", "show window");
      ////showWindowButton.Command = new Command(() => _uiVisualizerService.ShowDialog<ExampleDialogViewModel>());
      ////windowCommands.Items.Add(showWindowButton);

      return windowCommands;
    }

    /// <summary>Gets the flyouts.</summary>
    /// <returns></returns>
    public FlyoutsControl GetFlyouts()
    {
      return null;
    }

    /// <summary>Gets the main view.</summary>
    /// <returns></returns>
    public FrameworkElement GetMainView()
    {
      return new MainView();
    }

    /// <summary>Gets the status bar.</summary>
    /// <returns></returns>
    public FrameworkElement GetStatusBar()
    {
      return null;
    }

    /// <summary>
    ///   Returns the about info. If <c>null</c>, the shell will not show the about
    ///   window.
    /// </summary>
    /// <returns></returns>
    public AboutInfo GetAboutInfo()
    {
      return null;
    }

    #endregion
  }
}