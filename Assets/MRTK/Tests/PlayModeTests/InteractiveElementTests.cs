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
    /// Tests for an Interactive Element. 
    /// </summary>
    public class InteractiveElementTests : BasePlayModeTests
    {
        /// <summary>
        /// Tests adding listeners to the event configuration of the focus state.
        /// </summary>
        [UnityTest]
        public IEnumerator TestFocusEventConfiguration()
        {
            // Create an interactive cube 
            InteractiveElement interactiveElement = CreateInteractiveCube();
            yield return null;

            // The focus state is a state that is added by default
            InteractionState focusState = interactiveElement.GetState(CoreInteractionState.Focus);
            yield return null;

            // Get the event configuration for the focus state
            var eventConfiguration = interactiveElement.GetStateEvents<FocusEvents>("Focus");

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

            var focusEventConfiguration = interactiveElement.GetStateEvents<FocusEvents>("Focus");

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

        /// <summary>
        /// Test the OnStateActivated event in the State Manager.
        /// </summary>
        [UnityTest]
        public IEnumerator TestOnStateActivatedNewState()
        {
            // Create an interactive cube 
            InteractiveElement interactiveElement = CreateInteractiveCube();
            yield return null;

            // Create a new state
            interactiveElement.AddNewState("MyNewState");

            bool stateActivated = false;

            // Use the OnStateActivated event in the State Manager to set stateActivated
            interactiveElement.StateManager.OnStateActivated.AddListener((state) =>
            {
                if (state.Name == "MyNewState")
                {
                    stateActivated = true;
                }
            });

            // Set the state on
            interactiveElement.SetStateOn("MyNewState");

            // Make sure the state was activated
            Assert.True(stateActivated);  
        }

        /// <summary>
        /// Test the internal setting of the Default state.
        /// </summary>
        [UnityTest]
        public IEnumerator TestDefaultStateSetting()
        {
            // Create an interactive cube 
            InteractiveElement interactiveElement = CreateInteractiveCube();
            yield return null;

            int newStateCount = 5;

            // Add new states
            for (int i = 0; i < newStateCount; i++)
            {
                interactiveElement.AddNewState("State"+ i.ToString());
                yield return null;
            }

            // The Default state should only be active if no other states are active
            Assert.True(interactiveElement.IsStateActive("Default"));

            // Set each new states to active
            for (int i = 0; i < newStateCount; i++)
            {
                interactiveElement.SetStateOn("State" + i.ToString());

                // If any other state is active, the Default state should not be active 
                Assert.False(interactiveElement.IsStateActive("Default"));
            }

            // Set all states to not be active 
            for (int i = 0; i < newStateCount; i++)
            {
                interactiveElement.SetStateOff("State" + i.ToString());   
            }

            yield return null;

            // After all the states are deactivated, the Default state should be active
            Assert.True(interactiveElement.IsStateActive("Default"));
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
