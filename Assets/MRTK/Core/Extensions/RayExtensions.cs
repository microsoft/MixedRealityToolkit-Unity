// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Extension methods for Unity's Ray struct
    /// </summary>
    public static class RayExtensions
    {
        /// <summary>
        /// Determines whether or not a ray is valid.
        /// </summary>
        /// <param name="ray">The ray being tested.</param>
        /// <returns>True if the ray is valid, false otherwise.</returns>
        public static bool IsValid(this Ray ray)
        {
            return ray.direction != Vector3.zero;
        }
    }
}
