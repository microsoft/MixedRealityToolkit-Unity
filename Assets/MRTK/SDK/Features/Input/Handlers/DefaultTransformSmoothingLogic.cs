// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// A default implementation of TransformSmoother for smoothing transforms.
    /// </summary>
    public class DefaultTransformSmoothingLogic : TransformSmoothingLogic
    {
        public override Vector3 SmoothPosition(Vector3 source, Vector3 goal, float lerpTime, float deltaTime)
        {
            return Smoothing.SmoothTo(source, goal, lerpTime, deltaTime);
        }
        
        public override Quaternion SmoothRotation(Quaternion source, Quaternion goal, float slerpTime, float deltaTime)
        {
            return Smoothing.SmoothTo(source, goal, slerpTime, deltaTime);
        }
        
        public override Vector3 SmoothScale(Vector3 source, Vector3 goal, float lerpTime, float deltaTime)
        {
            return Smoothing.SmoothTo(source, goal, lerpTime, deltaTime);
        }
    }
}
