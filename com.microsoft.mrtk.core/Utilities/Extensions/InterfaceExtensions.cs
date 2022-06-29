// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Extension methods for working with interfaces.
    /// </summary>
    public static class InterfaceExtensions
    {
        /// <summary>
        /// Properly checks an interface for null and returns the MonoBehaviour implementing it.
        /// </summary>
        /// <returns>
        /// True if the implementer of the interface is a MonoBehaviour and the MonoBehaviour is not null.
        /// </returns>
        public static bool TryGetMonoBehaviour<T>(this T @interface, out MonoBehaviour monoBehaviour) where T :
            class => (monoBehaviour = @interface as MonoBehaviour) != null;
    }
}
