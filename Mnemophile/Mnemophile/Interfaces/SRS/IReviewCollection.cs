using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mnemophile.Const.SRS;

namespace Mnemophile.Interfaces.SRS
{
  public interface IReviewCollection : IEnumerable<ICard>
  {
    /// <summary>
    ///     Total count of cards to be reviewed, regardless of state.
    /// </summary>
    /// <value>
    ///     Review card count.
    /// </value>
    int Count { get; }

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
