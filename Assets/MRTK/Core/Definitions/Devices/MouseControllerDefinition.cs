// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Defines the base interactions and data that an controller can provide.
    /// </summary>
    public class MouseControllerDefinition : BaseControllerDefinition
    {
        /// <summary>
        /// 
        /// </summary>
        public MouseControllerDefinition() : base(Handedness.None)
        { }

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultInteractions { get; } =
        {
            new MixedRealityInteractionMapping(0, "Spatial Mouse Position", AxisType.SixDof, DeviceInputType.SpatialPointer),
            new MixedRealityInteractionMapping(1, "Mouse Delta Position", AxisType.DualAxis, DeviceInputType.PointerPosition),
            new MixedRealityInteractionMapping(2, "Mouse Scroll Position", AxisType.DualAxis, DeviceInputType.Scroll),
            new MixedRealityInteractionMapping(3, "Left Mouse Button", AxisType.Digital, DeviceInputType.ButtonPress),
            new MixedRealityInteractionMapping(4, "Right Mouse Button", AxisType.Digital, DeviceInputType.ButtonPress),
            new MixedRealityInteractionMapping(5, "Mouse Button 2", AxisType.Digital, DeviceInputType.ButtonPress),
            new MixedRealityInteractionMapping(6, "Mouse Button 3", AxisType.Digital, DeviceInputType.ButtonPress),
            new MixedRealityInteractionMapping(7, "Mouse Button 4", AxisType.Digital, DeviceInputType.ButtonPress),
            new MixedRealityInteractionMapping(8, "Mouse Button 5", AxisType.Digital, DeviceInputType.ButtonPress),
            new MixedRealityInteractionMapping(9, "Mouse Button 6", AxisType.Digital, DeviceInputType.ButtonPress),
        };
    }
}
