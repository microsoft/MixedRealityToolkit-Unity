// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking
{
    /// <summary>
    /// Checks whether the user is calibrated and prompts a notification to encourage the user to calibrate.
    /// </summary>
    public class EyeCalibrationChecker : MonoBehaviour
    {
        [Tooltip("For testing purposes, you can manually assign whether the user is eye calibrated or not.")]
        [SerializeField]
        private bool editorTestUserIsCalibrated = true;

        public UnityEvent OnEyeCalibrationDetected;
        public UnityEvent OnNoEyeCalibrationDetected;

        private bool? prevCalibrationStatus = null;
        private IMixedRealityInputSystem inputSystem = null;

        /// <summary>
        /// The active instance of the input system.
        /// </summary>
        private IMixedRealityInputSystem InputSystem
        {
            get
            {
                if (inputSystem == null)
                {
                    MixedRealityServiceRegistry.TryGetService<IMixedRealityInputSystem>(out inputSystem);
                }
                return inputSystem;
            }
        }

        // Update is called once per frame
        void Update()
        {
            bool? calibrationStatus;

            if (Application.isEditor)
            {
                calibrationStatus = editorTestUserIsCalibrated;
            }
            else
            {
                calibrationStatus = InputSystem?.EyeGazeProvider?.IsEyeCalibrationValid;
            }

            if (calibrationStatus != null)
            {
                if (prevCalibrationStatus != calibrationStatus)
                {
                    if (calibrationStatus == false)
                    {
                        OnNoEyeCalibrationDetected.Invoke();
                    }
                    else
                    {
                        OnEyeCalibrationDetected.Invoke();
                    }
                    prevCalibrationStatus = calibrationStatus;
                }
            }
        }
    }
}
