// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateTimeExpression.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using System;
using System.Diagnostics;

namespace Sidekick.FilterBuilder.Expressions
{
  [DebuggerDisplay("{ValueControlType} {SelectedCondition} {Value}")]
    public class DateTimeExpression : ValueDataTypeExpression<DateTime>
    {
        #region Constructors
        public DateTimeExpression()
            : this(true)
        {
        }

        public DateTimeExpression(bool isNullable)
        {
            IsNullable = isNullable;
            Value = DateTime.Now;
            ValueControlType = ValueControlType.DateTime;
        }
        #endregion
    }
}