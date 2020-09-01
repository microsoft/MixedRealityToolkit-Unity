// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.UI.Interaction;
using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    /// <summary>
    /// Tests for a BaseInteractiveElement and the StateVisualizer
    /// </summary>
    public class InteractiveElementTests : BasePlayModeTests
    {
        /// <summary>
        /// Tests adding listeners to the event configuration of the focus state.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator TestSettingEventConfiguraitonsOfATrackedCoreState()
        {
            // Create an interactive cube 
            InteractiveElement interactiveElement = CreateInteractiveCube();
            yield return null;

            // The focus state is a state that is added by default
            InteractionState focusState = interactiveElement.GetState(CoreInteractionState.Focus);
            yield return null;

            // Get the event configuration for the focus state
            FocusInteractionEventConfiguration eventConfiguration = focusState.EventConfiguration as FocusInteractionEventConfiguration;

            bool onFocusOn = false;
            bool onFocusOff = false;

            eventConfiguration.OnFocusOn.AddListener((eventData) =>
                {
                    onFocusOn = true;
                });

            eventConfiguration.OnFocusOff.AddListener((eventData) =>
                {
                    onFocusOff = true;
                });

            // Create a new hand and initialize it with an object in focus
            var leftHand = new TestHand(Handedness.Left);
            yield return ShowHandWithObjectInFocus(leftHand);

            // Check if OnFocusOn has fired
            Assert.True(onFocusOn);

            // Move the Hand out of focus 
            yield return MoveHandOutOfFocus(leftHand);

            // Check if OnFocusOn has fired
            Assert.True(onFocusOff);

            yield return null;
        }

        /// <summary>
        /// Test creating a new state and setting the values of the new state.
        /// </summary>
        [UnityTest]
        public IEnumerator TestAddingAndSettingNewState()
        {
            // Create an interactive cube 
            InteractiveElement interactiveElement = CreateInteractiveCube();
            yield return null;

            // Create a new state and add it to Tracked States
            interactiveElement.AddNewState("MyNewState");

            // Change the value of my new state by using the focus state events to set the new state
            InteractionState focusState = interactiveElement.GetState(CoreInteractionState.Focus);

            FocusInteractionEventConfiguration focusEventConfiguration = focusState.EventConfiguration as FocusInteractionEventConfiguration;

            focusEventConfiguration.OnFocusOn.AddListener((focusEventData) => 
            {
                // When the object comes into focus, set my new state to on
                interactiveElement.SetStateOn("MyNewState");
            });

            focusEventConfiguration.OnFocusOff.AddListener((focusEventData) =>
            {
                // When the object comes out of  focus, set my new state to off
                interactiveElement.SetStateOff("MyNewState");
            });

            // Make sure MyNewState is being tracked
            InteractionState myNewState = interactiveElement.GetState("MyNewState");
            Assert.IsNotNull(myNewState);

            // Make sure the value is 0/off initially
            Assert.AreEqual(myNewState.Value, 0);

            // Create a new hand and initialize it with an object in focus
            var leftHand = new TestHand(Handedness.Left);
            yield return ShowHandWithObjectInFocus(leftHand);

            // Make sure the value of MyNewState was changed when the object is in focus
            Assert.AreEqual(myNewState.Value, 1);

            // Move hand away from object to remove focus
            yield return MoveHandOutOfFocus(leftHand);

            // Make sure the value of MyNewState was changed when the object is no longer in focus
            Assert.AreEqual(myNewState.Value, 0);
        }


        #region Interaction Tests Helpers

        private InteractiveElement CreateInteractiveCube()
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = new Vector3(0, 0, 0.7f);
            cube.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

            InteractiveElement basicButton = cube.AddComponent<InteractiveElement>();

            return basicButton;
        }

        private IEnumerator ShowHandWithObjectInFocus(TestHand hand)
        {
            // Set hand position 
            Vector3 handStartPosition = new Vector3(0, 0.2f, 0.4f);
            yield return hand.Show(handStartPosition);
        }

        private IEnumerator MoveHandOutOfFocus(TestHand hand)
        {
            yield return hand.Move(new Vector3(0, -0.3f, 0), 30);
        }

        #endregion


    }
}
