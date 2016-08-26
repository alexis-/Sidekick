using Mnemophile.Const.SpacedRepetition;

namespace Mnemophile.Interfaces.SpacedRepetition
{
  public interface ICard
  {
    ConstSpacedRepetition.GradingInfo[] ComputeGrades();

    bool IsNew();
    bool IsLearning();
    bool IsDue();
    bool IsDismissed();
    bool IsSuspended();
  }
}