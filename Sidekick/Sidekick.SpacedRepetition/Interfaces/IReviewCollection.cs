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

namespace Sidekick.SpacedRepetition.Interfaces
{
  using System.Threading.Tasks;

  using Sidekick.SpacedRepetition.Const;
  using Sidekick.SpacedRepetition.Models;

  /// <summary>
  ///   Loads and schedule cards to be displayed for review.
  /// </summary>
  public interface IReviewCollection
  {
    #region Properties

    /// <summary>
    ///   Waitable Task to check whether ReviewCollection is ready.
    ///   ReviewCollection should be initialized before calling other any
    ///   other method.
    /// </summary>
    /// <value>
    ///   True if ReviewCollection is initialized and ready.
    ///   False if no card is available for review.
    /// </value>
    Task<bool> Initialized { get; }

    /// <summary>
    ///   Last fetched card.
    /// </summary>
    Card Current { get; }

    #endregion



    #region Methods

    /// <summary>
    ///   Answer current card and fetch next one.
    /// </summary>
    /// <param name="grade">Answer grade (fail, good, easy, ...)</param>
    /// <returns>
    ///   Whether any cards are available
    /// </returns>
    /// <exception cref="System.InvalidOperationException">
    ///   No card available (Current is null).
    /// </exception>
    Task<bool> AnswerAsync(Grade grade);

    /// <summary>
    ///   Dismiss current card and fetch next one.
    /// </summary>
    /// <returns>Whether any cards are available</returns>
    /// <exception cref="System.InvalidOperationException">
    ///   No card available (Current is null).
    /// </exception>
    Task<bool> DismissAsync();

    /// <summary>
    ///   State-dependent counts of cards to be reviewed.
    /// </summary>
    /// <param name="state">The state.</param>
    /// <returns>
    ///   State-dependent card count.
    /// </returns>
    int CountByState(CardPracticeStateFilterFlag state);

    #endregion
  }
}