// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Utility class to store subclasses of particular base class keys
    /// Reloads between play mode/edit mode and after re-compile of scripts
    /// </summary>
    public static class TypeCacheUtility
    {
        private static Dictionary<Type, List<Type>> cache = new Dictionary<Type, List<Type>>();

        /// <summary>
        /// Get all subclass types of base class type T
        /// </summary>
        /// <typeparam name="T">base class of type T</typeparam>
        /// <returns>list of subclass types for base class T</returns>
        public static List<Type> GetSubClasses<T>()
        {
            return GetSubClasses(typeof(T));
        }

        /// <summary>
        /// Get all subclass types of base class type parameter
        /// </summary>
        /// <param name="baseClassType">base class type</param>
        /// <returns>list of subclass types for base class type parameter</returns>
        public static List<Type> GetSubClasses(Type baseClassType)
        {
            if (baseClassType == null) { return null; }

            if (!cache.ContainsKey(baseClassType))
            {
                cache[baseClassType] = baseClassType.GetAllSubClassesOf();
            }

            return cache[baseClassType];
        }
    }
}