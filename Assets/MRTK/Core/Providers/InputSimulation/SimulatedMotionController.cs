// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Snapshot of simulated motion controller data.
    /// </summary>
    public class SimulatedMotionControllerData
    {
        [SerializeField]
        private bool isTracked = false;
        public bool IsTracked => isTracked;
        [SerializeField]
        private bool isSelecting = false;
        public bool IsSelecting => isSelecting;
        [SerializeField]
        private bool isGrabbing = false;
        public bool IsGrabbing => isGrabbing;
        [SerializeField]
        private bool isPressingMenu = false;
        public bool IsPressingMenu => isPressingMenu;

        public Vector3 Position { get; set; } = Vector3.zero;
        public Quaternion Rotation { get; set; } = Quaternion.identity;

        public delegate MixedRealityPose MotionControllerPoseUpdater();

        /// <summary>
        /// Replace the motion controller data with the given values.
        /// </summary>
        /// <returns>True if the motion controller data has been changed.</returns>
        /// <param name="isTrackedNew">True if the motion controller is currently tracked.</param>
        /// <param name="isSelectingNew">True if the motion controller is currently selecting something.</param>
        /// <param name="isGrabbingNew">True if the motion controller is currently grabbing something.</param>
        /// <param name="isPressingMenuNew">True if the menu button of the motion controller is currently being pressed.</param>
        /// <param name="updater">Delegate to function that updates the position and rotation of the motion controller. The delegate is only used when the motion controller is tracked.</param>
        /// <remarks>The timestamp of the motion controller data will be the current time, see [DateTime.UtcNow](https://docs.microsoft.com/dotnet/api/system.datetime.utcnow?view=netframework-4.8).</remarks>
        public bool Update(bool isTrackedNew, bool isSelectingNew, bool isGrabbingNew, bool isPressingMenuNew, MotionControllerPoseUpdater updater)
        {
            bool motionControllerDataChanged = false;

            if (isTracked != isTrackedNew || isSelecting != isSelectingNew || isGrabbing != isGrabbingNew || isPressingMenu != isPressingMenuNew)
            {
                isTracked = isTrackedNew;
                isSelecting = isSelectingNew;
                isGrabbing = isGrabbingNew;
                isPressingMenu = isPressingMenuNew;
                motionControllerDataChanged = true;
            }

            if (isTracked)
            {
                MixedRealityPose pose = updater();
                Position = pose.Position;
                Rotation = pose.Rotation;
                motionControllerDataChanged = true;
            }

            return motionControllerDataChanged;
        }
    }

    [MixedRealityController(
        SupportedControllerType.WindowsMixedReality,
        new[] { Handedness.Left, Handedness.Right })]
    public class SimulatedMotionController : BaseController
    {
        private MixedRealityPose currentPose = MixedRealityPose.ZeroIdentity;
        private MixedRealityPose lastPose = MixedRealityPose.ZeroIdentity;

        /// <summary>
        /// Constructor.
        /// </summary>
        public SimulatedMotionController(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
                : base(trackingState, controllerHandedness, inputSource, interactions) { }

        /// <summary>
        /// The simulated motion controller's default interactions.
        /// </summary>
        /// <remarks>A single interaction mapping works for both left and right controllers.</remarks>
        public override MixedRealityInteractionMapping[] DefaultInteractions => new[]
        {
            new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer),
            new MixedRealityInteractionMapping(1, "Spatial Grip", AxisType.SixDof, DeviceInputType.SpatialGrip),
            new MixedRealityInteractionMapping(2, "Grip Press", AxisType.SingleAxis, DeviceInputType.TriggerPress),
            new MixedRealityInteractionMapping(3, "Trigger Position", AxisType.SingleAxis, DeviceInputType.Trigger),
            new MixedRealityInteractionMapping(4, "Trigger Touch", AxisType.Digital, DeviceInputType.TriggerTouch),
            new MixedRealityInteractionMapping(5, "Trigger Press (Select)", AxisType.Digital, DeviceInputType.Select),
            new MixedRealityInteractionMapping(6, "Touchpad Position", AxisType.DualAxis, DeviceInputType.Touchpad),
            new MixedRealityInteractionMapping(7, "Touchpad Touch", AxisType.Digital, DeviceInputType.TouchpadTouch),
            new MixedRealityInteractionMapping(8, "Touchpad Press", AxisType.Digital, DeviceInputType.TouchpadPress),
            new MixedRealityInteractionMapping(9, "Menu Press", AxisType.Digital, DeviceInputType.Menu),
            new MixedRealityInteractionMapping(10, "Thumbstick Position", AxisType.DualAxis, DeviceInputType.ThumbStick),
            new MixedRealityInteractionMapping(11, "Thumbstick Press", AxisType.Digital, DeviceInputType.ThumbStickPress),
        };

        internal void UpdateState(SimulatedMotionControllerData motionControllerData)
        {
            lastPose = currentPose;
            currentPose.Position = motionControllerData.Position;
            currentPose.Rotation = motionControllerData.Rotation;
            IsPositionAvailable = IsRotationAvailable = motionControllerData.Position != Vector3.zero;

            if (lastPose != currentPose)
            {
                if (IsPositionAvailable && IsRotationAvailable)
                {
                    CoreServices.InputSystem?.RaiseSourcePoseChanged(InputSource, this, currentPose);
                }
            }

            for (int i = 0; i < Interactions?.Length; i++)
            {
                switch (Interactions[i].InputType)
                {
                    case DeviceInputType.SpatialPointer:
                        Interactions[i].PoseData = currentPose;
                        if (Interactions[i].Changed)
                        {
                            CoreServices.InputSystem?.RaisePoseInputChanged(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction, currentPose);
                        }
                        break;
                    case DeviceInputType.SpatialGrip:
                        Interactions[i].PoseData = currentPose;
                        if (Interactions[i].Changed)
                        {
                            CoreServices.InputSystem?.RaisePoseInputChanged(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction, currentPose);
                        }
                        break;
                    case DeviceInputType.Select:
                        Interactions[i].BoolData = motionControllerData.IsSelecting;
                        if (Interactions[i].Changed)
                        {
                            if (Interactions[i].BoolData)
                            {
                                CoreServices.InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction);
                            }
                            else
                            {
                                CoreServices.InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction);
                            }
                        }
                        break;
                    case DeviceInputType.TriggerPress:
                        Interactions[i].FloatData = motionControllerData.IsGrabbing ? 1 : 0;
                        if (Interactions[i].Changed)
                        {
                            CoreServices.InputSystem?.RaiseFloatInputChanged(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction, motionControllerData.IsGrabbing ? 1 : 0);
                        }
                        break;
                    case DeviceInputType.Menu:
                        Interactions[i].BoolData = motionControllerData.IsPressingMenu;
                        if (Interactions[i].Changed)
                        {
                            if (Interactions[i].BoolData)
                            {
                                CoreServices.InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction);
                            }
                            else
                            {
                                CoreServices.InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction);
                            }
                        }
                        break;
                }
            }
        }
    }
}