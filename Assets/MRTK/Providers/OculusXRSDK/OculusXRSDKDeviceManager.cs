// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Windows.Utilities;
using Microsoft.MixedReality.Toolkit.XRSDK.Input;
using System;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.XRSDK.WindowsMixedReality
{
    /// <summary>
    /// Manages XR SDK devices on the Oculus platform.
    /// </summary>
    [MixedRealityDataProvider(
        typeof(IMixedRealityInputSystem),
        SupportedPlatforms.Android,
        "XRSDK Oculus Quest Device Manager")]
    public class OculusXRSDKDeviceManager : XRSDKDeviceManager
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="inputSystem">The <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem"/> instance that receives data from this provider.</param>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        public OculusXRSDKDeviceManager(
            IMixedRealityInputSystem inputSystem,
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null) : base(inputSystem, name, priority, profile) { }

        #region Controller Utilities

        /// <inheritdoc />
        protected override Type GetControllerType(SupportedControllerType supportedControllerType)
        {
            UnityEngine.Debug.Log("getting a controller type of type " + supportedControllerType);
            switch (supportedControllerType)
            {
                case SupportedControllerType.OculusTouch:
                    return typeof(OculusXRSDKTouchController);
                case SupportedControllerType.OculusRemote:
                    return typeof(OculusXRSDKRemoteController);
                default:
                    return base.GetControllerType(supportedControllerType);
            }
        }

        /// <inheritdoc />
        protected override InputSourceType GetInputSourceType(SupportedControllerType supportedControllerType)
        {
            switch (supportedControllerType)
            {
                case SupportedControllerType.OculusTouch:
                case SupportedControllerType.OculusRemote:
                    return InputSourceType.Controller;
                default:
                    return base.GetInputSourceType(supportedControllerType);
            }
        }

        /// <inheritdoc />
        protected override SupportedControllerType GetCurrentControllerType(InputDevice inputDevice)
        {
            //if (inputDevice.characteristics.HasFlag(InputDeviceCharacteristics.HandTracking))
            //{
            //    if (inputDevice.characteristics.HasFlag(InputDeviceCharacteristics.Left) ||
            //        inputDevice.characteristics.HasFlag(InputDeviceCharacteristics.Right))
            //    {
            //        // If it's a hand with a reported handedness, assume HL2 articulated hand
            //        return SupportedControllerType.ArticulatedHand;
            //    }
            //    else
            //    {
            //        // Otherwise, assume HL1 hand
            //        return SupportedControllerType.GGVHand;
            //    }
            //}

            if (inputDevice.characteristics.HasFlag(InputDeviceCharacteristics.Controller))
            {
                return SupportedControllerType.OculusTouch;
            }

            return base.GetCurrentControllerType(inputDevice);
        }

        #endregion Controller Utilities
    }
}

