﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mnemophile.Utils;

namespace Mnemophile.SRS.Tests.Helpers
{
  public class TimeGenerator
  {
    // Config
    private int MaxTimeRange { get; }
    private bool AllowOverdue { get; }

    // Misc
    private static readonly Random Random = new Random();


    //
    // Constructor

    public TimeGenerator(int daysMaxRange = 1, bool allowOverdue = true)
    {
      MaxTimeRange = daysMaxRange * 24 * 60 * 60;
      AllowOverdue = allowOverdue;
    }


    //
    // Core methods

    private int RandomTime(int timeRange, bool allowOverdue)
    {
      return DateTime.Today
                     .AddSeconds(
                       Random.Next(
                         allowOverdue ? 0 : -timeRange,
                         timeRange))
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
      } while (cardIds.Contains(id));

      cardIds.Add(id);

      return id;
    }
  }
}
