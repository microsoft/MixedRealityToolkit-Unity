// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Information associated with a particular manipulation event.
    /// </summary>
    public class ManipulationEventData
    {
        /// <summary>
        /// Source of ManipulationEvent.
        /// </summary>
        public ManipulationHandler ManipulationSource { get; set; }

        /// <summary>
        /// Whether the Manipulation is a NearInteration or not.
        /// </summary>
        public bool IsNearInteraction {get; set; }

        /// <summary>
        /// Center of the <see cref="ManipulationHandler"/>'s Pointer in world space
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
