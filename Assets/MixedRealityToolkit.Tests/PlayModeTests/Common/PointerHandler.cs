// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using NUnit.Framework;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    /// <summary>
    /// A test implementation of pointer interfaces used to track the number of pointer related events.
    /// </summary>
    public class PointerHandler : MonoBehaviour, IMixedRealityFocusChangedHandler, IMixedRealityFocusHandler, IMixedRealityPointerHandler
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

                // Note that numPointerDragged is not asserted equal, because depending on
                // how many frames elapse during the test, the number of times something is
                // dragged can vary.
            }
        }

        public State state = new State();
        private int numFocusing;

        void IMixedRealityFocusChangedHandler.OnBeforeFocusChange(FocusEventData eventData)
        {
            Assert.AreNotSame(eventData.OldFocusedObject, eventData.NewFocusedObject);
            Assert.AreEqual(state.numFocusChanged, state.numBeforeFocusChange);

            // Uncomment to help debugging issues
            // var controllerPointer = eventData.Pointer as BaseControllerPointer;
            // Debug.Log($"Pointer '{eventData.Pointer}' Game Object '{controllerPointer?.gameObject}' Old '{eventData.OldFocusedObject}' New '{eventData.NewFocusedObject}'");

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
}
