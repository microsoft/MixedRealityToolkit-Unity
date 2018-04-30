// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.IO;
using UnityEngine;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Internal.Utilities.Editor.Setup
{
    /// <summary>
    /// Sets Force Text Serialization and visible meta files in all projects that use the Mixed Reality Toolkit.
    /// </summary>
    [InitializeOnLoad]
    public class EnforceEditorSettings
    {
        private const string SessionKey = "_MixedRealityToolkit_Editor_ShownSettingsPrompts";

        static EnforceEditorSettings()
        {
            if (!IsNewEditorSession())
            {
                return;
            }

            bool refresh = false;
            bool restart = false;

            if (EditorSettings.serializationMode != SerializationMode.ForceText)
            {
                if (EditorUtility.DisplayDialog(
                        "Force Text Asset Serialization?",
                        "The Mixed Reality Toolkit is easier to maintain if the asset serialization mode for this project is set to \"Force Text\". Would you like to make this change?",
                        "Force Text Serialization",
                        "Later"))
                {
                    EditorSettings.serializationMode = SerializationMode.ForceText;
                    Debug.Log("Setting Force Text Serialization");
                    refresh = true;
                }
            }

            if (!EditorSettings.externalVersionControl.Equals("Visible Meta Files"))
            {
                if (EditorUtility.DisplayDialog(
                    "Make Meta Files Visible?",
                    "The Mixed Reality Toolkit would like to make meta files visible so they can be more easily handled with common version control systems. Would you like to make this change?",
                    "Enable Visible Meta Files",
                    "Later"))
                {
                    EditorSettings.externalVersionControl = "Visible Meta Files";
                    Debug.Log("Updated external version control mode: " + EditorSettings.externalVersionControl);
                    refresh = true;
                }
            }

            if (PlayerSettings.scriptingRuntimeVersion != ScriptingRuntimeVersion.Latest)
            {
                if (EditorUtility.DisplayDialog(
                        "Change the Scripting Runtime Version to the 4.x Equivalent?",
                        "The Mixed Reality Toolkit would like to change the Scripting Runtime Version to use the .NET 4.x Equivalent.\n\n" +
                        "In order for the change to take place the Editor must be restarted, and any changes will be saved.\n\n" +
                        "WARNING: If you do not make this change, then your project will fail to compile.\n\n" +
                        "Would you like to make this change?",
                        "Enable .NET 4.x Equivalent",
                        "Later"))
                {
                    PlayerSettings.scriptingRuntimeVersion = ScriptingRuntimeVersion.Latest;
                    restart = true;
                }
            }

            if (PlayerSettings.scriptingRuntimeVersion == ScriptingRuntimeVersion.Latest)
            {
                var currentApiCompatibility = PlayerSettings.GetApiCompatibilityLevel(EditorUserBuildSettings.selectedBuildTargetGroup);
                if (currentApiCompatibility != ApiCompatibilityLevel.NET_4_6 && currentApiCompatibility != ApiCompatibilityLevel.NET_Standard_2_0)
                {
                    if (EditorUtility.DisplayDialog(
                            "Change the Scripting API Compatibility to .NET Standard 2.0?",
                            "The Mixed Reality Toolkit would like to change the Scripting API Compatibility to use .NET Standard 2.0.\n\n" +
                            "Would you like to make this change?",
                            "Enable .NET Standard 2.0",
                            "Later"))
                    {
                        PlayerSettings.SetApiCompatibilityLevel(EditorUserBuildSettings.selectedBuildTargetGroup, ApiCompatibilityLevel.NET_Standard_2_0);
                        Debug.Log("Updated Scripting API Compatibility to .NET Standard 2.0");
                        refresh = true;
                    }
                }
            }

            if (refresh || restart)
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            }

            if (restart)
            {
                EditorApplication.OpenProject(Directory.GetParent(Application.dataPath).ToString());
            }
        }

        /// <summary>
        /// Returns true the first time it is called within this editor session, and
        /// false for all subsequent calls.
        /// </summary>
        private static bool IsNewEditorSession()
        {
            if (!SessionState.GetBool(SessionKey, false))
            {
                SessionState.SetBool(SessionKey, true);
                return true;
            }

            return false;
        }
    }
}