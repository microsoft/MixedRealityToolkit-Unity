// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// 
    /// </summary>
    public class GenericOpenVRControllerDefinition : BaseInputSourceDefinition
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="handedness"></param>
        public GenericOpenVRControllerDefinition(Handedness handedness) : base(handedness)
        {
        }

        /// <inheritdoc />
        protected override MixedRealityInteractionMapping[] DefaultLeftHandedMappings => new[]
        {
            // Controller Pose
            new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer),
            // HTC Vive Controller - Left Controller Trigger (7) Squeeze
            // Oculus Touch Controller - Axis1D.PrimaryIndexTrigger Squeeze
            // Valve Knuckles Controller - Left Controller Trigger Squeeze
            // Windows Mixed Reality Controller - Left Trigger Squeeze
            new MixedRealityInteractionMapping(1, "Trigger Position", AxisType.SingleAxis, DeviceInputType.Trigger),
            // HTC Vive Controller - Left Controller Trigger (7)
            // Oculus Touch Controller - Axis1D.PrimaryIndexTrigger
            // Valve Knuckles Controller - Left Controller Trigger
            // Windows Mixed Reality Controller - Left Trigger Press (Select)
            new MixedRealityInteractionMapping(2, "Trigger Press (Select)", AxisType.Digital, DeviceInputType.Select),
            // HTC Vive Controller - Left Controller Trigger (7)
            // Oculus Touch Controller - Axis1D.PrimaryIndexTrigger
            // Valve Knuckles Controller - Left Controller Trigger
            new MixedRealityInteractionMapping(3, "Trigger Touch", AxisType.Digital, DeviceInputType.TriggerTouch),
            // HTC Vive Controller - Left Controller Grip Button (8)
            // Oculus Touch Controller - Axis1D.PrimaryHandTrigger
            // Valve Knuckles Controller - Left Controller Grip Average
            // Windows Mixed Reality Controller - Left Grip Button Press
            new MixedRealityInteractionMapping(4, "Grip Trigger Position", AxisType.SingleAxis, DeviceInputType.Trigger),
            // HTC Vive Controller - Left Controller Trackpad (2)
            // Oculus Touch Controller - Axis2D.PrimaryThumbstick
            // Valve Knuckles Controller - Left Controller Trackpad
            // Windows Mixed Reality Controller - Left Thumbstick Position
            new MixedRealityInteractionMapping(5, "Trackpad-Thumbstick Position", AxisType.DualAxis, DeviceInputType.Touchpad),
            // HTC Vive Controller - Left Controller Trackpad (2)
            // Oculus Touch Controller - Button.PrimaryThumbstick
            // Valve Knuckles Controller - Left Controller Trackpad
            // Windows Mixed Reality Controller - Left Touchpad Touch
            new MixedRealityInteractionMapping(6, "Trackpad-Thumbstick Touch", AxisType.Digital, DeviceInputType.TouchpadTouch),
            // HTC Vive Controller - Left Controller Trackpad (2)
            // Oculus Touch Controller - Button.PrimaryThumbstick
            // Valve Knuckles Controller - Left Controller Trackpad
            // Windows Mixed Reality Controller - Left Thumbstick Press
            new MixedRealityInteractionMapping(7, "Trackpad-Thumbstick Press", AxisType.Digital, DeviceInputType.TouchpadPress),
            // HTC Vive Controller - Left Controller Menu Button (1)
            // Oculus Touch Controller - Button.Three Press
            // Valve Knuckles Controller - Left Controller Inner Face Button
            // Windows Mixed Reality Controller - Left Menu Button
            new MixedRealityInteractionMapping(8, "Unity Button Id 2", AxisType.Digital, DeviceInputType.ButtonPress),
            // Oculus Touch Controller - Button.Four Press
            // Valve Knuckles Controller - Left Controller Outer Face Button
            new MixedRealityInteractionMapping(9, "Unity Button Id 3", AxisType.Digital, DeviceInputType.ButtonPress),
            new MixedRealityInteractionMapping(10, "WMR Touchpad Touch", AxisType.Digital, DeviceInputType.TouchpadTouch),
            new MixedRealityInteractionMapping(11, "WMR Touchpad Position", AxisType.DualAxis, DeviceInputType.Touchpad),
            new MixedRealityInteractionMapping(12, "Spatial Grip", AxisType.SixDof, DeviceInputType.SpatialGrip),
        };

        /// <inheritdoc />
        protected override MixedRealityInteractionMapping[] DefaultRightHandedMappings => new[]
        {
            // Controller Pose
            new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer),
            // HTC Vive Controller - Right Controller Trigger (7) Squeeze
            // Oculus Touch Controller - Axis1D.SecondaryIndexTrigger Squeeze
            // Valve Knuckles Controller - Right Controller Trigger Squeeze
            // Windows Mixed Reality Controller - Right Trigger Squeeze
            new MixedRealityInteractionMapping(1, "Trigger Position", AxisType.SingleAxis, DeviceInputType.Trigger),
            // HTC Vive Controller - Right Controller Trigger (7)
            // Oculus Touch Controller - Axis1D.SecondaryIndexTrigger
            // Valve Knuckles Controller - Right Controller Trigger
            // Windows Mixed Reality Controller - Right Trigger Press (Select)
            new MixedRealityInteractionMapping(2, "Trigger Press (Select)", AxisType.Digital, DeviceInputType.Select),
            // HTC Vive Controller - Right Controller Trigger (7)
            // Oculus Touch Controller - Axis1D.SecondaryIndexTrigger
            // Valve Knuckles Controller - Right Controller Trigger
            new MixedRealityInteractionMapping(3, "Trigger Touch", AxisType.Digital, DeviceInputType.TriggerTouch),
            // HTC Vive Controller - Right Controller Grip Button (8)
            // Oculus Touch Controller - Axis1D.SecondaryHandTrigger
            // Valve Knuckles Controller - Right Controller Grip Average
            // Windows Mixed Reality Controller - Right Grip Button Press
            new MixedRealityInteractionMapping(4, "Grip Trigger Position", AxisType.SingleAxis, DeviceInputType.Trigger),
            // HTC Vive Controller - Right Controller Trackpad (2)
            // Oculus Touch Controller - Axis2D.PrimaryThumbstick
            // Valve Knuckles Controller - Right Controller Trackpad
            // Windows Mixed Reality Controller - Right Thumbstick Position
            new MixedRealityInteractionMapping(5, "Trackpad-Thumbstick Position", AxisType.DualAxis, DeviceInputType.Touchpad),
            // HTC Vive Controller - Right Controller Trackpad (2)
            // Oculus Touch Controller - Button.SecondaryThumbstick
            // Valve Knuckles Controller - Right Controller Trackpad
            // Windows Mixed Reality Controller - Right Touchpad Touch
            new MixedRealityInteractionMapping(6, "Trackpad-Thumbstick Touch", AxisType.Digital, DeviceInputType.TouchpadTouch),
            // HTC Vive Controller - Right Controller Trackpad (2)
            // Oculus Touch Controller - Button.SecondaryThumbstick
            // Valve Knuckles Controller - Right Controller Trackpad
            // Windows Mixed Reality Controller - Right Thumbstick Press
            new MixedRealityInteractionMapping(7, "Trackpad-Thumbstick Press", AxisType.Digital, DeviceInputType.TouchpadPress),
            // HTC Vive Controller - Right Controller Menu Button (1)
            // Oculus Remote - Button.One Press
            // Oculus Touch Controller - Button.One Press
            // Valve Knuckles Controller - Right Controller Inner Face Button
            // Windows Mixed Reality Controller - Right Menu Button
            new MixedRealityInteractionMapping(8, "Unity Button Id 0", AxisType.Digital, DeviceInputType.ButtonPress),
            // Oculus Remote - Button.Two Press
            // Oculus Touch Controller - Button.Two Press
            // Valve Knuckles Controller - Right Controller Outer Face Button
            new MixedRealityInteractionMapping(9, "Unity Button Id 1", AxisType.Digital, DeviceInputType.ButtonPress),
            new MixedRealityInteractionMapping(10, "WMR Touchpad Touch", AxisType.Digital, DeviceInputType.TouchpadTouch),
            new MixedRealityInteractionMapping(11, "WMR Touchpad Position", AxisType.DualAxis, DeviceInputType.Touchpad),
            new MixedRealityInteractionMapping(12, "Spatial Grip", AxisType.SixDof, DeviceInputType.SpatialGrip),
        };
    }
}