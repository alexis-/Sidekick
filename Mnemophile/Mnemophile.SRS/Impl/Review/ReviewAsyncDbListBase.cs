using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mnemophile.Interfaces.DB;
using Mnemophile.SRS.Models;
using Mnemophile.Utils;
using Mnemophile.Utils.Collections;
using Mnemophile.Utils.LazyLoad;

namespace Mnemophile.SRS.Impl.Review
{
  internal abstract class ReviewAsyncDbListBase : AsyncDbListBase<Card>
  {
    //
    // Const

    protected const int IncrementalFurtherLoadMax = 10;
    protected const int IncrementalFurtherLoadMin = 5;



    //
    // Properties

    // State

    protected int FurtherLoadedIndex { get; set; }
    protected HashSet<int> DismissedIds { get; set; }


    
    //
    // Constructor

    protected ReviewAsyncDbListBase(IDatabase db)
      : base(db, true)
    {
      FurtherLoadedIndex = -1;
      DismissedIds = new HashSet<int>();

      Initialize(true);
    }
    


    // 
    // Core methods

    /// <summary>
    ///     Dismiss current item and calls
    ///     <see cref="AsyncDbListBase{T}.MoveNext()"/>
    /// </summary>
    /// <returns>Whether any item is available</returns>
    public Task<bool> Dismiss()
    {
      if (!CheckState(false))
        return TaskConstants.BooleanFalse;

      DismissedIds.Add(Current.Id);

      return MoveNext();
    }

    /// <summary>
    ///     Dismissed item count
    /// </summary>
    public int Dismissed => DismissedIds.Count;
    


    //
    // AsyncDbListBase core methods implementation

    /// <summary>
    ///     Fully load lazy loaded items.
    /// </summary>
    protected override void DoFurtherLoad()
    {
      IEnumerable<Card> cards = Objects
        .Skip(GetFurtherLoadedIndex())
        .Take(Math.Max(Index - GetFurtherLoadedIndex(), 0)
              + IncrementalFurtherLoadMax - IncrementalFurtherLoadMin);
      HashSet<int> cardsId = new HashSet<int>(cards.Select(c => c.Id));

      IEnumerable<Card> updateCards =
        Db.Table<Card>()
          .FurtherLoad(LazyLoader)
          .Where(c => cardsId.Contains(c.Id));

      lock (LockObject)
      {
        foreach (Card card in cards)
          LazyLoader.UpdateFromFurtherLoad(
            card, updateCards.FirstOrDefault(uc => uc.Id == card.Id));

        FurtherLoadedIndex = FurtherLoadedIndex + cardsId.Count;
      }
    }
  }
}
