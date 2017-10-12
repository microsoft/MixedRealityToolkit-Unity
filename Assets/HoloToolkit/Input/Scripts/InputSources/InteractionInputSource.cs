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
    /// Input source for gestures and interaction source information from the WSA APIs, which gives access to various system-supported gestures
    /// and positional information for the various inputs that Windows gestures supports.
    /// This is mostly a wrapper on top of GestureRecognizer and InteractionManager.
    /// </summary>
    public class InteractionInputSource : BaseInputSource
    {
        // This enumeration gives the manager two different ways to handle the recognizer. Both will
        // set up the recognizer. The first causes the recognizer to start
        // immediately. The second allows the recognizer to be manually started at a later time.
        public enum RecognizerStartBehavior { AutoStart, ManualStart }

        [Tooltip("Whether the recognizer should be activated on start.")]
        public RecognizerStartBehavior RecognizerStart;

        [Tooltip("Set to true to use the use rails (guides) for the navigation gesture, as opposed to full 3D navigation.")]
        public bool UseRailsNavigation = false;

#if UNITY_WSA
        protected GestureRecognizer gestureRecognizer;
        protected GestureRecognizer navigationGestureRecognizer;
#endif

        #region IInputSource Capabilities and SourceData

#if UNITY_WSA
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
                    Position = interactionSource.thumbstickPosition,
                };
            }

            public static AxisButton2D GetTouchpad(InteractionSourceState interactionSource)
            {
                return new AxisButton2D
                {
                    Pressed = interactionSource.touchpadPressed,
                    Position = interactionSource.touchpadPosition,
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
                    Touched = interactionSource.touchpadTouched,
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

        /// <summary>
        /// Data for an interaction source.
        /// </summary>
        private class SourceData
        {
            public SourceData(InteractionSource interactionSource)
            {
                Source = interactionSource;
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

            public uint SourceId { get { return Source.id; } }
            public InteractionSourceKind SourceKind { get { return Source.kind; } }
            public InteractionSourceHandedness Handedness { get { return Source.handedness; } }
            public readonly InteractionSource Source;
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

        /// <summary>
        /// Dictionary linking each source ID to its data.
        /// </summary>
        private readonly Dictionary<uint, SourceData> sourceIdToData = new Dictionary<uint, SourceData>(4);
#endif
        #endregion IInputSource Capabilities and SourceData

        #region MonoBehaviour Functions

        private void Awake()
        {
#if UNITY_WSA
            gestureRecognizer = new GestureRecognizer();
            gestureRecognizer.Tapped += GestureRecognizer_Tapped;

            gestureRecognizer.HoldStarted += GestureRecognizer_HoldStarted;
            gestureRecognizer.HoldCompleted += GestureRecognizer_HoldCompleted;
            gestureRecognizer.HoldCanceled += GestureRecognizer_HoldCanceled;

            gestureRecognizer.ManipulationStarted += GestureRecognizer_ManipulationStarted;
            gestureRecognizer.ManipulationUpdated += GestureRecognizer_ManipulationUpdated;
            gestureRecognizer.ManipulationCompleted += GestureRecognizer_ManipulationCompleted;
            gestureRecognizer.ManipulationCanceled += GestureRecognizer_ManipulationCanceled;

            gestureRecognizer.SetRecognizableGestures(GestureSettings.Tap |
                                                      GestureSettings.ManipulationTranslate |
                                                      GestureSettings.Hold);

            // We need a separate gesture recognizer for navigation, since it isn't compatible with manipulation
            navigationGestureRecognizer = new GestureRecognizer();

            navigationGestureRecognizer.NavigationStarted += NavigationGestureRecognizer_NavigationStarted;
            navigationGestureRecognizer.NavigationUpdated += NavigationGestureRecognizer_NavigationUpdated;
            navigationGestureRecognizer.NavigationCompleted += NavigationGestureRecognizer_NavigationCompleted;
            navigationGestureRecognizer.NavigationCanceled += NavigationGestureRecognizer_NavigationCanceled;

            if (UseRailsNavigation)
            {
                navigationGestureRecognizer.SetRecognizableGestures(GestureSettings.NavigationRailsX |
                                                                    GestureSettings.NavigationRailsY |
                                                                    GestureSettings.NavigationRailsZ);
            }
            else
            {
                navigationGestureRecognizer.SetRecognizableGestures(GestureSettings.NavigationX |
                                                                    GestureSettings.NavigationY |
                                                                    GestureSettings.NavigationZ);
            }
#endif
        }

        protected virtual void OnDestroy()
        {
#if UNITY_WSA
            InteractionManager.InteractionSourceUpdated -= InteractionManager_InteractionSourceUpdated;

            InteractionManager.InteractionSourceReleased -= InteractionManager_InteractionSourceReleased;
            InteractionManager.InteractionSourcePressed -= InteractionManager_InteractionSourcePressed;

            InteractionManager.InteractionSourceLost -= InteractionManager_InteractionSourceLost;
            InteractionManager.InteractionSourceDetected -= InteractionManager_InteractionSourceDetected;

            if (gestureRecognizer != null)
            {
                gestureRecognizer.Tapped -= GestureRecognizer_Tapped;

                gestureRecognizer.HoldStarted -= GestureRecognizer_HoldStarted;
                gestureRecognizer.HoldCompleted -= GestureRecognizer_HoldCompleted;
                gestureRecognizer.HoldCanceled -= GestureRecognizer_HoldCanceled;

                gestureRecognizer.ManipulationStarted -= GestureRecognizer_ManipulationStarted;
                gestureRecognizer.ManipulationUpdated -= GestureRecognizer_ManipulationUpdated;
                gestureRecognizer.ManipulationCompleted -= GestureRecognizer_ManipulationCompleted;
                gestureRecognizer.ManipulationCanceled -= GestureRecognizer_ManipulationCanceled;

                gestureRecognizer.Dispose();
            }

            if (navigationGestureRecognizer != null)
            {
                navigationGestureRecognizer.NavigationStarted -= NavigationGestureRecognizer_NavigationStarted;
                navigationGestureRecognizer.NavigationUpdated -= NavigationGestureRecognizer_NavigationUpdated;
                navigationGestureRecognizer.NavigationCompleted -= NavigationGestureRecognizer_NavigationCompleted;
                navigationGestureRecognizer.NavigationCanceled -= NavigationGestureRecognizer_NavigationCanceled;

                navigationGestureRecognizer.Dispose();
            }
#endif
        }

        protected override void OnEnableAfterStart()
        {
            base.OnEnableAfterStart();

#if UNITY_WSA
            if (RecognizerStart == RecognizerStartBehavior.AutoStart)
            {
                StartGestureRecognizers();
            }

            foreach (InteractionSourceState state in InteractionManager.GetCurrentReading())
            {
                GetOrAddSourceData(state.source);
                InputManager.Instance.RaiseSourceDetected(this, state.source.id);
            }

            InteractionManager.InteractionSourceUpdated += InteractionManager_InteractionSourceUpdated;

            InteractionManager.InteractionSourceReleased += InteractionManager_InteractionSourceReleased;
            InteractionManager.InteractionSourcePressed += InteractionManager_InteractionSourcePressed;

            InteractionManager.InteractionSourceLost += InteractionManager_InteractionSourceLost;
            InteractionManager.InteractionSourceDetected += InteractionManager_InteractionSourceDetected;
#else
            RecognizerStart = RecognizerStartBehavior.ManualStart;
#endif
        }

        protected override void OnDisableAfterStart()
        {
            StopGestureRecognizers();
#if UNITY_WSA
            InteractionManager.InteractionSourceUpdated -= InteractionManager_InteractionSourceUpdated;

            InteractionManager.InteractionSourceReleased -= InteractionManager_InteractionSourceReleased;
            InteractionManager.InteractionSourcePressed -= InteractionManager_InteractionSourcePressed;

            InteractionManager.InteractionSourceLost -= InteractionManager_InteractionSourceLost;
            InteractionManager.InteractionSourceDetected -= InteractionManager_InteractionSourceDetected;

            foreach (InteractionSourceState iss in InteractionManager.GetCurrentReading())
            {
                // NOTE: We don't care whether the source ID previously existed or not, so we blindly call Remove:
                sourceIdToData.Remove(iss.source.id);
                InputManager.Instance.RaiseSourceLost(this, iss.source.id);
            }
#endif

            base.OnDisableAfterStart();
        }

        #endregion MonoBehaviour Functions

        public void StartGestureRecognizers()
        {
#if UNITY_WSA
            if (gestureRecognizer != null)
            {
                gestureRecognizer.StartCapturingGestures();
            }

            if (navigationGestureRecognizer != null)
            {
                navigationGestureRecognizer.StartCapturingGestures();
            }
#endif
        }

        public void StopGestureRecognizers()
        {
#if UNITY_WSA
            if (gestureRecognizer != null)
            {
                gestureRecognizer.StopCapturingGestures();
            }

            if (navigationGestureRecognizer != null)
            {
                navigationGestureRecognizer.StopCapturingGestures();
            }
#endif
        }

        #region BaseInputSource implementations

        public override SupportedInputInfo GetSupportedInputInfo(uint sourceId)
        {
            var retVal = SupportedInputInfo.None;
#if UNITY_WSA
            SourceData sourceData;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData))
            {
                retVal |= GetSupportFlag(sourceData.PointerPosition, SupportedInputInfo.Position);
                retVal |= GetSupportFlag(sourceData.PointerRotation, SupportedInputInfo.Rotation);
                retVal |= GetSupportFlag(sourceData.PointingRay, SupportedInputInfo.Pointing);
                retVal |= GetSupportFlag(sourceData.Thumbstick, SupportedInputInfo.Thumbstick);
                retVal |= GetSupportFlag(sourceData.Touchpad, SupportedInputInfo.Touchpad);
                retVal |= GetSupportFlag(sourceData.Select, SupportedInputInfo.Select);
                retVal |= GetSupportFlag(sourceData.Menu, SupportedInputInfo.Menu);
                retVal |= GetSupportFlag(sourceData.Grasp, SupportedInputInfo.Grasp);
            }
#endif
            return retVal;
        }

        public override bool TryGetSourceKind(uint sourceId, out InteractionSourceInfo sourceKind)
        {
#if UNITY_WSA
            SourceData sourceData;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData))
            {
                sourceKind = (InteractionSourceInfo)sourceData.SourceKind;
                return true;
            }
#endif

            sourceKind = default(InteractionSourceInfo);
            return false;
        }

        public override bool TryGetPointerPosition(uint sourceId, out Vector3 position)
        {
#if UNITY_WSA
            SourceData sourceData;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData) && TryGetReading(sourceData.PointerPosition, out position))
            {
                return true;
            }
#endif

            position = default(Vector3);
            return false;
        }

        public override bool TryGetPointerRotation(uint sourceId, out Quaternion rotation)
        {
#if UNITY_WSA
            SourceData sourceData;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData) && TryGetReading(sourceData.PointerRotation, out rotation))
            {
                return true;
            }
#endif

            rotation = default(Quaternion);
            return false;
        }

        public override bool TryGetPointingRay(uint sourceId, out Ray pointingRay)
        {
#if UNITY_WSA
            SourceData sourceData;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData) && TryGetReading(sourceData.PointingRay, out pointingRay))
            {
                return true;
            }
#endif

            pointingRay = default(Ray);
            return false;
        }

        public override bool TryGetGripPosition(uint sourceId, out Vector3 position)
        {
#if UNITY_WSA
            SourceData sourceData;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData) && TryGetReading(sourceData.GripPosition, out position))
            {
                return true;
            }
#endif

            position = default(Vector3);
            return false;
        }

        public override bool TryGetGripRotation(uint sourceId, out Quaternion rotation)
        {
#if UNITY_WSA
            SourceData sourceData;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData) && TryGetReading(sourceData.GripRotation, out rotation))
            {
                return true;
            }
#endif

            rotation = default(Quaternion);
            return false;
        }

        public override bool TryGetThumbstick(uint sourceId, out bool thumbstickPressed, out Vector2 thumbstickPosition)
        {
#if UNITY_WSA
            SourceData sourceData;
            AxisButton2D thumbstick;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData) && TryGetReading(sourceData.Thumbstick, out thumbstick))
            {
                thumbstickPressed = thumbstick.Pressed;
                thumbstickPosition = thumbstick.Position;
                return true;
            }
#endif

            thumbstickPressed = false;
            thumbstickPosition = Vector2.zero;
            return false;
        }

        public override bool TryGetTouchpad(uint sourceId, out bool touchpadPressed, out bool touchpadTouched, out Vector2 touchpadPosition)
        {
#if UNITY_WSA
            SourceData sourceData;
            TouchpadData touchpad;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData) && TryGetReading(sourceData.Touchpad, out touchpad))
            {
                touchpadPressed = touchpad.AxisButton.Pressed;
                touchpadTouched = touchpad.Touched;
                touchpadPosition = touchpad.AxisButton.Position;
                return true;
            }
#endif

            touchpadPressed = false;
            touchpadTouched = false;
            touchpadPosition = Vector2.zero;
            return false;
        }

        public override bool TryGetSelect(uint sourceId, out bool selectPressed, out double selectPressedAmount)
        {
#if UNITY_WSA
            SourceData sourceData;
            AxisButton1D select;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData) && TryGetReading(sourceData.Select, out select))
            {
                selectPressed = select.Pressed;
                selectPressedAmount = select.PressedAmount;
                return true;
            }
#endif

            selectPressed = false;
            selectPressedAmount = 0;
            return false;
        }

        public override bool TryGetGrasp(uint sourceId, out bool graspPressed)
        {
#if UNITY_WSA
            SourceData sourceData;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData) && TryGetReading(sourceData.Grasp, out graspPressed))
            {
                return true;
            }
#endif

            graspPressed = false;
            return false;
        }

        public override bool TryGetMenu(uint sourceId, out bool menuPressed)
        {

#if UNITY_WSA
            SourceData sourceData;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData) && TryGetReading(sourceData.Menu, out menuPressed))
            {
                return true;
            }
#endif

            menuPressed = false;
            return false;
        }

#if UNITY_WSA
        private bool TryGetReading<TReading>(SourceCapability<TReading> capability, out TReading reading)
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

        private SupportedInputInfo GetSupportFlag<TReading>(SourceCapability<TReading> capability, SupportedInputInfo flagIfSupported)
        {
            return (capability.IsSupported ? flagIfSupported : SupportedInputInfo.None);
        }
#endif
        #endregion

        public void StartHaptics(uint sourceId, float intensity)
        {
#if UNITY_WSA
            SourceData sourceData;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData))
            {
                sourceData.Source.StartHaptics(intensity);
            }
#endif
        }

        public void StartHaptics(uint sourceId, float intensity, float durationInSeconds)
        {
#if UNITY_WSA
            SourceData sourceData;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData))
            {
                sourceData.Source.StartHaptics(intensity, durationInSeconds);
            }
#endif
        }

        public void StopHaptics(uint sourceId)
        {
#if UNITY_WSA
            SourceData sourceData;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData))
            {
                sourceData.Source.StopHaptics();
            }
#endif
        }

#if UNITY_WSA
        #region InteractionManager Events

        private void InteractionManager_InteractionSourceUpdated(InteractionSourceUpdatedEventArgs args)
        {
            SourceData sourceData = GetOrAddSourceData(args.state.source);

            sourceData.ResetUpdatedBooleans();

            UpdateSourceData(args.state, sourceData);

            if (sourceData.PositionUpdated)
            {
                InputManager.Instance.RaiseSourcePositionChanged(this, sourceData.SourceId, sourceData.PointerPosition.CurrentReading, sourceData.GripPosition.CurrentReading);
            }

            if (sourceData.RotationUpdated)
            {
                InputManager.Instance.RaiseSourceRotationChanged(this, sourceData.SourceId, sourceData.PointerRotation.CurrentReading, sourceData.GripRotation.CurrentReading);
            }

            if (sourceData.ThumbstickPositionUpdated)
            {
                InputManager.Instance.RaiseInputPositionChanged(this, sourceData.SourceId, InteractionSourcePressInfo.Thumbstick, sourceData.Thumbstick.CurrentReading.Position);
            }

            if (sourceData.TouchpadPositionUpdated)
            {
                InputManager.Instance.RaiseInputPositionChanged(this, sourceData.SourceId, InteractionSourcePressInfo.Touchpad, sourceData.Touchpad.CurrentReading.AxisButton.Position);
            }

            if (sourceData.TouchpadTouchedUpdated)
            {
                if (sourceData.Touchpad.CurrentReading.Touched)
                {
                    InputManager.Instance.RaiseTouchpadTouched(this, sourceData.SourceId);
                }
                else
                {
                    InputManager.Instance.RaiseTouchpadReleased(this, sourceData.SourceId);
                }
            }

            if (sourceData.SelectPressedAmountUpdated)
            {
                InputManager.Instance.RaiseSelectPressedAmountChanged(this, sourceData.SourceId, sourceData.Select.CurrentReading.PressedAmount);
            }
        }

        private void InteractionManager_InteractionSourceReleased(InteractionSourceReleasedEventArgs args)
        {
            InputManager.Instance.RaiseSourceUp(this, args.state.source.id, (InteractionSourcePressInfo)args.pressType);
        }

        private void InteractionManager_InteractionSourcePressed(InteractionSourcePressedEventArgs args)
        {
            InputManager.Instance.RaiseSourceDown(this, args.state.source.id, (InteractionSourcePressInfo)args.pressType);
        }

        private void InteractionManager_InteractionSourceLost(InteractionSourceLostEventArgs args)
        {
            // NOTE: We don't care whether the source ID previously existed or not, so we blindly call Remove:
            sourceIdToData.Remove(args.state.source.id);

            InputManager.Instance.RaiseSourceLost(this, args.state.source.id);
        }

        private void InteractionManager_InteractionSourceDetected(InteractionSourceDetectedEventArgs args)
        {
            // NOTE: We update the source state data, in case an app wants to query it on source detected.
            UpdateSourceData(args.state, GetOrAddSourceData(args.state.source));

            InputManager.Instance.RaiseSourceDetected(this, args.state.source.id);
        }

        #endregion InteractionManager Events

        /// <summary>
        /// Gets the source data for the specified interaction source if it already exists, otherwise creates it.
        /// </summary>
        /// <param name="interactionSource">Interaction source for which data should be retrieved.</param>
        /// <returns>The source data requested.</returns>
        private SourceData GetOrAddSourceData(InteractionSource interactionSource)
        {
            SourceData sourceData;
            if (!sourceIdToData.TryGetValue(interactionSource.id, out sourceData))
            {
                sourceData = new SourceData(interactionSource);
                sourceIdToData.Add(sourceData.SourceId, sourceData);

                // TODO: robertes: whenever we end up adding, should we first synthesize a SourceDetected? Or
                //       perhaps if we keep strict track of all sources, we should never need to just-in-time add anymore.
            }

            return sourceData;
        }

        /// <summary>
        /// Updates the source information.
        /// </summary>
        /// <param name="interactionSourceState">Interaction source to use to update the source information.</param>
        /// <param name="sourceData">SourceData structure to update.</param>
        private void UpdateSourceData(InteractionSourceState interactionSourceState, SourceData sourceData)
        {
            Debug.Assert(interactionSourceState.source.id == sourceData.SourceId, "An UpdateSourceState call happened with mismatched source ID.");
            Debug.Assert(interactionSourceState.source.kind == sourceData.SourceKind, "An UpdateSourceState call happened with mismatched source kind.");

            Vector3 newPointerPosition;
            sourceData.PointerPosition.IsAvailable = interactionSourceState.sourcePose.TryGetPosition(out newPointerPosition, InteractionSourceNode.Pointer);
            // Using a heuristic for IsSupported, since the APIs don't yet support querying this capability directly.
            sourceData.PointerPosition.IsSupported |= sourceData.PointerPosition.IsAvailable;

            Vector3 newGripPosition;
            sourceData.GripPosition.IsAvailable = interactionSourceState.sourcePose.TryGetPosition(out newGripPosition, InteractionSourceNode.Grip);
            // Using a heuristic for IsSupported, since the APIs don't yet support querying this capability directly.
            sourceData.GripPosition.IsSupported |= sourceData.GripPosition.IsAvailable;

            if (Camera.main.transform.parent != null)
            {
                newPointerPosition = Camera.main.transform.parent.TransformPoint(newPointerPosition);
                newGripPosition = Camera.main.transform.parent.TransformPoint(newGripPosition);
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

            if (Camera.main.transform.parent != null)
            {
                newPointerRotation.eulerAngles = Camera.main.transform.parent.TransformDirection(newPointerRotation.eulerAngles);
                newGripRotation.eulerAngles = Camera.main.transform.parent.TransformDirection(newGripRotation.eulerAngles);
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

            if (Camera.main.transform.parent != null)
            {
                pointerForward = Camera.main.transform.parent.TransformDirection(pointerForward);
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
            sourceData.Grasp.CurrentReading = (sourceData.Grasp.IsAvailable && interactionSourceState.grasped);

            sourceData.Menu.IsSupported = interactionSourceState.source.supportsMenu;
            sourceData.Menu.IsAvailable = sourceData.Menu.IsSupported;
            sourceData.Menu.CurrentReading = (sourceData.Menu.IsAvailable && interactionSourceState.menuPressed);
        }

        #region Raise GestureRecognizer Events

        // TODO: robertes: Should these also cause source state data to be stored/updated? What about SourceDetected synthesized events?

        protected void GestureRecognizer_Tapped(TappedEventArgs obj)
        {
            InputManager.Instance.RaiseInputClicked(this, obj.source.id, InteractionSourcePressInfo.Select, obj.tapCount);
        }

        protected void GestureRecognizer_HoldStarted(HoldStartedEventArgs obj)
        {
            InputManager.Instance.RaiseHoldStarted(this, obj.source.id);
        }

        protected void GestureRecognizer_HoldCanceled(HoldCanceledEventArgs obj)
        {
            InputManager.Instance.RaiseHoldCanceled(this, obj.source.id);
        }

        protected void GestureRecognizer_HoldCompleted(HoldCompletedEventArgs obj)
        {
            InputManager.Instance.RaiseHoldCompleted(this, obj.source.id);
        }

        protected void GestureRecognizer_ManipulationStarted(ManipulationStartedEventArgs obj)
        {
            InputManager.Instance.RaiseManipulationStarted(this, obj.source.id);
        }

        protected void GestureRecognizer_ManipulationUpdated(ManipulationUpdatedEventArgs obj)
        {
            InputManager.Instance.RaiseManipulationUpdated(this, obj.source.id, obj.cumulativeDelta);
        }

        protected void GestureRecognizer_ManipulationCompleted(ManipulationCompletedEventArgs obj)
        {
            InputManager.Instance.RaiseManipulationCompleted(this, obj.source.id, obj.cumulativeDelta);
        }

        protected void GestureRecognizer_ManipulationCanceled(ManipulationCanceledEventArgs obj)
        {
            InputManager.Instance.RaiseManipulationCanceled(this, obj.source.id);
        }

        protected void NavigationGestureRecognizer_NavigationStarted(NavigationStartedEventArgs obj)
        {
            InputManager.Instance.RaiseNavigationStarted(this, obj.source.id);
        }

        protected void NavigationGestureRecognizer_NavigationUpdated(NavigationUpdatedEventArgs obj)
        {
            InputManager.Instance.RaiseNavigationUpdated(this, obj.source.id, obj.normalizedOffset);
        }

        protected void NavigationGestureRecognizer_NavigationCompleted(NavigationCompletedEventArgs obj)
        {
            InputManager.Instance.RaiseNavigationCompleted(this, obj.source.id, obj.normalizedOffset);
        }

        protected void NavigationGestureRecognizer_NavigationCanceled(NavigationCanceledEventArgs obj)
        {
            InputManager.Instance.RaiseNavigationCanceled(this, obj.source.id);
        }

        #endregion //Raise GestureRecognizer Events
#endif

    }
}