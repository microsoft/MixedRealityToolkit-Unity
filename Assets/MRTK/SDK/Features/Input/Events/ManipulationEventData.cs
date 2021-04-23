// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Information associated with a particular manipulation event.
    /// </summary>
    public class ManipulationEventData
    {
        /// <summary>
        /// The object being manipulated
        /// </summary>
        public GameObject ManipulationSource { get; set; }

        /// <summary>
        /// The pointer manipulating the object or hovering over the object. Will be null for OnManipulationEnded.
        /// </summary>
        public IMixedRealityPointer Pointer { get; set; }

        /// <summary>
        /// Whether the Manipulation is a NearInteration or not.
        /// </summary>
        public bool IsNearInteraction { get; set; }

        /// <summary>
        /// Center of the <see cref="ObjectManipulator"/>'s Pointer in world space
        /// </summary>
        public Vector3 PointerCentroid { get; set; }

        /// <summary>
        /// Pointer's Velocity.
        /// </summary>
        public Vector3 PointerVelocity { get; set; }

        /// <summary>
        /// Pointer's Angular Velocity in Eulers.
        /// </summary>
        public Vector3 PointerAngularVelocity { get; set; }
    }
}
