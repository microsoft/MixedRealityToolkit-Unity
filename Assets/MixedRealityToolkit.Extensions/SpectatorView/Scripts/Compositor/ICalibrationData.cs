// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.Compositor
{
    /// <summary>
    /// Provides an abstraction over setting up a holographic camera from a stereo-calibrated
    /// camera rig.
    /// </summary>
    public interface ICalibrationData
    {
        /// <summary>
        /// Sets up the extrinsic parameters of the holographic camera such that the
        /// holographic camera's position and rotation are correctly offset
        /// from the HoloLens providing poses for the camera rig.
        /// </summary>
        /// <param name="cameraGO">The transform that contains the holographic camera.</param>
        void SetUnityCameraExtrinstics(Transform cameraGO);

        /// <summary>
        /// Sets up the intrinsic parameters (such as a projection matrix or field of view) of the holographic
        /// camera to match the video camera.
        /// </summary>
        /// <param name="camera">The holographic camera to set the intrinsis of.</param>
        void SetUnityCameraIntrinsics(Camera camera);
    }
}