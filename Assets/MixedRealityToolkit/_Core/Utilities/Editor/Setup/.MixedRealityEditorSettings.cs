// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;

namespace Microsoft.MixedReality.Toolkit.Core.Utilities.Editor.Setup
{
    /// <summary>
    /// Sets Force Text Serialization and visible meta files in all projects that use the Mixed Reality Toolkit.
    /// </summary>
    [InitializeOnLoad]
    public class MixedRealityEditorSettings : IActiveBuildTargetChanged
    {
        public MixedRealityEditorSettings()
        {
            callbackOrder = 0;
        }

        private const string SessionKey = "_MixedRealityToolkit_Editor_ShownSettingsPrompts";

        private static string mixedRealityToolkit_RelativeFolderPath = string.Empty;

        public static string MixedRealityToolkit_AbsoluteFolderPath
        {
            get
            {
                if (string.IsNullOrEmpty(mixedRealityToolkit_RelativeFolderPath))
                {
                    if (!FindDirectory(Application.dataPath, "MixedRealityToolkit", out mixedRealityToolkit_RelativeFolderPath))
                    {
                        Debug.LogError("Unable to find the Mixed Reality Toolkit's directory!");
                    }
                }

                return mixedRealityToolkit_RelativeFolderPath;
            }
        }

        public static string MixedRealityToolkit_RelativeFolderPath
        {
            get { return MixedRealityToolkit_AbsoluteFolderPath.Replace(Application.dataPath + "\\", "Assets/"); }
        }

        static MixedRealityEditorSettings()
        {
            if (!IsNewSession || Application.isPlaying)
            {
                return;
            }

            SetIconTheme();

            bool refresh = false;
            bool restart = false;

            if (EditorSettings.serializationMode != SerializationMode.ForceText ||
                PlayerSettings.GetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup) != ScriptingImplementation.IL2CPP ||
                !EditorSettings.externalVersionControl.Equals("Visible Meta Files") ||
                !PlayerSettings.virtualRealitySupported)
            {
                if (EditorUtility.DisplayDialog(
                        "Apply Mixed Reality Toolkit Default Settings?",
                        "The Mixed Reality Toolkit needs to apply the following settings to your project:\n\n" +
                        "- Enable XR Settings for your current platform\n" +
                        "- Force Text Serialization\n" +
                        "- Visible meta files\n" +
                        "- Change the Scripting Backend to use IL2CPP\n\n" +
                        "Would you like to make this change?",
                        "Apply",
                        "Later"))
                {
                    EditorSettings.serializationMode = SerializationMode.ForceText;
                    EditorSettings.externalVersionControl = "Visible Meta Files";
                    PlayerSettings.SetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup, ScriptingImplementation.IL2CPP);
                    PlayerSettings.virtualRealitySupported = true;
                    refresh = true;
                }
            }

            if (PlayerSettings.scriptingRuntimeVersion != ScriptingRuntimeVersion.Latest)
            {
                PlayerSettings.scriptingRuntimeVersion = ScriptingRuntimeVersion.Latest;
                restart = true;
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
        /// Returns true the first time it is called within this editor session, and false for all subsequent calls.
        /// <remarks>A new session is also true if the editor build target group is changed.</remarks>
        /// </summary>
        private static bool IsNewSession
        {
            get
            {
                if (SessionState.GetBool(SessionKey, false)) { return false; }

                SessionState.SetBool(SessionKey, true);
                return true;
            }
        }

        private static bool FindDirectory(string directoryPathToSearch, string directoryName, out string path)
        {
            path = string.Empty;

            var directories = Directory.GetDirectories(directoryPathToSearch);

            for (int i = 0; i < directories.Length; i++)
            {
                var name = Path.GetFileName(directories[i]);

                if (name != null && name.Equals(directoryName))
                {
                    path = directories[i];
                    return true;
                }

                if (FindDirectory(directories[i], directoryName, out path))
                {
                    return true;
                }
            }

            return false;
        }

        private static void SetIconTheme()
        {
            if (string.IsNullOrEmpty(MixedRealityToolkit_AbsoluteFolderPath))
            {
                Debug.LogError("Unable to find the Mixed Reality Toolkit's directory!");
                return;
            }

            var icons = Directory.GetFiles(MixedRealityToolkit_AbsoluteFolderPath + "/_Core/Resources/Icons");
            var icon = new Texture2D(2, 2);

            for (int i = 0; i < icons.Length; i++)
            {
                icons[i] = icons[i].Replace("/", "\\");
                if (icons[i].Contains("mixed_reality_icon") || icons[i].Contains(".meta")) { continue; }

                var imageData = File.ReadAllBytes(icons[i]);
                icon.LoadImage(imageData, false);

                var pixels = icon.GetPixels();
                for (int j = 0; j < pixels.Length; j++)
                {
                    pixels[j].r = EditorGUIUtility.isProSkin ? 1f : 0f;
                    pixels[j].g = EditorGUIUtility.isProSkin ? 1f : 0f;
                    pixels[j].b = EditorGUIUtility.isProSkin ? 1f : 0f;
                }

                icon.SetPixels(pixels);
                File.WriteAllBytes(icons[i], icon.EncodeToPNG());
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }

        /// <inheritdoc />
        public int callbackOrder { get; private set; }

        /// <inheritdoc />
        public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
        {
            SessionState.SetBool(SessionKey, false);
        }
    }
}