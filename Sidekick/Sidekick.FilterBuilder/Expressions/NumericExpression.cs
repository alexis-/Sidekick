// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntegerExpression.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using System;
using System.Diagnostics;
using Sidekick.FilterBuilder.Extensions;

namespace Sidekick.FilterBuilder.Expressions
{
  [DebuggerDisplay("{ValueControlType} {SelectedCondition} {Value}")]
    public class NumericExpression : NumericExpression<double>
    {
        public NumericExpression()
        {
            IsDecimal = true;
            IsSigned = true;
            ValueControlType = ValueControlType.Numeric;
        }

        public NumericExpression(Type type)
            : this()
        {
            IsNullable = type.IsNullable();
        }
    }
}