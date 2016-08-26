using Catel.Services;
using Catel.Services.Models;
using Sidekick.Shared.Interfaces.DB;
using Sidekick.Shared.Interfaces.SpacedRepetition;
using Sidekick.SpacedRepetition.Impl.Review;
using Sidekick.SpacedRepetition.Models;

namespace Sidekick.SpacedRepetition.Impl
{
  public class SM2Impl : ISpacedRepetition
  {
    public int Id => Name.GetHashCode();
    public string Name => "SM2";
    public string Version => "0.1";
    


    //
    // Constructor

    //public static SM2Impl Instance { get; private set; }

    //public static SM2Impl GetInstance()
    //{
    //  return Instance ?? (Instance = new SM2Impl());
    //}

    public SM2Impl() { }



    //
    // Core methods

    public INote CreateNote()
    {
      return new Note();
    }

    public IReviewCollection GetReviewCollection(IDatabase db)
    {
      return new ReviewCollectionImpl(db, CollectionConfig.Default);
    }
    

    
    //
    // Misc

    public ILanguageSource GetLanguageSource()
    {
      return new LanguageResourceSource(
        "Sidekick.SpacedRepetition",
        "Sidekick.SpacedRepetition.Properties",
        "Resources");
    }
  }
}
