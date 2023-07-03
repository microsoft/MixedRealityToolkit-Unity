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

        [UnityTest]
        public IEnumerator TestEyeCalibrationEvents()
        {
            // Create an EyeCalibrationChecker and add event listeners
            GameObject testButton = new GameObject("EyeCalibrationChecker");
            EyeCalibrationChecker checker = testButton.AddComponent<EyeCalibrationChecker>();
            checker.Calibrated.AddListener(NoEyeCalibration);
            checker.NotCalibrated.AddListener(YesEyeCalibration);
            yield return null;

            // Test whether the events fire when the status is changed
            isCalibrated = true;
            checker.EditorTestIsCalibrated = EyeCalibrationStatus.NotCalibrated;
            yield return null;
            Assert.IsFalse(isCalibrated, "OnNoEyeCalibrationDetected event was not fired.");
            yield return null;
            checker.EditorTestIsCalibrated = EyeCalibrationStatus.Calibrated;
            yield return null;
            Assert.IsTrue(isCalibrated, "OnEyeCalibrationDetected event was not fired.");
            yield return null;
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
