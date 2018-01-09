// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

#if UNITY_WSA && UNITY_2017_2_OR_NEWER
using UnityEngine.XR.WSA;
#endif

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// This script tells you if your head mounted display (HMD)
    /// is a transparent device or an occluded device.
    /// Based on those values, you can customize your camera settings.
    /// It also fires an OnDisplayDetected event.
    /// </summary>
    public class MixedRealityCameraManager : Singleton<MixedRealityCameraManager>
    {
        [Tooltip("The near clipping plane distance for an opaque display.")]
        public float NearClipPlane_OpaqueDisplay = 0.1f;

        [Tooltip("Values for Camera.clearFlags, determining what to clear when rendering a Camera for an opaque display.")]
        public CameraClearFlags CameraClearFlags_OpaqueDisplay = CameraClearFlags.Skybox;

        [Tooltip("Background color for a transparent display.")]
        public Color BackgroundColor_OpaqueDisplay = Color.black;

        [Tooltip("Set the desired quality for your application for opaque display.")]
        public int OpaqueQualityLevel;

        [Tooltip("The near clipping plane distance for a transparent display.")]
        public float NearClipPlane_TransparentDisplay = 0.85f;

        [Tooltip("Values for Camera.clearFlags, determining what to clear when rendering a Camera for an opaque display.")]
        public CameraClearFlags CameraClearFlags_TransparentDisplay = CameraClearFlags.SolidColor;

        [Tooltip("Background color for a transparent display.")]
        public Color BackgroundColor_TransparentDisplay = Color.clear;

        [Tooltip("Set the desired quality for your application for HoloLens.")]
        public int HoloLensQualityLevel;

        public enum DisplayType
        {
            Opaque = 0,
            Transparent
        };

        public DisplayType CurrentDisplayType { get; private set; }

        public delegate void DisplayEventHandler(DisplayType displayType);
        /// <summary>
        /// Event is fired when a display is detected.
        /// DisplayType enum value tells you if display is Opaque Vs Transparent.
        /// </summary>
        public event DisplayEventHandler OnDisplayDetected;

        private void Start()
        {
            CurrentDisplayType = DisplayType.Opaque;

#if UNITY_WSA
#if UNITY_2017_2_OR_NEWER
            if (!HolographicSettings.IsDisplayOpaque)
#endif
            {
                CurrentDisplayType = DisplayType.Transparent;
            }
#endif

            if (CurrentDisplayType == DisplayType.Opaque)
            {
                ApplySettingsForOpaqueDisplay();
            }
            else
            {
                ApplySettingsForTransparentDisplay();
            }

            if (OnDisplayDetected != null)
            {
                OnDisplayDetected(CurrentDisplayType);
            }
        }

        public void ApplySettingsForOpaqueDisplay()
        {
            Debug.Log("Display is Opaque");
            CameraCache.Main.clearFlags = CameraClearFlags_OpaqueDisplay;
            CameraCache.Main.nearClipPlane = NearClipPlane_OpaqueDisplay;
            CameraCache.Main.backgroundColor = BackgroundColor_OpaqueDisplay;
            SetQuality(OpaqueQualityLevel);
        }

        public void ApplySettingsForTransparentDisplay()
        {
            Debug.Log("Display is Transparent");
            CameraCache.Main.clearFlags = CameraClearFlags_TransparentDisplay;
            CameraCache.Main.backgroundColor = BackgroundColor_TransparentDisplay;
            CameraCache.Main.nearClipPlane = NearClipPlane_TransparentDisplay;
            SetQuality(HoloLensQualityLevel);
        }

        private static void SetQuality(int level)
        {
            QualitySettings.SetQualityLevel(level, false);
        }
    }
}
