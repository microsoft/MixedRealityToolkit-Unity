// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;

namespace Microsoft.MixedReality.Toolkit.Input
{
    public class HPMotionControllerDefinition : BaseInputSourceDefinition
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="handedness">The handedness that this definition instance represents.</param>
        public HPMotionControllerDefinition(Handedness handedness) : base(handedness)
        {
            if ((handedness != Handedness.Left) &&
                (handedness != Handedness.Right))
            {
                throw new System.ArgumentException($"Unsupported Handedness ({handedness}). The OculusTouchControllerDefinition supports Left and Right.");
            }
        }

        /// <inheritdoc />
        protected override MixedRealityInputActionMapping[] DefaultLeftHandedMappings => new[]
        {
            new MixedRealityInputActionMapping("Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer),
            new MixedRealityInputActionMapping("Spatial Grip", AxisType.SixDof, DeviceInputType.SpatialGrip),
            new MixedRealityInputActionMapping("Grip Position", AxisType.SingleAxis, DeviceInputType.Grip),
            new MixedRealityInputActionMapping("Grip Touch", AxisType.Digital, DeviceInputType.GripTouch),
            new MixedRealityInputActionMapping("Grip Press", AxisType.SingleAxis, DeviceInputType.GripPress),
            new MixedRealityInputActionMapping("Trigger Position", AxisType.SingleAxis, DeviceInputType.Trigger),
            new MixedRealityInputActionMapping("Trigger Touch", AxisType.Digital, DeviceInputType.TriggerTouch),
            new MixedRealityInputActionMapping("Trigger Press (Select)", AxisType.Digital, DeviceInputType.Select),
            new MixedRealityInputActionMapping("Button.X Press", AxisType.Digital, DeviceInputType.PrimaryButtonPress),
            new MixedRealityInputActionMapping("Button.Y Press", AxisType.Digital, DeviceInputType.SecondaryButtonPress),
            new MixedRealityInputActionMapping("Menu Press", AxisType.Digital, DeviceInputType.Menu),
            new MixedRealityInputActionMapping("Thumbstick Position", AxisType.DualAxis, DeviceInputType.ThumbStick),
            new MixedRealityInputActionMapping("Thumbstick Press", AxisType.Digital, DeviceInputType.ThumbStickPress),
        };

        /// <inheritdoc />
        protected override MixedRealityInputActionMapping[] DefaultRightHandedMappings => new[]
        {
            new MixedRealityInputActionMapping("Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer),
            new MixedRealityInputActionMapping("Spatial Grip", AxisType.SixDof, DeviceInputType.SpatialGrip),
            new MixedRealityInputActionMapping("Grip Position", AxisType.SingleAxis, DeviceInputType.Grip),
            new MixedRealityInputActionMapping("Grip Touch", AxisType.Digital, DeviceInputType.GripTouch),
            new MixedRealityInputActionMapping("Grip Press", AxisType.SingleAxis, DeviceInputType.GripPress),
            new MixedRealityInputActionMapping("Trigger Position", AxisType.SingleAxis, DeviceInputType.Trigger),
            new MixedRealityInputActionMapping("Trigger Touch", AxisType.Digital, DeviceInputType.TriggerTouch),
            new MixedRealityInputActionMapping("Trigger Press (Select)", AxisType.Digital, DeviceInputType.Select),
            new MixedRealityInputActionMapping("Button.A Press", AxisType.Digital, DeviceInputType.PrimaryButtonPress),
            new MixedRealityInputActionMapping("Button.B Press", AxisType.Digital, DeviceInputType.SecondaryButtonPress),
            new MixedRealityInputActionMapping("Menu Press", AxisType.Digital, DeviceInputType.Menu),
            new MixedRealityInputActionMapping("Thumbstick Position", AxisType.DualAxis, DeviceInputType.ThumbStick),
            new MixedRealityInputActionMapping("Thumbstick Press", AxisType.Digital, DeviceInputType.ThumbStickPress),
        };
    }
}
