// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FloatExpression.cs" company="WildGums">
//   Copyright (c) 2008 - 2016 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using System.Diagnostics;

namespace Sidekick.FilterBuilder.Expressions
{
  [DebuggerDisplay("{ValueControlType} {SelectedCondition} {Value}")]
    public class FloatExpression : NumericExpression<float>
    {
        #region Constructors
        public FloatExpression()
            : this(true)
        {
        }

        public FloatExpression(bool isNullable)
        {
            IsDecimal = true;
            IsNullable = isNullable;
            IsSigned = true;
            ValueControlType = ValueControlType.Float;
        }
        #endregion
    }
}
