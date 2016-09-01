// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoubleExpression.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using System.Diagnostics;

namespace Sidekick.FilterBuilder.Expressions
{
  [DebuggerDisplay("{ValueControlType} {SelectedCondition} {Value}")]
    public class DoubleExpression : NumericExpression<double>
    {
        #region Constructors
        public DoubleExpression()
            : this(true)
        {
        }

        public DoubleExpression(bool isNullable)
        {
            IsDecimal = true;
            IsNullable = isNullable;
            IsSigned = true;
            ValueControlType = ValueControlType.Double;
        }
        #endregion
    }
}