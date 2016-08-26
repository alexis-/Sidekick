using Catel.Data;
using Sidekick.Shared.Const.SpacedRepetition;
using Sidekick.Shared.Interfaces.SpacedRepetition;

namespace Sidekick.Shared.Base.SRS
{
  public abstract class BaseCard : ModelBase, ICard
  {
    public abstract ConstSpacedRepetition.GradingInfo[] ComputeGrades();
    public abstract bool IsNew();
    public abstract bool IsLearning();
    public abstract bool IsDue();
    public abstract bool IsDismissed();
    public abstract bool IsSuspended();
  }
}
