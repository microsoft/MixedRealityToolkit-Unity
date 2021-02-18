// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.CameraSystem;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// This Scriptable Object tells you if your head mounted display (HMD)
    /// is a transparent device or an occluded device.
    /// Based on those values, you can customize your camera and quality settings.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Profiles/Mixed Reality Camera Profile", fileName = "MixedRealityCameraProfile", order = (int)CreateProfileMenuItemIndices.Camera)]
    [MixedRealityServiceProfile(typeof(IMixedRealityCameraSystem))]
    [HelpURL("https://docs.microsoft.com/windows/mixed-reality/mrtk-unity/configuration/mixed-reality-configuration-guide#camera")]
    public class MixedRealityCameraProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        [Tooltip("Configuration objects describing the registered settings providers.")]
        private MixedRealityCameraSettingsConfiguration[] settingsConfigurations = new MixedRealityCameraSettingsConfiguration[0];

        /// <summary>
        /// Configuration objects describing the registered settings providers.
        /// </summary>
        public MixedRealityCameraSettingsConfiguration[] SettingsConfigurations
        {
            get { return settingsConfigurations; }
            internal set { settingsConfigurations = value; }
        }

        [SerializeField]
        [Tooltip("Near clipping plane distance for an opaque display.")]
        private float nearClipPlaneOpaqueDisplay = 0.1f;

        /// <summary>
        /// Near clipping plane distance for an opaque display.
        /// </summary>
        public float NearClipPlaneOpaqueDisplay => nearClipPlaneOpaqueDisplay;

        [SerializeField]
        [Tooltip("Far clipping plane distance for an opaque display.")]
        private float farClipPlaneOpaqueDisplay = 1000f;

        /// <summary>
        /// Far clipping plane distance for an opaque display.
        /// </summary>
        public float FarClipPlaneOpaqueDisplay => farClipPlaneOpaqueDisplay;

        [SerializeField]
        [Tooltip("Flags describing how to clear the camera for an opaque display.")]
        private CameraClearFlags cameraClearFlagsOpaqueDisplay = CameraClearFlags.Skybox;

        /// <summary>
        /// Flags describing how to clear the camera for an opaque display.
        /// </summary>
        public CameraClearFlags CameraClearFlagsOpaqueDisplay => cameraClearFlagsOpaqueDisplay;

        [SerializeField]
        [Tooltip("Background color for an opaque display.")]
        private Color backgroundColorOpaqueDisplay = Color.black;

        /// <summary>
        /// Background color for an opaque display.
        /// </summary>
        public Color BackgroundColorOpaqueDisplay => backgroundColorOpaqueDisplay;

        [SerializeField]
        [Tooltip("Quality level for an opaque display.")]
        private int opaqueQualityLevel = 0;

        /// <summary>
        /// Quality level for an opaque display.
        /// </summary>
        public int OpaqueQualityLevel => opaqueQualityLevel;

        [SerializeField]
        [Tooltip("Near clipping plane distance for a transparent display.")]
        private float nearClipPlaneTransparentDisplay = 0.85f;

        /// <summary>
        /// Near clipping plane distance for a transparent display.
        /// </summary>
        public float NearClipPlaneTransparentDisplay => nearClipPlaneTransparentDisplay;

        [SerializeField]
        [Tooltip("Far clipping plane distance for a transparent display.")]
        private float farClipPlaneTransparentDisplay = 50f;

        /// <summary>
        /// Far clipping plane distance for a transparent display.
        /// </summary>
        public float FarClipPlaneTransparentDisplay => farClipPlaneTransparentDisplay;

        [SerializeField]
        [Tooltip("Flags describing how to clear the camera for a transparent display.")]
        private CameraClearFlags cameraClearFlagsTransparentDisplay = CameraClearFlags.SolidColor;

        /// <summary>
        /// Flags describing how to clear the camera for a transparent display.
        /// </summary>
        public CameraClearFlags CameraClearFlagsTransparentDisplay => cameraClearFlagsTransparentDisplay;

        [SerializeField]
        [Tooltip("Background color for a transparent display.")]
        private Color backgroundColorTransparentDisplay = Color.clear;

        /// <summary>
        /// Background color for a transparent display.
        /// </summary>
        public Color BackgroundColorTransparentDisplay => backgroundColorTransparentDisplay;

        [SerializeField]
        [Tooltip("Quality level for a transparent display.")]
        [FormerlySerializedAs("holoLensQualityLevel")]
        private int transparentQualityLevel = 0;

        /// <summary>
        /// Quality level for a transparent display.
        /// </summary>
        public int TransparentQualityLevel => transparentQualityLevel;

        #region Obsolete properties

        /// <summary>
        /// Quality level for a HoloLens device.
        /// </summary>
        /// <remarks>
        /// HoloLensQualityLevel is obsolete and will be removed in a future Mixed Reality Toolkit release. Please use TransparentQualityLevel.
        /// </remarks>
        [Obsolete("HoloLensQualityLevel is obsolete and will be removed in a future Mixed Reality Toolkit release. Please use TransparentQualityLevel.")]
        public int HoloLensQualityLevel => transparentQualityLevel;

        #endregion Obsolete properties

    }
}
