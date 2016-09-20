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

namespace Sidekick.Shared.Extensions
{
  using System;

  /// <summary>
  ///   Extension methods for <see cref="DateTime" />
  /// </summary>
  public static class DateTimeExtensions
  {
    #region Properties

    /// <summary>
    ///   Gets a DateTime instance representing the first moment of next day.
    /// </summary>
    public static DateTime Tomorrow => DateTime.Today.AddDays(1);

    #endregion



    #region Methods

    /// <summary>
    ///   Converts a given DateTime into a Unix timestamp.
    /// </summary>
    /// <param name="value">Any DateTime</param>
    /// <returns>The given DateTime in Unix timestamp format</returns>
    public static int ToUnixTimestamp(this DateTime value)
    {
      return
        (int)
        Math.Truncate(value.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
    }

    /// <summary>
    ///   Gets a Unix timestamp representing the current moment.
    /// </summary>
    /// <param name="ignored">Parameter ignored</param>
    /// <returns>Now expressed as a Unix timestamp</returns>
    public static int UnixTimestamp(this DateTime ignored)
    {
      return (int)Math.Truncate(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
    }

    /// <summary>
    ///   Generates a DateTime object from given Unix timestamp.
    /// </summary>
    /// <param name="unixTimestamp">The unix timestamp</param>
    /// <returns>DateTime representation of Unix timestamp</returns>
    public static DateTime FromUnixTimestamp(int unixTimestamp)
    {
      return new DateTime(1970, 1, 1).AddSeconds(unixTimestamp);
    }

    #endregion
  }
}