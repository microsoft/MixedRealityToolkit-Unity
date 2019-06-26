// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if !WINDOWS_UWP
// When the .NET scripting backend is enabled and C# projects are built
// Unity doesn't include the the required assemblies (i.e. the ones below).
// Given that the .NET backend is deprecated by Unity at this point it's we have
// to work around this on our end.
using Microsoft.MixedReality.Toolkit.UI;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using UnityEditor;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using Microsoft.MixedReality.Toolkit.Input;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    class MyPointerHandler : MonoBehaviour, IMixedRealityFocusChangedHandler, IMixedRealityFocusHandler, IMixedRealityPointerHandler
    {
        public struct State
        {
            public int numBeforeFocusChange;
            public int numFocusChanged;
            public int numFocusEnter;
            public int numFocusExit;
            public int numPointerDown;
            public int numPointerUp;
            public int numPointerClicked;
            public int numPointerDragged;
        }

        public State state;
        int numFocusing;

        void IMixedRealityFocusChangedHandler.OnBeforeFocusChange(FocusEventData eventData)
        {
            Assert.AreNotSame(eventData.OldFocusedObject, eventData.NewFocusedObject);
            Assert.AreEqual(state.numFocusChanged, state.numBeforeFocusChange);

            Debug.Log("OnBeforeFocusChange");

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

    public class PointerEventTests
    {
        private InputSimulationService inputSimulation;
        private int numSteps = 30;

        private MyPointerHandler SetupScene()
        {
            TestUtilities.InitializeMixedRealityToolkit(true);

            inputSimulation = PlayModeTestUtilities.GetInputSimulationService();

            var testContent = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/MixedRealityToolkit.Tests/PlayModeTests/Prefabs/PointerEventsTests.prefab");
            Assert.IsNotNull(testContent);

            testContent = Object.Instantiate(testContent);
            var collider = testContent.GetComponentInChildren<Collider>();
            Assert.IsNotNull(collider);

            return collider.gameObject.AddComponent<MyPointerHandler>();
        }

        class Hand
        {
            public int numSteps = 30;
            private Handedness handedness;
            private Vector3 position;
            private ArticulatedHandPose.GestureId gestureId = ArticulatedHandPose.GestureId.Open;
            private InputSimulationService simulationService;

            public Hand(Handedness handedness, Vector3 position)
            {
                this.handedness = handedness;
                this.position = position;
                simulationService = PlayModeTestUtilities.GetInputSimulationService();
            }

            public void Show()
            {
                PlayModeTestUtilities.ShowHand(handedness, simulationService);
            }

            public void Hide()
            {
                PlayModeTestUtilities.ShowHand(handedness, simulationService);
            }

            public IEnumerator Move(Vector3 newPosition)
            {
                Vector3 oldPosition = position;
                position = newPosition;
                return PlayModeTestUtilities.MoveHandFromTo(oldPosition, newPosition, numSteps, gestureId, handedness, simulationService);
            }

            public void SetGesture(ArticulatedHandPose.GestureId newGestureId)
            {
                gestureId = newGestureId;
                PlayModeTestUtilities.MoveHandFromTo(position, position, 1, gestureId, handedness, simulationService);
            }
        }

        private IEnumerator MoveHand(Handedness handedness, Vector3 from, Vector3 to, ArticulatedHandPose.GestureId gestureId)
        {
            return PlayModeTestUtilities.MoveHandFromTo(from, to, numSteps, gestureId, handedness, inputSimulation);
        }

        static IEnumerator Count(int count)
        {
            for (int i = 0; i < count; ++i)
            {
                Debug.Log($"Count {i}");
                yield return null;
            }
        }

        static IEnumerator Enumerate(int count)
        {
            for (int i = 0; i < count; ++i)
            {
                Debug.Log($"Enumerate {i}");
                yield return Count(i);
            }
        }

        [UnityTest]
        public IEnumerator Test()
        {
            var handler = SetupScene();

            var a = Enumerate(3);
            while (a.MoveNext())
            {
                //Debug.Log($"Iteration {a.Current}");
            }

            Vector3 p1 = new Vector3(0.05f, 0, 0.5f);
            Vector3 p2 = new Vector3(0.05f, 0, 1.5f);

            yield return PlayModeTestUtilities.ShowHand(Handedness.Right, inputSimulation);
            yield return MoveHand(Handedness.Right, p1, p2, ArticulatedHandPose.GestureId.Open);

            var expected = new MyPointerHandler.State();
            expected.numBeforeFocusChange++;
            expected.numFocusChanged++;
            expected.numFocusEnter++;
            Assert.AreEqual(expected, handler.state);

            yield return MoveHand(Handedness.Right, p2, p2, ArticulatedHandPose.GestureId.Pinch);

            expected.numPointerDown++;
            expected.numPointerDragged += numSteps;
            Assert.AreEqual(expected, handler.state);

            yield return MoveHand(Handedness.Right, p2, p2, ArticulatedHandPose.GestureId.Open);

            expected.numPointerUp++;
            expected.numPointerClicked++;
            Assert.AreEqual(expected, handler.state);

            yield return MoveHand(Handedness.Right, p1, p1, ArticulatedHandPose.GestureId.Open);

            expected.numBeforeFocusChange++;
            expected.numFocusChanged++;
            expected.numFocusExit++;
            Assert.AreEqual(expected, handler.state);

            yield return PlayModeTestUtilities.HideHand(Handedness.Right, inputSimulation);
        }
    }
}


#endif