// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEngine;

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.Utilities;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.Editor
{
    /// <summary>
    /// Defines functionality for switching platforms in the Unity editor
    /// </summary>
    [CustomEditor(typeof(PlatformSwitcher))]
    public class PlatformSwitcherEditor : UnityEditor.Editor
    {
        private readonly float _buttonHeight = 30;

        public override void OnInspectorGUI()
        {
            GUILayout.BeginVertical();

            // Editor button for HoloLens platform and functionality
            if (GUILayout.Button("HoloLens", GUILayout.Height(_buttonHeight)))
            {
                EditorUserBuildSettings.wsaArchitecture = "x86";
                EditorUserBuildSettings.wsaSubtarget = WSASubtarget.HoloLens;
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WSA, BuildTarget.WSAPlayer);
                PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.InternetClient, true);
                PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.InternetClientServer, true);
                PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.PrivateNetworkClientServer, true);
                PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.WebCam, true);
                PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.Microphone, true);
                PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.PicturesLibrary, true);
                PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.VideosLibrary, true);
                PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.SpatialPerception, true);
                PlayerSettings.WSA.SetTargetDeviceFamily(PlayerSettings.WSATargetFamily.Holographic, true);
            }

            // Editor button for Android platform and functionality
            if (GUILayout.Button("Android", GUILayout.Height(_buttonHeight)))
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
                EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
                PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
                PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait; // Currently needed based on Marker Visual logic
            }

            // Editor button for iOS platform and functionality
            if (GUILayout.Button("iOS", GUILayout.Height(_buttonHeight)))
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);
                PlayerSettings.iOS.cameraUsageDescription = "Camera required for AR Foundation";
                PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait; // Currently needed based on Marker Visual logic
                PlayerSettings.SetArchitecture(BuildTargetGroup.iOS, 1); // Set Architecture to ARM64
            }

            GUILayout.EndVertical();
        }
    }
}
