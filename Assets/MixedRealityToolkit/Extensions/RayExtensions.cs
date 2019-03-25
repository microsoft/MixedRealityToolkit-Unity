// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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
