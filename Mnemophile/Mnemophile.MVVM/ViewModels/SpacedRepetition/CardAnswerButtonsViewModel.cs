using Catel.Fody;
using Catel.MVVM;
using Mnemophile.Const.SRS;

namespace Mnemophile.MVVM.ViewModels.SpacedRepetition
{
  public class CardAnswerButtonsViewModel : ViewModelBase
  {
    public ConstSRS.GradingInfo[] GradingInfos { get; set; }

    public CardAnswerButtonsViewModel(
      [NotNull]ConstSRS.GradingInfo[] gradingInfos)
    {
      GradingInfos = gradingInfos;
    }
  }
}
