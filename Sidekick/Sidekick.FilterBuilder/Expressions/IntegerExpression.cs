// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntegerExpression.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using System.Diagnostics;

namespace Sidekick.FilterBuilder.Expressions
{
  [DebuggerDisplay("{ValueControlType} {SelectedCondition} {Value}")]
    public class IntegerExpression : NumericExpression<int>
    {
        #region Constructors
        public IntegerExpression()
            : this(true)
        {
        }

        public IntegerExpression(bool isNullable)
        {
            IsDecimal = false;
            IsNullable = isNullable;
            IsSigned = true;
            ValueControlType = ValueControlType.Integer;
        }
        #endregion
    }
}