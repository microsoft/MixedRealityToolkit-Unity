// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.Compositor
{
    public class SpectatorViewTimeSynchronizer
    {
        private float deltaCameraToUnity;
        private float deltaPoseToUnity;

        private int prevCamFrame = -1;
        private int numCamFrameSamples = 0;
        private int prevPoseIndex = -1;
        private int numPoseIndexSamples = 0;

        public void Update(int camFrame, float camTime, int poseIndex, float poseTime)
        {
            if (camFrame != prevCamFrame)
            {
                prevCamFrame = camFrame;
                float newDelta = Time.time - camTime;
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

        public float GetPoseTimeFromCameraTime(float cameraTime)
        {
            return GetUnityTimeFromCameraTime(cameraTime) - deltaPoseToUnity;
        }

        public float GetUnityTimeFromCameraTime(float cameraTime)
        {
            return cameraTime + deltaCameraToUnity;
        }

        public void Reset()
        {
            prevCamFrame = -1;
            numCamFrameSamples = 0;
            prevPoseIndex = -1;
            numPoseIndexSamples = 0;
        }
    }
}