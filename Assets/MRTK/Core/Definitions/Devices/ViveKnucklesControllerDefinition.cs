// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// 
    /// </summary>
    public class ViveKnucklesControllerDefinition : BaseInputSourceDefinition
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="handedness">The handedness that this definition instance represents.</param>
        public ViveKnucklesControllerDefinition(Handedness handedness) : base(handedness)
        {
            if ((handedness != Handedness.Left) &&
                (handedness != Handedness.Right))
            {
                throw new System.ArgumentException($"Unsupported Handedness ({handedness}). The ViveKnucklesControllerDefinition supports Left and Right.");
            }
        }

        /// <inheritdoc />
        protected override MixedRealityInputActionMapping[] DefaultMappings => new[]
        {
            new MixedRealityInputActionMapping("Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer),
            new MixedRealityInputActionMapping("Trigger Position", AxisType.SingleAxis, DeviceInputType.Trigger),
            new MixedRealityInputActionMapping("Trigger Press (Select)", AxisType.Digital, DeviceInputType.Select),
            new MixedRealityInputActionMapping("Trigger Touch", AxisType.Digital, DeviceInputType.TriggerTouch),
            new MixedRealityInputActionMapping("Grip Average", AxisType.SingleAxis, DeviceInputType.Trigger),
            new MixedRealityInputActionMapping("Trackpad Position", AxisType.DualAxis, DeviceInputType.Touchpad),
            new MixedRealityInputActionMapping("Trackpad Touch", AxisType.Digital, DeviceInputType.TouchpadTouch),
            new MixedRealityInputActionMapping("Trackpad Press", AxisType.Digital, DeviceInputType.TouchpadPress),
            new MixedRealityInputActionMapping("Inner Face Button", AxisType.Digital, DeviceInputType.ButtonPress),
            new MixedRealityInputActionMapping("Outer Face Button", AxisType.Digital, DeviceInputType.ButtonPress),
            new MixedRealityInputActionMapping("Index Finger Cap Sensor", AxisType.SingleAxis, DeviceInputType.IndexFinger),
            new MixedRealityInputActionMapping("Middle Finger Cap Sensor", AxisType.SingleAxis, DeviceInputType.MiddleFinger),
            new MixedRealityInputActionMapping("Ring Finger Cap Sensor", AxisType.SingleAxis, DeviceInputType.RingFinger),
            new MixedRealityInputActionMapping("Pinky Finger Cap Sensor", AxisType.SingleAxis, DeviceInputType.PinkyFinger),
        };
    }
}