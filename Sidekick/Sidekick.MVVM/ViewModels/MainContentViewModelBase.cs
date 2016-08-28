using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catel.MVVM;
using Sidekick.Shared.Utils;

namespace Sidekick.MVVM.ViewModels
{
  public abstract class MainContentViewModelBase : ViewModelBase
  {
    /// <summary>
    /// Called when Main view's content ViewModel is changing.
    /// Allows to interrupt navigation if ongoing work might be lost.
    /// </summary>
    /// <returns>Whether to continue navigation</returns>
    public virtual Task<bool> OnContentChange()
    {
      return TaskConstants.BooleanTrue;
    }
  }
}
