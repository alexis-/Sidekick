using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.CompilerServices;

namespace Mnemophile.Utils
{
  /// <summary>
  /// Handles delay conversion in minutes, days, etc.
  /// Backing field in seconds, implicit operators in seconds.
  /// </summary>
  public class Delay
  {
    public const int Days2Sec = 24 * Hours2Sec;
    public const int Hours2Sec = 3600;
    public const int Minutes2Sec = 60;

    private int Value { get; set; }

    //
    // Convertions

    public int Seconds => Value;
    public float Minutes => (float)Value / Minutes2Sec;
    public float Hours => (float)Value / Hours2Sec;
    public float Days => (float)Value / Days2Sec;


    //
    // Static initializer

    public static Delay FromMinutes(int minutes)
    {
      return new Delay { Value = minutes * Minutes2Sec };
    }

    public static Delay FromHours(int hours)
    {
      return new Delay { Value = hours * Hours2Sec };
    }

    public static Delay FromDays(int days)
    {
      return new Delay { Value = days * Days2Sec };
    }


    //
    // Implicit operators

    public static implicit operator Delay(int delay)
    {
      return new Delay { Value = delay };
    }

    public static implicit operator int(Delay delay)
    {
      return delay.Value;
    }
  }
}
