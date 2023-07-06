// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Core.Tests;
using Microsoft.MixedReality.Toolkit.Input.Tests;
using Microsoft.MixedReality.Toolkit.Input;
using NUnit.Framework;
using System.Collections;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.UX.Runtime.Tests
{
    /// <summary>
    /// Tests for the EyeCalibrationChecker. 
    /// </summary>
    public class EyeCalibrationCheckerTests : BaseRuntimeInputTests
    {
        private bool isCalibrated;
        private EyeCalibrationStatus calibrationStatus;

        [UnityTest]
        public IEnumerator TestEyeCalibrationEvents()
        {
            // Create an EyeCalibrationChecker and add event listeners
            GameObject testButton = new GameObject("EyeCalibrationChecker");
            EyeCalibrationChecker checker = testButton.AddComponent<EyeCalibrationChecker>();
            checker.Calibrated.AddListener(YesEyeCalibration);
            checker.NotCalibrated.AddListener(NoEyeCalibration);
            checker.CalibratedStatusChanged.AddListener(CalibrationEvent);
            yield return null;

            // Test whether the events fire when the status is changed
            isCalibrated = true;
            checker.EditorTestIsCalibrated = EyeCalibrationStatus.Calibrated;
            yield return null;
            checker.EditorTestIsCalibrated = EyeCalibrationStatus.NotCalibrated;
            yield return null;
            Assert.IsFalse(isCalibrated, "NotCalibrated event was not fired.");
            Assert.AreEqual(calibrationStatus, EyeCalibrationStatus.NotCalibrated, "CalibratedStatusChanged event was not fired.");
            yield return null;
            checker.EditorTestIsCalibrated = EyeCalibrationStatus.Calibrated;
            yield return null;
            Assert.IsTrue(isCalibrated, "Calibrated event was not fired.");
            Assert.AreEqual(calibrationStatus, EyeCalibrationStatus.Calibrated, "CalibratedStatusChanged event was not fired.");
            yield return null;

            checker.Calibrated.RemoveListener(NoEyeCalibration);
            checker.NotCalibrated.RemoveListener(YesEyeCalibration);
            checker.CalibratedStatusChanged.RemoveListener(CalibrationEvent);
        }

        private void CalibrationEvent(EyeCalibrationStatusEventArgs args)
        {
            calibrationStatus = args.CalibratedStatus;
        }

        private void YesEyeCalibration()
        {
            isCalibrated = true;
        }

        private void NoEyeCalibration()
        {
            isCalibrated = false;
        }
    }
}
