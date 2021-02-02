// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Defines the base interactions and data that an controller can provide.
    /// </summary>
    public class MouseControllerDefinition : BaseInputSourceDefinition
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public MouseControllerDefinition() : base(Handedness.None)
        { }

        /// <inheritdoc />
        protected override MixedRealityInputActionMapping[] DefaultMappings => new[]
        {
            new MixedRealityInputActionMapping("Spatial Mouse Position", AxisType.SixDof, DeviceInputType.SpatialPointer),
            new MixedRealityInputActionMapping("Mouse Delta Position", AxisType.DualAxis, DeviceInputType.PointerPosition),
            new MixedRealityInputActionMapping("Mouse Scroll Position", AxisType.DualAxis, DeviceInputType.Scroll),
            new MixedRealityInputActionMapping("Left Mouse Button", AxisType.Digital, DeviceInputType.ButtonPress),
            new MixedRealityInputActionMapping("Right Mouse Button", AxisType.Digital, DeviceInputType.ButtonPress),
            new MixedRealityInputActionMapping("Mouse Button 2", AxisType.Digital, DeviceInputType.ButtonPress),
            new MixedRealityInputActionMapping("Mouse Button 3", AxisType.Digital, DeviceInputType.ButtonPress),
            new MixedRealityInputActionMapping("Mouse Button 4", AxisType.Digital, DeviceInputType.ButtonPress),
            new MixedRealityInputActionMapping("Mouse Button 5", AxisType.Digital, DeviceInputType.ButtonPress),
            new MixedRealityInputActionMapping("Mouse Button 6", AxisType.Digital, DeviceInputType.ButtonPress),
        };
    }
}
