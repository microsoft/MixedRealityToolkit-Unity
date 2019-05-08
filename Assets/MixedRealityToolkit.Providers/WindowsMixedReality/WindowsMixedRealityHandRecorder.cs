// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.WindowsMixedReality.Input;
using System;
using System.Collections.Generic;
using UnityEngine;

#if WINDOWS_UWP
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
#endif

namespace Microsoft.MixedReality.Toolkit.Input
{

    /// <summary>
    /// Record joint positions of a hand and log them for use in simulated hands.
    /// </summary>
    public class WindowsMixedRealityHandRecorder : MonoBehaviour
    {
        private static readonly int jointCount = Enum.GetNames(typeof(TrackedHandJoint)).Length;

        /// <summary>
        /// The joint positioned at the origin at the start of the recording.
        /// </summary>
        /// <remarks>
        /// If the reference joint moves between start and stop of recording then final position is used as an offset.
        /// Example: A "poke" gesture can be simulated by moving the index finger forward between start and stop,
        /// giving an offset that creates a poking motion when interpolated.
        /// </remarks>
        public TrackedHandJoint ReferenceJoint { get; set; } = TrackedHandJoint.IndexTip;

        /// <summary>
        /// Default output filename for saving the recorded pose.
        /// </summary>
        public string OutputFileName { get; } = "ArticulatedHandPose";

        private Vector3 offset = Vector3.zero;
        private Handedness recordingHand = Handedness.None;

        public void RecordLeftHandStart()
        {
            RecordHandStart(Handedness.Left);
        }

        public void RecordRightHandStart()
        {
            RecordHandStart(Handedness.Right);
        }

        private void RecordHandStart(Handedness handedness)
        {
            HandJointUtils.TryGetJointPose<WindowsMixedRealityArticulatedHand>(ReferenceJoint, handedness, out MixedRealityPose joint);
            offset = joint.Position;
            recordingHand = handedness;
        }

        public void RecordHandStop()
        {
            MixedRealityPose[] jointPoses = new MixedRealityPose[jointCount];
            for (int i = 0; i < jointCount; ++i)
            {
                HandJointUtils.TryGetJointPose<WindowsMixedRealityArticulatedHand>((TrackedHandJoint)i, recordingHand, out jointPoses[i]);
            }

            ArticulatedHandPose pose = new ArticulatedHandPose();
            pose.ParseFromJointPoses(jointPoses, recordingHand, Quaternion.identity, offset);

            recordingHand = Handedness.None;

            var filename = String.Format("{0}-{1}.json", OutputFileName, DateTime.UtcNow.ToString("yyyyMMdd-HHmmss"));
            StoreRecordedHandPose(pose.ToJson(), filename);
        }

        #if WINDOWS_UWP
        private static void StoreRecordedHandPose(string data, string filename)
        {
            string path = Path.Combine(Application.persistentDataPath, filename);
            using (TextWriter writer = File.CreateText(path))
            {
                writer.Write(data);
            }
        }
        #else
        private static void StoreRecordedHandPose(string data, string filename)
        {
            Debug.Log(data);
        }
        #endif
    }

}