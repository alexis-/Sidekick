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
  using System;
  using System.Collections.Generic;
  using System.Threading.Tasks;

  using Catel;
  using Catel.Data;
  using Catel.IoC;
  using Catel.MVVM;

  using Orc.FilterBuilder.ViewModels;

  using Sidekick.Shared.Interfaces.Database;
  using Sidekick.Shared.Utils;
  using Sidekick.Windows.Models;
  using Sidekick.WPF.Controls;

  /// <summary>
  ///   Link QueryBuilderView[Model] and other views (preview, command buttons, ...)
  /// </summary>
  /// <seealso cref="Catel.MVVM.ViewModelBase" />
  [InterestedIn(typeof(FilterBuilderViewModel))]
  public class BrowserQueryBuilderViewModel : ViewModelBase, IRadioControllerMonitor
  {
    #region Fields

    private readonly IDatabaseAsync _db;

    private readonly FilterBuilderViewModel _filterBuilderViewModel;

    #endregion



    #region Constructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="BrowserQueryBuilderViewModel" /> class.
    /// </summary>
    /// <param name="db">Database instance.</param>
    public BrowserQueryBuilderViewModel(IDatabaseAsync db)
      : this(TypeFactory.Default.CreateInstance<CollectionQuery>(), db)
    {
      CurrentQuery.Title = $"New Query {DateTime.Now}";
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="BrowserQueryBuilderViewModel" /> class.
    /// </summary>
    /// <param name="currentQuery">Query to edit.</param>
    /// <param name="db">Database instance.</param>
    public BrowserQueryBuilderViewModel(CollectionQuery currentQuery, IDatabaseAsync db)
      : base(true)
    {
      Argument.IsNotNull(() => db);
      Argument.IsNotNull(() => currentQuery);

      _db = db;

      CurrentQuery = currentQuery;
      RadioController = new RadioController(this);

      _filterBuilderViewModel =
        TypeFactory.Default.CreateInstanceWithParametersAndAutoCompletion(
                     typeof(FilterBuilderViewModel), CurrentQuery) as FilterBuilderViewModel;

      SaveCommand = new TaskCommand(
        OnSaveCommandExecuteAsync, OnSaveCommandCanExecute, "QueryBuilderDone");
      CancelCommand = new TaskCommand(OnCancelCommandExecuteAsync, tag: "QueryBuilderDone");
    }

    #endregion



    #region Properties

    /// <summary>
    ///   Handle content toggle buttons (builder/preview).
    /// </summary>
    public RadioController RadioController { get; set; }

    /// <summary>
    ///   Displayed view model (builder or preview).
    /// </summary>
    public ViewModelBase CurrentModel { get; set; }

    /// <summary>
    ///   Current query filter.
    /// </summary>
    [Model]
    public CollectionQuery CurrentQuery { get; set; }

    /// <summary>
    ///   Current filter title.
    /// </summary>
    [ViewModelToModel("CurrentQuery", "Title")]
    public string QueryTitle { get; set; }

    /// <summary>
    ///   Whether filter expression is valid.
    /// </summary>
    [ViewModelToModel("CurrentQuery", "IsExpressionValid")]
    public bool IsQueryValid { get; set; }

    /// <summary>
    ///   Save query.
    /// </summary>
    public TaskCommand SaveCommand { get; set; }

    /// <summary>
    ///   Cancel operations and return to datagrid view.
    /// </summary>
    public TaskCommand CancelCommand { get; set; }

    #endregion



    #region Methods

    /// <summary>
    ///   Notify on selection changes and allows to asynchronously validate whether to endorse it
    ///   or not.
    /// </summary>
    /// <param name="selectedItem">The selected item.</param>
    /// <param name="parameter">Item context (e.g. binding).</param>
    /// <returns>
    ///   Waitable task for validation result.
    /// </returns>
    public Task<bool> RadioControllerOnSelectionChangedAsync(
      object selectedItem, object parameter)
    {
      switch (parameter as string)
      {
        case "Builder":
          SetCurrentModel(_filterBuilderViewModel);
          break;

        case "Preview":
          var modelView =
            TypeFactory.Default.CreateInstanceWithParametersAndAutoCompletion(
                         typeof(CollectionDataGridViewModel), CurrentQuery) as ViewModelBase;

          SetCurrentModel(modelView);
          break;
      }

      return TaskConstants.BooleanTrue;
    }

    /// <inheritdoc />
    protected override Task InitializeAsync()
    {
      CurrentModel = _filterBuilderViewModel;

      return base.InitializeAsync();
    }

    /// <inheritdoc />
    protected override void ValidateFields(List<IFieldValidationResult> validationResults)
    {
      if (string.IsNullOrWhiteSpace(QueryTitle))
        validationResults.Add(
          FieldValidationResult.CreateError(() => QueryTitle, "Title cannot be empty"));
    }

    /// <inheritdoc />
    protected override void OnViewModelPropertyChanged(
      IViewModel viewModel, string propertyName)
    {
      // TODO: Do something

      base.OnViewModelPropertyChanged(viewModel, propertyName);
    }

    private void SetCurrentModel(ViewModelBase viewModel)
    {
      if (CurrentModel != viewModel)
        CurrentModel = viewModel;
    }

    private async Task OnCancelCommandExecuteAsync()
    {
      await CancelViewModelAsync().ConfigureAwait(false);
    }

    private bool OnSaveCommandCanExecute()
    {
      return IsQueryValid && !SaveCommand.IsExecuting;
    }

    private async Task OnSaveCommandExecuteAsync()
    {
      await _db.InsertOrReplaceAsync(CurrentQuery).ConfigureAwait(false);
    }

    #endregion
  }
}