// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.CameraSystem;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// This Scriptable Object tells you if your head mounted display (HMD)
    /// is a transparent device or an occluded device.
    /// Based on those values, you can customize your camera and quality settings.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Camera Profile", fileName = "MixedRealityCameraProfile", order = (int)CreateProfileMenuItemIndices.Camera)]
    [MixedRealityServiceProfile(typeof(IMixedRealityCameraSystem))]
    public class MixedRealityCameraProfile : BaseMixedRealityProfile
    {
        public float NearClipPlaneOpaqueDisplay => nearClipPlaneOpaqueDisplay;
        public CameraClearFlags CameraClearFlagsOpaqueDisplay => cameraClearFlagsOpaqueDisplay;
        public Color BackgroundColorOpaqueDisplay => backgroundColorOpaqueDisplay;
        public int OpaqueQualityLevel => opaqueQualityLevel;

        public float NearClipPlaneTransparentDisplay => nearClipPlaneTransparentDisplay;
        public CameraClearFlags CameraClearFlagsTransparentDisplay => cameraClearFlagsTransparentDisplay;
        public Color BackgroundColorTransparentDisplay => backgroundColorTransparentDisplay;
        public int HoloLensQualityLevel => holoLensQualityLevel;

        [SerializeField]
        [Tooltip("The near clipping plane distance for an opaque display.")]
        private float nearClipPlaneOpaqueDisplay = 0.1f;

        [SerializeField]
        [Tooltip("Values for Camera.clearFlags, determining what to clear when rendering a Camera for an opaque display.")]
        private CameraClearFlags cameraClearFlagsOpaqueDisplay = CameraClearFlags.Skybox;

        [SerializeField]
        [Tooltip("Background color for a transparent display.")]
        private Color backgroundColorOpaqueDisplay = Color.black;

        [SerializeField]
        [Tooltip("Set the desired quality for your application for opaque display.")]
        private int opaqueQualityLevel = 0;

        [SerializeField]
        [Tooltip("The near clipping plane distance for a transparent display.")]
        private float nearClipPlaneTransparentDisplay = 0.85f;

        [SerializeField]
        [Tooltip("Values for Camera.clearFlags, determining what to clear when rendering a Camera for an opaque display.")]
        private CameraClearFlags cameraClearFlagsTransparentDisplay = CameraClearFlags.SolidColor;

        [SerializeField]
        [Tooltip("Background color for a transparent display.")]
        private Color backgroundColorTransparentDisplay = Color.clear;

        [SerializeField]
        [Tooltip("Set the desired quality for your application for HoloLens.")]
        private int holoLensQualityLevel = 0;
    }
}
