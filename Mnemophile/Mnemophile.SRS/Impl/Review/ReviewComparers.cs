using System;
using System.Collections.Generic;
using Mnemophile.SRS.Models;

namespace Mnemophile.SRS.Impl.Review
{
  /// <summary>
  ///     <see cref="IComparer{T}"/> implementation for Card comparison :
  ///     Random or Due sorting.
  /// </summary>
  internal class ReviewComparers
  {
    //
    // Comparer instances
    public static IComparer<Card> DueComparer => new DueComparerImpl();
    public static IComparer<Card> RandomComparer => new RandomComparerImpl();


    /// <summary>
    ///     Due comparer implementation. Used for sorting new, learning and
    ///     due cards.
    /// </summary>
    /// <seealso cref="IComparer{Card}" />
    private class DueComparerImpl : IComparer<Card>
    {
      private readonly IComparer<int> _intComparer = Comparer<int>.Default;

      public int Compare(Card c1, Card c2)
      {
        return _intComparer.Compare(c1.Due, c2.Due);
      }
    }

    /// <summary>
    ///     Random comparer implementation.
    /// </summary>
    /// <seealso cref="IComparer{Card}" />
    private class RandomComparerImpl : IComparer<Card>
    {
      private readonly Random _random = new Random();

      public int Compare(Card c1, Card c2)
      {
        return _random.Next(int.MinValue, int.MaxValue);
      }
    }
  }
}