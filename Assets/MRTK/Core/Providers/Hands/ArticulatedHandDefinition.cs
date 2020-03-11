// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
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

        /// <summary>
        /// Updates the current hand joints with new data.
        /// </summary>
        /// <param name="jointPoses">The new joint poses.</param>
        public void UpdateHandJoints(Dictionary<TrackedHandJoint, MixedRealityPose> jointPoses)
        {
            unityJointPoses = jointPoses;
            CoreServices.InputSystem?.RaiseHandJointsUpdated(inputSource, handedness, unityJointPoses);
        }

        /// <summary>
        /// Updates the MixedRealityInteractionMapping with the latest index pose and fires a corresponding pose event.
        /// </summary>
        /// <param name="interactionMapping">The index finger's interaction mapping.</param>
        public void UpdateCurrentIndexPose(MixedRealityInteractionMapping interactionMapping)
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
