// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.Compositor
{
    /// <summary>
    /// Caches poses with timestamps from a HoloLens and allows computing a synthesized pose from any timestamp.
    /// </summary>
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

        private const int MaximumPoseCount = 60;
        private int lastPoseIndex;

        public List<PoseData> poses = new List<PoseData>(MaximumPoseCount + 1);

        /// <summary>
        /// Gets the latest pose added to the cache.
        /// </summary>
        /// <returns>The most-recent pose available.</returns>
        public PoseData GetLatestPose() { return poses.Count > 0 ? poses[0] : null; }

        /// <summary>
        /// Adds a pose to the cache.
        /// </summary>
        /// <param name="position">The position for the pose.</param>
        /// <param name="rotation">The rotation for the pose.</param>
        /// <param name="timeStamp">The timestamp in seconds of the pose.</param>
        /// <returns>True if the pose was insert, false if the pose was already in the cache.</returns>
        public bool AddPose(Vector3 position, Quaternion rotation, float timeStamp)
        {
            //Already have this pose
            if (poses.Count > 0 && poses[0].TimeStamp == timeStamp)
            {
                return false;
            }

            //Find index to insert at
            int i = 0;
            while (i < poses.Count && poses[i].TimeStamp > timeStamp)
            {
                i++;
            }

            poses.Insert(i, new PoseData() { Position = position, Rotation = rotation, TimeStamp = timeStamp, Index = lastPoseIndex++ });

            //Remove oldest
            if (poses.Count >= MaximumPoseCount)
            {
                poses.RemoveAt(poses.Count - 1);
            }

            return true;
        }

        /// <summary>
        /// Gets a synthesized pose from a timestamp. Positions are synthensized by interpolating
        /// the recorded poses nearest to the pose time.
        /// </summary>
        /// <param name="poseTime">The timestamp to synthesize a pose for.</param>
        /// <param name="position">The position synthesized for the pose.</param>
        /// <param name="rotation">The rotation synthesized for the pose.</param>
        /// <returns></returns>
        public bool GetPose(float poseTime, out Vector3 position, out Quaternion rotation)
        {
            if (poses.Count == 0)
            {
                position = Vector3.zero;
                rotation = Quaternion.identity;
                return false;
            }

            //Find index for time
            int selectedIndex = 0;
            while (selectedIndex < poses.Count)
            {
                if (poses[selectedIndex].TimeStamp < poseTime)
                {
                    break;
                }
                selectedIndex++;
            }


            if (selectedIndex == 0)
            {
                position = poses[0].Position;
                rotation = poses[0].Rotation;
            }
            else if (selectedIndex == poses.Count)
            {
                position = poses[poses.Count - 1].Position;
                rotation = poses[poses.Count - 1].Rotation;
            }
            else
            {
                //Lerp between 2 poses
                var next = poses[selectedIndex];
                var prev = poses[selectedIndex - 1];
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