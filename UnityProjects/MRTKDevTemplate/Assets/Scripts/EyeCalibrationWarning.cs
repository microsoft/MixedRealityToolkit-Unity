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

        /// <summary>
        /// The EyeCalibrationChecker used to notify the user about calibration status.
        /// </summary>
        public EyeCalibrationChecker Checker
        {
            get => checker;
            set => checker = value;
        }

        [Tooltip("The TMP Text used to notify the user about calibration status.")]
        [SerializeField]
        private TMP_Text text;

        /// <summary>
        /// The TMP Text used to notify the user about calibration status.
        /// </summary>
        public TMP_Text Text
        {
            get => text;
            set => text = value;
        }

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
                UpdateMessage(checker.CalibratedStatus);
                checker.CalibratedStatusChanged.AddListener(UpdateMessageFromEvent);
            }
        }

        private void UpdateMessageFromEvent(EyeCalibrationStatusEventArgs args)
        {
            UpdateMessage(args.CalibratedStatus);
        }

        private void UpdateMessage(EyeCalibrationStatus status)
        {
            string warning = "Eye Calibration Status: Not Calibrated. ";
            switch (status)
            {
                case EyeCalibrationStatus.Unsupported:
                    text.text = "";
                    return;
                case EyeCalibrationStatus.Calibrated:
                    text.text = "Eye Calibration Status: Calibrated.";
                    text.color = Color.green;
                    return;
                case EyeCalibrationStatus.NotCalibrated:
                    warning += "Please calibrate eye tracking.";
                    break;
                case EyeCalibrationStatus.NotTracked:
                    warning += "Please ensure this app is granted the appropriate permissions.";
                    break;
                default:
                    break;
            }
            text.text = warning;
            text.color = Color.red;
        }
    }
}
