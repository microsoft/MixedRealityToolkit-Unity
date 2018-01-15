// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace MixedRealityToolkit.Examples.Grabbables
{
    public struct ControllerReleaseData
    {
        public Vector3 Velocity;
        public Vector3 AngleVelocity;

        public ControllerReleaseData(Vector3 _vel, Vector3 _angVel)
        {
            Velocity = _vel;
            AngleVelocity = _angVel;
        }
    }
}
