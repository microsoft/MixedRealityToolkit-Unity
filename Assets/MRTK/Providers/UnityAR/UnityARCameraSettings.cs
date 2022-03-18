// Copyright (c) Microsoft Corporation.
// Copyright(c) 2019 Takahiro Miyaura
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.CameraSystem;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

#if ARFOUNDATION_PRESENT
using UnityEngine.SpatialTracking;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
#endif // ARFOUNDATION_PRESENT

namespace Microsoft.MixedReality.Toolkit.Experimental.UnityAR
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
    public class UnityARCameraSettings : BaseCameraSettingsProvider
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="cameraSystem">The instance of the camera system which is managing this provider.</param>
        /// <param name="name">Friendly name of the provider.</param>
        /// <param name="priority">Provider priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The provider's configuration profile.</param>
        public UnityARCameraSettings(
            IMixedRealityCameraSystem cameraSystem,
            string name = null,
            uint priority = DefaultPriority,
            BaseCameraSettingsProfile profile = null) : base(cameraSystem, name, priority, profile)
        {
            ReadProfile();
        }

        private ArTrackedPose poseSource = ArTrackedPose.ColorCamera;
        private ArTrackingType trackingType = ArTrackingType.RotationAndPosition;
        private ArUpdateType updateType = ArUpdateType.UpdateAndBeforeRender;

        private void ReadProfile()
        {
            if (SettingsProfile == null)
            {
                Debug.LogWarning("A profile was not specified for the Unity AR Camera Settings provider.\nUsing Microsoft Mixed Reality Toolkit default options.");
                return;
            }

            poseSource = SettingsProfile.PoseSource;
            trackingType = SettingsProfile.TrackingType;
            updateType = SettingsProfile.UpdateType;
        }

        #region IMixedRealityCameraSettings

        /// <inheritdoc/>
        public override bool IsOpaque => poseSource != ArTrackedPose.ColorCamera;

        #endregion IMixedRealityCameraSettings

        /// <summary>
        /// The profile used to configure the camera.
        /// </summary>
        public UnityARCameraSettingsProfile SettingsProfile => ConfigurationProfile as UnityARCameraSettingsProfile;

#if ARFOUNDATION_PRESENT
        private bool isSupportedArConfiguration = true;
        private bool isInitialized = false;

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

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();

            // Android platforms support both AR Foundation and VR.
            // AR Foundation does not use the player's XR Settings.
            // If the loaded device name is not an empty string, then a VR
            // SDK is in use (not using AR Foundation).
            if (Application.platform == RuntimePlatform.Android)
            {
                isSupportedArConfiguration = string.IsNullOrWhiteSpace(XRSettings.loadedDeviceName);
            }
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
        /// <para>This method ensures AR Foundation required components (ex: AR Session, Tracked Pose Driver, etc) are
        /// exist or are added to the appropriate scene objects. These components are used by AR Foundation to
        /// communicate with the underlying AR platform (ex: AR Core), track the device and perform other necessary tasks.</para>
        /// </remarks>
        private void InitializeARFoundation()
        {
            if (!isSupportedArConfiguration) { return; }

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

            trackedPoseDriver.SetPoseSource(
                TrackedPoseDriver.DeviceType.GenericXRDevice,
                ArEnumConversion.ToUnityTrackedPose(poseSource));
            trackedPoseDriver.trackingType = ArEnumConversion.ToUnityTrackingType(trackingType);
            trackedPoseDriver.updateType = ArEnumConversion.ToUnityUpdateType(updateType);
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

            isInitialized = false;
        }
#endif // ARFOUNDATION_PRESENT
    }
}
