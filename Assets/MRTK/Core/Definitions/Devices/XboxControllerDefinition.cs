// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Defines the base interactions and data that an controller can provide.
    /// </summary>
    public class XboxControllerDefinition : BaseInputSourceDefinition
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public XboxControllerDefinition() : base(Handedness.None)
        { }

        /// <inheritdoc />
        protected override MixedRealityInputActionMapping[] DefaultMappings => new[]
        {
            new MixedRealityInputActionMapping("Left Thumbstick", AxisType.DualAxis, DeviceInputType.ThumbStick),
            new MixedRealityInputActionMapping("Left Thumbstick Click", AxisType.Digital, DeviceInputType.ButtonPress),
            new MixedRealityInputActionMapping("Right Thumbstick", AxisType.DualAxis, DeviceInputType.ThumbStick),
            new MixedRealityInputActionMapping("Right Thumbstick Click", AxisType.Digital, DeviceInputType.ButtonPress),
            new MixedRealityInputActionMapping("D-Pad", AxisType.DualAxis, DeviceInputType.DirectionalPad),
            new MixedRealityInputActionMapping("Shared Trigger", AxisType.SingleAxis, DeviceInputType.Trigger),
            new MixedRealityInputActionMapping("Left Trigger", AxisType.SingleAxis, DeviceInputType.Trigger),
            new MixedRealityInputActionMapping("Right Trigger", AxisType.SingleAxis, DeviceInputType.Trigger),
            new MixedRealityInputActionMapping("View", AxisType.Digital, DeviceInputType.ButtonPress),
            new MixedRealityInputActionMapping("Menu", AxisType.Digital, DeviceInputType.ButtonPress),
            new MixedRealityInputActionMapping("Left Bumper", AxisType.Digital, DeviceInputType.ButtonPress),
            new MixedRealityInputActionMapping("Right Bumper", AxisType.Digital, DeviceInputType.ButtonPress),
            new MixedRealityInputActionMapping("A", AxisType.Digital, DeviceInputType.ButtonPress),
            new MixedRealityInputActionMapping("B", AxisType.Digital, DeviceInputType.ButtonPress),
            new MixedRealityInputActionMapping("X", AxisType.Digital, DeviceInputType.ButtonPress),
            new MixedRealityInputActionMapping("Y", AxisType.Digital, DeviceInputType.ButtonPress),
        };
    }
}
