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
        /// Properly checks an interface for <see langword="null"/> and returns the <see cref="MonoBehaviour"/> implementing it.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the implementer of the interface is a <see cref="MonoBehaviour"/> and the <see cref="MonoBehaviour"/> is not <see langword="null"/>.
        /// </returns>
        public static bool TryGetMonoBehaviour<T>(this T @interface, out MonoBehaviour monoBehaviour) where T :
            class => (monoBehaviour = @interface as MonoBehaviour) != null;
    }
}
