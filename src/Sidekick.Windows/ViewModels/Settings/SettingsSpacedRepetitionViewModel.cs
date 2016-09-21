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

namespace Sidekick.Windows.ViewModels.Settings
{
  using System.Threading.Tasks;
  using System.Windows.Input;

  using AgnosticDatabase.Interfaces;

  using Catel.MVVM;
  using Catel.Services;

  using Sidekick.SpacedRepetition.Generators;
  using Sidekick.SpacedRepetition.Models;

  /// <summary>
  ///   SpacedRepetition-related settings panel View Model
  /// </summary>
  /// <seealso cref="Catel.MVVM.ViewModelBase" />
  public class SettingsSpacedRepetitionViewModel : ViewModelBase
  {
    #region Fields

    private readonly IDatabaseAsync _database;
    private readonly IMessageService _messageService;

    #endregion



    #region Constructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="SettingsSpacedRepetitionViewModel" /> class.
    /// </summary>
    /// <param name="database">Database instance.</param>
    /// <param name="messageService">The message service.</param>
    public SettingsSpacedRepetitionViewModel(
      IDatabaseAsync database, IMessageService messageService) : base(false)
    {
      _database = database;
      _messageService = messageService;

      GenerateTestCollectionCommand =
        new TaskCommand(OnGenerateTestCollectionCommandExecuteAsync);
    }

    #endregion



    #region Properties

    /// <summary>
    /// Gets or sets the generate test collection command.
    /// </summary>
    public ICommand GenerateTestCollectionCommand { get; set; }

    #endregion



    #region Methods

    private async Task OnGenerateTestCollectionCommandExecuteAsync()
    {
      var collectionGenerator =
        new CollectionGenerator(
          new CardGenerator(new TimeGenerator(90), CollectionConfig.Default, 200), 100, 3);

      await collectionGenerator.GenerateAsync(_database).ConfigureAwait(true);

      await
        _messageService.ShowInformationAsync(
                         "Collection successfully Generated", "PageableCollection Debugging")
                       .ConfigureAwait(true);
    }

    #endregion
  }
}