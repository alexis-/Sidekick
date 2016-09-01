﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilterSchemeSerializerModifier.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using Catel.Runtime.Serialization;
using Sidekick.FilterBuilder.Expressions;
using Sidekick.FilterBuilder.Models.Interfaces;

namespace Sidekick.FilterBuilder.Runtime.Serialization
{
  public class PropertyExpressionSerializerModifier : SerializerModifierBase<PropertyExpression>
    {
        private const string Separator = "||";

        public override void SerializeMember(ISerializationContext context, MemberValue memberValue)
        {
            if (string.Equals(memberValue.Name, "Property"))
            {
                var propertyInfo = memberValue.Value as IPropertyMetadata;
                if (propertyInfo != null)
                {
                    memberValue.Value = string.Format("{0}{1}{2}", propertyInfo.OwnerType.FullName, Separator, propertyInfo.Name);
                }
            }
        }

        public override void DeserializeMember(ISerializationContext context, MemberValue memberValue)
        {
            if (string.Equals(memberValue.Name, "Property"))
            {
                var propertyName = memberValue.Value as string;
                if (propertyName != null)
                {
                    // We need to delay this
                    ((PropertyExpression)context.Model).PropertySerializationValue = propertyName;
                }
            }
        }
    }
}