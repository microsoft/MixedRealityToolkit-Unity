// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

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
            if ((Handedness != Handedness.Left) &&
                (Handedness != Handedness.Right))
            {
                throw new ArgumentException($"Unsupported Handedness ({Handedness}). The ViveWandControllerDefinition supports Left and Right.");
            }
        }

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultInteractions =>
            (Handedness == Handedness.Left) ? DefaultLeftHandedInteractions : DefaultRightHandedInteractions;

        /// <summary>
        /// 
        /// </summary>
        private MixedRealityInteractionMapping[] DefaultLeftHandedInteractions => new[]
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

        /// <summary>
        /// 
        /// </summary>
        private MixedRealityInteractionMapping[] DefaultRightHandedInteractions => new[]
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