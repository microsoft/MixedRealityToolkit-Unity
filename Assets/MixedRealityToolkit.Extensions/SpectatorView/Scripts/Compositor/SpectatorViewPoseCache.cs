using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.Compositor
{
    [Serializable]
    public class SpectatorViewPoseCache
    {
        [Serializable]
        public class PoseData
        {
            public Vector3 Position;
            public Quaternion Rotation;
            public float TimeStamp;
            public int Index;
        }

        const int MAX_NUM_POSES = 60;
        int lastPoseIndex;

        public List<PoseData> poses = new List<PoseData>(MAX_NUM_POSES + 1);

        public PoseData GetLatestPose() { return poses.Count > 0 ? poses[0] : null; }
        public int LastSelectedIndex { get; private set; }

        public bool AddPose(Vector3 position, Quaternion rotation, float timeStamp)
        {
            //Already have this pose
            if (poses.Count > 0 && poses[0].TimeStamp == timeStamp)
                return false;

            //Find index to insert at
            int i = 0;
            while (i < poses.Count && poses[i].TimeStamp > timeStamp)
                i++;
            poses.Insert(i, new PoseData() { Position = position, Rotation = rotation, TimeStamp = timeStamp, Index = lastPoseIndex++ });

            //Remove oldest
            if (poses.Count >= MAX_NUM_POSES)
                poses.RemoveAt(poses.Count - 1);
            return true;
        }

        public bool GetPose(out Vector3 position, out Quaternion rotation, float poseTime)
        {
            if (poses.Count == 0)
            {
                position = Vector3.zero;
                rotation = Quaternion.identity;
                return false;
            }

            //Find index for time
            LastSelectedIndex = 0;
            while (LastSelectedIndex < poses.Count)
            {
                if (poses[LastSelectedIndex].TimeStamp < poseTime)
                    break;
                LastSelectedIndex++;
            }


            if (LastSelectedIndex == 0)
            {
                position = poses[0].Position;
                rotation = poses[0].Rotation;
            }
            else if (LastSelectedIndex == poses.Count)
            {
                position = poses[poses.Count - 1].Position;
                rotation = poses[poses.Count - 1].Rotation;
            }
            else
            {
                //Lerp between 2 poses
                var next = poses[LastSelectedIndex];
                var prev = poses[LastSelectedIndex - 1];
                float lerpVal = Mathf.InverseLerp(next.TimeStamp, prev.TimeStamp, poseTime);
                position = Vector3.Lerp(next.Position, prev.Position, lerpVal);
                rotation = Quaternion.Slerp(next.Rotation, prev.Rotation, lerpVal);
            }

            return true;
        }

        public void Reset()
        {
            lastPoseIndex = 0;
            poses.Clear();
        }
    }
}