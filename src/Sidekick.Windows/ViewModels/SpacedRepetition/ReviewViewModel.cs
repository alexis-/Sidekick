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

namespace Sidekick.Windows.ViewModels.SpacedRepetition
{
  using System;
  using System.Linq;
  using System.Threading.Tasks;

  using Catel;
  using Catel.MVVM;
  using Catel.Services;

  using Sidekick.MVVM.ViewModels;
  using Sidekick.Shared.Interfaces.Database;
  using Sidekick.SpacedRepetition.Extensions;
  using Sidekick.SpacedRepetition.Interfaces;
  using Sidekick.SpacedRepetition.Models;
  using Sidekick.Windows.Services.Interfaces;

  /// <summary>Main Spaced Repetition view's model. Should not be unloaded on view close.</summary>
  /// <seealso cref="Catel.MVVM.ViewModelBase" />
  public class ReviewViewModel : MainContentViewModelBase
  {
    #region Fields

    private readonly IDatabaseAsync _database;
    private readonly ILanguageService _languageService;
    private readonly IMessageService _messageService;
    private readonly IPleaseWaitService _pleaseWaitService;
    private readonly ISpacedRepetition _spacedRepetition;

    private IReviewCollection _reviewCollection;

    #endregion



    #region Constructors

    /// <summary>Initializes a new instance of the <see cref="ReviewViewModel" /> class.</summary>
    /// <param name="database">The database.</param>
    /// <param name="spacedRepetition">The spaced repetition.</param>
    /// <param name="languageService">The language service.</param>
    /// <param name="pleaseWaitService">The please wait service.</param>
    /// <param name="messageService">The message service.</param>
    /// <param name="commandManager">The command manager.</param>
    public ReviewViewModel(
      IDatabaseAsync database, ISpacedRepetition spacedRepetition,
      ILanguageService languageService, IPleaseWaitService pleaseWaitService,
      IMessageService messageService, ICommandManagerEx commandManager)
    {
      _database = database;
      _spacedRepetition = spacedRepetition;
      _languageService = languageService;
      _pleaseWaitService = pleaseWaitService;
      _messageService = messageService;

      AnswerTaskCommand = new TaskCommand<Grade>(
        OnAnswerExecuteAsync, g => !AnswerTaskCommand.IsExecuting);
      AnswerKeyPressCommand = new Command<int>(
        OnAnswerKeyPressExecute,
        i => i <= ReviewAnswerInfos.Length && !AnswerTaskCommand.IsExecuting);

      foreach (
        var answerGesture in Commands.SpacedRepetition.CollectionReview.AnswerGestures.Take(5))
        commandManager.RegisterCommand(
          answerGesture.Item1, AnswerKeyPressCommand, answerGesture.Item3);
    }

    #endregion



    #region Properties

    /// <summary>Gets or sets the card.</summary>
    [Model]
    public Card Card { get; set; }

    /// <summary>Gets or sets the grade infos.</summary>
    public ReviewAnswerInfo[] ReviewAnswerInfos { get; set; }

    /// <summary>Gets or sets the answer task command.</summary>
    public TaskCommand<Grade> AnswerTaskCommand { get; set; }

    /// <summary>Gets or sets the answer task command.</summary>
    public Command<int> AnswerKeyPressCommand { get; set; }

    #endregion



    #region Methods

    /// <summary>
    ///   Called when Main view's content ViewModel is changing. Allows to interrupt navigation
    ///   if ongoing work might be lost.
    /// </summary>
    /// <returns>Whether to continue navigation</returns>
    public override Task<bool> OnContentChange()
    {
      // If a review session is ongoing, prompt user to validate interruption
      return _reviewCollection?.Current != null
               ? OnContentChangeAsync()
               : base.OnContentChange();
    }

    /// <inheritdoc />
    protected override async Task InitializeAsync()
    {
      await base.InitializeAsync().ConfigureAwait(true);

      // Get review collection
      _reviewCollection = _spacedRepetition.GetReviewCollection(_database);

      // Wait for initialization

      if (!_reviewCollection.Initialized.IsCompleted && !_reviewCollection.Initialized.Wait(100))
      {
        _pleaseWaitService.Push(_languageService.GetString("SpacedRepetition_Review_Loading"));

        if (await _reviewCollection.Initialized.ConfigureAwait(true))
          DisplayCard();

        _pleaseWaitService.Pop();
      }
      else if (_reviewCollection.Initialized.Result)
        DisplayCard();
    }

    private async Task OnAnswerExecuteAsync(Grade grade)
    {
      if (await _reviewCollection.AnswerAsync(grade).ConfigureAwait(true))
        DisplayCard();

      else
        await _messageService.ShowInformationAsync("All cards reviewed.").ConfigureAwait(false);
    }

    private void OnAnswerKeyPressExecute(int i)
    {
      Argument.IsMaximum(() => i, ReviewAnswerInfos.Length);
      Argument.IsMinimal(() => i, 1);

      AnswerTaskCommand.Execute(ReviewAnswerInfos[i - 1]);
    }

    private void DisplayCard()
    {
      // Get current card
      Card card = _reviewCollection.Current;

      if (card == null)
        throw new InvalidOperationException("Current card is NULL.");

      // Get grading options, and display buttons
      ReviewAnswerInfos = card.ComputeGrades();
    }

    private async Task<bool> OnContentChangeAsync()
    {
      MessageResult messageResult =
        await
          _messageService.ShowAsync(
                           _languageService.GetString(
                             "SpacedRepetition_Review_NavigateAwayPrompt"), string.Empty,
                           MessageButton.YesNoCancel, MessageImage.Question)
                         .ConfigureAwait(true);

      // Cancel review
      if (messageResult == MessageResult.No)
        _reviewCollection = null;

      return messageResult != MessageResult.Cancel;
    }

    #endregion
  }
}