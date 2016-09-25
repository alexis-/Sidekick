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
  using System.Collections.Generic;

  /// <summary>
  /// Extension methods for <see cref="Dictionary{TKey,TValue}"/>
  /// </summary>
  public static class DictionaryExtensions
  {
    #region Methods

    /// <summary>Return value if dictionary contains key, or default TValue type value</summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="dictionary">The dictionary.</param>
    /// <param name="key">The key.</param>
    /// <returns></returns>
    public static TValue SafeGet<TKey, TValue>(
      this Dictionary<TKey, TValue> dictionary, TKey key)
    {
      if (dictionary.ContainsKey(key))
        return dictionary[key];

      return default(TValue);
    }

    /// <summary>
    /// Gets the value if it exists in the dictionary, adds it otherwise
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="dictionary">The dictionary.</param>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    /// <returns></returns>
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

    /// <summary>
    /// Gets the value if it exists in the dictionary, adds it otherwise
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="dictionary">The dictionary.</param>
    /// <param name="key">The key.</param>
    /// <param name="valueFunc">The value provider function.</param>
    /// <returns></returns>
    public static TValue GetOrAdd<TKey, TValue>(
      this Dictionary<TKey, TValue> dictionary, TKey key, Func<TValue> valueFunc)
    {
      TValue retValue;

      if (!dictionary.TryGetValue(key, out retValue))
      {
        retValue = valueFunc();

        dictionary.Add(key, retValue);
      }

      return retValue;
    }

    #endregion
  }
}