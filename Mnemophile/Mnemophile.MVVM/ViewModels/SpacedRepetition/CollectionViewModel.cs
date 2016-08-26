using System;
using System.Threading.Tasks;
using Catel.Fody;
using Catel.IoC;
using Catel.MVVM;
using Catel.Services;
using Catel.Threading;
using Mnemophile.Base.SpacedRepetition;
using Mnemophile.Const.SpacedRepetition;
using Mnemophile.Interfaces.DB;
using Mnemophile.Interfaces.SpacedRepetition;
using Mnemophile.MVVM.Properties;

namespace Mnemophile.MVVM.ViewModels.SpacedRepetition
{
  public class CollectionViewModel : ViewModelBase
  {
    #region Fields
    //
    // Attributes

    private readonly IDatabase _database;
    private readonly ISpacedRepetition _spacedRepetition;
    private readonly ILanguageService _languageService;
    private readonly IPleaseWaitService _pleaseWaitService;

    private IReviewCollection _reviewCollection;

    #endregion



    #region Constructors
    //
    // Constructors

    public CollectionViewModel(
      IDatabase database,
      ISpacedRepetition spacedRepetition,
      ILanguageService languageService,
      IPleaseWaitService pleaseWaitService)
    {
      _database = database;
      _spacedRepetition = spacedRepetition;
      _languageService = languageService;
      _pleaseWaitService = pleaseWaitService;
    }

    #endregion



    #region Properties
    //
    // Properties
    [Model]
    //[Expose("Data")]
    public BaseCard Card { get; set; }

    public ConstSpacedRepetition.GradingInfo[] Gradings { get; set; }

    #endregion



    #region Methods
    //
    // Initialization

    protected override async Task InitializeAsync()
    {
      await base.InitializeAsync();

      // Get review collection
      _reviewCollection =
        _spacedRepetition.GetReviewCollection(_database);

      // Wait for initialization

      if (!_reviewCollection.Initialized.IsCompleted
          && !_reviewCollection.Initialized.Wait(100))
      {
        _pleaseWaitService.Push(_languageService.GetString(
          "SpacedRepetition_Review_Loading"));

        if (await _reviewCollection.Initialized)
          DisplayCard();

        _pleaseWaitService.Pop();
      }
      else if (_reviewCollection.Initialized.Result)
        DisplayCard();
    }

    

    //
    // Core methods

    private void DisplayCard()
    {
      // Get current card
      ICard card = _reviewCollection.Current;

      if (card == null)
        throw new InvalidOperationException("Current card is NULL.");

      // Get grading options, and display buttons
      Gradings = card.ComputeGrades();
    }

    #endregion
  }
}
