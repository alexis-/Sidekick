// 
// The MIT License (MIT)
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

namespace Sidekick.Windows.ViewModels.SpacedRepetition
{
  using System.Threading.Tasks;

  using Catel.IoC;
  using Catel.Messaging;
  using Catel.MVVM;

  using Orc.FilterBuilder.Models;

  using Sidekick.MVVM.ViewModels;

  /// <summary>
  ///   View Model for main browser view. Handles navigation between the different parts of
  ///   browser (display content, add queries, ...)
  /// </summary>
  /// <seealso cref="Sidekick.MVVM.ViewModels.MainContentViewModelBase" />
  [InterestedIn(typeof(BrowserQueryViewerViewModel))]
  [InterestedIn(typeof(BrowserQueryBuilderViewModel))]
  public class BrowserViewModel : MainContentViewModelBase
  {
    #region Fields

    private readonly BrowserQueryViewerViewModel _browserQueryViewerViewModel;

    #endregion



    #region Constructors

    /// <summary>Initializes a new instance of the <see cref="BrowserViewModel" /> class.</summary>
    /// <param name="mediator">The mediator.</param>
    public BrowserViewModel(IMessageMediator mediator)
    {
      _browserQueryViewerViewModel =
        TypeFactory.Default.CreateInstance<BrowserQueryViewerViewModel>();

      CurrentModel = _browserQueryViewerViewModel;

      mediator.Register<SimpleMessage>(this, OnMessageReceived);
    }

    #endregion



    #region Properties

    /// <summary>Currently displayed ViewModel.</summary>
    public ViewModelBase CurrentModel { get; set; }

    #endregion



    #region Methods

    /// <inheritdoc />
    /// <summary>Handles commands from child ViewModels (Add query, ...)</summary>
    protected override void OnViewModelCommandExecuted(
      IViewModel viewModel, ICatelCommand command, object commandParameter)
    {
      switch (command.Tag as string)
      {
        case "AddQuery":
          ShowQueryBuilder();
          break;

        case "EditQuery":
          ShowQueryBuilder(commandParameter as FilterScheme);
          break;
      }
    }

    /// <summary>
    ///   Closes this instance. Always called after the
    ///   <see cref="M:Catel.MVVM.ViewModelBase.Cancel" /> of
    ///   <see cref="M:Catel.MVVM.ViewModelBase.Save" /> method.
    /// </summary>
    /// <returns></returns>
    protected override Task CloseAsync()
    {
      return _browserQueryViewerViewModel.CloseViewModelAsync(null);
    }

    private void OnMessageReceived(SimpleMessage msg)
    {
      switch (msg.Data)
      {
        case BrowserQueryBuilderViewModel.MessageDone:
          CurrentModel = _browserQueryViewerViewModel;
          break;
      }
    }

    private void ShowQueryBuilder(FilterScheme filterScheme = null)
    {
      CurrentModel = filterScheme == null
                       ? TypeFactory.Default.CreateInstance<BrowserQueryBuilderViewModel>()
                       : TypeFactory.Default.CreateInstanceWithParametersAndAutoCompletion(
                                      typeof(BrowserQueryBuilderViewModel), filterScheme) as
                           BrowserQueryBuilderViewModel;
    }

    #endregion
  }
}