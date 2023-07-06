// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;

namespace Microsoft.MixedReality.Toolkit.Examples
{
    /// <summary>
    /// A CSV file logging structure for eye gaze data.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Examples/LogStructureEyeGaze")]
    public class LogStructureEyeGaze : LogStructure
    {
        [Tooltip("Reference to the FuzzyGazeInteractor in the scene")]
        [SerializeField]
        private FuzzyGazeInteractor gazeInteractor;

        /// <summary>
        /// The headers of the eye gaze file structure.
        /// </summary>
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

        /// <summary>
        /// Returns the eye gaze data captured during an update frame.
        /// </summary>
        public override object[] GetData()
        {
            // Let's prepare all the data we wanna log

            // Eye gaze hit position
            Vector3 eyeHitPos = gazeInteractor.PreciseHitResult.raycastHit.point;

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

                eyeHitPos.x,
                eyeHitPos.y,
                eyeHitPos.z
            };

            return data;
        }
    }
}
