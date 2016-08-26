using Catel.Services;
using Mnemophile.Interfaces.DB;

namespace Mnemophile.Interfaces.SpacedRepetition
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
