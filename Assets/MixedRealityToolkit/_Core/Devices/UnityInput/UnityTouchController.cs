// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Core.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Devices.UnityInput
{
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


        public override MixedRealityInteractionMapping[] DefaultInteractions { get; } =
        {
            new MixedRealityInteractionMapping(0, "Touch Pointer Position", AxisType.DualAxis, DeviceInputType.PointerPosition, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(1, "Touch Press", AxisType.Digital, DeviceInputType.PointerClick, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(2, "Touch Hold", AxisType.Digital, DeviceInputType.TouchpadPress, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(3, "Touch Manipulation", AxisType.DualAxis, DeviceInputType.Touchpad, MixedRealityInputAction.None)
        };

        private bool isTouched;

        private bool isHolding;

        private bool isManipulating;

        public override void SetupDefaultInteractions(Handedness controllerHandedness)
        {
            AssignControllerMappings(DefaultInteractions);
        }

        public void StartTouch()
        {
            InputSystem?.RaisePointerDown(InputSource.Pointers[0], Interactions[1].MixedRealityInputAction);
            InputSystem?.RaiseHoldStarted(InputSource, Interactions[2].MixedRealityInputAction);
            isTouched = true;
            isHolding = true;
        }

        public void Update()
        {
            if (!isTouched) { return; }

            Lifetime += Time.deltaTime;

            if (TouchData.phase == TouchPhase.Moved)
            {
                InputSystem?.RaisePositionInputChanged(InputSource, Interactions[0].MixedRealityInputAction, TouchData.position);

                if (isHolding)
                {
                    InputSystem?.RaiseHoldCanceled(InputSource, Interactions[2].MixedRealityInputAction);
                    isHolding = false;
                }

                if (!isManipulating)
                {
                    InputSystem?.RaiseManipulationStarted(InputSource, Interactions[3].MixedRealityInputAction);
                    isManipulating = true;
                }
                else
                {
                    InputSystem?.RaiseManipulationUpdated(InputSource, Interactions[3].MixedRealityInputAction, TouchData.deltaPosition);
                }
            }
        }

        public void EndTouch()
        {
            if (TouchData.phase == TouchPhase.Ended)
            {
                if (Lifetime < K_CONTACT_EPSILON)
                {
                    if (isHolding)
                    {
                        InputSystem?.RaiseHoldCanceled(InputSource, Interactions[2].MixedRealityInputAction);
                    }

                    if (isManipulating)
                    {
                        InputSystem?.RaiseManipulationCanceled(InputSource, Interactions[3].MixedRealityInputAction);
                    }
                }
                else if (Lifetime < MaxTapContactTime)
                {
                    if (isHolding)
                    {
                        InputSystem?.RaiseHoldCanceled(InputSource, Interactions[2].MixedRealityInputAction);
                    }

                    if (isManipulating)
                    {
                        InputSystem?.RaiseManipulationCanceled(InputSource, Interactions[3].MixedRealityInputAction);
                    }

                    InputSystem?.RaisePointerClicked(InputSource.Pointers[0], Interactions[1].MixedRealityInputAction, TouchData.tapCount);
                }
                else if (isHolding)
                {
                    InputSystem?.RaiseHoldCompleted(InputSource, Interactions[2].MixedRealityInputAction);
                }
                else if (isManipulating)
                {
                    InputSystem?.RaiseManipulationCompleted(InputSource, Interactions[3].MixedRealityInputAction, TouchData.deltaPosition);
                }
            }
            else if (isHolding)
            {
                InputSystem?.RaiseHoldCanceled(InputSource, Interactions[2].MixedRealityInputAction);
            }
            else if (isManipulating)
            {
                InputSystem?.RaiseManipulationCanceled(InputSource, Interactions[3].MixedRealityInputAction);
            }

            InputSystem?.RaisePointerUp(InputSource.Pointers[0], Interactions[1].MixedRealityInputAction);

            Lifetime = 0.0f;
            isHolding = false;
            isManipulating = false;
            isTouched = false;
        }
    }
}