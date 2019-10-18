// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using UnityEngine;
using UnityEngine.SpatialTracking;
using UnityEngine.XR.ARFoundation;

namespace Microsoft.MixedReality.Toolkit.CameraSystem
{
    /// <summary>
    /// todo
    /// </summary>
    [MixedRealityDataProvider(
        typeof(IMixedRealityCameraSystem),
        SupportedPlatforms.Android | SupportedPlatforms.IOS |
#if UNITY_2019_OR_LATER
        SupportedPlatorms.Standalone | SupportedPlatforms.UniversalWindows |
#endif
        SupportedPlatforms.WindowsEditor | SupportedPlatforms.MacEditor | SupportedPlatforms.LinuxEditor,
        "Unity AR Foundation Camera Settings")]
        // profile pending
        //"XRCameraSettings/Profiles/DefaultXRCameraSettingsProfile.asset",
        //"MixedRealityToolkit.Providers")]
    public class XRCameraSettings : BaseDataProvider, IMixedRealityCameraSettingsProvider

    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="registrar">The <see cref="IMixedRealityServiceRegistrar"/> instance that loaded the provider.</param>
        /// <param name="cameraSystem">The instance of the camera system which is managing this provider.</param>
        /// <param name="name">Friendly name of the provider.</param>
        /// <param name="priority">Provider priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The provider's configuration profile.</param>
        public XRCameraSettings(
            IMixedRealityServiceRegistrar registrar,
            IMixedRealityCameraSystem cameraSystem,
            string name = null,
            uint priority = DefaultPriority,
            BaseCameraSettingsProfile profile = null) : base(registrar, cameraSystem, name, priority, profile)
        { }

        //bool isInitialized = false;

        private GameObject arSessionObject = null;
        private bool preExistingArSessionObject = false;
        //private ARSession arSession = null;

        private GameObject arSessionOriginObject = null;
        private bool preExistingArSessionOriginObject = false;
        //private ARSessionOrigin arSessionOrigin = null;

        //private ARCameraManager arCameraManager = null;
        //private ARCameraBackground arCameraBackground = null;
        //private ARInputManager arInputManager = null;
        //private TrackedPoseDriver trackedPoseDriver = null;

        /// <summary>
        /// Examines the scene to determine if AR Foundation components are present.
        /// </summary>
        private void FindARFoundationComponents()
        {
            arSessionObject = GameObject.Find("AR Session");
            preExistingArSessionObject = (arSessionObject != null);
            arSessionOriginObject = GameObject.Find("AR Session Origin");
            preExistingArSessionOriginObject = (arSessionOriginObject != null);
        }

        // todo
    }
}
