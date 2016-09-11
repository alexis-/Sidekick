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

namespace Sidekick.Windows
{
  using System.Globalization;
  using System.Windows;

  using Catel.ApiCop;
  using Catel.ApiCop.Listeners;
  using Catel.IoC;
  using Catel.Logging;
  using Catel.Services;
  using Catel.Services.Models;

  using Orchestra.Services;

  using Sidekick.Windows.Views;

  /// <summary>
  ///   Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    #region Constructors

    public App() { }

    #endregion



    #region Methods

    /// <summary>
    ///   Raises the <see cref="E:System.Windows.Application.Startup" /> event.
    /// </summary>
    /// <param name="e">A <see cref="T:System.Windows.StartupEventArgs" /> that contains the event data.</param>
    protected override void OnStartup(StartupEventArgs e)
    {
      var dependencyResolver = this.GetDependencyResolver();

      //
      // Logging

#if DEBUG
      LogManager.AddDebugListener(true);
#endif


      //
      // Localization

      var languageService = dependencyResolver.Resolve<ILanguageService>();
      languageService.FallbackCulture = new CultureInfo("en-US");

      languageService.RegisterLanguageSource(
        new LanguageResourceSource(
          "Sidekick.Windows", "Sidekick.Windows.Properties", "Resources"));
      languageService.RegisterLanguageSource(
        new LanguageResourceSource("Sidekick.MVVM", "Sidekick.MVVM.Properties", "Resources"));


      //
      // Shell creation

      var serviceLocator = ServiceLocator.Default;
      var shellService = serviceLocator.ResolveType<IShellService>();

      shellService.CreateWithSplashAsync<MnemoShellWindow>();
    }

    /// <summary>
    ///   Raises the <see cref="E:System.Windows.Application.Exit" /> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.Windows.ExitEventArgs" /> that contains the event data.</param>
    protected override void OnExit(ExitEventArgs e)
    {
      //
      // API Cop output

#if DEBUG
      var apiCopListener = new ConsoleApiCopListener();
      ApiCopManager.AddListener(apiCopListener);
      ApiCopManager.WriteResults();
#endif

      base.OnExit(e);
    }

    #endregion
  }
}