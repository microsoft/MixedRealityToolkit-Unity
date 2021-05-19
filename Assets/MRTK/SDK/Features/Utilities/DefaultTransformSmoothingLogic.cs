// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// A default implementation of ITransformSmoothingLogic for smoothing transforms.
    /// </summary>
    public class DefaultTransformSmoothingLogic : ITransformSmoothingLogic
    {
        /// <inheritdoc />
        public Vector3 SmoothPosition(Vector3 source, Vector3 goal, float lerpTime, float deltaTime)
        {
            return Smoothing.SmoothTo(source, goal, lerpTime, deltaTime);
        }

        /// <inheritdoc />
        public Quaternion SmoothRotation(Quaternion source, Quaternion goal, float slerpTime, float deltaTime)
        {
            return Smoothing.SmoothTo(source, goal, slerpTime, deltaTime);
        }

        /// <inheritdoc />
        public Vector3 SmoothScale(Vector3 source, Vector3 goal, float lerpTime, float deltaTime)
        {
            return Smoothing.SmoothTo(source, goal, lerpTime, deltaTime);
        }
    }
}
