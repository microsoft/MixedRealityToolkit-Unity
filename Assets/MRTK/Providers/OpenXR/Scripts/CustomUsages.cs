// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.XRSDK.OpenXR
{
    public static class CustomUsages
    {
        /// <summary>
        /// Represents the origin of a user's tracked eye gaze ray.
        /// Use with <see cref="GazeRotation"/> to build a gaze ray.
        /// </summary>
        public static readonly InputFeatureUsage<Vector3> GazePosition = new InputFeatureUsage<Vector3>("GazePosition");

        /// <summary>
        /// Represents the orientation a user's tracked eye gaze ray.
        /// Use with <see cref="GazePosition"/> to build a gaze ray, where the ray direction is this rotation multiplied by Vector3.forward.
        /// </summary>
        public static readonly InputFeatureUsage<Quaternion> GazeRotation = new InputFeatureUsage<Quaternion>("GazeRotation");
    }
}
