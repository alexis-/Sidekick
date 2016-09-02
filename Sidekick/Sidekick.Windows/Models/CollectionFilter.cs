using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Catel;
using Catel.Logging;
using Catel.Runtime.Serialization.Json;
using MethodTimer;
using Sidekick.FilterBuilder.Conditions;
using Sidekick.FilterBuilder.Expressions;
using Sidekick.FilterBuilder.Filters;
using Sidekick.FilterBuilder.Models.Interfaces;
using Sidekick.FilterBuilder.Services.Interfaces;
using Sidekick.Shared.Interfaces.Database;
using Sidekick.SpacedRepetition.Models;
using SQLite.Net.Attributes;

namespace Sidekick.Windows.Models
{
  public class CollectionFilter : FilterScheme
  {
    private ILog _log;

    public CollectionFilter(
      IReflectionService reflectionService, IJsonSerializer serializer)
      : base(reflectionService, serializer)
    {
      _log = LogManager.GetCurrentClassLogger();
    }

    [Ignore]
    public override string TargetTypeName { get; set; }

    public Task InitializeAsync()
    {
      return InitializeAsync(typeof(Card));
    }

    public ITableQuery<Card> Apply(ITableQuery<Card> query)
    {
      Argument.IsNotNull(() => query);
      
      query.Where(ToLinqExpression<Card>());

      return query;
    }

#if false
    private ParameterExpression _parameterExpr = null;

    [Time]
    private Expression<Func<T, bool>> CreateExpression<T>()
    {
      _parameterExpr = Expression.Parameter(
        typeof(T), "obj");

      return null;
    }

    private Expression ConditionTreeItemToExpression(ConditionTreeItem item)
    {
      if (item is ConditionGroup)
        return ConditionGroupToExpression((ConditionGroup)item);

      if (item is PropertyExpression)
        return PropertyExpressionToExpression((PropertyExpression)item);

      throw new InvalidOperationException(
        "Unsupported ConditionTreeItem type");
    }

    private Expression ConditionGroupToExpression(
      ConditionGroup item)
    {
      var lastExpr = ConditionTreeItemToExpression(item.Items.First());

      if (item.Items.Count <= 1 && item.Parent != null)
      {
        _log.Warning("FilterScheme {0} contains a ConditionGroup node with "
                     + "a single child.", Title);

        return lastExpr;
      }

      for (int i = 1; i < item.Items.Count; i++)
      {
        var rightExpr = ConditionTreeItemToExpression(item.Items[i]);

        switch (item.Type)
        {
          case ConditionGroupType.And:
            lastExpr = Expression.AndAlso(lastExpr, rightExpr);
            break;

          case ConditionGroupType.Or:
            lastExpr = Expression.OrElse(lastExpr, rightExpr);
            break;

          default:
            throw new InvalidOperationException(
              $"Unsupported ConditionGroupType: {item.Type}");
        }
      }

      return lastExpr;
    }

    private Expression PropertyExpressionToExpression(
      PropertyExpression item)
    {
      //item.
      return null;
    }

    private Expression PropertyExpressionToProperty(
      IPropertyMetadata propertyMetadata)
    {
      return Expression.Property(
        _parameterExpr, propertyMetadata.Type, propertyMetadata.Name);
    }

    private Expression ValueDataTypeExpressionToExpression<TValue>(
      ValueDataTypeExpression<TValue> dataTypeExpression,
      Expression propExpr)
      where TValue : struct, IComparable, IFormattable, IComparable<TValue>,
        IEquatable<TValue>
    {
      Expression valueExpr;

      // Nullable type
      if (dataTypeExpression.IsNullable)
      {
        // Null-Check first
        if (dataTypeExpression.SelectedCondition == Condition.IsNull)
          return Expression.Equal(
            propExpr, Expression.Constant(null, typeof(TValue)));

        if (dataTypeExpression.SelectedCondition == Condition.NotIsNull)
          return Expression.NotEqual(
            propExpr, Expression.Constant(null, typeof(TValue)));

        // Not null-check, get provided constant value
        valueExpr = Expression.Constant(
          dataTypeExpression.Value, typeof(TValue?));
      }

      // Not nullable, get provided constant value
      else
        valueExpr = Expression.Constant(
          dataTypeExpression.Value, typeof(TValue));


      // Operators
      switch (dataTypeExpression.SelectedCondition)
      {
        case Condition.EqualTo:
          return Expression.Equal(propExpr, valueExpr);

        case Condition.NotEqualTo:
          return Expression.NotEqual(propExpr, valueExpr);

        case Condition.GreaterThan:
          return Expression.GreaterThan(propExpr, valueExpr);

        case Condition.LessThan:
          return Expression.LessThan(propExpr, valueExpr);

        case Condition.GreaterThanOrEqualTo:
          return Expression.GreaterThanOrEqual(propExpr, valueExpr);

        case Condition.LessThanOrEqualTo:
          return Expression.LessThanOrEqual(propExpr, valueExpr);

        default:
          throw new NotSupportedException(
            string.Format("Condition '{0}' is not supported.",
              dataTypeExpression.SelectedCondition));
      }
    }

    private Expression TimeSpanExpressionToProperty(
      TimeSpanExpression timeSpanExpression, Expression propExpr)
    {
      var valueExpr = Expression.Constant(
        timeSpanExpression.Value, typeof(TimeSpan));

      // Operators
      switch (timeSpanExpression.SelectedCondition)
      {
        case Condition.EqualTo:
          return Expression.Equal(propExpr, valueExpr);

        case Condition.NotEqualTo:
          return Expression.NotEqual(propExpr, valueExpr);

        case Condition.GreaterThan:
          return Expression.GreaterThan(propExpr, valueExpr);

        case Condition.LessThan:
          return Expression.LessThan(propExpr, valueExpr);

        case Condition.GreaterThanOrEqualTo:
          return Expression.GreaterThanOrEqual(propExpr, valueExpr);

        case Condition.LessThanOrEqualTo:
          return Expression.LessThanOrEqual(propExpr, valueExpr);

        default:
          throw new NotSupportedException(
            string.Format("Condition '{0}' is not supported.",
              timeSpanExpression.SelectedCondition));
      }
    }

    private Expression BooleanExpressionToProperty(
      BooleanExpression booleanExpression, Expression propExpr)
    {
      var valueExpr = Expression.Constant(
        booleanExpression.Value, typeof(bool));

      return Expression.Equal(propExpr, valueExpr);
    }

    private Expression StringExpressionToProperty(
      StringExpression stringExpression, Expression propExpr)
    {
      // Check non-value condition types
      if (stringExpression.SelectedCondition == Condition.IsEmpty)
        return Expression.Equal(propExpr, Expression.Constant(string.Empty));

      if (stringExpression.SelectedCondition == Condition.NotIsEmpty)
        return Expression.NotEqual(
          propExpr, Expression.Constant(string.Empty));

      if (stringExpression.SelectedCondition == Condition.IsNull)
        return Expression.Equal(propExpr, Expression.Constant(null));

      if (stringExpression.SelectedCondition == Condition.NotIsNull)
        return Expression.NotEqual(propExpr, Expression.Constant(null));


      // Check value condition types
      var valueExpr = Expression.Constant(
        stringExpression.Value, typeof(string));

      Type[] comparisonMethodParams =
      { typeof(string), typeof(string), typeof(StringComparison) };

      switch (stringExpression.SelectedCondition)
      {
        case Condition.EqualTo:
          return Expression.Equal(propExpr, valueExpr);

        case Condition.NotEqualTo:
          return Expression.NotEqual(propExpr, valueExpr);

        case Condition.Contains:
          return Expression.Call(
            valueExpr,
            typeof(string).GetMethod("Contains", new[] { typeof(string) }));

        case Condition.DoesNotContain:
          return Expression.Negate(Expression.Call(
            valueExpr,
            typeof(string).GetMethod("Contains", new[] { typeof(string) })));

        case Condition.StartsWith:
          return Expression.Call(
            valueExpr,
            typeof(string).GetMethod(
              "StartsWith", new[] { typeof(string), typeof(StringComparison) }),
            Expression.Constant(StringComparison.CurrentCultureIgnoreCase));

        case Condition.DoesNotStartWith:
          return Expression.Negate(Expression.Call(
            valueExpr,
            typeof(string).GetMethod(
              "StartsWith", new[] { typeof(string), typeof(StringComparison) }),
            Expression.Constant(StringComparison.CurrentCultureIgnoreCase)));

        case Condition.EndsWith:
          return Expression.Call(
            valueExpr,
            typeof(string).GetMethod(
              "EndsWith", new[] { typeof(string), typeof(StringComparison) }),
            Expression.Constant(StringComparison.CurrentCultureIgnoreCase));

        case Condition.DoesNotEndWith:
          return Expression.Negate(Expression.Call(
            valueExpr,
            typeof(string).GetMethod(
              "EndsWith", new[] { typeof(string), typeof(StringComparison) }),
            Expression.Constant(StringComparison.CurrentCultureIgnoreCase)));
          /*
        case Condition.Matches:
          return entityValue != null
                 &&
                 _regexIsValidCache.GetFromCacheOrFetch(Value,
                   () => RegexHelper.IsValid(Value))
                 &&
                 _regexCache.GetFromCacheOrFetch(Value, () => new Regex(Value))
                            .IsMatch(entityValue);

        case Condition.DoesNotMatch:
          return entityValue != null
                 &&
                 _regexIsValidCache.GetFromCacheOrFetch(Value,
                   () => RegexHelper.IsValid(Value))
                 &&
                 !_regexCache.GetFromCacheOrFetch(Value, () => new Regex(Value))
                             .IsMatch(entityValue);
                             */
        case Condition.GreaterThan:
          return Expression.GreaterThan(
            Expression.Call(
              Expression.Constant(null, typeof(string)),
              typeof(string).GetMethod("Compare", comparisonMethodParams),
              propExpr, valueExpr,
              Expression.Constant(StringComparison.CurrentCultureIgnoreCase)),
            Expression.Constant(0, typeof(int)));

        case Condition.GreaterThanOrEqualTo:
          return Expression.GreaterThanOrEqual(
            Expression.Call(
              Expression.Constant(null, typeof(string)),
              typeof(string).GetMethod("Compare", comparisonMethodParams),
              propExpr, valueExpr,
              Expression.Constant(StringComparison.CurrentCultureIgnoreCase)),
            Expression.Constant(0, typeof(int)));

        case Condition.LessThan:
          return Expression.LessThan(
            Expression.Call(
              Expression.Constant(null, typeof(string)),
              typeof(string).GetMethod("Compare", comparisonMethodParams),
              propExpr, valueExpr,
              Expression.Constant(StringComparison.CurrentCultureIgnoreCase)),
            Expression.Constant(0, typeof(int)));

        case Condition.LessThanOrEqualTo:
          return Expression.LessThanOrEqual(
            Expression.Call(
              Expression.Constant(null, typeof(string)),
              typeof(string).GetMethod("Compare", comparisonMethodParams),
              propExpr, valueExpr,
              Expression.Constant(StringComparison.CurrentCultureIgnoreCase)),
            Expression.Constant(0, typeof(int)));

        default:
          throw new NotSupportedException(
            string.Format("Condition '{0}' is not supported.",
              stringExpression.SelectedCondition));
      }
    }
#endif
  }
}
