// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.Devices;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem;
using MixedRealityGestureSettings = Microsoft.MixedReality.Toolkit.Core.Definitions.InputSystem.GestureSettings;

#if UNITY_WSA
using Microsoft.MixedReality.Toolkit.Core.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using UnityEngine.XR.WSA.Input;
using UnityEngine;
using WsaGestureSettings = UnityEngine.XR.WSA.Input.GestureSettings;
#endif // UNITY_WSA

namespace Microsoft.MixedReality.Toolkit.Core.Devices.WindowsMixedReality
{
    public class WindowsMixedRealityDeviceManager : BaseDeviceManager
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="priority"></param>
        public WindowsMixedRealityDeviceManager(string name, uint priority) : base(name, priority) { }

        /// <summary>
        /// Dictionary to capture all active controllers detected
        /// </summary>
        private readonly Dictionary<uint, IMixedRealityController> activeControllers = new Dictionary<uint, IMixedRealityController>();

        /// <inheritdoc/>
        public override IMixedRealityController[] GetActiveControllers()
        {
            return activeControllers.Values.ToArray();
        }

        private static bool gestureRecognizerEnabled;

        /// <summary>
        /// Enables or disables the gesture recognizer.
        /// </summary>
        /// <remarks>
        /// Automatically disabled navigation recognizer if enabled.
        /// </remarks>
        public static bool GestureRecognizerEnabled
        {
            get
            {
                return gestureRecognizerEnabled;
            }
            set
            {
#if UNITY_WSA
                if (gestureRecognizer == null)
                {
                    gestureRecognizerEnabled = false;
                    return;
                }
#endif // UNITY_WSA
                gestureRecognizerEnabled = value;
#if UNITY_WSA
                if (!gestureRecognizer.IsCapturingGestures() && gestureRecognizerEnabled)
                {
                    NavigationRecognizerEnabled = false;
                    gestureRecognizer.StartCapturingGestures();
                }

                if (gestureRecognizer.IsCapturingGestures() && !gestureRecognizerEnabled)
                {
                    gestureRecognizer.StopCapturingGestures();
                }
#endif // UNITY_WSA
            }
        }

        private static bool navigationRecognizerEnabled;

        /// <summary>
        /// Enables or disables the navigation recognizer.
        /// </summary>
        /// <remarks>
        /// Automatically disables the gesture recognizer if enabled.
        /// </remarks>
        public static bool NavigationRecognizerEnabled
        {
            get
            {
                return navigationRecognizerEnabled;
            }
            set
            {
#if UNITY_WSA
                if (navigationGestureRecognizer == null)
                {
                    navigationRecognizerEnabled = false;
                    return;
                }
#endif // UNITY_WSA
                navigationRecognizerEnabled = value;
#if UNITY_WSA
                if (!navigationGestureRecognizer.IsCapturingGestures() && navigationRecognizerEnabled)
                {
                    GestureRecognizerEnabled = false;
                    navigationGestureRecognizer.StartCapturingGestures();
                }

                if (navigationGestureRecognizer.IsCapturingGestures() && !navigationRecognizerEnabled)
                {
                    navigationGestureRecognizer.StopCapturingGestures();
                }
#endif // UNITY_WSA
            }
        }

        private static MixedRealityGestureSettings gestureSettings = MixedRealityGestureSettings.Tap | MixedRealityGestureSettings.ManipulationTranslate | MixedRealityGestureSettings.Hold;

        /// <summary>
        /// Current Gesture Settings for the GestureRecognizer
        /// </summary>
        public static MixedRealityGestureSettings GestureSettings
        {
            get { return gestureSettings; }
            set
            {
                gestureSettings = value;
#if UNITY_WSA
                gestureRecognizer.SetRecognizableGestures(WSAGestureSettings);
#endif
            }
        }

        public static MixedRealityGestureSettings NavigationSettings { get; set; } = MixedRealityGestureSettings.NavigationX | MixedRealityGestureSettings.NavigationY | MixedRealityGestureSettings.NavigationZ;

        public static MixedRealityGestureSettings RailsNavigationSettings { get; set; } = MixedRealityGestureSettings.NavigationRailsX | MixedRealityGestureSettings.NavigationRailsY | MixedRealityGestureSettings.NavigationRailsZ;

        private static bool useRailsNavigation = true;

        public static bool UseRailsNavigation
        {
            get { return useRailsNavigation; }
            set
            {
                useRailsNavigation = value;
#if UNITY_WSA
                navigationGestureRecognizer?.SetRecognizableGestures(useRailsNavigation ? WSANavigationSettings : WSARailsNavigationSettings);
#endif
            }
        }

#if UNITY_WSA
        private static GestureRecognizer gestureRecognizer;
        private static WsaGestureSettings WSAGestureSettings => (WsaGestureSettings)gestureSettings;
        private static GestureRecognizer navigationGestureRecognizer;
        private static WsaGestureSettings WSANavigationSettings => (WsaGestureSettings)NavigationSettings;
        private static WsaGestureSettings WSARailsNavigationSettings => (WsaGestureSettings)RailsNavigationSettings;

        #region IMixedRealityDeviceManager Interface

        /// <inheritdoc/>
        public override void Enable()
        {
            if (!Application.isPlaying) { return; }

            RegisterGestureEvents();
            RegisterNavigationEvents();

            InteractionManager.InteractionSourceDetected += InteractionManager_InteractionSourceDetected;
            InteractionManager.InteractionSourceUpdated += InteractionManager_InteractionSourceUpdated;
            InteractionManager.InteractionSourcePressed += InteractionManager_InteractionSourcePressed;
            InteractionManager.InteractionSourceReleased += InteractionManager_InteractionSourceReleased;
            InteractionManager.InteractionSourceLost += InteractionManager_InteractionSourceLost;

            InteractionSourceState[] states = InteractionManager.GetCurrentReading();

            // NOTE: We update the source state data, in case an app wants to query it on source detected.
            for (var i = 0; i < states.Length; i++)
            {
                var controller = GetController(states[i].source);

                if (controller != null)
                {
                    controller.UpdateController(states[i]);
                    InputSystem.RaiseSourceDetected(controller.InputSource, controller);
                }
            }
        }

        private void RegisterGestureEvents()
        {
            if (gestureRecognizer == null)
            {
                gestureRecognizer = new GestureRecognizer();
            }

            gestureRecognizer.Tapped += GestureRecognizer_Tapped;

            gestureRecognizer.HoldStarted += GestureRecognizer_HoldStarted;
            gestureRecognizer.HoldCompleted += GestureRecognizer_HoldCompleted;
            gestureRecognizer.HoldCanceled += GestureRecognizer_HoldCanceled;

            gestureRecognizer.ManipulationStarted += GestureRecognizer_ManipulationStarted;
            gestureRecognizer.ManipulationUpdated += GestureRecognizer_ManipulationUpdated;
            gestureRecognizer.ManipulationCompleted += GestureRecognizer_ManipulationCompleted;
            gestureRecognizer.ManipulationCanceled += GestureRecognizer_ManipulationCanceled;
        }

        private void UnregisterGestureEvents()
        {
            if (gestureRecognizer == null) { return; }

            gestureRecognizer.Tapped -= GestureRecognizer_Tapped;

            gestureRecognizer.HoldStarted -= GestureRecognizer_HoldStarted;
            gestureRecognizer.HoldCompleted -= GestureRecognizer_HoldCompleted;
            gestureRecognizer.HoldCanceled -= GestureRecognizer_HoldCanceled;

            gestureRecognizer.ManipulationStarted -= GestureRecognizer_ManipulationStarted;
            gestureRecognizer.ManipulationUpdated -= GestureRecognizer_ManipulationUpdated;
            gestureRecognizer.ManipulationCompleted -= GestureRecognizer_ManipulationCompleted;
            gestureRecognizer.ManipulationCanceled -= GestureRecognizer_ManipulationCanceled;
        }

        private void RegisterNavigationEvents()
        {
            if (navigationGestureRecognizer == null)
            {
                navigationGestureRecognizer = new GestureRecognizer();
            }

            navigationGestureRecognizer.NavigationStarted += NavigationGestureRecognizer_NavigationStarted;
            navigationGestureRecognizer.NavigationUpdated += NavigationGestureRecognizer_NavigationUpdated;
            navigationGestureRecognizer.NavigationCompleted += NavigationGestureRecognizer_NavigationCompleted;
            navigationGestureRecognizer.NavigationCanceled += NavigationGestureRecognizer_NavigationCanceled;
        }

        private void UnregisterNavigationEvents()
        {
            if (navigationGestureRecognizer == null) { return; }
            navigationGestureRecognizer.NavigationStarted -= NavigationGestureRecognizer_NavigationStarted;
            navigationGestureRecognizer.NavigationUpdated -= NavigationGestureRecognizer_NavigationUpdated;
            navigationGestureRecognizer.NavigationCompleted -= NavigationGestureRecognizer_NavigationCompleted;
            navigationGestureRecognizer.NavigationCanceled -= NavigationGestureRecognizer_NavigationCanceled;
        }

        /// <inheritdoc/>
        public override void Disable()
        {
            UnregisterGestureEvents();
            gestureRecognizer?.Dispose();

            UnregisterNavigationEvents();
            navigationGestureRecognizer?.Dispose();

            InteractionManager.InteractionSourceDetected -= InteractionManager_InteractionSourceDetected;
            InteractionManager.InteractionSourcePressed -= InteractionManager_InteractionSourcePressed;
            InteractionManager.InteractionSourceUpdated -= InteractionManager_InteractionSourceUpdated;
            InteractionManager.InteractionSourceReleased -= InteractionManager_InteractionSourceReleased;
            InteractionManager.InteractionSourceLost -= InteractionManager_InteractionSourceLost;

            InteractionSourceState[] states = InteractionManager.GetCurrentReading();
            for (var i = 0; i < states.Length; i++)
            {
                RemoveController(states[i]);
            }
        }

        #endregion IMixedRealityDeviceManager Interface

        #region Controller Utilities

        /// <summary>
        /// Retrieve the source controller from the Active Store, or create a new device and register it
        /// </summary>
        /// <param name="interactionSourceState">Source State provided by the SDK</param>
        /// <param name="addController">Should the Source be added as a controller if it isn't found?</param>
        /// <returns>New or Existing Controller Input Source</returns>
        private WindowsMixedRealityController GetController(InteractionSource interactionSourceState, bool addController = true)
        {
            //If a device is already registered with the ID provided, just return it.
            if (activeControllers.ContainsKey(interactionSourceState.id))
            {
                var controller = activeControllers[interactionSourceState.id] as WindowsMixedRealityController;
                Debug.Assert(controller != null);
                return controller;
            }

            if (!addController) { return null; }

            Handedness controllingHand;
            switch (interactionSourceState.handedness)
            {
                default:
                    controllingHand = Handedness.None;
                    break;
                case InteractionSourceHandedness.Left:
                    controllingHand = Handedness.Left;
                    break;
                case InteractionSourceHandedness.Right:
                    controllingHand = Handedness.Right;
                    break;
            }

            IMixedRealityPointer[] pointers = interactionSourceState.supportsPointing ? RequestPointers(typeof(WindowsMixedRealityController), controllingHand) : null;
            string nameModifier = controllingHand == Handedness.None ? "Hand" : controllingHand.ToString();
            var inputSource = InputSystem?.RequestNewGenericInputSource($"Mixed Reality Controller {nameModifier}", pointers);
            var detectedController = new WindowsMixedRealityController(TrackingState.NotTracked, controllingHand, inputSource);

            if (!detectedController.SetupConfiguration(typeof(WindowsMixedRealityController)))
            {
                // Controller failed to be setup correctly.
                // Return null so we don't raise the source detected.
                return null;
            }

            for (int i = 0; i < detectedController.InputSource?.Pointers?.Length; i++)
            {
                detectedController.InputSource.Pointers[i].Controller = detectedController;
            }

            activeControllers.Add(interactionSourceState.id, detectedController);
            return detectedController;
        }

        /// <summary>
        /// Remove the selected controller from the Active Store
        /// </summary>
        /// <param name="interactionSourceState">Source State provided by the SDK to remove</param>
        private void RemoveController(InteractionSourceState interactionSourceState)
        {
            var controller = GetController(interactionSourceState.source);

            if (controller != null)
            {
                InputSystem?.RaiseSourceLost(controller.InputSource, controller);
            }

            activeControllers.Remove(interactionSourceState.source.id);
        }

        #endregion Controller Utilities

        #region Unity InteractionManager Events

        /// <summary>
        /// SDK Interaction Source Detected Event handler
        /// </summary>
        /// <param name="args">SDK source detected event arguments</param>
        private void InteractionManager_InteractionSourceDetected(InteractionSourceDetectedEventArgs args)
        {
            var controller = GetController(args.state.source);

            if (controller != null)
            {
                InputSystem?.RaiseSourceDetected(controller.InputSource, controller);
            }

            controller?.UpdateController(args.state);
        }

        /// <summary>
        /// SDK Interaction Source Updated Event handler
        /// </summary>
        /// <param name="args">SDK source updated event arguments</param>
        private void InteractionManager_InteractionSourceUpdated(InteractionSourceUpdatedEventArgs args)
        {
            GetController(args.state.source)?.UpdateController(args.state);
        }

        /// <summary>
        /// SDK Interaction Source Pressed Event handler
        /// </summary>
        /// <param name="args">SDK source pressed event arguments</param>
        private void InteractionManager_InteractionSourcePressed(InteractionSourcePressedEventArgs args)
        {
            GetController(args.state.source)?.UpdateController(args.state);
        }

        /// <summary>
        /// SDK Interaction Source Released Event handler
        /// </summary>
        /// <param name="args">SDK source released event arguments</param>
        private void InteractionManager_InteractionSourceReleased(InteractionSourceReleasedEventArgs args)
        {
            GetController(args.state.source)?.UpdateController(args.state);
        }

        /// <summary>
        /// SDK Interaction Source Lost Event handler
        /// </summary>
        /// <param name="args">SDK source updated event arguments</param>
        private void InteractionManager_InteractionSourceLost(InteractionSourceLostEventArgs args)
        {
            RemoveController(args.state);
        }

        #endregion Unity InteractionManager Events

        #region Gesture Recognizer Events

        private void GestureRecognizer_Tapped(TappedEventArgs args)
        {
            var controller = GetController(args.source, false);
            if (controller != null)
            {
                for (int i = 0; i < controller.InputSource.Pointers.Length; i++)
                {
                    InputSystem.RaisePointerClicked(controller.InputSource.Pointers[i], (Handedness)args.source.handedness, controller.Interactions[5].MixedRealityInputAction, args.tapCount);
                }
            }
        }

        private void GestureRecognizer_HoldStarted(HoldStartedEventArgs args)
        {
            var controller = GetController(args.source, false);
            if (controller != null)
            {
                InputSystem.RaiseHoldStarted(controller.InputSource, (Handedness)args.source.handedness);
            }
        }

        private void GestureRecognizer_HoldCanceled(HoldCanceledEventArgs args)
        {
            var controller = GetController(args.source, false);
            if (controller != null)
            {
                InputSystem.RaiseHoldCanceled(controller.InputSource, (Handedness)args.source.handedness);
            }
        }

        private void GestureRecognizer_HoldCompleted(HoldCompletedEventArgs args)
        {
            var controller = GetController(args.source, false);
            if (controller != null)
            {
                InputSystem.RaiseHoldCompleted(controller.InputSource, (Handedness)args.source.handedness);
            }
        }

        private void GestureRecognizer_ManipulationStarted(ManipulationStartedEventArgs args)
        {
            var controller = GetController(args.source, false);
            if (controller != null)
            {
                InputSystem.RaiseManipulationStarted(controller.InputSource, (Handedness)args.source.handedness);
            }
        }

        private void GestureRecognizer_ManipulationUpdated(ManipulationUpdatedEventArgs args)
        {
            var controller = GetController(args.source, false);
            if (controller != null)
            {
                InputSystem.RaiseManipulationUpdated(controller.InputSource, (Handedness)args.source.handedness, args.cumulativeDelta);
            }
        }

        private void GestureRecognizer_ManipulationCompleted(ManipulationCompletedEventArgs args)
        {
            var controller = GetController(args.source, false);
            if (controller != null)
            {
                InputSystem.RaiseManipulationCompleted(controller.InputSource, (Handedness)args.source.handedness, args.cumulativeDelta);
            }
        }

        private void GestureRecognizer_ManipulationCanceled(ManipulationCanceledEventArgs args)
        {
            var controller = GetController(args.source, false);
            if (controller != null)
            {
                InputSystem.RaiseManipulationCanceled(controller.InputSource, (Handedness)args.source.handedness);
            }
        }

        #endregion Gesture Recognizer Events

        #region Navigation Recognizer Events

        private void NavigationGestureRecognizer_NavigationStarted(NavigationStartedEventArgs args)
        {
            var controller = GetController(args.source, false);
            if (controller != null)
            {
                InputSystem.RaiseNavigationStarted(controller.InputSource, (Handedness)args.source.handedness);
            }
        }

        private void NavigationGestureRecognizer_NavigationUpdated(NavigationUpdatedEventArgs args)
        {
            var controller = GetController(args.source, false);
            if (controller != null)
            {
                InputSystem.RaiseNavigationUpdated(controller.InputSource, (Handedness)args.source.handedness, args.normalizedOffset);
            }
        }

        private void NavigationGestureRecognizer_NavigationCompleted(NavigationCompletedEventArgs args)
        {
            var controller = GetController(args.source, false);
            if (controller != null)
            {
                InputSystem.RaiseNavigationCompleted(controller.InputSource, (Handedness)args.source.handedness, args.normalizedOffset);
            }
        }

        private void NavigationGestureRecognizer_NavigationCanceled(NavigationCanceledEventArgs args)
        {
            var controller = GetController(args.source, false);
            if (controller != null)
            {
                InputSystem.RaiseNavigationCanceled(controller.InputSource, (Handedness)args.source.handedness);
            }
        }

        #endregion Navigation Recognizer Events

#endif // UNITY_WSA

    }
}