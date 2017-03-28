// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA.Input;

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
        public enum RecognizerStartBehavior { AutoStart, ManualStart };

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
            public double X;
            public double Y;

            public static AxisButton2D GetThumbstick(InteractionSourceState interactionSource)
            {
                return new AxisButton2D
                {
                    Pressed = interactionSource.controllerProperties.thumbstickPressed,
                    X = interactionSource.controllerProperties.thumbstickX,
                    Y = interactionSource.controllerProperties.thumbstickY,
                };
            }

            public static AxisButton2D GetTouchpad(InteractionSourceState interactionSource)
            {
                return new AxisButton2D
                {
                    Pressed = interactionSource.controllerProperties.touchpadPressed,
                    X = interactionSource.controllerProperties.touchpadX,
                    Y = interactionSource.controllerProperties.touchpadY,
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
                    Touched = interactionSource.controllerProperties.touchpadTouched,
                };
            }
        }

        private struct AxisButton1D
        {
            public bool Pressed;
            public double PressedValue;

            public static AxisButton1D GetTrigger(InteractionSourceState interactionSource)
            {
                return new AxisButton1D
                {
                    Pressed = interactionSource.selectPressed,
                    PressedValue = interactionSource.selectPressedValue,
                };
            }
        }

        /// <summary>
        /// Data for an interaction source.
        /// </summary>
        private class SourceData
        {
            public SourceData(IInputSource inputSource, InteractionSource interactionSource)
            {
                SourceId = interactionSource.id;
                SourceKind = interactionSource.kind;
            }

            public readonly uint SourceId;
            public readonly InteractionSourceKind SourceKind;
            public SourceCapability<Vector3> Position;
            public SourceCapability<Quaternion> Orientation;
            public SourceCapability<Ray> PointingRay;
            public SourceCapability<AxisButton2D> Thumbstick;
            public SourceCapability<TouchpadData> Touchpad;
            public SourceCapability<AxisButton1D> Trigger;
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
            gestureRecognizer.TappedEvent += OnTappedEvent;

            gestureRecognizer.HoldStartedEvent += OnHoldStartedEvent;
            gestureRecognizer.HoldCompletedEvent += OnHoldCompletedEvent;
            gestureRecognizer.HoldCanceledEvent += OnHoldCanceledEvent;

            gestureRecognizer.ManipulationStartedEvent += OnManipulationStartedEvent;
            gestureRecognizer.ManipulationUpdatedEvent += OnManipulationUpdatedEvent;
            gestureRecognizer.ManipulationCompletedEvent += OnManipulationCompletedEvent;
            gestureRecognizer.ManipulationCanceledEvent += OnManipulationCanceledEvent;

            gestureRecognizer.SetRecognizableGestures(GestureSettings.Tap |
                                                      GestureSettings.ManipulationTranslate |
                                                      GestureSettings.Hold);

            // We need a separate gesture recognizer for navigation, since it isn't compatible with manipulation
            navigationGestureRecognizer = new GestureRecognizer();

            navigationGestureRecognizer.NavigationStartedEvent += OnNavigationStartedEvent;
            navigationGestureRecognizer.NavigationUpdatedEvent += OnNavigationUpdatedEvent;
            navigationGestureRecognizer.NavigationCompletedEvent += OnNavigationCompletedEvent;
            navigationGestureRecognizer.NavigationCanceledEvent += OnNavigationCanceledEvent;

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
                gestureRecognizer.StartCapturingGestures();
                navigationGestureRecognizer.StartCapturingGestures();
            }
        }

        protected virtual void OnDestroy()
        {
            if (gestureRecognizer != null)
            {
                gestureRecognizer.TappedEvent -= OnTappedEvent;

                gestureRecognizer.HoldStartedEvent -= OnHoldStartedEvent;
                gestureRecognizer.HoldCompletedEvent -= OnHoldCompletedEvent;
                gestureRecognizer.HoldCanceledEvent -= OnHoldCanceledEvent;

                gestureRecognizer.ManipulationStartedEvent -= OnManipulationStartedEvent;
                gestureRecognizer.ManipulationUpdatedEvent -= OnManipulationUpdatedEvent;
                gestureRecognizer.ManipulationCompletedEvent -= OnManipulationCompletedEvent;
                gestureRecognizer.ManipulationCanceledEvent -= OnManipulationCanceledEvent;

                gestureRecognizer.Dispose();
            }

            if (navigationGestureRecognizer != null)
            {
                navigationGestureRecognizer.NavigationStartedEvent -= OnNavigationStartedEvent;
                navigationGestureRecognizer.NavigationUpdatedEvent -= OnNavigationUpdatedEvent;
                navigationGestureRecognizer.NavigationCompletedEvent -= OnNavigationCompletedEvent;
                navigationGestureRecognizer.NavigationCanceledEvent -= OnNavigationCanceledEvent;

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

            InteractionManager.SourceUpdated += InteractionManager_SourceUpdated;

            InteractionManager.SourceReleased += InteractionManager_SourceReleased;
            InteractionManager.SourcePressed += InteractionManager_SourcePressed;

            InteractionManager.SourceLost += InteractionManager_SourceLost;
            InteractionManager.SourceDetected += InteractionManager_SourceDetected;

            // TODO: robertes: Should we use InteractionManager.GetCurrentReading() to get all sources currently available and synthesize a SourceDetected?
        }

        protected override void OnDisableAfterStart()
        {
            StopGestureRecognizer();

            InteractionManager.SourceUpdated -= InteractionManager_SourceUpdated;

            InteractionManager.SourceReleased -= InteractionManager_SourceReleased;
            InteractionManager.SourcePressed -= InteractionManager_SourcePressed;

            InteractionManager.SourceLost -= InteractionManager_SourceLost;
            InteractionManager.SourceDetected -= InteractionManager_SourceDetected;

            // TODO: robertes: Should we synthesize SourceLost for all outstanding sources and then clear our list?

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
                retVal |= GetSupportFlag(sourceData.Position, SupportedInputInfo.Position);
                retVal |= GetSupportFlag(sourceData.Orientation, SupportedInputInfo.Orientation);
                retVal |= GetSupportFlag(sourceData.PointingRay, SupportedInputInfo.PointingRay);
                retVal |= GetSupportFlag(sourceData.Thumbstick, SupportedInputInfo.Thumbstick);
                retVal |= GetSupportFlag(sourceData.Touchpad, SupportedInputInfo.Touchpad);
                retVal |= GetSupportFlag(sourceData.Trigger, SupportedInputInfo.Trigger);
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

        public override bool TryGetPosition(uint sourceId, out Vector3 position)
        {
            SourceData sourceData;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData) && TryGetReading(sourceData.Position, out position))
            {
                return true;
            }
            else
            {
                position = default(Vector3);
                return false;
            }
        }

        public override bool TryGetOrientation(uint sourceId, out Quaternion orientation)
        {
            SourceData sourceData;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData) && TryGetReading(sourceData.Orientation, out orientation))
            {
                return true;
            }
            else
            {
                orientation = default(Quaternion);
                return false;
            }
        }

        public override bool TryGetPointingRay(uint sourceId, out Ray pointingRay)
        {
            SourceData sourceData;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData) && TryGetReading(sourceData.PointingRay, out pointingRay))
            {
                return true;
            }
            else
            {
                pointingRay = default(Ray);
                return false;
            }
        }

        public override bool TryGetThumbstick(uint sourceId, out bool thumbstickPressed, out double thumbstickX, out double thumbstickY)
        {
            SourceData sourceData;
            AxisButton2D thumbstick;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData) && TryGetReading(sourceData.Thumbstick, out thumbstick))
            {
                thumbstickPressed = thumbstick.Pressed;
                thumbstickX = thumbstick.X;
                thumbstickY = thumbstick.Y;
                return true;
            }
            else
            {
                thumbstickPressed = false;
                thumbstickX = 0;
                thumbstickY = 0;
                return false;
            }
        }

        public override bool TryGetTouchpad(uint sourceId, out bool touchpadPressed, out bool touchpadTouched, out double touchpadX, out double touchpadY)
        {
            SourceData sourceData;
            TouchpadData touchpad;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData) && TryGetReading(sourceData.Touchpad, out touchpad))
            {
                touchpadPressed = touchpad.AxisButton.Pressed;
                touchpadTouched = touchpad.Touched;
                touchpadX = touchpad.AxisButton.X;
                touchpadY = touchpad.AxisButton.Y;
                return true;
            }
            else
            {
                touchpadPressed = false;
                touchpadTouched = false;
                touchpadX = 0;
                touchpadY = 0;
                return false;
            }
        }

        public override bool TryGetTrigger(uint sourceId, out bool triggerPressed, out double triggerPressedValue)
        {
            SourceData sourceData;
            AxisButton1D trigger;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData) && TryGetReading(sourceData.Trigger, out trigger))
            {
                triggerPressed = trigger.Pressed;
                triggerPressedValue = trigger.PressedValue;
                return true;
            }
            else
            {
                triggerPressed = false;
                triggerPressedValue = 0;
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

        private void InteractionManager_SourceUpdated(InteractionManager.SourceEventArgs args)
        {
            UpdateSourceState(args.state, GetOrAddSourceData(args.state.source));

            // TODO: robertes: raise update events?  Perhaps part of UpdateSourceState and have UpdateSourceState measure deltas and raise events iff changed... rename it to UpdateSourceStateAndRaiseUpdateEvents? ... and don't forget the TouchpadTouched events.
            // TODO: robertes: We should call UpdateSourceState in other handlers, too, right?  All handlers?
        }

        private void InteractionManager_SourceReleased(InteractionManager.SourceEventArgs args)
        {
            InputManager.Instance.RaiseSourceUp(this, args.state.source.id, args.kind);
        }

        private void InteractionManager_SourcePressed(InteractionManager.SourceEventArgs args)
        {
            InputManager.Instance.RaiseSourceDown(this, args.state.source.id, args.kind);
        }

        private void InteractionManager_SourceLost(InteractionManager.SourceEventArgs args)
        {
            // NOTE: We don't care whether the source ID previously existed or not, so we blindly call Remove:
            sourceIdToData.Remove(args.state.source.id);

            InputManager.Instance.RaiseSourceLost(this, args.state.source.id);
        }

        private void InteractionManager_SourceDetected(InteractionManager.SourceEventArgs args)
        {
            // NOTE: We don't need to use the data here. We just need to make sure it's added if it's not available yet:
            GetOrAddSourceData(args.state.source);

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
                sourceData = new SourceData(this, interactionSource);
                sourceIdToData.Add(sourceData.SourceId, sourceData);

                // TODO: robertes: whenever we end up adding, should we first synthesize a SourceDetected?  Or
                //       perhaps if we keep strict track of all sources, we should never need to just-in-time add anymore.
            }

            return sourceData;
        }

        /// <summary>
        /// Updates the source information.
        /// </summary>
        /// <param name="interactionSource">Interaction source to use to update the source information.</param>
        /// <param name="sourceData">SourceData structure to update.</param>
        private void UpdateSourceState(InteractionSourceState interactionSource, SourceData sourceData)
        {
            Debug.Assert(interactionSource.source.id == sourceData.SourceId, "An UpdateSourceState call happened with mismatched source ID.");
            Debug.Assert(interactionSource.source.kind == sourceData.SourceKind, "An UpdateSourceState call happened with mismatched source kind.");

            InteractionSourceLocation locationData = interactionSource.properties.location;

            Vector3 newPosition;
            sourceData.Position.IsAvailable = locationData.TryGetPosition(out newPosition);
            // Using a heuristic for IsSupported, since the APIs don't yet support querying this capability directly.
            sourceData.Position.IsSupported |= sourceData.Position.IsAvailable;
            if (sourceData.Position.IsAvailable)
            {
                if (!(sourceData.Position.CurrentReading.Equals(newPosition)))
                {
                    InputManager.Instance.RaiseSourcePositionChanged(this, sourceData.SourceId, newPosition);
                }
            }
            sourceData.Position.CurrentReading = newPosition;

            Quaternion newOrientation;
            sourceData.Orientation.IsAvailable = locationData.TryGetOrientation(out newOrientation);
            // Using a heuristic for IsSupported, since the APIs don't yet support querying this capability directly.
            sourceData.Orientation.IsSupported |= sourceData.Orientation.IsAvailable;
            if (sourceData.Orientation.IsAvailable)
            {
                if(!(sourceData.Orientation.CurrentReading.Equals(newOrientation)))
                {
                    InputManager.Instance.RaiseSourceOrientationChanged(this, sourceData.SourceId, newOrientation);
                }
            }
            sourceData.Orientation.CurrentReading = newOrientation;

            sourceData.PointingRay.IsSupported = interactionSource.source.supportsPointing;
            sourceData.PointingRay.IsAvailable = interactionSource.sourceRay.IsValid();
            sourceData.PointingRay.CurrentReading = interactionSource.sourceRay;

            InteractionController controller;
            bool gotController = interactionSource.source.TryGetController(out controller);

            sourceData.Thumbstick.IsSupported = (gotController && controller.hasThumbstick);
            sourceData.Thumbstick.IsAvailable = sourceData.Thumbstick.IsSupported;
            if (sourceData.Thumbstick.IsAvailable)
            {
                AxisButton2D newThumbstick = AxisButton2D.GetThumbstick(interactionSource);
                if  ((sourceData.Thumbstick.CurrentReading.X != newThumbstick.X) || (sourceData.Thumbstick.CurrentReading.Y != newThumbstick.Y))
                {
                    InputManager.Instance.RaiseInputXYChanged(this, sourceData.SourceId, InteractionPressKind.Thumbstick, newThumbstick.X, newThumbstick.Y);
                }
                sourceData.Thumbstick.CurrentReading = newThumbstick;
            }
            else
            {
                sourceData.Thumbstick.CurrentReading = default(AxisButton2D);
            }

            sourceData.Touchpad.IsSupported = (gotController && controller.hasTouchpad);
            sourceData.Touchpad.IsAvailable = sourceData.Touchpad.IsSupported;
            if (sourceData.Touchpad.IsAvailable)
            {
                TouchpadData newTouchpad = TouchpadData.GetTouchpad(interactionSource);
                if ((sourceData.Touchpad.CurrentReading.AxisButton.X != newTouchpad.AxisButton.X) || (sourceData.Touchpad.CurrentReading.AxisButton.Y != newTouchpad.AxisButton.Y))
                {
                    InputManager.Instance.RaiseInputXYChanged(this, sourceData.SourceId, InteractionPressKind.Touchpad, newTouchpad.AxisButton.X, newTouchpad.AxisButton.Y);
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

            sourceData.Trigger.IsSupported = true; // All input mechanisms support "select" which is considered the same as "trigger".
            sourceData.Trigger.IsAvailable = sourceData.Trigger.IsSupported;
            AxisButton1D newTrigger = AxisButton1D.GetTrigger(interactionSource);
            if (sourceData.Trigger.CurrentReading.PressedValue != newTrigger.PressedValue)
            {
                InputManager.Instance.RaiseTriggerPressedValueChanged(this, sourceData.SourceId, newTrigger.PressedValue);
            }
            sourceData.Trigger.CurrentReading = newTrigger;

            sourceData.Grasp.IsSupported = interactionSource.source.supportsGrasp;
            sourceData.Grasp.IsAvailable = sourceData.Grasp.IsSupported;
            sourceData.Grasp.CurrentReading = (sourceData.Grasp.IsAvailable ? interactionSource.grasped : false);

            sourceData.Menu.IsSupported = interactionSource.source.supportsMenu;
            sourceData.Menu.IsAvailable = sourceData.Menu.IsSupported;
            sourceData.Menu.CurrentReading = (sourceData.Menu.IsAvailable ? interactionSource.menuPressed : false);
        }

        #endregion

        #region Raise GestureRecognizer Events

        // TODO: robertes: Should these also cause source state data to be stored/updated? What about SourceDetected synthesized events?

        protected void OnTappedEvent(TappedEventArgs obj)
        {
            InputManager.Instance.RaiseInputClicked(this, (uint)obj.sourceId, InteractionPressKind.Select, obj.tapCount);
        }

        protected void OnHoldStartedEvent(HoldStartedEventArgs obj)
        {
            InputManager.Instance.RaiseHoldStarted(this, (uint)obj.sourceId);
        }

        protected void OnHoldCanceledEvent(HoldCanceledEventArgs obj)
        {
            InputManager.Instance.RaiseHoldCanceled(this, (uint)obj.sourceId);
        }

        protected void OnHoldCompletedEvent(HoldCompletedEventArgs obj)
        {
            InputManager.Instance.RaiseHoldCompleted(this, (uint)obj.sourceId);
        }

        protected void OnManipulationStartedEvent(ManipulationStartedEventArgs obj)
        {
            InputManager.Instance.RaiseManipulationStarted(this, (uint)obj.sourceId, obj.cumulativeDelta);
        }

        protected void OnManipulationUpdatedEvent(ManipulationUpdatedEventArgs obj)
        {
            InputManager.Instance.RaiseManipulationUpdated(this, (uint)obj.sourceId, obj.cumulativeDelta);
        }

        protected void OnManipulationCompletedEvent(ManipulationCompletedEventArgs obj)
        {
            InputManager.Instance.RaiseManipulationCompleted(this, (uint)obj.sourceId, obj.cumulativeDelta);
        }

        protected void OnManipulationCanceledEvent(ManipulationCanceledEventArgs obj)
        {
            InputManager.Instance.RaiseManipulationCanceled(this, (uint)obj.sourceId, obj.cumulativeDelta);
        }

        protected void OnNavigationStartedEvent(NavigationStartedEventArgs obj)
        {
            InputManager.Instance.RaiseNavigationStarted(this, (uint)obj.sourceId, obj.normalizedOffset);
        }

        protected void OnNavigationUpdatedEvent(NavigationUpdatedEventArgs obj)
        {
            InputManager.Instance.RaiseNavigationUpdated(this, (uint)obj.sourceId, obj.normalizedOffset);
        }

        protected void OnNavigationCompletedEvent(NavigationCompletedEventArgs obj)
        {
            InputManager.Instance.RaiseNavigationCompleted(this, (uint)obj.sourceId, obj.normalizedOffset);
        }

        protected void OnNavigationCanceledEvent(NavigationCanceledEventArgs obj)
        {
            InputManager.Instance.RaiseNavigationCanceled(this, (uint)obj.sourceId, obj.normalizedOffset);
        }

        #endregion
    }
}
