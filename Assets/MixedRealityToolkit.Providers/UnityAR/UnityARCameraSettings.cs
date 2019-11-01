// Copyright (c) Microsoft Corporation. All rights reserved.
// Copyright(c) 2019 Takahiro Miyaura
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.SpatialTracking;

#if !(WINDOWS_UWP && !ENABLE_IL2CPP)
using UnityEngine.XR.ARFoundation;
#endif // !(WINDOWS_UWP && !ENABLE_IL2CPP)

namespace Microsoft.MixedReality.Toolkit.CameraSystem
{
    /// <summary>
    /// Camera settings provider for use with the Unity AR Foundation system.
    /// </summary>
    [MixedRealityDataProvider(
        typeof(IMixedRealityCameraSystem),
        SupportedPlatforms.Android | SupportedPlatforms.IOS,
        "Unity AR Foundation Camera Settings",
        "UnityAR/Profiles/DefaultUnityARCameraSettingsProfile.asset",
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

        /// <summary>
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

#if !(WINDOWS_UWP && !ENABLE_IL2CPP)
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
#endif //!(WINDOWS_UWP && !ENABLE_IL2CPP)

        /// <inheritdoc />
        public override async void Initialize()
        {
            base.Initialize();

#if !(WINDOWS_UWP && !ENABLE_IL2CPP)
            ARSessionState sessionState = (ARSessionState)(await ARSession.CheckAvailability());
            if (ARSessionState.Ready > sessionState)
            {
                Debug.LogError("Unable to initialize the Unity AR Camera Settings provider. Device support for AR Foundation was not detected.");
                isInitialized = true;
            }
#endif //!(WINDOWS_UWP && !ENABLE_IL2CPP)
        }

        /// <inheritdoc />
        public override void Enable()
        {
            base.Enable();
            
            if (!isInitialized)
            {
                InitializeARFoundation();
            }
        }

        /// <inheritdoc />
        public override void Destroy()
        {
            UninitializeARFoundation();

            base.Destroy();
        }

        /// <summary>
        /// Initialize AR Foundation components.
        /// </summary>
        /// <remarks>
        /// This method ensures AR Foundation required components (ex: AR Session, Tracked Pose Driver, etc) are
        /// exist or are added to the appropriate scene objects. These components are used by AR Foundation to
        /// communicate with the underlying AR platform (ex: AR Core), track the device and perform other necessary tasks.
        /// </remarks>
        private void InitializeARFoundation()
        {
            if (isInitialized) { return; }

#if !(WINDOWS_UWP && !ENABLE_IL2CPP)
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
#endif //!(WINDOWS_UWP && !ENABLE_IL2CPP)

            isInitialized = true;
        }

        /// <summary>
        /// Uninitialize and clean up AR Foundation components.
        /// </summary>
        private void UninitializeARFoundation()
        {
            if (!isInitialized) { return; }

#if !(WINDOWS_UWP && !ENABLE_IL2CPP)
            if (!preExistingArSessionOriginObject &&
                (arSessionOriginObject != null))
            {
                UnityObjectExtensions.DestroyObject(trackedPoseDriver);
                trackedPoseDriver = null;
                UnityObjectExtensions.DestroyObject(arCameraBackground);
                arCameraBackground = null;
                UnityObjectExtensions.DestroyObject(arCameraManager);
                arCameraManager = null;
                UnityObjectExtensions.DestroyObject(arSessionOrigin);
                arSessionOrigin = null;
            }

            if (!preExistingArSessionObject &&
                (arSessionObject != null))
            {
                UnityObjectExtensions.DestroyObject(arInputManager);
                arInputManager = null;
                UnityObjectExtensions.DestroyObject(arSession);
                arSession = null;
                UnityObjectExtensions.DestroyObject(arSessionObject);
                arSessionObject = null;
            }
#endif // !(WINDOWS_UWP && !ENABLE_IL2CPP)

            isInitialized = false;
        }
    }
}
