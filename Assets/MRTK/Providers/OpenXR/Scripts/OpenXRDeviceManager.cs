// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.XRSDK.Input;
using System;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.XR;

#if UNITY_OPENXR
using UnityEngine.XR.OpenXR;
#endif // UNITY_OPENXR

#if MSFT_OPENXR_0_9_4_OR_NEWER && WINDOWS_UWP
using Microsoft.MixedReality.OpenXR;
using Microsoft.MixedReality.Toolkit.Windows.Input;
#endif // MSFT_OPENXR_0_9_4_OR_NEWER && WINDOWS_UWP

namespace Microsoft.MixedReality.Toolkit.XRSDK.OpenXR
{
    [MixedRealityDataProvider(
        typeof(IMixedRealityInputSystem),
        (SupportedPlatforms)(-1),
        "OpenXR XRSDK Device Manager",
        supportedUnityXRPipelines: SupportedUnityXRPipelines.XRSDK)]
    public class OpenXRDeviceManager : XRSDKDeviceManager
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="inputSystem">The <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem"/> instance that receives data from this provider.</param>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        public OpenXRDeviceManager(
            IMixedRealityInputSystem inputSystem,
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null) : base(inputSystem, name, priority, profile) { }

        private bool? IsActiveLoader =>
#if UNITY_OPENXR
            LoaderHelpers.IsLoaderActive<OpenXRLoaderBase>();
#else
            false;
#endif // UNITY_OPENXR

#if MSFT_OPENXR_0_9_4_OR_NEWER && WINDOWS_UWP
        private GestureRecognizer gestureRecognizer;
        private GestureRecognizer navigationGestureRecognizer;
        private GestureEventData eventData;
        private AutoStartBehavior autoStartBehavior;
        private WindowsGestureSettings gestureSettings;
        private WindowsGestureSettings navigationSettings;
        private WindowsGestureSettings railsNavigationSettings;
        private bool useRailsNavigation;

        private MixedRealityInputAction holdAction = MixedRealityInputAction.None;
        private MixedRealityInputAction navigationAction = MixedRealityInputAction.None;
        private MixedRealityInputAction manipulationAction = MixedRealityInputAction.None;
        private MixedRealityInputAction selectAction = MixedRealityInputAction.None;
#endif // MSFT_OPENXR_0_9_4_OR_NEWER && WINDOWS_UWP

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

#if MSFT_OPENXR_0_9_4_OR_NEWER && WINDOWS_UWP
            CreateGestureRecognizers();
#endif // MSFT_OPENXR_0_9_4_OR_NEWER && WINDOWS_UWP

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

#if MSFT_OPENXR_0_9_4_OR_NEWER && WINDOWS_UWP
        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();

            ReadProfile();
        }

        /// <inheritdoc />
        public override void Update()
        {
            if (!IsEnabled)
            {
                return;
            }

            base.Update();

            CheckForGestures();
        }

        /// <inheritdoc />
        public override void Disable()
        {
            if (!IsEnabled)
            {
                return;
            }

            gestureRecognizer?.Stop();
            gestureRecognizer?.Dispose();
            gestureRecognizer = null;

            navigationGestureRecognizer?.Stop();
            navigationGestureRecognizer?.Dispose();
            navigationGestureRecognizer = null;

            base.Disable();
        }
#endif // MSFT_OPENXR_0_9_4_OR_NEWER && WINDOWS_UWP

        #region Controller Utilities

        private static readonly ProfilerMarker GetOrAddControllerPerfMarker = new ProfilerMarker("[MRTK] OpenXRDeviceManager.GetOrAddController");

        /// <summary>
        /// The OpenXR plug-in uses extensions to expose all possible data, which might be surfaced through multiple input devices.
        /// This method is overridden to account for multiple input devices and reuse MRTK controllers if a match is found.
        /// </summary>
        protected override GenericXRSDKController GetOrAddController(InputDevice inputDevice)
        {
            using (GetOrAddControllerPerfMarker.Auto())
            {
                // If this is a new input device, search if an existing input device has matching characteristics
                if (!ActiveControllers.ContainsKey(inputDevice))
                {
                    foreach (InputDevice device in ActiveControllers.Keys)
                    {
                        if (((device.characteristics.HasFlag(InputDeviceCharacteristics.Controller) && inputDevice.characteristics.HasFlag(InputDeviceCharacteristics.Controller))
                            || (device.characteristics.HasFlag(InputDeviceCharacteristics.HandTracking) && inputDevice.characteristics.HasFlag(InputDeviceCharacteristics.HandTracking)))
                            && ((device.characteristics.HasFlag(InputDeviceCharacteristics.Left) && inputDevice.characteristics.HasFlag(InputDeviceCharacteristics.Left))
                            || (device.characteristics.HasFlag(InputDeviceCharacteristics.Right) && inputDevice.characteristics.HasFlag(InputDeviceCharacteristics.Right))))
                        {
                            ActiveControllers.Add(inputDevice, ActiveControllers[device]);
                            break;
                        }
                    }
                }

                if (inputDevice.characteristics.HasFlag(InputDeviceCharacteristics.HandTracking)
                    && inputDevice.TryGetFeatureValue(CommonUsages.isTracked, out bool isTracked)
                    && !isTracked)
                {
                    // If this is an input device from the Microsoft Hand Interaction profile, it doesn't go invalid but instead goes untracked. Ignore it if untracked.
                    return null;
                }

                return base.GetOrAddController(inputDevice);
            }
        }

        private static readonly ProfilerMarker RemoveControllerPerfMarker = new ProfilerMarker("[MRTK] OpenXRDeviceManager.RemoveController");

        /// <summary>
        /// The OpenXR plug-in uses extensions to expose all possible data, which might be surfaced through multiple input devices.
        /// This method is overridden to account for multiple input devices and reuse MRTK controllers if a match is found.
        /// </summary>
        protected override void RemoveController(InputDevice inputDevice)
        {
            using (RemoveControllerPerfMarker.Auto())
            {
                foreach (InputDevice device in ActiveControllers.Keys)
                {
                    if (device != inputDevice
                        && ((device.characteristics.HasFlag(InputDeviceCharacteristics.Controller) && inputDevice.characteristics.HasFlag(InputDeviceCharacteristics.Controller))
                        || (device.characteristics.HasFlag(InputDeviceCharacteristics.HandTracking) && inputDevice.characteristics.HasFlag(InputDeviceCharacteristics.HandTracking)))
                        && ((device.characteristics.HasFlag(InputDeviceCharacteristics.Left) && inputDevice.characteristics.HasFlag(InputDeviceCharacteristics.Left))
                        || (device.characteristics.HasFlag(InputDeviceCharacteristics.Right) && inputDevice.characteristics.HasFlag(InputDeviceCharacteristics.Right))))
                    {
                        ActiveControllers.Remove(inputDevice);
                        // Since an additional device exists, return so a lost source isn't reported
                        return;
                    }
                }

                base.RemoveController(inputDevice);
            }
        }

        /// <inheritdoc />
        protected override Type GetControllerType(SupportedControllerType supportedControllerType)
        {
            switch (supportedControllerType)
            {
                case SupportedControllerType.WindowsMixedReality:
                    return typeof(MicrosoftMotionController);
                case SupportedControllerType.HPMotionController:
                    return typeof(HPReverbG2Controller);
                case SupportedControllerType.ArticulatedHand:
                    return typeof(MicrosoftArticulatedHand);
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
                case SupportedControllerType.HPMotionController:
                    return InputSourceType.Controller;
                case SupportedControllerType.ArticulatedHand:
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
                return SupportedControllerType.ArticulatedHand;
            }

            if (inputDevice.characteristics.HasFlag(InputDeviceCharacteristics.Controller))
            {
                if (inputDevice.manufacturer == "HP")
                {
                    return SupportedControllerType.HPMotionController;
                }
                else // Fall back to the base WMR controller
                {
                    return SupportedControllerType.WindowsMixedReality;
                }
            }

            return base.GetCurrentControllerType(inputDevice);
        }

        #endregion Controller Utilities

        #region Gesture implementation

#if MSFT_OPENXR_0_9_4_OR_NEWER && WINDOWS_UWP
        private void ReadProfile()
        {
            if (InputSystemProfile.GesturesProfile != null)
            {
                MixedRealityGesturesProfile gestureProfile = InputSystemProfile.GesturesProfile;
                gestureSettings = gestureProfile.ManipulationGestures;
                navigationSettings = gestureProfile.NavigationGestures;
                railsNavigationSettings = gestureProfile.RailsNavigationGestures;
                useRailsNavigation = gestureProfile.UseRailsNavigation;
                autoStartBehavior = gestureProfile.WindowsGestureAutoStart;

                for (int i = 0; i < gestureProfile.Gestures.Length; i++)
                {
                    var gesture = gestureProfile.Gestures[i];

                    switch (gesture.GestureType)
                    {
                        case GestureInputType.Hold:
                            holdAction = gesture.Action;
                            break;
                        case GestureInputType.Manipulation:
                            manipulationAction = gesture.Action;
                            break;
                        case GestureInputType.Navigation:
                            navigationAction = gesture.Action;
                            break;
                        case GestureInputType.Select:
                            selectAction = gesture.Action;
                            break;
                    }
                }
            }
        }

        private void CreateGestureRecognizers()
        {
            if (holdAction != MixedRealityInputAction.None ||
                manipulationAction != MixedRealityInputAction.None ||
                selectAction != MixedRealityInputAction.None)
            {
                if (gestureRecognizer == null)
                {
                    try
                    {
                        gestureRecognizer = new GestureRecognizer((GestureSettings)gestureSettings);

                        if (autoStartBehavior == AutoStartBehavior.AutoStart)
                        {
                            gestureRecognizer.Start();
                        }
                    }
                    catch (Exception ex)
                    {
                        UnityEngine.Debug.LogWarning($"Failed to create gesture recognizer. OS version might not support it. Exception: {ex}");
                        gestureRecognizer = null;
                        return;
                    }
                }
            }

            if (navigationAction != MixedRealityInputAction.None)
            {
                if (navigationGestureRecognizer == null)
                {
                    try
                    {
                        navigationGestureRecognizer = new GestureRecognizer((GestureSettings)(useRailsNavigation ? railsNavigationSettings : navigationSettings));

                        if (autoStartBehavior == AutoStartBehavior.AutoStart)
                        {
                            navigationGestureRecognizer.Start();
                        }
                    }
                    catch (Exception ex)
                    {
                        UnityEngine.Debug.LogWarning($"Failed to create gesture recognizer. OS version might not support it. Exception: {ex}");
                        navigationGestureRecognizer = null;
                        return;
                    }
                }
            }
        }

        private void CheckForGestures()
        {
            if (gestureRecognizer != null)
            {
                while (gestureRecognizer.TryGetNextEvent(ref eventData))
                {
                    switch (eventData.EventType)
                    {
                        case GestureEventType.Tapped:
                            if (selectAction != MixedRealityInputAction.None)
                            {
                                GenericXRSDKController controller = FindMatchingController(eventData.Handedness);
                                if (controller != null)
                                {
                                    Service?.RaiseGestureCompleted(controller, selectAction);
                                }
                            }
                            break;
                        case GestureEventType.HoldStarted:
                            if (holdAction != MixedRealityInputAction.None)
                            {
                                GenericXRSDKController controller = FindMatchingController(eventData.Handedness);
                                if (controller != null)
                                {
                                    Service?.RaiseGestureStarted(controller, holdAction);
                                }
                            }
                            break;
                        case GestureEventType.HoldCompleted:
                            if (holdAction != MixedRealityInputAction.None)
                            {
                                GenericXRSDKController controller = FindMatchingController(eventData.Handedness);
                                if (controller != null)
                                {
                                    Service?.RaiseGestureCompleted(controller, holdAction);
                                }
                            }
                            break;
                        case GestureEventType.HoldCanceled:
                            if (holdAction != MixedRealityInputAction.None)
                            {
                                GenericXRSDKController controller = FindMatchingController(eventData.Handedness);
                                if (controller != null)
                                {
                                    Service?.RaiseGestureCanceled(controller, holdAction);
                                }
                            }
                            break;
                        case GestureEventType.ManipulationStarted:
                            if (manipulationAction != MixedRealityInputAction.None)
                            {
                                GenericXRSDKController controller = FindMatchingController(eventData.Handedness);
                                if (controller != null)
                                {
                                    Service?.RaiseGestureStarted(controller, manipulationAction);
                                }
                            }
                            break;
                        case GestureEventType.ManipulationUpdated:
                            if (manipulationAction != MixedRealityInputAction.None)
                            {
                                GenericXRSDKController controller = FindMatchingController(eventData.Handedness);
                                if (controller != null)
                                {
                                    Service?.RaiseGestureUpdated(controller, manipulationAction, eventData.ManipulationData.GetValueOrDefault().CumulativeTranslation);
                                }
                            }
                            break;
                        case GestureEventType.ManipulationCompleted:
                            if (manipulationAction != MixedRealityInputAction.None)
                            {
                                GenericXRSDKController controller = FindMatchingController(eventData.Handedness);
                                if (controller != null)
                                {
                                    Service?.RaiseGestureCompleted(controller, manipulationAction, eventData.ManipulationData.GetValueOrDefault().CumulativeTranslation);
                                }
                            }
                            break;
                        case GestureEventType.ManipulationCanceled:
                            if (manipulationAction != MixedRealityInputAction.None)
                            {
                                GenericXRSDKController controller = FindMatchingController(eventData.Handedness);
                                if (controller != null)
                                {
                                    Service?.RaiseGestureCanceled(controller, manipulationAction);
                                }
                            }
                            break;
                    }
                }
            }

            if (navigationGestureRecognizer != null)
            {
                while (navigationGestureRecognizer.TryGetNextEvent(ref eventData))
                {
                    switch (eventData.EventType)
                    {
                        case GestureEventType.NavigationStarted:
                            if (navigationAction != MixedRealityInputAction.None)
                            {
                                GenericXRSDKController controller = FindMatchingController(eventData.Handedness);
                                if (controller != null)
                                {
                                    Service?.RaiseGestureStarted(controller, navigationAction);
                                }
                            }
                            break;
                        case GestureEventType.NavigationUpdated:
                            if (navigationAction != MixedRealityInputAction.None)
                            {
                                GenericXRSDKController controller = FindMatchingController(eventData.Handedness);
                                if (controller != null)
                                {
                                    Service?.RaiseGestureUpdated(controller, navigationAction, eventData.NavigationData.GetValueOrDefault().NormalizedOffset);
                                }
                            }
                            break;
                        case GestureEventType.NavigationCompleted:
                            if (navigationAction != MixedRealityInputAction.None)
                            {
                                GenericXRSDKController controller = FindMatchingController(eventData.Handedness);
                                if (controller != null)
                                {
                                    Service?.RaiseGestureCompleted(controller, navigationAction, eventData.NavigationData.GetValueOrDefault().NormalizedOffset);
                                }
                            }
                            break;
                        case GestureEventType.NavigationCanceled:
                            if (navigationAction != MixedRealityInputAction.None)
                            {
                                GenericXRSDKController controller = FindMatchingController(eventData.Handedness);
                                if (controller != null)
                                {
                                    Service?.RaiseGestureCanceled(controller, navigationAction);
                                }
                            }
                            break;
                    }
                }
            }
        }

        private GenericXRSDKController FindMatchingController(GestureHandedness gestureHandedness)
        {
            Utilities.Handedness handedness = gestureHandedness == GestureHandedness.Left ? Utilities.Handedness.Left : Utilities.Handedness.Right;

            foreach (GenericXRSDKController controller in ActiveControllers.Values)
            {
                if (controller.ControllerHandedness == handedness)
                {
                    return controller;
                }
            }

            return null;
        }
#endif // MSFT_OPENXR_0_9_4_OR_NEWER && WINDOWS_UWP

        #endregion Gesture implementation
    }
}
