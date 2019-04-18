// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{

    /// <summary>
    /// Record joint positions of a hand and log them for use in simulated hands
    /// </summary>
    public class HandGestureRecorder : MonoBehaviour
    {
        private static readonly int jointCount = Enum.GetNames(typeof(TrackedHandJoint)).Length;

        public TrackedHandJoint ReferenceJoint = TrackedHandJoint.IndexTip;

        private Vector3 offset = Vector3.zero;

        void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.F9))
            {
                HandJointUtils.TryGetJointPose(ReferenceJoint, Handedness.Left, out MixedRealityPose joint);
                offset = joint.Position;
            }
            if (UnityEngine.Input.GetKeyUp(KeyCode.F9))
            {
                RecordPose(Handedness.Left);
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.F10))
            {
                HandJointUtils.TryGetJointPose(ReferenceJoint, Handedness.Right, out MixedRealityPose joint);
                offset = joint.Position;
            }
            if (UnityEngine.Input.GetKeyUp(KeyCode.F10))
            {
                RecordPose(Handedness.Right);
            }
        }

        private void RecordPose(Handedness handedness)
        {
            Vector3[] jointPositions = new Vector3[jointCount];

            for (int i = 0; i < jointCount; ++i)
            {
                GetJointPosition(jointPositions, (TrackedHandJoint)i, handedness);
            }

            SimulatedHandPose pose = new SimulatedHandPose();
            pose.ParseFromJointPositions(jointPositions, handedness, Quaternion.identity, offset);

            Debug.Log(pose.GenerateInitializerCode());
        }

        private void GetJointPosition(Vector3[] positions, TrackedHandJoint jointId, Handedness handedness)
        {
            HandJointUtils.TryGetJointPose(jointId, handedness, out MixedRealityPose joint);
            positions[(int)jointId] = joint.Position;
        }
    }

}