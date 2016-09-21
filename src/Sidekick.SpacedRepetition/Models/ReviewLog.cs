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

namespace Sidekick.SpacedRepetition.Models
{
  using AgnosticDatabase.Attributes;

  /// <summary>Describes a single card review</summary>
  [Table("ReviewsLogs")]
  public class ReviewLog
  {
    #region Constructors

    /// <summary>Initializes a new instance of the <see cref="ReviewLog" /> class.</summary>
    public ReviewLog() { }

    /// <summary>Initializes a new instance of the <see cref="ReviewLog" /> class.</summary>
    /// <param name="id">Creation timestamp.</param>
    /// <param name="cardId">The card identifier.</param>
    /// <param name="lastDue">The last due.</param>
    /// <param name="lastState">The last state.</param>
    /// <param name="lastInterval">The last interval.</param>
    /// <param name="lastEFactor">The last e factor.</param>
    /// <param name="evalTime">The eval time.</param>
    public ReviewLog(
      int id, int cardId, int lastDue, PracticeState lastState, int lastInterval,
      float lastEFactor, int evalTime)
    {
      Id = id;
      CardId = cardId;
      LastDue = lastDue;
      LastState = lastState;
      LastInterval = lastInterval;
      LastEFactor = lastEFactor;
      EvalTime = evalTime;
    }

    /// <summary>Initializes a new instance of the <see cref="ReviewLog" /> class.</summary>
    /// <param name="id">Creation timestamp.</param>
    /// <param name="cardId">The card identifier.</param>
    /// <param name="grade">The grade.</param>
    /// <param name="lastDue">The last due.</param>
    /// <param name="newDue">The new due.</param>
    /// <param name="lastState">The last state.</param>
    /// <param name="newState">The new state.</param>
    /// <param name="lastInterval">The last interval.</param>
    /// <param name="newInterval">The new interval.</param>
    /// <param name="lastEFactor">The last e factor.</param>
    /// <param name="newEFactor">The new e factor.</param>
    /// <param name="evalTime">The eval time.</param>
    public ReviewLog(
      int id, int cardId, Grade grade, int lastDue, int newDue, PracticeState lastState,
      PracticeState newState, int lastInterval, int newInterval, float lastEFactor,
      float newEFactor, int evalTime)
    {
      Id = id;
      CardId = cardId;
      Grade = grade;
      LastDue = lastDue;
      NewDue = newDue;
      LastState = lastState;
      NewState = newState;
      LastInterval = lastInterval;
      NewInterval = newInterval;
      LastEFactor = lastEFactor;
      NewEFactor = newEFactor;
      EvalTime = evalTime;
    }

    #endregion



    #region Properties

    /// <summary>Database field. Database identifier, date time of creation time - Unix timestamp.</summary>
    [PrimaryKey]
    public int Id { get; set; }

    /// <summary>Database field. Associated card id.</summary>
    [Indexed]
    public int CardId { get; set; }

    /// <summary>Database field. Grade used in this review.</summary>
    public int Grade { get; set; }

    /// <summary>Database field. Due before reviewing.</summary>
    public int LastDue { get; set; }

    /// <summary>Database field. Due after reviewing.</summary>
    public int NewDue { get; set; }

    /// <summary>Database field. State before reviewing.</summary>
    public short LastState { get; set; }

    /// <summary>Database field. State after reviewing.</summary>
    public short NewState { get; set; }

    /// <summary>Database field. Interval before reviewing.</summary>
    public int LastInterval { get; set; }

    /// <summary>Database field. Interval after reviewing.</summary>
    public int NewInterval { get; set; }

    /// <summary>Database field. EFactor before reviewing.</summary>
    public float LastEFactor { get; set; }

    /// <summary>Database field. EFactor after reviewing.</summary>
    public float NewEFactor { get; set; }

    /// <summary>Database field. Duration of the review evaluation. Limited to 60 seconds</summary>
    public int EvalTime { get; set; }

    #endregion



    #region Methods

    /// <summary>Completes the review informations.</summary>
    /// <param name="grade">The grade.</param>
    /// <param name="newDue">The new due.</param>
    /// <param name="newState">The new state.</param>
    /// <param name="newInterval">The new interval.</param>
    /// <param name="newEFactor">The new e factor.</param>
    public void CompleteReview(
      Grade grade, int newDue, PracticeState newState, int newInterval, float newEFactor)
    {
      Grade = grade;
      NewDue = newDue;
      NewState = newState;
      NewInterval = newInterval;
      NewEFactor = newEFactor;
    }

    #endregion
  }
}