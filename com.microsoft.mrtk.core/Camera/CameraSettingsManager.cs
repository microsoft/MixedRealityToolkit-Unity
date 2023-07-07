// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Unity.Profiling;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Class that applies the appropriate camera settings based on the display type.
    /// </summary>
    [AddComponentMenu("MRTK/Core/Camera Settings Manager")]
    public class CameraSettingsManager : MonoBehaviour
    {
        [SerializeField]
        private CameraSettings opaqueDisplay = new CameraSettings(DisplayType.Opaque);

        /// <summary>
        /// The settings to apply when the display is opaque (VR).
        /// </summary>
        public CameraSettings OpaqueDisplay
        {
            get => opaqueDisplay;
            set => opaqueDisplay = value;
        }

        [SerializeField]
        private CameraSettings transparentDisplay = new CameraSettings(DisplayType.Transparent);

        /// <summary>
        /// The settings to apply when the display is transparent (AR).
        /// </summary>
        public CameraSettings TransparentDisplay
        {
            get => transparentDisplay;
            set => transparentDisplay = value;
        }

        private DisplayType displayType;

        private void Start()
        {
            displayType = GetDisplayType();
            UpdateCameraSettings(displayType);
        }

        private static readonly ProfilerMarker UpdatePerfMarker =
            new ProfilerMarker("[MRTK] CameraSettingsManager.Update");

        private void Update()
        {
            using (UpdatePerfMarker.Auto())
            {
                // Check to see if opacity has changed and if so, update the
                // camera settings.
                DisplayType type = GetDisplayType();
                if (type != displayType)
                {
                    UpdateCameraSettings(type);
                    displayType = type;
                }
            }
        }

        private static readonly ProfilerMarker UpdateCameraSettingsPerfMarker =
            new ProfilerMarker("[MRTK] CameraSettingsManager.UpdateCameraSettings");

        /// <summary>
        /// Applies camera settings based on display opacity.
        /// </summary>
        /// <param name="type">
        /// The type of the display (ex: <see cref="DisplayType.Opaque"/> for VR devices.
        /// </param>
        private void UpdateCameraSettings(DisplayType type)
        {
            using (UpdateCameraSettingsPerfMarker.Auto())
            {
                UnityEngine.Camera mainCamera = Camera.main;

                switch (type)
                {
                    case DisplayType.Opaque:
                        mainCamera.clearFlags = opaqueDisplay.ClearMode;
                        mainCamera.backgroundColor = opaqueDisplay.ClearColor;
                        mainCamera.nearClipPlane = opaqueDisplay.NearPlaneDistance;
                        mainCamera.farClipPlane = opaqueDisplay.FarPlaneDistance;
                        if (opaqueDisplay.AdjustQualityLevel)
                        {
                            QualitySettings.SetQualityLevel(opaqueDisplay.QualityLevel, false); // do not apply expensive changes
                        }
                        break;

                    case DisplayType.Transparent:
                        mainCamera.clearFlags = transparentDisplay.ClearMode;
                        mainCamera.backgroundColor = transparentDisplay.ClearColor;
                        mainCamera.nearClipPlane = transparentDisplay.NearPlaneDistance;
                        mainCamera.farClipPlane = transparentDisplay.FarPlaneDistance;
                        if (transparentDisplay.AdjustQualityLevel)
                        {
                            QualitySettings.SetQualityLevel(transparentDisplay.QualityLevel, false); // do not apply expensive changes
                        }
                        break;

                    default:
#if !UNITY_EDITOR
                        Debug.LogWarning($"Unknown DisplayType value: {type}. No camera settings changes made.");
#endif
                        break;
                }
            }
        }

        private static readonly ProfilerMarker GetDisplayTypePerfMarker =
            new ProfilerMarker("[MRTK] CameraSettingsManager.GetDisplayType");

        /// <summary>
        /// Determines the type of the display (ex: transparent).
        /// </summary>
        /// <returns>
        /// <see cref="DisplayType"/> value describing the display.
        /// </returns>
        /// <remarks>
        /// If it is not possible to determine the type of display at the time GetDisplayType is called,
        /// <see cref="DisplayType.Transparent"/> will be returned.
        /// </remarks>
        private DisplayType GetDisplayType()
        {
            using (GetDisplayTypePerfMarker.Auto())
            {
                if (XRSubsystemHelpers.DisplaySubsystem == null) { return DisplayType.Unknown; }
                return XRSubsystemHelpers.DisplaySubsystem.displayOpaque ? DisplayType.Opaque : DisplayType.Transparent;
            }
        }
    }
}
