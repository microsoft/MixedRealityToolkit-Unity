// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// A helper that uses reflection to find objects that implement base types of the
    /// Interactable types that populate the various state, theme, and event inspectors.
    /// </summary>
    public class InteractableTypeFinder
    {
        /// <summary>
        /// A convenience wrapper provided for editor code to turn a list of types into a form that 
        /// matches their existing structure.
        /// </summary>
        /// <remarks>
        /// This is primarily a crutch because of how the inspector code stores parallel arrays of
        /// objects, rather than just storing an array of objects (i.e. it stores three arrays
        /// of objects which happen to have matching indices, rather than storing a single array
        /// of objects which have state relevant within the object).
        /// </remarks>
        public static InteractableTypesContainer Find(List<Type> types, TypeRestriction typeRestriction)
        {
#if UNITY_EDITOR
            return new InteractableTypesContainer(FindTypes(types, typeRestriction));
#else
            // Due to other code structure, it's possible that this can still be invoked at runtime, but should
            // not return anything (because type information should be read from serialized assembly data, rather
            // than using reflection at runtime).
            return new InteractableTypesContainer(new List<InteractableType>());
#endif
        }

#if UNITY_EDITOR
        /// <summary>
        /// Used to cache lookups for Types (for example, InteractableThemeBase) to their classes that implement
        /// that type.
        /// </summary>
        private static Dictionary<Type, List<InteractableType>> cache = new Dictionary<Type, List<InteractableType>>();

        /// <summary>
        /// Gets the list of InteractableType objects for classes that support the specified types.
        /// </summary>
        private static List<InteractableType> FindTypes(List<Type> types, TypeRestriction typeRestriction)
        {
            EnsureCacheForTypes(types, typeRestriction);
            return GetTypesFromCache(types);
        }

        /// <summary>
        /// Gets the list of InteractableType objects for classes that support the specified types by
        /// looking directly in the cache.
        /// </summary>
        /// <remarks>
        /// Assumes it is called after EnsureCacheForTypes. Otherwise, this is dangerous to call.
        /// </remarks>
        private static List<InteractableType> GetTypesFromCache(List<Type> types)
        {
            List<InteractableType> interactableTypes = new List<InteractableType>();
            foreach (Type type in types)
            {
                interactableTypes.AddRange(cache[type]);
            }
            return interactableTypes;
        }

        /// <summary>
        /// Ensures a cache entry is setup for all types in the InteractableType enum.
        /// </summary>
        /// <remarks>
        /// Note that this is not invoked at runtime and is assumed to be invoked from a single
        /// threaded UI context, and is thus not locked.
        /// </remarks>
        private static void EnsureCacheForTypes(List<Type> types, TypeRestriction typeRestriction)
        {
            HashSet<Type> cacheMisses = new HashSet<Type>();
            foreach (Type type in types)
            {
                if (!cache.ContainsKey(type))
                {
                    cacheMisses.Add(type);
                }
            }

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Type type in cacheMisses)
            {
                cache[type] = GetTypesFromAssemblies(type, typeRestriction, assemblies);
            }
        }

        /// <summary>
        /// Loads the classes that derive from the given type by looking through all of the assemblies.
        /// </summary>
        private static List<InteractableType> GetTypesFromAssemblies(Type type, TypeRestriction typeRestriction, Assembly[] assemblies)
        {
            List<InteractableType> interactableTypes = new List<InteractableType>();

            if (typeRestriction == TypeRestriction.AllowBase)
            {
                InteractableType interactableType = new InteractableType(type);
                interactableTypes.Add(interactableType);
            }

            foreach (Assembly assembly in assemblies)
            {
                foreach (Type assemblyType in assembly.GetTypes())
                {
                    TypeInfo info = assemblyType.GetTypeInfo();   
                    if (info.IsSubclassOf(type))
                    {
                        InteractableType interactableType = new InteractableType(assemblyType);
                        interactableTypes.Add(interactableType);
                    }
                }
            }
            return interactableTypes;
        }
#endif
    }
}
