// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using Microsoft.MixedReality.Toolkit.Editor;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

#if WINDOWS_UWP
using UnityEngine.Assertions;
#endif

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public static class TestUtilities
    {
        const string primaryTestSceneTemporarySavePath = "Assets/__temp_primary_test_scene.unity";
        const string additiveTestSceneTemporarySavePath = "Assets/__temp_additive_test_scene_#.unity";
        public static Scene primaryTestScene;
        public static Scene[] additiveTestScenes = System.Array.Empty<Scene>();

        /// <summary>
        /// Destroys all scene assets that were created over the course of testing.
        /// Used only in editor tests.
        /// </summary>
        public static void EditorTearDownScenes()
        {
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying)
            {
                // If any of our scenes were saved, tear down the assets
                SceneAsset primaryTestSceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(primaryTestSceneTemporarySavePath);
                if (primaryTestSceneAsset != null)
                {
                    AssetDatabase.DeleteAsset(primaryTestSceneTemporarySavePath);
                }

                for (int i = 0; i < additiveTestScenes.Length; i++)
                {
                    string path = additiveTestSceneTemporarySavePath.Replace("#", i.ToString());
                    SceneAsset additiveTestSceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
                    if (additiveTestSceneAsset != null)
                    {
                        AssetDatabase.DeleteAsset(path);
                    }
                }
                AssetDatabase.Refresh();
            }
#endif
        }

        /// <summary>
        /// Creates a number of scenes and loads them additively for testing. Must create a minimum of 1.
        /// Used only in editor tests.
        /// </summary>
        public static void EditorCreateScenes(int numScenesToCreate = 1)
        {
            // Create default test scenes.
            // In the editor this can be done using EditorSceneManager with a default setup.
            // In playmode the scene needs to be set up manually.

#if UNITY_EDITOR
            Assert.False(EditorApplication.isPlaying, "This method should only be called during edit mode tests. Use PlaymodeTestUtilities.");

            List<Scene> additiveTestScenesList = new List<Scene>();

            if (numScenesToCreate == 1)
            {   // No need to save this scene, we're just creating one
                primaryTestScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            }
            else
            {
                // Make the first scene single so it blows away previously loaded scenes
                primaryTestScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
                // Save the scene (temporarily) so we can load additively on top of it
                EditorSceneManager.SaveScene(primaryTestScene, primaryTestSceneTemporarySavePath);

                for (int i = 1; i < numScenesToCreate; i++)
                {
                    string path = additiveTestSceneTemporarySavePath.Replace("#", additiveTestScenesList.Count.ToString());
                    // Create subsequent scenes additively
                    Scene additiveScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Additive);
                    additiveTestScenesList.Add(additiveScene);
                    // Save the scene (temporarily) so we can load additively on top of it
                    EditorSceneManager.SaveScene(additiveScene, path);
                }
            }

            additiveTestScenes = additiveTestScenesList.ToArray();
#endif
        }

        /// <summary>
        /// Creates a playspace and moves it into a default position.
        /// </summary>
        public static void InitializePlayspace()
        {
            MixedRealityPlayspace.PerformTransformation(
            p =>
            {
                p.position = new Vector3(1.0f, 1.5f, -2.0f);
                p.LookAt(Vector3.zero);
            });
        }

        /// <summary>
        /// Forces the playspace camera to face forward.
        /// </summary>
        public static void PlayspaceToOriginLookingForward()
        {
            // Move the camera to origin looking at +z to more easily see the a target at 0,0,0
            MixedRealityPlayspace.PerformTransformation(
            p =>
            {
                p.position = Vector3.zero;
                p.LookAt(Vector3.forward);
            });
        }

        /// <summary>
        /// Creates the requested number of scenes, then creates one instance of the MixedRealityToolkit in the active scene.
        /// </summary>
        public static void InitializeMixedRealityToolkitAndCreateScenes(bool useDefaultProfile = false, int numScenesToCreate = 1)
        {
            // Setup
            EditorCreateScenes(numScenesToCreate);
            InitializeMixedRealityToolkit(useDefaultProfile);
        }

        public static void InitializeCamera()
        {
            Camera[] cameras = GameObject.FindObjectsOfType<Camera>();

            if (cameras.Length == 0)
            {
                new GameObject("Main Camera", typeof(Camera), typeof(AudioListener)) { tag = "MainCamera" }.GetComponent<Camera>();
            }
        }

        public static void InitializeMixedRealityToolkit(MixedRealityToolkitConfigurationProfile configuration)
        {
            InitializeCamera();

            if (!MixedRealityToolkit.IsInitialized)
            {
                MixedRealityToolkit mixedRealityToolkit = new GameObject("MixedRealityToolkit").AddComponent<MixedRealityToolkit>();
                MixedRealityToolkit.SetActiveInstance(mixedRealityToolkit);
                MixedRealityToolkit.ConfirmInitialized();
            }

            // Todo: this condition shouldn't be here.
            // It's here due to some edit mode tests initializing MRTK instance in Edit mode, causing some of 
            // event handler registration to live over tests and cause next tests to fail.
            // Exact reason requires investigation.
            if (Application.isPlaying)
            {
                BaseEventSystem.enableDanglingHandlerDiagnostics = true;
            }

            Assert.IsTrue(MixedRealityToolkit.IsInitialized);
            Assert.IsNotNull(MixedRealityToolkit.Instance);


            MixedRealityToolkit.Instance.ActiveProfile = configuration;
            Assert.IsTrue(MixedRealityToolkit.Instance.ActiveProfile != null);
        }

        public static void InitializeMixedRealityToolkit(bool useDefaultProfile = false)
        {
            var configuration = useDefaultProfile
                ? GetDefaultMixedRealityProfile<MixedRealityToolkitConfigurationProfile>()
                : ScriptableObject.CreateInstance<MixedRealityToolkitConfigurationProfile>();

            Assert.IsTrue(configuration != null, "Failed to find the Default Mixed Reality Configuration Profile");
            InitializeMixedRealityToolkit(configuration);
        }

        public static void ShutdownMixedRealityToolkit()
        {
            MixedRealityToolkit.SetInstanceInactive(MixedRealityToolkit.Instance);
            if (Application.isPlaying)
            {
                MixedRealityPlayspace.Destroy();
            }

            BaseEventSystem.enableDanglingHandlerDiagnostics = false;
        }

        private static T GetDefaultMixedRealityProfile<T>() where T : BaseMixedRealityProfile
        {
#if UNITY_EDITOR
            return ScriptableObjectExtensions.GetAllInstances<T>().FirstOrDefault(profile => profile.name.Equals($"Default{typeof(T).Name}"));
#else
            return ScriptableObject.CreateInstance<T>();
#endif
        }

        public static void AssertAboutEqual(Vector3 actual, Vector3 expected, string message, float tolerance = 0.01f)
        {
            var dist = (actual - expected).magnitude;
            Debug.Assert(dist < tolerance, $"{message}, expected {expected.ToString("0.000")}, was {actual.ToString("0.000")}");
        }

        public static void AssertAboutEqual(Quaternion actual, Quaternion expected, string message, float tolerance = 0.01f)
        {
            var angle = Quaternion.Angle(actual, expected);
            Debug.Assert(angle < tolerance, $"{message}, expected {expected.ToString("0.000")}, was {actual.ToString("0.000")}");
        }

        public static void AssertNotAboutEqual(Vector3 val1, Vector3 val2, string message, float tolerance = 0.01f)
        {
            var dist = (val1 - val2).magnitude;
            Debug.Assert(dist >= tolerance, $"{message}, val1 {val1.ToString("0.000")} almost equals val2 {val2.ToString("0.000")}");
        }

        public static void AssertNotAboutEqual(Quaternion val1, Quaternion val2, string message, float tolerance = 0.01f)
        {
            var angle = Quaternion.Angle(val1, val2);
            Debug.Assert(angle >= tolerance, $"{message}, val1 {val1.ToString("0.000")} almost equals val2 {val2.ToString("0.000")}");
        }

#if UNITY_EDITOR
        [MenuItem("Mixed Reality Toolkit/Utilities/Update/Icons/Tests")]
        private static void UpdateTestScriptIcons()
        {
            Texture2D icon = null;

            foreach (string iconPath in MixedRealityToolkitFiles.GetFiles("StandardAssets/Icons"))
            {
                if (iconPath.EndsWith("test_icon.png"))
                {
                    icon = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
                    break;
                }
            }

            if (icon == null)
            {
                Debug.Log("Couldn't find test icon.");
                return;
            }
            
            IEnumerable<string> testDirectories = MixedRealityToolkitFiles.GetDirectories(MixedRealityToolkitModuleType.Tests);

            foreach (string directory in testDirectories)
            {
                string[] scriptGuids = AssetDatabase.FindAssets("t:MonoScript", new string[] { MixedRealityToolkitFiles.GetAssetDatabasePath(directory) });

                for (int i = 0; i < scriptGuids.Length; i++)
                {
                    string scriptPath = AssetDatabase.GUIDToAssetPath(scriptGuids[i]);

                    EditorUtility.DisplayProgressBar("Updating Icons...", $"{i} of {scriptGuids.Length} {scriptPath}", i / (float)scriptGuids.Length);

                    MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(scriptPath);

                    Texture2D currentIcon = getIconForObject?.Invoke(null, new object[] { script }) as Texture2D;
                    if (currentIcon == null || !currentIcon.Equals(icon))
                    {
                        setIconForObject?.Invoke(null, new object[] { script, icon });
                        copyMonoScriptIconToImporters?.Invoke(null, new object[] { script });
                    }
                }
            }

            EditorUtility.ClearProgressBar();
        }

        private static readonly MethodInfo getIconForObject = typeof(EditorGUIUtility).GetMethod("GetIconForObject", BindingFlags.Static | BindingFlags.NonPublic);
        private static readonly MethodInfo setIconForObject = typeof(EditorGUIUtility).GetMethod("SetIconForObject", BindingFlags.Static | BindingFlags.NonPublic);
        private static readonly MethodInfo copyMonoScriptIconToImporters = typeof(MonoImporter).GetMethod("CopyMonoScriptIconToImporters", BindingFlags.Static | BindingFlags.NonPublic);
#endif
    }
}
