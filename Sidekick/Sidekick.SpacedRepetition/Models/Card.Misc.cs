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
  /// <summary>
  ///   Card model for SpacedRepetition system. Keeps track of individual cards progress
  ///   (ease, interval, ...) and datas.
  /// </summary>
  public partial class Card
  {
    #region Methods

    /// <summary>
    /// Performs a deep copy of current Card
    /// </summary>
    /// <returns>Deep copied clone</returns>
    public Card Clone()
    {
      return new Card
      {
        Id = this.Id,
        NoteId = this.NoteId,
        Config = this.Config,
        PracticeState = this.PracticeState,
        MiscState = this.MiscState,
        _eFactor = this.EFactor,
        _interval = this.Interval,
        CurrentReviewTime = this.CurrentReviewTime,
        Due = this.Due,
        Lapses = this.Lapses,
        LastModified = this.LastModified
      };
    }

    #endregion
  }
}