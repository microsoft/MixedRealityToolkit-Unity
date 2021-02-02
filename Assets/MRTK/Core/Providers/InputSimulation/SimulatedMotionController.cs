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
        /// <summary>
        /// Whether the motion controller is currently being tracked
        /// </summary>
        public bool IsTracked => isTracked;

        private SimulatedMotionControllerButtonState buttonState = new SimulatedMotionControllerButtonState();
        /// <summary>
        /// States of buttons on the motion controller
        /// </summary>
        public SimulatedMotionControllerButtonState ButtonState => buttonState;

        /// <summary>
        /// Position of the motion controller
        /// </summary>
        public Vector3 Position { get; set; } = Vector3.zero;

        /// <summary>
        /// Rotation of the motion controller
        /// </summary>
        public Quaternion Rotation { get; set; } = Quaternion.identity;

        /// <summary>
        /// Delegate to function updating the position and rotation of the motion controller
        /// </summary>
        public delegate MixedRealityPose MotionControllerPoseUpdater();

        /// <summary>
        /// Replace the motion controller data with the given values.
        /// </summary>
        /// <returns>True if the motion controller data has been changed.</returns>
        /// <param name="isTrackedNew">True if the motion controller is currently tracked.</param>
        /// <param name="buttonStateNew">New set of states of buttons on the motion controller.</param>
        /// <param name="updater">Delegate to function that updates the position and rotation of the motion controller. The delegate is only used when the motion controller is tracked.</param>
        /// <remarks>The timestamp of the motion controller data will be the current time, see [DateTime.UtcNow](https://docs.microsoft.com/dotnet/api/system.datetime.utcnow?view=netframework-4.8).</remarks>
        public bool Update(bool isTrackedNew, SimulatedMotionControllerButtonState buttonStateNew, MotionControllerPoseUpdater updater)
        {
            bool motionControllerDataChanged = false;

            if (isTracked != isTrackedNew || buttonState != buttonStateNew)
            {
                isTracked = isTrackedNew;
                buttonState = buttonStateNew;
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
        public SimulatedMotionController(
            TrackingState trackingState,
            Handedness controllerHandedness,
            IMixedRealityInputSource inputSource = null,
            MixedRealityInteractionMapping[] interactions = null)
            : base(trackingState, controllerHandedness, inputSource, interactions, new WindowsMixedRealityControllerDefinition(controllerHandedness))
        { }

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
                    case DeviceInputType.SpatialGrip:
                        Interactions[i].PoseData = currentPose;
                        if (Interactions[i].Changed)
                        {
                            CoreServices.InputSystem?.RaisePoseInputChanged(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction, currentPose);
                        }
                        break;
                    case DeviceInputType.Select:
                        Interactions[i].BoolData = motionControllerData.ButtonState.IsSelecting;
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
                    case DeviceInputType.GripPress:
                    case DeviceInputType.TriggerPress:
                        Interactions[i].FloatData = motionControllerData.ButtonState.IsGrabbing ? 1 : 0;
                        if (Interactions[i].Changed)
                        {
                            CoreServices.InputSystem?.RaiseFloatInputChanged(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction, motionControllerData.ButtonState.IsGrabbing ? 1 : 0);
                        }
                        break;
                    case DeviceInputType.Menu:
                        Interactions[i].BoolData = motionControllerData.ButtonState.IsPressingMenu;
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
