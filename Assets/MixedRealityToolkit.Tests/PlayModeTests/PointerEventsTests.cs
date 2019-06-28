// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if !WINDOWS_UWP
// When the .NET scripting backend is enabled and C# projects are built
// Unity doesn't include the the required assemblies (i.e. the ones below).
// Given that the .NET backend is deprecated by Unity at this point it's we have
// to work around this on our end.
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using UnityEditor;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Input;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    // Tests to verify that pointer events are being raised correctly
    public class PointerEventsTests
    {

        // Helper script used to keep track of the number of pointer events raised
        private class PointerHandler : MonoBehaviour, IMixedRealityFocusChangedHandler, IMixedRealityFocusHandler, IMixedRealityPointerHandler
        {
            public class State
            {
                public int numBeforeFocusChange;
                public int numFocusChanged;
                public int numFocusEnter;
                public int numFocusExit;
                public int numPointerDown;
                public int numPointerUp;
                public int numPointerClicked;
                public int numPointerDragged;

                public void AssertEqual(State expected)
                {
                    Assert.AreEqual(expected.numBeforeFocusChange, numBeforeFocusChange);
                    Assert.AreEqual(expected.numFocusChanged, numFocusChanged);
                    Assert.AreEqual(expected.numFocusEnter, numFocusEnter);
                    Assert.AreEqual(expected.numFocusExit, numFocusExit);
                    Assert.AreEqual(expected.numPointerDown, numPointerDown);
                    Assert.AreEqual(expected.numPointerUp, numPointerUp);
                    Assert.AreEqual(expected.numPointerClicked, numPointerClicked);
                    Assert.AreEqual(expected.numPointerDragged, numPointerDragged);
                }
            }

            public State state = new State();
            private int numFocusing;

            void IMixedRealityFocusChangedHandler.OnBeforeFocusChange(FocusEventData eventData)
            {
                Assert.AreNotSame(eventData.OldFocusedObject, eventData.NewFocusedObject);
                Assert.AreEqual(state.numFocusChanged, state.numBeforeFocusChange);

                state.numBeforeFocusChange++;

                if (eventData.OldFocusedObject == gameObject)
                {
                    numFocusing--;
                }
                else if (eventData.NewFocusedObject == gameObject)
                {
                    numFocusing++;
                }
            }

            void IMixedRealityFocusChangedHandler.OnFocusChanged(FocusEventData eventData)
            {
                Assert.AreNotSame(eventData.OldFocusedObject, eventData.NewFocusedObject);
                Assert.AreEqual(state.numBeforeFocusChange, state.numFocusChanged + 1);

                state.numFocusChanged++;
            }

            void IMixedRealityFocusHandler.OnFocusEnter(FocusEventData eventData)
            {
                Assert.AreEqual(1, numFocusing);

                state.numFocusEnter++;
            }

            void IMixedRealityFocusHandler.OnFocusExit(FocusEventData eventData)
            {
                Assert.AreEqual(0, numFocusing);

                state.numFocusExit++;
            }

            void IMixedRealityPointerHandler.OnPointerClicked(MixedRealityPointerEventData eventData)
            {
                state.numPointerClicked++;
            }

            void IMixedRealityPointerHandler.OnPointerDown(MixedRealityPointerEventData eventData)
            {
                state.numPointerDown++;
            }

            void IMixedRealityPointerHandler.OnPointerDragged(MixedRealityPointerEventData eventData)
            {
                state.numPointerDragged++;
            }

            void IMixedRealityPointerHandler.OnPointerUp(MixedRealityPointerEventData eventData)
            {
                state.numPointerUp++;
            }
        }

        private const string testContentPath = "Assets/MixedRealityToolkit.Tests/PlayModeTests/Prefabs/PointerEventsTests.prefab";

        // Initializes MRTK, instantiates the test content prefab and adds a pointer handler to the test collider
        private PointerHandler SetupScene()
        {
            TestUtilities.InitializeMixedRealityToolkit(true);

            var testContent = AssetDatabase.LoadAssetAtPath<GameObject>(testContentPath);
            Assert.IsNotNull(testContent);

            testContent = Object.Instantiate(testContent);
            var collider = testContent.GetComponentInChildren<Collider>();
            Assert.IsNotNull(collider);

            return collider.gameObject.AddComponent<PointerHandler>();
        }

        // Tests that the right pointer events are raised when using a single pointer to interact with a collider
        [UnityTest]
        public IEnumerator TestOnePointer()
        {
            var handler = SetupScene();
            var rightHand = new TestHand(Handedness.Right);

            // Show hand far enough from the test collider to not give it focus
            Vector3 noFocusPos = new Vector3(0.05f, 0, 0.5f);
            yield return rightHand.Show(noFocusPos);

            // Make sure no events have been raised
            var expected = new PointerHandler.State();
            handler.state.AssertEqual(expected);

            // Move hand closer to the collider so it gives it focus
            Vector3 focusPos = new Vector3(0.05f, 0, 1.5f);
            yield return rightHand.MoveTo(focusPos);

            expected.numBeforeFocusChange++;
            expected.numFocusChanged++;
            expected.numFocusEnter++;
            handler.state.AssertEqual(expected);

            // Trigger pinch
            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Pinch);

            expected.numPointerDown++;
            expected.numPointerDragged++;
            handler.state.AssertEqual(expected);

            // Maintain pinch for a number of frames
            int numStepsPinching = 30;
            yield return rightHand.MoveTo(focusPos, numStepsPinching);

            expected.numPointerDragged += numStepsPinching;
            handler.state.AssertEqual(expected);

            // Release pinch
            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Open);

            expected.numPointerUp++;
            expected.numPointerClicked++;
            handler.state.AssertEqual(expected);

            // Move back to no focus
            yield return rightHand.MoveTo(noFocusPos);

            expected.numBeforeFocusChange++;
            expected.numFocusChanged++;
            expected.numFocusExit++;
            handler.state.AssertEqual(expected);

            yield return rightHand.Hide();
        }

        // Tests that the right pointer events are raised when using two pointers to interact with a collider
        [UnityTest]
        public IEnumerator TestTwoPointers()
        {
            var handler = SetupScene();

            // Show right hand far enough from the test collider to not give it focus
            var rightHand = new TestHand(Handedness.Right);
            Vector3 rightNoFocusPos = new Vector3(0.05f, 0, 0.5f);
            yield return rightHand.Show(rightNoFocusPos);

            // Make sure no events have been raised
            var expected = new PointerHandler.State();
            handler.state.AssertEqual(expected);

            // Show left hand far enough from the test collider to not give it focus
            var leftHand = new TestHand(Handedness.Left);
            Vector3 leftNoFocusPos = new Vector3(-0.05f, 0, 0.5f);
            yield return leftHand.Show(leftNoFocusPos);

            // Make sure no events have been raised
            handler.state.AssertEqual(expected);

            // Move left hand to give focus
            Vector3 leftFocusPos = new Vector3(-0.05f, 0, 1.5f);
            yield return leftHand.MoveTo(leftFocusPos);

            expected.numBeforeFocusChange++;
            expected.numFocusChanged++;
            expected.numFocusEnter++;
            handler.state.AssertEqual(expected);

            // Move right hand to give focus
            Vector3 rightFocusPos = new Vector3(0.05f, 0, 1.5f);
            yield return rightHand.MoveTo(rightFocusPos);

            expected.numBeforeFocusChange++;
            expected.numFocusChanged++;
            handler.state.AssertEqual(expected);

            // Pinch left
            yield return leftHand.SetGesture(ArticulatedHandPose.GestureId.Pinch);

            expected.numPointerDown++;
            expected.numPointerDragged++;
            handler.state.AssertEqual(expected);

            // Pinch right
            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Pinch);

            expected.numPointerDown++;
            expected.numPointerDragged += 2; // One for each pointer down
            handler.state.AssertEqual(expected);

            // Open left
            yield return leftHand.SetGesture(ArticulatedHandPose.GestureId.Open);

            expected.numPointerUp++;
            expected.numPointerClicked++;
            expected.numPointerDragged++; // Right pointer is still down
            handler.state.AssertEqual(expected);

            // Open right
            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Open);

            expected.numPointerUp++;
            expected.numPointerClicked++;
            handler.state.AssertEqual(expected);

            // Move left out of focus
            yield return leftHand.MoveTo(leftNoFocusPos);

            expected.numBeforeFocusChange++;
            expected.numFocusChanged++;
            handler.state.AssertEqual(expected);

            // Move right out of focus
            yield return rightHand.MoveTo(rightNoFocusPos);

            expected.numBeforeFocusChange++;
            expected.numFocusChanged++;
            expected.numFocusExit++;
            handler.state.AssertEqual(expected);

            yield return leftHand.Hide();
            yield return rightHand.Hide();
        }
    }
}

#endif
