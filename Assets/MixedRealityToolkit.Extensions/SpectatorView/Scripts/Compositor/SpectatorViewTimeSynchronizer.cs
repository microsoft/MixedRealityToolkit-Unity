// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.Compositor
{
    /// <summary>
    /// Synchronizes time adjustments between the compositor and the HoloLens.
    /// </summary>
    public class SpectatorViewTimeSynchronizer
    {
        private float deltaCameraToUnity;
        private float deltaPoseToUnity;

        private int prevCamFrame = -1;
        private int numCamFrameSamples = 0;
        private int prevPoseIndex = -1;
        private int numPoseIndexSamples = 0;

        /// <summary>
        /// Records a sample to update the time synchronizer.
        /// </summary>
        /// <param name="cameraFrame">The index of the current video frame.</param>
        /// <param name="cameraTime">The timestamp of the current video frame.</param>
        /// <param name="poseIndex">The index of the current HoloLens pose.</param>
        /// <param name="poseTime">The timestamp of the current HoloLens pose.</param>
        public void Update(int cameraFrame, float cameraTime, int poseIndex, float poseTime)
        {
            if (cameraFrame != prevCamFrame)
            {
                prevCamFrame = cameraFrame;
                float newDelta = Time.time - cameraTime;
                numCamFrameSamples++;
                deltaCameraToUnity = Mathf.Lerp(deltaCameraToUnity, newDelta, 1.0f / Mathf.Min(numCamFrameSamples, 60));
            }
            if (poseIndex != prevPoseIndex)
            {
                prevPoseIndex = poseIndex;
                float newDelta = Time.time - poseTime;
                numPoseIndexSamples++;
                deltaPoseToUnity = Mathf.Lerp(deltaPoseToUnity, newDelta, 1.0f / Mathf.Min(numPoseIndexSamples, 60));
            }
        }

        /// <summary>
        /// Gets the timestamp of a HoloLens pose from a video camera time.
        /// </summary>
        /// <param name="cameraTime">The timestamp in the video camera timeline.</param>
        /// <returns>The timestamp of the HoloLens pose associated with the video, in the HoloLens timeline.</returns>
        public float GetPoseTimeFromCameraTime(float cameraTime)
        {
            return GetUnityTimeFromCameraTime(cameraTime) - deltaPoseToUnity;
        }

        /// <summary>
        /// Gets the timestamp of a HoloLens pose from a video camera time.
        /// </summary>
        /// <param name="cameraTime">The timestamp in the video camera timeline.</param>
        /// <returns>The timestamp of the video frame in Unity's timeline.</returns>
        public float GetUnityTimeFromCameraTime(float cameraTime)
        {
            return cameraTime + deltaCameraToUnity;
        }

        /// <summary>
        /// Resets the time synchronization for a new synchronization session.
        /// </summary>
        public void Reset()
        {
            prevCamFrame = -1;
            numCamFrameSamples = 0;
            prevPoseIndex = -1;
            numPoseIndexSamples = 0;
        }
    }
}