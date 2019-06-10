// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if !WINDOWS_UWP
// When the .NET scripting backend is enabled and C# projects are built
// Unity doesn't include the required assemblies (i.e. the ones below).
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
    public class ManipulationHandlerTests
    {
        /// <summary>
        /// Test creating adding a ManipulationHandler to GameObject programmatically.
        /// Should be able to run scene without getting any exceptions.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator Test01_ManipulationHandlerInstantiate()
        {
            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObject.transform.localScale = Vector3.one * 0.2f;

            var manipHandler = testObject.AddComponent<ManipulationHandler>();
            // Wait for two frames to make sure we don't get null pointer exception.
            yield return null;
            yield return null;

            GameObject.Destroy(testObject);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        [SetUp]
        public void SetupMrtk()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes(true);
            TestUtilities.InitializePlayspace();
        }

        [TearDown]
        public void ShutdownMrtk()
        {
            TestUtilities.ShutdownMixedRealityToolkit();
        }

        /// <summary>
        /// Test creating ManipulationHandler and receiving hover enter/exit events
        /// from gaze provider.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator Test02_ManipulationHandlerGazeHover()
        {

            var testObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testObject.transform.localScale = Vector3.one * 0.2f;

            var manipHandler = testObject.AddComponent<ManipulationHandler>();
            int hoverEnterCount = 0;
            int hoverExitCount = 0;

            manipHandler.OnHoverEntered.AddListener((eventData) => hoverEnterCount++);
            manipHandler.OnHoverExited.AddListener((eventData) => hoverExitCount++);

            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.AreEqual(1, hoverEnterCount, $"ManipulationHandler did not receive hover enter event, count is {hoverEnterCount}");

            testObject.transform.Translate(Vector3.up);

            // First yield for physics. Second for normal frame step.
            // Without first one, second might happen before translation is applied.
            // Without second one services will not be stepped.
            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.AreEqual(1, hoverExitCount, "ManipulationHandler did not receive hover exit event");

            testObject.transform.Translate(5 * Vector3.up);

            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.IsTrue(hoverExitCount == 1, "ManipulationHandler received the second hover event");

            GameObject.Destroy(testObject);

            // Wait for a frame to give Unity a chance to actually destroy the object
            yield return null;
        }

    }
}
#endif