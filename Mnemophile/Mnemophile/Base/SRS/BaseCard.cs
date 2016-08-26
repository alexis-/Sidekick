using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catel.Data;
using Mnemophile.Const.SpacedRepetition;
using Mnemophile.Interfaces.SpacedRepetition;

namespace Mnemophile.Base.SpacedRepetition
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
