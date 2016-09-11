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

// ReSharper disable PossibleNullReferenceException
namespace Sidekick.Shared
{
  using System;
  using System.Threading;

  using Anotar.Catel;

  /// <summary>
  ///   In combination with AsyncErrorHandler.Fody, all async methods exceptions are proxied here
  /// </summary>
  public class AsyncErrorHandler
  {
    #region Methods

    /// <summary>
    ///   Handles the async exception.
    /// </summary>
    /// <param name="exception">The exception.</param>
    public static void HandleException(Exception exception)
    {
      LogTo.Error(exception, "Async exception caught through AsyncErrorHandler");

#if DEBUG
      SynchronizationContext.Current.Post(RethrowOnMainThread, exception);
#endif
    }

#if DEBUG
    private static void RethrowOnMainThread(object state)
    {
      Exception ex = state as Exception;

      throw ex;
    }
#endif

#endregion
  }
}