// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
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

        private const string IgnoreKey = "_MixedRealityToolkit_Editor_IgnoreSettingsPrompts";
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
            get { return MakePathRelativeToProject(MixedRealityToolkit_AbsoluteFolderPath); }
        }

        static MixedRealityEditorSettings()
        {
            if (!IsNewSession || Application.isPlaying)
            {
                return;
            }

            bool refresh = false;
            bool restart = false;

            var ignoreSettings = EditorPrefs.GetBool(IgnoreKey, false);

            if (!ignoreSettings)
            {
                var message = "The Mixed Reality Toolkit needs to apply the following settings to your project:\n\n";

                var forceTextSerialization = EditorSettings.serializationMode == SerializationMode.ForceText;

                if (!forceTextSerialization)
                {
                    message += "- Force Text Serialization\n";
                }

                var il2Cpp = PlayerSettings.GetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup) == ScriptingImplementation.IL2CPP;

                if (!il2Cpp)
                {
                    message += "- Change the Scripting Backend to use IL2CPP\n";
                }

                var visibleMetaFiles = EditorSettings.externalVersionControl.Equals("Visible Meta Files");

                if (!visibleMetaFiles)
                {
                    message += "- Visible meta files\n";
                }

                if (!PlayerSettings.virtualRealitySupported)
                {
                    message += "- Enable XR Settings for your current platform\n";
                }

                message += "\nWould you like to make this change?";

                if (!forceTextSerialization || !il2Cpp || !visibleMetaFiles || !PlayerSettings.virtualRealitySupported)
                {
                    var choice = EditorUtility.DisplayDialogComplex("Apply Mixed Reality Toolkit Default Settings?", message, "Apply", "Ignore", "Later");

                    switch (choice)
                    {
                        case 0:
                            EditorSettings.serializationMode = SerializationMode.ForceText;
                            EditorSettings.externalVersionControl = "Visible Meta Files";
                            PlayerSettings.SetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup, ScriptingImplementation.IL2CPP);
                            PlayerSettings.virtualRealitySupported = true;
                            refresh = true;
                            break;
                        case 1:
                            EditorPrefs.SetBool(IgnoreKey, true);
                            break;
                        case 2:
                            break;
                    }
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

        /// <summary>
        /// Finds the path of a directory relative to the project folder.
        /// </summary>
        /// <param name="directoryPathToSearch">
        /// The subtree's root path to search in.
        /// </param>
        /// <param name="directoryName">
        /// The name of the directory to search for.
        /// </param>
        /// <param name="path"></param>
        internal static bool FindRelativeDirectory(string directoryPathToSearch, string directoryName, out string path)
        {
            string absolutePath;
            if (FindDirectory(directoryPathToSearch, directoryName, out absolutePath))
            {
                path = MakePathRelativeToProject(absolutePath);
                return true;
            }

            path = string.Empty;
            return false;
        }

        /// <summary>
        /// Finds the absolute path of a directory.
        /// </summary>
        /// <param name="directoryPathToSearch">
        /// The subtree's root path to search in.
        /// </param>
        /// <param name="directoryName">
        /// The name of the directory to search for.
        /// </param>
        /// <param name="path"></param>
        internal static bool FindDirectory(string directoryPathToSearch, string directoryName, out string path)
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

        private static string MakePathRelativeToProject(string absolutePath)
        {
            return absolutePath.Replace(
                Application.dataPath + Path.DirectorySeparatorChar,
                "Assets" + Path.DirectorySeparatorChar);
        }

        private static void SetIconTheme()
        {
            if (string.IsNullOrEmpty(MixedRealityToolkit_AbsoluteFolderPath))
            {
                Debug.LogError("Unable to find the Mixed Reality Toolkit's directory!");
                return;
            }

            var icons = Directory.GetFiles(MixedRealityToolkit_AbsoluteFolderPath + "/StandardAssets/Icons");
            var icon = new Texture2D(2, 2);
            var iconColor = new Color32(4, 165, 240, 255);

            for (int i = 0; i < icons.Length; i++)
            {
                icons[i] = icons[i].Replace("/", "\\");
                if (icons[i].Contains(".meta")) { continue; }

                var imageData = File.ReadAllBytes(icons[i]);
                icon.LoadImage(imageData, false);

                var pixels = icon.GetPixels32();
                for (int j = 0; j < pixels.Length; j++)
                {
                    pixels[j].r = iconColor.r;
                    pixels[j].g = iconColor.g;
                    pixels[j].b = iconColor.b;
                }

                icon.SetPixels32(pixels);
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
