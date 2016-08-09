using Mnemophile.Const.SRS;

namespace Mnemophile.Interfaces.SRS
{
  public interface ICard
  {
    ConstSRS.GradingInfo[] ComputeGrades();

    bool IsNew();
    bool IsLearning();
    bool IsDue();
    bool IsDismissed();
    bool IsSuspended();
  }
}