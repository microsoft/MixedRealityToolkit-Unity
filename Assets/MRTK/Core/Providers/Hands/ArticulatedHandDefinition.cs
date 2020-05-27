// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Defines the interactions and data that an articulated hand can provide.
    /// </summary>
    public class ArticulatedHandDefinition
    {
        public ArticulatedHandDefinition(IMixedRealityInputSource source, Handedness handedness)
        {
            inputSource = source;
            this.handedness = handedness;
        }

        protected readonly IMixedRealityInputSource inputSource;
        protected readonly Handedness handedness;

        private readonly float cursorBeamBackwardTolerance = 0.5f;
        private readonly float cursorBeamUpTolerance = 0.8f;

        private Dictionary<TrackedHandJoint, MixedRealityPose> unityJointPoses = new Dictionary<TrackedHandJoint, MixedRealityPose>();
        private MixedRealityPose currentIndexPose = MixedRealityPose.ZeroIdentity;

        // Minimum distance between the index and the thumb tip required to enter a pinch
        private readonly float minimumPinchDistance = 0.015f;

        // Maximum distance between the index and thumb tip required to exit the pinch gesture
        private readonly float maximumPinchDistance = 0.1f;

        // Default enterPinchDistance value
        private float enterPinchDistance = 0.02f;

        /// <summary>
        /// The distance between the index finger tip and the thumb tip required to enter the pinch/air tap selection gesture.
        /// The pinch gesture enter will be registered for all values less than the EnterPinchDistance. The default EnterPinchDistance value is 0.02 and must be between 0.015 and 0.1. 
        /// </summary>
        public float EnterPinchDistance 
        {
            get => enterPinchDistance;
            set
            {
                if (value >= minimumPinchDistance && value <= maximumPinchDistance)
                {
                    enterPinchDistance = value;
                }
                else
                {
                    Debug.LogError("EnterPinchDistance must be between 0.015 and 0.1, please change Enter Pinch Distance in the Leap Motion Device Manager Profile");
                }   
            }
        }

        // Default exitPinchDistance value
        private float exitPinchDistance = 0.05f;

        /// <summary>
        /// The distance between the index finger tip and the thumb tip required to exit the pinch/air tap gesture.
        /// The pinch gesture exit will be registered for all values greater than the ExitPinchDistance. The default ExitPinchDistance value is 0.05 and must be between 0.015 and 0.1. 
        /// </summary>
        public float ExitPinchDistance
        {
            get => exitPinchDistance;
            set
            {
                if (value >= minimumPinchDistance && value <= maximumPinchDistance)
                {
                    exitPinchDistance = value;
                }
                else
                {
                    Debug.LogError("ExitPinchDistance must be between 0.015 and 0.1, please change Exit Pinch Distance in the Leap Motion Device Manager Profile");
                }
            }
        }

        private bool isPinching = false;

        /// <summary>
        /// The articulated hands default interactions.
        /// </summary>
        /// <remarks>A single interaction mapping works for both left and right articulated hands.</remarks>
        public MixedRealityInteractionMapping[] DefaultInteractions => new[]
        {
            new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer),
            new MixedRealityInteractionMapping(1, "Spatial Grip", AxisType.SixDof, DeviceInputType.SpatialGrip),
            new MixedRealityInteractionMapping(2, "Select", AxisType.Digital, DeviceInputType.Select),
            new MixedRealityInteractionMapping(3, "Grab", AxisType.SingleAxis, DeviceInputType.TriggerPress),
            new MixedRealityInteractionMapping(4, "Index Finger Pose", AxisType.SixDof, DeviceInputType.IndexFinger)
        };

        /// <summary>
        /// Calculates whether the current pose allows for pointing/distant interactions.
        /// </summary>
        public bool IsInPointingPose
        {
            get
            {
                MixedRealityPose palmJoint;
                if (unityJointPoses.TryGetValue(TrackedHandJoint.Palm, out palmJoint))
                {
                    Vector3 palmNormal = palmJoint.Rotation * (-1 * Vector3.up);
                    if (cursorBeamBackwardTolerance >= 0)
                    {
                        Vector3 cameraBackward = -CameraCache.Main.transform.forward;
                        if (Vector3.Dot(palmNormal.normalized, cameraBackward) > cursorBeamBackwardTolerance)
                        {
                            return false;
                        }
                    }
                    if (cursorBeamUpTolerance >= 0)
                    {
                        if (Vector3.Dot(palmNormal, Vector3.up) > cursorBeamUpTolerance)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        }

        private static readonly ProfilerMarker UpdateHandJointsPerfMarker = new ProfilerMarker("[MRTK] ArticulatedHandDefinition.UpdateHandJoints");

        /// <summary>
        /// Calculates whether the current the current joint pose is selecting (air tap gesture).
        /// </summary>
        public bool IsPinching
        {
            get
            {
                MixedRealityPose thumbTip;
                MixedRealityPose indexTip;
                if (unityJointPoses.TryGetValue(TrackedHandJoint.ThumbTip, out thumbTip) && unityJointPoses.TryGetValue(TrackedHandJoint.IndexTip, out indexTip))
                {
                    float distance = Vector3.Distance(thumbTip.Position, indexTip.Position);

                    if (isPinching && distance > ExitPinchDistance)
                    {
                        isPinching = false;
                    }
                    else if (!isPinching && distance < EnterPinchDistance)
                    {
                        isPinching = true;
                    }
                }
                else
                {
                    isPinching = false;
                }

                return isPinching;
            }
        }

        /// <summary>
        /// Updates the current hand joints with new data.
        /// </summary>
        /// <param name="jointPoses">The new joint poses.</param>
        public void UpdateHandJoints(Dictionary<TrackedHandJoint, MixedRealityPose> jointPoses)
        {
            using (UpdateHandJointsPerfMarker.Auto())
            {
                unityJointPoses = jointPoses;
                CoreServices.InputSystem?.RaiseHandJointsUpdated(inputSource, handedness, unityJointPoses);
            }
        }

        private static readonly ProfilerMarker UpdateCurrentIndexPosePerfMarker = new ProfilerMarker("[MRTK] ArticulatedHandDefinition.UpdateCurrentIndexPose");

        /// <summary>
        /// Updates the MixedRealityInteractionMapping with the latest index pose and fires a corresponding pose event.
        /// </summary>
        /// <param name="interactionMapping">The index finger's interaction mapping.</param>
        public void UpdateCurrentIndexPose(MixedRealityInteractionMapping interactionMapping)
        {
            using (UpdateCurrentIndexPosePerfMarker.Auto())
            {
                if (unityJointPoses.TryGetValue(TrackedHandJoint.IndexTip, out currentIndexPose))
                {
                    // Update the interaction data source
                    interactionMapping.PoseData = currentIndexPose;

                    // If our value changed raise it
                    if (interactionMapping.Changed)
                    {
                        // Raise input system event if it's enabled
                        CoreServices.InputSystem?.RaisePoseInputChanged(inputSource, handedness, interactionMapping.MixedRealityInputAction, currentIndexPose);
                    }
                }
            }
        }
    }
}
