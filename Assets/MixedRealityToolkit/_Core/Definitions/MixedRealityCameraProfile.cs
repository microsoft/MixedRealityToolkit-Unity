// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions
{
    /// <summary>
    /// This Scriptable Object tells you if your head mounted display (HMD)
    /// is a transparent device or an occluded device.
    /// Based on those values, you can customize your camera and quality settings.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Camera Profile", fileName = "MixedRealityCameraProfile", order = (int)CreateProfileMenuItemIndices.Camera)]
    public class MixedRealityCameraProfile : ScriptableObject
    {
        private enum DisplayType
        {
            Opaque = 0,
            Transparent
        }

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

        private static DisplayType currentDisplayType;

        public static bool IsOpaque
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

        public void ApplySettingsForOpaqueDisplay()
        {
            CameraCache.Main.clearFlags = cameraClearFlagsOpaqueDisplay;
            CameraCache.Main.nearClipPlane = nearClipPlaneOpaqueDisplay;
            CameraCache.Main.backgroundColor = backgroundColorOpaqueDisplay;
            SetQuality(opaqueQualityLevel);
        }

        public void ApplySettingsForTransparentDisplay()
        {
            CameraCache.Main.clearFlags = cameraClearFlagsTransparentDisplay;
            CameraCache.Main.backgroundColor = backgroundColorTransparentDisplay;
            CameraCache.Main.nearClipPlane = nearClipPlaneTransparentDisplay;
            SetQuality(holoLensQualityLevel);
        }

        private static void SetQuality(int level)
        {
            QualitySettings.SetQualityLevel(level, false);
        }
    }
}
