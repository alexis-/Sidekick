using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catel.IoC;
using Orchestra.Services;
using Sidekick.Windows.Services;
using Sidekick.Windows.Services.Initialization;

/// <summary>
///   Used by the ModuleInit. All code inside the Initialize method is ran as
///   soon as the assembly is loaded.
/// </summary>
public static class ModuleInitializer
{
  /// <summary>
  /// Initializes the module.
  /// </summary>
  public static void Initialize()
  {
    var serviceLocator = ServiceLocator.Default;

    serviceLocator.RegisterType<IMahAppsService, MahAppsService>();
    serviceLocator.RegisterType<IApplicationInitializationService,
      ApplicationInitializationService>();

    FilterBuilderModuleInitializer.Initialize();

    // ***** IMPORTANT NOTE *****
    //
    // Only register the shell services in the ModuleInitializer. All other
    // types must be registered in the ApplicationInitializationService.
  }
}