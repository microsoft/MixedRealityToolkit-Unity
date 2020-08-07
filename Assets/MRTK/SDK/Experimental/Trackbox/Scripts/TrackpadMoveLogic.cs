// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Physics
{
    /// <summary>
    /// Implements a move logic that will move an object based on the initial position of 
    /// the grab point relative to the pointer and relative to the object, and subsequent
    /// changes to the pointer and the object's rotation
    /// 
    /// Usage:
    /// When a manipulation starts, call Setup.
    /// Call Update any time to update the move logic and get a new rotation for the object.
    /// </summary>
    internal class TrackpadMoveLogic
    {

        private MixedRealityPose originalObjectPose;
        private Vector3 originalGrabPosition;

        /// <summary>
        /// Setup function
        /// </summary>
        public void Setup(Vector3 grabCentroid, MixedRealityPose objectPose)
        {
            originalObjectPose = objectPose;
            originalGrabPosition = grabCentroid;
        }

        /// <summary>
        /// Update the position based on input.
        /// </summary>
        /// <returns>A Vector3 describing the desired position</returns>
        public Vector3 Update(Vector3 grabPoint, float manipulationScale)
        {
            return originalObjectPose.Position + (grabPoint - originalGrabPosition) * manipulationScale;
        }
    }
}
