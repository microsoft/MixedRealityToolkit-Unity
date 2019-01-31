// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Attributes;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Core.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Services;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Providers.UnityInput
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

        private const float K_CONTACT_EPSILON = 30.0f;

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
            new MixedRealityInteractionMapping(0, "Touch Pointer Delta", AxisType.DualAxis, DeviceInputType.PointerPosition, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(1, "Touch Pointer Position", AxisType.SixDof, DeviceInputType.SpatialPointer, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(2, "Touch Press", AxisType.Digital, DeviceInputType.PointerClick, MixedRealityInputAction.None),
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
            if (MixedRealityToolkit.Instance.ActiveProfile.IsInputSystemEnabled &&
                MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.GesturesProfile != null)
            {
                for (int i = 0; i < MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.GesturesProfile.Gestures.Length; i++)
                {
                    var gesture = MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.GesturesProfile.Gestures[i];

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

        /// <summary>
        /// Start the touch.
        /// </summary>
        public void StartTouch()
        {
            MixedRealityToolkit.InputSystem?.RaisePointerDown(InputSource.Pointers[0], Interactions[2].MixedRealityInputAction);
            isTouched = true;
            MixedRealityToolkit.InputSystem?.RaiseGestureStarted(this, holdingAction);
            isHolding = true;
        }

        /// <summary>
        /// Update the touch data.
        /// </summary>
        public void Update()
        {
            if (!isTouched) { return; }

            Lifetime += Time.deltaTime;

            if (TouchData.phase == TouchPhase.Moved)
            {
                Interactions[0].Vector2Data = TouchData.deltaPosition;

                if (Interactions[0].Changed)
                {
                    MixedRealityToolkit.InputSystem?.RaisePositionInputChanged(InputSource, Interactions[0].MixedRealityInputAction, TouchData.deltaPosition);
                }

                lastPose.Position = InputSource.Pointers[0].BaseCursor.Position;
                lastPose.Rotation = InputSource.Pointers[0].BaseCursor.Rotation;
                MixedRealityToolkit.InputSystem?.RaiseSourcePoseChanged(InputSource, this, lastPose);

                Interactions[1].PoseData = lastPose;

                if (Interactions[1].Changed)
                {
                    MixedRealityToolkit.InputSystem?.RaisePoseInputChanged(InputSource, Interactions[1].MixedRealityInputAction, lastPose);
                }

                if (!isManipulating)
                {
                    if (Mathf.Abs(TouchData.deltaPosition.x) > ManipulationThreshold ||
                        Mathf.Abs(TouchData.deltaPosition.y) > ManipulationThreshold)
                    {
                        MixedRealityToolkit.InputSystem?.RaiseGestureCanceled(this, holdingAction);
                        isHolding = false;

                        MixedRealityToolkit.InputSystem?.RaiseGestureStarted(this, manipulationAction);
                        isManipulating = true;
                    }
                }
                else
                {
                    MixedRealityToolkit.InputSystem?.RaiseGestureUpdated(this, manipulationAction, TouchData.deltaPosition);
                }
            }
        }

        /// <summary>
        /// End the touch.
        /// </summary>
        public void EndTouch()
        {
            if (TouchData.phase == TouchPhase.Ended)
            {
                if (Lifetime < K_CONTACT_EPSILON)
                {
                    if (isHolding)
                    {
                        MixedRealityToolkit.InputSystem?.RaiseGestureCanceled(this, holdingAction);
                        isHolding = false;
                    }

                    if (isManipulating)
                    {
                        MixedRealityToolkit.InputSystem?.RaiseGestureCanceled(this, manipulationAction);
                        isManipulating = false;
                    }
                }
                else if (Lifetime < MaxTapContactTime)
                {
                    if (isHolding)
                    {
                        MixedRealityToolkit.InputSystem?.RaiseGestureCanceled(this, holdingAction);
                        isHolding = false;
                    }

                    if (isManipulating)
                    {
                        MixedRealityToolkit.InputSystem?.RaiseGestureCanceled(this, manipulationAction);
                        isManipulating = false;
                    }

                    MixedRealityToolkit.InputSystem?.RaisePointerClicked(InputSource.Pointers[0], Interactions[2].MixedRealityInputAction, TouchData.tapCount);
                }

                if (isHolding)
                {
                    MixedRealityToolkit.InputSystem?.RaiseGestureCompleted(this, holdingAction);
                    isHolding = false;
                }

                if (isManipulating)
                {
                    MixedRealityToolkit.InputSystem?.RaiseGestureCompleted(this, manipulationAction, TouchData.deltaPosition);
                    isManipulating = false;
                }
            }

            if (isHolding)
            {
                MixedRealityToolkit.InputSystem?.RaiseGestureCompleted(this, holdingAction);
                isHolding = false;
            }

            Debug.Assert(!isHolding);

            if (isManipulating)
            {
                MixedRealityToolkit.InputSystem?.RaiseGestureCompleted(this, manipulationAction, TouchData.deltaPosition);
                isManipulating = false;
            }

            Debug.Assert(!isManipulating);

            MixedRealityToolkit.InputSystem?.RaisePointerUp(InputSource.Pointers[0], Interactions[2].MixedRealityInputAction);

            Lifetime = 0.0f;
            isTouched = false;
            Interactions[1].PoseData = MixedRealityPose.ZeroIdentity;
            Interactions[0].Vector2Data = Vector2.zero;
        }
    }
}