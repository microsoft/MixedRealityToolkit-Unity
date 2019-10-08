// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using MRConfig = MixedRealityProjectConfigurator.Configurations;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    public class MixedRealityProjectConfiguratorWindow : EditorWindow
    {

        private Dictionary<MRConfig, bool> trackToggles = new Dictionary<MRConfig, bool>()
        {
            {MRConfig.ForceTextSerialization, true },
            {MRConfig.VisibleMetaFiles, true },
            {MRConfig.VirtualRealitySupported, true },
            {MRConfig.SinglePassInstancing, true },
            {MRConfig.SpatialAwarenessLayer, true },
            {MRConfig.MicrophoneCapability, true },
            {MRConfig.InternetClientCapability, true },
            {MRConfig.SpatialPerceptionCapability, true },
            //{MRConfig.EyeTrackingCapability, true },
        };

        [MenuItem("Mixed Reality Toolkit/Utilities/Configurator Window", false, 0)]
        public static void OpenWindow()
        {
            var window = GetWindow<MixedRealityProjectConfiguratorWindow>();
            window.titleContent = new GUIContent("Project Configurator", EditorGUIUtility.IconContent("d_PreMatCube").image);
            window.Show();
        }

        public static MixedRealityProjectConfiguratorWindow Instance { get; private set; }

        public static bool IsOpen
        {
            get { return Instance != null; }
        }

        private void OnEnable()
        {
            Instance = this;
        }

        private void OnGUI()
        {
            MixedRealityInspectorUtility.RenderMixedRealityToolkitLogo();

            EditorGUILayout.LabelField("Project Settings", EditorStyles.boldLabel);

            RenderToggle(MRConfig.ForceTextSerialization, "Enable Force Text Serialization");
            RenderToggle(MRConfig.VisibleMetaFiles, "Enable Visible meta files");
            RenderToggle(MRConfig.VirtualRealitySupported, "Enable VR Supported");
            RenderToggle(MRConfig.SinglePassInstancing, "Set Single Pass Instanced rendering path");
            RenderToggle(MRConfig.SpatialAwarenessLayer, "Set Default Spatial Awareness Layer");

            EditorGUILayout.Space();

            if (MixedRealityOptimizeUtils.IsBuildTargetUWP())
            {
                EditorGUILayout.LabelField("UWP Capabilities", EditorStyles.boldLabel);

                RenderToggle(MRConfig.MicrophoneCapability, "Enable Microphone Capability");
                RenderToggle(MRConfig.InternetClientCapability, "Enable Internet Client Capability");
                //RenderToggle(MRConfig.EyeTrackingCapabilityKey, "Enable Eye Tracking Capability");
                RenderToggle(MRConfig.SpatialPerceptionCapability, "Enable Spatial Perception Capability");
            }
            else
            {
                trackToggles[MRConfig.MicrophoneCapability] = false;
                trackToggles[MRConfig.InternetClientCapability] = false;
                trackToggles[MRConfig.SpatialPerceptionCapability] = false;
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Apply Configuration"))
            {
                ApplyConfigurations();
            }
        }

        private void ApplyConfigurations()
        {
            var configurationFilter = new HashSet<MRConfig>();
            foreach (var item in trackToggles)
            {
                if (item.Value)
                {
                    configurationFilter.Add(item.Key);
                }
            }

            MixedRealityProjectConfigurator.ConfigureProject(configurationFilter);
        }

        private void RenderToggle(MRConfig configKey, string title)
        {
            bool configured = MixedRealityProjectConfigurator.IsConfigured(configKey);
            using (new EditorGUI.DisabledGroupScope(configured))
            {
                if (configured)
                {
                    EditorGUILayout.LabelField(new GUIContent(title, InspectorUIUtility.SuccessIcon));
                    trackToggles[configKey] = false;
                }
                else
                {
                    trackToggles[configKey] = EditorGUILayout.ToggleLeft(title, trackToggles[configKey]);
                }
            }
        }
    }
}