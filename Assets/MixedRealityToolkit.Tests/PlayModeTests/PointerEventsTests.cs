// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if !WINDOWS_UWP
// When the .NET scripting backend is enabled and C# projects are built
// The assembly that this file is part of is still built for the player,
// even though the assembly itself is marked as a test assembly (this is not
// expected because test assemblies should not be included in player builds).
// Because the .NET backend is deprecated in 2018 and removed in 2019 and this
// issue will likely persist for 2018, this issue is worked around by wrapping all
// play mode tests in this check.

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

                // Uncomment to help debugging issues
                //var controllerPointer = eventData.Pointer as BaseControllerPointer;
                //Debug.Log($"Pointer '{eventData.Pointer}' Game Object '{controllerPointer?.gameObject}' Old '{eventData.OldFocusedObject}' New '{eventData.NewFocusedObject}'");

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
        private PointerHandler handler;

        // Keeping this low by default so the test runs fast. Increase it to be able to see hand movements in the editor.
        private const int numFramesPerMove = 1;

        // Initializes MRTK, instantiates the test content prefab and adds a pointer handler to the test collider
        [SetUp]
        public void SetUp()
        {
            TestUtilities.InitializeMixedRealityToolkit(true);

            var testContent = AssetDatabase.LoadAssetAtPath<GameObject>(testContentPath);
            Assert.IsNotNull(testContent);

            testContent = Object.Instantiate(testContent);
            var collider = testContent.GetComponentInChildren<Collider>();
            Assert.IsNotNull(collider);

            handler = collider.gameObject.AddComponent<PointerHandler>();
        }

        [TearDown]
        public void TearDown()
        {
            TestUtilities.ShutdownMixedRealityToolkit();
        }

        [UnityTest]
        public IEnumerator Test()
        {
            // Step once to receive focus events from gaze pointer
            yield return null;

            var expected = new PointerHandler.State();
            expected.numBeforeFocusChange++;
            expected.numFocusChanged++;
            expected.numFocusEnter++;
            handler.state.AssertEqual(expected);

            //
            // Test that the correct pointer events are raised when using a single pointer
            //

            // Show hand far enough from the test collider to not give it focus
            var rightHand = new TestHand(Handedness.Right);
            Vector3 rightNoFocusPos = new Vector3(0.05f, 0, 1.0f);
            yield return rightHand.Show(rightNoFocusPos);

            // Should receive focus left events from gaze pointer
            expected.numBeforeFocusChange++;
            expected.numFocusChanged++;
            expected.numFocusExit++;
            handler.state.AssertEqual(expected);

            // Move hand closer to the collider so it gives it focus
            Vector3 rightFocusPos = new Vector3(0.05f, 0, 1.5f);
            yield return rightHand.MoveTo(rightFocusPos, numFramesPerMove);

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
            yield return rightHand.MoveTo(rightFocusPos, numFramesPerMove);

            expected.numPointerDragged += numFramesPerMove;
            handler.state.AssertEqual(expected);

            // Release pinch
            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Open);

            expected.numPointerUp++;
            expected.numPointerClicked++;
            handler.state.AssertEqual(expected);

            // Move back to no focus
            yield return rightHand.MoveTo(rightNoFocusPos, numFramesPerMove);

            expected.numBeforeFocusChange++;
            expected.numFocusChanged++;
            expected.numFocusExit++;
            handler.state.AssertEqual(expected);

            //
            // Test that the correct pointer events are raised when using two pointers
            //

            // Show left hand far enough from the test collider to not give it focus
            var leftHand = new TestHand(Handedness.Left);
            Vector3 leftNoFocusPos = new Vector3(-0.05f, 0, 1.0f);
            yield return leftHand.Show(leftNoFocusPos);

            // Make sure no events have been raised
            handler.state.AssertEqual(expected);

            // Move left hand to give focus
            Vector3 leftFocusPos = new Vector3(-0.05f, 0, 1.5f);
            yield return leftHand.MoveTo(leftFocusPos, numFramesPerMove);

            expected.numBeforeFocusChange++;
            expected.numFocusChanged++;
            expected.numFocusEnter++;
            handler.state.AssertEqual(expected);

            // Move right hand to give focus
            yield return rightHand.MoveTo(rightFocusPos, numFramesPerMove);

            // Focus enter should not be raised as the object is already in focus.
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
            yield return leftHand.MoveTo(leftNoFocusPos, numFramesPerMove);

            // Focus exit should not be raised as the object is still in focus.
            expected.numBeforeFocusChange++;
            expected.numFocusChanged++;
            handler.state.AssertEqual(expected);

            // Move right out of focus
            yield return rightHand.MoveTo(rightNoFocusPos, numFramesPerMove);

            expected.numBeforeFocusChange++;
            expected.numFocusChanged++;
            expected.numFocusExit++;
            handler.state.AssertEqual(expected);
        }
    }
}

#endif
