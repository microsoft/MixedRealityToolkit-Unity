// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if !WINDOWS_UWP
// When the .NET scripting backend is enabled and C# projects are built
// Unity doesn't include the the required assemblies (i.e. the ones below).
// Given that the .NET backend is deprecated by Unity at this point it's we have
// to work around this on our end.
using Microsoft.MixedReality.Toolkit.UI;
using NUnit.Framework;
using System.Collections;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Microsoft.MixedReality.Toolkit;
namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class TestFixture_02_ManipulationTests
    {
        /// <summary>
        /// Test creating adding a ManipulationHandler to GameObject programmatically.
        /// Should be able to run scene without getting any exceptions.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator Test01_ManipulationHandlerInstantiate()
        {
            TestUtilities.InitializeMixedRealityToolkitScene(true);

            var playspace = MixedRealityToolkit.Instance.MixedRealityPlayspace;
            playspace.transform.position = new Vector3(1.0f, 1.5f, -2.0f);
            playspace.transform.LookAt(Vector3.zero);

            var testLight = new GameObject("TestLight");
            var light = testLight.AddComponent<Light>();
            light.type = LightType.Directional;
            testLight.transform.position = new Vector3(-2.5f, 3.0f, -1.2f);
            testLight.transform.LookAt(Vector3.zero);

            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObject.transform.localScale = Vector3.one * 0.2f;

            var manipHandler = testObject.AddComponent<ManipulationHandler>();
            yield return WaitForFrames(2);
        }

        /// <summary>
        /// Test creating ManipulationHandler and receiving hover enter/exit events
        /// from gaze provider.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator Test02_ManipulationHandlerGazeHover()
        {
            TestUtilities.InitializeMixedRealityToolkitScene(true);

            var playspace = MixedRealityToolkit.Instance.MixedRealityPlayspace;
            playspace.transform.position = new Vector3(1.0f, 1.5f, -2.0f);
            playspace.transform.LookAt(Vector3.zero);

            var testLight = new GameObject("TestLight");
            var light = testLight.AddComponent<Light>();
            light.type = LightType.Directional;
            testLight.transform.position = new Vector3(-2.5f, 3.0f, -1.2f);
            testLight.transform.LookAt(Vector3.zero);

            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObject.transform.localScale = Vector3.one * 0.2f;

            var manipHandler = testObject.AddComponent<ManipulationHandler>();
            int hoverEnterCount = 0;
            int hoverExitCount = 0;

            manipHandler.OnHoverEntered.AddListener((eventData) => hoverEnterCount++);
            manipHandler.OnHoverExited.AddListener((eventData) => hoverExitCount++);

            yield return WaitForFrames(2);
            Assert.AreEqual(hoverEnterCount, 1, $"ManipulationHandler did not receive hover enter event, count is {hoverEnterCount}");

            testObject.transform.Translate(Vector3.up);
            yield return new WaitForEndOfFrame();
            Assert.IsTrue(hoverExitCount == 1, "ManipulationHandler did not receive hover exit event");
        }

        private IEnumerator WaitForFrames(int nFrames)
        {
            for (int i = 0; i < nFrames; i++)
            {
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
#endif