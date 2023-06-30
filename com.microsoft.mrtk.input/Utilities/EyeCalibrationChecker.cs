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
            // The eye calibration status is not defined. 
            Undefined,

            // The eye calibration status could not be retrieved because this is a non-UWP device.
            IsNonUWPDevice,

            // The eye calibration status could not be retrieved becaause eyes are not being tracked.
            // This occurs when SpatialPointerPose's Eyes property is null. 
            IsNotTracked,

            // The eye calibration check has expired. Assume that eyes are not calibrated.
            IsExpired,

            // The eye calibration status was retrieved and eyes are not calibrated. 
            IsNotCalibrated,

            // The eye calibration status was retrieved and eyes are calibrated. 
            IsCalibrated
        };

        #endregion Public Enums

        #region Serialized Fields

        [Tooltip("For testing purposes, you can manually assign whether eyes are calibrated or not.")]
        [SerializeField]
        private EyeCalibrationStatus editorTestUserIsCalibrated = EyeCalibrationStatus.IsCalibrated;

        #endregion Serialized Fields

        #region Private Fields
        private EyeCalibrationStatus calibrationStatus;

        /// <summary>
        /// Checks whether eyes are present and calibrated. 
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
        public UnityEvent OnEyeCalibrationDetected;

        /// <summary>
        /// Event fired when IsCalibrated changes from true to false.
        /// </summary>
        public UnityEvent OnNoEyeCalibrationDetected;
        #endregion Events

        #region MonoBehaviour Functions
        private void Update()
        {
            if (Application.isEditor)
            {
                calibrationStatus = editorTestUserIsCalibrated;
            }
            else
            {
                calibrationStatus = CheckCalibrationStatus();
            }

            if (prevCalibrationStatus != calibrationStatus)
            {
                Debug.Log("New calibration just dropped");
                if (calibrationStatus == EyeCalibrationStatus.IsCalibrated)
                {

                    Debug.Log("Calibrated.");
                    OnEyeCalibrationDetected.Invoke();
                }
                else
                {
                    Debug.Log("Not calibrated.");
                    OnNoEyeCalibrationDetected.Invoke();
                }
                prevCalibrationStatus = calibrationStatus;
            }
        }
        #endregion MonoBehaviour Functions

        #region Private Functions

        private EyeCalibrationStatus CheckCalibrationStatus()
        {
            Debug.Log(System.DateTime.Now.ToString());
#if WINDOWS_UWP
            Debug.Log("Yes UWP.");
            if (MixedReality.OpenXR.PerceptionInterop.GetSceneCoordinateSystem(Pose.identity) is SpatialCoordinateSystem worldOrigin)
            {
                Debug.Log("yes spatial coordinate system.");
                SpatialPointerPose pointerPose = SpatialPointerPose.TryGetAtTimestamp(worldOrigin, PerceptionTimestampHelper.FromHistoricalTargetTime(DateTimeOffset.Now));
                if (pointerPose != null)
                {
                    Debug.Log("yes pointer pose");
                    EyesPose eyes = pointerPose.Eyes;
                    if (eyes != null)
                    {
                        Debug.Log("yes eyes");
                        Debug.Log(eyes.IsCalibrationValid);
                        Debug.Log(eyes.UpdateTimestamp.TargetTime.DateTime);
                        if (eyes.IsCalibrationValid)
                        {
                            return EyeCalibrationStatus.IsCalibrated;
                        }
                        else
                        {
                            return EyeCalibrationStatus.IsNotCalibrated;
                        }
                    }
                    else
                    {
                        return EyeCalibrationStatus.IsNotTracked;
                    }
                }
            }
#endif // WINDOWS_UWP

            return EyeCalibrationStatus.IsNonUWPDevice;
        }

        #endregion Private Functions
    }
}
