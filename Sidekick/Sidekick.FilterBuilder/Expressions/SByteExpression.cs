// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SByteExpression.cs" company="WildGums">
//   Copyright (c) 2008 - 2016 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using System.Diagnostics;

namespace Sidekick.FilterBuilder.Expressions
{
  [DebuggerDisplay("{ValueControlType} {SelectedCondition} {Value}")]
    public class SByteExpression : NumericExpression<sbyte>
    {
        #region Constructors
        public SByteExpression()
            : this(true)
        {
        }

        public SByteExpression(bool isNullable)
        {
            IsDecimal = false;
            IsNullable = isNullable;
            IsSigned = true;
            ValueControlType = ValueControlType.SByte;
        }
        #endregion
    }
}
