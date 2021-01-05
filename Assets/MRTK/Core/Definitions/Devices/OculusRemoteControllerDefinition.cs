// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// 
    /// </summary>
    public class OculusRemoteControllerDefinition : BaseControllerDefinition
    {
        /// <summary>
        /// 
        /// </summary>
        public OculusRemoteControllerDefinition() : base(Handedness.None) { }

        /// <inheritdoc />
        protected override MixedRealityInteractionMapping[] DefaultMappings => new[]
        {
            new MixedRealityInteractionMapping(0, "D-Pad Position", AxisType.DualAxis, DeviceInputType.DirectionalPad),
            new MixedRealityInteractionMapping(1, "Button.One", AxisType.Digital, DeviceInputType.ButtonPress),
            new MixedRealityInteractionMapping(2, "Button.Two", AxisType.Digital, DeviceInputType.ButtonPress),
        };
    }
}