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

namespace Sidekick.Windows.Services.Interfaces
{
  using System.Windows.Input;

  using Catel.MVVM;

  /// <summary>
  ///   Extended command manager
  /// </summary>
  /// <seealso cref="Catel.MVVM.ICommandManager" />
  public interface ICommandManagerEx : ICommandManager
  {
    #region Methods

    /// <summary>Registers a command with the specified command name.</summary>
    /// <param name="commandName">Name of the command.</param>
    /// <param name="command">The command.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="viewModel">The view model.</param>
    /// <exception cref="T:System.ArgumentException">The <paramref name="commandName" /> is
    /// <c>null</c> or whitespace.</exception>
    /// <exception cref="T:System.ArgumentNullException">The <paramref name="command" /> is
    /// <c>null</c>.</exception>
    /// <exception cref="T:System.InvalidOperationException">The specified command is not created using the
    /// <see cref="M:Catel.MVVM.CommandManager.CreateCommand(System.String,Catel.Windows.Input.InputGesture,Catel.MVVM.ICompositeCommand,System.Boolean)" />
    /// method.</exception>
    void RegisterCommand(string commandName, ICommand command, object parameter, IViewModel viewModel = null);

    #endregion
  }
}