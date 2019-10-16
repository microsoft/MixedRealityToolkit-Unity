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
    public class ARFoundationCameraSystem : BaseCoreSystem, IMixedRealityCameraSystem
    {
        public ARFoundationCameraSystem(
            IMixedRealityServiceRegistrar registrar,
            BaseMixedRealityProfile profile = null) : base(registrar, profile)
        {
            // todo: find objects in scene... this will keep us from double adding components....
        }

        private bool arFoundationSupported = false;
        public bool ARFoundationSupported
        {
            get => arFoundationSupported;
            private set
            {
                arFoundationSupported = value;
            }
        }

//#region IMixedRealityService

        /// <inheritdoc/>
        public override string Name { get; protected set; } = "Mixed Reality Camera System for AR Foundation";

        /// <inheritdoc />
        public uint SourceId { get; } = 0;

        /// <inheritdoc />
        public string SourceName => Name;

        public override /*async*/ void Initialize()
        {
            base.Initialize();
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

        bool isInitialized = false;

        private GameObject arSessionObject = null;
        private ARCameraBackground arCameraBackground = null;
        private ARCameraManager arCameraManager = null;
        private ARSession arSession = null;
        private ARSessionOrigin arSessionOrigin = null;
        private TrackedPoseDriver trackedPoseDriver = null;

        private void InitializeARFoundation()
        {
            if (isInitialized) { return; }

            // todo: look for object(s) and scripts
            GameObject arSessionObject = new GameObject("AR Session");
            arSessionObject.transform.parent = null;
            arSession = arSessionObject.AddComponent<ARSession>();
            arSession.attemptUpdate = true;
            arSession.matchFrameRate = true;
            arSessionObject.AddComponent<ARInputManager>();

            Camera arCamera = CameraCache.Main;
            trackedPoseDriver = arCamera.gameObject.AddComponent<TrackedPoseDriver>();
            trackedPoseDriver.SetPoseSource(
                TrackedPoseDriver.DeviceType.GenericXRDevice,
                TrackedPoseDriver.TrackedPose.ColorCamera);

            arCameraManager = arCamera.gameObject.AddComponent<ARCameraManager>();
            arCameraBackground = arCamera.gameObject.AddComponent<ARCameraBackground>();

            GameObject playspaceObject = MixedRealityPlayspace.Transform.gameObject;
            arSessionOrigin = playspaceObject.AddComponent<ARSessionOrigin>();
            arSessionOrigin.camera = arCamera;

            isInitialized = true;
        }

        private void UninitializeARFoundation()
        {
            if (!isInitialized) { return; }

            if (!Application.isEditor)
            {
                Object.Destroy(trackedPoseDriver);
                Object.Destroy(arCameraBackground);
                Object.Destroy(arCameraManager);
                Object.Destroy(arSessionOrigin);
                Object.Destroy(arSession);
                Object.Destroy(arSessionObject);
            }
            else
            {
                Object.DestroyImmediate(trackedPoseDriver);
                Object.DestroyImmediate(arCameraBackground);
                Object.DestroyImmediate(arCameraManager);
                Object.DestroyImmediate(arSessionOrigin);
                Object.DestroyImmediate(arSession);
                Object.DestroyImmediate(arSessionObject);
            }

            trackedPoseDriver = null;
            arCameraBackground = null;
            arCameraManager = null;
            arSessionOrigin = null;
            arSession = null;
            arSessionObject = null;

            isInitialized = false;
        }

//#endregion IMixedRealityService

//#region IMixedRealityCameraSystem

        /// <inheritdoc/>
        public bool IsOpaque => true;

        public MixedRealityCameraProfile CameraProfile
        {
            get
            {
                return ConfigurationProfile as MixedRealityCameraProfile;
            }
        }

//#endregion IMixedRealityCameraSystem

//#region IEqualityComparer

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

//#endregion IEqualityComparer
    }
}
