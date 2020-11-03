// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// 
    /// </summary>
    public class ViveKnucklesControllerDefinition : BaseControllerDefinition
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="handedness"></param>
        public ViveKnucklesControllerDefinition(Handedness handedness) : base(handedness)
        {
            if ((Handedness != Handedness.Left) &&
                (Handedness != Handedness.Right))
            {
                throw new ArgumentException($"Unsupported Handedness ({Handedness}). The ViveKnucklesControllerDefinition supports Left and Right.");
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
            new MixedRealityInteractionMapping(4, "Grip Average", AxisType.SingleAxis, DeviceInputType.Trigger),
            new MixedRealityInteractionMapping(5, "Trackpad Position", AxisType.DualAxis, DeviceInputType.Touchpad),
            new MixedRealityInteractionMapping(6, "Trackpad Touch", AxisType.Digital, DeviceInputType.TouchpadTouch),
            new MixedRealityInteractionMapping(7, "Trackpad Press", AxisType.Digital, DeviceInputType.TouchpadPress),
            new MixedRealityInteractionMapping(8, "Inner Face Button", AxisType.Digital, DeviceInputType.ButtonPress),
            new MixedRealityInteractionMapping(9, "Outer Face Button", AxisType.Digital, DeviceInputType.ButtonPress),
            new MixedRealityInteractionMapping(10, "Index Finger Cap Sensor", AxisType.SingleAxis, DeviceInputType.IndexFinger),
            new MixedRealityInteractionMapping(11, "Middle Finger Cap Sensor", AxisType.SingleAxis, DeviceInputType.MiddleFinger),
            new MixedRealityInteractionMapping(12, "Ring Finger Cap Sensor", AxisType.SingleAxis, DeviceInputType.RingFinger),
            new MixedRealityInteractionMapping(13, "Pinky Finger Cap Sensor", AxisType.SingleAxis, DeviceInputType.PinkyFinger),
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
            new MixedRealityInteractionMapping(4, "Grip Average", AxisType.SingleAxis, DeviceInputType.Trigger),
            new MixedRealityInteractionMapping(5, "Trackpad Position", AxisType.DualAxis, DeviceInputType.Touchpad),
            new MixedRealityInteractionMapping(6, "Trackpad Touch", AxisType.Digital, DeviceInputType.TouchpadTouch),
            new MixedRealityInteractionMapping(7, "Trackpad Press", AxisType.Digital, DeviceInputType.TouchpadPress),
            new MixedRealityInteractionMapping(8, "Inner Face Button", AxisType.Digital, DeviceInputType.ButtonPress),
            new MixedRealityInteractionMapping(9, "Outer Face Button", AxisType.Digital, DeviceInputType.ButtonPress),
            new MixedRealityInteractionMapping(10, "Index Finger Cap Sensor", AxisType.SingleAxis, DeviceInputType.IndexFinger),
            new MixedRealityInteractionMapping(11, "Middle Finger Cap Sensor", AxisType.SingleAxis, DeviceInputType.MiddleFinger),
            new MixedRealityInteractionMapping(12, "Ring Finger Cap Sensor", AxisType.SingleAxis, DeviceInputType.RingFinger),
            new MixedRealityInteractionMapping(13, "Pinky Finger Cap Sensor", AxisType.SingleAxis, DeviceInputType.PinkyFinger),
        };
    }
}