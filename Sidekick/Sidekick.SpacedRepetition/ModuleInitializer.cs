using Catel.IoC;
using Catel.Services;
using Sidekick.SpacedRepetition;
using Sidekick.SpacedRepetition.Interfaces;

/// <summary>
/// Used by the ModuleInit. All code inside the Initialize method is ran as
/// soon as the assembly is loaded.
/// </summary>
public static class ModuleInitializer
{
  /// <summary>
  /// Initializes the module.
  /// </summary>
  public static void Initialize()
  {
    var serviceLocator = ServiceLocator.Default;

    serviceLocator.RegisterType<ISpacedRepetition, SM2Impl>();

    SM2Impl spacedRepetition =
      (SM2Impl)serviceLocator.ResolveType<ISpacedRepetition>();
    ILanguageService languageService =
      serviceLocator.ResolveType<ILanguageService>();

    languageService.RegisterLanguageSource(
      spacedRepetition.GetLanguageSource());
  }
}