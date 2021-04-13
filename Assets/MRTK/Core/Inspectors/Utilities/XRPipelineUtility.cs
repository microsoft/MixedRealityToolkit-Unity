// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
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
        public void Enable()
        {
            tab = XRSettingsUtilities.IsLegacyXRActive ? 0 : 1;
        }

        public void RenderXRPipelineTabs()
        {
            tab = GUILayout.Toolbar(tab, Tabs);
            SelectedPipeline = Tabs[tab] == XRSDKLabel ? SupportedUnityXRPipelines.XRSDK : SupportedUnityXRPipelines.LegacyXR;

            switch (SelectedPipeline)
            {
                case SupportedUnityXRPipelines.LegacyXR:
                    if (!XRSettingsUtilities.IsLegacyXRActive)
                    {
                        EditorGUILayout.HelpBox("Legacy XR is not active, these data providers will not be loaded at runtime", MessageType.Info);
                    }
                    break;
                case SupportedUnityXRPipelines.XRSDK:
                    if (XRSettingsUtilities.IsLegacyXRActive)
                    {
                        EditorGUILayout.HelpBox("XR SDK is not active, these data providers will not be loaded at runtime", MessageType.Info);
                    }
                    break;
            }
        }
#endif // UNITY_2019
    }
}
