// Copyright (c) Microsoft Corporation. All rights reserved.
// Copyright(c) 2019 Takahiro Miyaura
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Utilities;
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
          SupportedPlatforms.Android 
        | SupportedPlatforms.IOS
#if UNITY_2019_OR_LATER
        | SupportedPlatorms.Standalone 
        | SupportedPlatforms.UniversalWindows
#endif
        ,
        "Unity AR Foundation Camera Settings",
        "XRCameraSettings/Profiles/DefaultXRCameraSettingsProfile.asset",
        "MixedRealityToolkit.Providers")]
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

#region IMixedRealityCameraSettings

        // todo:
        // android && iOs are always passthrough, others may not be
        //private DisplayType displayType = DisplayType.Opaque;

        /// <inheritdoc/>
        public DisplayType DisplayType => DisplayType.PassThrough;
        //{
        //    get 
        //    { 

        //    }

        //    private set 
        //    {
        //        displayType = value;
        //    }
        //}

        /// <inheritdoc/>
        public bool IsOpaque => (DisplayType == DisplayType.Opaque);

        /// <inheritdoc/>
        public void ApplyDisplaySettings()
        {
            MixedRealityCameraProfile cameraProfile = (Service as IMixedRealityCameraSystem)?.CameraProfile;
            if (cameraProfile == null) { return; } 

            if (IsOpaque)
            {
                CameraCache.Main.clearFlags = cameraProfile.CameraClearFlagsOpaqueDisplay;
                CameraCache.Main.nearClipPlane = cameraProfile.NearClipPlaneOpaqueDisplay;
                CameraCache.Main.farClipPlane = cameraProfile.FarClipPlaneOpaqueDisplay;
                CameraCache.Main.backgroundColor = cameraProfile.BackgroundColorOpaqueDisplay;
                QualitySettings.SetQualityLevel(cameraProfile.OpaqueQualityLevel, false);
            }
            else
            {
                CameraCache.Main.clearFlags = cameraProfile.CameraClearFlagsTransparentDisplay;
                CameraCache.Main.backgroundColor = cameraProfile.BackgroundColorTransparentDisplay;
                CameraCache.Main.nearClipPlane = cameraProfile.NearClipPlaneTransparentDisplay;
                CameraCache.Main.farClipPlane = cameraProfile.FarClipPlaneTransparentDisplay;
                QualitySettings.SetQualityLevel(cameraProfile.TransparentQualityLevel, false);
            }
        }

        #endregion IMixedRealityCameraSettings

        bool isInitialized = false;

        private GameObject arSessionObject = null;
        private bool preExistingArSessionObject = false;
        private ARSession arSession = null;

        private GameObject arSessionOriginObject = null;
        private bool preExistingArSessionOriginObject = false;
        private ARSessionOrigin arSessionOrigin = null;

        private ARCameraManager arCameraManager = null;
        private ARCameraBackground arCameraBackground = null;
        private ARInputManager arInputManager = null;
        private TrackedPoseDriver trackedPoseDriver = null;

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

        public override void Initialize()
        {
            base.Initialize();

            if (isInitialized) { return; }

            FindARFoundationComponents();

            if (arSessionObject == null)
            {
                arSessionObject = new GameObject("AR Session");
                arSessionObject.transform.parent = null;
            }
            arSession = arSessionObject.EnsureComponent<ARSession>();
            arInputManager = arSessionObject.EnsureComponent<ARInputManager>();

            if (arSessionOriginObject == null)
            {
                arSessionOriginObject = MixedRealityPlayspace.Transform.gameObject;
            }
            CameraCache.Main.transform.parent = arSessionOriginObject.transform;

            arSessionOrigin = arSessionOriginObject.EnsureComponent<ARSessionOrigin>();
            arSessionOrigin.camera = CameraCache.Main;

            GameObject cameraObject = arSessionOrigin.camera.gameObject;

            arCameraManager = cameraObject.EnsureComponent<ARCameraManager>();
            arCameraBackground = cameraObject.EnsureComponent<ARCameraBackground>();

            trackedPoseDriver = cameraObject.EnsureComponent<TrackedPoseDriver>();

            XRCameraSettingsProfile settingsProfile = (ConfigurationProfile as XRCameraSettingsProfile);
            TrackedPoseDriver.TrackedPose poseSource;
            TrackedPoseDriver.TrackingType trackingType;
            TrackedPoseDriver.UpdateType updateType;

            if (settingsProfile != null)
            {
                // Read settings to be applied to the camera.
                poseSource = settingsProfile.PoseSource;
                trackingType = settingsProfile.TrackingType;
                updateType = settingsProfile.UpdateType;
            }
            else
            {
                Debug.LogWarning("A profile was not specified for the XR Camera Settings provider.\nApplying Microsoft Mixed Reality Toolkit default options.");
                // Use default settings.
                poseSource = TrackedPoseDriver.TrackedPose.ColorCamera;
                trackingType = TrackedPoseDriver.TrackingType.RotationAndPosition;
                updateType = TrackedPoseDriver.UpdateType.UpdateAndBeforeRender;
            }

            trackedPoseDriver.SetPoseSource(
                TrackedPoseDriver.DeviceType.GenericXRDevice,
                poseSource);
            trackedPoseDriver.trackingType = trackingType;
            trackedPoseDriver.updateType = updateType;
            trackedPoseDriver.UseRelativeTransform = false;

            isInitialized = true;
        }

        public override void Destroy()
        {
            if (!isInitialized) { return; }

            if (!preExistingArSessionOriginObject &&
                (arSessionOriginObject != null))
            {
                if (Application.isEditor && !Application.isPlaying)
                {
                    Object.DestroyImmediate(trackedPoseDriver);
                    Object.DestroyImmediate(arCameraBackground);
                    Object.DestroyImmediate(arCameraManager);
                    Object.DestroyImmediate(arSessionOrigin);
                }
                else
                {
                    Object.Destroy(trackedPoseDriver);
                    Object.Destroy(arCameraBackground);
                    Object.Destroy(arCameraManager);
                    Object.Destroy(arSessionOrigin);
                }

                trackedPoseDriver = null;
                arCameraBackground = null;
                arCameraManager = null;
                arSessionOrigin = null;
            }

            if (!preExistingArSessionObject &&
                (arSessionObject != null))
            {
                if (Application.isEditor && !Application.isPlaying)
                {
                    Object.DestroyImmediate(arInputManager);
                    Object.DestroyImmediate(arSession);
                    Object.DestroyImmediate(arSessionObject);
                }
                else
                {
                    Object.Destroy(arInputManager);
                    Object.Destroy(arSession);
                    Object.Destroy(arSessionObject);
                }

                arInputManager = null;
                arSession = null;
                arSessionObject = null;
            }

            isInitialized = false;

            base.Destroy();
        }

        /// <summary>
        /// The profile used to configure the camera.
        /// </summary>
        public MixedRealityCameraProfile CameraProfile
        {
            get
            {
                return ConfigurationProfile as MixedRealityCameraProfile;
            }
        }
    }
}
