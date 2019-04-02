// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking.Logging
{
    public class LogStructure_EyeGaze : LogStructure
    {
        private IMixedRealityEyeGazeProvider EyeTrackingProvider => eyeTrackingProvider ?? (eyeTrackingProvider = MixedRealityToolkit.InputSystem?.EyeGazeProvider);
        private IMixedRealityEyeGazeProvider eyeTrackingProvider = null;

        public override string[] GetHeaderColumns()
        {
            return new string[] {
                // UserId
                "UserId",
                // SessionType
                "SessionType",
                // Timestamp
                "dt in ms",
                // Cam / Head tracking
                "HeadOrigin.x",
                "HeadOrigin.y",
                "HeadOrigin.z",
                "HeadDir.x",
                "HeadDir.y",
                "HeadDir.z",
                // Smoothed eye gaze tracking 
                "EyeOrigin.x",
                "EyeOrigin.y",
                "EyeOrigin.z",
                "EyeDir.x",
                "EyeDir.y",
                "EyeDir.z",
                "EyeHitPos.x",
                "EyeHitPos.y",
                "EyeHitPos.z",
            };
        }

        public override object[] GetData(string inputType, string inputStatus, EyeTrackingTarget intTarget)
        {
            // Let's prepare all the data we wanna log

            // Eye gaze hit position
            Vector3? eyeHitPos = null;
            if (EyeTrackingProvider?.GazeTarget != null && EyeTrackingProvider.IsEyeGazeValid)
                eyeHitPos = EyeTrackingProvider.HitPosition;


            object[] data = new object[]
            { 
                //-------------------------------
                // Cam / Head tracking
                CameraCache.Main.transform.position.x,
                CameraCache.Main.transform.position.y,
                CameraCache.Main.transform.position.z,
                0,
                0,
                0,

                // Smoothed eye gaze signal 
                EyeTrackingProvider.IsEyeGazeValid ? EyeTrackingProvider.GazeOrigin.x : 0,
                EyeTrackingProvider.IsEyeGazeValid ? EyeTrackingProvider.GazeOrigin.y : 0,
                EyeTrackingProvider.IsEyeGazeValid ? EyeTrackingProvider.GazeOrigin.z : 0,

                EyeTrackingProvider.IsEyeGazeValid ? EyeTrackingProvider.GazeDirection.x : 0,
                EyeTrackingProvider.IsEyeGazeValid ? EyeTrackingProvider.GazeDirection.y : 0,
                EyeTrackingProvider.IsEyeGazeValid ? EyeTrackingProvider.GazeDirection.z : 0,

                (eyeHitPos != null) ? eyeHitPos.Value.x : float.NaN,
                (eyeHitPos != null) ? eyeHitPos.Value.y : float.NaN,
                (eyeHitPos != null) ? eyeHitPos.Value.z : float.NaN,
        };

            return data;
        }
    }
}