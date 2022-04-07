// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Windows.Utilities;
using Microsoft.MixedReality.Toolkit.WindowsMixedReality;
using Microsoft.MixedReality.Toolkit.XRSDK.Input;
using System;
using UnityEngine;
using UnityEngine.XR;

#if HP_CONTROLLER_ENABLED
using Microsoft.MixedReality.Input;
using MotionControllerHandedness = Microsoft.MixedReality.Input.Handedness;
using System.Collections.Generic;
using Unity.Profiling;
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
        "XR SDK Windows Mixed Reality Device Manager",
        supportedUnityXRPipelines: SupportedUnityXRPipelines.XRSDK)]
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

        private bool? IsActiveLoader =>
#if WMR_ENABLED
            LoaderHelpers.IsLoaderActive("Windows MR Loader");
#else
            false;
#endif // WMR_ENABLED

        #region IMixedRealityDeviceManager Interface

        /// <inheritdoc />
        public override void Enable()
        {
            if (!IsActiveLoader.HasValue)
            {
                IsEnabled = false;
                EnableIfLoaderBecomesActive();
                return;
            }
            else if (!IsActiveLoader.Value)
            {
                IsEnabled = false;
                return;
            }

            if (WindowsMixedRealityUtilities.UtilitiesProvider == null)
            {
                WindowsMixedRealityUtilities.UtilitiesProvider = new XRSDKWindowsMixedRealityUtilitiesProvider();
            }

#if (UNITY_WSA && DOTNETWINRT_PRESENT) || WINDOWS_UWP
            mixedRealityGazeProviderHeadOverride = Service?.GazeProvider as IMixedRealityGazeProviderHeadOverride;
#endif // (UNITY_WSA && DOTNETWINRT_PRESENT) || WINDOWS_UWP

#if WINDOWS_UWP
            WindowsMixedRealityUtilities.SpatialInteractionManager.SourcePressed += SpatialInteractionManager_SourcePressed;
#endif // WINDOWS_UWP

#if HP_CONTROLLER_ENABLED
            // Listens to events to track the HP Motion Controller
            motionControllerWatcher = new MotionControllerWatcher();
            motionControllerWatcher.MotionControllerAdded += AddTrackedMotionController;
            motionControllerWatcher.MotionControllerRemoved += RemoveTrackedMotionController;
            var nowait = motionControllerWatcher.StartAsync();
#endif // HP_CONTROLLER_ENABLED

            base.Enable();
        }

        private async void EnableIfLoaderBecomesActive()
        {
            await new WaitUntil(() => IsActiveLoader.HasValue);
            if (IsActiveLoader.Value)
            {
                Enable();
            }
        }

#if (UNITY_WSA && DOTNETWINRT_PRESENT) || WINDOWS_UWP
        private IMixedRealityGazeProviderHeadOverride mixedRealityGazeProviderHeadOverride = null;

        /// <inheritdoc />
        public override void Update()
        {
            if (!IsEnabled)
            {
                return;
            }

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

#if WINDOWS_UWP
            if (shouldSendVoiceEvents)
            {
                WindowsMixedRealityXRSDKGGVHand controller = GetOrAddVoiceController();
                if (controller != null)
                {
                    // RaiseOnInputDown for "select"
                    controller.UpdateVoiceState(true);
                    // RaiseOnInputUp for "select"
                    controller.UpdateVoiceState(false);

                    // On WMR, the voice recognizer does not actually register the phrase 'select'
                    // when you add it to the speech commands profile. Therefore, simulate
                    // the "select" voice command running to ensure that we get a select voice command
                    // registered. This is used by FocusProvider to detect when the select pointer is active.
                    Service?.RaiseSpeechCommandRecognized(controller.InputSource, RecognitionConfidenceLevel.High, TimeSpan.MinValue, DateTime.Now, new SpeechCommands("select", KeyCode.Alpha1, MixedRealityInputAction.None));
                }

                shouldSendVoiceEvents = false;
            }
#endif // WINDOWS_UWP

            base.Update();
        }
#endif // (UNITY_WSA && DOTNETWINRT_PRESENT) || WINDOWS_UWP

#if WINDOWS_UWP
        /// <inheritdoc/>
        public override void Disable()
        {
            WindowsMixedRealityUtilities.SpatialInteractionManager.SourcePressed -= SpatialInteractionManager_SourcePressed;

            if (voiceController != null)
            {
                RemoveControllerFromScene(voiceController);
                voiceController = null;
            }

            base.Disable();
        }
#endif // WINDOWS_UWP

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

#if HP_CONTROLLER_ENABLED
        private MotionControllerWatcher motionControllerWatcher;

        /// <summary>
        /// Dictionary to capture all active HP controllers detected
        /// </summary>
        private readonly Dictionary<uint, MotionControllerState> trackedMotionControllerStates = new Dictionary<uint, MotionControllerState>();

        private readonly Dictionary<uint, GenericXRSDKController> activeMotionControllers = new Dictionary<uint, GenericXRSDKController>();

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

        // Creates a unique key for the controller based on its vendor ID, product ID, version number, and handedness
        private uint GetControllerId(uint handedness)
        {
            return handedness;
        }

        private uint GetControllerId(MotionController mc)
        {
            var handedness = (uint)(mc.Handedness == MotionControllerHandedness.Right ? 2 : (mc.Handedness == MotionControllerHandedness.Left ? 1 : 0));
            return GetControllerId(handedness);
        }

        private uint GetControllerId(InputDevice inputDevice)
        {
            var handedness = (uint)(inputDevice.characteristics.IsMaskSet(InputDeviceCharacteristics.Right) ? 2 : (inputDevice.characteristics.IsMaskSet(InputDeviceCharacteristics.Left) ? 1 : 0));
            return GetControllerId(handedness);
        }
#endif // HP_CONTROLLER_ENABLED

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
            if (inputDevice.characteristics.IsMaskSet(InputDeviceCharacteristics.HandTracking))
            {
                if (inputDevice.characteristics.IsMaskSet(InputDeviceCharacteristics.Left) ||
                    inputDevice.characteristics.IsMaskSet(InputDeviceCharacteristics.Right))
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

            if (inputDevice.characteristics.IsMaskSet(InputDeviceCharacteristics.Controller))
            {
                // primary2DAxis represents the touchpad in Windows XR Plugin.
                // The HP motion controller doesn't have a touchpad, so we check for its existence in the feature usages.
                if (!inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out _))
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

        #region SpatialInteractionManager events

#if WINDOWS_UWP
        /// <summary>
        /// SDK Interaction Source Pressed Event handler. Used only for voice.
        /// </summary>
        /// <param name="args">SDK source pressed event arguments</param>
        private void SpatialInteractionManager_SourcePressed(SpatialInteractionManager sender, SpatialInteractionSourceEventArgs args)
        {
            if (args.State.Source.Kind == SpatialInteractionSourceKind.Voice)
            {
                shouldSendVoiceEvents = true;
            }
        }

        private WindowsMixedRealityXRSDKGGVHand voiceController = null;
        private bool shouldSendVoiceEvents = false;

        private WindowsMixedRealityXRSDKGGVHand GetOrAddVoiceController()
        {
            if (voiceController != null)
            {
                return voiceController;
            }

            IMixedRealityInputSource inputSource = Service?.RequestNewGenericInputSource("Mixed Reality Voice", sourceType: InputSourceType.Voice);
            WindowsMixedRealityXRSDKGGVHand detectedController = new WindowsMixedRealityXRSDKGGVHand(TrackingState.NotTracked, Handedness.None, inputSource);

            if (!detectedController.Enabled)
            {
                // Controller failed to be setup correctly.
                // Return null so we don't raise the source detected.
                return null;
            }

            for (int i = 0; i < detectedController.InputSource?.Pointers?.Length; i++)
            {
                detectedController.InputSource.Pointers[i].Controller = detectedController;
            }

            Service?.RaiseSourceDetected(detectedController.InputSource, detectedController);

            voiceController = detectedController;
            return voiceController;
        }
#endif // WINDOWS_UWP

        #endregion SpatialInteractionManager events
    }
}
