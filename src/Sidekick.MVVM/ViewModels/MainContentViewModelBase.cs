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

namespace Sidekick.MVVM.ViewModels
{
  using System.Threading.Tasks;

  using Catel.MVVM;

  using Sidekick.Shared.Utils;

  /// <summary>
  ///   Abstract base for main view's content ViewModels.
  /// </summary>
  /// <seealso cref="Catel.MVVM.ViewModelBase" />
  public abstract class MainContentViewModelBase : ViewModelBase
  {
    #region Methods

    /// <summary>
    ///   Called when Main view's content ViewModel is changing.
    ///   Allows to interrupt navigation if ongoing work might be lost.
    /// </summary>
    /// <returns>Whether to continue navigation</returns>
    public virtual Task<bool> OnContentChange()
    {
      return TaskConstants.BooleanTrue;
    }

    #endregion
  }
}