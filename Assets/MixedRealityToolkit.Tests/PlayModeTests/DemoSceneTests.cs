using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using NUnit.Framework;

namespace Microsoft.MixedReality.Toolkit.Tests.SanityTests
{
    public class DemoSceneTests
    {
        const string HandInteractionExamplesSceneName = "HandInteractionExamples";
        const string HandInteractionExamplesScenePath = "MixedRealityToolkit.Examples/Demos/HandTracking/Scenes/HandInteractionExamples.unity";

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

            IMixedRealityInputSystem inputSystem = null;
            MixedRealityServiceRegistry.TryGetService(out inputSystem);

            Assert.NotNull(inputSystem);
        }

        [TearDown]
        public void TearDown()
        {
            Scene scene = SceneManager.GetSceneByName(HandInteractionExamplesSceneName);
            if (scene.isLoaded)
            {
                SceneManager.UnloadSceneAsync(scene.buildIndex);
            }
        }
    }
}