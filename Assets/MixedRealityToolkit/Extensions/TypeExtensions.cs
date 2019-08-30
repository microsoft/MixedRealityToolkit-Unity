// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.MixedReality.Toolkit
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Returns a list of types for all classes that extend from the current type and are not abstract
        /// </summary>
        /// <param name="rootType">The class type from which to search for inherited classes</param>
        /// <returns>Null if rootType is not a class, otherwise returns list of types for sub-classes of rootType</returns>
        public static List<Type> GetAllSubClassesOf(this Type rootType)
        {
            if (!rootType.IsClass) return null;

            var results = new List<Type>();
            foreach (var type in Assembly.GetAssembly(rootType).GetTypes())
            {
                if (type.IsClass && !type.IsAbstract && type.IsSubclassOf(rootType))
                {
                    results.Add(type);
                }
            }
            return results;
        }
    }
}