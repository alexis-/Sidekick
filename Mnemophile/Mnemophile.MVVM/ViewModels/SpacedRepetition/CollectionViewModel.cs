using System.Threading.Tasks;
using Catel.MVVM;
using Mnemophile.Interfaces.DB;
using Mnemophile.Interfaces.SRS;

namespace Mnemophile.MVVM.ViewModels.SpacedRepetition
{
  public class CollectionViewModel : ViewModelBase
  {
    private readonly IDatabase _database;

    public CollectionViewModel(IDatabase database)
    {
      _database = database;
    }

    protected override Task InitializeAsync()
    {
      IReviewCollection reviewCollection = null;

      return base.InitializeAsync();
    }
  }
}
