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

using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Catel.Collections;
using Catel.MVVM;
using Catel.Services;
using Sidekick.FilterBuilder.Filters;
using Sidekick.Shared.Interfaces.Database;
using Sidekick.Windows.Models;

namespace Sidekick.Windows.ViewModels.SpacedRepetition
{
  public class BrowserQueryViewerViewModel : ViewModelBase
  {
    private readonly IPleaseWaitService _pleaseWaitService;
    private readonly IDatabase _db;

    #region Constructors

    public BrowserQueryViewerViewModel(
      IDatabase db, IPleaseWaitService pleaseWaitService)
    {
      _db = db;
      _pleaseWaitService = pleaseWaitService;

      AddQueryCommand = new Command(
        OnAddQueryCommandExecute, tag: "AddQuery");
    }

    #endregion

    #region Properties

    public FastObservableCollection<CollectionFilter> Queries { get; set; }
    public FilterScheme SelectedQuery { get; set; }
    public ICommand AddQueryCommand { get; set; }

    #endregion

    #region Methods

    protected override async Task InitializeAsync()
    {
      _pleaseWaitService.Push("Loading");

      await Task.Run(
        () =>
        {
          Queries = new FastObservableCollection<CollectionFilter>(
            _db.Table<CollectionFilter>());
        });
      SelectedQuery = Queries.FirstOrDefault();

      _pleaseWaitService.Pop();
    }

    private void OnAddQueryCommandExecute()
    {
    }

    #endregion
  }
}