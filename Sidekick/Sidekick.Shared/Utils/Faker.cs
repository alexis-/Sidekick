// 
// The MIT License (MIT)
// Copyright (c) 2016 Incogito
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

using System;
using System.Collections.Generic;
using System.Linq;

namespace Sidekick.Shared.Utils
{
  public static class Faker
  {
    #region Fields

    private static readonly Random Random = new Random();

    #endregion

    #region Methods

    public static string RandomString(int length)
    {
      const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

      return new string(Enumerable.Repeat(chars, 20)
                                  .Select(s => s[Random.Next(s.Length)])
                                  .ToArray());
    }

    public static int RandomInt(
      int min = int.MinValue, int max = int.MaxValue)
    {
      return Random.Next(min, max);
    }

    public static IEnumerable<T> RandomCollection<T>(
      Func<T> createFunc, int minElements = 5, int maxElements = 20)
    {
      int count = RandomInt(Math.Max(1, minElements), maxElements);
      List<T> randomList = new List<T>(count);

      while (count-- > 0)
        randomList.Add(createFunc());

      return randomList;
    }

    #endregion
  }
}