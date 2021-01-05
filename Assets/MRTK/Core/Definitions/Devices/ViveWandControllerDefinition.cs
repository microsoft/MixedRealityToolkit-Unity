// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// 
    /// </summary>
    public class ViveWandControllerDefinition : BaseControllerDefinition
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="handedness"></param>
        public ViveWandControllerDefinition(Handedness handedness) : base(handedness)
        {
            if ((handedness != Handedness.Left) &&
                (handedness != Handedness.Right))
            {
                throw new ArgumentException($"Unsupported Handedness ({handedness}). The ViveWandControllerDefinition supports Left and Right.");
            }
        }

        /// <inheritdoc />
        protected override MixedRealityInteractionMapping[] DefaultMappings => new[]
        {
            new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer),
            new MixedRealityInteractionMapping(1, "Trigger Position", AxisType.SingleAxis, DeviceInputType.Trigger),
            new MixedRealityInteractionMapping(2, "Trigger Press (Select)", AxisType.Digital, DeviceInputType.Select),
            new MixedRealityInteractionMapping(3, "Trigger Touch", AxisType.Digital, DeviceInputType.TriggerTouch),
            new MixedRealityInteractionMapping(4, "Grip Press", AxisType.SingleAxis, DeviceInputType.Trigger),
            new MixedRealityInteractionMapping(5, "Trackpad Position", AxisType.DualAxis, DeviceInputType.Touchpad),
            new MixedRealityInteractionMapping(6, "Trackpad Touch", AxisType.Digital, DeviceInputType.TouchpadTouch),
            new MixedRealityInteractionMapping(7, "Trackpad Press", AxisType.Digital, DeviceInputType.TouchpadPress),
            new MixedRealityInteractionMapping(8, "Menu Button", AxisType.Digital, DeviceInputType.ButtonPress),
        };
    }
}