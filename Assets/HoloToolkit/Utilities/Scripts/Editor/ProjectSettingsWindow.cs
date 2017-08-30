// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEditor;
using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Renders the UI and handles update logic for HoloToolkit/Configure/Apply HoloLens Project Settings.
    /// </summary>
    public class ProjectSettingsWindow : AutoConfigureWindow<ProjectSettingsWindow.ProjectSetting>
    {
        #region Nested Types

        public enum ProjectSetting
        {
            BuildWsaUwp,
            WsaEnableVR,
            WsaUwpBuildToD3D,
            WsaFastestQuality,
            SharingServices
        }

        #endregion // Nested Types

        #region Overrides / Event Handlers

        protected override void ApplySettings()
        {
            // Apply individual settings
            if (Values[ProjectSetting.BuildWsaUwp])
            {
                if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.WSAPlayer)
                {
                    EditorUserBuildSettings.SwitchActiveBuildTargetAsync(BuildTargetGroup.WSA, BuildTarget.WSAPlayer);
                }
                else
                {
                    UpdateSettings(EditorUserBuildSettings.activeBuildTarget);
                }
            }
            else
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
            }
        }

        protected override void LoadSettings()
        {
            for (int i = (int)ProjectSetting.BuildWsaUwp; i <= (int)ProjectSetting.SharingServices; i++)
            {
                switch ((ProjectSetting)i)
                {
                    case ProjectSetting.BuildWsaUwp:
                    case ProjectSetting.WsaEnableVR:
                    case ProjectSetting.WsaUwpBuildToD3D:
                    case ProjectSetting.WsaFastestQuality:
                        Values[(ProjectSetting)i] = true;
                        break;
                    case ProjectSetting.SharingServices:
                        Values[(ProjectSetting)i] = EditorPrefsUtility.GetEditorPref(Names[(ProjectSetting)i], false);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void UpdateSettings(BuildTarget currentBuildTarget)
        {
            if (currentBuildTarget != BuildTarget.WSAPlayer) { return; }

            EditorUserBuildSettings.wsaUWPBuildType = Values[ProjectSetting.WsaUwpBuildToD3D]
                ? WSAUWPBuildType.D3D
                : WSAUWPBuildType.XAML;

            if (Values[ProjectSetting.WsaFastestQuality])
            {
                int settingId = -1;
                for (var i = 0; i < QualitySettings.names.Length; i++)
                {
                    if (QualitySettings.names[i].Equals("Fastest"))
                    {
                        QualitySettings.SetQualityLevel(i);
                        settingId = i;
                        break;
                    }
                }

                if (settingId < 0)
                {
                    Debug.LogWarning("Failed to find Quality Setting 'Fastest'");
                }
            }

            UnityEditorInternal.VR.VREditor.SetVREnabledOnTargetGroup(BuildTargetGroup.WSA, Values[ProjectSetting.WsaEnableVR]);
            if (Values[ProjectSetting.WsaEnableVR])
            {
                EditorUserBuildSettings.wsaSubtarget = WSASubtarget.HoloLens;
                UnityEditorInternal.VR.VREditor.SetVREnabledDevicesOnTargetGroup(BuildTargetGroup.WSA, new[] { "HoloLens" });
            }

            EditorPrefsUtility.SetEditorPref(Names[ProjectSetting.SharingServices], Values[ProjectSetting.SharingServices]);
            if (Values[ProjectSetting.SharingServices])
            {
                PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.InternetClientServer, true);
                PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.PrivateNetworkClientServer, true);
            }
            else
            {
                PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.InternetClient, false);
                PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.InternetClientServer, false);
                PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.PrivateNetworkClientServer, false);
            }

            Close();
        }

        protected override void OnGuiChanged()
        {
        }

        protected override void LoadStrings()
        {
            Names[ProjectSetting.BuildWsaUwp] = "Target Windows Store and UWP";
            Descriptions[ProjectSetting.BuildWsaUwp] = "Required\n\nSwitches the currently active target to produce a Store app targeting the Universal Windows Platform.\n\n" +
                                                       "Since HoloLens only supports Windows Store apps, this option should remain checked unless you plan to manually switch " +
                                                       "the target later before you build.";

            Names[ProjectSetting.WsaEnableVR] = "Enable VR and Target HoloLens Device";
            Descriptions[ProjectSetting.WsaEnableVR] = "Required\n\nEnables VR for Windows Store apps and adds the HoloLens as a target VR device.\n\n" +
                                                       "The application will not compile for HoloLens and tools like Holographic Remoting will not function " +
                                                       "without this enabled. Therefore this option should remain checked unless you plan to manually " +
                                                       "perform these steps later.";

            Names[ProjectSetting.WsaUwpBuildToD3D] = "Build for Direct3D";
            Descriptions[ProjectSetting.WsaUwpBuildToD3D] = "Recommended\n\nProduces an app that targets Direct3D instead of Xaml.\n\nPure Direct3D apps run " +
                                                            "faster than applications that include Xaml. This option should remain checked unless you plan to " +
                                                            "overlay Unity content with Xaml content or you plan to switch between Unity views and Xaml views at runtime.";

            Names[ProjectSetting.WsaFastestQuality] = "Set Quality to Fastest";
            Descriptions[ProjectSetting.WsaFastestQuality] = "Recommended\n\nChanges the quality settings for Windows Store apps to the 'Fastest' setting.\n\n" +
                                                             "'Fastest' is the recommended quality setting for HoloLens apps, but this option can be unchecked " +
                                                             "if you have already optimized your project for the HoloLens.";

            Names[ProjectSetting.SharingServices] = "Enable Sharing Services";
            Descriptions[ProjectSetting.SharingServices] = "Enables the use of the Sharing Services in your project.\n\n" +
                                                           "Requires the SpatialPerception, InternetClient, InternetClientServer, PrivateNetworkClientServer, " +
                                                           "and Microphone Capabilities.\n\n" +
                                                           "Start the Sharing Server though HoloToolkit->Sharing Service->Launch Sharing Service.";
        }

        protected override void OnEnable()
        {
            // Pass to base first
            base.OnEnable();

            AutoConfigureMenu.ActiveBuildTargetChanged += UpdateSettings;

            // Set size
            minSize = new Vector2(350, 350);
            maxSize = minSize;
        }

        #endregion // Overrides / Event Handlers
    }
}