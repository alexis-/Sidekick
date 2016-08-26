using Catel.Fody;
using Catel.MVVM;
using Mnemophile.Const.SpacedRepetition;

namespace Mnemophile.MVVM.ViewModels.SpacedRepetition
{
  public class CardAnswerButtonsViewModel : ViewModelBase
  {
    public ConstSpacedRepetition.GradingInfo[] GradingInfos { get; set; }

    public CardAnswerButtonsViewModel(
      [NotNull]ConstSpacedRepetition.GradingInfo[] gradingInfos)
    {
      GradingInfos = gradingInfos;
    }
  }
}
