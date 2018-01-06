// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using UnityEngine.EventSystems;

#if UNITY_WSA
#if UNITY_2017_2_OR_NEWER
using UnityEngine.XR.WSA.Input;
#else
using UnityEngine.VR.WSA.Input;
#endif
using System.Collections.Generic;
#endif

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Input source for gestures and interaction source information from the WSA APIs, which gives access to various system-supported gestures
    /// and positional information for the various inputs that Windows gestures supports.
    /// This is mostly a wrapper on top of GestureRecognizer and InteractionManager.
    /// </summary>
    public class InteractionInputSources : Singleton<InteractionInputSources>
    {
        // This enumeration gives the manager two different ways to handle the recognizer. Both will
        // set up the recognizer. The first causes the recognizer to start
        // immediately. The second allows the recognizer to be manually started at a later time.
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
        private readonly Dictionary<uint, InputSource> sourceIdToData = new Dictionary<uint, InputSource>(4);

        #region IInputSource Capabilities and InputSource

        private class InputSource : IPointingSource
        {
#if UNITY_WSA
            public readonly InteractionSource Source;
            public InputSource(InteractionSource source)
            {
                Source = source;
                SourceId = source.id;
            }
#endif

            public uint SourceId { get; private set; }

            public SupportedInputInfo GetSupportedInputInfo()
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

            public bool SupportsInputInfo(SupportedInputInfo inputInfo)
            {
                return (GetSupportedInputInfo() & inputInfo) == inputInfo;
            }

            public bool InteractionEnabled { get; private set; }

            public float? ExtentOverride { get; private set; }

            public RayStep[] Rays { get; private set; }

            public LayerMask[] PrioritizedLayerMasksOverride { get; private set; }

            public PointerResult Result { get; set; }

            public void OnPreRaycast()
            {
            }

            public void OnPostRaycast()
            {
            }

            public bool OwnsInput(BaseEventData eventData)
            {
                throw new NotImplementedException();
            }

            public bool FocusLocked { get; set; }

            public bool TryGetPointerPosition(uint sourceId, out Vector3 position)
            {
                throw new NotImplementedException();
            }

            public bool TryGetPointingRay(uint sourceId, out Ray pointingRay)
            {
                throw new NotImplementedException();
            }

            public bool TryGetPointerRotation(uint sourceId, out Quaternion rotation)
            {
                throw new NotImplementedException();
            }

            public void ResetUpdatedBooleans()
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
        }

        private struct SourceCapability<TReading>
        {
            public bool IsSupported;
            public bool IsAvailable;
            public TReading CurrentReading;
        }

        private struct AxisButton2D
        {
            public bool Pressed;
            public Vector2 Position;

            public static AxisButton2D GetThumbstick(InteractionSourceState interactionSource)
            {
                return new AxisButton2D
                {
                    Pressed = interactionSource.thumbstickPressed,
                    Position = interactionSource.thumbstickPosition
                };
            }

            public static AxisButton2D GetTouchpad(InteractionSourceState interactionSource)
            {
                return new AxisButton2D
                {
                    Pressed = interactionSource.touchpadPressed,
                    Position = interactionSource.touchpadPosition
                };
            }
        }

        private struct TouchpadData
        {
            public AxisButton2D AxisButton;
            public bool Touched;

            public static TouchpadData GetTouchpad(InteractionSourceState interactionSource)
            {
                return new TouchpadData
                {
                    AxisButton = AxisButton2D.GetTouchpad(interactionSource),
                    Touched = interactionSource.touchpadTouched
                };
            }
        }

        private struct AxisButton1D
        {
            public bool Pressed;
            public double PressedAmount;

            public static AxisButton1D GetSelect(InteractionSourceState interactionSource)
            {
                return new AxisButton1D
                {
                    Pressed = interactionSource.selectPressed,
                    PressedAmount = interactionSource.selectPressedAmount,
                };
            }
        }

        #endregion IInputSource Capabilities and InputSource

        #region MonoBehaviour APIs

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
                InputManager.Instance.RaiseSourceLost(GetOrAddInteractionSource(states[i].source), string.Format("{0} {1}", states[i].source.handedness, states[i].source.kind));
                // NOTE: We don't care whether the source ID previously existed or not, so we blindly call Remove:
                sourceIdToData.Remove(states[i].source.id);
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

        #endregion MonoBehaviour APIs

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

        public void StartHaptics(uint sourceId, float intensity)
        {
#if UNITY_WSA
            InputSource sourceData;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData))
            {
                sourceData.Source.StartHaptics(intensity);
            }
#endif
        }

        public void StartHaptics(uint sourceId, float intensity, float durationInSeconds)
        {
#if UNITY_WSA
            InputSource sourceData;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData))
            {
                sourceData.Source.StartHaptics(intensity, durationInSeconds);
            }
#endif
        }

        public void StopHaptics(uint sourceId)
        {
#if UNITY_WSA
            InputSource sourceData;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData))
            {
                sourceData.Source.StopHaptics();
            }
#endif
        }

        #region BaseInputSource Implementations

        public SupportedInputInfo GetSupportedInputInfo(uint sourceId)
        {
            InputSource sourceData;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData))
            {
                return sourceData.GetSupportedInputInfo();
            }

            return SupportedInputInfo.None;
        }

#if UNITY_WSA
        public bool TryGetSourceKind(uint sourceId, out InteractionSourceKind sourceKind)
        {
            InputSource sourceData;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData))
            {
                sourceKind = sourceData.Source.kind;
                return true;
            }

            sourceKind = default(InteractionSourceKind);
            return false;
        }
#endif

        public bool TryGetPointingRay(uint sourceId, out Ray pointingRay)
        {
            InputSource sourceData;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData) && TryGetReading(sourceData.PointingRay, out pointingRay))
            {
                return true;
            }

            pointingRay = default(Ray);
            return false;
        }

        public bool TryGetPointerPosition(uint sourceId, out Vector3 position)
        {
            InputSource sourceData;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData) && TryGetReading(sourceData.PointerPosition, out position))
            {
                return true;
            }

            position = default(Vector3);
            return false;
        }

        public bool TryGetPointerRotation(uint sourceId, out Quaternion rotation)
        {
            InputSource sourceData;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData) && TryGetReading(sourceData.PointerRotation, out rotation))
            {
                return true;
            }

            rotation = default(Quaternion);
            return false;
        }

        public bool TryGetGripPosition(uint sourceId, out Vector3 position)
        {
            InputSource sourceData;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData) && TryGetReading(sourceData.GripPosition, out position))
            {
                return true;
            }

            position = default(Vector3);
            return false;
        }

        public bool TryGetGripRotation(uint sourceId, out Quaternion rotation)
        {
            InputSource sourceData;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData) && TryGetReading(sourceData.GripRotation, out rotation))
            {
                return true;
            }

            rotation = default(Quaternion);
            return false;
        }

        public bool TryGetThumbstick(uint sourceId, out bool thumbstickPressed, out Vector2 thumbstickPosition)
        {
            InputSource sourceData;
            AxisButton2D thumbstick;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData) && TryGetReading(sourceData.Thumbstick, out thumbstick))
            {
                thumbstickPressed = thumbstick.Pressed;
                thumbstickPosition = thumbstick.Position;
                return true;
            }

            thumbstickPressed = false;
            thumbstickPosition = Vector2.zero;
            return false;
        }

        public bool TryGetTouchpad(uint sourceId, out bool touchpadPressed, out bool touchpadTouched, out Vector2 touchpadPosition)
        {
            InputSource sourceData;
            TouchpadData touchpad;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData) && TryGetReading(sourceData.Touchpad, out touchpad))
            {
                touchpadPressed = touchpad.AxisButton.Pressed;
                touchpadTouched = touchpad.Touched;
                touchpadPosition = touchpad.AxisButton.Position;
                return true;
            }

            touchpadPressed = false;
            touchpadTouched = false;
            touchpadPosition = Vector2.zero;
            return false;
        }

        public bool TryGetSelect(uint sourceId, out bool selectPressed, out double selectPressedAmount)
        {
            InputSource sourceData;
            AxisButton1D select;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData) && TryGetReading(sourceData.Select, out select))
            {
                selectPressed = select.Pressed;
                selectPressedAmount = select.PressedAmount;
                return true;
            }

            selectPressed = false;
            selectPressedAmount = 0;
            return false;
        }

        public bool TryGetGrasp(uint sourceId, out bool graspPressed)
        {
            InputSource sourceData;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData) && TryGetReading(sourceData.Grasp, out graspPressed))
            {
                return true;
            }

            graspPressed = false;
            return false;
        }

        public bool TryGetMenu(uint sourceId, out bool menuPressed)
        {
            InputSource sourceData;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData) && TryGetReading(sourceData.Menu, out menuPressed))
            {
                return true;
            }

            menuPressed = false;
            return false;
        }

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


        #endregion BaseInputSource Implementations

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
                var sourceData = GetOrAddInteractionSource(states[i].source);
                InputManager.Instance.RaiseSourceDetected(sourceData, string.Format("{0} {1}", sourceData.Source.handedness, sourceData.Source.kind));
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

        /// <summary>
        /// Gets the source data for the specified interaction source if it already exists, otherwise creates it.
        /// </summary>
        /// <param name="interactionSource">Interaction source for which data should be retrieved.</param>
        /// <returns>The source data requested.</returns>
        private InputSource GetOrAddInteractionSource(InteractionSource interactionSource)
        {
            InputSource sourceData;
            if (!sourceIdToData.TryGetValue(interactionSource.id, out sourceData))
            {
                sourceData = new InputSource(interactionSource);
                sourceIdToData.Add(sourceData.SourceId, sourceData);
            }

            return sourceData;
        }

        /// <summary>
        /// Updates the source information.
        /// </summary>
        /// <param name="interactionSourceState">Interaction source to use to update the source information.</param>
        /// <param name="sourceData">InputSource structure to update.</param>
        private static void UpdateInteractionSource(InteractionSourceState interactionSourceState, InputSource sourceData)
        {
            Debug.Assert(interactionSourceState.source.id == sourceData.SourceId, "An UpdateSourceState call happened with mismatched source ID.");
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
                sourceData.PositionUpdated = !(sourceData.PointerPosition.CurrentReading.Equals(newPointerPosition) && sourceData.GripPosition.CurrentReading.Equals(newGripPosition));
            }

            sourceData.PointerPosition.CurrentReading = newPointerPosition;
            sourceData.GripPosition.CurrentReading = newGripPosition;

            Quaternion newPointerRotation;
            sourceData.PointerRotation.IsAvailable = interactionSourceState.sourcePose.TryGetRotation(out newPointerRotation, InteractionSourceNode.Pointer);

            // Using a heuristic for IsSupported, since the APIs don't yet support querying this capability directly.
            sourceData.PointerRotation.IsSupported |= sourceData.PointerRotation.IsAvailable;

            Quaternion newGripRotation;
            sourceData.GripRotation.IsAvailable = interactionSourceState.sourcePose.TryGetRotation(out newGripRotation, InteractionSourceNode.Grip);
            // Using a heuristic for IsSupported, since the APIs don't yet support querying this capability directly.
            sourceData.GripRotation.IsSupported |= sourceData.GripRotation.IsAvailable;

            if (CameraCache.Main.transform.parent != null)
            {
                newPointerRotation.eulerAngles = CameraCache.Main.transform.parent.TransformDirection(newPointerRotation.eulerAngles);
                newGripRotation.eulerAngles = CameraCache.Main.transform.parent.TransformDirection(newGripRotation.eulerAngles);
            }

            if (sourceData.PointerRotation.IsAvailable || sourceData.GripRotation.IsAvailable)
            {
                sourceData.RotationUpdated = !(sourceData.PointerRotation.CurrentReading.Equals(newPointerRotation) && sourceData.GripRotation.CurrentReading.Equals(newGripRotation));
            }

            sourceData.PointerRotation.CurrentReading = newPointerRotation;
            sourceData.GripRotation.CurrentReading = newGripRotation;

            Vector3 pointerForward = Vector3.zero;
            sourceData.PointingRay.IsSupported = interactionSourceState.source.supportsPointing;
            sourceData.PointingRay.IsAvailable = sourceData.PointerPosition.IsAvailable && interactionSourceState.sourcePose.TryGetForward(out pointerForward, InteractionSourceNode.Pointer);

            if (CameraCache.Main.transform.parent != null)
            {
                pointerForward = CameraCache.Main.transform.parent.TransformDirection(pointerForward);
            }

            sourceData.PointingRay.CurrentReading = new Ray(sourceData.PointerPosition.CurrentReading, pointerForward);

            sourceData.Thumbstick.IsSupported = interactionSourceState.source.supportsThumbstick;
            sourceData.Thumbstick.IsAvailable = sourceData.Thumbstick.IsSupported;
            if (sourceData.Thumbstick.IsAvailable)
            {
                AxisButton2D newThumbstick = AxisButton2D.GetThumbstick(interactionSourceState);
                sourceData.ThumbstickPositionUpdated = sourceData.Thumbstick.CurrentReading.Position != newThumbstick.Position;
                sourceData.Thumbstick.CurrentReading = newThumbstick;
            }
            else
            {
                sourceData.Thumbstick.CurrentReading = default(AxisButton2D);
            }

            sourceData.Touchpad.IsSupported = interactionSourceState.source.supportsTouchpad;
            sourceData.Touchpad.IsAvailable = sourceData.Touchpad.IsSupported;

            if (sourceData.Touchpad.IsAvailable)
            {
                TouchpadData newTouchpad = TouchpadData.GetTouchpad(interactionSourceState);
                sourceData.TouchpadPositionUpdated = !sourceData.Touchpad.CurrentReading.AxisButton.Position.Equals(newTouchpad.AxisButton.Position);
                sourceData.TouchpadTouchedUpdated = !sourceData.Touchpad.CurrentReading.Touched.Equals(newTouchpad.Touched);
                sourceData.Touchpad.CurrentReading = newTouchpad;
            }
            else
            {
                sourceData.Touchpad.CurrentReading = default(TouchpadData);
            }

            sourceData.Select.IsSupported = true; // All input mechanisms support "select".
            sourceData.Select.IsAvailable = sourceData.Select.IsSupported;
            AxisButton1D newSelect = AxisButton1D.GetSelect(interactionSourceState);
            sourceData.SelectPressedAmountUpdated = !sourceData.Select.CurrentReading.PressedAmount.Equals(newSelect.PressedAmount);
            sourceData.Select.CurrentReading = newSelect;

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
            InputSource sourceData = GetOrAddInteractionSource(args.state.source);

            sourceData.ResetUpdatedBooleans();

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
                    InputManager.Instance.RaiseOnInputDown(sourceData, (Handedness)args.state.source.handedness);
                }
                else
                {
                    InputManager.Instance.RaiseOnInputUp(sourceData, (Handedness)args.state.source.handedness);
                }
            }

            if (sourceData.SelectPressedAmountUpdated)
            {
                InputManager.Instance.RaiseOnInputPressed(sourceData, sourceData.Select.CurrentReading.PressedAmount, (Handedness)args.state.source.handedness);
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
            InputManager.Instance.RaiseSourceLost(GetOrAddInteractionSource(args.state.source), string.Format("{0} {1}", args.state.source.kind, args.state.source.id));

            // NOTE: We don't care whether the source ID previously existed or not, so we blindly call Remove:
            sourceIdToData.Remove(args.state.source.id);
        }

        private void InteractionManager_InteractionSourceDetected(InteractionSourceDetectedEventArgs args)
        {
            var sourceData = GetOrAddInteractionSource(args.state.source);

            // NOTE: We update the source state data, in case an app wants to query it on source detected.
            UpdateInteractionSource(args.state, sourceData);

            InputManager.Instance.RaiseSourceDetected(sourceData, string.Format("{0} {1}", args.state.source.kind, args.state.source.id));
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
