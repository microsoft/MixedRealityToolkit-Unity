// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Microsoft.MixedReality.Toolkit
{
    public static class TypeExtensions
    {
#if !NETFX_CORE
        /// <summary>
        /// Returns a list of types for all classes that extend from the current type and are not abstract
        /// </summary>
        /// <param name="rootType">The class type from which to search for inherited classes</param>
        /// <param name="searchAssemblies">List of assemblies to search through for types. If null, default is to grab all assemblies in current app domain</param>
        /// <returns>Null if rootType is not a class, otherwise returns list of types for sub-classes of rootType</returns>
        public static List<Type> GetAllSubClassesOf(this Type rootType, Assembly[] searchAssemblies = null)
        {
            if (!rootType.IsClass) return null;

            if (searchAssemblies == null) { searchAssemblies = AppDomain.CurrentDomain.GetAssemblies(); }

            var results = new List<Type>();

            Parallel.ForEach(searchAssemblies, (assembly) =>
            {
                Parallel.ForEach(assembly.GetLoadableTypes(), (type) =>
                {
                    if (type != null && type.IsClass && !type.IsAbstract && type.IsSubclassOf(rootType))
                    {
                        results.Add(type);
                    }
                });
            });

            return results;
        }
#endif
    }
}