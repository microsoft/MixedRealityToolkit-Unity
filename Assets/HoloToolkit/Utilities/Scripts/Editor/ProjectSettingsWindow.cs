// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Renders the UI and handles update logic for HoloToolkit/Configure/Apply HoloLens Project Settings.
    /// </summary>
    public class ProjectSettingsWindow : AutoConfigureWindow<ProjectSettingsWindow.ProjectSetting>
    {
        private const string InputManagerAssetURL = "https://raw.githubusercontent.com/Microsoft/MixedRealityToolkit-Unity/master/ProjectSettings/InputManager.asset";

        #region Nested Types

        public enum ProjectSetting
        {
            BuildWsaUwp,
            WsaEnableVR,
            WsaUwpBuildToD3D,
            WsaFastestQuality,
            SharingServices,
            XboxControllerSupport
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
#if UNITY_2017_1_OR_NEWER
                    EditorUserBuildSettings.SwitchActiveBuildTargetAsync(BuildTargetGroup.WSA, BuildTarget.WSAPlayer);
#else
                    EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WSA, BuildTarget.WSAPlayer);
#endif
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
            for (int i = (int)ProjectSetting.BuildWsaUwp; i <= (int)ProjectSetting.XboxControllerSupport; i++)
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
                    case ProjectSetting.XboxControllerSupport:
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
                for (var i = 0; i < QualitySettings.names.Length; i++)
                {
                    QualitySettings.DecreaseLevel(true);
                }

                int currentQualityLevel = QualitySettings.GetQualityLevel();

                // HACK: Edits QualitySettings.asset Directly
                // TODO: replace with friendlier version that uses built in APIs when Unity fixes or makes available.
                // See: http://answers.unity3d.com/questions/886160/how-do-i-change-qualitysetting-for-my-platform-fro.html
                try
                {
                    // Find the WSA element under the platform quality list and replace it's value with the current level.
                    string settingsPath = "ProjectSettings/QualitySettings.asset";
                    string matchPattern = @"(m_PerPlatformDefaultQuality.*Windows Store Apps:) (\d+)";
                    string replacePattern = @"$1 " + currentQualityLevel;

                    string settings = File.ReadAllText(settingsPath);
                    settings = Regex.Replace(settings, matchPattern, replacePattern, RegexOptions.Singleline);

                    File.WriteAllText(settingsPath, settings);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            UnityEditorInternal.VR.VREditor.SetVREnabledOnTargetGroup(BuildTargetGroup.WSA, Values[ProjectSetting.WsaEnableVR]);
            if (Values[ProjectSetting.WsaEnableVR])
            {
                EditorUserBuildSettings.wsaSubtarget = WSASubtarget.HoloLens;
                UnityEditorInternal.VR.VREditor.SetVREnabledDevicesOnTargetGroup(BuildTargetGroup.WSA, new[] { "HoloLens" });
                PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.HumanInterfaceDevice, Values[ProjectSetting.XboxControllerSupport]);
            }
            else
            {
                EditorUserBuildSettings.wsaSubtarget = WSASubtarget.AnyDevice;
                PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.HumanInterfaceDevice, false);
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

            var inputManagerPath = Directory.GetParent(Path.GetFullPath(Application.dataPath)).FullName + "\\ProjectSettings\\InputManager.asset";
            bool userPermission = Values[ProjectSetting.XboxControllerSupport];

            if (userPermission)
            {
                userPermission = EditorUtility.DisplayDialog("Attention!",
                    "Hi there, we noticed that you've enabled the Xbox Controller support.\n\n" +
                    "Do you give us permission to download the latest input mapping definitions from " +
                    "the Mixed Reality Toolkit's GitHub page and replace your project's InputManager.asset?\n\n",
                    "OK", "Cancel");


                if (userPermission)
                {
                    try
                    {
                        using (var webRequest = UnityWebRequest.Get(InputManagerAssetURL)
                        )
                        {
                            webRequest.Send();

                            while (!webRequest.isDone)
                            {
                                if (webRequest.downloadProgress != -1)
                                {
                                    EditorUtility.DisplayProgressBar("Downloading InputManager.asset from GitHub",
                                        "Progress...", webRequest.downloadProgress);
                                }
                            }
                            EditorUtility.ClearProgressBar();

#if UNITY_2017_1_OR_NEWER
                            if (webRequest.isNetworkError || webRequest.isHttpError)
#else
                            if (webRequest.isError)
#endif
                            {
                                throw new UnityException("Network Error: " + webRequest.error);
                            }

                            File.Copy(inputManagerPath, inputManagerPath + ".old", true);
                            File.WriteAllText(inputManagerPath, webRequest.downloadHandler.text);
                        }
                    }
                    catch (Exception)
                    {
                        Close();
                        throw;
                    }
                }
            }

            if (!userPermission)
            {
                Values[ProjectSetting.XboxControllerSupport] = false;
                if (File.Exists(inputManagerPath + ".old"))
                {
                    File.Copy(inputManagerPath + ".old", inputManagerPath, true);
                    File.Delete(inputManagerPath + ".old");
                    Debug.Log("Previous Input Mapping Restored.");
                }
                else
                {
                    Debug.LogWarning("No old Input Mapping found!");
                }
            }
            EditorPrefsUtility.SetEditorPref(Names[ProjectSetting.XboxControllerSupport], Values[ProjectSetting.XboxControllerSupport]);

            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
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

            Names[ProjectSetting.XboxControllerSupport] = "Enable Xbox Controller Support";
            Descriptions[ProjectSetting.XboxControllerSupport] = "Enables the use of Xbox Controller support for all UWP apps.\n\n" +
                                                                 "<color=#ff0000ff><b>Warning!</b></color> Enabling this feature will copy your old InputManager.asset " +
                                                                 "and append it with \".old\".  To revert simply disable Xbox Controller Support.\n\n" +
                                                                 "<color=#ffff00ff><b>Note:</b></color> ONLY the HoloLens platform target requires the HID capabilities.  This capability is automatically " +
                                                                 "enabled for you if you enable Xbox Controller Support and enable VR and target the HoloLens device.";
        }

        protected override void OnEnable()
        {
            // Pass to base first
            base.OnEnable();

#if UNITY_2017_1_OR_NEWER
            AutoConfigureMenu.ActiveBuildTargetChanged += UpdateSettings;
#endif

            // Set size
            minSize = new Vector2(350, 350);
            maxSize = minSize;
        }

        #endregion // Overrides / Event Handlers
    }
}