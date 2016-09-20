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

namespace Sidekick.WPF.Controls
{
  using System.ComponentModel;
  using System.Linq;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Input;

  /// <summary>Sort controller which allows custom sorting implementation.</summary>
  public interface ISortController
  {
    #region Methods

    /// <summary>Called when control sorting is updated.</summary>
    /// <param name="sortDescriptions">The sort descriptions.</param>
    /// <returns>Whether to refresh items or not.</returns>
    bool OnSorting(SortDescriptionCollection sortDescriptions);

    /// <summary>Determines whether sort is enabled.</summary>
    bool CanSort();

    #endregion
  }

  /// <summary>Extends <see cref="DataGrid" /> to add new features (sorting, ...)</summary>
  /// <seealso cref="System.Windows.Controls.DataGrid" />
  public class DataGridEx : DataGrid
  {
    #region Fields

    /// <summary>The sort controller property</summary>
    public static readonly DependencyProperty SortControllerProperty =
      DependencyProperty.Register(
        "SortController", typeof(ISortController), typeof(DataGridEx),
        new PropertyMetadata(default(ISortController)));

    /// <summary>Local record of sorting definitions.</summary>
    private readonly SortDescriptionCollection _sortDescriptions =
      new SortDescriptionCollection();

    #endregion



    #region Properties

    /// <summary>Gets or sets the (optional) sort controller.</summary>
    public ISortController SortController
    {
      get { return (ISortController)GetValue(SortControllerProperty); }
      set { SetValue(SortControllerProperty, value); }
    }

    #endregion



    #region Methods

    /// <summary>Raises the <see cref="E:System.Windows.Controls.DataGrid.Sorting" /> event.</summary>
    /// <param name="eventArgs">The data for the event.</param>
    protected override void OnSorting(DataGridSortingEventArgs eventArgs)
    {
      if (SortController != null)
      {
        eventArgs.Handled = true;

        // Can't sort right now, just update columns sort definitions.
        if (!SortController.CanSort())
        {
          UpdateColumnSortDefinitions();

          return;
        }

        // Just in case, remove data grid sorting descriptions.
        if (Items.SortDescriptions.Any())
          Items.SortDescriptions.Clear();

        // If user is holding shift, clear all columns sorting.
        if ((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift)
          _sortDescriptions.Clear();

        // Update our sort record.
        UpdateSortDefinitions(eventArgs.Column);

        // Actually do the sorting.
        if (SortController.OnSorting(_sortDescriptions))
          Items.Refresh();
      }

      else
        base.OnSorting(eventArgs);
    }

    private void UpdateSortDefinitions(DataGridColumn updatedColumn)
    {
      // Not nullable, check if there already is a sort description for that column.
      if (_sortDescriptions.Any(i => i.PropertyName == updatedColumn.SortMemberPath))
      {
        // Get the description.
        var columnSortDescription =
          _sortDescriptions.FirstOrDefault(i => i.PropertyName == updatedColumn.SortMemberPath);

        var columnSortDirection = updatedColumn.SortDirection;

        // Always remove from definitions, updating value throws an exception.
        _sortDescriptions.Remove(columnSortDescription);

        // If there is a direction, add the new value.
        if (columnSortDirection != null)
          _sortDescriptions.Add(
            new SortDescription(updatedColumn.SortMemberPath, GetSortDirection(updatedColumn)));
      }

      // Add column to definitions.
      else
        _sortDescriptions.Add(
          new SortDescription(updatedColumn.SortMemberPath, GetSortDirection(updatedColumn)));

      UpdateColumnSortDefinitions();
    }

    private void UpdateColumnSortDefinitions()
    {
      // Update columns order, base DataGrid clears other columns directions everytime.
      foreach (var sortDescription in _sortDescriptions)
      {
        var column =
          Columns.FirstOrDefault(c => c.SortMemberPath == sortDescription.PropertyName);

        if (column != null)
          column.SortDirection = sortDescription.Direction;
      }
    }

    private ListSortDirection GetSortDirection(DataGridColumn column)
    {
      var sortDirection = ListSortDirection.Ascending;
      var columnSortDirection = column.SortDirection;

      if (columnSortDirection.HasValue
          && columnSortDirection.Value == ListSortDirection.Ascending)
        sortDirection = ListSortDirection.Descending;

      return sortDirection;
    }

    #endregion
  }
}