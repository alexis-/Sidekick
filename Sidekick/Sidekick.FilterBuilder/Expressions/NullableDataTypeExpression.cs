// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataTypeExpression.cs" company="WildGums">
//   Copyright (c) 2008 - 2016 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Sidekick.FilterBuilder.Expressions
{
  public abstract class NullableDataTypeExpression : DataTypeExpression
    {
        protected NullableDataTypeExpression()
            : base()
        {

        }

        #region Properties
        public bool IsNullable { get; set; }
        #endregion
    }
}
