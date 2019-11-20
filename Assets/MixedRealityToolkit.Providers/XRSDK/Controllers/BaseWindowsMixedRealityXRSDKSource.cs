// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.XR;

#if WMR_ENABLED
using Unity.XR.WindowsMR;
#endif // WMR_ENABLED

namespace Microsoft.MixedReality.Toolkit.XRSDK.Input
{
    /// <summary>
    /// A Windows Mixed Reality Source instance.
    /// </summary>
    public abstract class BaseWindowsMixedRealityXRSDKSource : GenericXRSDKController
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        protected BaseWindowsMixedRealityXRSDKSource(TrackingState trackingState, Handedness sourceHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
                : base(trackingState, sourceHandedness, inputSource, interactions)
        {
        }


        /// <summary>
        /// Update spatial pointer and spatial grip data.
        /// </summary>
        protected override void UpdatePoseData(MixedRealityInteractionMapping interactionMapping, InputDevice inputDevice)
        {
            Debug.Assert(interactionMapping.AxisType == AxisType.SixDof);

            InputFeatureUsage<Vector3> positionUsage;
            InputFeatureUsage<Quaternion> rotationUsage;

            // Update the interaction data source
            switch (interactionMapping.InputType)
            {
                case DeviceInputType.SpatialPointer:
#if WMR_ENABLED
                    positionUsage = WindowsMRUsages.PointerPosition;
                    rotationUsage = WindowsMRUsages.PointerRotation;
                    break;
#else
                    return;
#endif // WMR_ENABLED
                case DeviceInputType.SpatialGrip:
                    positionUsage = CommonUsages.devicePosition;
                    rotationUsage = CommonUsages.deviceRotation;
                    break;
                default:
                    return;
            }

            if (inputDevice.TryGetFeatureValue(positionUsage, out Vector3 position))
            {
                CurrentControllerPose.Position = position;
            }

            if (inputDevice.TryGetFeatureValue(rotationUsage, out Quaternion rotation))
            {
                CurrentControllerPose.Rotation = rotation;
            }

            interactionMapping.PoseData = CurrentControllerPose;

            // If our value changed raise it.
            if (interactionMapping.Changed)
            {
                // Raise input system event if it's enabled
                CoreServices.InputSystem?.RaisePoseInputChanged(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, interactionMapping.PoseData);
            }
        }
    }
}
