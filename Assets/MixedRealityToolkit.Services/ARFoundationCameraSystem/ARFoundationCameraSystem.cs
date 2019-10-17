// Copyright (c) Microsoft Corporation. All rights reserved.
// Copyright(c) 2019 Takahiro Miyaura
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using UnityEngine;
using UnityEngine.SpatialTracking;
using UnityEngine.XR.ARFoundation;

namespace Microsoft.MixedReality.Toolkit.CameraSystem
{
    /// <summary>
    /// Camera system implementation that uses Unity's AR Foundation components.
    /// </summary>
    public class ARFoundationCameraSystem : BaseCoreSystem, IMixedRealityCameraSystem
    {
        public ARFoundationCameraSystem(
            IMixedRealityServiceRegistrar registrar,
            BaseMixedRealityProfile profile = null) : base(registrar, profile)
        { }

        /// <inheritdoc/>
        public override string Name { get; protected set; } = "Mixed Reality Camera System for AR Foundation";

        /// <inheritdoc />
        public uint SourceId { get; } = 0;

        /// <inheritdoc />
        public string SourceName => Name;

        public override /*async*/ void Initialize()
        {
            base.Initialize();

            ApplyCameraSettings();

            //ARSessionState arState = (ARSessionState)(await ARSession.CheckAvailability());
            //ARFoundationSupported = (ARSessionState.Ready <= arState);
            //if (ARFoundationSupported)
            {
                InitializeARFoundation();
            }
        }

        public override void Destroy()
        {
            //if (ARFoundationSupported)
            {
                UninitializeARFoundation();
            }
            base.Destroy();
        }

        // todo private bool arFoundationSupported = false;
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

        /// <summary>
        /// Apply the camera settings from the configuration profile.
        /// </summary>
        /// <remarks>
        /// This camera system uses the device camera as a passthrough, therefore it uses the transparent camera settings.
        /// </remarks>
        private void ApplyCameraSettings()
        {
            CameraCache.Main.clearFlags = CameraProfile.CameraClearFlagsTransparentDisplay;
            CameraCache.Main.backgroundColor = CameraProfile.BackgroundColorTransparentDisplay;
            CameraCache.Main.nearClipPlane = CameraProfile.NearClipPlaneTransparentDisplay;
            CameraCache.Main.farClipPlane = CameraProfile.FarClipPlaneTransparentDisplay;
            QualitySettings.SetQualityLevel(CameraProfile.TransparentQualityLevel, false);
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
            arSession.attemptUpdate = true;
            arSession.matchFrameRate = true;

            arInputManager = arSessionObject.EnsureComponent<ARInputManager>();

            if (arSessionOriginObject == null)
            {
                arSessionOriginObject = new GameObject("AR Session Origin");
                arSessionOriginObject.transform.parent = null;
            }
            else
            {
                arSessionOriginObject = MixedRealityPlayspace.Transform.gameObject;
                CameraCache.Main.transform.parent = arSessionOriginObject.transform;
            }

            arSessionOrigin = arSessionOriginObject.EnsureComponent<ARSessionOrigin>();
            arSessionOrigin.camera = CameraCache.Main;

            GameObject cameraObject = arSessionOrigin.camera.gameObject;
            
            arCameraManager = cameraObject.EnsureComponent<ARCameraManager>();
            arCameraBackground = cameraObject.EnsureComponent<ARCameraBackground>();
            trackedPoseDriver = cameraObject.EnsureComponent<TrackedPoseDriver>();

            trackedPoseDriver.SetPoseSource(
                TrackedPoseDriver.DeviceType.GenericXRDevice,
                TrackedPoseDriver.TrackedPose.ColorCamera);
            trackedPoseDriver.trackingType = TrackedPoseDriver.TrackingType.RotationAndPosition;
            trackedPoseDriver.updateType = TrackedPoseDriver.UpdateType.UpdateAndBeforeRender;
            trackedPoseDriver.UseRelativeTransform = false;

            isInitialized = true;
        }

        /// <summary>
        /// Uninitialize and clean up AR Foundation components.
        /// </summary>
        private void UninitializeARFoundation()
        {
            if (!isInitialized) { return; }

            if (!preExistingArSessionOriginObject)
            {
                if (Application.isEditor && !Application.isPlaying)
                {
                    Object.DestroyImmediate(trackedPoseDriver);
                    Object.DestroyImmediate(arCameraBackground);
                    Object.DestroyImmediate(arCameraManager);
                    Object.DestroyImmediate(arSessionOrigin);
                    Object.DestroyImmediate(arSessionOriginObject);
                }
                else
                {
                    Object.Destroy(trackedPoseDriver);
                    Object.Destroy(arCameraBackground);
                    Object.Destroy(arCameraManager);
                    Object.Destroy(arSessionOrigin);
                    Object.Destroy(arSessionOriginObject);
                }

                trackedPoseDriver = null;
                arCameraBackground = null;
                arCameraManager = null;
                arSessionOrigin = null;
                arSessionOriginObject = null;
            }

            if (!preExistingArSessionObject)
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

        /// <inheritdoc/>
        public bool IsOpaque => true;

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

        /// <inheritdoc />
        bool IEqualityComparer.Equals(object x, object y)
        {
            // There shouldn't be other Camera Systems to compare to.
            return false;
        }

        /// <inheritdoc />
        int IEqualityComparer.GetHashCode(object obj)
        {
            return Mathf.Abs(SourceName.GetHashCode());
        }
    }
}
