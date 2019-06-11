// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.CameraSystem
{
    /// <summary>
    /// The Camera system controls the settings of the main camera.
    /// </summary>
    [DocLink("https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/MixedRealityConfigurationGuide.html#camera")]
    public class MixedRealityCameraSystem : BaseCoreSystem, IMixedRealityCameraSystem
    {
        private enum DisplayType
        {
            Opaque = 0,
            Transparent
        }

        public MixedRealityCameraSystem(
            IMixedRealityServiceRegistrar registrar,
            BaseMixedRealityProfile profile = null) : base(registrar, profile)
        {
        }

        /// <summary>
        /// Is the current camera displaying on an Opaque (AR) device or a VR / immersive device
        /// </summary>
        public bool IsOpaque
        {
            get
            {
                currentDisplayType = DisplayType.Opaque;
#if UNITY_WSA
                if (!UnityEngine.XR.WSA.HolographicSettings.IsDisplayOpaque)
                {
                    currentDisplayType = DisplayType.Transparent;
                }
#endif
                return currentDisplayType == DisplayType.Opaque;
            }
        }

        /// <inheritdoc />
        public uint SourceId { get; } = 0;

        /// <inheritdoc />
        public string SourceName { get; } = "Mixed Reality Camera System";

        private MixedRealityCameraProfile cameraProfile = null;

        /// <inheritdoc/>
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
        
        /// <inheritdoc/>
        public Camera Main
        {
            get
            {
                return FindOrCreateMainCamera();
            }
        }

        private DisplayType currentDisplayType;
        private bool cameraOpaqueLastFrame = false;
        private static Camera cachedCamera;

        /// <inheritdoc />
        public override void Enable()
        {
            cameraOpaqueLastFrame = IsOpaque;

            FindOrCreateMainCamera();

            if (IsOpaque)
            {
                ApplySettingsForOpaqueDisplay();
            }
            else
            {
                ApplySettingsForTransparentDisplay();
            }
        }

        /// <inheritdoc />
        public override void Update()
        {
            if (IsOpaque != cameraOpaqueLastFrame)
            {
                cameraOpaqueLastFrame = IsOpaque;

                if (IsOpaque)
                {
                    ApplySettingsForOpaqueDisplay();
                }
                else
                {
                    ApplySettingsForTransparentDisplay();
                }
            }
        }

        /// <summary>
        /// Applies opaque settings from camera profile.
        /// </summary>
        private void ApplySettingsForOpaqueDisplay()
        {
            Main.clearFlags = CameraProfile.CameraClearFlagsOpaqueDisplay;
            Main.nearClipPlane = CameraProfile.NearClipPlaneOpaqueDisplay;
            Main.farClipPlane = CameraProfile.FarClipPlaneOpaqueDisplay;
            Main.backgroundColor = CameraProfile.BackgroundColorOpaqueDisplay;
            QualitySettings.SetQualityLevel(CameraProfile.OpaqueQualityLevel, false);
        }

        /// <summary>
        /// Applies transparent settings from camera profile.
        /// </summary>
        private void ApplySettingsForTransparentDisplay()
        {
            Main.clearFlags = CameraProfile.CameraClearFlagsTransparentDisplay;
            Main.backgroundColor = CameraProfile.BackgroundColorTransparentDisplay;
            Main.nearClipPlane = CameraProfile.NearClipPlaneTransparentDisplay;
            Main.farClipPlane = CameraProfile.FarClipPlaneTransparentDisplay;
            QualitySettings.SetQualityLevel(CameraProfile.HoloLensQualityLevel, false);
        }

        private Camera FindOrCreateMainCamera()
        {
            if (cachedCamera != null)
            {   // Ensure that the camera is parented under the mixed reality playspace
                cachedCamera.transform.SetParent(MixedRealityPlayspace.Transform);
                return cachedCamera;
            }

            // If the cached camera is null, search for main
            var mainCamera = Camera.main;

            if (mainCamera == null)
            {   // If no main camera was found, create it now
                Debug.LogWarning("No main camera found. The Mixed Reality Toolkit requires at least one camera in the scene. One will be generated now.");
                mainCamera = new GameObject("Main Camera", typeof(Camera)) { tag = "MainCamera" }.GetComponent<Camera>();
            }

            // Cache the main camera
            cachedCamera = mainCamera;
            // Ensure that the camera is parented under the mixed reality playspace
            cachedCamera.transform.SetParent(MixedRealityPlayspace.Transform);

            return cachedCamera;
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