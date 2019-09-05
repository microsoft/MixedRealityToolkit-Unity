// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public static List<Type> GetAllSubClassesOf(this Type rootType, Assembly[] assemblies = null)
        {
            if (!rootType.IsClass) return null;

            if (assemblies == null) { assemblies = AppDomain.CurrentDomain.GetAssemblies(); }

            var results = new List<Type>();
            Stopwatch st = new Stopwatch();
            st.Start();
            //Parallel.ForEach(
            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsClass && !type.IsAbstract && type.IsSubclassOf(rootType))
                    {
                        results.Add(type);
                    }
                }
            }
            st.Stop();
            UnityEngine.Debug.Log("Time:" + st.ElapsedMilliseconds);
            return results;
        }
    }
}