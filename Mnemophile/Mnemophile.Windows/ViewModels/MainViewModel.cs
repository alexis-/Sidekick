using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catel.IoC;
using Catel.MVVM;
using Catel.Reflection;
using Catel.Services;
using Mnemophile.MVVM.ViewModels.SpacedRepetition;

namespace Mnemophile.Windows.ViewModels
{
  public class MainViewModel : ViewModelBase
  {
    #region Constructors
    //
    // Constructors
    public MainViewModel(ILanguageService languageService)
    {
      Title = languageService.GetString("App_Title");

      // VM
      CurrentModel =
        TypeFactory.Default.CreateInstance<CollectionViewModel>();

      // Commands
      ShowSettings = new Command(OnShowSettingsExecute);
    }

    #endregion



    #region Properties
    //
    // Properties

    public ViewModelBase CurrentModel { get; set; }

    #endregion



    #region Commands
    //
    // Commands

    public Command ShowSettings { get; set; }

    private void OnShowSettingsExecute()
    {
      CurrentModel = new SettingsViewModel();
    }

    #endregion
  }
}
