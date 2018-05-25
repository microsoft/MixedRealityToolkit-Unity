// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.using UnityEngine;

using UnityEngine;
using UnityEditor;

namespace HoloToolkit.Unity.Preview.SpectatorView
{
    [CustomEditor(typeof(PlatformSwitcher))]
    public class PlatformSwitcherEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            PlatformSwitcher platformSwitcher = (PlatformSwitcher)target;

            GUILayout.BeginHorizontal();

            // Editor button for HoloLens platfrom and functionality
            if (GUILayout.Button("Hololens", GUILayout.Height(70)))
            {
                platformSwitcher.SwitchPlatform(PlatformSwitcher.Platform.Hololens);
                serializedObject.FindProperty("targetPlatform").enumValueIndex = (int)PlatformSwitcher.Platform.Hololens;
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WSA, BuildTarget.WSAPlayer);
                PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.InternetClientServer, true);
                PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.PrivateNetworkClientServer, true);
                PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.WebCam, true);
                PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.SpatialPerception, true);
            }

            // Editor button for iOS platfrom and functionality
            if (GUILayout.Button("IPhone", GUILayout.Height(70)))
            {
                platformSwitcher.SwitchPlatform(PlatformSwitcher.Platform.IPhone);
                serializedObject.FindProperty("targetPlatform").enumValueIndex = (int)PlatformSwitcher.Platform.IPhone;
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);
                PlayerSettings.iOS.cameraUsageDescription = "Camera required for ARKit";
            }

            GUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
