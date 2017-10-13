using UnityEngine;

namespace HoloToolkit.Unity.InputModule
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
        public bool TryGetFunctionsReturnTrue;
        public bool IsPositionAvailable;
        public bool IsRotationAvailable;

        public Vector3 Position;
        public Vector3 Velocity;
        public Quaternion Rotation;
        public Ray? PointerRay;

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
            if (!TryGetFunctionsReturnTrue)
            {
                return false;
            }
            return true;
        }

        public bool TryGetVelocity(out Vector3 velocity)
        {
            velocity = Velocity;
            if (!TryGetFunctionsReturnTrue)
            {
                return false;
            }
            return true;
        }

        public bool TryGetRotation(out Quaternion rotation)
        {
            rotation = Rotation;
            if (!TryGetFunctionsReturnTrue || !IsRotationAvailable)
            {
                return false;
            }
            return true;
        }

        public bool TryGetPointerRay(out Ray pointerRay)
        {
            pointerRay = (Ray)PointerRay;
            if (PointerRay == null)
            {
                return false;
            }
            return true;
        }
    }
}