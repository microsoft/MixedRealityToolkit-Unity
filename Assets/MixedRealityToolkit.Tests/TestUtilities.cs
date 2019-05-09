// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using System;
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
            var testScene = SceneManager.GetActiveScene();

            // Tests
            Assert.IsTrue(testScene.IsValid());
            Assert.IsTrue(testScene.isLoaded);
            Assert.IsTrue(MixedRealityToolkit.IsInitialized);
            Assert.IsNotNull(MixedRealityToolkit.Instance);
            Assert.IsTrue(MixedRealityToolkit.Instance.HasActiveProfile);
        }

        public static PlayableDirector RunPlayable(PlayableAsset playableAsset)
        {
            GameObject directorObject = new GameObject();
            PlayableDirector director = directorObject.AddComponent<PlayableDirector>();
            director.playableAsset = playableAsset;

            director.Play();

            return director;
        }
    }

    /// <summary>
    /// Utility class that waits until the PlayableDirector reaches a specified local time.
    /// </summary>
    public class WaitForPlayableTime : CustomYieldInstruction
    {
        private PlayableDirector director;
        private double playableTime;

        public override bool keepWaiting
        {
            get
            {
                return director.time < playableTime;
            }
        }

        public WaitForPlayableTime(PlayableDirector director, double time)
        {
            this.director = director;
            this.playableTime = time;
        }
    }

    
    /// <summary>
    /// Utility class that waits until the PlayableDirector reaches a specified local time.
    /// </summary>
    public class WaitForPlayableEnded : CustomYieldInstruction
    {
        private PlayableDirector director;

        public override bool keepWaiting
        {
            get
            {
                var graph = director.playableGraph;
                return graph.IsValid() && graph.IsPlaying();
            }
        }

        public WaitForPlayableEnded(PlayableDirector director)
        {
            this.director = director;
        }
    }

    public class ComponentTester<T> where T : MonoBehaviour
    {
        private T component;
        public T Component => component;

        public ComponentTester(string objectName)
        {
            var ob = GameObject.Find(objectName);
            Assert.IsNotNull(ob, $"Could not find object {objectName}");

            component = ob.GetComponent<T>();
            Assert.IsNotNull(component, $"Could not find {typeof(T).Name} component in object {objectName}");
        }
    }

    public class InteractableTester : ComponentTester<Interactable>
    {
        public InteractableTester(string objectName)
            : base(objectName)
        {
        }

        public void TestState(bool expectFocus, bool expectPress)
        {
            Assert.IsTrue(Component.HasFocus == expectFocus, $"{Component.gameObject.name}: Expected Interactable focus to be {expectFocus}, but is {Component.HasFocus}");
            Assert.IsTrue(Component.HasPress == expectPress, $"{Component.gameObject.name}: Expected Interactable press to be {expectPress}, but is {Component.HasPress}");
        }
    }

    public class ManipulationHandlerTester : ComponentTester<ManipulationHandler>
    {
        private bool isHovered;
        private bool isManipulating;

        public ManipulationHandlerTester(string objectName)
            : base(objectName)
        {
            Component.OnHoverEntered.AddListener(OnHoverEntered);
            Component.OnHoverExited.AddListener(OnHoverExited);
            Component.OnManipulationStarted.AddListener(OnManipulationStarted);
            Component.OnManipulationEnded.AddListener(OnManipulationEnded);
        }

        ~ManipulationHandlerTester()
        {
            Component.OnHoverEntered.RemoveListener(OnHoverEntered);
            Component.OnHoverExited.RemoveListener(OnHoverExited);
            Component.OnManipulationStarted.RemoveListener(OnManipulationStarted);
            Component.OnManipulationEnded.RemoveListener(OnManipulationEnded);
        }

        public void TestState(bool expectHovered, bool expectManipulating)
        {
            Assert.IsTrue(isHovered == expectHovered, $"{Component.gameObject.name}: Expected ManipulationHandler hovered state to be {expectHovered}, but is {isHovered}");
            Assert.IsTrue(isManipulating == expectManipulating, $"{Component.gameObject.name}: Expected ManipulationHandler manipulation state to be {expectManipulating}, but is {isManipulating}");
        }

        private void OnHoverEntered(ManipulationEventData evt)
        {
            isHovered = true;
        }

        private void OnHoverExited(ManipulationEventData evt)
        {
            isHovered = false;
        }

        private void OnManipulationStarted(ManipulationEventData evt)
        {
            isManipulating = true;
        }

        private void OnManipulationEnded(ManipulationEventData evt)
        {
            isManipulating = false;
        }
    }
}