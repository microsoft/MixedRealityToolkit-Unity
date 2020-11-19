// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.UI.Interaction;
using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    /// <summary>
    /// Tests for an Interactive Element. 
    /// </summary>
    public class InteractiveElementTests : BasePlayModeTests
    {
        private string focusStateName = CoreInteractionState.Focus.ToString();
        private string focusNearStateName = CoreInteractionState.FocusNear.ToString();
        private string focusFarStateName = CoreInteractionState.FocusFar.ToString();
        private string touchStateName = CoreInteractionState.Touch.ToString();
        private string defaultStateName = CoreInteractionState.Default.ToString();
        private string newStateName = "MyNewState";

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
            InteractionState focusState = interactiveElement.GetState(focusStateName);
            yield return null;

            // Get the event configuration for the focus state
            var eventConfiguration = interactiveElement.GetStateEvents<FocusEvents>(focusStateName);

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
        /// Test the state changes and events for the FocusNear and FocusFar states.
        /// </summary>
        [UnityTest]
        public IEnumerator TestFocusNearFarEventConfiguration()
        {
            // Create an interactive cube 
            InteractiveElement interactiveElement = CreateInteractiveCube();
            yield return null;

            // The focus state is a state that is added by default
            InteractionState focusState = interactiveElement.GetState(focusStateName);
            yield return null;

            // Add FocusNear state
            InteractionState focusNearState = interactiveElement.AddNewState(focusNearStateName);
            yield return null;

            // Add FocusFar state
            InteractionState focusFarState = interactiveElement.AddNewState(focusFarStateName);
            yield return null;

            // Get event configuration for the states
            var eventConfigurationFocusNear = interactiveElement.GetStateEvents<FocusEvents>(focusNearStateName);
            var eventConfigurationFocusFar = interactiveElement.GetStateEvents<FocusEvents>(focusFarStateName);

            // Define flags for events
            bool onFocusNearOn = false;
            bool onFocusNearOff = false;
            bool onFocusFarOn = false;
            bool onFocusFarOff = false;

            // Add Focus Near event listeners 
            eventConfigurationFocusNear.OnFocusOn.AddListener((eventData) => { onFocusNearOn = true; });
            eventConfigurationFocusNear.OnFocusOff.AddListener((eventData) => { onFocusNearOff = true; });

            // Add Focus Far event listeners 
            eventConfigurationFocusFar.OnFocusOn.AddListener((eventData) => { onFocusFarOn = true; });
            eventConfigurationFocusFar.OnFocusOff.AddListener((eventData) =>{ onFocusFarOff = true; });

            // Create a new hand and initialize it with an object in focus
            var leftHand = new TestHand(Handedness.Left);
            yield return ShowHandWithObjectInFocus(leftHand);

            // Make sure the Focus state and Focus Far state are on
            Assert.AreEqual(focusState.Value, 1);
            Assert.AreEqual(focusFarState.Value, 1);
            Assert.True(onFocusFarOn);

            // Move the Hand out of focus 
            yield return MoveHandOutOfFocus(leftHand);

            // Make sure the Focus state and Focus Far state are off
            Assert.AreEqual(focusState.Value, 0);
            Assert.AreEqual(focusFarState.Value, 0);
            Assert.True(onFocusFarOff);

            // Move hand to a near focus position
            yield return leftHand.Move(new Vector3(0, 0.22f, 0.221f));

            // Make sure the Focus state and Focus Near state are on
            Assert.AreEqual(focusState.Value, 1);
            Assert.AreEqual(focusNearState.Value, 1);
            Assert.True(onFocusNearOn);

            // Move the Hand out of focus 
            yield return leftHand.Hide();

            // Make sure the Focus state and Focus Near state are off
            Assert.AreEqual(focusState.Value, 0);
            Assert.AreEqual(focusNearState.Value, 0);
            Assert.True(onFocusNearOff);
        }

        /// <summary>
        /// Tests adding listeners to the event configuration of the touch state.
        /// </summary>
        [UnityTest]
        public IEnumerator TestTouchEventConfiguration()
        {
            // Create an interactive cube 
            InteractiveElement interactiveElement = CreateInteractiveCube();
            yield return null;

            // Add the touch state
            InteractionState touchState = interactiveElement.AddNewState(touchStateName);
            yield return null;

            // Get the event configuration for the touch state
            var eventConfiguration = interactiveElement.GetStateEvents<TouchEvents>(touchStateName);

            bool onTouchStarted = false;
            bool onTouchCompleted = false;
            bool onTouchUpdated = false;

            eventConfiguration.OnTouchStarted.AddListener((eventData) =>
            {
                onTouchStarted = true;
            });

            eventConfiguration.OnTouchCompleted.AddListener((eventData) =>
            {
                onTouchCompleted = true;
            });

            eventConfiguration.OnTouchUpdated.AddListener((eventData) =>
            {
                onTouchUpdated = true;
            });

            // Create a new hand and initialize it with an object in focus
            var leftHand = new TestHand(Handedness.Left);

            // Show hand at starting position
            yield return ShowHandWithObjectInFocus(leftHand);

            // Move hand to Touch the object
            yield return MoveHandTouchObject(leftHand);

            Assert.True(onTouchStarted);
            yield return null;

            Assert.True(onTouchUpdated);
            yield return null;

            yield return MoveHandOutOfFocus(leftHand);

            // Make sure the touch has completed when the hand moves off the object
            Assert.True(onTouchCompleted);
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
            interactiveElement.AddNewState(newStateName);

            // Change the value of my new state by using the focus state events to set the new state
            InteractionState focusState = interactiveElement.GetState(CoreInteractionState.Focus.ToString());

            var focusEventConfiguration = interactiveElement.GetStateEvents<FocusEvents>(focusStateName);
            yield return null;

            focusEventConfiguration.OnFocusOn.AddListener((focusEventData) =>
            {
                // When the object comes into focus, set my new state to on
                interactiveElement.SetStateOn(newStateName);
            });

            focusEventConfiguration.OnFocusOff.AddListener((focusEventData) =>
            {
                // When the object comes out of  focus, set my new state to off
                interactiveElement.SetStateOff(newStateName);
            });

            // Make sure MyNewState is being tracked
            InteractionState myNewState = interactiveElement.GetState(newStateName);
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
            interactiveElement.AddNewState(newStateName);

            bool stateActivated = false;

            // Use the OnStateActivated event in the State Manager to set stateActivated
            interactiveElement.StateManager.OnStateActivated.AddListener((state) =>
            {
                if (state.Name == newStateName)
                {
                    stateActivated = true;
                }
            });

            // Set the state on
            interactiveElement.SetStateOn(newStateName);

            // Make sure the state was activated
            Assert.True(stateActivated);
        }

        /// <summary>
        /// Test the OnStateActivated event in the State Manager.
        /// </summary>
        [UnityTest]
        public IEnumerator TestEventConfigOfNewState()
        {
            // Create an interactive cube 
            InteractiveElement interactiveElement = CreateInteractiveCube();
            yield return null;

            // Create a new state
            interactiveElement.AddNewState(newStateName);

            // Get the event configuration 
            var eventConfiguration = interactiveElement.GetStateEvents<StateEvents>(newStateName);

            bool onStateOn = false;
            bool onStateOff = false;

            eventConfiguration.OnStateOn.AddListener(() =>
            {
                onStateOn = true;
            });

            eventConfiguration.OnStateOff.AddListener(() =>
            {
                onStateOff = true;
            });

            interactiveElement.SetStateOn(newStateName);
            yield return null;

            // Check if OnFocusOn has fired
            Assert.True(onStateOn);

            interactiveElement.SetStateOff(newStateName);
            yield return null;

            // Check if OnFocusOn has fired
            Assert.True(onStateOff);

            yield return null;
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
                interactiveElement.AddNewState("State" + i.ToString());
                yield return null;
            }

            // The Default state should only be active if no other states are active
            Assert.True(interactiveElement.IsStateActive(defaultStateName));

            // Set each new states to active
            for (int i = 0; i < newStateCount; i++)
            {
                interactiveElement.SetStateOn("State" + i.ToString());

                // If any other state is active, the Default state should not be active 
                Assert.False(interactiveElement.IsStateActive(defaultStateName));
            }

            // Set all states to not be active 
            for (int i = 0; i < newStateCount; i++)
            {
                interactiveElement.SetStateOff("State" + i.ToString());
            }

            yield return null;

            // After all the states are deactivated, the Default state should be active
            Assert.True(interactiveElement.IsStateActive(defaultStateName));
        }

        /// <summary>
        /// Test the Active property on interactive element by toggling the property and checking
        /// if states are updated
        /// </summary>
        [UnityTest]
        public IEnumerator TestActiveInactive()
        {
            // Create an interactive cube 
            InteractiveElement interactiveElement = CreateInteractiveCube();
            yield return null;

            // Get the focus state
            InteractionState focusState = interactiveElement.GetState(focusStateName);

            // Add the touch state
            InteractionState touchState = interactiveElement.AddNewState(touchStateName);
            yield return null;

            Assert.True(interactiveElement.Active);

            // Make sure the Focus and Touch state are not on
            Assert.AreEqual(touchState.Value, 0);
            Assert.AreEqual(focusState.Value, 0);

            // Create a new hand and initialize it with an object in focus
            var leftHand = new TestHand(Handedness.Left);
            yield return ShowHandWithObjectInFocus(leftHand);

            // Move hand to Touch the object
            yield return MoveHandTouchObject(leftHand);

            // Make sure the values change when the hand is moved to touch the cube
            Assert.AreEqual(touchState.Value, 1);
            Assert.AreEqual(focusState.Value, 1);
            yield return null;

            yield return MoveHandOutOfFocus(leftHand);

            // Set Active to false to disable internal updates
            interactiveElement.Active = false;

            // Show hand at starting position
            yield return ShowHandWithObjectInFocus(leftHand);

            // Move hand to Touch the object
            yield return MoveHandTouchObject(leftHand);

            // Make sure the values do not change when the hand is moved to touch the cube
            Assert.AreEqual(touchState.Value, 0);
            Assert.AreEqual(focusState.Value, 0);
            yield return null;
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
            yield return hand.Move(new Vector3(0, -0.3f, 0));
        }

        private IEnumerator MoveHandTouchObject(TestHand hand)
        {
            yield return hand.Move(new Vector3(0, -0.25f, 0.221f));
        }

        #endregion
    }
}
