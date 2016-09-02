// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExpression.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Catel.Caching;
using Catel.Caching.Policies;
using Catel.Reflection;
using Sidekick.FilterBuilder.Conditions;
using Sidekick.FilterBuilder.Extensions;
using Sidekick.FilterBuilder.Helpers;
using Sidekick.FilterBuilder.Models.Interfaces;

namespace Sidekick.FilterBuilder.Expressions
{
  [DebuggerDisplay("{ValueControlType} {SelectedCondition} {Value}")]
  public class StringExpression : DataTypeExpression
  {
    #region Fields

    private static readonly CacheStorage<string, Regex> _regexCache =
      new CacheStorage<string, Regex>(
        () => ExpirationPolicy.Sliding(TimeSpan.FromMinutes(1)), false,
        EqualityComparer<string>.Default);

    private static readonly CacheStorage<string, bool> _regexIsValidCache =
      new CacheStorage<string, bool>(
        () => ExpirationPolicy.Sliding(TimeSpan.FromMinutes(1)), false,
        EqualityComparer<string>.Default);

    #endregion

    #region Constructors

    public StringExpression()
    {
      SelectedCondition = Condition.Contains;
      Value = string.Empty;
      ValueControlType = ValueControlType.Text;
    }

    #endregion

    #region Properties

    public string Value { get; set; }

    #endregion

    #region Methods

    public override bool CalculateResult(IPropertyMetadata propertyMetadata,
      object entity)
    {
      var entityValue = propertyMetadata.GetValue<string>(entity);
      if (entityValue == null && propertyMetadata.Type.IsEnumEx())
      {
        var entityValueAsObject = propertyMetadata.GetValue(entity);
        if (entityValueAsObject != null)
        {
          entityValue = entityValueAsObject.ToString();
        }
      }

      switch (SelectedCondition)
      {
        case Condition.Contains:
          return entityValue != null &&
                 entityValue.IndexOf(Value,
                   StringComparison.CurrentCultureIgnoreCase) != -1;

        case Condition.DoesNotContain:
          return entityValue != null &&
                 entityValue.IndexOf(Value,
                   StringComparison.CurrentCultureIgnoreCase) == -1;

        case Condition.EndsWith:
          return entityValue != null &&
                 entityValue.EndsWith(Value,
                   StringComparison.CurrentCultureIgnoreCase);

        case Condition.DoesNotEndWith:
          return entityValue != null &&
                 !entityValue.EndsWith(Value,
                   StringComparison.CurrentCultureIgnoreCase);

        case Condition.EqualTo:
          return entityValue == Value;

        case Condition.GreaterThan:
          return
            string.Compare(entityValue, Value,
              StringComparison.OrdinalIgnoreCase) > 0;

        case Condition.GreaterThanOrEqualTo:
          return
            string.Compare(entityValue, Value,
              StringComparison.OrdinalIgnoreCase) >= 0;

        case Condition.IsEmpty:
          return entityValue == string.Empty;

        case Condition.IsNull:
          return entityValue == null;

        case Condition.LessThan:
          return
            string.Compare(entityValue, Value,
              StringComparison.OrdinalIgnoreCase) < 0;

        case Condition.LessThanOrEqualTo:
          return
            string.Compare(entityValue, Value,
              StringComparison.OrdinalIgnoreCase) <= 0;

        case Condition.NotEqualTo:
          return entityValue != Value;

        case Condition.NotIsEmpty:
          return entityValue != string.Empty;

        case Condition.NotIsNull:
          return entityValue != null;

        case Condition.StartsWith:
          return entityValue != null &&
                 entityValue.StartsWith(Value,
                   StringComparison.CurrentCultureIgnoreCase);

        case Condition.DoesNotStartWith:
          return entityValue != null &&
                 !entityValue.StartsWith(Value,
                   StringComparison.CurrentCultureIgnoreCase);

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

        default:
          throw new NotSupportedException(
            string.Format("Condition '{0}' is not supported.",
              SelectedCondition));
      }
    }

    public override Expression ToLinqExpression(Expression propertyExpr)
    {
      // Check non-value condition types
      if (SelectedCondition == Condition.IsEmpty)
        return Expression.Equal(propertyExpr, Expression.Constant(string.Empty));

      if (SelectedCondition == Condition.NotIsEmpty)
        return Expression.NotEqual(
          propertyExpr, Expression.Constant(string.Empty));

      if (SelectedCondition == Condition.IsNull)
        return Expression.Equal(propertyExpr, Expression.Constant(null));

      if (SelectedCondition == Condition.NotIsNull)
        return Expression.NotEqual(propertyExpr, Expression.Constant(null));


      // Check value condition types
      var valueExpr = Expression.Constant(Value, typeof(string));

      Type[] comparisonMethodParams =
      { typeof(string), typeof(string), typeof(StringComparison) };

      switch (SelectedCondition)
      {
        case Condition.EqualTo:
          return Expression.Equal(propertyExpr, valueExpr);

        case Condition.NotEqualTo:
          return Expression.NotEqual(propertyExpr, valueExpr);

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
              propertyExpr, valueExpr,
              Expression.Constant(StringComparison.CurrentCultureIgnoreCase)),
            Expression.Constant(0, typeof(int)));

        case Condition.GreaterThanOrEqualTo:
          return Expression.GreaterThanOrEqual(
            Expression.Call(
              Expression.Constant(null, typeof(string)),
              typeof(string).GetMethod("Compare", comparisonMethodParams),
              propertyExpr, valueExpr,
              Expression.Constant(StringComparison.CurrentCultureIgnoreCase)),
            Expression.Constant(0, typeof(int)));

        case Condition.LessThan:
          return Expression.LessThan(
            Expression.Call(
              Expression.Constant(null, typeof(string)),
              typeof(string).GetMethod("Compare", comparisonMethodParams),
              propertyExpr, valueExpr,
              Expression.Constant(StringComparison.CurrentCultureIgnoreCase)),
            Expression.Constant(0, typeof(int)));

        case Condition.LessThanOrEqualTo:
          return Expression.LessThanOrEqual(
            Expression.Call(
              Expression.Constant(null, typeof(string)),
              typeof(string).GetMethod("Compare", comparisonMethodParams),
              propertyExpr, valueExpr,
              Expression.Constant(StringComparison.CurrentCultureIgnoreCase)),
            Expression.Constant(0, typeof(int)));

        default:
          throw new NotSupportedException(
            string.Format("Condition '{0}' is not supported.",
              SelectedCondition));
      }
    }

    public override string ToString()
    {
      return string.Format("{0} '{1}'", SelectedCondition.Humanize(), Value);
    }

    #endregion
  }
}