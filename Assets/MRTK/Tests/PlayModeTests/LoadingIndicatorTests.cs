// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if !WINDOWS_UWP
// When the .NET scripting backend is enabled and C# projects are built
// Unity doesn't include the required assemblies (i.e. the ones below).
// Given that the .NET backend is deprecated by Unity at this point, we have
// to work around this on our end.

using Microsoft.MixedReality.Toolkit.UI;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using System.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Microsoft.MixedReality.Toolkit.Tests
{
    class ProgressIndicatorTests
    {
        // SDK/Features/UX/Prefabs/ProgressIndicators/ProgressIndicatorLoadingBar.prefab
        private const string progressIndicatorLoadingBarPrefabGuid = "57d2436112e7d424da7e9a8e41c608dc";
        private static readonly string progressIndicatorLoadingBarPrefabPath = AssetDatabase.GUIDToAssetPath(progressIndicatorLoadingBarPrefabGuid);

        // SDK/Features/UX/Prefabs/ProgressIndicators/ProgressIndicatorRotatingObject.prefab
        private const string progressIndicatorRotatingObjectPrefabGuid = "274fde8ad8cd85a4a88acb4c2c892028";
        private static readonly string progressIndicatorRotatingObjectPrefabPath = AssetDatabase.GUIDToAssetPath(progressIndicatorRotatingObjectPrefabGuid);

        // SDK/Features/UX/Prefabs/ProgressIndicators/ProgressIndicatorRotatingOrbs.prefab
        private const string progressIndicatorRotatingOrbsPrefabGuid = "65fa42bb01c733c42b05a4e91628f494";
        private static readonly string progressIndicatorRotatingOrbsPrefabPath = AssetDatabase.GUIDToAssetPath(progressIndicatorRotatingOrbsPrefabGuid);

        /// <summary>
        /// Tests that prefab can be opened and closed at runtime.
        /// </summary>
        [UnityTest]
        public IEnumerator TestOpenCloseLoadingBarPrefab()
        {
            GameObject progressIndicatorObject;
            IProgressIndicator progressIndicator;
            InstantiatePrefab(progressIndicatorLoadingBarPrefabPath, out progressIndicatorObject, out progressIndicator);
            Task testTask = TestOpenCloseProgressIndicatorAsync(progressIndicatorObject, progressIndicator);
            while (!testTask.IsCompleted)
            {
                yield return null;
            }

            // clean up
            GameObject.Destroy(progressIndicatorObject);
            yield return null;
        }

        /// <summary>
        /// Tests that prefab can be opened and closed at runtime.
        /// </summary>
        [UnityTest]
        public IEnumerator TestOpenCloseRotatingObjectPrefab()
        {
            GameObject progressIndicatorObject;
            IProgressIndicator progressIndicator;
            InstantiatePrefab(progressIndicatorRotatingObjectPrefabPath, out progressIndicatorObject, out progressIndicator);
            Task testTask = TestOpenCloseProgressIndicatorAsync(progressIndicatorObject, progressIndicator);
            while (!testTask.IsCompleted)
            {
                yield return null;
            }

            // clean up
            GameObject.Destroy(progressIndicatorObject);
            yield return null;
        }

        /// <summary>
        /// Tests that prefab can be opened and closed at runtime.
        /// </summary>
        [UnityTest]
        public IEnumerator TestOpenCloseRotatingOrbsPrefab()
        {
            GameObject progressIndicatorObject;
            IProgressIndicator progressIndicator;
            InstantiatePrefab(progressIndicatorRotatingOrbsPrefabPath, out progressIndicatorObject, out progressIndicator);
            Task testTask = TestOpenCloseProgressIndicatorAsync(progressIndicatorObject, progressIndicator, 3f);
            while (!testTask.IsCompleted)
            {
                yield return null;
            }

            // clean up
            GameObject.Destroy(progressIndicatorObject);
            yield return null;
        }

        /// <summary>
        /// Tests that prefab finishes closing after being disabled at runtime.
        /// </summary>
        [UnityTest]
        public IEnumerator TestHideBeforeClosingRotatingOrbsPrefab()
        {
            GameObject progressIndicatorObject;
            IProgressIndicator progressIndicator;
            InstantiatePrefab(progressIndicatorRotatingOrbsPrefabPath, out progressIndicatorObject, out progressIndicator);
            Task testTask = TestOpenCloseProgressIndicatorAsync(progressIndicatorObject, progressIndicator, 3f, hideAfterOpening: true);

            // Wait a maximum time before considering the progress bar as stuck
            float timeStarted = Time.time;
            const float timeout = 5.0f; 
            while (!testTask.IsCompleted)
            {
                if (Time.time < timeStarted + timeout)
                {
                    yield return null;
                }
                else
                {
                    Assert.Fail("The progress bar is stuck closing.");
                }
            }

            // clean up
            GameObject.Destroy(progressIndicatorObject);
            yield return null;
        }

        private async Task TestOpenCloseProgressIndicatorAsync(GameObject progressIndicatorObject, IProgressIndicator progressIndicator, float timeOpen = 2f, bool hideAfterOpening = false)
        {
            // Deactivate the progress indicator
            progressIndicatorObject.SetActive(false);

            // Make sure it's closed
            Assert.True(progressIndicator.State == ProgressIndicatorState.Closed, "Progress indicator was not in correct state on startup: " + progressIndicator.State);

            // Make sure we can set progress and message
            progressIndicator.Progress = 0;
            progressIndicator.Message = "Progress Test";

            // Wait for it to open
            await progressIndicator.OpenAsync();

            // Make sure it's actually open
            Assert.True(progressIndicator.State == ProgressIndicatorState.Open, "Progress indicator was not open after open async call: " + progressIndicator.State);

            // Hide the gameObject if requested
            if (hideAfterOpening)
            {
                progressIndicatorObject.SetActive(false);
            }

            // Make sure we can set its progress and message while open
            // Also make sure we can set progress to a value greater than 1 without blowing anything up
            float timeStarted = Time.time;
            while (Time.time < timeStarted + timeOpen)
            {
                progressIndicator.Progress = Time.time - timeStarted;
                progressIndicator.Message = "Current Time: " + Time.time;
                await Task.Yield();
            }

            // Wait for it to close
            await progressIndicator.CloseAsync();

            // Make sure it's actually closed
            Assert.True(progressIndicator.State == ProgressIndicatorState.Closed, "Progress indicator was not closed after close async call: " + progressIndicator.State);
        }

        private void InstantiatePrefab(string path, out GameObject progressIndicatorObject, out IProgressIndicator progressIndicator)
        {
            progressIndicatorObject = null;
            progressIndicator = null;

#if UNITY_EDITOR
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            progressIndicatorObject = Object.Instantiate(prefab);
            progressIndicator = (IProgressIndicator)progressIndicatorObject.GetComponent(typeof(IProgressIndicator));
#endif
        }
    }
}
#endif