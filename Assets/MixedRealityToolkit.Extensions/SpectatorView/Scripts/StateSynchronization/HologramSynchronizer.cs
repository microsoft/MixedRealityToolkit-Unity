// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class HologramSynchronizer
    {
        private class FramePackages
        {
            public float TimeStamp;
            public List<byte[]> Data = new List<byte[]>();
        }

        private float lastDisplayedPackageTimeStamp;
        private float lastLerpTimeStamp;
        private float latestTimeStamp;
        private float localToPackageTimeDelta;
        private int numSamples;
        private SocketEndpoint currentConnection;
        private List<FramePackages> frames = new List<FramePackages>();

        public void Reset(SocketEndpoint newConnection)
        {
            currentConnection = newConnection;
            frames.Clear();
            lastDisplayedPackageTimeStamp = 0.0f;
            lastLerpTimeStamp = 0.0f;
            latestTimeStamp = 0.0f;
            localToPackageTimeDelta = 0.0f;
            numSamples = 0;
        }

        public void RegisterCameraUpdate(float timeStamp)
        {
            UpdateFrameTime(timeStamp);
            //Store empty frames to perform correct lerps
            GetOrInsertFramePackages(timeStamp);
        }

        public void RegisterFrameData(byte[] data, float timeStamp)
        {
            UpdateFrameTime(timeStamp);
            GetOrInsertFramePackages(timeStamp).Data.Add(data);
        }

        public void UpdateHolograms()
        {
            if (frames.Count == 0)
            {
                return;
            }

            float displayTime = (TimeSynchronizer.IsInitialized ? TimeSynchronizer.Instance.GetHologramTime() : Time.time) + localToPackageTimeDelta;

            // Perform all full updates, but make sure the last frame is not consumed, as it's possible
            // that network updates are still coming in for that frame, and consuming that frame
            // before the complete packet is available can result in rendering artifacts for the frame.
            while (frames.Count > 1 && frames[0].TimeStamp <= displayTime)
            {
                FramePackages package = frames[0];
                foreach (var d in package.Data)
                {
                    using (MemoryStream stream = new MemoryStream(d))
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        SynchronizedSceneManager.Instance.ReceiveMessage(currentConnection, reader);
                    }
                }
                lastDisplayedPackageTimeStamp = frames[0].TimeStamp;
                lastLerpTimeStamp = lastDisplayedPackageTimeStamp;
                frames.RemoveAt(0);
            }

            // Perform lerps to next update, but make sure the last frame is not consumed, as it's possible
            // that network updates are still coming in for that frame, and consuming that frame
            // before the complete packet is available can result in rendering artifacts for the frame.
            if (frames.Count > 1)
            {
                float lerpVal = Mathf.InverseLerp(lastLerpTimeStamp, frames[0].TimeStamp, displayTime);
                if (lerpVal > 0.0f)
                {
                    FramePackages package = frames[0];
                    foreach (var d in package.Data)
                    {
                        using (MemoryStream stream = new MemoryStream(d))
                        using (BinaryReader reader = new BinaryReader(stream))
                        {
                            SynchronizedSceneManager.Instance.LerpReceiveMessage(reader, lerpVal);
                        }
                    }
                    lastLerpTimeStamp = displayTime;
                }
            }
        }

        private FramePackages GetOrInsertFramePackages(float timeStamp)
        {
            FramePackages package = frames.Find((f) => f.TimeStamp == timeStamp);
            if (package == null)
            {
                package = new FramePackages() { TimeStamp = timeStamp };
                int i = 0;
                //Insert sort
                while (i < frames.Count && frames[i].TimeStamp < timeStamp)
                {
                    i++;
                }

                frames.Insert(i, package);
            }
            return package;
        }

        private void UpdateFrameTime(float timeStamp)
        {
            //Keep track of when we got the latest timestamp
            if (timeStamp > latestTimeStamp)
            {
                latestTimeStamp = timeStamp;
                localToPackageTimeDelta = Mathf.Lerp(localToPackageTimeDelta, latestTimeStamp - Time.time, 1.0f / numSamples);
                numSamples = Mathf.Min(numSamples + 1, 100);
            }
        }
    }
}
