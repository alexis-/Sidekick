using Catel.Windows.Controls;

namespace Sidekick.Windows.Views.SpacedRepetition
{
  /// <summary>
  /// Interaction logic for CollectionView.xaml
  /// </summary>
  public partial class CollectionView : UserControl
  {
    public CollectionView()
    {
      InitializeComponent();

      CloseViewModelOnUnloaded = false;
    }
  }
}
