using Mnemophile.Const.SRS;

namespace Mnemophile.Interfaces.SRS
{
  public interface ICard
  {
    void Answer(ConstSRS.Grade grade);
    ConstSRS.GradingInfo[] ComputeGrades();

    bool IsNew();
    bool IsLearning();
    bool IsDue();
    bool IsDismissed();
    bool IsSuspended();
  }
}