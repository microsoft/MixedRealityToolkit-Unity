// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    [MixedRealityController(
        SupportedControllerType.ArticulatedHand,
        new[] { Handedness.Left, Handedness.Right })]
    public class SimulatedArticulatedHand : SimulatedHand
    {
        public override ControllerSimulationMode SimulationMode => ControllerSimulationMode.ArticulatedHand;

        private Vector3 currentPointerPosition = Vector3.zero;
        private Quaternion currentPointerRotation = Quaternion.identity;
        private MixedRealityPose lastPointerPose = MixedRealityPose.ZeroIdentity;
        private MixedRealityPose currentPointerPose = MixedRealityPose.ZeroIdentity;
        private MixedRealityPose currentIndexPose = MixedRealityPose.ZeroIdentity;
        private MixedRealityPose currentGripPose = MixedRealityPose.ZeroIdentity;
        private MixedRealityPose lastGripPose = MixedRealityPose.ZeroIdentity;

        /// <summary>
        /// Constructor.
        /// </summary>
        public SimulatedArticulatedHand(
            TrackingState trackingState,
            Handedness controllerHandedness,
            IMixedRealityInputSource inputSource = null,
            MixedRealityInteractionMapping[] interactions = null)
            : base(trackingState, controllerHandedness, inputSource, interactions, new ArticulatedHandDefinition(inputSource, controllerHandedness))
        { }

        private ArticulatedHandDefinition handDefinition;
        private ArticulatedHandDefinition HandDefinition => handDefinition ?? (handDefinition = Definition as ArticulatedHandDefinition);

        /// <inheritdoc />
        protected override void UpdateHandJoints(SimulatedHandData handData)
        {
            for (int i = 0; i < jointCount; i++)
            {
                TrackedHandJoint handJoint = (TrackedHandJoint)i;

                if (!jointPoses.ContainsKey(handJoint))
                {
                    jointPoses.Add(handJoint, handData.Joints[i]);
                }
                else
                {
                    jointPoses[handJoint] = handData.Joints[i];
                }
            }

            HandDefinition?.UpdateHandJoints(jointPoses);
        }

        /// <inheritdoc />
        protected override void UpdateInteractions(SimulatedHandData handData)
        {
            lastPointerPose = currentPointerPose;
            lastGripPose = currentGripPose;

            // For convenience of simulating in Unity Editor, make the ray use the index
            // finger position instead of knuckle, since the index finger doesn't move when we press.
            Vector3 pointerPosition = jointPoses[TrackedHandJoint.IndexTip].Position;
            IsPositionAvailable = IsRotationAvailable = pointerPosition != Vector3.zero;

            if (IsPositionAvailable)
            {
                HandRay.Update(pointerPosition, GetPalmNormal(), CameraCache.Main.transform, ControllerHandedness);

                Ray ray = HandRay.Ray;

                currentPointerPose.Position = ray.origin;
                currentPointerPose.Rotation = Quaternion.LookRotation(ray.direction);

                currentGripPose = jointPoses[TrackedHandJoint.Palm];
                currentIndexPose = jointPoses[TrackedHandJoint.IndexTip];
            }

            if (lastGripPose != currentGripPose)
            {
                if (IsPositionAvailable && IsRotationAvailable)
                {
                    CoreServices.InputSystem?.RaiseSourcePoseChanged(InputSource, this, currentGripPose);
                }
                else if (IsPositionAvailable && !IsRotationAvailable)
                {
                    CoreServices.InputSystem?.RaiseSourcePositionChanged(InputSource, this, currentPointerPosition);
                }
                else if (!IsPositionAvailable && IsRotationAvailable)
                {
                    CoreServices.InputSystem?.RaiseSourceRotationChanged(InputSource, this, currentPointerRotation);
                }
            }

            for (int i = 0; i < Interactions?.Length; i++)
            {
                switch (Interactions[i].InputType)
                {
                    case DeviceInputType.SpatialPointer:
                        Interactions[i].PoseData = currentPointerPose;
                        if (Interactions[i].Changed)
                        {
                            CoreServices.InputSystem?.RaisePoseInputChanged(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction, currentPointerPose);
                        }
                        break;
                    case DeviceInputType.SpatialGrip:
                        Interactions[i].PoseData = currentGripPose;
                        if (Interactions[i].Changed)
                        {
                            CoreServices.InputSystem?.RaisePoseInputChanged(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction, currentGripPose);
                        }
                        break;
                    case DeviceInputType.Select:
                    case DeviceInputType.TriggerPress:
                        Interactions[i].BoolData = handData.IsPinching;

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
                    case DeviceInputType.IndexFinger:
                        Interactions[i].PoseData = currentIndexPose;
                        if (Interactions[i].Changed)
                        {
                            CoreServices.InputSystem?.RaisePoseInputChanged(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction, currentIndexPose);
                        }
                        break;
                    case DeviceInputType.ThumbStick:
                        HandDefinition?.UpdateCurrentTeleportPose(Interactions[i]);
                        break;
                }
            }
        }
    }
}