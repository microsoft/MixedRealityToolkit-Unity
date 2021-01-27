// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Windows.Utilities;
using Microsoft.MixedReality.Toolkit.XRSDK.Input;
using System;
using UnityEngine.XR;

#if (UNITY_WSA && DOTNETWINRT_PRESENT) || WINDOWS_UWP
using Microsoft.MixedReality.Toolkit.WindowsMixedReality;
#endif // (UNITY_WSA && DOTNETWINRT_PRESENT) || WINDOWS_UWP

#if WINDOWS_UWP
using Windows.Perception;
using Windows.Perception.People;
using Windows.UI.Input.Spatial;
#elif UNITY_WSA && DOTNETWINRT_PRESENT
using Microsoft.Windows.Perception;
using Microsoft.Windows.Perception.People;
using Microsoft.Windows.UI.Input.Spatial;
#endif

namespace Microsoft.MixedReality.Toolkit.XRSDK.WindowsMixedReality
{
    /// <summary>
    /// Manages XR SDK devices on the Windows Mixed Reality platform.
    /// </summary>
    [MixedRealityDataProvider(
        typeof(IMixedRealityInputSystem),
        SupportedPlatforms.WindowsStandalone | SupportedPlatforms.WindowsUniversal,
        "XR SDK Windows Mixed Reality Device Manager")]
    public class WindowsMixedRealityDeviceManager : XRSDKDeviceManager
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="inputSystem">The <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem"/> instance that receives data from this provider.</param>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        public WindowsMixedRealityDeviceManager(
            IMixedRealityInputSystem inputSystem,
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null) : base(inputSystem, name, priority, profile) { }

        #region IMixedRealityDeviceManager Interface

#if (UNITY_WSA && DOTNETWINRT_PRESENT) || WINDOWS_UWP
        private IMixedRealityGazeProviderHeadOverride mixedRealityGazeProviderHeadOverride = null;

        /// <inheritdoc />
        public override void Enable()
        {
            base.Enable();

            if (WindowsMixedRealityUtilities.UtilitiesProvider == null)
            {
                WindowsMixedRealityUtilities.UtilitiesProvider = new XRSDKWindowsMixedRealityUtilitiesProvider();
            }

            mixedRealityGazeProviderHeadOverride = Service?.GazeProvider as IMixedRealityGazeProviderHeadOverride;
        }

        /// <inheritdoc />
        public override void Update()
        {
            // Override gaze before base.Update() updates the controllers
            if (mixedRealityGazeProviderHeadOverride != null && mixedRealityGazeProviderHeadOverride.UseHeadGazeOverride && WindowsMixedRealityUtilities.SpatialCoordinateSystem != null)
            {
                SpatialPointerPose pointerPose = SpatialPointerPose.TryGetAtTimestamp(WindowsMixedRealityUtilities.SpatialCoordinateSystem, PerceptionTimestampHelper.FromHistoricalTargetTime(DateTimeOffset.Now));
                if (pointerPose != null)
                {
                    HeadPose head = pointerPose.Head;
                    if (head != null)
                    {
                        mixedRealityGazeProviderHeadOverride.OverrideHeadGaze(head.Position.ToUnityVector3(), head.ForwardDirection.ToUnityVector3());
                    }
                }
            }

            base.Update();
        }
#endif // (UNITY_WSA && DOTNETWINRT_PRESENT) || WINDOWS_UWP

        #endregion IMixedRealityDeviceManager Interface

        #region IMixedRealityCapabilityCheck Implementation

        /// <inheritdoc />
        public override bool CheckCapability(MixedRealityCapability capability)
        {
            if (WindowsApiChecker.IsMethodAvailable(
                "Windows.UI.Input.Spatial",
                "SpatialInteractionManager",
                "IsSourceKindSupported"))
            {
#if WINDOWS_UWP
                switch (capability)
                {
                    case MixedRealityCapability.ArticulatedHand:
                    case MixedRealityCapability.GGVHand:
                        return SpatialInteractionManager.IsSourceKindSupported(SpatialInteractionSourceKind.Hand);

                    case MixedRealityCapability.MotionController:
                        return SpatialInteractionManager.IsSourceKindSupported(SpatialInteractionSourceKind.Controller);
                }
#endif // WINDOWS_UWP
            }
            else // Pre-Windows 10 1903.
            {
                if (XRSubsystemHelpers.DisplaySubsystem != null && !XRSubsystemHelpers.DisplaySubsystem.displayOpaque)
                {
                    // HoloLens supports GGV hands
                    return capability == MixedRealityCapability.GGVHand;
                }
                else
                {
                    // Windows Mixed Reality immersive devices support motion controllers
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
                case SupportedControllerType.GGVHand:
                    return typeof(WindowsMixedRealityXRSDKGGVHand);
                default:
                    return base.GetControllerType(supportedControllerType);
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
                case SupportedControllerType.GGVHand:
                    return InputSourceType.Hand;
                default:
                    return base.GetInputSourceType(supportedControllerType);
            }
        }

        /// <inheritdoc />
        protected override SupportedControllerType GetCurrentControllerType(InputDevice inputDevice)
        {
            if (inputDevice.characteristics.HasFlag(InputDeviceCharacteristics.HandTracking))
            {
                if (inputDevice.characteristics.HasFlag(InputDeviceCharacteristics.Left) ||
                    inputDevice.characteristics.HasFlag(InputDeviceCharacteristics.Right))
                {
                    // If it's a hand with a reported handedness, assume HL2 articulated hand
                    return SupportedControllerType.ArticulatedHand;
                }
                else
                {
                    // Otherwise, assume HL1 hand
                    return SupportedControllerType.GGVHand;
                }
            }

            if (inputDevice.characteristics.HasFlag(InputDeviceCharacteristics.Controller))
            {
                return SupportedControllerType.WindowsMixedReality;
            }

            return base.GetCurrentControllerType(inputDevice);
        }

        #endregion Controller Utilities
    }
}

