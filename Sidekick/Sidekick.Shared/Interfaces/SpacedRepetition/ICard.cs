using Sidekick.Shared.Const.SpacedRepetition;

namespace Sidekick.Shared.Interfaces.SpacedRepetition
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