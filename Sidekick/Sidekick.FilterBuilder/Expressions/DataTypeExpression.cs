// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataTypeExpression.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using System.Linq.Expressions;
using Catel.Data;
using Sidekick.FilterBuilder.Conditions;
using Sidekick.FilterBuilder.Conditions.Helpers;
using Sidekick.FilterBuilder.Models.Interfaces;

namespace Sidekick.FilterBuilder.Expressions
{
  public abstract class DataTypeExpression : ModelBase
  {
    protected DataTypeExpression()
    {
      IsValueRequired = true;
    }

    #region Properties

    public Condition SelectedCondition { get; set; }

    public bool IsValueRequired { get; set; }

    public ValueControlType ValueControlType { get; set; }

    #endregion

    #region Methods

    private void OnSelectedConditionChanged()
    {
      IsValueRequired = ConditionHelper.GetIsValueRequired(SelectedCondition);
    }

    public abstract bool CalculateResult(IPropertyMetadata propertyMetadata,
      object entity);
    public abstract Expression ToLinqExpression(Expression propertyExpr);

    #endregion
  }
}