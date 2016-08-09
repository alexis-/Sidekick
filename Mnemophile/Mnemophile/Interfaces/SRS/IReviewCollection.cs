using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mnemophile.Const.SRS;

namespace Mnemophile.Interfaces.SRS
{
  public interface IReviewCollection
  {
    /// <summary>
    ///     Waitable Task to check whether ReviewCollection is ready.
    ///     ReviewCollection should be initialized before calling other any
    ///     other method.
    /// </summary>
    /// <value>
    ///     True if ReviewCollection is initialized and ready.
    ///     False if no card is available for review.
    /// </value>
    Task<bool> Initialized { get; }

    /// <summary>
    ///     Answer current card and fetch next one.
    /// </summary>
    /// <returns>Whether any cards are available</returns>
    Task<bool> Answer(ConstSRS.Grade grade);

    /// <summary>
    ///     Dismiss current card and fetch next one.
    /// </summary>
    /// <returns>Whether any cards are available</returns>
    Task<bool> Dismiss();

    /// <summary>
    ///     Last fetched card.
    /// </summary>
    ICard Current { get; }

    /// <summary>
    ///     State-dependent counts of cards to be reviewed.
    /// </summary>
    /// <param name="state">The state.</param>
    /// <returns>
    ///     State-dependent card count.
    /// </returns>
    int CountByState(ConstSRS.CardPracticeStateFilterFlag state);
  }
}
