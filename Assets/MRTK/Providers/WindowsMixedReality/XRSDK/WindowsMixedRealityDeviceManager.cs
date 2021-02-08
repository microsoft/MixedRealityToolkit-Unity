// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Windows.Utilities;
using Microsoft.MixedReality.Toolkit.XRSDK.Input;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Unity.Profiling;
using Microsoft.MixedReality.Toolkit.WindowsMixedReality;

#if HP_CONTROLLER_ENABLED
using Microsoft.MixedReality.Input;
using MotionControllerHandedness = Microsoft.MixedReality.Input.Handedness;
using Handedness = Microsoft.MixedReality.Toolkit.Utilities.Handedness;
#endif

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

#if HP_CONTROLLER_ENABLED
            // Listens to events to track the HP Motion Controller
            motionControllerWatcher = new MotionControllerWatcher();
            motionControllerWatcher.MotionControllerAdded += AddTrackedMotionController;
            motionControllerWatcher.MotionControllerRemoved += RemoveTrackedMotionController;
            var nowait = motionControllerWatcher.StartAsync();
#endif
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

#if HP_CONTROLLER_ENABLED
        private MotionControllerWatcher motionControllerWatcher;

        /// <summary>
        /// Dictionary to capture all active HP controllers detected
        /// </summary>
        private readonly Dictionary<uint, MotionControllerState> trackedMotionControllerStates = new Dictionary<uint, MotionControllerState>();

        private readonly Dictionary<uint, GenericXRSDKController> activeMotionControllers = new Dictionary<uint, GenericXRSDKController>();
#endif

        #endregion IMixedRealityCapabilityCheck Implementation


        #region Controller Utilities
#if HP_CONTROLLER_ENABLED
        private static readonly ProfilerMarker GetOrAddControllerPerfMarker = new ProfilerMarker("[MRTK] WindwosMixedRealityXRSDKDeviceManager.GetOrAddController");

        protected override GenericXRSDKController GetOrAddController(InputDevice inputDevice)
        {
            using (GetOrAddControllerPerfMarker.Auto())
            {
                GenericXRSDKController detectedController = base.GetOrAddController(inputDevice);
                SupportedControllerType currentControllerType = GetCurrentControllerType(inputDevice);

                // Add the Motion Controller state if it's an HPMotionController
                if (currentControllerType == SupportedControllerType.HPMotionController)
                {
                    lock (trackedMotionControllerStates)
                    {
                        uint controllerId = GetControllerId(inputDevice);
                        if (trackedMotionControllerStates.ContainsKey(controllerId) && detectedController is HPMotionController hpController)
                        {
                            hpController.MotionControllerState = trackedMotionControllerStates[controllerId];
                        }
                    }
                }

                return detectedController;
            }
        }

        private void AddTrackedMotionController(object sender, MotionController motionController)
        {
            lock (trackedMotionControllerStates)
            {
                uint controllerId = GetControllerId(motionController);
                trackedMotionControllerStates[controllerId] = new MotionControllerState(motionController);

                if (activeMotionControllers.ContainsKey(controllerId) && activeMotionControllers[controllerId] is HPMotionController hpController)
                {
                    hpController.MotionControllerState = trackedMotionControllerStates[controllerId];
                }
            }
        }

        private void RemoveTrackedMotionController(object sender, MotionController motionController)
        {
            lock (trackedMotionControllerStates)
            {
                uint controllerId = GetControllerId(motionController);
                trackedMotionControllerStates.Remove(controllerId);

                if (activeMotionControllers.ContainsKey(controllerId) && activeMotionControllers[controllerId] is HPMotionController hpController)
                {
                    hpController.MotionControllerState = null;
                }
            }
        }
#endif

        // Creates a unique key for the controller based on it's vendor ID, product ID, version number, and handedness
        private uint GetControllerId(uint handedness)
        {
            return handedness;
        }

#if HP_CONTROLLER_ENABLED
        private uint GetControllerId(MotionController mc)
        {
            var handedness = ((uint)(mc.Handedness == MotionControllerHandedness.Right ? 2 : (mc.Handedness == MotionControllerHandedness.Left ? 1 : 0)));
            return GetControllerId(handedness);
        }
#endif

        private uint GetControllerId(InputDevice inputDevice)
        {
            var handedness = ((uint)(inputDevice.characteristics.HasFlag(InputDeviceCharacteristics.Right) ? 2 : inputDevice.characteristics.HasFlag(InputDeviceCharacteristics.Left) ? 1 : 0));
            return GetControllerId(handedness);
        }

        /// <inheritdoc />
        protected override Type GetControllerType(SupportedControllerType supportedControllerType)
        {
            switch (supportedControllerType)
            {
                case SupportedControllerType.WindowsMixedReality:
                    return typeof(WindowsMixedRealityXRSDKMotionController);
                case SupportedControllerType.HPMotionController:
                    return typeof(HPMotionController);
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
                case SupportedControllerType.HPMotionController:
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
                List<InputFeatureUsage> featureUsages = new List<InputFeatureUsage>();
                bool hasTouchpad = inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out var reading);

                bool isHPController = !hasTouchpad;
                uint controllerId = GetControllerId(inputDevice);

                if (isHPController)
                {
                    return SupportedControllerType.HPMotionController;
                }
                else
                {
                    return SupportedControllerType.WindowsMixedReality;
                }
            }

            return base.GetCurrentControllerType(inputDevice);
        }

#endregion Controller Utilities
    }
}
