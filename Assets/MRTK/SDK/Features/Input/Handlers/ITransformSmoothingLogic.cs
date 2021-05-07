// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using UnityEngine;
namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Base class for smoothing transforms
    /// </summary>
    public interface ITransformSmoothingLogic
    {
        Vector3 SmoothPosition(Vector3 source, Vector3 goal, float lerpTime, float deltaTime);

        Quaternion SmoothRotation(Quaternion source, Quaternion goal, float slerpTime, float deltaTime);

        Vector3 SmoothScale(Vector3 source, Vector3 goal, float lerpTime, float deltaTime);
    }
}