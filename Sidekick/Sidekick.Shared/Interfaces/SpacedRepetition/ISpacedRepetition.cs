using Catel.Services;
using Sidekick.Shared.Interfaces.DB;

namespace Sidekick.Shared.Interfaces.SpacedRepetition
{
  public interface ISpacedRepetition
  {
    //
    // Spaced Repetition System Identity

    int Id { get; }
    string Name { get; }
    string Version { get; }



    //
    // Core methods

    INote CreateNote();
    IReviewCollection GetReviewCollection(IDatabase db);



    //
    // Misc

    ILanguageSource GetLanguageSource();
  }
}
