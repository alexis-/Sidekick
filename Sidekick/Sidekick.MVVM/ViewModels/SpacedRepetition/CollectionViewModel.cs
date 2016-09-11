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

namespace Sidekick.MVVM.ViewModels.SpacedRepetition
{
  using System;
  using System.Threading.Tasks;

  using Catel.MVVM;
  using Catel.Services;

  using Sidekick.Shared.Interfaces.Database;
  using Sidekick.SpacedRepetition.Const;
  using Sidekick.SpacedRepetition.Interfaces;
  using Sidekick.SpacedRepetition.Models;

  /// <summary>
  ///   Main Spaced Repetition view's model.
  ///   Should not be unloaded on view close.
  /// </summary>
  /// <seealso cref="Catel.MVVM.ViewModelBase" />
  [InterestedIn(typeof(CardAnswerButtonsViewModel))]
  public class CollectionViewModel : MainContentViewModelBase
  {
    #region Fields

    //
    // Attributes

    private readonly IDatabaseAsync _database;
    private readonly ILanguageService _languageService;
    private readonly IMessageService _messageService;
    private readonly IPleaseWaitService _pleaseWaitService;
    private readonly ISpacedRepetition _spacedRepetition;

    private IReviewCollection _reviewCollection;

    #endregion



    #region Constructors

    //
    // Constructors

    public CollectionViewModel(
      IDatabaseAsync database, ISpacedRepetition spacedRepetition,
      ILanguageService languageService, IPleaseWaitService pleaseWaitService,
      IMessageService messageService)
    {
      _database = database;
      _spacedRepetition = spacedRepetition;
      _languageService = languageService;
      _pleaseWaitService = pleaseWaitService;
      _messageService = messageService;
    }

    #endregion



    #region Properties

    //
    // Properties
    [Model]
    //[Expose("Data")]
    public Card Card { get; set; }

    public GradeInfo[] GradeInfos { get; set; }

    #endregion



    #region Methods

    /// <summary>
    ///   Called when Main view's content ViewModel is changing.
    ///   Allows to interrupt navigation if ongoing work might be lost.
    /// </summary>
    /// <returns>
    ///   Whether to continue navigation
    /// </returns>
    public override Task<bool> OnContentChange()
    {
      // If a review session is ongoing, prompt user to validate interruption
      return _reviewCollection?.Current != null
               ? OnContentChangeAsync()
               : base.OnContentChange();
    }

    private async Task<bool> OnContentChangeAsync()
    {
      MessageResult messageResult =
        await
          _messageService.ShowAsync(
            _languageService.GetString("SpacedRepetition_Review_NavigateAwayPrompt"), "",
            MessageButton.YesNoCancel, MessageImage.Question);

      // Cancel review
      if (messageResult == MessageResult.No)
        _reviewCollection = null;

      return messageResult != MessageResult.Cancel;
    }

    private void DisplayCard()
    {
      // Get current card
      Card card = _reviewCollection.Current;

      if (card == null)
        throw new InvalidOperationException("Current card is NULL.");

      // Get grading options, and display buttons
      GradeInfos = card.ComputeGrades();
    }

    private async Task AnswerCardAsync(Grade grade)
    {
      if (await _reviewCollection.AnswerAsync(grade).ConfigureAwait(true))
        DisplayCard();

      else
        await _messageService.ShowInformationAsync("All cards reviewed.").ConfigureAwait(false);
    }

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

    protected override void OnViewModelCommandExecuted(
      IViewModel viewModel, ICatelCommand command, object commandParameter)
    {
      // TODO: This is not correct. Button will be enabled and may be pressed several times.
      if (commandParameter is Grade)
#pragma warning disable 4014
        AnswerCardAsync((Grade)commandParameter);
#pragma warning restore 4014

      base.OnViewModelCommandExecuted(viewModel, command, commandParameter);
    }

    #endregion
  }
}