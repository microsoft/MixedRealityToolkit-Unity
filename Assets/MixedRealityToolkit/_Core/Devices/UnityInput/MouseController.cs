// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Core.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Devices.UnityInput
{
    /// <summary>
    /// Manages the mouse using unity input system.
    /// </summary>
    public class MouseController : BaseController
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="trackingState"></param>
        /// <param name="controllerHandedness"></param>
        /// <param name="inputSource"></param>
        /// <param name="interactions"></param>
        public MouseController(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
            : base(trackingState, controllerHandedness, inputSource, interactions)
        {
        }

        private Vector2 screenPosition = Vector2.zero;

        /// <summary>
        /// Current screen position.
        /// </summary>
        public Vector2 ScreenPosition => screenPosition;

        public override MixedRealityInteractionMapping[] DefaultInteractions { get; } =
        {
            new MixedRealityInteractionMapping(0, "Pointer Screen Position", AxisType.DualAxis, DeviceInputType.PointerPosition, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(1, "Spatial Mouse Position", AxisType.SixDof, DeviceInputType.SpatialPointer, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(2, "Left Mouse Button", AxisType.Digital, DeviceInputType.ButtonPress, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(3, "Right Mouse Button", AxisType.Digital, DeviceInputType.ButtonPress, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(4, "Middle Mouse Button", AxisType.Digital, DeviceInputType.ButtonPress, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(5, "Mouse Drag Manipulation", AxisType.DualAxis, DeviceInputType.Touchpad, MixedRealityInputAction.None)
        };

        /// <inheritdoc />
        public override void SetupDefaultInteractions(Handedness controllerHandedness)
        {
            AssignControllerMappings(DefaultInteractions);
        }

        private Vector3 pointerPosition;

        /// <summary>
        /// Update controller.
        /// </summary>
        public void Update()
        {
            screenPosition.x = Input.mousePosition.x;
            screenPosition.y = Input.mousePosition.y;
            InputSystem?.RaiseSourcePositionChanged(InputSource, this, screenPosition);
            InputSource.Pointers[0].TryGetPointerPosition(out pointerPosition);
            InputSystem?.RaiseSourcePositionChanged(InputSource, this, pointerPosition);
        }
    }
}
