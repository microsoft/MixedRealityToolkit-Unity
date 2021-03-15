// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// 
    /// </summary>
    public class OculusTouchControllerDefinition : BaseInputSourceDefinition
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="handedness">The handedness that this definition instance represents.</param>
        public OculusTouchControllerDefinition(Handedness handedness) : base(handedness)
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
            new MixedRealityInputActionMapping("Axis1D.PrimaryIndexTrigger", AxisType.SingleAxis, DeviceInputType.Trigger),
            new MixedRealityInputActionMapping("Axis1D.PrimaryIndexTrigger Touch", AxisType.Digital, DeviceInputType.TriggerTouch),
            new MixedRealityInputActionMapping("Axis1D.PrimaryIndexTrigger Near Touch", AxisType.Digital, DeviceInputType.TriggerNearTouch),
            new MixedRealityInputActionMapping("Axis1D.PrimaryIndexTrigger Press", AxisType.Digital, DeviceInputType.TriggerPress),
            new MixedRealityInputActionMapping("Axis1D.PrimaryHandTrigger Press", AxisType.SingleAxis, DeviceInputType.GripPress),
            new MixedRealityInputActionMapping("Axis2D.PrimaryThumbstick", AxisType.DualAxis, DeviceInputType.ThumbStick),
            new MixedRealityInputActionMapping("Button.PrimaryThumbstick Touch", AxisType.Digital, DeviceInputType.ThumbStickTouch),
            new MixedRealityInputActionMapping("Button.PrimaryThumbstick Near Touch", AxisType.Digital, DeviceInputType.ThumbNearTouch),
            new MixedRealityInputActionMapping("Button.PrimaryThumbstick Press", AxisType.Digital, DeviceInputType.ThumbStickPress),
            new MixedRealityInputActionMapping("Button.Three Press", AxisType.Digital, DeviceInputType.PrimaryButtonPress),
            new MixedRealityInputActionMapping("Button.Four Press", AxisType.Digital, DeviceInputType.SecondaryButtonPress),
            new MixedRealityInputActionMapping("Button.Start Press", AxisType.Digital, DeviceInputType.Menu),
            new MixedRealityInputActionMapping("Button.Three Touch", AxisType.Digital, DeviceInputType.PrimaryButtonTouch),
            new MixedRealityInputActionMapping("Button.Four Touch", AxisType.Digital, DeviceInputType.SecondaryButtonTouch),
            new MixedRealityInputActionMapping("Touch.PrimaryThumbRest Touch", AxisType.Digital, DeviceInputType.ThumbTouch),
            new MixedRealityInputActionMapping("Touch.PrimaryThumbRest Near Touch", AxisType.Digital, DeviceInputType.ThumbNearTouch),
        };

        /// <inheritdoc />
        protected override MixedRealityInputActionMapping[] DefaultRightHandedMappings => new[]
        {
            new MixedRealityInputActionMapping("Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer),
            new MixedRealityInputActionMapping("Axis1D.SecondaryIndexTrigger", AxisType.SingleAxis, DeviceInputType.Trigger),
            new MixedRealityInputActionMapping("Axis1D.SecondaryIndexTrigger Touch", AxisType.Digital, DeviceInputType.TriggerTouch),
            new MixedRealityInputActionMapping("Axis1D.SecondaryIndexTrigger Near Touch", AxisType.Digital, DeviceInputType.TriggerNearTouch),
            new MixedRealityInputActionMapping("Axis1D.SecondaryIndexTrigger Press", AxisType.Digital, DeviceInputType.TriggerPress),
            new MixedRealityInputActionMapping("Axis1D.SecondaryHandTrigger Press", AxisType.SingleAxis, DeviceInputType.GripPress),
            new MixedRealityInputActionMapping("Axis2D.SecondaryThumbstick", AxisType.DualAxis, DeviceInputType.ThumbStick),
            new MixedRealityInputActionMapping("Button.SecondaryThumbstick Touch", AxisType.Digital, DeviceInputType.ThumbStickTouch),
            new MixedRealityInputActionMapping("Button.SecondaryThumbstick Near Touch", AxisType.Digital, DeviceInputType.ThumbNearTouch),
            new MixedRealityInputActionMapping("Button.SecondaryThumbstick Press", AxisType.Digital, DeviceInputType.ThumbStickPress),
            new MixedRealityInputActionMapping("Button.One Press", AxisType.Digital, DeviceInputType.PrimaryButtonPress),
            new MixedRealityInputActionMapping("Button.Two Press", AxisType.Digital, DeviceInputType.SecondaryButtonPress),
            new MixedRealityInputActionMapping("Button.One Touch", AxisType.Digital, DeviceInputType.PrimaryButtonTouch),
            new MixedRealityInputActionMapping("Button.Two Touch", AxisType.Digital, DeviceInputType.SecondaryButtonTouch),
            new MixedRealityInputActionMapping("Touch.SecondaryThumbRest Touch", AxisType.Digital, DeviceInputType.ThumbTouch),
            new MixedRealityInputActionMapping("Touch.SecondaryThumbRest Near Touch", AxisType.Digital, DeviceInputType.ThumbNearTouch),
        };
    }
}