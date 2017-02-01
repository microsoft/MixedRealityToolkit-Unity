// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Renders the UI and handles update logic for HoloToolkit/Configure/Apply HoloLens Capability Settings.
    /// </summary>
    public class CapabilitySettingsWindow : AutoConfigureWindow<PlayerSettings.WSACapability>
    {
        #region Internal Methods
        private void ApplySetting(PlayerSettings.WSACapability setting)
        {
            PlayerSettings.WSA.SetCapability(setting, Values[setting]);
        }

        private void LoadSetting(PlayerSettings.WSACapability setting)
        {
            Values[setting] = PlayerSettings.WSA.GetCapability(setting);
        }
        #endregion // Internal Methods

        #region Overrides / Event Handlers
        protected override void ApplySettings()
        {
            ApplySetting(PlayerSettings.WSACapability.Microphone);
            ApplySetting(PlayerSettings.WSACapability.SpatialPerception);
            ApplySetting(PlayerSettings.WSACapability.WebCam);
            ApplySetting(PlayerSettings.WSACapability.InternetClient);
        }

        protected override void LoadSettings()
        {
            LoadSetting(PlayerSettings.WSACapability.Microphone);
            LoadSetting(PlayerSettings.WSACapability.SpatialPerception);
            LoadSetting(PlayerSettings.WSACapability.WebCam);
            LoadSetting(PlayerSettings.WSACapability.InternetClient);
        }

        protected override void LoadStrings()
        {
            Names[PlayerSettings.WSACapability.Microphone] = "Microphone";
            Descriptions[PlayerSettings.WSACapability.Microphone] = "Required for access to the HoloLens microphone. This includes behaviors like DictationRecognizer, GrammarRecognizer, and KeywordRecognizer. This capability is NOT required for the 'Select' keyword.\n\nRecommendation: Only enable if your application needs access to the microphone beyond the 'Select' keyword.The microphone is considered a privacy sensitive resource.";

            Names[PlayerSettings.WSACapability.SpatialPerception] = "Spatial Perception";
            Descriptions[PlayerSettings.WSACapability.SpatialPerception] = "Required for access to the HoloLens world mapping capabilities.These include behaviors like SurfaceObserver, SpatialMappingManager and SpatialAnchor.\n\nRecommendation: Enabled, unless your application doesn't use spatial mapping or spatial collisions in any way.";

            Names[PlayerSettings.WSACapability.WebCam] = "Webcam";
            Descriptions[PlayerSettings.WSACapability.WebCam] = "Required for access to the HoloLens RGB camera (also known as the locatable camera). This includes APIs like PhotoCapture and VideoCapture. This capability is NOT required for mixed reality streaming or for capturing photos or videos using the start menu.\n\nRecommendation: Only enable if your application needs to programmatically capture photos or videos from the RGB camera.The RGB camera is considered a privacy sensitive resource.";

            Names[PlayerSettings.WSACapability.InternetClient] = "Internet Client";
            Descriptions[PlayerSettings.WSACapability.InternetClient] = "Required if your application needs to access the Internet.\n\nRecommendation: Leave unchecked unless your application uses online services.";
        }

        protected override void OnEnable()
        {
            // Pass to base first
            base.OnEnable();

            // Set size
            minSize = new Vector2(350, 255);
            maxSize = minSize;
        }
        #endregion // Overrides / Event Handlers
    }
}