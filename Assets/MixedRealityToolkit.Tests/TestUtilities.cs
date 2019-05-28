// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

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
        public static Scene[] additiveTestScenes = new Scene[0];

        public static void InitializeMixedRealityToolkit()
        {
            MixedRealityToolkit.ConfirmInitialized();
        }

        /// <summary>
        /// Destroys all scene assets that were created over the course of testing
        /// </summary>
        public static void TearDownScenes()
        {
#if UNITY_EDITOR
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
#endif
        }

        /// <summary>
        /// Creates a number of scenes and loads them additively for testing. Must create a minimum of 1.
        /// </summary>
        /// <param name="numScenesToCreate"></param>
        public static void CreateScenes(int numScenesToCreate = 1)
        {
            Debug.Assert(numScenesToCreate > 0);

            // Create default test scenes.
            // In the editor this can be done using EditorSceneManager with a default setup.
            // In playmode the scene needs to be set up manually.

#if UNITY_EDITOR
            if (!EditorApplication.isPlaying)
            {
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

                return;
            }
#endif
        }

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
        /// Creates the requested number of scenes, then creates one instance of the MixedRealityToolkit in the active scene.
        /// </summary>
        /// <param name="useDefaultProfile"></param>
        /// <param name="numScenesToCreate"></param>
        public static void InitializeMixedRealityToolkitAndCreateScenes(bool useDefaultProfile = false, int numScenesToCreate = 1)
        {
            // Setup
            CreateScenes(numScenesToCreate);
            InitializeMixedRealityToolkit(useDefaultProfile);
        }

        public static void InitializeMixedRealityToolkit(bool useDefaultProfile = false)
        {
            if (!MixedRealityToolkit.IsInitialized)
            {
                MixedRealityToolkit mixedRealityToolkit = new GameObject("MixedRealityToolkit").AddComponent<MixedRealityToolkit>();
                MixedRealityToolkit.SetActiveInstance(mixedRealityToolkit);
                MixedRealityToolkit.ConfirmInitialized();
            }

            // Tests
            Assert.IsTrue(MixedRealityToolkit.IsInitialized);
            Assert.IsNotNull(MixedRealityToolkit.Instance);
            if (!MixedRealityToolkit.Instance.HasActiveProfile)
            {
                var configuration = useDefaultProfile
                    ? GetDefaultMixedRealityProfile<MixedRealityToolkitConfigurationProfile>()
                    : ScriptableObject.CreateInstance<MixedRealityToolkitConfigurationProfile>();

                Assert.IsTrue(configuration != null, "Failed to find the Default Mixed Reality Configuration Profile");
                MixedRealityToolkit.Instance.ActiveProfile = configuration;
                Assert.IsTrue(MixedRealityToolkit.Instance.ActiveProfile != null);
            }
        }

        private static T GetDefaultMixedRealityProfile<T>() where T : BaseMixedRealityProfile
        {
#if UNITY_EDITOR
            return ScriptableObjectExtensions.GetAllInstances<T>().FirstOrDefault(profile => profile.name.Equals($"Default{typeof(T).Name}"));
#else
            return ScriptableObject.CreateInstance<T>();
#endif
        }
    }
}
