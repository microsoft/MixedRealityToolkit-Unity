// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEngine;

using Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.Utilities;

namespace Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.Editor
{
    [CustomEditor(typeof(PlatformSwitcher))]
    public class PlatformSwitcherEditor : UnityEditor.Editor
    {
        const float _buttonHeight = 30;

        public override void OnInspectorGUI()
        {
            GUILayout.BeginVertical();

            // Editor button for HoloLens platform and functionality
            if (GUILayout.Button("HoloLens", GUILayout.Height(_buttonHeight)))
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WSA, BuildTarget.WSAPlayer);
                PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.InternetClientServer, true);
                PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.PrivateNetworkClientServer, true);
                PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.WebCam, true);
                PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.SpatialPerception, true);
            }

            // Editor button for Android platform and functionality
            if (GUILayout.Button("Android", GUILayout.Height(_buttonHeight)))
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
                PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
            }

            // Editor button for iOS platform and functionality
            if (GUILayout.Button("iOS", GUILayout.Height(_buttonHeight)))
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);
                PlayerSettings.iOS.cameraUsageDescription = "Camera required for AR Foundation";
            }

            GUILayout.EndVertical();
        }
    }
}
