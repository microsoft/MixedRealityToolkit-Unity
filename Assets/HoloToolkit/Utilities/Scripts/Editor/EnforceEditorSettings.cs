// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Sets Force Text Serialization and visible meta files in all projects that use the MixedRealityToolkit.
    /// </summary>
    [InitializeOnLoad]
    public class EnforceEditorSettings
    {
        private const string AssemblyReloadTimestampKey = "_LastAssemblyReload";

        static EnforceEditorSettings()
        {
            if (!IsNewEditorSession())
            {
                return;
            }

            bool hasUpdatedSettings = false;

            if (EditorSettings.serializationMode != SerializationMode.ForceText)
            {
                if (EditorUtility.DisplayDialog(
                        "Force Text Asset Serialization?",
                        "MixedRealityToolkit is easier to maintain if the asset serialization mode for this project is set to \"Force Text\". Would you like to make this change?",
                        "Force Text Serialization",
                        "Later"))
                {
                    EditorSettings.serializationMode = SerializationMode.ForceText;
                    Debug.Log("Setting Force Text Serialization");
                    hasUpdatedSettings = true;
                }
            }

            if (!EditorSettings.externalVersionControl.Equals("Visible Meta Files"))
            {
                if (EditorUtility.DisplayDialog(
                    "Make Meta Files Visible?",
                    "MixedRealityToolkit would like to make meta files visible so they can be more easily handled with common version control systems. Would you like to make this change?",
                    "Enable Visible Meta Files",
                    "Later"))
                {
                    EditorSettings.externalVersionControl = "Visible Meta Files";
                    Debug.Log("Updated external version control mode: " + EditorSettings.externalVersionControl);
                    hasUpdatedSettings = true;
                }
            }

            if (!EditorPrefsUtility.GetEditorPref("_DepthBufferSharingEnabled", false))
            {
                if (EditorUtility.DisplayDialog(
                    "Enable Depth Buffer Sharing?",
                    "MixedRealityToolkit would like to enable the Depth Buffer Sharing in the Windows Mixed Reality SDK. Would you like to make this change?",
                    "Enable Depth Buffer Sharing",
                    "Later"))
                {
                    SetDepthBufferSharing();
                    Debug.Log("Enable Depth Buffer Sharing");
                    hasUpdatedSettings = true;
                }
            }

            if (hasUpdatedSettings)
            {
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            }
        }

        /// <summary>
        /// Returns true the first time it is called within this editor session, and
        /// false for all subsequent calls.
        /// </summary>
        /// <remarks>
        /// The Unity Editor does not provide a callback for when a project is opened.
        /// InitializeOnLoad is triggered for all assembly reloads, including entering
        /// and exiting PlayMode, and whenever a script is modified and recompiled.
        ///
        /// To ensure execution only when opening a project in a new instance of the editor,
        /// store a timestamp in the editor key-value store whenever this function is called. 
        /// The stored timestamp is then compared with the true start time of this editor
        /// instance.
        /// </remarks>
        private static bool IsNewEditorSession()
        {
            // Determine the launch date for this editor session using the current time, and the time since startup.
            DateTime thisLaunchDate = DateTime.UtcNow.AddSeconds(-EditorApplication.timeSinceStartup);

            // Determine the last known launch date of the editor by loading it from the PlayerPrefs cache.
            // If no key exists set the time to this session.
            string dateString = EditorPrefsUtility.GetEditorPref(AssemblyReloadTimestampKey, thisLaunchDate.ToString(CultureInfo.InvariantCulture));

            DateTime lastLaunchDate;
            DateTime.TryParse(dateString, out lastLaunchDate);

            // If the current session was launched later than the last known session start date, then this must be 
            // a new session, and we can display the first-time prompt.
            if ((thisLaunchDate - lastLaunchDate).Seconds > 0)
            {
                EditorPrefsUtility.SetEditorPref(AssemblyReloadTimestampKey, dateString);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Enabled DepthBufferSharing for Windows Mixed Reality SDK found in XR Settings in the Player Settings Window.
        /// </summary>
        public static void SetDepthBufferSharing()
        {
            // HACK: Edits ProjectSettings.asset Directly
            // TODO: replace with friendlier version that uses built in APIs when Unity fixes or makes available.
            try
            {
                // Enable the depth buffer sharing.
                string settingsPath = "ProjectSettings/ProjectSettings.asset";
                string matchPattern = @"(depthBufferSharingEnabled:) (\d+)";
                string replacement = @"$1 1";

                string settings = File.ReadAllText(settingsPath);
                settings = Regex.Replace(settings, matchPattern, replacement, RegexOptions.Singleline);

                File.WriteAllText(settingsPath, settings);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            EditorPrefsUtility.SetEditorPref("_DepthBufferSharingEnabled", true);
        }
    }
}