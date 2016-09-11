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

namespace Sidekick.SpacedRepetition.Generators
{
  using System;
  using System.Collections.Generic;

  using Sidekick.Shared.Extensions;

  /// <summary>
  ///   Generates parametrized random timestamps
  /// </summary>
  public class TimeGenerator
  {
    #region Fields

    // Misc
    private static readonly Random Random = new Random();

    #endregion



    #region Constructors

    //
    // Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="TimeGenerator"/> class.
    /// </summary>
    /// <param name="daysMaxRange">The days maximum range.</param>
    /// <param name="allowOverdue">if set to <c>true</c> [allow overdue].</param>
    public TimeGenerator(int daysMaxRange = 1, bool allowOverdue = true)
    {
      MaxTimeRange = daysMaxRange * 24 * 60 * 60;
      AllowOverdue = allowOverdue;
    }

    #endregion



    #region Properties

    // Config
    private int MaxTimeRange { get; }
    private bool AllowOverdue { get; }

    #endregion



    #region Methods

    //
    // Core methods

    private int RandomTime(int timeRange, bool allowOverdue)
    {
      return
        DateTime.Today.AddSeconds(Random.Next(allowOverdue ? 0 : -timeRange, timeRange))
                .ToUnixTimestamp();
    }

    public int RandomTime(int timeRange)
    {
      return RandomTime(timeRange, AllowOverdue);
    }

    public int RandomTime()
    {
      return RandomTime(MaxTimeRange, AllowOverdue);
    }

    public int RandomId(HashSet<int> cardIds)
    {
      int id;

      do
      {
        id = RandomTime(MaxTimeRange, true);
      }
      while (cardIds.Contains(id));

      cardIds.Add(id);

      return id;
    }

    #endregion
  }
}