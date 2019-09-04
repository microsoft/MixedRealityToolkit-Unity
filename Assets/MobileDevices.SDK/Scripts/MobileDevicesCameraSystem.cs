// Copyright(c) 2019 Takahiro Miyaura
// Released under the MIT license
// http://opensource.org/licenses/mit-license.php

using System.Collections;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.CameraSystem;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.SpatialTracking;
using UnityEngine.XR.ARFoundation;

namespace Com.Reseul.MobileDevices.CameraSystem
{
    public class MobileDevicesCameraSystem : BaseCoreSystem, IMixedRealityCameraSystem
    {
        private ARCameraBackground _aRCameraBackground;
        private ARCameraManager _aRCameraManager;
        private GameObject _arSession;

        private GameObject _arSessionOrigin;
        private TrackedPoseDriver _trackedPoseDriver;

        private MixedRealityCameraProfile cameraProfile;

        public MobileDevicesCameraSystem(
            IMixedRealityServiceRegistrar registrar,
            BaseMixedRealityProfile profile = null) : base(registrar, profile)
        {
        }

        public bool IsOpaque => true;

        public override void Destroy()
        {
#if !UNITY_EDITOR
            ReleaseForARFoundation();
#endif
            base.Destroy();
        }

        /// <inheritdoc />
        public uint SourceId { get; } = 0;

        /// <inheritdoc />
        public string SourceName { get; } = "MRTK For Smart Phone Camera System";

        /// <inheritdoc />
        public MixedRealityCameraProfile CameraProfile
        {
            get
            {
                if (cameraProfile == null)
                {
                    cameraProfile = ConfigurationProfile as MixedRealityCameraProfile;
                }

                return cameraProfile;
            }
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();
#if !UNITY_EDITOR
            ApplySettingsForARFoundation();
#endif
        }

        /// <inheritdoc />
        public override void Update()
        {
            base.Update();
#if !UNITY_EDITOR
            ApplySettingsForARFoundation();
#endif
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

        private void ReleaseForARFoundation()
        {
            if (MixedRealityPlayspace.Transform.GetComponentInChildren<ARSessionOrigin>() == null) return;
            if (MixedRealityPlayspace.Transform.GetComponentInChildren<ARSession>() == null) return;

            CameraCache.Main.transform.parent = MixedRealityPlayspace.Transform;
            Object.DestroyImmediate(_trackedPoseDriver);
            Object.DestroyImmediate(_aRCameraBackground);
            Object.DestroyImmediate(_aRCameraManager);
            Object.DestroyImmediate(_arSessionOrigin);
            Object.DestroyImmediate(_arSession);

            _arSessionOrigin = null;
            _arSession = null;
            _aRCameraBackground = null;
            _aRCameraManager = null;
            _trackedPoseDriver = null;
        }

        private void ApplySettingsForARFoundation()
        {
            if (MixedRealityPlayspace.Transform.GetComponentInChildren<ARSessionOrigin>() != null) return;
            if (MixedRealityPlayspace.Transform.GetComponentInChildren<ARSession>() != null) return;

            //Setting Camera for ARFoundation.
            _trackedPoseDriver = CameraCache.Main.gameObject.AddComponent<TrackedPoseDriver>();
            _aRCameraManager = CameraCache.Main.gameObject.AddComponent<ARCameraManager>();
            _aRCameraBackground = CameraCache.Main.gameObject.AddComponent<ARCameraBackground>();

            _arSessionOrigin = new GameObject();
            _arSessionOrigin.name = "AR Session Origin";
            _arSessionOrigin.transform.parent = MixedRealityPlayspace.Transform;
            CameraCache.Main.transform.parent = _arSessionOrigin.transform;
            var arSessionOrigin = _arSessionOrigin.AddComponent<ARSessionOrigin>();
            arSessionOrigin.camera = CameraCache.Main;

            _arSession = new GameObject();
            _arSession.name = "AR Session";
            _arSession.transform.parent = MixedRealityPlayspace.Transform;
            var arSessionComp = _arSession.AddComponent<ARSession>();
            arSessionComp.attemptUpdate = true;
            arSessionComp.matchFrameRate = true;
            _arSession.AddComponent<ARInputManager>();

            _trackedPoseDriver.SetPoseSource(TrackedPoseDriver.DeviceType.GenericXRDevice,
                TrackedPoseDriver.TrackedPose.ColorCamera);
        }
    }
}