namespace Mnemophile.Interfaces.SRS
{
  public interface ICard
  {
    bool IsNew();
    bool IsLearning();
    bool IsDue();
    bool IsBuried();
    bool IsSuspended();
  }
}