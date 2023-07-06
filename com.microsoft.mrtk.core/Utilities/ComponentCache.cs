// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{

    /// <summary>
    /// A static helper class which caches instances of a component of type T.
    /// This is called in place of calling FindFirstObjectByType and FindObjectsByType to improve performance.
    /// </summary>
    public static class ComponentCache<T> where T : Component
    {
        private static T cacheFirstInstance = null;
        private static T[] cacheList = null;

        /// <summary>
        /// Finds the first instance of an object of the given type. 
        /// </summary>
        /// <param name="includeInactive">Specifies whether the search should include inactive objects.</param>
        /// <returns>
        /// The first instance of an object of the given type. 
        /// </returns>
        public static T FindFirstInstance(FindObjectsInactive includeInactive = FindObjectsInactive.Exclude)
        {
            _ = TryFindFirstInstance(out T result, includeInactive);
            return result;
        }

        /// <summary>
        /// Finds the first instance of an object of the given type. 
        /// </summary>
        /// <param name="includeInactive">Specifies whether the search should include inactive objects.</param>
        /// <param name="result">A reference of the given type, which will be set to the first instance of an object if one is found.</param>
        /// <returns>
        /// True if an instance was found, and false otherwise. 
        /// </returns>
        public static bool TryFindFirstInstance(out T result, FindObjectsInactive includeInactive)
        {
            if (cacheFirstInstance == null)
            {
                cacheFirstInstance = Object.FindFirstObjectByType<T>(includeInactive);
            }

            result = cacheFirstInstance;
            return result != null;
        }

        /// <summary>
        /// Finds all instances of a given type. 
        /// </summary>
        /// <param name="includeInactive">Specifies whether the search should include inactive objects. Defaults to excluding inactive objects.</param>
        /// <returns>
        /// All instances of a given type. 
        /// </returns>
        public static T[] FindInstanceList(FindObjectsInactive includeInactive = FindObjectsInactive.Exclude)
        {
            _ = TryFindInstanceList(out T[] result, includeInactive);
            return result;
        }

        /// <summary>
        /// Finds all instances of a given type. 
        /// </summary>
        /// <param name="includeInactive">Specifies whether the search should include inactive objects.</param>
        /// <param name="result">A reference to a list of the given type, which will be set to all instances that are found.</param>
        /// <returns>
        /// True if any instance was found, and false otherwise. 
        /// </returns>
        public static bool TryFindInstanceList(out T[] result, FindObjectsInactive includeInactive)
        {
            if (cacheList == null)
            {
                cacheList = Object.FindObjectsByType<T>(includeInactive, FindObjectsSortMode.InstanceID);
            }

            result = cacheList;
            return result != null;
        }
    }
}
