// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Input source for gestures information from the WSA APIs, which gives access to various system-supported gestures
    /// and positional information for the various inputs that Windows gestures supports.
    /// This is mostly a wrapper on top of GestureRecognizer and InteractionManager.
    /// </summary>
    public class GesturesInput : BaseInputSource
    {
        // This enumeration gives the manager two different ways to handle the recognizer. Both will
        // set up the recognizer. The first causes the recognizer to start
        // immediately. The second allows the recognizer to be manually started at a later time.
        public enum RecognizerStartBehavior { AutoStart, ManualStart }

        [Tooltip("Whether the recognizer should be activated on start.")]
        public RecognizerStartBehavior RecognizerStart;

        [Tooltip("Set to true to use the use rails (guides) for the navigation gesture, as opposed to full 3D navigation.")]
        public bool UseRailsNavigation = false;

        protected GestureRecognizer gestureRecognizer;
        protected GestureRecognizer navigationGestureRecognizer;

        #region IInputSource Capabilities and SourceData

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
                SourceId = interactionSource.id;
                SourceKind = interactionSource.kind;
            }

            public readonly uint SourceId;
            public readonly InteractionSourceKind SourceKind;
            public SourceCapability<Vector3> PointerPosition;
            public SourceCapability<Quaternion> PointerRotation;
            public SourceCapability<Ray> PointerRay;
            public SourceCapability<Vector3> GripPosition;
            public SourceCapability<Quaternion> GripRotation;
            public SourceCapability<Ray> GripRay;
            public SourceCapability<AxisButton2D> Thumbstick;
            public SourceCapability<TouchpadData> Touchpad;
            public SourceCapability<AxisButton1D> Select;
            public SourceCapability<bool> Grasp;
            public SourceCapability<bool> Menu;
        }

        /// <summary>
        /// Dictionary linking each source ID to its data.
        /// </summary>
        private readonly Dictionary<uint, SourceData> sourceIdToData = new Dictionary<uint, SourceData>(4);

        #endregion

        protected override void Start()
        {
            base.Start();

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

            if (RecognizerStart == RecognizerStartBehavior.AutoStart)
            {
                StartGestureRecognizer();
            }
        }

        protected virtual void OnDestroy()
        {
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
        }

        protected override void OnEnableAfterStart()
        {
            base.OnEnableAfterStart();

            if (RecognizerStart == RecognizerStartBehavior.AutoStart)
            {
                StartGestureRecognizer();
            }

            foreach (InteractionSourceState iss in InteractionManager.GetCurrentReading())
            {
                GetOrAddSourceData(iss.source);
                InputManager.Instance.RaiseSourceDetected(this, iss.source.id);
            }

            InteractionManager.InteractionSourceUpdated += InteractionManager_InteractionSourceUpdated;

            InteractionManager.InteractionSourceReleased += InteractionManager_InteractionSourceReleased;
            InteractionManager.InteractionSourcePressed += InteractionManager_InteractionSourcePressed;

            InteractionManager.InteractionSourceLost += InteractionManager_InteractionSourceLost;
            InteractionManager.InteractionSourceDetected += InteractionManager_InteractionSourceDetected;
        }

        protected override void OnDisableAfterStart()
        {
            StopGestureRecognizer();

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

            base.OnDisableAfterStart();
        }

        public void StartGestureRecognizer()
        {
            if (gestureRecognizer != null)
            {
                gestureRecognizer.StartCapturingGestures();
            }

            if (navigationGestureRecognizer != null)
            {
                navigationGestureRecognizer.StartCapturingGestures();
            }
        }

        public void StopGestureRecognizer()
        {
            if (gestureRecognizer != null)
            {
                gestureRecognizer.StopCapturingGestures();
            }

            if (navigationGestureRecognizer != null)
            {
                navigationGestureRecognizer.StopCapturingGestures();
            }
        }

        #region BaseInputSource implementations

        public override SupportedInputInfo GetSupportedInputInfo(uint sourceId)
        {
            SupportedInputInfo retVal = SupportedInputInfo.None;

            SourceData sourceData;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData))
            {
                retVal |= GetSupportFlag(sourceData.PointerPosition, SupportedInputInfo.Position);
                retVal |= GetSupportFlag(sourceData.PointerRotation, SupportedInputInfo.Rotation);
                retVal |= GetSupportFlag(sourceData.PointerRay, SupportedInputInfo.Ray);
                retVal |= GetSupportFlag(sourceData.Thumbstick, SupportedInputInfo.Thumbstick);
                retVal |= GetSupportFlag(sourceData.Touchpad, SupportedInputInfo.Touchpad);
                retVal |= GetSupportFlag(sourceData.Select, SupportedInputInfo.Select);
                retVal |= GetSupportFlag(sourceData.Menu, SupportedInputInfo.Menu);
                retVal |= GetSupportFlag(sourceData.Grasp, SupportedInputInfo.Grasp);
            }

            return retVal;
        }

        public override bool TryGetSourceKind(uint sourceId, out InteractionSourceKind sourceKind)
        {
            SourceData sourceData;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData))
            {
                sourceKind = sourceData.SourceKind;
                return true;
            }
            else
            {
                sourceKind = default(InteractionSourceKind);
                return false;
            }
        }

        public override bool TryGetPointerPosition(uint sourceId, out Vector3 position)
        {
            SourceData sourceData;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData) && TryGetReading(sourceData.PointerPosition, out position))
            {
                return true;
            }
            else
            {
                position = default(Vector3);
                return false;
            }
        }

        public override bool TryGetPointerRotation(uint sourceId, out Quaternion rotation)
        {
            SourceData sourceData;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData) && TryGetReading(sourceData.PointerRotation, out rotation))
            {
                return true;
            }
            else
            {
                rotation = default(Quaternion);
                return false;
            }
        }

        public override bool TryGetPointerRay(uint sourceId, out Ray pointerRay)
        {
            SourceData sourceData;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData) && TryGetReading(sourceData.PointerRay, out pointerRay))
            {
                return true;
            }
            else
            {
                pointerRay = default(Ray);
                return false;
            }
        }

        public override bool TryGetGripPosition(uint sourceId, out Vector3 position)
        {
            SourceData sourceData;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData) && TryGetReading(sourceData.GripPosition, out position))
            {
                return true;
            }
            else
            {
                position = default(Vector3);
                return false;
            }
        }

        public override bool TryGetGripRotation(uint sourceId, out Quaternion rotation)
        {
            SourceData sourceData;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData) && TryGetReading(sourceData.GripRotation, out rotation))
            {
                return true;
            }
            else
            {
                rotation = default(Quaternion);
                return false;
            }
        }

        public override bool TryGetGripRay(uint sourceId, out Ray gripRay)
        {
            SourceData sourceData;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData) && TryGetReading(sourceData.GripRay, out gripRay))
            {
                return true;
            }
            else
            {
                gripRay = default(Ray);
                return false;
            }
        }

        public override bool TryGetThumbstick(uint sourceId, out bool thumbstickPressed, out Vector2 thumbstickPosition)
        {
            SourceData sourceData;
            AxisButton2D thumbstick;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData) && TryGetReading(sourceData.Thumbstick, out thumbstick))
            {
                thumbstickPressed = thumbstick.Pressed;
                thumbstickPosition = thumbstick.Position;
                return true;
            }
            else
            {
                thumbstickPressed = false;
                thumbstickPosition = Vector2.zero;
                return false;
            }
        }

        public override bool TryGetTouchpad(uint sourceId, out bool touchpadPressed, out bool touchpadTouched, out Vector2 touchpadPosition)
        {
            SourceData sourceData;
            TouchpadData touchpad;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData) && TryGetReading(sourceData.Touchpad, out touchpad))
            {
                touchpadPressed = touchpad.AxisButton.Pressed;
                touchpadTouched = touchpad.Touched;
                touchpadPosition = touchpad.AxisButton.Position;
                return true;
            }
            else
            {
                touchpadPressed = false;
                touchpadTouched = false;
                touchpadPosition = Vector2.zero;
                return false;
            }
        }

        public override bool TryGetSelect(uint sourceId, out bool selectPressed, out double selectPressedAmount)
        {
            SourceData sourceData;
            AxisButton1D select;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData) && TryGetReading(sourceData.Select, out select))
            {
                selectPressed = select.Pressed;
                selectPressedAmount = select.PressedAmount;
                return true;
            }
            else
            {
                selectPressed = false;
                selectPressedAmount = 0;
                return false;
            }
        }

        public override bool TryGetGrasp(uint sourceId, out bool graspPressed)
        {
            SourceData sourceData;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData) && TryGetReading(sourceData.Grasp, out graspPressed))
            {
                return true;
            }
            else
            {
                graspPressed = false;
                return false;
            }
        }

        public override bool TryGetMenu(uint sourceId, out bool menuPressed)
        {
            SourceData sourceData;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData) && TryGetReading(sourceData.Menu, out menuPressed))
            {
                return true;
            }
            else
            {
                menuPressed = false;
                return false;
            }
        }

        private bool TryGetReading<TReading>(SourceCapability<TReading> capability, out TReading reading)
        {
            if (capability.IsAvailable)
            {
                Debug.Assert(capability.IsSupported);

                reading = capability.CurrentReading;
                return true;
            }
            else
            {
                reading = default(TReading);
                return false;
            }
        }

        private SupportedInputInfo GetSupportFlag<TReading>(SourceCapability<TReading> capability, SupportedInputInfo flagIfSupported)
        {
            return (capability.IsSupported ? flagIfSupported : SupportedInputInfo.None);
        }

        #endregion

        private void InteractionManager_InteractionSourceUpdated(InteractionSourceUpdatedEventArgs args)
        {
            UpdateSourceState(args.state, GetOrAddSourceData(args.state.source));

            // TODO: robertes: raise update events?  Perhaps part of UpdateSourceState and have UpdateSourceState measure deltas and raise events iff changed... rename it to UpdateSourceStateAndRaiseUpdateEvents? ... and don't forget the TouchpadTouched events.
            // TODO: robertes: We should call UpdateSourceState in other handlers, too, right?  All handlers?
        }

        private void InteractionManager_InteractionSourceReleased(InteractionSourceReleasedEventArgs args)
        {
            InputManager.Instance.RaiseSourceUp(this, args.state.source.id, args.pressType);
        }

        private void InteractionManager_InteractionSourcePressed(InteractionSourcePressedEventArgs args)
        {
            InputManager.Instance.RaiseSourceDown(this, args.state.source.id, args.pressType);
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
            UpdateSourceState(args.state, GetOrAddSourceData(args.state.source));
            InputManager.Instance.RaiseSourceDetected(this, args.state.source.id);
        }

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

                // TODO: robertes: whenever we end up adding, should we first synthesize a SourceDetected?  Or
                //       perhaps if we keep strict track of all sources, we should never need to just-in-time add anymore.
            }

            return sourceData;
        }

        /// <summary>
        /// Updates the source information.
        /// </summary>
        /// <param name="interactionSourceState">Interaction source to use to update the source information.</param>
        /// <param name="sourceData">SourceData structure to update.</param>
        private void UpdateSourceState(InteractionSourceState interactionSourceState, SourceData sourceData)
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

            if (sourceData.PointerPosition.IsAvailable || sourceData.GripPosition.IsAvailable)
            {
                if (!(sourceData.PointerPosition.CurrentReading.Equals(newPointerPosition) && sourceData.GripPosition.CurrentReading.Equals(newGripPosition)))
                {
                    // TODO: Raising events here may cause reentrancy complexity. Consider delaying all event-raising till
                    //       after all updates are stored. Alternatively, consider switching from polling to responding to
                    //       InteractionManager events.
                    InputManager.Instance.RaiseSourcePositionChanged(this, sourceData.SourceId, newPointerPosition, newGripPosition);
                }
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
            if (sourceData.PointerRotation.IsAvailable || sourceData.GripRotation.IsAvailable)
            {
                if (!(sourceData.PointerRotation.CurrentReading.Equals(newPointerRotation) && sourceData.GripRotation.CurrentReading.Equals(newGripRotation)))
                {
                    InputManager.Instance.RaiseSourceRotationChanged(this, sourceData.SourceId, newPointerRotation, newGripRotation);
                }
            }
            sourceData.PointerRotation.CurrentReading = newPointerRotation;
            sourceData.GripRotation.CurrentReading = newGripRotation;

            Vector3 pointerForward = Vector3.zero, gripForward = Vector3.zero;
            sourceData.PointerRay.IsSupported = sourceData.GripRay.IsSupported = interactionSourceState.source.supportsPointing;
            sourceData.PointerRay.IsAvailable = sourceData.PointerPosition.IsAvailable && interactionSourceState.sourcePose.TryGetForward(out pointerForward, InteractionSourceNode.Pointer);
            sourceData.PointerRay.CurrentReading = new Ray(sourceData.PointerPosition.CurrentReading, pointerForward);

            sourceData.GripRay.IsAvailable = sourceData.GripPosition.IsAvailable && interactionSourceState.sourcePose.TryGetForward(out gripForward, InteractionSourceNode.Grip);
            sourceData.GripRay.CurrentReading = new Ray(sourceData.GripPosition.CurrentReading, gripForward);

            sourceData.Thumbstick.IsSupported = interactionSourceState.source.supportsThumbstick;
            sourceData.Thumbstick.IsAvailable = sourceData.Thumbstick.IsSupported;
            if (sourceData.Thumbstick.IsAvailable)
            {
                AxisButton2D newThumbstick = AxisButton2D.GetThumbstick(interactionSourceState);
                if (sourceData.Thumbstick.CurrentReading.Position != newThumbstick.Position)
                {
                    InputManager.Instance.RaiseInputPositionChanged(this, sourceData.SourceId, InteractionSourcePressType.Thumbstick, newThumbstick.Position);
                }
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
                if (sourceData.Touchpad.CurrentReading.AxisButton.Position != newTouchpad.AxisButton.Position)
                {
                    InputManager.Instance.RaiseInputPositionChanged(this, sourceData.SourceId, InteractionSourcePressType.Touchpad, newTouchpad.AxisButton.Position);
                }
                if (sourceData.Touchpad.CurrentReading.Touched != newTouchpad.Touched)
                {
                    if (newTouchpad.Touched)
                    {
                        InputManager.Instance.RaiseTouchpadTouched(this, sourceData.SourceId);
                    }
                    else
                    {
                        InputManager.Instance.RaiseTouchpadReleased(this, sourceData.SourceId);
                    }
                }
                sourceData.Touchpad.CurrentReading = newTouchpad;
            }
            else
            {
                sourceData.Touchpad.CurrentReading = default(TouchpadData);
            }

            sourceData.Select.IsSupported = true; // All input mechanisms support "select".
            sourceData.Select.IsAvailable = sourceData.Select.IsSupported;
            AxisButton1D newSelect = AxisButton1D.GetSelect(interactionSourceState);
            if (sourceData.Select.CurrentReading.PressedAmount != newSelect.PressedAmount)
            {
                InputManager.Instance.RaiseSelectPressedValueChanged(this, sourceData.SourceId, newSelect.PressedAmount);
            }
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
            InputManager.Instance.RaiseInputClicked(this, obj.source.id, InteractionSourcePressType.Select, obj.tapCount);
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

        #endregion
    }
}