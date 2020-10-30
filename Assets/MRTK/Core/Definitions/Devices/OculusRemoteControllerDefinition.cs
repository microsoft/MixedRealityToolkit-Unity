// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// 
    /// </summary>
    public class OpenVROculusRemoteControllerDefinition : BaseControllerDefinition
    {
        public OpenVROculusRemoteControllerDefinition(
            IMixedRealityInputSource source) : base(source, Handedness.None)
        { }

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultInteractions => new[]
        {
            new MixedRealityInteractionMapping(0, "D-Pad Position", AxisType.DualAxis, DeviceInputType.DirectionalPad),
            new MixedRealityInteractionMapping(1, "Button.One", AxisType.Digital, DeviceInputType.ButtonPress),
            new MixedRealityInteractionMapping(2, "Button.Two", AxisType.Digital, DeviceInputType.ButtonPress),
        };
    }
}