using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.MixedReality.Toolkit.SDK.UX.Interactable.TypeResolution
{
    /// <summary>
    /// A helper that uses reflection to find objects that implement base types of the
    /// Interactable types that populate the various state, theme, and event inspectors.
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    public class InteractableTypeFinder
    {
        /// <summary>
        /// Controls the behavior of the InteractableTypeFinder.FindTypes function. See individual
        /// enum values for more details.
        /// </summary>
        public enum TypeRestriction
        {
            /// <summary>
            /// When this is specified, only classes derived from the specified type will be
            /// returned by the lookup. This means that if you pass InteractableStates, the
            /// lookup will only return classes whose base class is InteractableStates but
            /// will not return InteractableStates itself.
            /// </summary>
            DerivedOnly,

            /// <summary>
            /// When this is specified, classes derived from the specified type AND the class
            /// itself will be returned by the lookup. This means that if you pass 
            /// InteractableStates, the lookup will both classes whose base class is 
            /// InteractableStates and InteractableStates itself.
            /// </summary>
            AllowBase,
        };

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
            return new InteractableTypeList(new List<InteractableType>());
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
            List<Type> cacheMisses = new List<Type>();
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
            foreach (Assembly assembly in assemblies)
            {
                foreach (Type assemblyType in assembly.GetTypes())
                {
                    TypeInfo info = assemblyType.GetTypeInfo();
                    bool exactBaseMatch = typeRestriction == TypeRestriction.AllowBase && assemblyType.Equals(type);
                    bool derivedMatch = info.BaseType != null && info.BaseType.Equals(type);
                    if (exactBaseMatch || derivedMatch)
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
