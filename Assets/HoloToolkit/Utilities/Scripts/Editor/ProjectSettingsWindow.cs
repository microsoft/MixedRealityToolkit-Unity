// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
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
            WsaUwpBuildToD3D,
            WsaFastestQuality,
            WsaEnableVR,
            SharingServices
        }
        #endregion // Nested Types

        #region Internal Methods
        /// <summary>
        /// Enables virtual reality for WSA and ensures HoloLens is in the supported SDKs.
        /// </summary>
        private static void EnableVirtualReality()
        {
            try
            {
                // Grab the text from the project settings asset file
                string settingsPath = "ProjectSettings/ProjectSettings.asset";
                string settings = File.ReadAllText(settingsPath);

                // We're looking for the list of VR devices for the current build target, then
                // ensuring that the HoloLens is in that list
                bool foundBuildTargetVRSettings = false;
                bool foundBuildTargetMetro = false;
                bool foundBuildTargetEnabled = false;
                bool foundDevices = false;
                bool foundHoloLens = false;

                StringBuilder builder = new StringBuilder(); // Used to build the final output
                string[] lines = settings.Split(new char[] { '\n' });
                for (int i = 0; i < lines.Length; ++i)
                {
                    string line = lines[i];

                    // Look for the build target VR settings
                    if (!foundBuildTargetVRSettings)
                    {
                        if (line.Contains("m_BuildTargetVRSettings:"))
                        {
                            // If no targets are enabled at all, just create the known entries and skip the rest of the tests
                            if (line.Contains("[]"))
                            {
                                // Remove the empty array symbols
                                line = line.Replace(" []", "\n");

                                // Generate the new lines
                                line += "  - m_BuildTarget: Metro\n";
                                line += "    m_Enabled: 1\n";
                                line += "    m_Devices:\n";
                                line += "    - HoloLens";

                                // Mark all fields as found so we don't search anymore
                                foundBuildTargetVRSettings = true;
                                foundBuildTargetMetro = true;
                                foundBuildTargetEnabled = true;
                                foundDevices = true;
                                foundHoloLens = true;
                            }
                            else
                            {
                                // The target VR settings were found but the others
                                // still need to be searched for.
                                foundBuildTargetVRSettings = true;
                            }
                        }
                    }

                    // Look for the build target for Metro
                    else if (!foundBuildTargetMetro)
                    {
                        if (line.Contains("m_BuildTarget: Metro"))
                        {
                            foundBuildTargetMetro = true;
                        }
                    }

                    else if (!foundBuildTargetEnabled)
                    {
                        if (line.Contains("m_Enabled"))
                        {
                            line = "    m_Enabled: 1";
                            foundBuildTargetEnabled = true;
                        }
                    }

                    // Look for the enabled Devices list
                    else if (!foundDevices)
                    {
                        if (line.Contains("m_Devices:"))
                        {
                            // Clear the empty array symbols if any
                            line = line.Replace(" []", "");
                            foundDevices = true;
                        }
                    }

                    // Once we've found the list look for HoloLens or the next non element
                    else if (!foundHoloLens)
                    {
                        // If this isn't an element in the device list
                        if (!line.Contains("-"))
                        {
                            // add the HoloLens element, and mark it found
                            builder.Append("    - HoloLens\n");
                            foundHoloLens = true;
                        }

                        // Otherwise test if this is the HoloLens device
                        else if (line.Contains("HoloLens"))
                        {
                            foundHoloLens = true;
                        }
                    }

                    builder.Append(line);

                    // Write out a \n for all but the last line
                    // NOTE: Specifically preserving unix line endings by avoiding StringBuilder.AppendLine
                    if (i != lines.Length - 1)
                    {
                        builder.Append('\n');
                    }
                }

                // Capture the final string
                settings = builder.ToString();

                File.WriteAllText(settingsPath, settings);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        /// <summary>
        /// Modifies the WSA default quality setting to the fastest
        /// </summary>
        private static void SetFastestDefaultQuality()
        {
            try
            {
                // Find the WSA element under the platform quality list and replace it's value with 0
                string settingsPath = "ProjectSettings/QualitySettings.asset";
                string matchPattern = @"(m_PerPlatformDefaultQuality.*Windows Store Apps:) (\d+)";
                string replacePattern = @"$1 0";

                string settings = File.ReadAllText(settingsPath);
                settings = Regex.Replace(settings, matchPattern, replacePattern, RegexOptions.Singleline);

                File.WriteAllText(settingsPath, settings);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        #endregion // Internal Methods

        #region Overrides / Event Handlers
        protected override void ApplySettings()
        {
            // Apply individual settings
            if (Values[ProjectSetting.BuildWsaUwp])
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WSA, BuildTarget.WSAPlayer);
            }

            if (Values[ProjectSetting.WsaUwpBuildToD3D])
            {
                EditorUserBuildSettings.wsaUWPBuildType = WSAUWPBuildType.D3D;
            }

            if (Values[ProjectSetting.WsaFastestQuality])
            {
                SetFastestDefaultQuality();
            }

            if (Values[ProjectSetting.WsaEnableVR])
            {
                EnableVirtualReality();
            }

            if (Values[ProjectSetting.SharingServices])
            {
                PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.InternetClient, true);
                PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.InternetClientServer, true);
                PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.PrivateNetworkClientServer, true);
            }
            else
            {
                PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.InternetClient, false);
                PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.InternetClientServer, false);
                PlayerSettings.WSA.SetCapability(PlayerSettings.WSACapability.PrivateNetworkClientServer, false);
            }

            // Since we went behind Unity's back to tweak some settings we 
            // need to reload the project to have them take effect
            bool canReload = EditorUtility.DisplayDialog(
                "Project reload required!",
                "Some changes require a project reload to take effect.\n\nReload now?",
                "Yes", "No");

            if (canReload)
            {
                string projectPath = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
                EditorApplication.OpenProject(projectPath);
            }
        }

        protected override void LoadSettings()
        {
            for (int i = (int)ProjectSetting.BuildWsaUwp; i <= (int)ProjectSetting.SharingServices; i++)
            {
                Values[(ProjectSetting)i] = true;
            }
        }

        protected override void LoadStrings()
        {
            Names[ProjectSetting.BuildWsaUwp] = "Target Windows Store and UWP";
            Descriptions[ProjectSetting.BuildWsaUwp] = "Required\n\nSwitches the currently active target to produce a Store app targeting the Universal Windows Platform.\n\nSince HoloLens only supports Windows Store apps, this option should remain checked unless you plan to manually switch the target later before you build.";

            Names[ProjectSetting.WsaUwpBuildToD3D] = "Build for Direct3D";
            Descriptions[ProjectSetting.WsaUwpBuildToD3D] = "Recommended\n\nProduces an app that targets Direct3D instead of Xaml.\n\nPure Direct3D apps run faster than applications that include Xaml. This option should remain checked unless you plan to overlay Unity content with Xaml content or you plan to switch between Unity views and Xaml views at runtime.";

            Names[ProjectSetting.WsaFastestQuality] = "Set Quality to Fastest";
            Descriptions[ProjectSetting.WsaFastestQuality] = "Recommended\n\nChanges the quality settings for Windows Store apps to the 'Fastest' setting.\n\n'Fastest' is the recommended quality setting for HoloLens apps, but this option can be unchecked if you have already optimized your project for the HoloLens.";

            Names[ProjectSetting.WsaEnableVR] = "Enable VR";
            Descriptions[ProjectSetting.WsaEnableVR] = "Required\n\nEnables VR for Windows Store apps and adds the HoloLens as a target VR device.\n\nThe application will not compile for HoloLens and tools like Holographic Remoting will not function without this enabled. Therefore this option should remain checked unless you plan to manually perform these steps later.";

            Names[ProjectSetting.SharingServices] = "Enable Sharing Services";
            Descriptions[ProjectSetting.SharingServices] = "Enables the use of the Sharing Services in your project.\n\n" +
                "Requires the SpatialPerception, InternetClient, InternetClientServer, PrivateNetworkClientServer, and Microphone Capabilities.\n\n" +
                "Start the Sharing Server though HoloToolkit->Sharing Service->Launch Sharing Service.";
        }

        protected override void OnEnable()
        {
            // Pass to base first
            base.OnEnable();

            // Set size
            minSize = new Vector2(350, 350);
            maxSize = minSize;
        }
        #endregion // Overrides / Event Handlers
    }
}