// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.XRSDK.Input;
using System;
using UnityEngine;
using UnityEngine.XR;
using Microsoft.MixedReality.Toolkit.Windows.Utilities;

#if (UNITY_WSA && DOTNETWINRT_PRESENT) || WINDOWS_UWP
using Microsoft.MixedReality.Toolkit.WindowsMixedReality;
#endif // (UNITY_WSA && DOTNETWINRT_PRESENT) || WINDOWS_UWP

#if WINDOWS_UWP
using WindowsInputSpatial = Windows.UI.Input.Spatial;
#endif // WINDOWS_UWP

namespace Microsoft.MixedReality.Toolkit.XRSDK.WindowsMixedReality
{
    /// <summary>
    /// Manages XR SDK devices on the Windows Mixed Reality platform.
    /// </summary>
    [MixedRealityDataProvider(
        typeof(IMixedRealityInputSystem),
        SupportedPlatforms.WindowsStandalone | SupportedPlatforms.WindowsUniversal,
        "XRSDK Windows Mixed Reality Device Manager")]
    public class WindowsMixedRealityDataProvider : XRSDKDeviceManager
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="inputSystem">The <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem"/> instance that receives data from this provider.</param>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        public WindowsMixedRealityDataProvider(
            IMixedRealityInputSystem inputSystem,
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null) : base(inputSystem, name, priority, profile) { }

#if (UNITY_WSA && DOTNETWINRT_PRESENT) || WINDOWS_UWP
        /// <inheritdoc />
        public override void Enable()
        {
            base.Enable();

            if (WindowsMixedRealityUtilities.UtilitiesProvider == null)
            {
                WindowsMixedRealityUtilities.UtilitiesProvider = new XRSDKWindowsMixedRealityUtilitiesProvider();
            }
    }
#endif // (UNITY_WSA && DOTNETWINRT_PRESENT) || WINDOWS_UWP

        #region IMixedRealityCapabilityCheck Implementation

        /// <inheritdoc />
        public override bool CheckCapability(MixedRealityCapability capability)
        {
            if (WindowsApiChecker.UniversalApiContractV8_IsAvailable) // Windows 10 1903 or later
            {
#if WINDOWS_UWP
                switch (capability)
                {
                    case MixedRealityCapability.ArticulatedHand:
                    case MixedRealityCapability.GGVHand:
                        return WindowsInputSpatial.SpatialInteractionManager.IsSourceKindSupported(WindowsInputSpatial.SpatialInteractionSourceKind.Hand);

                    case MixedRealityCapability.MotionController:
                        return WindowsInputSpatial.SpatialInteractionManager.IsSourceKindSupported(WindowsInputSpatial.SpatialInteractionSourceKind.Controller);
                }
#endif // WINDOWS_UWP
            }
            else // Pre-Windows 10 1903.
            {
                if (XRSDKSubsystemHelpers.DisplaySubsystem != null && !XRSDKSubsystemHelpers.DisplaySubsystem.displayOpaque)
                {
                    // HoloLens supports GGV hands
                    return capability == MixedRealityCapability.GGVHand;
                }
                else
                {
                    // Windows Mixed Reality Immersive devices support motion controllers
                    return capability == MixedRealityCapability.MotionController;
                }
            }

            return false;
        }

        #endregion IMixedRealityCapabilityCheck Implementation

        #region Controller Utilities

        /// <inheritdoc />
        protected override Type GetControllerType(SupportedControllerType supportedControllerType)
        {
            switch (supportedControllerType)
            {
                case SupportedControllerType.WindowsMixedReality:
                    return typeof(WindowsMixedRealityXRSDKMotionController);
                case SupportedControllerType.ArticulatedHand:
                    return typeof(WindowsMixedRealityXRSDKArticulatedHand);
                default:
                    return null;
            }
        }

        /// <inheritdoc />
        protected override InputSourceType GetInputSourceType(SupportedControllerType supportedControllerType)
        {
            switch (supportedControllerType)
            {
                case SupportedControllerType.WindowsMixedReality:
                    return InputSourceType.Controller;
                case SupportedControllerType.ArticulatedHand:
                    return InputSourceType.Hand;
                default:
                    return InputSourceType.Other;
            }
        }

        /// <inheritdoc />
        protected override SupportedControllerType GetCurrentControllerType(InputDevice inputDevice)
        {
            Debug.Log(inputDevice.name);

            if (inputDevice.characteristics.HasFlag(InputDeviceCharacteristics.HandTracking))
            {
                return SupportedControllerType.ArticulatedHand;
            }

            if (inputDevice.characteristics.HasFlag(InputDeviceCharacteristics.Controller))
            {
                return SupportedControllerType.WindowsMixedReality;
            }

            Debug.Log($"{inputDevice.name} does not have a defined controller type, falling back to generic controller type");

            return SupportedControllerType.GenericUnity;
        }

        #endregion Controller Utilities
    }
}

