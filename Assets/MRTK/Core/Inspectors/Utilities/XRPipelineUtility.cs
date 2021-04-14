// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    /// <summary>
    /// Provides MRTK UI helpers for supporting multiple XR pipelines in one profile.
    /// </summary>
    internal class XRPipelineUtility
    {
#if UNITY_2019
        private const string LegacyXRLabel = "Legacy XR";
        private const string XRSDKLabel = "XR SDK";
        private static readonly string[] Tabs = new string[] { LegacyXRLabel, XRSDKLabel };
        private int tab = 0;
#endif // UNITY_2019

        public SupportedUnityXRPipelines SelectedPipeline { get; private set; } =
#if UNITY_2019_3_OR_NEWER
            SupportedUnityXRPipelines.XRSDK;
#else
            SupportedUnityXRPipelines.LegacyXR;
#endif // UNITY_2019_3_OR_NEWER

#if UNITY_2019
        /// <summary>
        /// Call this in the inspector's OnEnable to properly set the default tab.
        /// </summary>
        public void Enable()
        {
            tab = XRSettingsUtilities.IsLegacyXRActive ? 0 : 1;
        }

        /// <summary>
        /// Renders two tabs, one for XR SDK and one for legacy XR. This allows the profile to support both pipelines at once.
        /// </summary>
        /// <remarks>This is only needed for Unity 2019, since that's the only version where these two XR pipelines exist together.</remarks>
        public void RenderXRPipelineTabs()
        {
            // The tabs should always be enabled. They're only used for visualization, not settings.
            using (new GUIEnabledWrapper())
            {
                tab = GUILayout.Toolbar(tab, Tabs);
                SelectedPipeline = Tabs[tab] == XRSDKLabel ? SupportedUnityXRPipelines.XRSDK : SupportedUnityXRPipelines.LegacyXR;

                switch (SelectedPipeline)
                {
                    case SupportedUnityXRPipelines.LegacyXR:
                        if (!XRSettingsUtilities.IsLegacyXRActive)
                        {
                            EditorGUILayout.HelpBox("Legacy XR is not active, so these data providers will not be loaded at runtime.", MessageType.Info);
                        }
                        break;
                    case SupportedUnityXRPipelines.XRSDK:
                        if (XRSettingsUtilities.IsLegacyXRActive)
                        {
                            EditorGUILayout.HelpBox("XR SDK is not active, so these data providers will not be loaded at runtime.", MessageType.Info);
                        }
                        break;
                }
            }
        }
#endif // UNITY_2019
    }
}
