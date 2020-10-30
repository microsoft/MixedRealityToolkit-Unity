// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;

namespace Microsoft.MixedReality.Toolkit.Input
{
    public class HPMotionControllerDefinition
    {
        public HPMotionControllerDefinition(IMixedRealityInputSource source, Handedness handedness)
        {
            inputSource = source;
            this.handedness = handedness;
        }

        protected readonly IMixedRealityInputSource inputSource;
        protected readonly Handedness handedness;

        /// <inheritdoc />
        public MixedRealityInteractionMapping[] DefaultLeftHandedInteractions => new[]
        {
            new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer),
            new MixedRealityInteractionMapping(1, "Spatial Grip", AxisType.SixDof, DeviceInputType.SpatialGrip),
            new MixedRealityInteractionMapping(2, "Grip Position", AxisType.SingleAxis, DeviceInputType.Grip),
            new MixedRealityInteractionMapping(3, "Grip Touch", AxisType.Digital, DeviceInputType.GripTouch),
            new MixedRealityInteractionMapping(4, "Grip Press", AxisType.SingleAxis, DeviceInputType.GripPress),
            new MixedRealityInteractionMapping(5, "Trigger Position", AxisType.SingleAxis, DeviceInputType.Trigger),
            new MixedRealityInteractionMapping(6, "Trigger Touch", AxisType.Digital, DeviceInputType.TriggerTouch),
            new MixedRealityInteractionMapping(7, "Trigger Press (Select)", AxisType.Digital, DeviceInputType.Select),
            new MixedRealityInteractionMapping(8, "Button.X Press", AxisType.Digital, DeviceInputType.PrimaryButtonPress),
            new MixedRealityInteractionMapping(9, "Button.Y Press", AxisType.Digital, DeviceInputType.SecondaryButtonPress),
            new MixedRealityInteractionMapping(10, "Menu Press", AxisType.Digital, DeviceInputType.Menu),
            new MixedRealityInteractionMapping(11, "Thumbstick Position", AxisType.DualAxis, DeviceInputType.ThumbStick),
            new MixedRealityInteractionMapping(12, "Thumbstick Press", AxisType.Digital, DeviceInputType.ThumbStickPress)
        };

        /// <inheritdoc />
        public MixedRealityInteractionMapping[] DefaultRightHandedInteractions => new[]
        {
            new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer),
            new MixedRealityInteractionMapping(1, "Spatial Grip", AxisType.SixDof, DeviceInputType.SpatialGrip),
            new MixedRealityInteractionMapping(2, "Grip Position", AxisType.SingleAxis, DeviceInputType.Grip),
            new MixedRealityInteractionMapping(3, "Grip Touch", AxisType.Digital, DeviceInputType.GripTouch),
            new MixedRealityInteractionMapping(4, "Grip Press", AxisType.SingleAxis, DeviceInputType.GripPress),
            new MixedRealityInteractionMapping(5, "Trigger Position", AxisType.SingleAxis, DeviceInputType.Trigger),
            new MixedRealityInteractionMapping(6, "Trigger Touch", AxisType.Digital, DeviceInputType.TriggerTouch),
            new MixedRealityInteractionMapping(7, "Trigger Press (Select)", AxisType.Digital, DeviceInputType.Select),
            new MixedRealityInteractionMapping(8, "Button.A Press", AxisType.Digital, DeviceInputType.PrimaryButtonPress),
            new MixedRealityInteractionMapping(9, "Button.B Press", AxisType.Digital, DeviceInputType.SecondaryButtonPress),
            new MixedRealityInteractionMapping(10, "Menu Press", AxisType.Digital, DeviceInputType.Menu),
            new MixedRealityInteractionMapping(11, "Thumbstick Position", AxisType.DualAxis, DeviceInputType.ThumbStick),
            new MixedRealityInteractionMapping(12, "Thumbstick Press", AxisType.Digital, DeviceInputType.ThumbStickPress)
        };
    }
}
