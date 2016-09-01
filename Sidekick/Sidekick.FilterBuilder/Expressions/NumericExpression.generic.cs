// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NumericExpression.generic.cs" company="WildGums">
//   Copyright (c) 2008 - 2016 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using System;

namespace Sidekick.FilterBuilder.Expressions
{
  public abstract class NumericExpression<TValue> : ValueDataTypeExpression<TValue>
        where TValue : struct, IComparable, IFormattable, IComparable<TValue>, IEquatable<TValue>
    {
        protected NumericExpression()
            : base()
        {

        }

        #region Properties
        public bool IsDecimal { get; protected set; }
        public bool IsSigned { get; protected set; }
        #endregion
    }
}
