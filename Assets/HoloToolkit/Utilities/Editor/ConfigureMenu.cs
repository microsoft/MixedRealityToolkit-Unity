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
    /// Configuration options derived from here: 
    /// https://developer.microsoft.com/en-us/windows/holographic/unity_development_overview#Configuring_a_Unity_project_for_HoloLens
    /// </summary>
    public static class AutoConfigureMenu
    {
        [MenuItem("HoloToolkit/Configure/Show Help", priority = 1)]
        public static void ShowHelp()
        {
            Application.OpenURL("https://developer.microsoft.com/en-us/windows/holographic/unity_development_overview#Configuring_a_Unity_project_for_HoloLens");
        }

        /// <summary>
        /// Applies recommended scene settings to the current scenes
        /// </summary>
        [MenuItem("HoloToolkit/Configure/Apply HoloLens Scene Settings", priority = 0)]
        public static void ApplySceneSettings()
        {
            if (Camera.main == null)
            {
                Debug.LogWarning(@"Could not apply settings - no camera tagged with ""MainCamera""");
                return;
            }

            Camera.main.transform.position = Vector3.zero;
            Camera.main.clearFlags = CameraClearFlags.SolidColor;
            Camera.main.backgroundColor = Color.clear;
            Camera.main.nearClipPlane = 0.85f;
            Camera.main.fieldOfView = 16.0f;
        }

        /// <summary>
        /// Applies recommended project settings to the current project
        /// </summary>
        [MenuItem("HoloToolkit/Configure/Apply HoloLens Project Settings", priority = 0)]
        public static void ApplyProjectSettings()
        {
            // Build settings
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.WSAPlayer);
            EditorUserBuildSettings.wsaSDK = WSASDK.UWP;
            EditorUserBuildSettings.wsaUWPBuildType = WSAUWPBuildType.D3D;

            // See the blow notes for why text asset serialization is required
            if (EditorSettings.serializationMode != SerializationMode.ForceText)
            {
                // NOTE: PlayerSettings.virtualRealitySupported would be ideal, except that it only reports/affects whatever platform tab
                // is currently selected in the Player settings window. As we don't have code control over what view is selected there
                // this property is fairly useless from script.

                // NOTE: There is no current way to change the default quality setting from script

                string title = "Updates require text serialization of assets";
                string message = "Unity doesn't provide apis for updating the default quality or enabling VR support.\n\n" +
                    "Is it ok if we force text serialization of assets so that we can modify the properties directly?";

                bool forceText = EditorUtility.DisplayDialog(title, message, "Yes", "No");
                if (!forceText)
                    return;

                EditorSettings.serializationMode = SerializationMode.ForceText;
            }

            SetFastestDefaultQuality();
            EnableVirtualReality();

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

                // Set VR for WSA to 1
                string matchPattern = @"(Metro::VR::enable:) (\d+)";
                string replacePattern = @"$1 1";
                settings = Regex.Replace(settings, matchPattern, replacePattern, RegexOptions.Singleline);

                // This is a bit more complex, we're looking for the WSA list of enabled VR devices, then
                // ensuring that the HoloLens is in that list
                bool foundDevices = false;
                bool foundHoloLens = false;

                var builder = new StringBuilder(); // Used to build the final output
                string[] lines = settings.Split(new char[] { '\n' });
                for (int i = 0; i < lines.Length; ++i)
                {
                    string line = lines[i];

                    // Look for the enabled Devices list
                    if (!foundDevices)
                    {
                        if (line.Contains("Metro::VR::enabledDevices:"))
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
                            // add the hololens element, and mark it found
                            builder.Append("  - HoloLens\n");
                            foundHoloLens = true;
                        }

                        // Otherwise test if this is the hololens device
                        else if (line.Contains("HoloLens"))
                        {
                            foundHoloLens = true;
                        }
                    }

                    builder.Append(line);

                    // Write out a \n for all but the last line
                    // NOTE: Specifically preserving unix line endings by avoiding StringBuilder.AppendLine
                    if (i != lines.Length - 1)
                        builder.Append('\n');
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
    }
}