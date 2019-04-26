// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Windows.Input;
using UnityEngine;

#if UNITY_WSA
using System.Collections.Generic;
using System.Linq;
using UnityEngine.XR.WSA.Input;
using WsaGestureSettings = UnityEngine.XR.WSA.Input.GestureSettings;
#endif // UNITY_WSA

namespace Microsoft.MixedReality.Toolkit.WindowsMixedReality.Input
{
    [MixedRealityDataProvider(
        typeof(IMixedRealityInputSystem),
        SupportedPlatforms.WindowsUniversal,
        "Windows Mixed Reality Device Manager")]
    public class WindowsMixedRealityDeviceManager : BaseInputDeviceManager
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="registrar">The <see cref="IMixedRealityServiceRegistrar"/> instance that loaded the data provider.</param>
        /// <param name="inputSystem">The <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem"/> instance that receives data from this provider.</param>
        /// <param name="inputSystemProfile">The input system configuration profile.</param>
        /// <param name="playspace">The <see href="https://docs.unity3d.com/ScriptReference/Transform.html">Transform</see> of the playspace object.</param>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        public WindowsMixedRealityDeviceManager(
            IMixedRealityServiceRegistrar registrar,
            IMixedRealityInputSystem inputSystem,
            MixedRealityInputSystemProfile inputSystemProfile,
            Transform playspace,
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null) : base(registrar, inputSystem, inputSystemProfile, playspace, name, priority, profile) { }

#if UNITY_WSA

        /// <summary>
        /// The max expected sources is two - two controllers and/or two hands.
        /// We'll set it to 20 just to be certain we can't run out of sources.
        /// </summary>
        public const int MaxInteractionSourceStates = 20;

        /// <summary>
        /// Dictionary to capture all active controllers detected
        /// </summary>
        private readonly Dictionary<uint, IMixedRealityController> activeControllers = new Dictionary<uint, IMixedRealityController>();

        /// <summary>
        /// Cache of the states captured from the Unity InteractionManager for UWP
        /// </summary>
        InteractionSourceState[] interactionmanagerStates = new InteractionSourceState[MaxInteractionSourceStates];

        /// <summary>
        /// The number of states captured most recently
        /// </summary>
        private int numInteractionManagerStates;

        /// <summary>
        /// The current source state reading for the Unity InteractionManager for UWP
        /// </summary>
        public InteractionSourceState[] LastInteractionManagerStateReading { get; protected set; }

        /// <inheritdoc/>
        public override IMixedRealityController[] GetActiveControllers()
        {
            return activeControllers.Values.ToArray();
        }

        #region Gesture Settings

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
                if (gestureRecognizer == null)
                {
                    gestureRecognizerEnabled = false;
                    return;
                }
                gestureRecognizerEnabled = value;
                if (!Application.isPlaying) { return; }

                if (!gestureRecognizer.IsCapturingGestures() && gestureRecognizerEnabled)
                {
                    NavigationRecognizerEnabled = false;
                    gestureRecognizer.StartCapturingGestures();
                }

                if (gestureRecognizer.IsCapturingGestures() && !gestureRecognizerEnabled)
                {
                    gestureRecognizer.CancelGestures();
                }
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
                if (navigationGestureRecognizer == null)
                {
                    navigationRecognizerEnabled = false;
                    return;
                }

                navigationRecognizerEnabled = value;

                if (!Application.isPlaying) { return; }

                if (!navigationGestureRecognizer.IsCapturingGestures() && navigationRecognizerEnabled)
                {
                    GestureRecognizerEnabled = false;
                    navigationGestureRecognizer.StartCapturingGestures();
                }

                if (navigationGestureRecognizer.IsCapturingGestures() && !navigationRecognizerEnabled)
                {
                    navigationGestureRecognizer.CancelGestures();
                }
            }
        }

        private static WindowsGestureSettings gestureSettings = WindowsGestureSettings.Hold | WindowsGestureSettings.ManipulationTranslate;

        /// <summary>
        /// Current Gesture Settings for the GestureRecognizer
        /// </summary>
        public static WindowsGestureSettings GestureSettings
        {
            get { return gestureSettings; }
            set
            {
                gestureSettings = value;

                if (Application.isPlaying)
                {
                    gestureRecognizer?.UpdateAndResetGestures(WSAGestureSettings);
                }
            }
        }

        private static WindowsGestureSettings navigationSettings = WindowsGestureSettings.NavigationX | WindowsGestureSettings.NavigationY | WindowsGestureSettings.NavigationZ;

        /// <summary>
        /// Current Navigation Gesture Recognizer Settings.
        /// </summary>
        public static WindowsGestureSettings NavigationSettings
        {
            get { return navigationSettings; }
            set
            {
                navigationSettings = value;

                if (Application.isPlaying)
                {
                    navigationGestureRecognizer?.UpdateAndResetGestures(WSANavigationSettings);
                }
            }
        }

        private static WindowsGestureSettings railsNavigationSettings = WindowsGestureSettings.NavigationRailsX | WindowsGestureSettings.NavigationRailsY | WindowsGestureSettings.NavigationRailsZ;

        /// <summary>
        /// Current Navigation Gesture Recognizer Rails Settings.
        /// </summary>
        public static WindowsGestureSettings RailsNavigationSettings
        {
            get { return railsNavigationSettings; }
            set
            {
                railsNavigationSettings = value;

                if (Application.isPlaying)
                {
                    navigationGestureRecognizer?.UpdateAndResetGestures(WSARailsNavigationSettings);
                }
            }
        }

        private static bool useRailsNavigation = true;

        /// <summary>
        /// Should the Navigation Gesture Recognizer use Rails?
        /// </summary>
        public static bool UseRailsNavigation
        {
            get { return useRailsNavigation; }
            set
            {
                useRailsNavigation = value;

                if (Application.isPlaying)
                {
                    navigationGestureRecognizer?.UpdateAndResetGestures(useRailsNavigation ? WSANavigationSettings : WSARailsNavigationSettings);
                }
            }
        }

        private MixedRealityInputAction holdAction = MixedRealityInputAction.None;
        private MixedRealityInputAction navigationAction = MixedRealityInputAction.None;
        private MixedRealityInputAction manipulationAction = MixedRealityInputAction.None;

        private static GestureRecognizer gestureRecognizer;
        private static WsaGestureSettings WSAGestureSettings => (WsaGestureSettings)gestureSettings;

        private static GestureRecognizer navigationGestureRecognizer;
        private static WsaGestureSettings WSANavigationSettings => (WsaGestureSettings)navigationSettings;
        private static WsaGestureSettings WSARailsNavigationSettings => (WsaGestureSettings)railsNavigationSettings;

        #endregion Gesture Settings

        #region IMixedRealityDeviceManager Interface

        /// <inheritdoc/>
        public override void Enable()
        {
            if (!Application.isPlaying) { return; }

            if (InputSystemProfile == null) { return; }

            IMixedRealityInputSystem inputSystem = Service as IMixedRealityInputSystem;

            RegisterGestureEvents();
            RegisterNavigationEvents();

            if ((inputSystem != null) &&
                InputSystemProfile.GesturesProfile != null)
            {
                var gestureProfile = InputSystemProfile.GesturesProfile;
                GestureSettings = gestureProfile.ManipulationGestures;
                NavigationSettings = gestureProfile.NavigationGestures;
                RailsNavigationSettings = gestureProfile.RailsNavigationGestures;
                UseRailsNavigation = gestureProfile.UseRailsNavigation;

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
                    }
                }
            }

            InteractionManager.InteractionSourceDetected += InteractionManager_InteractionSourceDetected;
            InteractionManager.InteractionSourceLost += InteractionManager_InteractionSourceLost;
            InteractionManager.InteractionSourcePressed += InteractionManager_InteractionSourcePressed;
            InteractionManager.InteractionSourceReleased += InteractionManager_InteractionSourceReleased;

            numInteractionManagerStates = InteractionManager.GetCurrentReading(interactionmanagerStates);

            // Avoids a Unity Editor bug detecting a controller from the previous run during the first frame
#if !UNITY_EDITOR
            // NOTE: We update the source state data, in case an app wants to query it on source detected.
            for (var i = 0; i < numInteractionManagerStates; i++)
            {
                var controller = GetController(interactionmanagerStates[i].source);

                if (controller != null)
                {
                    controller.UpdateController(interactionmanagerStates[i]);
                    inputSystem?.RaiseSourceDetected(controller.InputSource, controller);
                }
            }
#endif

            if ((inputSystem != null) &&
                InputSystemProfile.GesturesProfile != null &&
                InputSystemProfile.GesturesProfile.WindowsGestureAutoStart == AutoStartBehavior.AutoStart)
            {
                GestureRecognizerEnabled = true;
            }
        }

        /// <inheritdoc/>
        public override void Update()
        {
            base.Update();

            numInteractionManagerStates = InteractionManager.GetCurrentReading(interactionmanagerStates);

            for (var i = 0; i < numInteractionManagerStates; i++)
            {
                // SourceDetected gets raised when a new controller is detected and, if previously present, 
                // when OnEnable is called. Do not create a new controller here.
                var controller = GetController(interactionmanagerStates[i].source, false);

                if (controller != null)
                {
                    controller.UpdateController(interactionmanagerStates[i]);
                }
            }

            LastInteractionManagerStateReading = interactionmanagerStates;
        }

        private void RegisterGestureEvents()
        {
            if (gestureRecognizer == null)
            {
                gestureRecognizer = new GestureRecognizer();
            }

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
            InteractionManager.InteractionSourceLost -= InteractionManager_InteractionSourceLost;
            InteractionManager.InteractionSourcePressed -= InteractionManager_InteractionSourcePressed;
            InteractionManager.InteractionSourceReleased -= InteractionManager_InteractionSourceReleased;

            InteractionSourceState[] states = InteractionManager.GetCurrentReading();
            for (var i = 0; i < states.Length; i++)
            {
                RemoveController(states[i].source);
            }
        }

        #endregion IMixedRealityDeviceManager Interface

        #region Controller Utilities

        /// <summary>
        /// Retrieve the source controller from the Active Store, or create a new device and register it
        /// </summary>
        /// <param name="interactionSource">Source State provided by the SDK</param>
        /// <param name="addController">Should the Source be added as a controller if it isn't found?</param>
        /// <returns>New or Existing Controller Input Source</returns>
        private WindowsMixedRealityController GetController(InteractionSource interactionSource, bool addController = true)
        {
            //If a device is already registered with the ID provided, just return it.
            if (activeControllers.ContainsKey(interactionSource.id))
            {
                var controller = activeControllers[interactionSource.id] as WindowsMixedRealityController;
                Debug.Assert(controller != null);
                return controller;
            }

            if (!addController) { return null; }

            Handedness controllingHand;
            switch (interactionSource.handedness)
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


            IMixedRealityInputSystem inputSystem = Service as IMixedRealityInputSystem;

            IMixedRealityPointer[] pointers = null;
            InputSourceType inputSourceType = InputSourceType.Other;
            switch(interactionSource.kind)
            {
                case InteractionSourceKind.Controller:
                    pointers = RequestPointers(SupportedControllerType.WindowsMixedReality, controllingHand);
                    inputSourceType = InputSourceType.Controller;
                    break;
                case InteractionSourceKind.Hand:
                    if(interactionSource.supportsPointing)
                    {
                        pointers = RequestPointers(SupportedControllerType.ArticulatedHand, controllingHand);
                    }
                    else
                    {
                        pointers = RequestPointers(SupportedControllerType.GGVHand, controllingHand);
                    }
                    inputSourceType = InputSourceType.Hand;
                    break;
                case InteractionSourceKind.Voice:
                    // set to null: when pointers are null we use head gaze, which is what we want for voice
                    break;
                default:
                    Debug.LogError($"Unknown new type in WindowsMixedRealityDeviceManager {interactionSource.kind}, make sure to add a SupportedControllerType");
                    break;

            }

            string nameModifier = controllingHand == Handedness.None ? interactionSource.kind.ToString() : controllingHand.ToString();
            var inputSource = inputSystem?.RequestNewGenericInputSource($"Mixed Reality Controller {nameModifier}", pointers, inputSourceType);

            WindowsMixedRealityController detectedController;
            if (interactionSource.kind == InteractionSourceKind.Hand)
            {
                if (interactionSource.supportsPointing)
                {
                    detectedController = new WindowsMixedRealityArticulatedHand(TrackingState.NotTracked, controllingHand, inputSource);
                    if (!detectedController.SetupConfiguration(typeof(WindowsMixedRealityArticulatedHand), inputSourceType))
                    {
                        // Controller failed to be setup correctly.
                        // Return null so we don't raise the source detected.
                        return null;
                    }
                }
                else
                {
                    detectedController = new WindowsMixedRealityGGVHand(TrackingState.NotTracked, controllingHand, inputSource);
                    if (!detectedController.SetupConfiguration(typeof(WindowsMixedRealityGGVHand), inputSourceType))
                    {
                        // Controller failed to be setup correctly.
                        // Return null so we don't raise the source detected.
                        return null;
                    }

                }

            }
            else
            {
                detectedController = new WindowsMixedRealityController(TrackingState.NotTracked, controllingHand, inputSource);

                if (!detectedController.SetupConfiguration(typeof(WindowsMixedRealityController), inputSourceType))
                {
                    // Controller failed to be setup correctly.
                    // Return null so we don't raise the source detected.
                    return null;
                }
            }

            for (int i = 0; i < detectedController.InputSource?.Pointers?.Length; i++)
            {
                detectedController.InputSource.Pointers[i].Controller = detectedController;
            }

            activeControllers.Add(interactionSource.id, detectedController);
            return detectedController;
        }

        /// <summary>
        /// Remove the selected controller from the Active Store
        /// </summary>
        /// <param name="interactionSourceState">Source State provided by the SDK to remove</param>
        private void RemoveController(InteractionSource interactionSource)
        {
            var controller = GetController(interactionSource, false);

            if (controller != null)
            {
                IMixedRealityInputSystem inputSystem = Service as IMixedRealityInputSystem;

                inputSystem?.RaiseSourceLost(controller.InputSource, controller);

                foreach (IMixedRealityPointer pointer in controller.InputSource.Pointers)
                {
                    if (pointer != null)
                    {
                        pointer.Controller = null;
                    }
                }
            }

            activeControllers.Remove(interactionSource.id);
        }

        #endregion Controller Utilities

        #region Unity InteractionManager Events

        /// <summary>
        /// SDK Interaction Source Detected Event handler
        /// </summary>
        /// <param name="args">SDK source detected event arguments</param>
        private void InteractionManager_InteractionSourceDetected(InteractionSourceDetectedEventArgs args)
        {

            // Avoids a Unity Editor bug detecting a controller from the previous run during the first frame
#if UNITY_EDITOR
            if (Time.frameCount <= 1)
            {
                return;
            }
#endif

            bool raiseSourceDetected = !activeControllers.ContainsKey(args.state.source.id);

            var controller = GetController(args.state.source);

            if (controller != null && raiseSourceDetected)
            {
                IMixedRealityInputSystem inputSystem = Service as IMixedRealityInputSystem;
                inputSystem?.RaiseSourceDetected(controller.InputSource, controller);
            }

            controller?.UpdateController(args.state);
        }

        /// <summary>
        /// SDK Interaction Source Pressed Event handler. Used only for voice.
        /// </summary>
        /// <param name="args">SDK source pressed event arguments</param>
        private void InteractionManager_InteractionSourcePressed(InteractionSourcePressedEventArgs args)
        {
            if (args.state.source.kind == InteractionSourceKind.Voice)
            {
                GetController(args.state.source)?.UpdateController(args.state);
            }
        }

        /// <summary>
        /// SDK Interaction Source Released Event handler. Used only for voice.
        /// </summary>
        /// <param name="args">SDK source released event arguments</param>
        private void InteractionManager_InteractionSourceReleased(InteractionSourceReleasedEventArgs args)
        {
            if (args.state.source.kind == InteractionSourceKind.Voice)
            {
                GetController(args.state.source)?.UpdateController(args.state);
            }
        }

        /// <summary>
        /// SDK Interaction Source Lost Event handler
        /// </summary>
        /// <param name="args">SDK source updated event arguments</param>
        private void InteractionManager_InteractionSourceLost(InteractionSourceLostEventArgs args)
        {
            RemoveController(args.state.source);
        }

        #endregion Unity InteractionManager Events

        #region Gesture Recognizer Events

        private void GestureRecognizer_HoldStarted(HoldStartedEventArgs args)
        {
            var controller = GetController(args.source, false);
            if (controller != null)
            {
                IMixedRealityInputSystem inputSystem = Service as IMixedRealityInputSystem;
                inputSystem?.RaiseGestureStarted(controller, holdAction);
            }
        }

        private void GestureRecognizer_HoldCompleted(HoldCompletedEventArgs args)
        {
            var controller = GetController(args.source, false);
            if (controller != null)
            {
                IMixedRealityInputSystem inputSystem = Service as IMixedRealityInputSystem;
                inputSystem.RaiseGestureCompleted(controller, holdAction);
            }
        }

        private void GestureRecognizer_HoldCanceled(HoldCanceledEventArgs args)
        {
            var controller = GetController(args.source, false);
            if (controller != null)
            {
                IMixedRealityInputSystem inputSystem = Service as IMixedRealityInputSystem;
                inputSystem.RaiseGestureCanceled(controller, holdAction);
            }
        }

        private void GestureRecognizer_ManipulationStarted(ManipulationStartedEventArgs args)
        {
            var controller = GetController(args.source, false);
            if (controller != null)
            {
                IMixedRealityInputSystem inputSystem = Service as IMixedRealityInputSystem;
                inputSystem.RaiseGestureStarted(controller, manipulationAction);
            }
        }

        private void GestureRecognizer_ManipulationUpdated(ManipulationUpdatedEventArgs args)
        {
            var controller = GetController(args.source, false);
            if (controller != null)
            {
                IMixedRealityInputSystem inputSystem = Service as IMixedRealityInputSystem;
                inputSystem.RaiseGestureUpdated(controller, manipulationAction, args.cumulativeDelta);
            }
        }

        private void GestureRecognizer_ManipulationCompleted(ManipulationCompletedEventArgs args)
        {
            var controller = GetController(args.source, false);
            if (controller != null)
            {
                IMixedRealityInputSystem inputSystem = Service as IMixedRealityInputSystem;
                inputSystem.RaiseGestureCompleted(controller, manipulationAction, args.cumulativeDelta);
            }
        }

        private void GestureRecognizer_ManipulationCanceled(ManipulationCanceledEventArgs args)
        {
            var controller = GetController(args.source, false);
            if (controller != null)
            {
                IMixedRealityInputSystem inputSystem = Service as IMixedRealityInputSystem;
                inputSystem.RaiseGestureCanceled(controller, manipulationAction);
            }
        }

        #endregion Gesture Recognizer Events

        #region Navigation Recognizer Events

        private void NavigationGestureRecognizer_NavigationStarted(NavigationStartedEventArgs args)
        {
            var controller = GetController(args.source, false);
            if (controller != null)
            {
                IMixedRealityInputSystem inputSystem = Service as IMixedRealityInputSystem;
                inputSystem.RaiseGestureStarted(controller, navigationAction);
            }
        }

        private void NavigationGestureRecognizer_NavigationUpdated(NavigationUpdatedEventArgs args)
        {
            var controller = GetController(args.source, false);
            if (controller != null)
            {
                IMixedRealityInputSystem inputSystem = Service as IMixedRealityInputSystem;
                inputSystem.RaiseGestureUpdated(controller, navigationAction, args.normalizedOffset);
            }
        }

        private void NavigationGestureRecognizer_NavigationCompleted(NavigationCompletedEventArgs args)
        {
            var controller = GetController(args.source, false);
            if (controller != null)
            {
                IMixedRealityInputSystem inputSystem = Service as IMixedRealityInputSystem;
                inputSystem.RaiseGestureCompleted(controller, navigationAction, args.normalizedOffset);
            }
        }

        private void NavigationGestureRecognizer_NavigationCanceled(NavigationCanceledEventArgs args)
        {
            var controller = GetController(args.source, false);
            if (controller != null)
            {
                IMixedRealityInputSystem inputSystem = Service as IMixedRealityInputSystem;
                inputSystem.RaiseGestureCanceled(controller, navigationAction);
            }
        }

        #endregion Navigation Recognizer Events

#endif // UNITY_WSA

    }
}
