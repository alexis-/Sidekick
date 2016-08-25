using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catel.MVVM;
using Catel.Reflection;
using Catel.Services;
using Mnemophile.MVVM.ViewModels.SpacedRepetition;

namespace Mnemophile.Windows.ViewModels
{
  public class MainViewModel : ViewModelBase
  {
    public MainViewModel(ILanguageService languageService)
    {
      Title = languageService.GetString("App_Title");

      // VM
      CurrentModel = new CollectionViewModel();

      // Commands
      ShowSettings = new Command(OnShowSettingsExecute);
    }

    public ViewModelBase CurrentModel { get; set; }

    //
    // Commands

    #region Commands

    public Command ShowSettings { get; set; }

    private void OnShowSettingsExecute()
    {
      CurrentModel = new SettingsViewModel();
    }

    #endregion
  }
}
