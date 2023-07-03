// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Events;
#if WINDOWS_UWP
using Windows.Perception;
using Windows.Perception.People;
using Windows.Perception.Spatial;
using Windows.UI.Input.Spatial;
#endif

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// A helper class used to check eye calibration status. Only for UWP Platforms. 
    /// </summary>
    [AddComponentMenu("MRTK/Input/Eye Calibration Checker")]
    public class EyeCalibrationChecker : MonoBehaviour
    {
        #region Public Enums

        public enum EyeCalibrationStatus
        {
            /// <summary>
            /// The eye calibration status is not defined. 
            /// </summary>
            Undefined,

            /// <summary>
            /// The eye calibration status could not be retrieved because this is a non-UWP device.
            /// </summary>
            IsNonUWPDevice,

            /// <summary>
            /// The eye calibration status could not be retrieved becaause eyes are not being tracked.
            /// This occurs when SpatialPointerPose's Eyes property is null. 
            /// </summary>
            IsNotTracked,

            /// <summary>
            /// The eye calibration check has expired. Assume that eyes are not calibrated.
            /// </summary>
            IsExpired,

            /// <summary>
            /// The eye calibration status was retrieved and eyes are not calibrated. 
            /// </summary> 
            IsNotCalibrated,

            /// <summary>
            /// The eye calibration status was retrieved and eyes are calibrated. 
            /// </summary>
            IsCalibrated
        };

        #endregion Public Enums

        #region Serialized Fields

        /// <summary>
        /// For testing purposes, you can manually assign whether eyes are calibrated or not in editor. 
        /// </summary>
        [field: SerializeField, Tooltip("For testing purposes, you can manually assign whether eyes are calibrated or not in editor.")]
        public EyeCalibrationStatus EditorTestIsCalibrated = EyeCalibrationStatus.IsCalibrated;

        #endregion Serialized Fields

        #region Private Fields
        private EyeCalibrationStatus calibrationStatus;

        /// <summary>
        /// Tracks whether eyes are present and calibrated. 
        /// </summary>
        public EyeCalibrationStatus CalibratedStatus
        {
            get => calibrationStatus;
        }

        private EyeCalibrationStatus prevCalibrationStatus;
        #endregion Private Fields

        #region Events
        /// <summary>
        /// Event fired when IsCalibrated changes from false to true.
        /// </summary>
        public UnityEvent OnEyeCalibrationDetected = new UnityEvent();

        /// <summary>
        /// Event fired when IsCalibrated changes from true to false.
        /// </summary>
        public UnityEvent OnNoEyeCalibrationDetected = new UnityEvent();
        #endregion Events

        #region MonoBehaviour Functions
        private void Update()
        {
            if (Application.isEditor)
            {
                calibrationStatus = EditorTestIsCalibrated;
            }
            else
            {
                calibrationStatus = CheckCalibrationStatus();
            }

            if (prevCalibrationStatus != calibrationStatus)
            {
                if (calibrationStatus == EyeCalibrationStatus.IsCalibrated)
                {
                    OnEyeCalibrationDetected.Invoke();
                }
                else
                {
                    OnNoEyeCalibrationDetected.Invoke();
                }
                prevCalibrationStatus = calibrationStatus;
            }
        }
        #endregion MonoBehaviour Functions

        #region Private Functions

        private EyeCalibrationStatus CheckCalibrationStatus()
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
                        // If it's been longer than a second since the last perception snapshot, assume the information has expired.
                        if ((DateTimeOffset.Now - eyes.UpdateTimestamp.TargetTime.DateTime).TotalSeconds > 1)
                        {
                            return EyeCalibrationStatus.IsExpired;
                        }
                        if (eyes.IsCalibrationValid)
                        {
                            return EyeCalibrationStatus.IsCalibrated;
                        }
                        else
                        {
                            return EyeCalibrationStatus.IsNotCalibrated;
                        }
                    }
                }
            }
            return EyeCalibrationStatus.IsNotTracked;
#else
            return EyeCalibrationStatus.IsNonUWPDevice;
#endif // WINDOWS_UWP
        }

        #endregion Private Functions
    }
}
