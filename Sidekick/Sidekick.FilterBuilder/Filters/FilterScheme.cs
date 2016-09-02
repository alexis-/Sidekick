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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Catel;
using Catel.Runtime.Serialization.Json;
using MethodTimer;
using Sidekick.FilterBuilder.Conditions;
using Sidekick.FilterBuilder.Expressions;
using Sidekick.FilterBuilder.Models.Interfaces;
using Sidekick.FilterBuilder.Services.Interfaces;
using Sidekick.Shared.Attributes.Database;

namespace Sidekick.FilterBuilder.Filters
{
  /// <summary>
  ///   Filter conditions container.
  ///   Provides functionalities to dynamically build a filtering
  ///   Condition Tree.
  /// </summary>
  public class FilterScheme
  {
    #region Fields

    protected readonly IReflectionService _reflectionService;
    protected readonly IJsonSerializer _serializer;

    #endregion

    #region Constructors

    public FilterScheme(
      IReflectionService reflectionService, IJsonSerializer serializer)
    {
      Argument.IsNotNull(() => reflectionService);
      Argument.IsNotNull(() => serializer);

      _reflectionService = reflectionService;
      _serializer = serializer;

      OptimizeTree = true;
    }

    #endregion

    #region Properties

    //
    // DB Properties
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Data
    {
      get { return Serialize(RootItem); }
      set { Deserialize(value); }
    }

    public string Title { get; set; }

    public virtual string TargetTypeName
    {
      get { return TargetType.AssemblyQualifiedName; }
      set { TargetType = Type.GetType(value); }
    }


    //
    // Other properties


    /// <summary>
    ///   If true, will attempt to optimize tree when a
    ///   <see cref="ConditionGroup"/> value is changed.
    ///   Optimization also occurs when changing this value to true.
    /// </summary>
    /// <remarks>Defaults to true.</remarks>
    [Ignore]
    public bool OptimizeTree { get; set; } // TODO : Optimize away

    /// <summary>
    ///   Target type properties metadata
    /// </summary>
    [Ignore]
    public IEnumerable<IPropertyMetadata> TargetProperties { get; set; }

    /// <summary>
    ///   Root <see cref="ConditionTreeItem"/> item, of type
    ///   <see cref="ConditionGroup"/>.
    /// </summary>
    /// <remarks>Defaults to <see cref="ConditionGroupType.And"/></remarks>
    [Ignore]
    public ConditionTreeItem RootItem => ItemCollection.FirstOrDefault();

    [Ignore]
    public ObservableCollection<ConditionTreeItem> ItemCollection { get; set; }

    [Ignore]
    protected Type TargetType { get; set; }

    #endregion

    #region Methods

    /// <summary>
    ///   Initializes scheme asynchronously.
    /// </summary>
    /// <returns>Waitable task.</returns>
    public async Task InitializeAsync(Type targetType)
    {
      TargetType = targetType;

      TargetProperties =
        (await _reflectionService.GetInstancePropertiesAsync(TargetType))
          .Properties;

      ObservableCollection<ConditionTreeItem> itemCollection =
        new ObservableCollection<ConditionTreeItem>();
      ItemCollection = itemCollection;

      itemCollection.Add(CreateConditionGroup(ConditionGroupType.And));
      RootItem.Items.Add(CreatePropertyExpression(RootItem));
    }

    [Time]
    public Expression<Func<T, bool>> ToLinqExpression<T>()
    {
      if (typeof(T) != TargetType)
        throw new InvalidOperationException("Invalid type");

      // TODO: Cache expression

      ParameterExpression parameterExpr = Expression.Parameter(
        typeof(T), "obj");

      return Expression.Lambda<Func<T, bool>>(
        RootItem.ToLinqExpression(parameterExpr), parameterExpr);
    }

    /// <summary>
    ///   Determines whether target item can be removed.
    /// </summary>
    /// <param name="targetItem">
    ///   Target item underlying type should be
    ///   <see cref="PropertyExpression"/>.
    /// </param>
    /// <returns>Whether target item can be removed.</returns>
    public bool CanRemove([Catel.Fody.NotNull] ConditionTreeItem targetItem)
    {
      Argument.IsOfType("targetItem", targetItem, typeof(PropertyExpression));

      // Not root condition
      if (targetItem.Parent.Parent != null)
        return
          targetItem.Parent.Items.Count(i => i is PropertyExpression) > 1;

      // If root condition, check there if there are other properties
      return
        targetItem.Parent.Items.Count(i => i is PropertyExpression) > 1
        || targetItem.Parent.Items.Count > 2;
    }

    /// <summary>
    ///   Adds a new item to target's parent <see cref="ConditionGroup"/> if
    ///   type is the same as parent's <see cref="ConditionGroupType"/>, or 
    ///   create a new <see cref="ConditionGroup"/> containing targetItem and
    ///   a new, blank <see cref="PropertyExpression"/> item otherwise.
    /// </summary>
    /// <param name="targetItem">Target item</param>
    /// <param name="type">Condition type (and/or)</param>
    public void Add(
      [Catel.Fody.NotNull] ConditionTreeItem targetItem,
      ConditionGroupType type)
    {
      Argument.IsOfType("targetItem", targetItem, typeof(PropertyExpression));

      ConditionGroup parent = targetItem.Parent as ConditionGroup;
      PropertyExpression newItem = CreatePropertyExpression();

      // Same condition type, add new item to parent
      if (type == parent.Type)
      {
        int targetItemIndex = parent.Items.IndexOf(targetItem);

        parent.Items.Insert(targetItemIndex + 1, newItem);
        newItem.Parent = parent;
      }

      // Different condition type
      else
      {
        // Special case, where parent is root, and contains a single item.
        // Since condition type is different, and a condition cannot contain
        // a single item of ConditionGroup type, switch root type to new type
        if (parent.Parent == null && parent.Items.Count == 1)
        {
          parent.Type = type;

          parent.Items.Add(newItem);
          newItem.Parent = parent;
        }

        // New condition group
        // Create new condition group, relate with targetItem and newItem, and
        // deny initial parent targetItem's care in profit of that of new
        // condition group
        else
        {
          ConditionGroup newParent = CreateConditionGroup(
            type, parent, targetItem, newItem);

          parent.Items.Remove(targetItem);
          parent.Items.Add(newParent);
        }
      }
    }

    /// <summary>
    ///   Removes specified target item.
    /// </summary>
    /// <param name="targetItem">
    ///   Target item underlying type should be
    ///   <see cref="PropertyExpression"/>.
    /// </param>
    public void Remove([Catel.Fody.NotNull] ConditionTreeItem targetItem)
    {
      Argument.IsOfType("targetItem", targetItem, typeof(PropertyExpression));

      if (!CanRemove(targetItem))
        return;

      ConditionGroup parent = targetItem.Parent as ConditionGroup;
      parent.Items.Remove(targetItem);

      // Last expression in condition group, if parent is not root condition,
      // merge this condition with parent's parent
      if (parent.Items.Count == 1 && parent.Parent != null)
      {
        int propExpIndex = parent.Parent.Items.Count(
          cti => cti is PropertyExpression);

        parent.Parent.Items.Insert(
          propExpIndex, parent.Items.FirstOrDefault());
        parent.Items.Clear();

        parent.Parent.Items.Remove(parent);
      }
    }

    private PropertyExpression CreatePropertyExpression(
      ConditionTreeItem parent = null)
    {
      return new PropertyExpression
      {
        Parent = parent,
        Property = TargetProperties.FirstOrDefault()
      };
    }

    private ConditionGroup CreateConditionGroup(
      ConditionGroupType type, ConditionTreeItem parent = null,
      ConditionTreeItem child1 = null, ConditionTreeItem child2 = null)
    {
      var conditionGroup = new ConditionGroup
      {
        Parent = parent,
        Type = type
      };

      if (child1 != null)
      {
        conditionGroup.Items.Add(child1);
        child1.Parent = conditionGroup;
      }

      if (child2 != null)
      {
        conditionGroup.Items.Add(child2);
        child2.Parent = conditionGroup;
      }

      return conditionGroup;
    }

    private string Serialize(ConditionTreeItem model)
    {
      using (var stream = new MemoryStream())
      {
        _serializer.Serialize(model, stream, null);
        stream.Position = 0L;

        using (var streamReader = new StreamReader(stream))
        {
          return streamReader.ReadToEnd();
        }
      }
    }

    private ConditionTreeItem Deserialize(string json)
    {
      using (var stream = new MemoryStream())
      {
        using (var streamWriter = new StreamWriter(stream))
        {
          streamWriter.Write(json);
        }

        stream.Position = 0L;
        return (ConditionTreeItem)_serializer.Deserialize(json, stream, null);
      }
    }

    #endregion
  }
}