// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.Events;

#if WINDOWS_UWP
using System;
using Windows.Perception;
using Windows.Perception.People;
using Windows.Perception.Spatial;
using Windows.UI.Input.Spatial;
#endif

namespace Microsoft.MixedReality.Toolkit.Examples
{
    /// <summary>
    /// Checks whether the user is calibrated and prompts a notification to encourage the user to calibrate.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Examples/EyeCalibrationChecker")]
    public class EyeCalibrationChecker : MonoBehaviour
    {
        [Tooltip("For testing purposes, you can manually assign whether the user is eye calibrated or not.")]
        [SerializeField]
        private bool editorTestUserIsCalibrated = true;

        public UnityEvent OnEyeCalibrationDetected;
        public UnityEvent OnNoEyeCalibrationDetected;

        private bool? previousCalibrationStatus;

        private void Update()
        {
            bool calibrationStatus = CheckCalibrationStatus();
#if UNITY_EDITOR
            calibrationStatus = editorTestUserIsCalibrated;
#endif

            if (previousCalibrationStatus.HasValue && previousCalibrationStatus.Value != calibrationStatus)
            {
                if (!calibrationStatus)
                {
                    OnNoEyeCalibrationDetected.Invoke();
                }
                else
                {
                    OnEyeCalibrationDetected.Invoke();
                }
                
            }

            previousCalibrationStatus = calibrationStatus;
        }

        private bool CheckCalibrationStatus()
        {
#if WINDOWS_UWP
            if (MixedReality.OpenXR.PerceptionInterop.GetSceneCoordinateSystem(Pose.identity) is SpatialCoordinateSystem worldOrigin)
            {
                SpatialPointerPose pointerPose = SpatialPointerPose.TryGetAtTimestamp(worldOrigin, PerceptionTimestampHelper.FromHistoricalTargetTime(DateTimeOffset.Now));
                if (pointerPose != null)
                {
                    EyesPose eyes = pointerPose.Eyes;
                    if (eyes != null)
                    {
                        return eyes.IsCalibrationValid;
                    }
                }
            }
#endif

            return true;
        }
    }
}
