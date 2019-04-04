﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

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
        private static Scene testScene;
        public static Scene TestScene => testScene;

        public static void CleanupScene()
        {
            // Create a default test scene.
            // In the editor this can be done using EditorSceneManager with a default setup.
            // In playmode the scene needs to be set up manually.

#if UNITY_EDITOR
            if (!EditorApplication.isPlaying)
            {
                EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
                return;
            }
#endif

            if (testScene.IsValid() && testScene.isLoaded)
            {
                SceneManager.UnloadSceneAsync(testScene);
            }

            testScene = SceneManager.CreateScene("TestScene");
            SceneManager.SetActiveScene(testScene);

            var cameraObject = new GameObject("Camera");
            var camera = cameraObject.AddComponent<Camera>();
            cameraObject.tag = "MainCamera";
        }

        public static void InitializeMixedRealityToolkit()
        {
            MixedRealityToolkit.ConfirmInitialized();
        }

        public static void InitializeMixedRealityToolkitScene(bool useDefaultProfile = false)
        {
            // Setup
            CleanupScene();

            if (!MixedRealityToolkit.IsInitialized)
            {
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

        public static IEnumerator LoadTestSceneAsync(string sceneName)
        {
            var loadSceneOp = SceneManager.LoadSceneAsync(sceneName);
            loadSceneOp.allowSceneActivation = true;
            while (!loadSceneOp.isDone)
            {
                yield return null;
            }
            testScene = SceneManager.GetActiveScene();

            // Tests
            Assert.IsTrue(testScene.IsValid());
            Assert.IsTrue(testScene.isLoaded);
            Assert.IsTrue(MixedRealityToolkit.IsInitialized);
            Assert.IsNotNull(MixedRealityToolkit.Instance);
            Assert.IsTrue(MixedRealityToolkit.Instance.HasActiveProfile);
        }

        public static IEnumerator RunPlayableGraphAsync(PlayableDirector director = null)
        {
            if (!director)
            {
                director = Object.FindObjectOfType<PlayableDirector>();
            }
            if (!director)
            {
                yield break;
            }

            director.Play();

            var graph = director.playableGraph;
            while (graph.IsPlaying())
            {
                yield return null;
            }
        }
    }
}