using System;

namespace Sidekick.Shared.Utils
{
  public static class DateTimeEx
  {
    /// <summary>
    /// Converts a given DateTime into a Unix timestamp.
    /// </summary>
    /// <param name="value">Any DateTime</param>
    /// <returns>The given DateTime in Unix timestamp format</returns>
    public static int ToUnixTimestamp(this DateTime value)
    {
      return (int)Math.Truncate(value.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
    }

    /// <summary>
    /// Gets a Unix timestamp representing the current moment.
    /// </summary>
    /// <param name="ignored">Parameter ignored</param>
    /// <returns>Now expressed as a Unix timestamp</returns>
    public static int UnixTimestamp(this DateTime ignored)
    {
      return (int)Math.Truncate(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
    }

    /// <summary>
    /// Generates a DateTime object from given Unix timestamp.
    /// </summary>
    /// <param name="unixTimestamp">The unix timestamp</param>
    /// <returns>DateTime representation of Unix timestamp</returns>
    public static DateTime FromUnixTimestamp(int unixTimestamp)
    {
      return new DateTime(1970, 1, 1).AddSeconds(unixTimestamp);
    }
  }
}
