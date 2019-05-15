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
    public class InteractionTests_Press
    {
        // /// Test InteractionTest_Press_Far
        // [UnityTest]
        // public IEnumerator Test_InteractionTest_Press_Far()
        // {
        //     // Load test scene
        //     var loadOp = TestUtilities.LoadTestSceneAsync("HandInteractionExamples");
        //     while (loadOp.MoveNext())
        //     {
        //         yield return new WaitForFixedUpdate();
        //     }

        //     var buttonTester = new InteractableTester("PressExamples/PressableButtons/PressableButtonPlated");

        //     // Load input animation
        //     var inputAsset = (InputAnimationAsset)Resources.Load("InteractionTest_Press_Far", typeof(InputAnimationAsset));

        //     // Component to drive animation in the scene
        //     var director = TestUtilities.RunPlayable(inputAsset);

        //     // Default
        //     {
        //         InputAnimationMarker marker = inputAsset.InputAnimation.GetMarker(0);
        //         yield return new WaitForPlayableTime(director, marker.time);
        //         buttonTester.TestState(false, false);
        //     }
        //     // EnterFocus
        //     {
        //         InputAnimationMarker marker = inputAsset.InputAnimation.GetMarker(1);
        //         yield return new WaitForPlayableTime(director, marker.time);
        //         buttonTester.TestState(true, false);
        //     }
        //     // Press
        //     {
        //         InputAnimationMarker marker = inputAsset.InputAnimation.GetMarker(2);
        //         yield return new WaitForPlayableTime(director, marker.time);
        //         buttonTester.TestState(true, true);
        //     }
        //     // Release
        //     {
        //         InputAnimationMarker marker = inputAsset.InputAnimation.GetMarker(3);
        //         yield return new WaitForPlayableTime(director, marker.time);
        //         buttonTester.TestState(true, false);
        //     }
        //     // ExitFocus
        //     {
        //         InputAnimationMarker marker = inputAsset.InputAnimation.GetMarker(4);
        //         yield return new WaitForPlayableTime(director, marker.time);
        //         buttonTester.TestState(false, false);
        //     }

        //     yield return new WaitForPlayableEnded(director);
        // }

        // /// Test InteractionTest_Press_Near
        // [UnityTest]
        // public IEnumerator Test_InteractionTest_Press_Near()
        // {
        //     // Load test scene
        //     var loadOp = TestUtilities.LoadTestSceneAsync("HandInteractionExamples");
        //     while (loadOp.MoveNext())
        //     {
        //         yield return new WaitForFixedUpdate();
        //     }

        //     var buttonTester = new InteractableTester("PressExamples/PressableButtons/PressableButtonPlated");

        //     // Load input animation
        //     var inputAsset = (InputAnimationAsset)Resources.Load("InteractionTest_Press_Near", typeof(InputAnimationAsset));

        //     // Component to drive animation in the scene
        //     var director = TestUtilities.RunPlayable(inputAsset);

        //     // Idle
        //     {
        //         InputAnimationMarker marker = inputAsset.InputAnimation.GetMarker(0);
        //         yield return new WaitForPlayableTime(director, marker.time);
        //         // buttonTester.TestState(false, false);
        //     }
        //     // EnterFocus
        //     {
        //         InputAnimationMarker marker = inputAsset.InputAnimation.GetMarker(1);
        //         yield return new WaitForPlayableTime(director, marker.time);
        //         // buttonTester.TestState(true, false);
        //     }
        //     // Press
        //     {
        //         InputAnimationMarker marker = inputAsset.InputAnimation.GetMarker(2);
        //         yield return new WaitForPlayableTime(director, marker.time);
        //         // buttonTester.TestState(true, true);
        //     }
        //     // Release
        //     {
        //         InputAnimationMarker marker = inputAsset.InputAnimation.GetMarker(3);
        //         yield return new WaitForPlayableTime(director, marker.time);
        //         // buttonTester.TestState(true, false);
        //     }
        //     // ExitFocus
        //     {
        //         InputAnimationMarker marker = inputAsset.InputAnimation.GetMarker(4);
        //         yield return new WaitForPlayableTime(director, marker.time);
        //         // buttonTester.TestState(false, false);
        //     }

        //     yield return new WaitForPlayableEnded(director);
        // }

        // [TearDown]
        // public void TearDown()
        // {
        // }
    }
}