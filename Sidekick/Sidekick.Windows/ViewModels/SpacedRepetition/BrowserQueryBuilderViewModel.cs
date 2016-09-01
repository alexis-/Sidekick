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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Catel.IoC;
using Catel.MVVM;
using Sidekick.FilterBuilder.Conditions;
using Sidekick.FilterBuilder.Filters;
using Sidekick.FilterBuilder.Models.Interfaces;
using Sidekick.MVVM.ViewModels;

namespace Sidekick.Windows.ViewModels.SpacedRepetition
{
  public class BrowserQueryBuilderViewModel : MainContentViewModelBase
  {
    #region Fields

    private readonly FilterScheme _currentQuery;

    private readonly Type _targetType;

    #endregion

    #region Constructors

    public BrowserQueryBuilderViewModel(Type targetType)
    {
      _targetType = targetType;
      _currentQuery =
        (FilterScheme)TypeFactory.Default.CreateInstance(
          typeof(FilterScheme));

      DeleteCommand = new Command<ConditionTreeItem>(
        OnDeleteCommandExecute, OnDeleteCommandCanExecute);
      AddAndConditionCommand = new Command<ConditionTreeItem>(
        OnAddAndConditionCommandExecute, OnAddAndConditionCommandCanExecute);
      AddOrConditionCommand = new Command<ConditionTreeItem>(
        OnAddOrConditionCommandExecute, OnAddOrConditionCommandCanExecute);
    }

    #endregion

    #region Properties

    public IEnumerable<ConditionTreeItem> ItemCollection =>
      _currentQuery.ItemCollection;

    public IEnumerable<IPropertyMetadata> TargetProperties =>
      _currentQuery.TargetProperties;

    public ICommand AddAndConditionCommand { get; set; }
    public ICommand AddOrConditionCommand { get; set; }
    public ICommand DeleteCommand { get; set; }

    #endregion

    #region Methods

    protected override async Task InitializeAsync()
    {
      await _currentQuery.InitializeAsync(_targetType);
    }

    private bool OnAddAndConditionCommandCanExecute(ConditionTreeItem item)
    {
      return true;
    }

    private void OnAddAndConditionCommandExecute(ConditionTreeItem item)
    {
      _currentQuery.Add(item, ConditionGroupType.And);
    }

    private bool OnAddOrConditionCommandCanExecute(ConditionTreeItem item)
    {
      return true;
    }

    private void OnAddOrConditionCommandExecute(ConditionTreeItem item)
    {
      _currentQuery.Add(item, ConditionGroupType.Or);
    }

    private bool OnDeleteCommandCanExecute(ConditionTreeItem item)
    {
      return _currentQuery.CanRemove(item);
    }

    private void OnDeleteCommandExecute(ConditionTreeItem item)
    {
      _currentQuery.Remove(item);
    }

    #endregion
  }
}