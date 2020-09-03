// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Physics
{
    public interface IBaseRayStabilizer
    {
        Vector3 StablePosition { get; }
        Quaternion StableRotation { get; }
        Ray StableRay { get; }
        void UpdateStability(Vector3 position, Quaternion rotation);
        void UpdateStability(Vector3 position, Vector3 direction);
    }
}