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
    /// Camera settings provider for use with the Unity AR Foundation system.
    /// </summary>
    [MixedRealityDataProvider(
        typeof(IMixedRealityCameraSystem),
        SupportedPlatforms.Android | SupportedPlatforms.IOS,
        "Unity AR Foundation Camera Settings",
        "UnityARCameraSettings/Profiles/DefaultUnityARCameraSettingsProfile.asset",
        "MixedRealityToolkit.Providers")]
    public class UnityARCameraSettings : BaseDataProvider, IMixedRealityCameraSettingsProvider
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="registrar">The <see cref="IMixedRealityServiceRegistrar"/> instance that loaded the provider.</param>
        /// <param name="cameraSystem">The instance of the camera system which is managing this provider.</param>
        /// <param name="name">Friendly name of the provider.</param>
        /// <param name="priority">Provider priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The provider's configuration profile.</param>
        public UnityARCameraSettings(
            IMixedRealityServiceRegistrar registrar,
            IMixedRealityCameraSystem cameraSystem,
            string name = null,
            uint priority = DefaultPriority,
            BaseCameraSettingsProfile profile = null) : base(registrar, cameraSystem, name, priority, profile)
        { }

        #region IMixedRealityCameraSettings

        /// <inheritdoc/>
        public bool IsOpaque => false;

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

        /// The profile used to configure the camera.
        /// </summary>
        public UnityARCameraSettingsProfile SettingsProfile
        {
            get
            {
                return ConfigurationProfile as UnityARCameraSettingsProfile;
            }
        }

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

        public override async void Initialize()
        {
            base.Initialize();

            ARSessionState sessionState = (ARSessionState)(await ARSession.CheckAvailability());
            if (ARSessionState.Ready > sessionState)
            {
                Debug.LogError("Unable to initialize the Unity AR Camera Settings provider. Device support for AR Foundation was not detected.");
                isInitialized = true;
            }
        }

        public override void Enable()
        {
            base.Enable();
            
            if (!isInitialized)
            {
                InitializeARFoundation();
            }
        }

        public override void Destroy()
        {
            UninitializeARFoundation();

            base.Destroy();
        }

        /// <summary>
        /// Initialize AR Foundation components.
        /// </summary>
        private void InitializeARFoundation()
        {
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

            TrackedPoseDriver.TrackedPose poseSource;
            TrackedPoseDriver.TrackingType trackingType;
            TrackedPoseDriver.UpdateType updateType;

            if (SettingsProfile != null)
            {
                // Read settings to be applied to the camera.
                poseSource = SettingsProfile.PoseSource;
                trackingType = SettingsProfile.TrackingType;
                updateType = SettingsProfile.UpdateType;
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

        /// <summary>
        /// Uninitialize and clean up AR Foundation components.
        /// </summary>
        private void UninitializeARFoundation()
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
        }
    }
}
