﻿// 
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

namespace Sidekick.Shared.Extensions
{
  public static class DictionaryExtensions
  {
    public static TValue GetOrAdd<TKey, TValue>(
      this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
    {
      TValue retValue;

      if (!dictionary.TryGetValue(key, out retValue))
      {
        retValue = value;

        dictionary.Add(key, value);
      }

      return retValue;
    }

    public static TValue GetOrAdd<TKey, TValue>(
      this Dictionary<TKey, TValue> dictionary,
      TKey key, Func<TValue> valueFunc)
    {
      TValue retValue;

      if (!dictionary.TryGetValue(key, out retValue))
      {
        retValue = valueFunc();

        dictionary.Add(key, retValue);
      }

      return retValue;
    }
  }
}