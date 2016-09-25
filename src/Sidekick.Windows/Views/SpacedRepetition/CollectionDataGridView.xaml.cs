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

namespace Sidekick.Windows.Views.SpacedRepetition
{
  using Catel.Windows.Controls;

  using Sidekick.Windows.ViewModels.SpacedRepetition;

  /// <summary>Interaction logic for CollectionDataGridView.xaml</summary>
  public partial class CollectionDataGridView : UserControl
  {
    #region Constructors

    /// <summary>Initializes a new instance of the <see cref="CollectionDataGridView" /> class.</summary>
    /// <remarks>This method is required for design time support.</remarks>
    public CollectionDataGridView()
    {
      InitializeComponent();

      CloseViewModelOnUnloaded = false;
    }

    #endregion



    #region Methods

    /// <summary>
    ///   Called when the <see cref="P:Catel.Windows.Controls.UserControl.ViewModel" /> has
    ///   changed.
    /// </summary>
    /// <remarks>
    ///   This method does not implement any logic and saves a developer from
    ///   subscribing/unsubscribing to the
    ///   <see cref="E:Catel.Windows.Controls.UserControl.ViewModelChanged" /> event inside the same
    ///   user control.
    /// </remarks>
    protected override void OnViewModelChanged()
    {
      if (ViewModel != null)
      {
        var viewModel = (CollectionDataGridViewModel)ViewModel;

        viewModel.SfDataPager = DataPager;
      }

      base.OnViewModelChanged();
    }

    #endregion
  }
}