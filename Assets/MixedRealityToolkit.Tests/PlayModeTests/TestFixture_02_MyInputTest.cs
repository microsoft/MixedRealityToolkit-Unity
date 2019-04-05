// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.TestTools;
using System.Collections;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class TestFixture_01_MyInputTest
    {
        [UnityTest]
        public IEnumerator Test01_MyInputTest()
        {
            var loadOp = TestUtilities.LoadTestSceneAsync("MyTestScene");
            while (loadOp.MoveNext())
            {
                yield return new WaitForFixedUpdate();
            }

            var cheese = GameObject.Find("Cheese");
            var manipHandler = cheese.GetComponent<ManipulationHandler>();

            bool isHovered = false;
            manipHandler.OnHoverEntered.AddListener((ManipulationEventData) => { isHovered = true; });
            manipHandler.OnHoverExited.AddListener((ManipulationEventData) => { isHovered = false; });

            bool isManipulating = false;
            manipHandler.OnManipulationStarted.AddListener((ManipulationEventData) => { isManipulating = true; });
            manipHandler.OnManipulationEnded.AddListener((ManipulationEventData) => { isManipulating = false; });

            var director = Object.FindObjectOfType<PlayableDirector>();

            var playOp = TestUtilities.RunPlayableGraphAsync(director);
            Assert.IsFalse(isHovered);
            Assert.IsFalse(isManipulating);

            // Just before focusing on the cheese
            yield return new WaitForPlayableTime(director, 2.25);
            Assert.IsFalse(isHovered);
            Assert.IsFalse(isManipulating);

            // Just after focusing on the cheese
            yield return new WaitForPlayableTime(director, 2.43);
            Assert.IsTrue(isHovered);
            Assert.IsFalse(isManipulating);

            // Just before grabbing the cheese
            yield return new WaitForPlayableTime(director, 4.42);
            Assert.IsTrue(isHovered);
            Assert.IsFalse(isManipulating);

            // Just after grabbing the cheese
            yield return new WaitForPlayableTime(director, 4.67);
            Assert.IsTrue(isHovered);
            Assert.IsTrue(isManipulating);

            yield return playOp;
        }

        [TearDown]
        public void TearDown()
        {
        }
    }
}