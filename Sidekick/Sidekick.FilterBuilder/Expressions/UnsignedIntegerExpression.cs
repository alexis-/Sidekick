// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnsignedIntegerExpression.cs" company="WildGums">
//   Copyright (c) 2008 - 2016 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using System.Diagnostics;

namespace Sidekick.FilterBuilder.Expressions
{
  [DebuggerDisplay("{ValueControlType} {SelectedCondition} {Value}")]
    public class UnsignedIntegerExpression : NumericExpression<uint>
    {
        #region Constructors
        public UnsignedIntegerExpression()
            : this(true)
        {
        }

        public UnsignedIntegerExpression(bool isNullable)
        {
            IsDecimal = false;
            IsNullable = isNullable;
            IsSigned = false;
            ValueControlType = ValueControlType.UnsignedInteger;
        }
        #endregion
    }
}
