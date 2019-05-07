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
    /// Record joint positions of a hand and log them for use in simulated hands
    /// </summary>
    public class WindowsMixedRealityHandRecorder : MonoBehaviour
    {
        private static readonly int jointCount = Enum.GetNames(typeof(TrackedHandJoint)).Length;

        public TrackedHandJoint ReferenceJoint = TrackedHandJoint.IndexTip;

        private Vector3 offset = Vector3.zero;
        private Handedness recordingHand = Handedness.None;

        void Update()
        {
            // if (UnityEngine.Input.GetKeyDown(KeyCode.F9))
            // {
            //     RecordHandStart(Handedness.Left);
            // }
            // if (UnityEngine.Input.GetKeyUp(KeyCode.F9))
            // {
            //     RecordHandStop(Handedness.Left);
            // }

            // if (UnityEngine.Input.GetKeyDown(KeyCode.F10))
            // {
            //     RecordHandStart(Handedness.Right);
            // }
            // if (UnityEngine.Input.GetKeyUp(KeyCode.F10))
            // {
            //     RecordHandStop(Handedness.Right);
            // }
        }

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

            StoreRecordedHandPose(pose.ToJson());
            // StoreRecordedHandPose(pose.GenerateInitializerCode());
        }

        #if WINDOWS_UWP
        private static void StoreRecordedHandPose(string data)
        // private static async void StoreRecordedHandPose(string data)
        {
            string path = Path.Combine(Application.persistentDataPath, "ArticulatedHandPose.json");
            using (TextWriter writer = File.CreateText(path))
            {
                writer.Write(data);
            }
            // StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            // StorageFile storageFile = await storageFolder.CreateFileAsync("ArticulatedHandPose.json", CreationCollisionOption.ReplaceExisting);
            // await FileIO.WriteTextAsync(storageFile, data);
        }
        #else
        private static void StoreRecordedHandPose(string data)
        {
            Debug.Log(data);
        }
        #endif
    }

}