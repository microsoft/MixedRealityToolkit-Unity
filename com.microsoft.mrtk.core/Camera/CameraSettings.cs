// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Class that contains settings which are applied to <see cref="Microsoft.MixedReality.Toolkit.CameraSettingsManager">CameraSettingsManager</see>.
    /// </summary>
    [Serializable]
    [AddComponentMenu("MRTK/Core/Camera Settings")]
    public class CameraSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CameraSettings"/> class.
        /// </summary>
        public CameraSettings() : this(DisplayType.Transparent)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CameraSettings"/> class.
        /// </summary>
        /// <param name="displayType">
        /// <see cref="DisplayType"/> value describing the device display.
        /// </param>
        public CameraSettings(DisplayType displayType)
        {
            switch (displayType)
            {
                case DisplayType.Transparent:
                    ClearMode = DefaultClearModeTransparent;
                    ClearColor = DefaultClearColorTransparent;
                    NearPlaneDistance = DefaultNearPlaneDistanceTransparent;
                    FarPlaneDistance = DefaultFarPlaneDistanceTransparent;
                    AdjustQualityLevel = true;
                    QualityLevel = DefaultQualityLevelTransparent;
                    break;

                case DisplayType.Opaque:
                    ClearMode = DefaultClearModeOpaque;
                    ClearColor = DefaultClearColorOpaque;
                    NearPlaneDistance = DefaultNearPlaneDistanceOpaque;
                    FarPlaneDistance = DefaultFarPlaneDistanceOpaque;
                    AdjustQualityLevel = true;
                    QualityLevel = DefaultQualityLevelOpaque;
                    break;
            }
        }

        /// <summary>
        /// The default clear mode used for opaque displays.
        /// </summary>
        public static readonly CameraClearFlags DefaultClearModeOpaque = CameraClearFlags.Skybox;

        /// <summary>
        /// The default clear mode used for transparent displays.
        /// </summary>
        public static readonly CameraClearFlags DefaultClearModeTransparent = CameraClearFlags.SolidColor;

        [SerializeField]
        [Tooltip("How the display should be cleared")]
        private CameraClearFlags clearMode;

        /// <summary>
        /// How the display should be cleared.
        /// </summary>
        public CameraClearFlags ClearMode
        {
            get => clearMode;
            set => clearMode = value;
        }

        public static readonly Color DefaultClearColorOpaque = Color.black;
        public static readonly Color DefaultClearColorTransparent = Color.black;

        [SerializeField]
        [Tooltip("Color to use when clearing (does not apply to Skybox or Depth)")]
        private Color clearColor;

        /// <summary>
        /// Color to use when clearing (does not apply to Skybox or Depth).
        /// </summary>
        public Color ClearColor
        {
            get => clearColor;
            set => clearColor = value;
        }

        public static readonly float DefaultNearPlaneDistanceOpaque = 0.1f;
        public static readonly float DefaultNearPlaneDistanceTransparent = 0.1f;

        [SerializeField]
        [Tooltip("Closest distance (in meters) at which holograms will display.")]
        private float nearPlaneDistance;

        /// <summary>
        /// Closest distance (in meters) at which holograms will display.
        /// </summary>
        public float NearPlaneDistance
        {
            get => nearPlaneDistance;
            set => nearPlaneDistance = value;
        }

        public static readonly float DefaultFarPlaneDistanceOpaque = 1000f;
        public static readonly float DefaultFarPlaneDistanceTransparent = 50f;

        [SerializeField]
        [Tooltip("Furthest distance (in meters) at which holograms will display.")]
        private float farPlaneDistance;

        /// <summary>
        /// Furthest distance (in meters) at which holograms will display.
        /// </summary>
        public float FarPlaneDistance
        {
            get => farPlaneDistance;
            set => farPlaneDistance = value;
        }

        [SerializeField]
        [Tooltip("Should the tracking origin be adjusted base on camera type?")]
        private bool adjustTrackingOrigin = true;

        /// <summary>
        /// Should the tracking origin be adjusted based on camera type?
        /// </summary>
        public bool AdjustTrackingOrigin
        {
            get => adjustTrackingOrigin;
            set => adjustTrackingOrigin = value;
        }

        [SerializeField]
        [Tooltip("Should the quality level be adjusted based on camera type?")]
        private bool adjustQualityLevel = true;

        /// <summary>
        /// Should the quality level be adjusted based on camera type?
        /// </summary>
        public bool AdjustQualityLevel
        {
            get => adjustQualityLevel;
            set => adjustQualityLevel = value;
        }

        public static readonly int DefaultQualityLevelOpaque = 5;       // Ultra
        public static readonly int DefaultQualityLevelTransparent = 0;  // Very Low

        [SerializeField]
        [Tooltip("The desired level of graphical quality.")]
        private int qualityLevel;

        /// <summary>
        /// The desired level of graphical quality.
        /// </summary>
        public int QualityLevel
        {
            get => qualityLevel;
            set => qualityLevel = value;
        }
    }
}
