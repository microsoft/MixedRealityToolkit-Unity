// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if UNITY_EDITOR

using Microsoft.MixedReality.Toolkit;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Utility class for checking if a scene has been properly set up for use with MixedRealityToolkit
    /// </summary>
    [InitializeOnLoad]
    public class MixedRealityToolkitSceneChecker
    {
        const string HideNoActiveToolkitWarningKey = "MRTK_HideNoActiveToolkitWarningKey";
        private static bool HideNoActiveToolkitWarning = true;

        static MixedRealityToolkitSceneChecker()
        {
            // Check for a valid MRTK instance when scenes are opened or created.
            EditorSceneManager.sceneOpened += SceneOpened;
            EditorSceneManager.newSceneCreated += NewSceneCreated;
        }

        private static void SceneOpened(Scene scene, OpenSceneMode mode)
        {
            CheckMixedRealityToolkitScene();
        }

        private static void NewSceneCreated(Scene scene, NewSceneSetup setup, NewSceneMode mode)
        {
            switch (setup)
            {
                // Ignore the check when the scene is explicitly empty.
                // This includes empty scenes in playmode tests.
                case NewSceneSetup.EmptyScene:
                    break;

                case NewSceneSetup.DefaultGameObjects:
                    CheckMixedRealityToolkitScene();
                    break;
            }
        }

        private static void CheckMixedRealityToolkitScene()
        {
            // If building, don't perform this check
            if (BuildPipeline.isBuildingPlayer)
            {
                return;
            }

            // Create The MR Manager if none exists.
            if (!MixedRealityToolkit.IsInitialized)
            {
                // Search the scene for one, in case we've just hot reloaded the assembly.
                var managerSearch = Object.FindObjectsOfType<MixedRealityToolkit>();

                if (managerSearch.Length == 0)
                {
                    HideNoActiveToolkitWarning = SessionState.GetBool(HideNoActiveToolkitWarningKey, false);
                    if (!HideNoActiveToolkitWarning)
                    {
                        NoActiveToolkitWarning.OpenWindow(null);
                    }
                    return; 
                }
            }
        }

        internal class NoActiveToolkitWarning : EditorWindow
        {
            private static NoActiveToolkitWarning activeWindow;
            [SerializeField]
            public MixedRealityToolkitConfigurationProfile configurationProfile;
            private bool hideWarning = false;

            public static void OpenWindow(MixedRealityToolkitConfigurationProfile configurationProfile)
            {
                // If we already have an active window, bail
                if (activeWindow != null)
                {
                    return;
                }

                activeWindow = EditorWindow.GetWindow<NoActiveToolkitWarning>();
                activeWindow.configurationProfile = configurationProfile;
                activeWindow.maxSize = new Vector2(400, 140);
                activeWindow.minSize = new Vector2(400, 140);
                activeWindow.titleContent = new GUIContent("No Active Toolkit Found");

                activeWindow.ShowPopup(); 
            }

            private void OnGUI()
            {
                EditorGUILayout.HelpBox("There is no active Mixed Reality Toolkit in your scene. Would you like to create one now?", MessageType.Warning);

                hideWarning = EditorGUILayout.Toggle("Don't show this again", hideWarning);

                configurationProfile = (MixedRealityToolkitConfigurationProfile)EditorGUILayout.ObjectField(configurationProfile, typeof(MixedRealityToolkitConfigurationProfile), false);
                if (configurationProfile == null)
                {
                    EditorGUILayout.HelpBox("Select a configuration profile.", MessageType.Info);
                }

                EditorGUILayout.BeginHorizontal();
                GUI.enabled = (configurationProfile != null);
                if (GUILayout.Button("Yes"))
                {
                    var playspace = MixedRealityToolkit.Instance.MixedRealityPlayspace;
                    Debug.Assert(playspace != null);
                    MixedRealityToolkit.Instance.ActiveProfile = configurationProfile;

                    SessionState.SetBool(HideNoActiveToolkitWarningKey, hideWarning);
                    Close();
                }
                GUI.enabled = true;

                if (GUILayout.Button("No"))
                {
                    SessionState.SetBool(HideNoActiveToolkitWarningKey, hideWarning);
                    Close();
                }
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}

#endif