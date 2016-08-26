using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catel.Services;
using Catel.Services.Models;
using Mnemophile.Interfaces.DB;
using Mnemophile.Interfaces.SpacedRepetition;
using Mnemophile.SpacedRepetition.Models;
using Mnemophile.SpacedRepetition.Impl;
using Mnemophile.SpacedRepetition.Impl.Review;

namespace Mnemophile.SpacedRepetition.Impl
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
        "Mnemophile.SpacedRepetition",
        "Mnemophile.SpacedRepetition.Properties",
        "Resources");
    }
  }
}
