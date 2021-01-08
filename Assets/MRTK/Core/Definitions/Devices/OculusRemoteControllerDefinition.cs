// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// 
    /// </summary>
    public class OculusRemoteControllerDefinition : BaseInputSourceDefinition
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public OculusRemoteControllerDefinition() : base(Handedness.None)
        { }

        /// <inheritdoc />
        protected override MixedRealityInputActionMapping[] DefaultMappings => new[]
        {
            new MixedRealityInputActionMapping("D-Pad Position", AxisType.DualAxis, DeviceInputType.DirectionalPad),
            new MixedRealityInputActionMapping("Button.One", AxisType.Digital, DeviceInputType.ButtonPress),
            new MixedRealityInputActionMapping("Button.Two", AxisType.Digital, DeviceInputType.ButtonPress),
        };
    }
}