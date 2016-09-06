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

using System;
using System.Collections.Generic;
using Sidekick.SpacedRepetition.Models;

namespace Sidekick.SpacedRepetition.Review
{
  /// <summary>
  ///     <see cref="IComparer{T}"/> implementation for Card comparison :
  ///     Random or Due sorting.
  /// </summary>
  internal class ReviewComparers
  {
    #region Properties

    //
    // Comparer instances
    public static IComparer<Card> DueComparer => new DueComparerImpl();
    public static IComparer<Card> RandomComparer => new RandomComparerImpl();

    #endregion

    /// <summary>
    ///     Due comparer implementation. Used for sorting new, learning and
    ///     due cards.
    /// </summary>
    /// <seealso cref="IComparer{Card}" />
    private class DueComparerImpl : IComparer<Card>
    {
      #region Fields

      private readonly IComparer<int> _intComparer = Comparer<int>.Default;

      #endregion

      #region Methods

      public int Compare(Card c1, Card c2)
      {
        return _intComparer.Compare(c1.Due, c2.Due);
      }

      #endregion
    }

    /// <summary>
    ///     Random comparer implementation.
    /// </summary>
    /// <seealso cref="IComparer{Card}" />
    private class RandomComparerImpl : IComparer<Card>
    {
      #region Fields

      private readonly Random _random = new Random();

      #endregion

      #region Methods

      public int Compare(Card c1, Card c2)
      {
        return _random.Next(int.MinValue, int.MaxValue);
      }

      #endregion
    }
  }
}