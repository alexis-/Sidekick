﻿// 
// The MIT License (MIT)
// Copyright (c) 2016 Incogito
// 
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the 
// Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using Catel.IoC;
using Catel.MVVM;
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