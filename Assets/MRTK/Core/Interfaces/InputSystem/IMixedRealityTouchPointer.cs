// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Interface for handling touch pointers.
    /// </summary>
    public interface IMixedRealityTouchPointer : IMixedRealityPointer
    {
        /// <summary>
        /// Current finger id of the touch.
        /// </summary>
        int FingerId { get; set; }

        /// <summary>
        /// Current touch ray.
        /// </summary>
        Ray TouchRay { get; set; }
    }
}