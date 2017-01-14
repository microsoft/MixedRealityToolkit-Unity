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

    
    public class AutoConfigureMenu : MonoBehaviour
    {
		/// <summary>
		/// Displays a help page for the HoloToolkit.
		/// </summary>
		[MenuItem("HoloToolkit/Configure/Show Help", false, 3)]
        public static void ShowHelp()
        {
            Application.OpenURL("https://github.com/Microsoft/HoloToolkit-Unity/wiki");
        }

        /// <summary>
        /// Applies recommended scene settings to the current scenes
        /// </summary>
        [MenuItem("HoloToolkit/Configure/Apply HoloLens Scene Settings", false, 1)]
        public static void ApplySceneSettings()
        {
			SceneSettingsWindow window = (SceneSettingsWindow)EditorWindow.GetWindow(typeof(SceneSettingsWindow), true, "Apply HoloLens Scene Settings");
			window.Show();
		}

		/// <summary>
		/// Applies recommended project settings to the current project
		/// </summary>
		[MenuItem("HoloToolkit/Configure/Apply HoloLens Project Settings", false, 1)]
        public static void ApplyProjectSettings()
        {
			ProjectSettingsWindow window = (ProjectSettingsWindow)EditorWindow.GetWindow(typeof(ProjectSettingsWindow), true, "Apply HoloLens Project Settings");
			window.Show();
		}

		/// <summary>
		/// Applies recommended capability settings to the current project
		/// </summary>
		[MenuItem("HoloToolkit/Configure/Apply HoloLens Capability Settings", false, 2)]
        static void ApplyHoloLensCapabilitySettings()
        {
			CapabilitySettingsWindow window = (CapabilitySettingsWindow)EditorWindow.GetWindow(typeof(CapabilitySettingsWindow), true, "Apply HoloLens Capability Settings");
            window.Show();
        }
    }
}