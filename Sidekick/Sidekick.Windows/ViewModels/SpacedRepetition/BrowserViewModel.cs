// 
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
using Sidekick.MVVM.ViewModels;
using Sidekick.SpacedRepetition.Models;

namespace Sidekick.Windows.ViewModels.SpacedRepetition
{
  [InterestedIn(typeof(BrowserQueryViewerViewModel))]
  public class BrowserViewModel : MainContentViewModelBase
  {
    #region Fields

    private readonly QueryBuilderViewModel _browserQueryBuilderViewModel;
    private readonly BrowserQueryViewerViewModel _browserQueryViewerViewModel;

    #endregion

    #region Constructors

    public BrowserViewModel()
    {
      _browserQueryViewerViewModel =
        TypeFactory.Default.CreateInstance<BrowserQueryViewerViewModel>();
      _browserQueryBuilderViewModel = new QueryBuilderViewModel(typeof(Card));
      // TODO: BrowserQueryBuilder w/ buttons, preview, ...

      CurrentModel = _browserQueryViewerViewModel;
    }

    #endregion

    #region Properties

    public ViewModelBase CurrentModel { get; set; }

    #endregion

    protected override void OnViewModelCommandExecuted(
      IViewModel viewModel, ICatelCommand command, object commandParameter)
    {
      switch (command.Tag as string)
      {
        case "AddQuery":
          break;
      }
    }
  }
}