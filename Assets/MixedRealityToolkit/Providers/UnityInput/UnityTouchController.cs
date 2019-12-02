// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input.UnityInput
{
    [MixedRealityController(
        SupportedControllerType.TouchScreen,
        new[] { Handedness.Any })]
    public class UnityTouchController : BaseController
    {
        public UnityTouchController(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
                : base(trackingState, controllerHandedness, inputSource, interactions)
        {
        }

        /// <summary>
        /// Time in seconds to determine if the contact registers as a tap or a hold
        /// </summary>
        public float MaxTapContactTime { get; set; } = 0.5f;

        /// <summary>
        /// The threshold a finger must move before starting a manipulation gesture.
        /// </summary>
        public float ManipulationThreshold { get; set; } = 5f;

        /// <summary>
        /// Current Touch Data for the Controller.
        /// </summary>
        public Touch TouchData { get; internal set; }

        /// <summary>
        /// Current Screen point ray for the Touch.
        /// </summary>
        public Ray ScreenPointRay { get; internal set; }

        /// <summary>
        /// The current lifetime of the Touch.
        /// </summary>
        public float Lifetime { get; private set; } = 0.0f;

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultInteractions { get; } =
        {
            new MixedRealityInteractionMapping(0, "Touch Pointer Delta", AxisType.DualAxis, DeviceInputType.PointerPosition),
            new MixedRealityInteractionMapping(1, "Touch Pointer Position", AxisType.SixDof, DeviceInputType.SpatialPointer),
            new MixedRealityInteractionMapping(2, "Touch Press", AxisType.Digital, DeviceInputType.PointerClick),
        };

        private bool isTouched;
        private MixedRealityInputAction holdingAction;
        private bool isHolding;
        private MixedRealityInputAction manipulationAction;
        private bool isManipulating;
        private MixedRealityPose lastPose = MixedRealityPose.ZeroIdentity;

        /// <inheritdoc />
        public override void SetupDefaultInteractions(Handedness controllerHandedness)
        {
            AssignControllerMappings(DefaultInteractions);
            if (CoreServices.InputSystem?.InputSystemProfile.GesturesProfile != null)
            {
                var gestures = CoreServices.InputSystem.InputSystemProfile.GesturesProfile.Gestures;
                for (int i = 0; i < gestures.Length; i++)
                {
                    var gesture = gestures[i];

                    switch (gesture.GestureType)
                    {
                        case GestureInputType.Hold:
                            holdingAction = gesture.Action;
                            break;
                        case GestureInputType.Manipulation:
                            manipulationAction = gesture.Action;
                            break;
                    }
                }
            }
        }

        private bool isNewController = false;

        /// <summary>
        /// Start the touch.
        /// </summary>
        public void StartTouch()
        {
            // Indicate that this is a new controller.
            isNewController = true;
        }

        /// <summary>
        /// Update the touch data.
        /// </summary>
        public void Update()
        {
            var inputSystem = CoreServices.InputSystem;
            if (inputSystem == null)
            {
                return;
            }

            if (isNewController)
            {
                isNewController = false;

                inputSystem.RaiseOnInputDown(InputSource, Handedness.None, Interactions[2].MixedRealityInputAction);
                inputSystem.RaisePointerDown(InputSource.Pointers[0], Interactions[2].MixedRealityInputAction);
                isTouched = true;
                inputSystem.RaiseGestureStarted(this, holdingAction);
                isHolding = true;
            }

            if (!isTouched) { return; }

            Lifetime += Time.deltaTime;

            if (TouchData.phase == TouchPhase.Moved)
            {
                Interactions[0].Vector2Data = TouchData.deltaPosition;

                if (Interactions[0].Changed)
                {
                    inputSystem.RaisePositionInputChanged(InputSource, ControllerHandedness, Interactions[0].MixedRealityInputAction, TouchData.deltaPosition);
                }

                lastPose.Position = InputSource.Pointers[0].Position;
                lastPose.Rotation = InputSource.Pointers[0].Rotation;
                inputSystem.RaiseSourcePoseChanged(InputSource, this, lastPose);

                Interactions[1].PoseData = lastPose;

                if (Interactions[1].Changed)
                {
                    inputSystem.RaisePoseInputChanged(InputSource, ControllerHandedness, Interactions[1].MixedRealityInputAction, lastPose);
                }

                if (!isManipulating)
                {
                    if (Mathf.Abs(TouchData.deltaPosition.x) > ManipulationThreshold ||
                        Mathf.Abs(TouchData.deltaPosition.y) > ManipulationThreshold)
                    {
                        inputSystem?.RaiseGestureCanceled(this, holdingAction);
                        isHolding = false;

                        inputSystem?.RaiseGestureStarted(this, manipulationAction);
                        isManipulating = true;
                    }
                }
                else
                {
                    inputSystem.RaiseGestureUpdated(this, manipulationAction, TouchData.deltaPosition);
                }

                // Send dragged event, to inform manipulation handlers.
                inputSystem.RaisePointerDragged(InputSource.Pointers[0], Interactions[1].MixedRealityInputAction);
            }
        }

        /// <summary>
        /// End the touch.
        /// </summary>
        public void EndTouch()
        {
            var inputSystem = CoreServices.InputSystem;

            if (inputSystem == null)
            {
                return;
            }

            if (TouchData.phase == TouchPhase.Ended)
            {
                if (Lifetime < MaxTapContactTime)
                {
                    if (isHolding)
                    {
                        inputSystem.RaiseGestureCanceled(this, holdingAction);
                        isHolding = false;
                    }

                    if (isManipulating)
                    {
                        inputSystem.RaiseGestureCanceled(this, manipulationAction);
                        isManipulating = false;
                    }

                    inputSystem.RaisePointerClicked(InputSource.Pointers[0], Interactions[2].MixedRealityInputAction, TouchData.tapCount);
                }

                if (isHolding)
                {
                    inputSystem.RaiseGestureCompleted(this, holdingAction);
                    isHolding = false;
                }

                if (isManipulating)
                {
                    inputSystem.RaiseGestureCompleted(this, manipulationAction, TouchData.deltaPosition);
                    isManipulating = false;
                }
            }

            if (isHolding)
            {
                inputSystem.RaiseGestureCompleted(this, holdingAction);
                isHolding = false;
            }

            Debug.Assert(!isHolding);

            if (isManipulating)
            {
                inputSystem.RaiseGestureCompleted(this, manipulationAction, TouchData.deltaPosition);
                isManipulating = false;
            }

            Debug.Assert(!isManipulating);

            inputSystem.RaiseOnInputUp(InputSource, Handedness.None, Interactions[2].MixedRealityInputAction);
            inputSystem.RaisePointerUp(InputSource.Pointers[0], Interactions[2].MixedRealityInputAction);

            Lifetime = 0.0f;
            isTouched = false;
            Interactions[1].PoseData = MixedRealityPose.ZeroIdentity;
            Interactions[0].Vector2Data = Vector2.zero;
        }
    }
}