// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConditionGroupType.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using Catel.ComponentModel;

namespace Sidekick.FilterBuilder.Conditions
{
    public enum ConditionGroupType
    {
        [DisplayName("FilterBuilder_And")]
        And,
        [DisplayName("FilterBuilder_Or")]
        Or
    }
}