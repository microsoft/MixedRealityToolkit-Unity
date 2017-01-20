// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// A base class for a stabilizer that takes an input position and orientation, and performs operations on them
    /// to stabilize, or smooth deltas, in the data. 
    /// </summary>
    public abstract class BaseRayStabilizer : MonoBehaviour
    {
        /// <summary>
        /// The stabilized position.
        /// </summary>
        public abstract Vector3 StablePosition { get; }

        /// <summary>
        /// The stabilized rotation.
        /// </summary>
        public abstract Quaternion StableRotation { get; }

        /// <summary>
        /// A ray representing the stable position and orientation
        /// </summary>
        public abstract Ray StableRay { get; }

        /// <summary>
        /// Call this each frame to smooth out changes to a position and orientation.
        /// </summary>
        /// <param name="position">Input position to smooth.</param>
        /// <param name="rotation">Input orientation to smooth.</param>
        public abstract void UpdateStability(Vector3 position, Quaternion rotation);
    }
}