// 
// The MIT License (MIT)
// Copyright (c) 2016 Incogito
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sidekick.Shared.Interfaces.Database;
using Sidekick.Shared.Utils;
using Sidekick.Shared.Utils.Collections;
using Sidekick.Shared.Utils.LazyLoad;
using Sidekick.SpacedRepetition.Models;

namespace Sidekick.SpacedRepetition.Review
{
  internal abstract class ReviewAsyncDbListBase : AsyncDbListBase<Card>
  {
    #region Fields

    //
    // Const

    protected const int IncrementalFurtherLoadMax = 10;
    protected const int IncrementalFurtherLoadMin = 5;

    #endregion

    #region Constructors

    //
    // Constructor

    protected ReviewAsyncDbListBase(IDatabase db)
      : base(db, true)
    {
      FurtherLoadedIndex = -1;
      DismissedIds = new HashSet<int>();

      Initialize(true);
    }

    #endregion

    #region Properties

    //
    // Properties

    // State

    protected int FurtherLoadedIndex { get; set; }
    protected HashSet<int> DismissedIds { get; set; }

    /// <summary>
    ///     Dismissed item count
    /// </summary>
    public int Dismissed => DismissedIds.Count;

    /// <summary>
    ///     Number of card reviewed
    /// </summary>
    public int ReviewedCardCount => Index + 1 - Dismissed;

    #endregion

    #region Methods

    // 
    // Core methods

    /// <summary>
    ///   Returns waitable task for list initialization
    /// </summary>
    /// <returns>Waitable Task</returns>
    public Task Initialized()
    {
      if (Objects.Count > 0)
        return TaskConstants.Completed;

      lock (LockObject)
        return LoadCompletionSource?.Task ?? TaskConstants.Completed;
    }

    /// <summary>
    ///     Dismiss current item and calls
    ///     <see cref="AsyncDbListBase{T}.MoveNext()"/>
    /// </summary>
    /// <returns>Whether any item is available</returns>
    public Task<bool> Dismiss()
    {
      if (!CheckState(false))
        throw new InvalidOperationException("Invalid state");

      DismissedIds.Add(Current.Id);

      return MoveNext();
    }

    /// <summary>
    ///     Dismisses card siblings (cards of the same note).
    ///     Works by swapping cards.
    /// </summary>
    /// <returns>Number of dismissed cards</returns>
    /// <exception cref="System.InvalidOperationException">
    ///     Is thrown if invalid state.
    /// </exception>
    public int DismissSiblings(Card card)
    {
      if (!CheckState(false))
        throw new InvalidOperationException("Invalid state");

      lock (LockObject)
      {
        int noteId = card.NoteId;
        int cardId = card.Id;

        List<int> cardIndices = new List<int>();
        int cardIndex = Index + 1;

        if (cardIndex >= Objects.Count)
          return 0;

        // Find siblings indices
        while ((cardIndex =
                Objects.FindIndex(
                  cardIndex + 1,
                  c => c.NoteId == noteId && c.Id != cardId)) > 0)
          cardIndices.Add(cardIndex);

        // No siblings
        if (!cardIndices.Any())
          return 0;

        // Swap current card so that it becomes new Index current Index
        if (Index >= 0)
          SwapCards(Index, Index + cardIndices.Count);

        int idx = Math.Max(Index, 0);
        int furtherLoadedOffset = 0;

        // Swap siblings
        for (int i = 0; i < cardIndices.Count; i++)
        {
          Card sibling = Objects[cardIndices[i]];

          Objects[cardIndices[i]] = Objects[idx + i];
          Objects[idx + i] = sibling;

          // Add sibling's Id to dismissed cards
          DismissedIds.Add(sibling.Id);

          // Deal with lazy loading
          if (cardIndices[i] > FurtherLoadedIndex)
            furtherLoadedOffset++;

          else
            LazyLoader.LazyUnload(sibling);
        }

        // Set idx
        FurtherLoadedIndex += furtherLoadedOffset;
        Index += cardIndices.Count;

        // Sort if necessary
        Sort();

        return cardIndices.Count;
      }
    }

    /// <summary>
    ///     Good ol' swap.
    /// </summary>
    /// <param name="idx1">First card's index</param>
    /// <param name="idx2">Second card's index</param>
    private void SwapCards(int idx1, int idx2)
    {
      Card tmp = Objects[idx1];

      Objects[idx1] = Objects[idx2];
      Objects[idx2] = tmp;
    }

    public abstract int AvailableCount();
    public abstract int ReviewCount();


    //
    // AsyncDbListBase core methods implementation

    /// <summary>
    ///     Fully load lazy loaded items.
    /// </summary>
    protected override void DoFurtherLoad()
    {
      IEnumerable<Card> cards;
      HashSet<int> cardsId;

      lock (LockObject)
      {
        cards = Objects
          .Skip(GetFurtherLoadedIndex())
          .Take(Math.Max(Index - GetFurtherLoadedIndex(), 0)
                + IncrementalFurtherLoadMax - IncrementalFurtherLoadMin);
        cardsId = new HashSet<int>(cards.Select(c => c.Id));
      }

      IEnumerable<Card> updateCards;

      using (Db.Lock())
      {
        updateCards =
          Db.Table<Card>()
            .FurtherLoad(LazyLoader)
            .Where(c => cardsId.Contains(c.Id));
      }

      lock (LockObject)
      {
        foreach (Card card in cards)
          LazyLoader.UpdateFromFurtherLoad(
            card, updateCards.FirstOrDefault(uc => uc.Id == card.Id));

        FurtherLoadedIndex = FurtherLoadedIndex + updateCards.Count();
      }
    }

    #endregion
  }
}