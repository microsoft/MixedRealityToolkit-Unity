// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.IO;
using System;
using System.Text;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.Compositor
{
    public class Calibration : MonoBehaviour
    {
        public bool IsLoaded
        {
            get
            {
                return CalibrationData != null;
            }
        }

        public ICalibrationData CalibrationData { get; set; }

        public void SetUnityCameraExtrinstics(Transform cameraGO)
        {
            CalibrationData.SetUnityCameraExtrinstics(cameraGO);
        }

        public void SetUnityCameraIntrinsics(Camera camera)
        {
            CalibrationData.SetUnityCameraIntrinsics(camera);
        }
    }
}
