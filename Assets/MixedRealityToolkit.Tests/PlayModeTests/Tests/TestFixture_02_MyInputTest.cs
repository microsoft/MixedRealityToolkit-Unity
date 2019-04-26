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
            var loadOp = TestUtilities.LoadTestSceneAsync("HandInteractionExamples");
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

            var inputAsset = (InputAnimationAsset)Resources.Load("ButtonsPushFar", typeof(InputAnimationAsset));

            // var playOp = TestUtilities.RunPlayableAsync(inputAsset);
            var director = TestUtilities.RunPlayable(inputAsset);

            // ADD COMMENT HERE
            var marker0 = inputAsset.InputAnimation.GetMarker(0);
            yield return new WaitForPlayableTime(director, marker0.time);
            Debug.Log($"{marker0.name}");
            // ADD TEST CONDITIONS HERE

            // ADD COMMENT HERE
            var marker1 = inputAsset.InputAnimation.GetMarker(1);
            yield return new WaitForPlayableTime(director, marker1.time);
            Debug.Log($"{marker1.name}");
            // ADD TEST CONDITIONS HERE

            yield return new WaitForPlayableEnded(director);
        }

        [TearDown]
        public void TearDown()
        {
        }
    }
}