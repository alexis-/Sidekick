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

namespace Sidekick.SpacedRepetition.Extensions
{
  using System;
  using System.Threading.Tasks;

  using Sidekick.Shared.Interfaces.Database;
  using Sidekick.SpacedRepetition.Const;
  using Sidekick.SpacedRepetition.Models;

  /// <summary>Extensions methods for Card non-spaced repetition related logic.</summary>
  public static class CardExtensions
  {
    #region Methods

    /// <summary>
    /// Dismisses the asynchronous.
    /// </summary>
    /// <param name="card">The card.</param>
    /// <param name="db">Database instance.</param>
    /// <returns></returns>
    public static async Task DismissAsync(this Card card, IDatabaseAsync db)
    {
      card.Dismiss();

      await db.UpdateAsync(card).ConfigureAwait(false);
    }

    /// <summary>
    ///   Answers current card with specified grade and calculate outcomes. Modifications are
    ///   saved to database.
    /// </summary>
    /// <param name="card">Card instance</param>
    /// <param name="grade">The grade.</param>
    /// <param name="db">Database instance</param>
    public static async Task AnswerAsync(this Card card, Grade grade, IDatabaseAsync db)
    {
      CardAction cardAction = card.Answer(grade);

      if (db != null)
        switch (cardAction)
        {
          case CardAction.Delete:
            await db.DeleteAsync<Card>(card.Id).ConfigureAwait(false);
            break;

          case CardAction.Update:
            await db.UpdateAsync(card).ConfigureAwait(false);
            break;

          default:
            throw new InvalidOperationException(
              "Card.Answer ended up in an invalid Card Action state.");
        }
    }

    /// <summary>Save card modifications to database according to CardAction.</summary>
    /// <param name="card">Card instance</param>
    /// <param name="cardAction">The resulting card action.</param>
    /// <param name="db">Database instance</param>
    public static async Task AnswerAsync(
      this Card card, CardAction cardAction, IDatabaseAsync db)
    {
      if (db != null)
        switch (cardAction)
        {
          case CardAction.Delete:
            await db.DeleteAsync<Card>(card.Id).ConfigureAwait(false);
            break;

          case CardAction.Update:
            await db.UpdateAsync(card).ConfigureAwait(false);
            break;

          default:
            throw new InvalidOperationException(
              "Card.Answer ended up in an invalid Card Action state.");
        }
    }

    #endregion
  }
}