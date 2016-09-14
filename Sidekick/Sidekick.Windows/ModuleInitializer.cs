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

using Catel.IoC;

using Orchestra.Services;

using Sidekick.Windows.Services;
using Sidekick.Windows.Services.Initialization;

/// <summary>
///   Used by the ModuleInit. All code inside the Initialize method is ran as soon as
///   the assembly is loaded.
/// </summary>
public static class ModuleInitializer
{
  #region Methods

  /// <summary>Initializes the module.</summary>
  public static void Initialize()
  {
    var serviceLocator = ServiceLocator.Default;

    serviceLocator.RegisterType<IMahAppsService, MahAppsService>();
    serviceLocator
      .RegisterType<IApplicationInitializationService, ApplicationInitializationService>();

    // ***** IMPORTANT NOTE *****
    //
    // Only register the shell services in the ModuleInitializer. All other
    // types must be registered in the ApplicationInitializationService.
  }

  #endregion
}