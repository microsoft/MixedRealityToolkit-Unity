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

        public override MixedRealityInteractionMapping[] DefaultInteractions { get; } =
        {
            new MixedRealityInteractionMapping(0, "Mouse Delta Position", AxisType.DualAxis, DeviceInputType.PointerPosition, MixedRealityInputAction.None),
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
        private Vector2 mouseDelta;

        /// <summary>
        /// Update controller.
        /// </summary>
        public void Update()
        {
            // Don't ask me why it's mapped weird. Bc Unity...
            mouseDelta.x = Input.GetAxis("Mouse X");
            mouseDelta.y = -Input.GetAxis("Mouse Y");
            InputSystem?.RaiseSourcePositionChanged(InputSource, this, mouseDelta);
            InputSource.Pointers[0].TryGetPointerPosition(out pointerPosition);
            InputSystem?.RaiseSourcePositionChanged(InputSource, this, pointerPosition);
        }
    }
}
