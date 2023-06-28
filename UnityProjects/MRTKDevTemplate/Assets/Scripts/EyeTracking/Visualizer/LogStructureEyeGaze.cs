// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples
{
    using Input;

    [AddComponentMenu("Scripts/MRTK/Examples/LogStructureEyeGaze")]
    public class LogStructureEyeGaze : LogStructure
    {
        [SerializeField]
        private FuzzyGazeInteractor gazeInteractor;

        public override string[] GetHeaderColumns()
        {
            return new string[]
            {
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
            Vector3? eyeHitPos = gazeInteractor.PreciseHitResult.raycastHit.point;

            object[] data = new object[]
            { 
                // Cam / Head tracking
                Camera.main.transform.position.x,
                Camera.main.transform.position.y,
                Camera.main.transform.position.z,
                0,
                0,
                0,

                // Smoothed eye gaze signal 
                gazeInteractor.rayOriginTransform.position.x,
                gazeInteractor.rayOriginTransform.position.y,
                gazeInteractor.rayOriginTransform.position.z,

                gazeInteractor.rayOriginTransform.forward.x,
                gazeInteractor.rayOriginTransform.forward.y,
                gazeInteractor.rayOriginTransform.forward.z,

                eyeHitPos.HasValue ? eyeHitPos.Value.x : float.NaN,
                eyeHitPos.HasValue ? eyeHitPos.Value.y : float.NaN,
                eyeHitPos.HasValue ? eyeHitPos.Value.z : float.NaN
            };

            return data;
        }
    }
}
