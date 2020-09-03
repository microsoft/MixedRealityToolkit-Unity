// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if !WINDOWS_UWP
// When the .NET scripting backend is enabled and C# projects are built
// The assembly that this file is part of is still built for the player,
// even though the assembly itself is marked as a test assembly (this is not
// expected because test assemblies should not be included in player builds).
// Because the .NET backend is deprecated in 2018 and removed in 2019 and this
// issue will likely persist for 2018, this issue is worked around by wrapping all
// play mode tests in this check.

using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests.SanityTests
{
    public class DemoSceneTests
    {
        const string HandInteractionExamplesSceneName = "HandInteractionExamples";
        const string HandInteractionExamplesScenePath = "MRTK/Examples/Demos/HandTracking/Scenes/HandInteractionExamples.unity";

        const float ScenePlayDuration = 1f;

        [UnityTest]
        public IEnumerator LoadHandInteractionExamplesScene()
        {
            AsyncOperation loadOp = SceneManager.LoadSceneAsync(HandInteractionExamplesSceneName);
            loadOp.allowSceneActivation = true;
            while (!loadOp.isDone)
            {
                yield return null;
            }

            yield return new WaitForSeconds(ScenePlayDuration);

            Assert.NotNull(CoreServices.InputSystem);
        }

        [TearDown]
        public void TearDown()
        {
            Scene scene = SceneManager.GetSceneByName(HandInteractionExamplesSceneName);
            if (scene.isLoaded)
            {
                Scene playModeTestScene = SceneManager.CreateScene("Empty");
                SceneManager.SetActiveScene(playModeTestScene);
                SceneManager.UnloadSceneAsync(scene.buildIndex);
            }
        }
    }
}
#endif