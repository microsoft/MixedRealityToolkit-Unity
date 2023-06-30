// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.Events;
using Microsoft.MixedReality.Toolkit.Input;
using TMPro;

namespace Microsoft.MixedReality.Toolkit.Examples
{
	/// <summary>
	/// Checks whether eyes are calibrated and prompts a notification to encourage the user to calibrate.
	/// </summary>
	[AddComponentMenu("MRTK/Examples/Eye Calibration Notifier")]
	public class EyeCalibrationWarning : MonoBehaviour
	{
        [Tooltip("The EyeCalibrationChecker used to notify the user about calibration status.")]
        [SerializeField]
        private EyeCalibrationChecker checker;

        [Tooltip("The TMP Text used to notify the user about calibration status.")]
        [SerializeField]
        private TMP_Text text;

        private void OnEnable()
		{
            if (checker == null)
            {
                checker = GetComponent<EyeCalibrationChecker>();
            }

            if (text == null)
            {
                text = GetComponent<TMP_Text>();
            }

            if (checker != null)
            {
                switch (checker.CalibratedStatus)
                {
                    case EyeCalibrationChecker.EyeCalibrationStatus.IsCalibrated:
                        NotifyYesEyeCalibration();
                        break;
                    default:
                        WarnNoEyeCalibration();
                        break;
                }
                checker.OnNoEyeCalibrationDetected.AddListener(WarnNoEyeCalibration);
                checker.OnEyeCalibrationDetected.AddListener(NotifyYesEyeCalibration);
            }
        }

		private void WarnNoEyeCalibration()
		{
            string warning = "Eye Calibration Status: Not Calibrated. ";
            if (checker != null)
            {
                switch (checker.CalibratedStatus)
                { 
                    case EyeCalibrationChecker.EyeCalibrationStatus.IsNotCalibrated:
                        warning += "Please calibrate your eyes.";
                        break;
                    case EyeCalibrationChecker.EyeCalibrationStatus.IsNonUWPDevice:
                        warning += "You are not on a UWP Device.";
                        break;
                    case EyeCalibrationChecker.EyeCalibrationStatus.IsNotTracked:
                        warning += "Please ensure this app is granted the appropriate permissions.";
                        break;
                    default:
                        break;
                }
            }
			text.text = warning;
            text.color = Color.red;
        }

        private void NotifyYesEyeCalibration()
        {
            text.text = "Eye Calibration Status: Calibrated.";
            text.color = Color.green;
        }
    }
}
