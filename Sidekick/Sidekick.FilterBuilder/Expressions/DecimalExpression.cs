// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DecimalExpression.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using System.Diagnostics;

namespace Sidekick.FilterBuilder.Expressions
{
  [DebuggerDisplay("{ValueControlType} {SelectedCondition} {Value}")]
    public class DecimalExpression : NumericExpression<decimal>
    {
        #region Constructors
        public DecimalExpression()
            : this(true)
        {
        }

        public DecimalExpression(bool isNullable)
        {
            IsDecimal = true;
            IsNullable = isNullable;
            IsSigned = true;
            ValueControlType = ValueControlType.Decimal;
        }
        #endregion
    }
}