// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilterCustomizationService.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using System.Collections.Generic;
using Catel;
using Sidekick.FilterBuilder.Helpers;
using Sidekick.FilterBuilder.Models.Interfaces;
using Sidekick.FilterBuilder.Services.Interfaces;

namespace Sidekick.FilterBuilder.Services
{
  public class FilterCustomizationService : IFilterCustomizationService
    {
        public virtual void CustomizeInstanceProperties(IPropertyCollection instanceProperties)
        {
            Argument.IsNotNull(() => instanceProperties);

            var catelProperties = new HashSet<string> {
                "BusinessRuleErrorCount",
                "BusinessRuleWarningCount",
                "FieldErrorCount",
                "FieldWarningCount",
                "HasErrors",
                "HasWarnings",
                "IsDirty",
                "IsEditable",
                "IsInEditSession",
                "IsReadOnly"
            };

            // Remove Catel properties
            instanceProperties.Properties.RemoveAll(x => catelProperties.Contains(x.Name));

            // Remove unsupported type properties
            instanceProperties.Properties.RemoveAll(x => !x.IsSupportedType());
        }
    }
}