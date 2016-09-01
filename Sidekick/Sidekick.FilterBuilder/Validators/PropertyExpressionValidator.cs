// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyExpressionValidator.cs" company="WildGums">
//   Copyright (c) 2008 - 2016 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using System.Collections.Generic;
using Catel.Data;
using Sidekick.FilterBuilder.Expressions;

namespace Sidekick.FilterBuilder.Validators
{
  public class PropertyExpressionValidator : ValidatorBase<PropertyExpression>
    {
        protected override void ValidateFields(PropertyExpression instance, List<IFieldValidationResult> validationResults)
        {
            if (instance.Property == null)
            {
                validationResults.Add(FieldValidationResult.CreateError("Property", "Property can not be null"));
            }
        }
    }
}