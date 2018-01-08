// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

#if UNITY_WSA
using UnityEngine.XR.WSA.Input;
using System.Collections.Generic;
#endif

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Input sources for gestures and interaction source information from the WSA APIs, which gives access to various system-supported gestures
    /// and positional information for the various inputs that Windows gestures supports.
    /// This is mostly a wrapper on top of GestureRecognizer and InteractionManager.
    /// </summary>
    public class InteractionInputSources : Singleton<InteractionInputSources>
    {
        /// <summary>
        /// This enumeration gives the manager two different ways to handle the recognizer. Both will
        /// set up the recognizer. The first causes the recognizer to start
        /// immediately. The second allows the recognizer to be manually started at a later time.
        /// </summary>
        public enum RecognizerStartBehavior { AutoStart, ManualStart }

        [Tooltip("Whether the recognizer should be activated on start.")]
        public RecognizerStartBehavior RecognizerStart;

        [Tooltip("Set to true to use the use rails (guides) for the navigation gesture, as opposed to full 3D navigation.")]
        public bool UseRailsNavigation;

        /// <summary>
        /// Always true initially so we only initialize our interaction sources 
        /// after all <see cref="Singleton{T}"/> Instances have been properly initialized.
        /// </summary>
        private bool delayInitialization = true;

#if UNITY_WSA
        protected GestureRecognizer GestureRecognizer;
        protected GestureRecognizer NavigationGestureRecognizer;
#endif

        /// <summary>
        /// Dictionary linking each source ID to its data.
        /// </summary>
        private readonly HashSet<InteractionInputSource> interactionInputSources = new HashSet<InteractionInputSource>();

        #region IInputSource Capabilities and GenericInputPointingSource

        private class InteractionInputSource : GenericInputPointingSource
        {
#if UNITY_WSA
            public readonly InteractionSource Source;

            public InteractionInputSource(InteractionSource source, uint sourceId, string name) : base(sourceId, name)
            {
                Source = source;
            }
#endif

            public override SupportedInputInfo GetSupportedInputInfo()
            {
                var retVal = SupportedInputInfo.None;
                retVal |= GetSupportFlag(PointerPosition, SupportedInputInfo.Position);
                retVal |= GetSupportFlag(PointerRotation, SupportedInputInfo.Rotation);
                retVal |= GetSupportFlag(PointingRay, SupportedInputInfo.Pointing);
                retVal |= GetSupportFlag(Thumbstick, SupportedInputInfo.Thumbstick);
                retVal |= GetSupportFlag(Touchpad, SupportedInputInfo.Touch);
                retVal |= GetSupportFlag(Select, SupportedInputInfo.Select);
                retVal |= GetSupportFlag(Menu, SupportedInputInfo.Menu);
                retVal |= GetSupportFlag(Grasp, SupportedInputInfo.Grasp);

                return retVal;
            }

            private static SupportedInputInfo GetSupportFlag<TReading>(SourceCapability<TReading> capability, SupportedInputInfo flagIfSupported)
            {
                return capability.IsSupported ? flagIfSupported : SupportedInputInfo.None;
            }

            public void Reset()
            {
                ThumbstickPositionUpdated = false;
                TouchpadPositionUpdated = false;
                TouchpadTouchedUpdated = false;
                PositionUpdated = false;
                RotationUpdated = false;
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
            public bool PositionUpdated;
            public bool RotationUpdated;
            public bool SelectPressedAmountUpdated;

            public override bool TryGetPointerPosition(out Vector3 position)
            {
                position = Vector3.zero;
                if (PointerPosition.IsSupported && PointerPosition.IsAvailable)
                {
                    position = PointerPosition.CurrentReading;
                    return true;
                }

                return false;
            }

            public override bool TryGetPointerRotation(out Quaternion rotation)
            {
                rotation = Quaternion.identity;
                if (PointerRotation.IsSupported && PointerRotation.IsAvailable)
                {
                    rotation = PointerRotation.CurrentReading;
                    return true;
                }

                return false;
            }

            public override bool TryGetPointingRay(out Ray pointingRay)
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

        #endregion IInputSource Capabilities and GenericInputPointingSource

        #region MonoBehaviour Implementation

        protected override void Awake()
        {
            base.Awake();

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

            if (UseRailsNavigation)
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

            if (RecognizerStart == RecognizerStartBehavior.AutoStart)
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
                foreach (var inputSource in interactionInputSources)
                {
                    if (inputSource.Source.id == states[i].source.id)
                    {
                        InputManager.Instance.RaiseSourceLost(inputSource);
                        interactionInputSources.Remove(inputSource);
                    }
                }
            }
#endif
        }

        protected override void OnDestroy()
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
            base.OnDestroy();
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

        /// <summary>
        /// Get the Supported Input Info for the specified Input Source.
        /// </summary>
        /// <param name="sourceId"></param>
        /// <returns><see cref="SupportedInputInfo"/></returns>
        public SupportedInputInfo GetSupportedInputInfo(uint sourceId)
        {
            foreach (var inputSource in interactionInputSources)
            {
                if (inputSource.SourceId == sourceId)
                {
                    return inputSource.GetSupportedInputInfo();
                }
            }

            return SupportedInputInfo.None;
        }

#if UNITY_WSA

        /// <summary>
        /// Try to get the Source Kind of the specified Input Source.
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="sourceKind"></param>
        /// <returns>True if data is available.</returns>
        public bool TryGetSourceKind(uint sourceId, out InteractionSourceKind sourceKind)
        {
            foreach (var inputSource in interactionInputSources)
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
            foreach (var inputSource in interactionInputSources)
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
        public bool TryGetPointerPosition(uint sourceId, out Vector3 position)
        {
            foreach (var inputSource in interactionInputSources)
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
            foreach (var inputSource in interactionInputSources)
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
        public bool TryGetGripPosition(uint sourceId, out Vector3 position)
        {
            foreach (var inputSource in interactionInputSources)
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
            foreach (var inputSource in interactionInputSources)
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
            foreach (var inputSource in interactionInputSources)
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

            foreach (var inputSource in interactionInputSources)
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
            foreach (var inputSource in interactionInputSources)
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
            foreach (var inputSource in interactionInputSources)
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
            foreach (var inputSource in interactionInputSources)
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
            foreach (var inputSource in interactionInputSources)
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
            foreach (var inputSource in interactionInputSources)
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
            foreach (var inputSource in interactionInputSources)
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
            InputManager.AssertIsInitialized();

#if UNITY_WSA
            if (RecognizerStart == RecognizerStartBehavior.AutoStart)
            {
                StartGestureRecognizer();
            }

            InteractionSourceState[] states = InteractionManager.GetCurrentReading();
            for (var i = 0; i < states.Length; i++)
            {
                InputManager.Instance.RaiseSourceDetected(GetOrAddInteractionSource(states[i].source));
            }

            InteractionManager.InteractionSourceDetected += InteractionManager_InteractionSourceDetected;
            InteractionManager.InteractionSourcePressed += InteractionManager_InteractionSourcePressed;
            InteractionManager.InteractionSourceUpdated += InteractionManager_InteractionSourceUpdated;
            InteractionManager.InteractionSourceReleased += InteractionManager_InteractionSourceReleased;
            InteractionManager.InteractionSourceLost += InteractionManager_InteractionSourceLost;
#else
            RecognizerStart = RecognizerStartBehavior.ManualStart;
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
            foreach (var inputSource in interactionInputSources)
            {
                if (inputSource.Source.id == interactionSource.id)
                {
                    return inputSource;
                }
            }

            var sourceData = new InteractionInputSource(interactionSource, InputManager.GenerateNewSourceId(),
                    string.Format("{0} {1}", interactionSource.handedness, interactionSource.kind));
            interactionInputSources.Add(sourceData);

            return sourceData;
        }
#endif

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

            if (sourceData.PointerPosition.IsAvailable || sourceData.GripPosition.IsAvailable)
            {
                sourceData.PositionUpdated = sourceData.PointerPosition.CurrentReading != newPointerPosition || sourceData.GripPosition.CurrentReading != newGripPosition;
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

            if (sourceData.PointerRotation.IsAvailable || sourceData.GripRotation.IsAvailable)
            {
                sourceData.RotationUpdated = sourceData.PointerRotation.CurrentReading != newPointerRotation || sourceData.GripRotation.CurrentReading != newGripRotation;
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

#if UNITY_WSA

        #region InteractionManager Events

        private void InteractionManager_InteractionSourceUpdated(InteractionSourceUpdatedEventArgs args)
        {
            InteractionInputSource sourceData = GetOrAddInteractionSource(args.state.source);

            sourceData.Reset();

            UpdateInteractionSource(args.state, sourceData);

            if (sourceData.PositionUpdated)
            {
                InputManager.Instance.RaiseSourcePositionChanged(sourceData, sourceData.PointerPosition.CurrentReading, sourceData.GripPosition.CurrentReading, (Handedness)args.state.source.handedness);
            }

            if (sourceData.RotationUpdated)
            {
                InputManager.Instance.RaiseSourceRotationChanged(sourceData, sourceData.PointerRotation.CurrentReading, sourceData.GripRotation.CurrentReading, (Handedness)args.state.source.handedness);
            }

            if (sourceData.ThumbstickPositionUpdated)
            {
                InputManager.Instance.RaiseInputPositionChanged(sourceData, sourceData.Thumbstick.CurrentReading.Position, InteractionSourcePressType.Thumbstick, (Handedness)args.state.source.handedness);
            }

            if (sourceData.TouchpadPositionUpdated)
            {
                InputManager.Instance.RaiseInputPositionChanged(sourceData, sourceData.Touchpad.CurrentReading.AxisButton.Position, InteractionSourcePressType.Touchpad, (Handedness)args.state.source.handedness);
            }

            if (sourceData.TouchpadTouchedUpdated)
            {
                if (sourceData.Touchpad.CurrentReading.Touched)
                {
                    InputManager.Instance.RaiseOnInputDown(sourceData, InteractionSourcePressType.Touchpad, (Handedness)args.state.source.handedness);
                }
                else
                {
                    InputManager.Instance.RaiseOnInputUp(sourceData, InteractionSourcePressType.Touchpad, (Handedness)args.state.source.handedness);
                }
            }

            if (sourceData.SelectPressedAmountUpdated)
            {
                InputManager.Instance.RaiseOnInputPressed(sourceData, sourceData.Select.CurrentReading.PressedAmount, InteractionSourcePressType.Select, (Handedness)args.state.source.handedness);
            }
        }

        private void InteractionManager_InteractionSourceReleased(InteractionSourceReleasedEventArgs args)
        {
            InputManager.Instance.RaiseOnInputUp(GetOrAddInteractionSource(args.state.source), args.pressType, (Handedness)args.state.source.handedness);
        }

        private void InteractionManager_InteractionSourcePressed(InteractionSourcePressedEventArgs args)
        {
            InputManager.Instance.RaiseOnInputDown(GetOrAddInteractionSource(args.state.source), args.pressType, (Handedness)args.state.source.handedness);
        }

        private void InteractionManager_InteractionSourceLost(InteractionSourceLostEventArgs args)
        {
            foreach (var inputSource in interactionInputSources)
            {
                if (inputSource.Source.id == args.state.source.id)
                {
                    InputManager.Instance.RaiseSourceLost(inputSource);
                    interactionInputSources.Remove(inputSource);
                }
            }
        }

        private void InteractionManager_InteractionSourceDetected(InteractionSourceDetectedEventArgs args)
        {
            var sourceData = GetOrAddInteractionSource(args.state.source);

            // NOTE: We update the source state data, in case an app wants to query it on source detected.
            UpdateInteractionSource(args.state, sourceData);

            InputManager.Instance.RaiseSourceDetected(sourceData);
        }

        #endregion InteractionManager Events

        #region Raise GestureRecognizer Events

        private void GestureRecognizer_Tapped(TappedEventArgs args)
        {
            InputManager.Instance.RaiseInputClicked(GetOrAddInteractionSource(args.source), args.tapCount, (Handedness)args.source.handedness);
        }

        private void GestureRecognizer_HoldStarted(HoldStartedEventArgs args)
        {
            InputManager.Instance.RaiseHoldStarted(GetOrAddInteractionSource(args.source), (Handedness)args.source.handedness);
        }

        private void GestureRecognizer_HoldCanceled(HoldCanceledEventArgs args)
        {
            InputManager.Instance.RaiseHoldCanceled(GetOrAddInteractionSource(args.source), (Handedness)args.source.handedness);
        }

        private void GestureRecognizer_HoldCompleted(HoldCompletedEventArgs args)
        {
            InputManager.Instance.RaiseHoldCompleted(GetOrAddInteractionSource(args.source), (Handedness)args.source.handedness);
        }

        private void GestureRecognizer_ManipulationStarted(ManipulationStartedEventArgs args)
        {
            InputManager.Instance.RaiseManipulationStarted(GetOrAddInteractionSource(args.source), (Handedness)args.source.handedness);
        }

        private void GestureRecognizer_ManipulationUpdated(ManipulationUpdatedEventArgs args)
        {
            InputManager.Instance.RaiseManipulationUpdated(GetOrAddInteractionSource(args.source), args.cumulativeDelta, (Handedness)args.source.handedness);
        }

        private void GestureRecognizer_ManipulationCompleted(ManipulationCompletedEventArgs args)
        {
            InputManager.Instance.RaiseManipulationCompleted(GetOrAddInteractionSource(args.source), args.cumulativeDelta, (Handedness)args.source.handedness);
        }

        private void GestureRecognizer_ManipulationCanceled(ManipulationCanceledEventArgs args)
        {
            InputManager.Instance.RaiseManipulationCanceled(GetOrAddInteractionSource(args.source), (Handedness)args.source.handedness);
        }

        private void NavigationGestureRecognizer_NavigationStarted(NavigationStartedEventArgs args)
        {
            InputManager.Instance.RaiseNavigationStarted(GetOrAddInteractionSource(args.source), (Handedness)args.source.handedness);
        }

        private void NavigationGestureRecognizer_NavigationUpdated(NavigationUpdatedEventArgs args)
        {
            InputManager.Instance.RaiseNavigationUpdated(GetOrAddInteractionSource(args.source), args.normalizedOffset, (Handedness)args.source.handedness);
        }

        private void NavigationGestureRecognizer_NavigationCompleted(NavigationCompletedEventArgs args)
        {
            InputManager.Instance.RaiseNavigationCompleted(GetOrAddInteractionSource(args.source), args.normalizedOffset, (Handedness)args.source.handedness);
        }

        private void NavigationGestureRecognizer_NavigationCanceled(NavigationCanceledEventArgs args)
        {
            InputManager.Instance.RaiseNavigationCanceled(GetOrAddInteractionSource(args.source), (Handedness)args.source.handedness);
        }

        #endregion Raise GestureRecognizer Events

#endif

    }
}
