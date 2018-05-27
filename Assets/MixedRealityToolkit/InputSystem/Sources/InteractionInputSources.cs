// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.InputSystem.Pointers;
using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Extensions;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Managers;
using Microsoft.MixedReality.Toolkit.Internal.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_WSA
using UnityEngine.XR.WSA.Input;
#endif

namespace Microsoft.MixedReality.Toolkit.InputSystem.Sources
{
    /// <summary>
    /// Input sources for gestures and interaction source information from the WSA APIs, which gives access to various system-supported gestures
    /// and positional information for the various inputs that Windows gestures supports.
    /// This is mostly a wrapper on top of GestureRecognizer and InteractionManager.
    /// </summary>
    public class InteractionInputSources : MonoBehaviour
    {
        /// <summary>
        /// This enumeration gives the manager two different ways to handle the recognizer. Both will
        /// set up the recognizer. The first causes the recognizer to start
        /// immediately. The second allows the recognizer to be manually started at a later time.
        /// </summary>
        private enum RecognizerStartBehavior { AutoStart, ManualStart }

        [SerializeField]
        [Tooltip("Whether the recognizer should be activated on start.")]
        private RecognizerStartBehavior recognizerStart = RecognizerStartBehavior.AutoStart;

        [SerializeField]
        [Tooltip("Set to true to use the use rails (guides) for the navigation gesture, as opposed to full 3D navigation.")]
        private bool useRailsNavigation = false;

        [Serializable]
        private struct ControllerPointerOptions
        {
            [Tooltip("The Controller to assign the Pointer to.")]
            public Handedness TargetController;

            [Tooltip("The Pointer to assign.")]
            public GameObject PointerPrefab;

            public ControllerPointerOptions(Handedness targetController, GameObject pointerPrefab)
            {
                TargetController = targetController;
                PointerPrefab = pointerPrefab;
            }
        }

        [SerializeField]
        [Tooltip("Set custom pointers for your controllers.")]
        private ControllerPointerOptions[] pointerOptions = new ControllerPointerOptions[0];

        private bool delayInitialization = true;

#if UNITY_WSA
        protected GestureRecognizer GestureRecognizer;
        protected GestureRecognizer NavigationGestureRecognizer;
#endif

        /// <summary>
        /// Dictionary linking each source ID to its data.
        /// </summary>
        private static readonly HashSet<InteractionInputSource> InteractionInputSourceList = new HashSet<InteractionInputSource>();

        private IMixedRealityInputSystem inputSystem;

        #region IMixedRealityInputSource Capabilities and GenericInputPointingSource

        private class InteractionInputSource : BaseGenericInputSource
        {
#if UNITY_WSA
            public readonly InteractionSource Source;
            public readonly BaseControllerPointer[] PointerSceneObjects;

            public InteractionInputSource(InteractionSource source, string name, BaseControllerPointer[] pointerSceneObjects, IMixedRealityPointer[] pointers)
                : base(name, pointers)
            {
                Source = source;
                PointerSceneObjects = pointerSceneObjects;
                foreach (var pointer in pointerSceneObjects)
                {
                    pointer.InputSourceParent = this;
                }
            }
#else
            public InteractionInputSource() : base(string.Empty, null) { }
#endif

            private static DeviceInputType GetSupportFlag<TReading>(SourceCapability<TReading> capability, DeviceInputType flagIfSupported)
            {
                return capability.IsSupported ? flagIfSupported : DeviceInputType.None;
            }

            public void Reset()
            {
                ThumbstickPositionUpdated = false;
                TouchpadPositionUpdated = false;
                TouchpadTouchedUpdated = false;
                PointerPositionUpdated = false;
                PointerRotationUpdated = false;
                GripPositionUpdated = false;
                GripRotationUpdated = false;
                SelectPressedAmountUpdated = false;
            }

            public SourceCapability<Vector3> PointerPosition;
            public SourceCapability<Quaternion> PointerRotation;
            public SourceCapability<Ray> PointingRay;
            public SourceCapability<Vector3> GripPosition;
            public SourceCapability<Quaternion> GripRotation;
            public SourceCapability<AxisButton2D> Thumbstick;
            public SourceCapability<TouchpadData> Touchpad;
            public SourceCapability<AxisButton1D> Select;
            public SourceCapability<bool> Grasp;
            public SourceCapability<bool> Menu;

            public bool ThumbstickPositionUpdated;
            public bool TouchpadPositionUpdated;
            public bool TouchpadTouchedUpdated;
            public bool PointerPositionUpdated;
            public bool PointerRotationUpdated;
            public bool GripPositionUpdated;
            public bool GripRotationUpdated;
            public bool SelectPressedAmountUpdated;

            public bool TryGetPointerPosition(IMixedRealityPointer pointer, out Vector3 position)
            {
                position = Vector3.zero;
                if (PointerPosition.IsSupported && PointerPosition.IsAvailable)
                {
                    position = PointerPosition.CurrentReading;
                    return true;
                }

                return false;
            }

            public bool TryGetPointerRotation(IMixedRealityPointer pointer, out Quaternion rotation)
            {
                rotation = Quaternion.identity;
                if (PointerRotation.IsSupported && PointerRotation.IsAvailable)
                {
                    rotation = PointerRotation.CurrentReading;
                    return true;
                }

                return false;
            }

            public bool TryGetPointingRay(IMixedRealityPointer pointer, out Ray pointingRay)
            {
                pointingRay = default(Ray);
                if (PointingRay.IsSupported && PointingRay.IsAvailable)
                {
                    pointingRay = PointingRay.CurrentReading;
                    return true;
                }

                return false;
            }
        }

        private struct SourceCapability<TReading>
        {
            public bool IsSupported;
            public bool IsAvailable;
            public TReading CurrentReading;
        }

        private struct AxisButton1D
        {
            public bool Pressed;
            public double PressedAmount;
        }

        private struct AxisButton2D
        {
            public bool Pressed;
            public Vector2 Position;
        }

        private struct TouchpadData
        {
            public AxisButton2D AxisButton;
            public bool Touched;
        }

        #endregion IMixedRealityInputSource Capabilities and GenericInputPointingSource

        #region MonoBehaviour Implementation

        private void Awake()
        {
            inputSystem = MixedRealityManager.Instance.GetManager<IMixedRealityInputSystem>();

#if UNITY_WSA
            GestureRecognizer = new GestureRecognizer();

            GestureRecognizer.Tapped += GestureRecognizer_Tapped;

            GestureRecognizer.HoldStarted += GestureRecognizer_HoldStarted;
            GestureRecognizer.HoldCompleted += GestureRecognizer_HoldCompleted;
            GestureRecognizer.HoldCanceled += GestureRecognizer_HoldCanceled;

            GestureRecognizer.ManipulationStarted += GestureRecognizer_ManipulationStarted;
            GestureRecognizer.ManipulationUpdated += GestureRecognizer_ManipulationUpdated;
            GestureRecognizer.ManipulationCompleted += GestureRecognizer_ManipulationCompleted;
            GestureRecognizer.ManipulationCanceled += GestureRecognizer_ManipulationCanceled;

            GestureRecognizer.SetRecognizableGestures(GestureSettings.Tap |
                                                      GestureSettings.ManipulationTranslate |
                                                      GestureSettings.Hold);

            // We need a separate gesture recognizer for navigation, since it isn't compatible with manipulation
            NavigationGestureRecognizer = new GestureRecognizer();

            NavigationGestureRecognizer.NavigationStarted += NavigationGestureRecognizer_NavigationStarted;
            NavigationGestureRecognizer.NavigationUpdated += NavigationGestureRecognizer_NavigationUpdated;
            NavigationGestureRecognizer.NavigationCompleted += NavigationGestureRecognizer_NavigationCompleted;
            NavigationGestureRecognizer.NavigationCanceled += NavigationGestureRecognizer_NavigationCanceled;

            if (useRailsNavigation)
            {
                NavigationGestureRecognizer.SetRecognizableGestures(GestureSettings.NavigationRailsX |
                                                                    GestureSettings.NavigationRailsY |
                                                                    GestureSettings.NavigationRailsZ);
            }
            else
            {
                NavigationGestureRecognizer.SetRecognizableGestures(GestureSettings.NavigationX |
                                                                    GestureSettings.NavigationY |
                                                                    GestureSettings.NavigationZ);
            }

            if (recognizerStart == RecognizerStartBehavior.AutoStart)
            {
                GestureRecognizer.StartCapturingGestures();
                NavigationGestureRecognizer.StartCapturingGestures();
            }
#endif
        }

        protected virtual void OnEnable()
        {
            if (!delayInitialization)
            {
                // The first time we call OnEnable we skip this.
                InitializeSources();
            }
        }

        protected virtual void Start()
        {
            if (delayInitialization)
            {
                delayInitialization = false;
                InitializeSources();
            }
        }

        protected virtual void OnDisable()
        {
#if UNITY_WSA
            StopGestureRecognizer();

            InteractionManager.InteractionSourceDetected -= InteractionManager_InteractionSourceDetected;
            InteractionManager.InteractionSourcePressed -= InteractionManager_InteractionSourcePressed;
            InteractionManager.InteractionSourceUpdated -= InteractionManager_InteractionSourceUpdated;
            InteractionManager.InteractionSourceReleased -= InteractionManager_InteractionSourceReleased;
            InteractionManager.InteractionSourceLost -= InteractionManager_InteractionSourceLost;

            InteractionSourceState[] states = InteractionManager.GetCurrentReading();
            for (var i = 0; i < states.Length; i++)
            {
                RemoveInteractionInputSource(GetOrAddInteractionSource(states[i].source));
            }
#endif
        }

        private void OnDestroy()
        {
#if UNITY_WSA
            if (GestureRecognizer != null)
            {
                GestureRecognizer.Tapped -= GestureRecognizer_Tapped;

                GestureRecognizer.HoldStarted -= GestureRecognizer_HoldStarted;
                GestureRecognizer.HoldCompleted -= GestureRecognizer_HoldCompleted;
                GestureRecognizer.HoldCanceled -= GestureRecognizer_HoldCanceled;

                GestureRecognizer.ManipulationStarted -= GestureRecognizer_ManipulationStarted;
                GestureRecognizer.ManipulationUpdated -= GestureRecognizer_ManipulationUpdated;
                GestureRecognizer.ManipulationCompleted -= GestureRecognizer_ManipulationCompleted;
                GestureRecognizer.ManipulationCanceled -= GestureRecognizer_ManipulationCanceled;

                GestureRecognizer.Dispose();
            }

            if (NavigationGestureRecognizer != null)
            {
                NavigationGestureRecognizer.NavigationStarted -= NavigationGestureRecognizer_NavigationStarted;
                NavigationGestureRecognizer.NavigationUpdated -= NavigationGestureRecognizer_NavigationUpdated;
                NavigationGestureRecognizer.NavigationCompleted -= NavigationGestureRecognizer_NavigationCompleted;
                NavigationGestureRecognizer.NavigationCanceled -= NavigationGestureRecognizer_NavigationCanceled;

                NavigationGestureRecognizer.Dispose();
            }
#endif
        }

        #endregion MonoBehaviour Implementation

        /// <summary>
        /// Make sure the gesture recognizer is off, then start it.
        /// Otherwise, leave it alone because it's already in the desired state.
        /// </summary>
        public void StartGestureRecognizer()
        {
#if UNITY_WSA
            if (GestureRecognizer != null && !GestureRecognizer.IsCapturingGestures())
            {
                GestureRecognizer.StartCapturingGestures();
            }
            if (NavigationGestureRecognizer != null && !NavigationGestureRecognizer.IsCapturingGestures())
            {
                NavigationGestureRecognizer.StartCapturingGestures();
            }
#endif
        }

        /// <summary>
        /// Make sure the gesture recognizer is on, then stop it.
        /// Otherwise, leave it alone because it's already in the desired state.
        /// </summary>
        public void StopGestureRecognizer()
        {
#if UNITY_WSA
            if (GestureRecognizer != null && GestureRecognizer.IsCapturingGestures())
            {
                GestureRecognizer.StopCapturingGestures();
            }

            if (NavigationGestureRecognizer != null && NavigationGestureRecognizer.IsCapturingGestures())
            {
                NavigationGestureRecognizer.StopCapturingGestures();
            }
#endif
        }

        #region GenericInputPointingSource Methods

#if UNITY_WSA

        /// <summary>
        /// Try to get the Source Kind of the specified Input Source.
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="sourceKind"></param>
        /// <returns>True if data is available.</returns>
        public static bool TryGetSourceKind(uint sourceId, out InteractionSourceKind sourceKind)
        {
            foreach (var inputSource in InteractionInputSourceList)
            {
                if (inputSource.SourceId == sourceId)
                {
                    sourceKind = inputSource.Source.kind;
                    return true;
                }
            }

            sourceKind = default(InteractionSourceKind);
            return false;
        }

#endif // UNITY_WSA

        /// <summary>
        /// Try to get the current Pointing Ray input reading from the specified Input Source.
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="pointingRay"></param>
        /// <returns>True if data is available.</returns>
        public bool TryGetPointingRay(uint sourceId, out Ray pointingRay)
        {
            foreach (var inputSource in InteractionInputSourceList)
            {
                if (inputSource.SourceId == sourceId && TryGetReading(inputSource.PointingRay, out pointingRay))
                {
                    return true;
                }
            }

            pointingRay = default(Ray);
            return false;
        }

        /// <summary>
        /// Try to get the current Pointer Position input reading from the specified Input Source.
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="position"></param>
        /// <returns>True if data is available.</returns>
        public static bool TryGetPointerPosition(uint sourceId, out Vector3 position)
        {
            foreach (var inputSource in InteractionInputSourceList)
            {
                if (inputSource.SourceId == sourceId && TryGetReading(inputSource.PointerPosition, out position))
                {
                    return true;
                }
            }

            position = default(Vector3);
            return false;
        }

        /// <summary>
        /// Try to get the current Pointer Rotation input reading from the specified Input Source.
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="rotation"></param>
        /// <returns>True if data is available.</returns>
        public bool TryGetPointerRotation(uint sourceId, out Quaternion rotation)
        {
            foreach (var inputSource in InteractionInputSourceList)
            {
                if (inputSource.SourceId == sourceId && TryGetReading(inputSource.PointerRotation, out rotation))
                {
                    return true;
                }
            }

            rotation = default(Quaternion);
            return false;
        }

        /// <summary>
        /// Try to get the current Grip Position input reading from the specified Input Source.
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="position"></param>
        /// <returns>True if data is available.</returns>
        public static bool TryGetGripPosition(uint sourceId, out Vector3 position)
        {
            foreach (var inputSource in InteractionInputSourceList)
            {
                if (inputSource.SourceId == sourceId && TryGetReading(inputSource.GripPosition, out position))
                {
                    return true;
                }
            }

            position = default(Vector3);
            return false;
        }

        /// <summary>
        /// Try to get the current Grip Rotation input reading from the specified Input Source.
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="rotation"></param>
        /// <returns>True if data is available.</returns>
        public bool TryGetGripRotation(uint sourceId, out Quaternion rotation)
        {
            foreach (var inputSource in InteractionInputSourceList)
            {
                if (inputSource.SourceId == sourceId && TryGetReading(inputSource.GripRotation, out rotation))
                {
                    return true;
                }
            }

            rotation = default(Quaternion);
            return false;
        }

        /// <summary>
        /// Try to get the current Thumbstick input reading from the specified Input Source.
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="thumbstickPressed"></param>
        /// <param name="thumbstickPosition"></param>
        /// <returns>True if data is available.</returns>
        public bool TryGetThumbstick(uint sourceId, out bool thumbstickPressed, out Vector2 thumbstickPosition)
        {
            foreach (var inputSource in InteractionInputSourceList)
            {
                AxisButton2D thumbstick;
                if (inputSource.SourceId == sourceId && TryGetReading(inputSource.Thumbstick, out thumbstick))
                {
                    thumbstickPressed = thumbstick.Pressed;
                    thumbstickPosition = thumbstick.Position;
                    return true;
                }
            }

            thumbstickPressed = false;
            thumbstickPosition = Vector2.zero;
            return false;
        }

        /// <summary>
        /// Try to get the current Touchpad input reading from the specified Input Source.
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="touchpadPressed"></param>
        /// <param name="touchpadTouched"></param>
        /// <param name="touchpadPosition"></param>
        /// <returns>True if data is available.</returns>
        public bool TryGetTouchpad(uint sourceId, out bool touchpadPressed, out bool touchpadTouched, out Vector2 touchpadPosition)
        {

            foreach (var inputSource in InteractionInputSourceList)
            {
                TouchpadData touchpad;
                if (inputSource.SourceId == sourceId && TryGetReading(inputSource.Touchpad, out touchpad))
                {
                    touchpadPressed = touchpad.AxisButton.Pressed;
                    touchpadTouched = touchpad.Touched;
                    touchpadPosition = touchpad.AxisButton.Position;
                    return true;
                }
            }

            touchpadPressed = false;
            touchpadTouched = false;
            touchpadPosition = Vector2.zero;
            return false;
        }

        /// <summary>
        /// Try to get the current Select input reading from the specified Input Source.
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="selectPressed"></param>
        /// <param name="selectPressedAmount"></param>
        /// <returns>True if data is available.</returns>
        public bool TryGetSelect(uint sourceId, out bool selectPressed, out double selectPressedAmount)
        {
            foreach (var inputSource in InteractionInputSourceList)
            {
                AxisButton1D select;
                if (inputSource.SourceId == sourceId && TryGetReading(inputSource.Select, out select))
                {
                    selectPressed = select.Pressed;
                    selectPressedAmount = select.PressedAmount;
                    return true;
                }
            }

            selectPressed = false;
            selectPressedAmount = 0;
            return false;
        }

        /// <summary>
        /// Try to get the current Grasp input reading from the specified Input Source.
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="graspPressed"></param>
        /// <returns>True if data is available.</returns>
        public bool TryGetGrasp(uint sourceId, out bool graspPressed)
        {
            foreach (var inputSource in InteractionInputSourceList)
            {
                if (inputSource.SourceId == sourceId && TryGetReading(inputSource.Grasp, out graspPressed))
                {
                    return true;
                }
            }

            graspPressed = false;
            return false;
        }

        /// <summary>
        /// Try to get the current Menu input reading from the specified Input Source.
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="menuPressed"></param>
        /// <returns>True if data is available.</returns>
        public bool TryGetMenu(uint sourceId, out bool menuPressed)
        {
            foreach (var inputSource in InteractionInputSourceList)
            {
                if (inputSource.SourceId == sourceId && TryGetReading(inputSource.Menu, out menuPressed))
                {
                    return true;
                }
            }

            menuPressed = false;
            return false;
        }

        /// <summary>
        /// Internal Utility for getting the current input reading from the Specified Input Source.
        /// </summary>
        /// <typeparam name="TReading"></typeparam>
        /// <param name="capability"></param>
        /// <param name="reading"></param>
        /// <returns>True if data is available.</returns>
        private static bool TryGetReading<TReading>(SourceCapability<TReading> capability, out TReading reading)
        {
            if (capability.IsAvailable)
            {
                Debug.Assert(capability.IsSupported);

                reading = capability.CurrentReading;
                return true;
            }

            reading = default(TReading);
            return false;
        }

        /// <summary>
        /// Start the Haptics for the Specified Input Source.
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="intensity"></param>
        public void StartHaptics(uint sourceId, float intensity)
        {
#if UNITY_WSA
            foreach (var inputSource in InteractionInputSourceList)
            {
                if (inputSource.SourceId == sourceId)
                {
                    inputSource.Source.StartHaptics(intensity);
                }
            }
#endif
        }

        /// <summary>
        /// Start the Haptics for the specified Input Source.
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="intensity"></param>
        /// <param name="durationInSeconds"></param>
        public void StartHaptics(uint sourceId, float intensity, float durationInSeconds)
        {
#if UNITY_WSA
            foreach (var inputSource in InteractionInputSourceList)
            {
                if (inputSource.SourceId == sourceId)
                {
                    inputSource.Source.StartHaptics(intensity, durationInSeconds);
                }
            }
#endif
        }

        /// <summary>
        /// Stops the Haptics for the Specified Input Source.
        /// </summary>
        /// <param name="sourceId"></param>
        public void StopHaptics(uint sourceId)
        {
#if UNITY_WSA
            foreach (var inputSource in InteractionInputSourceList)
            {
                if (inputSource.SourceId == sourceId)
                {
                    inputSource.Source.StopHaptics();
                }
            }
#endif
        }

        #endregion GenericInputPointingSource Methods

        private void InitializeSources()
        {
#if UNITY_WSA
            if (recognizerStart == RecognizerStartBehavior.AutoStart)
            {
                StartGestureRecognizer();
            }

            InteractionSourceState[] states = InteractionManager.GetCurrentReading();

            for (var i = 0; i < states.Length; i++)
            {
                var inputSource = GetOrAddInteractionSource(states[i].source);

                if (inputSource == null) { continue; }

                // NOTE: We update the source state data, in case an app wants to query it on source detected.
                UpdateInteractionSource(states[i], inputSource);
                inputSystem.RaiseSourceDetected(inputSource);
            }

            InteractionManager.InteractionSourceDetected += InteractionManager_InteractionSourceDetected;
            InteractionManager.InteractionSourcePressed += InteractionManager_InteractionSourcePressed;
            InteractionManager.InteractionSourceUpdated += InteractionManager_InteractionSourceUpdated;
            InteractionManager.InteractionSourceReleased += InteractionManager_InteractionSourceReleased;
            InteractionManager.InteractionSourceLost += InteractionManager_InteractionSourceLost;
#else
            recognizerStart = RecognizerStartBehavior.ManualStart;
#endif
        }

#if UNITY_WSA
        /// <summary>
        /// Gets the source data for the specified interaction source if it already exists, otherwise creates it.
        /// </summary>
        /// <param name="interactionSource">Interaction source for which data should be retrieved.</param>
        /// <returns>The source data requested.</returns>
        private InteractionInputSource GetOrAddInteractionSource(InteractionSource interactionSource)
        {
            foreach (var inputSource in InteractionInputSourceList)
            {
                if (interactionSource.kind == InteractionSourceKind.Other &&
                    inputSource.Source.kind == InteractionSourceKind.Hand)
                {
                    return inputSource;
                }

                if (inputSource.Source.id == interactionSource.id)
                {
                    return inputSource;
                }
            }

            if (interactionSource.kind == InteractionSourceKind.Other)
            {
                return null;
            }

            var pointerList = new List<BaseControllerPointer>(0);

            foreach (var pointerOption in pointerOptions)
            {
                Debug.Assert(pointerOption.TargetController != Handedness.None, "Interaction Source Pointer must be set to Left, Right, or Both.");

                if (interactionSource.handedness == InteractionSourceHandedness.Unknown ||
                    interactionSource.handedness == InteractionSourceHandedness.Left && pointerOption.TargetController == Handedness.Right ||
                    interactionSource.handedness == InteractionSourceHandedness.Right && pointerOption.TargetController == Handedness.Left)
                {
                    continue;
                }

                var pointerObject = Instantiate(pointerOption.PointerPrefab);
                var pointer = pointerObject.GetComponent<BaseControllerPointer>();
                pointer.Handedness = (Handedness)interactionSource.handedness;
                pointer.PointerName = $"{interactionSource.handedness}_{interactionSource.kind}_{pointer.GetType().Name}";
                pointerList.Add(pointer);
            }

            var pointers = new IMixedRealityPointer[pointerList.Count];
            for (var i = 0; i < pointerList.Count; i++)
            {
                pointers[i] = pointerList[i];
            }

            BaseControllerPointer[] pointerSceneObjects = pointerList.ToArray();

            var sourceData = new InteractionInputSource(
                interactionSource,
                $"{(interactionSource.handedness == InteractionSourceHandedness.Unknown ? "" : $"{interactionSource.handedness}_")}{interactionSource.kind}",
                pointerSceneObjects,
                pointers);

            InteractionInputSourceList.Add(sourceData);

            return sourceData;
        }

        private void RemoveInteractionInputSource(InteractionInputSource interactionSource)
        {
            if (interactionSource == null) { return; }

            inputSystem.RaiseSourceLost(interactionSource);
            InteractionInputSourceList.Remove(interactionSource);

            for (var j = 0; j < interactionSource.PointerSceneObjects.Length; j++)
            {
                if (interactionSource.PointerSceneObjects[j] != null)
                {
                    Destroy(interactionSource.PointerSceneObjects[j].gameObject);
                }
            }
        }

        /// <summary>
        /// Updates the source information.
        /// </summary>
        /// <param name="interactionSourceState">Interaction source to use to update the source information.</param>
        /// <param name="sourceData">GenericInputPointingSource structure to update.</param>
        private static void UpdateInteractionSource(InteractionSourceState interactionSourceState, InteractionInputSource sourceData)
        {
            Debug.Assert(interactionSourceState.source.id == sourceData.Source.id, "An UpdateSourceState call happened with mismatched source ID.");
            Debug.Assert(interactionSourceState.source.kind == sourceData.Source.kind, "An UpdateSourceState call happened with mismatched source kind.");

            Vector3 newPointerPosition;
            sourceData.PointerPosition.IsAvailable = interactionSourceState.sourcePose.TryGetPosition(out newPointerPosition, InteractionSourceNode.Pointer);
            // Using a heuristic for IsSupported, since the APIs don't yet support querying this capability directly.
            sourceData.PointerPosition.IsSupported |= sourceData.PointerPosition.IsAvailable;

            Vector3 newGripPosition;
            sourceData.GripPosition.IsAvailable = interactionSourceState.sourcePose.TryGetPosition(out newGripPosition, InteractionSourceNode.Grip);
            // Using a heuristic for IsSupported, since the APIs don't yet support querying this capability directly.
            sourceData.GripPosition.IsSupported |= sourceData.GripPosition.IsAvailable;

            if (CameraCache.Main.transform.parent != null)
            {
                newPointerPosition = CameraCache.Main.transform.parent.TransformPoint(newPointerPosition);
                newGripPosition = CameraCache.Main.transform.parent.TransformPoint(newGripPosition);
            }

            if (sourceData.PointerPosition.IsAvailable)
            {
                sourceData.PointerPositionUpdated = sourceData.PointerPosition.CurrentReading != newPointerPosition;
            }

            if (sourceData.GripPosition.IsAvailable)
            {
                sourceData.GripPositionUpdated = sourceData.GripPosition.CurrentReading != newGripPosition;
            }

            sourceData.PointerPosition.CurrentReading = newPointerPosition;
            sourceData.GripPosition.CurrentReading = newGripPosition;

            Quaternion newPointerRotation;
            sourceData.PointerRotation.IsAvailable = interactionSourceState.sourcePose.TryGetRotation(out newPointerRotation, InteractionSourceNode.Pointer);
            // Using a heuristic for IsSupported, since the APIs don't yet support querying this capability directly.
            sourceData.PointerRotation.IsSupported |= sourceData.PointerRotation.IsAvailable;

            Quaternion newGripRotation;
            sourceData.GripRotation.IsAvailable = interactionSourceState.sourcePose.TryGetRotation(out newGripRotation);
            // Using a heuristic for IsSupported, since the APIs don't yet support querying this capability directly.
            sourceData.GripRotation.IsSupported |= sourceData.GripRotation.IsAvailable;

            if (CameraCache.Main.transform.parent != null)
            {
                newPointerRotation.eulerAngles = CameraCache.Main.transform.parent.TransformDirection(newPointerRotation.eulerAngles);
                newGripRotation.eulerAngles = CameraCache.Main.transform.parent.TransformDirection(newGripRotation.eulerAngles);
            }

            if (sourceData.PointerRotation.IsAvailable)
            {
                sourceData.PointerRotationUpdated = sourceData.PointerRotation.CurrentReading != newPointerRotation;
            }

            else if (sourceData.GripRotation.IsAvailable)
            {
                sourceData.GripRotationUpdated = sourceData.GripRotation.CurrentReading != newGripRotation;
            }

            sourceData.PointerRotation.CurrentReading = newPointerRotation;
            sourceData.GripRotation.CurrentReading = newGripRotation;

            Vector3 pointerForward = Vector3.zero;
            sourceData.PointingRay.IsSupported = interactionSourceState.source.supportsPointing;
            sourceData.PointingRay.IsAvailable = sourceData.PointerPosition.IsAvailable;

            if (CameraCache.Main.transform.parent != null)
            {
                pointerForward = CameraCache.Main.transform.parent.TransformDirection(pointerForward);
            }

            sourceData.PointingRay.CurrentReading = new Ray(sourceData.PointerPosition.CurrentReading, pointerForward);

            sourceData.Thumbstick.IsSupported = interactionSourceState.source.supportsThumbstick;
            sourceData.Thumbstick.IsAvailable = sourceData.Thumbstick.IsSupported;
            if (sourceData.Thumbstick.IsAvailable)
            {
                sourceData.ThumbstickPositionUpdated = sourceData.Thumbstick.CurrentReading.Position != interactionSourceState.thumbstickPosition;
                sourceData.Thumbstick.CurrentReading.Position = interactionSourceState.thumbstickPosition;
                sourceData.Thumbstick.CurrentReading.Pressed = interactionSourceState.thumbstickPressed;
            }
            else
            {
                sourceData.Thumbstick.CurrentReading = default(AxisButton2D);
            }

            sourceData.Touchpad.IsSupported = interactionSourceState.source.supportsTouchpad;
            sourceData.Touchpad.IsAvailable = sourceData.Touchpad.IsSupported;

            if (sourceData.Touchpad.IsAvailable)
            {
                sourceData.TouchpadPositionUpdated = sourceData.Touchpad.CurrentReading.AxisButton.Position != interactionSourceState.touchpadPosition;
                sourceData.TouchpadTouchedUpdated = sourceData.Touchpad.CurrentReading.Touched != interactionSourceState.touchpadTouched;
                sourceData.Touchpad.CurrentReading.Touched = interactionSourceState.touchpadTouched;
                sourceData.Touchpad.CurrentReading.AxisButton.Pressed = interactionSourceState.touchpadPressed;
                sourceData.Touchpad.CurrentReading.AxisButton.Position = interactionSourceState.touchpadPosition;
            }
            else
            {
                sourceData.Touchpad.CurrentReading = default(TouchpadData);
            }

            sourceData.Select.IsSupported = true; // All input mechanisms support "select".
            sourceData.Select.IsAvailable = sourceData.Select.IsSupported;
            sourceData.SelectPressedAmountUpdated = !sourceData.Select.CurrentReading.PressedAmount.Equals(interactionSourceState.selectPressedAmount);
            sourceData.Select.CurrentReading.Pressed = interactionSourceState.selectPressed;
            sourceData.Select.CurrentReading.PressedAmount = interactionSourceState.selectPressedAmount;

            sourceData.Grasp.IsSupported = interactionSourceState.source.supportsGrasp;
            sourceData.Grasp.IsAvailable = sourceData.Grasp.IsSupported;
            sourceData.Grasp.CurrentReading = sourceData.Grasp.IsAvailable && interactionSourceState.grasped;

            sourceData.Menu.IsSupported = interactionSourceState.source.supportsMenu;
            sourceData.Menu.IsAvailable = sourceData.Menu.IsSupported;
            sourceData.Menu.CurrentReading = sourceData.Menu.IsAvailable && interactionSourceState.menuPressed;
        }

        #region InteractionManager Events

        private void InteractionManager_InteractionSourceDetected(InteractionSourceDetectedEventArgs args)
        {
            var inputSource = GetOrAddInteractionSource(args.state.source);
            if (inputSource == null) { return; }

            // NOTE: We update the source state data, in case an app wants to query it on source detected.
            UpdateInteractionSource(args.state, inputSource);
            inputSystem.RaiseSourceDetected(inputSource);
        }

        private void InteractionManager_InteractionSourcePressed(InteractionSourcePressedEventArgs args)
        {
            InteractionInputSource inputSource = GetOrAddInteractionSource(args.state.source);
            if (inputSource == null) { return; }
            //InputType inputType;
            //switch (args.pressType)
            //{
            //    case InteractionSourcePressType.None:
            //        inputType = InputType.None;
            //        break;
            //    case InteractionSourcePressType.Select:
            //        inputType = InputType.Select;
            //        break;
            //    case InteractionSourcePressType.Menu:
            //        inputType = InputType.Menu;
            //        break;
            //    case InteractionSourcePressType.Grasp:
            //        inputType = InputType.GripPress;
            //        break;
            //    case InteractionSourcePressType.Touchpad:
            //        inputType = InputType.Touchpad;
            //        break;
            //    case InteractionSourcePressType.Thumbstick:
            //        inputType = InputType.ThumbStick;
            //        break;
            //    default:
            //        throw new ArgumentOutOfRangeException();
            //}

            //inputSystem.RaiseOnInputDown(inputSource, (Handedness)args.state.source.handedness, inputType);
        }

        private void InteractionManager_InteractionSourceUpdated(InteractionSourceUpdatedEventArgs args)
        {
            InteractionInputSource inputSource = GetOrAddInteractionSource(args.state.source);

            if (inputSource == null) { return; }

            inputSource.Reset();

            UpdateInteractionSource(args.state, inputSource);

            //if (inputSource.PointerPositionUpdated)
            //{
            //    inputSystem.Raise3DoFInputChanged(inputSource, (Handedness)args.state.source.handedness, InputType.PointerPosition, inputSource.PointerPosition.CurrentReading);
            //}

            //if (inputSource.GripPositionUpdated)
            //{
            //    inputSystem.Raise3DoFInputChanged(inputSource, (Handedness)args.state.source.handedness, InputType.GripPosition, inputSource.GripPosition.CurrentReading);
            //}

            //if (inputSource.PointerRotationUpdated)
            //{
            //    inputSystem.Raise3DoFInputChanged(inputSource, (Handedness)args.state.source.handedness, InputType.PointerRotation, inputSource.PointerRotation.CurrentReading);
            //}

            //if (inputSource.GripRotationUpdated)
            //{
            //    inputSystem.Raise3DoFInputChanged(inputSource, (Handedness)args.state.source.handedness, InputType.GripRotation, inputSource.GripRotation.CurrentReading);
            //}

            //if (inputSource.ThumbstickPositionUpdated)
            //{
            //    inputSystem.Raise2DoFInputChanged(inputSource, (Handedness)args.state.source.handedness, InputType.ThumbStick, inputSource.Thumbstick.CurrentReading.Position);
            //}

            //if (inputSource.TouchpadPositionUpdated)
            //{
            //    inputSystem.Raise2DoFInputChanged(inputSource, (Handedness)args.state.source.handedness, InputType.Touchpad, inputSource.Touchpad.CurrentReading.AxisButton.Position);
            //}

            //if (inputSource.TouchpadTouchedUpdated)
            //{
            //    if (inputSource.Touchpad.CurrentReading.Touched)
            //    {
            //        inputSystem.RaiseOnInputDown(inputSource, (Handedness)args.state.source.handedness, InputType.Touchpad);
            //    }
            //    else
            //    {
            //        inputSystem.RaiseOnInputUp(inputSource, (Handedness)args.state.source.handedness, InputType.Touchpad);
            //    }
            //}

            //if (inputSource.SelectPressedAmountUpdated)
            //{
            //    inputSystem.RaiseOnInputPressed(inputSource, (Handedness)args.state.source.handedness, InputType.Select, (float)inputSource.Select.CurrentReading.PressedAmount);
            //}
        }

        private void InteractionManager_InteractionSourceReleased(InteractionSourceReleasedEventArgs args)
        {
            InteractionInputSource inputSource = GetOrAddInteractionSource(args.state.source);
            if (inputSource == null) { return; }

            //InputType inputType;
            //switch (args.pressType)
            //{
            //    case InteractionSourcePressType.None:
            //        inputType = InputType.None;
            //        break;
            //    case InteractionSourcePressType.Select:
            //        inputType = InputType.Select;
            //        break;
            //    case InteractionSourcePressType.Menu:
            //        inputType = InputType.Menu;
            //        break;
            //    case InteractionSourcePressType.Grasp:
            //        inputType = InputType.GripPress;
            //        break;
            //    case InteractionSourcePressType.Touchpad:
            //        inputType = InputType.Touchpad;
            //        break;
            //    case InteractionSourcePressType.Thumbstick:
            //        inputType = InputType.ThumbStick;
            //        break;
            //    default:
            //        throw new ArgumentOutOfRangeException();
            //}

            //inputSystem.RaiseOnInputUp(inputSource, (Handedness)args.state.source.handedness, inputType);
        }

        private void InteractionManager_InteractionSourceLost(InteractionSourceLostEventArgs args)
        {
            RemoveInteractionInputSource(GetOrAddInteractionSource(args.state.source));
        }

        #endregion InteractionManager Events

        #region Raise GestureRecognizer Events

        private void GestureRecognizer_Tapped(TappedEventArgs args)
        {
            InteractionInputSource inputSource = GetOrAddInteractionSource(args.source);
            if (inputSource == null) { return; }
            //inputSystem.RaiseInputClicked(inputSource.Pointers[0], (Handedness)args.source.handedness, InputType.Select, args.tapCount);
        }

        private void GestureRecognizer_HoldStarted(HoldStartedEventArgs args)
        {
            InteractionInputSource inputSource = GetOrAddInteractionSource(args.source);
            if (inputSource == null) { return; }
            inputSystem.RaiseHoldStarted(inputSource, (Handedness)args.source.handedness);
        }

        private void GestureRecognizer_HoldCanceled(HoldCanceledEventArgs args)
        {
            InteractionInputSource inputSource = GetOrAddInteractionSource(args.source);
            if (inputSource == null) { return; }
            inputSystem.RaiseHoldCanceled(inputSource, (Handedness)args.source.handedness);
        }

        private void GestureRecognizer_HoldCompleted(HoldCompletedEventArgs args)
        {
            InteractionInputSource inputSource = GetOrAddInteractionSource(args.source);
            if (inputSource == null) { return; }
            inputSystem.RaiseHoldCompleted(inputSource, (Handedness)args.source.handedness);
        }

        private void GestureRecognizer_ManipulationStarted(ManipulationStartedEventArgs args)
        {
            InteractionInputSource inputSource = GetOrAddInteractionSource(args.source);
            if (inputSource == null) { return; }
            inputSystem.RaiseManipulationStarted(inputSource, (Handedness)args.source.handedness);
        }

        private void GestureRecognizer_ManipulationUpdated(ManipulationUpdatedEventArgs args)
        {
            InteractionInputSource inputSource = GetOrAddInteractionSource(args.source);
            if (inputSource == null) { return; }
            inputSystem.RaiseManipulationUpdated(inputSource, (Handedness)args.source.handedness, args.cumulativeDelta);
        }

        private void GestureRecognizer_ManipulationCompleted(ManipulationCompletedEventArgs args)
        {
            InteractionInputSource inputSource = GetOrAddInteractionSource(args.source);
            if (inputSource == null) { return; }
            inputSystem.RaiseManipulationCompleted(inputSource, (Handedness)args.source.handedness, args.cumulativeDelta);
        }

        private void GestureRecognizer_ManipulationCanceled(ManipulationCanceledEventArgs args)
        {
            InteractionInputSource inputSource = GetOrAddInteractionSource(args.source);
            if (inputSource == null) { return; }
            inputSystem.RaiseManipulationCanceled(inputSource, (Handedness)args.source.handedness);
        }

        private void NavigationGestureRecognizer_NavigationStarted(NavigationStartedEventArgs args)
        {
            InteractionInputSource inputSource = GetOrAddInteractionSource(args.source);
            if (inputSource == null) { return; }
            inputSystem.RaiseNavigationStarted(inputSource, (Handedness)args.source.handedness);
        }

        private void NavigationGestureRecognizer_NavigationUpdated(NavigationUpdatedEventArgs args)
        {
            InteractionInputSource inputSource = GetOrAddInteractionSource(args.source);
            if (inputSource == null) { return; }
            inputSystem.RaiseNavigationUpdated(inputSource, (Handedness)args.source.handedness, args.normalizedOffset);
        }

        private void NavigationGestureRecognizer_NavigationCompleted(NavigationCompletedEventArgs args)
        {
            InteractionInputSource inputSource = GetOrAddInteractionSource(args.source);
            if (inputSource == null) { return; }
            inputSystem.RaiseNavigationCompleted(inputSource, (Handedness)args.source.handedness, args.normalizedOffset);
        }

        private void NavigationGestureRecognizer_NavigationCanceled(NavigationCanceledEventArgs args)
        {
            InteractionInputSource inputSource = GetOrAddInteractionSource(args.source);
            if (inputSource == null) { return; }
            inputSystem.RaiseNavigationCanceled(inputSource, (Handedness)args.source.handedness);
        }

        #endregion Raise GestureRecognizer Events

#endif

    }
}