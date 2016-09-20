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

namespace Sidekick.SpacedRepetition
{
  using System;

  /// <summary>
  ///   Card state.
  ///   Flag enum, allows card filtering.
  /// </summary>
  [Flags]
  public enum CardPracticeStateFilterFlag
  {
    /// <summary>New card state flag</summary>
    New = 1,

    /// <summary>Learning card state flag</summary>
    Learning = 2,

    /// <summary>Due card state flag</summary>
    Due = 4,

    /// <summary>All cards states flag</summary>
    All = New | Learning | Due
  }

  /// <summary>
  ///   Card state.
  ///   Flag enum, misc states for cards (suspended, dismissed, ...).
  /// </summary>
  [Flags]
  public enum CardMiscStateFlag : short
  {
    /// <summary>No special state</summary>
    None = 0,

    /// <summary>Suspended state (on hold)</summary>
    Suspended = 1,

    /// <summary>Dismissed state (postponed)</summary>
    Dismissed = 2,
  }

  /// <summary>
  ///   Configuration option.
  ///   Which actions to take when a card is leeching.
  /// </summary>
  public enum CardLeechAction
  {
    /// <summary>Suspend card on leech</summary>
    Suspend,

    /// <summary>Delete card on leech</summary>
    Delete,
  }

  /// <summary>
  ///   Configuration option.
  ///   How to order new cards review.
  /// </summary>
  public enum CardOrderingOption
  {
    /// <summary>By creation date</summary>
    Linear,

    /// <summary>At random</summary>
    Random,
  }

  /// <summary>
  /// Card reviewing.
  /// Outcome of a review action (answer or dismiss).
  /// </summary>
  public enum CardAction
  {
    /// <summary>No such action</summary>
    Invalid = -1,

    /// <summary>Update card</summary>
    Update,

    /// <summary>Delete card</summary>
    Delete,

    /// <summary>Dismiss card</summary>
    Dismiss
  }
}