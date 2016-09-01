using Catel.IoC;
using Catel.Services;
using Catel.Services.Models;
using Sidekick.FilterBuilder.Services.Interfaces;
using Sidekick.FilterBuilder.Services;

/// <summary>
/// Used by the ModuleInit. All code inside the Initialize method is ran as
/// soon as the assembly is loaded.
/// </summary>
public static class FilterBuilderModuleInitializer
{
  /// <summary>
  /// Initializes the module.
  /// </summary>
  public static void Initialize()
  {
    var serviceLocator = ServiceLocator.Default;
    
    serviceLocator.RegisterTypeIfNotYetRegistered<IReflectionService,
      ReflectionService>();
    serviceLocator.RegisterTypeIfNotYetRegistered<IFilterCustomizationService,
      FilterCustomizationService>();
  }
}