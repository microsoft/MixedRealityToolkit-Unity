// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Defines a method of smoothing the components of a transform.
    /// </summary>
    public interface ITransformSmoothingLogic
    {
        /// <summary>
        /// Smooths from source to goal, provided lerptime and a deltaTime.
        /// </summary>
        Vector3 SmoothPosition(Vector3 source, Vector3 goal, float lerpTime, float deltaTime);

        /// <summary>
        /// Smooths from source to goal, provided slerptime and a deltaTime.
        /// </summary>
        Quaternion SmoothRotation(Quaternion source, Quaternion goal, float slerpTime, float deltaTime);

        /// <summary>
        /// Smooths from source to goal, provided lerptime and a deltaTime.
        /// </summary>
        Vector3 SmoothScale(Vector3 source, Vector3 goal, float lerpTime, float deltaTime);
    }
}