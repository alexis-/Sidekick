﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CollectionHelper.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using System;
using System.Collections;
using Catel;
using Catel.Logging;

namespace Sidekick.FilterBuilder.Helpers
{
  public static class CollectionHelper
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public static Type GetTargetType(IEnumerable collection)
        {
            Argument.IsNotNull(() => collection);

            //var collectionType = collection.GetType();
            //if (collectionType.IsGenericTypeEx())
            //{
            //    var genericArguments = collectionType.GetGenericArgumentsEx();
            //    return genericArguments[0];
            //}

            var enumerator = collection.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                Log.Warning("Collection does not contain items, cannot get any type information");
                return null;
            }

            var firstElement = enumerator.Current;
            return firstElement.GetType();
        }
    }
}