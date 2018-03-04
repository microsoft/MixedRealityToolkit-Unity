// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace MixedRealityToolkit.InputModule.Utilities.Interactions
{
    /// <summary>
    /// Since the InteractionSourcePose is internal to UnityEngine.VR.WSA.Input,
    /// this is a fake InteractionSourcePose structure to keep the test code consistent.
    /// </summary>
    public class DebugInteractionSourcePose
    {
        /// <summary>
        /// In the typical InteractionSourcePose, the hardware determines if
        /// TryGetPosition and TryGetVelocity will return true or not. Here
        /// we manually emulate this state with TryGetFunctionsReturnTrue.
        /// </summary>
        public bool TryGetFunctionsReturnTrue { get; set; }
        public bool IsPositionAvailable { get; set; }
        public bool IsRotationAvailable { get; set; }

        public Vector3 Position { get; set; }
        public Vector3 Velocity { get; set; }
        public Quaternion Rotation { get; set; }
        public Ray? PointerRay { get; set; }

        public DebugInteractionSourcePose()
        {
            TryGetFunctionsReturnTrue = false;
            IsPositionAvailable = false;
            IsRotationAvailable = false;
            Position = new Vector3(0, 0, 0);
            Velocity = new Vector3(0, 0, 0);
            Rotation = Quaternion.identity;
        }

        public bool TryGetPosition(out Vector3 position)
        {
            position = Position;
            return TryGetFunctionsReturnTrue;
        }

        public bool TryGetVelocity(out Vector3 velocity)
        {
            velocity = Velocity;
            return TryGetFunctionsReturnTrue;
        }

        public bool TryGetRotation(out Quaternion rotation)
        {
            rotation = Rotation;
            return TryGetFunctionsReturnTrue && IsRotationAvailable;
        }

        public bool TryGetPointerRay(out Ray pointerRay)
        {
            pointerRay = default(Ray);
            if (PointerRay == null)
            {
                return false;
            }

            pointerRay = (Ray)PointerRay;
            return true;
        }
    }
}