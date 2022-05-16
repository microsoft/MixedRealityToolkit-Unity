﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Windows.Input;
using Microsoft.MixedReality.Toolkit.Windows.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

#if HP_CONTROLLER_ENABLED
using Microsoft.MixedReality.Input;
using MotionControllerHandedness = Microsoft.MixedReality.Input.Handedness;
using Handedness = Microsoft.MixedReality.Toolkit.Utilities.Handedness;
#endif

#if UNITY_WSA
using System.Linq;
using Unity.Profiling;
using UnityEngine.XR.WSA.Input;
using WsaGestureSettings = UnityEngine.XR.WSA.Input.GestureSettings;
#endif // UNITY_WSA

#if WINDOWS_UWP
using Windows.Perception;
using Windows.Perception.People;
using Windows.UI.Input.Spatial;
#elif UNITY_WSA && DOTNETWINRT_PRESENT
using Microsoft.Windows.Perception;
using Microsoft.Windows.Perception.People;
using Microsoft.Windows.UI.Input.Spatial;
#endif

namespace Microsoft.MixedReality.Toolkit.WindowsMixedReality.Input
{
    [MixedRealityDataProvider(
        typeof(IMixedRealityInputSystem),
        SupportedPlatforms.WindowsUniversal,
        "Windows Mixed Reality Device Manager",
        supportedUnityXRPipelines: SupportedUnityXRPipelines.LegacyXR)]
    public class WindowsMixedRealityDeviceManager : BaseInputDeviceManager, IMixedRealityCapabilityCheck
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="registrar">The <see cref="IMixedRealityServiceRegistrar"/> instance that loaded the data provider.</param>
        /// <param name="inputSystem">The <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem"/> instance that receives data from this provider.</param>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        [Obsolete("This constructor is obsolete (registrar parameter is no longer required) and will be removed in a future version of the Microsoft Mixed Reality Toolkit.")]
        public WindowsMixedRealityDeviceManager(
            IMixedRealityServiceRegistrar registrar,
            IMixedRealityInputSystem inputSystem,
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null) : this(inputSystem, name, priority, profile)
        {
            Registrar = registrar;
        }

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

        #region IMixedRealityCapabilityCheck Implementation

        /// <inheritdoc />
        public bool CheckCapability(MixedRealityCapability capability)
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
            else
            {
                if (!UnityEngine.XR.WSA.HolographicSettings.IsDisplayOpaque)
                {
                    // HoloLens supports GGV hands
                    return (capability == MixedRealityCapability.GGVHand);
                }
                else
                {
                    // Windows Mixed Reality immersive devices support motion controllers
                    return (capability == MixedRealityCapability.MotionController);
                }
            }

            return false;
        }

        #endregion IMixedRealityCapabilityCheck Implementation

#if HP_CONTROLLER_ENABLED
        private MotionControllerWatcher motionControllerWatcher;

        /// <summary>
        /// Dictionary to capture all active HP controllers detected
        /// </summary>
        private readonly Dictionary<uint, MotionControllerState> trackedMotionControllerStates = new Dictionary<uint, MotionControllerState>();
#endif

#if UNITY_WSA
        /// <summary>
        /// The initial size of interactionmanagerStates.
        /// </summary>
        /// <remarks>
        /// <para>This value is arbitrary but chosen to be a number larger than the typical expected number (to avoid
        /// having to do further allocations).</para>
        /// </remarks>
        public const int MaxInteractionSourceStates = 20;

        /// <summary>
        /// This number controls how much the interactionmanagerStates array should grow by each time it must
        /// be resized (larger) in order to accommodate more InteractionSourceState values.
        /// </summary>
        /// <remarks>
        /// This must be a value greater than 1.
        /// </remarks>
        private const int InteractionManagerStatesGrowthFactor = 2;

        /// <summary>
        /// Dictionary to capture all active controllers detected
        /// </summary>
        private readonly Dictionary<uint, IMixedRealityController> activeControllers = new Dictionary<uint, IMixedRealityController>();

        /// <summary>
        /// Cache of the states captured from the Unity InteractionManager for UWP
        /// </summary>
        InteractionSourceState[] interactionManagerStates = new InteractionSourceState[MaxInteractionSourceStates];

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

                if (Application.isPlaying && !useRailsNavigation)
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

                if (Application.isPlaying && useRailsNavigation)
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
                    navigationGestureRecognizer?.UpdateAndResetGestures(useRailsNavigation ? WSARailsNavigationSettings : WSANavigationSettings);
                }
            }
        }

        private MixedRealityInputAction holdAction = MixedRealityInputAction.None;
        private MixedRealityInputAction navigationAction = MixedRealityInputAction.None;
        private MixedRealityInputAction manipulationAction = MixedRealityInputAction.None;
        private MixedRealityInputAction selectAction = MixedRealityInputAction.None;

        private static GestureRecognizer gestureRecognizer;
        private static WsaGestureSettings WSAGestureSettings => (WsaGestureSettings)gestureSettings;

        private static GestureRecognizer navigationGestureRecognizer;
        private static WsaGestureSettings WSANavigationSettings => (WsaGestureSettings)navigationSettings;
        private static WsaGestureSettings WSARailsNavigationSettings => (WsaGestureSettings)railsNavigationSettings;

        #endregion Gesture Settings

        #region IMixedRealityDeviceManager Interface

#if (UNITY_WSA && DOTNETWINRT_PRESENT) || WINDOWS_UWP
        private IMixedRealityGazeProviderHeadOverride mixedRealityGazeProviderHeadOverride = null;
#endif // (UNITY_WSA && DOTNETWINRT_PRESENT) || WINDOWS_UWP

        /// <inheritdoc/>
        public override void Enable()
        {
            if (!Application.isPlaying) { return; }

            if (InputSystemProfile == null) { return; }

#if (UNITY_WSA && DOTNETWINRT_PRESENT) || WINDOWS_UWP
            if (WindowsMixedRealityUtilities.UtilitiesProvider == null)
            {
                WindowsMixedRealityUtilities.UtilitiesProvider = new WindowsMixedRealityUtilitiesProvider();
            }

            mixedRealityGazeProviderHeadOverride = Service?.GazeProvider as IMixedRealityGazeProviderHeadOverride;
#endif // (UNITY_WSA && DOTNETWINRT_PRESENT) || WINDOWS_UWP

#if HP_CONTROLLER_ENABLED
            // Listens to events to track the HP Motion Controller
            motionControllerWatcher = new MotionControllerWatcher();
            motionControllerWatcher.MotionControllerAdded += AddTrackedMotionController;
            motionControllerWatcher.MotionControllerRemoved += RemoveTrackedMotionController;
            var nowait = motionControllerWatcher.StartAsync();
#endif

            if (InputSystemProfile.GesturesProfile != null)
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
                        case GestureInputType.Select:
                            selectAction = gesture.Action;
                            break;
                    }
                }
            }

            RegisterGestureEvents();
            RegisterNavigationEvents();

            InteractionManager.InteractionSourceDetected += InteractionManager_InteractionSourceDetected;
            InteractionManager.InteractionSourceLost += InteractionManager_InteractionSourceLost;
            InteractionManager.InteractionSourcePressed += InteractionManager_InteractionSourcePressed;
            InteractionManager.InteractionSourceReleased += InteractionManager_InteractionSourceReleased;

            UpdateInteractionManagerReading();

            // NOTE: We update the source state data, in case an app wants to query it on source detected.
            for (var i = 0; i < numInteractionManagerStates; i++)
            {
                GetOrAddController(interactionManagerStates[i]);
            }

            if (InputSystemProfile.GesturesProfile != null &&
                InputSystemProfile.GesturesProfile.WindowsGestureAutoStart == AutoStartBehavior.AutoStart)
            {
                GestureRecognizerEnabled = true;
            }

            // Call the base here to ensure any early exits do not
            // artificially declare the service as enabled.
            base.Enable();
        }

        private static readonly ProfilerMarker GetOrAddControllerPerfMarker = new ProfilerMarker("[MRTK] WindowsMixedRealityDeviceManager.GetOrAddController");

        private void GetOrAddController(InteractionSourceState interactionSourceState)
        {
            using (GetOrAddControllerPerfMarker.Auto())
            {
                // If this is a new detected controller, raise source detected event with input system
                // check needs to be here because GetOrAddController adds it to the activeControllers Dictionary
                // this could be cleaned up because that's not clear
                bool raiseSourceDetected = !activeControllers.ContainsKey(interactionSourceState.source.id);

                var controller = GetOrAddController(interactionSourceState.source);

                if (controller != null)
                {
                    if (raiseSourceDetected)
                    {
                        Service?.RaiseSourceDetected(controller.InputSource, controller);
                    }

                    controller.UpdateController(interactionSourceState);
                }
            }
        }

        private static readonly ProfilerMarker UpdatePerfMarker = new ProfilerMarker("[MRTK] WindowsMixedRealityDeviceManager.Update");

        /// <inheritdoc/>
        public override void Update()
        {
            using (UpdatePerfMarker.Auto())
            {
                base.Update();

#if (UNITY_WSA && DOTNETWINRT_PRESENT) || WINDOWS_UWP
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
#endif // (UNITY_WSA && DOTNETWINRT_PRESENT) || WINDOWS_UWP

                UpdateInteractionManagerReading();

                for (var i = 0; i < numInteractionManagerStates; i++)
                {
                    // SourceDetected gets raised when a new controller is detected and, if previously present, 
                    // when OnEnable is called. Do not create a new controller here.
                    var controller = GetOrAddController(interactionManagerStates[i].source, false);

                    if (controller != null)
                    {
                        controller.UpdateController(interactionManagerStates[i]);
                    }
                }

                LastInteractionManagerStateReading = interactionManagerStates;
            }
        }

        private void RegisterGestureEvents()
        {
            if (holdAction != MixedRealityInputAction.None ||
                manipulationAction != MixedRealityInputAction.None ||
                selectAction != MixedRealityInputAction.None)
            {
                if (gestureRecognizer == null)
                {
                    try
                    {
                        gestureRecognizer = new GestureRecognizer();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"Failed to create gesture recognizer. OS version might not support it. Exception: {ex}");
                        gestureRecognizer = null;
                        return;
                    }
                    gestureRecognizer.SetRecognizableGestures(WSAGestureSettings);
                }

                if (holdAction != MixedRealityInputAction.None)
                {
                    gestureRecognizer.HoldStarted += GestureRecognizer_HoldStarted;
                    gestureRecognizer.HoldCompleted += GestureRecognizer_HoldCompleted;
                    gestureRecognizer.HoldCanceled += GestureRecognizer_HoldCanceled;
                }

                if (manipulationAction != MixedRealityInputAction.None)
                {
                    gestureRecognizer.ManipulationStarted += GestureRecognizer_ManipulationStarted;
                    gestureRecognizer.ManipulationUpdated += GestureRecognizer_ManipulationUpdated;
                    gestureRecognizer.ManipulationCompleted += GestureRecognizer_ManipulationCompleted;
                    gestureRecognizer.ManipulationCanceled += GestureRecognizer_ManipulationCanceled;
                }

                if (selectAction != MixedRealityInputAction.None)
                {
                    gestureRecognizer.Tapped += GestureRecognizer_Tapped;
                }
            }
        }

        private void UnregisterGestureEvents()
        {
            if (gestureRecognizer == null) { return; }

            if (holdAction != MixedRealityInputAction.None)
            {
                gestureRecognizer.HoldStarted -= GestureRecognizer_HoldStarted;
                gestureRecognizer.HoldCompleted -= GestureRecognizer_HoldCompleted;
                gestureRecognizer.HoldCanceled -= GestureRecognizer_HoldCanceled;
            }

            if (manipulationAction != MixedRealityInputAction.None)
            {
                gestureRecognizer.ManipulationStarted -= GestureRecognizer_ManipulationStarted;
                gestureRecognizer.ManipulationUpdated -= GestureRecognizer_ManipulationUpdated;
                gestureRecognizer.ManipulationCompleted -= GestureRecognizer_ManipulationCompleted;
                gestureRecognizer.ManipulationCanceled -= GestureRecognizer_ManipulationCanceled;
            }

            if (selectAction != MixedRealityInputAction.None)
            {
                gestureRecognizer.Tapped -= GestureRecognizer_Tapped;
            }
        }

        private void RegisterNavigationEvents()
        {
            if (navigationAction != MixedRealityInputAction.None)
            {
                if (navigationGestureRecognizer == null)
                {
                    try
                    {
                        navigationGestureRecognizer = new GestureRecognizer();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"Failed to create gesture recognizer. OS version might not support it. Exception: {ex}");
                        navigationGestureRecognizer = null;
                        return;
                    }
                    navigationGestureRecognizer.SetRecognizableGestures(useRailsNavigation ? WSARailsNavigationSettings : WSANavigationSettings);
                }

                navigationGestureRecognizer.NavigationStarted += NavigationGestureRecognizer_NavigationStarted;
                navigationGestureRecognizer.NavigationUpdated += NavigationGestureRecognizer_NavigationUpdated;
                navigationGestureRecognizer.NavigationCompleted += NavigationGestureRecognizer_NavigationCompleted;
                navigationGestureRecognizer.NavigationCanceled += NavigationGestureRecognizer_NavigationCanceled;
            }
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
            gestureRecognizer = null;

            UnregisterNavigationEvents();
            navigationGestureRecognizer?.Dispose();
            navigationGestureRecognizer = null;

            InteractionManager.InteractionSourceDetected -= InteractionManager_InteractionSourceDetected;
            InteractionManager.InteractionSourceLost -= InteractionManager_InteractionSourceLost;
            InteractionManager.InteractionSourcePressed -= InteractionManager_InteractionSourcePressed;
            InteractionManager.InteractionSourceReleased -= InteractionManager_InteractionSourceReleased;

            InteractionSourceState[] states = InteractionManager.GetCurrentReading();
            for (var i = 0; i < states.Length; i++)
            {
                RemoveController(states[i].source);
            }

            base.Disable();
        }

        #endregion IMixedRealityDeviceManager Interface

        #region Controller Utilities

        /// <summary>
        /// Creates a unique key for the controller based on its vendor ID, product ID, version number, and handedness.
        /// </summary>
        private uint GetControllerId(uint vid, uint pid, uint version, uint handedness)
        {
            return (vid << 48) + (pid << 32) + (version << 16) + handedness;
        }

#if HP_CONTROLLER_ENABLED
        private uint GetControllerId(MotionController mc)
        {
            var handedness = ((uint)(mc.Handedness == MotionControllerHandedness.Right ? 2 : (mc.Handedness == MotionControllerHandedness.Left ? 1 : 0)));
            return GetControllerId(mc.VendorId, mc.ProductId, mc.Version, handedness);
        }
#endif

        private uint GetControllerId(InteractionSource interactionSource)
        {
            var handedness = ((uint)(interactionSource.handedness == InteractionSourceHandedness.Right ? 2 : (interactionSource.handedness == InteractionSourceHandedness.Left ? 1 : 0)));
            return GetControllerId(interactionSource.vendorId, interactionSource.productId, interactionSource.productVersion, handedness);
        }

        /// <summary>
        /// Retrieve the source controller from the Active Store, or create a new device and register it
        /// </summary>
        /// <param name="interactionSource">Source State provided by the SDK</param>
        /// <param name="addController">Should the Source be added as a controller if it isn't found?</param>
        /// <returns>New or Existing Controller Input Source</returns>
        private BaseWindowsMixedRealitySource GetOrAddController(InteractionSource interactionSource, bool addController = true)
        {
            uint controllerId = GetControllerId(interactionSource);
            // If a device is already registered with the ID provided, just return it.
            if (activeControllers.ContainsKey(controllerId))
            {
                var controller = activeControllers[controllerId] as BaseWindowsMixedRealitySource;
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

            IMixedRealityPointer[] pointers = null;
            InputSourceType inputSourceType = InputSourceType.Other;
            switch (interactionSource.kind)
            {
                case InteractionSourceKind.Controller:
                    if (interactionSource.supportsPointing)
                    {
                        pointers = RequestPointers(SupportedControllerType.WindowsMixedReality, controllingHand);
                    }
                    else
                    {
                        pointers = RequestPointers(SupportedControllerType.GGVHand, controllingHand);
                    }
                    inputSourceType = InputSourceType.Controller;
                    break;
                case InteractionSourceKind.Hand:
                    if (interactionSource.supportsPointing)
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

            bool isHPController = !interactionSource.supportsTouchpad && interactionSource.kind == InteractionSourceKind.Controller;

            string kindModifier = interactionSource.kind.ToString();
            string handednessModifier = controllingHand == Handedness.None ? string.Empty : controllingHand.ToString();

            string inputSourceName = isHPController ? $"HP Motion {kindModifier} {handednessModifier}" : $"Mixed Reality {kindModifier} {handednessModifier}";

            var inputSource = Service?.RequestNewGenericInputSource(inputSourceName, pointers, inputSourceType);

            BaseWindowsMixedRealitySource detectedController;
            if (interactionSource.supportsPointing)
            {
                if (interactionSource.kind == InteractionSourceKind.Hand)
                {
                    detectedController = new WindowsMixedRealityArticulatedHand(TrackingState.NotTracked, controllingHand, inputSource);
                    if (!detectedController.Enabled)
                    {
                        // Controller failed to be setup correctly.
                        // Return null so we don't raise the source detected.
                        return null;
                    }
                }
                else if (interactionSource.kind == InteractionSourceKind.Controller)
                {
                    if (isHPController)
                    {
                        // Add the controller as a HP Motion Controller
                        HPMotionController hpController = new HPMotionController(TrackingState.NotTracked, controllingHand, inputSource);

#if HP_CONTROLLER_ENABLED
                        lock (trackedMotionControllerStates)
                        {
                            if (trackedMotionControllerStates.ContainsKey(controllerId))
                            {
                                hpController.MotionControllerState = trackedMotionControllerStates[controllerId];
                            }
                        }
#endif

                        detectedController = hpController;
                    }
                    else
                    {
                        detectedController = new WindowsMixedRealityController(TrackingState.NotTracked, controllingHand, inputSource);
                    }
                }
                else
                {
                    Debug.Log($"Unhandled source type {interactionSource.kind} detected.");
                    return null;
                }
            }
            else
            {
                detectedController = new WindowsMixedRealityGGVHand(TrackingState.NotTracked, controllingHand, inputSource);
            }

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

            activeControllers.Add(controllerId, detectedController);
            return detectedController;
        }

        /// <summary>
        /// Remove the selected controller from the Active Store
        /// </summary>
        /// <param name="interactionSourceState">Source State provided by the SDK to remove</param>
        private void RemoveController(InteractionSource interactionSource)
        {
            var controller = GetOrAddController(interactionSource, false);
            var controllerId = GetControllerId(interactionSource);

            if (controller != null)
            {
                RemoveControllerFromScene(controller);
                activeControllers.Remove(controllerId);
            }
        }

        /// <summary>
        /// Removes the controller from the scene and handles any additional cleanup
        /// </summary>
        private void RemoveControllerFromScene(BaseWindowsMixedRealitySource controller)
        {
            Service?.RaiseSourceLost(controller.InputSource, controller);

            RecyclePointers(controller.InputSource);

            var visualizer = controller.Visualizer;

            if (!visualizer.IsNull() &&
                visualizer.GameObjectProxy != null)
            {
                visualizer.GameObjectProxy.SetActive(false);
            }
        }

        [Obsolete("This function exists to workaround a bug in Unity and will be removed in an upcoming release. For more details, see https://github.com/microsoft/MixedRealityToolkit-Unity/pull/8101")]
        public void RemoveControllerForSuspendWorkaround(InteractionSource interactionSource)
        {
            RemoveController(interactionSource);
        }


#if HP_CONTROLLER_ENABLED
        private void AddTrackedMotionController(object sender, MotionController motionController)
        {
            lock (trackedMotionControllerStates)
            {
                uint controllerId = GetControllerId(motionController);
                trackedMotionControllerStates[controllerId] = new MotionControllerState(motionController);

                if (activeControllers.ContainsKey(controllerId) && activeControllers[controllerId] is HPMotionController hpController)
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

                if (activeControllers.ContainsKey(controllerId) && activeControllers[controllerId] is HPMotionController hpController)
                {
                    hpController.MotionControllerState = null;
                }
            }
        }
#endif

        #endregion Controller Utilities

        #region Unity InteractionManager Events

        /// <summary>
        /// SDK Interaction Source Detected Event handler
        /// </summary>
        /// <param name="args">SDK source detected event arguments</param>
        private void InteractionManager_InteractionSourceDetected(InteractionSourceDetectedEventArgs args) => GetOrAddController(args.state);

        /// <summary>
        /// SDK Interaction Source Pressed Event handler. Used only for voice.
        /// </summary>
        /// <param name="args">SDK source pressed event arguments</param>
        private void InteractionManager_InteractionSourcePressed(InteractionSourcePressedEventArgs args)
        {
            if (args.state.source.kind == InteractionSourceKind.Voice)
            {
                var controller = GetOrAddController(args.state.source);
                if (controller != null)
                {
                    controller.UpdateController(args.state);
                    // On WMR, the voice recognizer does not actually register the phrase 'select'
                    // when you add it to the speech commands profile. Therefore, simulate
                    // the "select" voice command running to ensure that we get a select voice command
                    // registered. This is used by FocusProvider to detect when the select pointer is active
                    Service?.RaiseSpeechCommandRecognized(controller.InputSource, RecognitionConfidenceLevel.High, TimeSpan.MinValue, DateTime.Now, new SpeechCommands("select", KeyCode.Alpha1, MixedRealityInputAction.None));
                }
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
                GetOrAddController(args.state.source)?.UpdateController(args.state);
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
            var controller = GetOrAddController(args.source, false);
            if (controller != null)
            {
                Service?.RaiseGestureStarted(controller, holdAction);
            }
        }

        private void GestureRecognizer_HoldCompleted(HoldCompletedEventArgs args)
        {
            var controller = GetOrAddController(args.source, false);
            if (controller != null)
            {
                Service?.RaiseGestureCompleted(controller, holdAction);
            }
        }

        private void GestureRecognizer_HoldCanceled(HoldCanceledEventArgs args)
        {
            var controller = GetOrAddController(args.source, false);
            if (controller != null)
            {
                Service?.RaiseGestureCanceled(controller, holdAction);
            }
        }

        private void GestureRecognizer_ManipulationStarted(ManipulationStartedEventArgs args)
        {
            var controller = GetOrAddController(args.source, false);
            if (controller != null)
            {
                Service?.RaiseGestureStarted(controller, manipulationAction);
            }
        }

        private void GestureRecognizer_ManipulationUpdated(ManipulationUpdatedEventArgs args)
        {
            var controller = GetOrAddController(args.source, false);
            if (controller != null)
            {
                Service?.RaiseGestureUpdated(controller, manipulationAction, args.cumulativeDelta);
            }
        }

        private void GestureRecognizer_ManipulationCompleted(ManipulationCompletedEventArgs args)
        {
            var controller = GetOrAddController(args.source, false);
            if (controller != null)
            {
                Service?.RaiseGestureCompleted(controller, manipulationAction, args.cumulativeDelta);
            }
        }

        private void GestureRecognizer_ManipulationCanceled(ManipulationCanceledEventArgs args)
        {
            var controller = GetOrAddController(args.source, false);
            if (controller != null)
            {
                Service?.RaiseGestureCanceled(controller, manipulationAction);
            }
        }

        private void GestureRecognizer_Tapped(TappedEventArgs args)
        {
            var controller = GetOrAddController(args.source, false);
            if (controller != null)
            {
                Service?.RaiseGestureCompleted(controller, selectAction);
            }
        }

        #endregion Gesture Recognizer Events

        #region Navigation Recognizer Events

        private void NavigationGestureRecognizer_NavigationStarted(NavigationStartedEventArgs args)
        {
            var controller = GetOrAddController(args.source, false);
            if (controller != null)
            {
                Service?.RaiseGestureStarted(controller, navigationAction);
            }
        }

        private void NavigationGestureRecognizer_NavigationUpdated(NavigationUpdatedEventArgs args)
        {
            var controller = GetOrAddController(args.source, false);
            if (controller != null)
            {
                Service?.RaiseGestureUpdated(controller, navigationAction, args.normalizedOffset);
            }
        }

        private void NavigationGestureRecognizer_NavigationCompleted(NavigationCompletedEventArgs args)
        {
            var controller = GetOrAddController(args.source, false);
            if (controller != null)
            {
                Service?.RaiseGestureCompleted(controller, navigationAction, args.normalizedOffset);
            }
        }

        private void NavigationGestureRecognizer_NavigationCanceled(NavigationCanceledEventArgs args)
        {
            var controller = GetOrAddController(args.source, false);
            if (controller != null)
            {
                Service?.RaiseGestureCanceled(controller, navigationAction);
            }
        }

        #endregion Navigation Recognizer Events

        #region Private Methods

        private static readonly ProfilerMarker UpdateInteractionManagerReadingPerfMarker = new ProfilerMarker("[MRTK] WindowsMixedRealityDeviceManager.UpdateInteractionManagerReading");

        /// <summary>
        /// Gets the latest interaction manager states and counts from InteractionManager
        /// </summary>
        /// <remarks>
        /// <para>Abstracts away some of the array resize handling and another underlying Unity issue
        /// when InteractionManager.GetCurrentReading is called when there are no detected sources.</para>
        /// </remarks>
        private void UpdateInteractionManagerReading()
        {
            using (UpdateInteractionManagerReadingPerfMarker.Auto())
            {
                int newSourceStateCount = InteractionManager.numSourceStates;
                // If there isn't enough space in the cache to hold the results, we should grow it so that it can, but also
                // grow it in a way that is unlikely to require re-allocations each time.
                if (newSourceStateCount > interactionManagerStates.Length)
                {
                    interactionManagerStates = new InteractionSourceState[newSourceStateCount * InteractionManagerStatesGrowthFactor];
                }

                // Note that InteractionManager.GetCurrentReading throws when invoked when the number of
                // source states is zero. In that case, we want to just update the number of read states to be zero.
                if (newSourceStateCount == 0)
                {
                    // clean up existing controllers that didn't trigger the InteractionSourceLost event.
                    // this can happen eg. when unity is registering cached controllers from a previous play session in the editor.
                    // those actually don't exist in the current session and therefor won't receive the InteractionSourceLost once  
                    // Unity's InteractionManager catches up
                    for (int i = 0; i < numInteractionManagerStates; ++i)
                    {
                        RemoveController(interactionManagerStates[i].source);
                    }

                    numInteractionManagerStates = newSourceStateCount;
                }
                else
                {
                    numInteractionManagerStates = InteractionManager.GetCurrentReading(interactionManagerStates);
                }
            }
        }

        #endregion Private Methods

#endif // UNITY_WSA

    }
}
