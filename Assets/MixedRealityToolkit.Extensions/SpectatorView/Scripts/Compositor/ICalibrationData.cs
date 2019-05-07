// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.Compositor
{
    public interface ICalibrationData
    {
        void SetUnityCameraExtrinstics(Transform cameraGO);
        void SetUnityCameraIntrinsics(Camera camera);
    }
}