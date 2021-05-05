// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Base class for smoothing transforms
    /// </summary>
    public abstract class TransformSmoothingLogic
    {
        public abstract Vector3 SmoothPosition(Vector3 source, Vector3 goal, float lerpTime, float deltaTime);
        public abstract Quaternion SmoothRotation(Quaternion source, Quaternion goal, float slerpTime, float deltaTime);
        public abstract Vector3 SmoothScale(Vector3 source, Vector3 goal, float lerpTime, float deltaTime);
    }
}
