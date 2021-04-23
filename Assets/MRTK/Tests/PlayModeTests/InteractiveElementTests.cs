// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using System;
using System.Linq;

#if UNITY_2019_3_OR_NEWER
using Microsoft.MixedReality.Toolkit.Experimental.InteractiveElement;
#endif

namespace Microsoft.MixedReality.Toolkit.Tests
{
    /// <summary>
    /// Tests for an Interactive Element. 
    /// </summary>
    public class InteractiveElementTests : BasePlayModeTests
    {
#if UNITY_2019_3_OR_NEWER
        private string focusStateName = CoreInteractionState.Focus.ToString();
        private string focusNearStateName = CoreInteractionState.FocusNear.ToString();
        private string focusFarStateName = CoreInteractionState.FocusFar.ToString();
        private string touchStateName = CoreInteractionState.Touch.ToString();
        private string defaultStateName = CoreInteractionState.Default.ToString();
        private string selectFarStateName = CoreInteractionState.SelectFar.ToString();
        private string clickedStateName = CoreInteractionState.Clicked.ToString();
        private string toggleOnStateName = CoreInteractionState.ToggleOn.ToString();
        private string toggleOffStateName = CoreInteractionState.ToggleOff.ToString();

        // The PressedNear state is specific to the CompressableButton class and is not a CoreInteractionState
        private string pressedNearStateName = "PressedNear";
        private string newStateName = "MyNewState";

        private const string compressableCubeButtonPrefabGUID = "cd2f1e5c2c0ca934ab7b77568de1b45a";
        private readonly string compressableCubeButtonPrefabPath = AssetDatabase.GUIDToAssetPath(compressableCubeButtonPrefabGUID);

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
            eventConfigurationFocusFar.OnFocusOff.AddListener((eventData) => { onFocusFarOff = true; });

            // Create a new hand and initialize it with an object in focus
            var leftHand = new TestHand(Handedness.Left);
            yield return ShowHandWithObjectInFocus(leftHand);

            // Make sure the Focus state and Focus Far state are on
            Assert.AreEqual(1, focusState.Value);
            Assert.AreEqual(1, focusFarState.Value);
            Assert.True(onFocusFarOn);

            // Move the Hand out of focus 
            yield return MoveHandOutOfFocus(leftHand);

            // Make sure the Focus state and Focus Far state are off
            Assert.AreEqual(0, focusState.Value);
            Assert.AreEqual(0, focusFarState.Value);
            Assert.True(onFocusFarOff);

            // Move hand to a near focus position
            yield return leftHand.Move(new Vector3(0, 0.22f, 0.221f));

            // Make sure the Focus state and Focus Near state are on
            Assert.AreEqual(1, focusState.Value);
            Assert.AreEqual(1, focusNearState.Value);
            Assert.True(onFocusNearOn);

            // Move the Hand out of focus 
            yield return leftHand.Hide();

            // Make sure the Focus state and Focus Near state are off
            Assert.AreEqual(0, focusState.Value);
            Assert.AreEqual(0, focusNearState.Value);
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
        /// Test SelectFar state and setting the global property.
        /// </summary>
        [UnityTest]
        public IEnumerator TestSelectFarEventConfiguration()
        {
            // Create an interactive cube 
            InteractiveElement interactiveElement = CreateInteractiveCube();
            yield return null;

            // Add the selectFar state
            InteractionState selectFar = interactiveElement.AddNewState(selectFarStateName);
            yield return null;

            // Get the event configuration for the SelectFar state
            var eventConfiguration = interactiveElement.GetStateEvents<SelectFarEvents>(selectFarStateName);

            // Set global to true, this registers the IMixedRealityPointerHandler
            eventConfiguration.Global = true;

            bool onSelectDown = false;
            bool onSelectHold = false;
            bool onSelectClicked = false;
            bool onSelectUp = false;

            eventConfiguration.OnSelectDown.AddListener((eventData) => { onSelectDown = true; });
            eventConfiguration.OnSelectHold.AddListener((eventData) => { onSelectHold = true; });
            eventConfiguration.OnSelectClicked.AddListener((eventData) => { onSelectClicked = true; });
            eventConfiguration.OnSelectUp.AddListener((eventData) => { onSelectUp = true; });

            // Create a new hand and initialize it with an object in focus
            var leftHand = new TestHand(Handedness.Left);

            // Show hand at starting position
            yield return ShowHandWithObjectInFocus(leftHand);

            yield return MoveHandOutOfFocus(leftHand);

            // Click the hand to trigger far select events
            yield return leftHand.Click();

            Assert.True(onSelectDown);
            Assert.True(onSelectHold);
            Assert.True(onSelectClicked);
            Assert.True(onSelectUp);

            eventConfiguration.Global = false;

            yield return leftHand.SetGesture(ArticulatedHandPose.GestureId.Pinch);

            // Make sure the SelectFar state is not active after setting global to false without an object in focus
            Assert.AreEqual(0, selectFar.Value);
        }


        /// <summary>
        /// Test Clicked state with a far interaction click as the entry point.
        /// </summary>
        [UnityTest]
        public IEnumerator TestClickedEventConfiguration()
        {
            // Create an interactive cube 
            InteractiveElement interactiveElement = CreateInteractiveCube();
            yield return null;

            // Add the clicked state
            InteractionState clicked = interactiveElement.AddNewState(clickedStateName);
            yield return null;

            // Get the event configuration for the Clicked state
            var eventConfiguration = interactiveElement.GetStateEvents<ClickedEvents>(clickedStateName);

            bool onClicked = false;

            eventConfiguration.OnClicked.AddListener(() => { onClicked = true; });

            // Create a new hand and initialize it with an object in focus
            var leftHand = new TestHand(Handedness.Left);

            // Show hand at starting position
            yield return ShowHandWithObjectInFocus(leftHand);

            // Click the hand to trigger far select events
            yield return leftHand.Click();

            Assert.True(onClicked);
        }

        /// <summary>
        /// Test the events of the ToggleOn and ToggleOff states
        /// </summary>
        [UnityTest]
        public IEnumerator TestToggleEvents()
        {
            // Create an interactive cube 
            InteractiveElement interactiveElement = CreateInteractiveCube();
            yield return null;

            interactiveElement.AddToggleStates();
            yield return null;

            // Get the event configuration for the ToggleOn state
            var eventConfigurationToggleOn = interactiveElement.GetStateEvents<ToggleOnEvents>(toggleOnStateName);

            // Get the event configuration for the ToggleOn state
            var eventConfigurationToggleOff = interactiveElement.GetStateEvents<ToggleOffEvents>(toggleOffStateName);

            bool onToggleOn = false;
            bool onToggleOff = false;

            eventConfigurationToggleOn.OnToggleOn.AddListener(() => { onToggleOn = true; });
            eventConfigurationToggleOff.OnToggleOff.AddListener(() => { onToggleOff = true; });

            interactiveElement.SetToggleStates();
            yield return null;

            // Make sure the toggle is on at the start
            Assert.True(onToggleOn);

            // Create a new hand and initialize it with an object in focus
            var leftHand = new TestHand(Handedness.Left);

            // Show hand at starting position
            yield return ShowHandWithObjectInFocus(leftHand);

            // Click the hand 
            yield return leftHand.Click();

            Assert.True(onToggleOff);
        }

        /// <summary>
        /// Test adding and removing states
        /// </summary>
        [UnityTest]
        public IEnumerator TestAddAndRemoveAllStates()
        {
            // Create an interactive cube 
            InteractiveElement interactiveElement = CreateInteractiveCube();
            yield return null;

            string[] coreStates = Enum.GetNames(typeof(CoreInteractionState)).ToArray();

            foreach (string coreStateName in coreStates)
            {
                // The Default and Focus states are present by default
                if (coreStateName != defaultStateName && coreStateName != focusStateName)
                {
                    interactiveElement.AddNewState(coreStateName);
                    yield return null;
                }
            }

            foreach (string coreStateName in coreStates)
            {
                Assert.IsNotNull(interactiveElement.GetState(coreStateName), $"The {coreStateName} state is null after it was added.");
                yield return null;
            }

            foreach (string coreStateName in coreStates)
            {
                if (coreStateName != defaultStateName)
                {
                    interactiveElement.RemoveState(coreStateName);
                    yield return null;
                }
            }

            foreach (string coreStateName in coreStates)
            {
                if (coreStateName != defaultStateName)
                {
                    Assert.IsNull(interactiveElement.GetState(coreStateName), $"The {coreStateName} state is not null after it was removed.");
                    yield return null;
                }
            }

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
            Assert.AreEqual(0, myNewState.Value);

            // Create a new hand and initialize it with an object in focus
            var leftHand = new TestHand(Handedness.Left);
            yield return ShowHandWithObjectInFocus(leftHand);

            // Make sure the value of MyNewState was changed when the object is in focus
            Assert.AreEqual(1, myNewState.Value);

            // Move hand away from object to remove focus
            yield return MoveHandOutOfFocus(leftHand);

            // Make sure the value of MyNewState was changed when the object is no longer in focus
            Assert.AreEqual(0, myNewState.Value);
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
            Assert.AreEqual(0, touchState.Value);
            Assert.AreEqual(0, focusState.Value);

            // Create a new hand and initialize it with an object in focus
            var leftHand = new TestHand(Handedness.Left);
            yield return ShowHandWithObjectInFocus(leftHand);

            // Move hand to Touch the object
            yield return MoveHandTouchObject(leftHand);

            // Make sure the values change when the hand is moved to touch the cube
            Assert.AreEqual(1, touchState.Value);
            Assert.AreEqual(1, focusState.Value);
            yield return null;

            yield return MoveHandOutOfFocus(leftHand);

            // Set Active to false to disable internal updates
            interactiveElement.Active = false;

            // Show hand at starting position
            yield return ShowHandWithObjectInFocus(leftHand);

            // Move hand to Touch the object
            yield return MoveHandTouchObject(leftHand);

            // Make sure the values do not change when the hand is moved to touch the cube
            Assert.AreEqual(0, touchState.Value);
            Assert.AreEqual(0, focusState.Value);
            yield return null;
        }

        #region Compressable Button

        /// <summary>
        /// Test the CompressableButtonCube prefab which contains the PressedNear state.
        /// </summary>
        [UnityTest]
        public IEnumerator TestCompressableButtonCube()
        {
            // Instantiate the CompressableCubeButton prefab
            CompressableButton compressableButton = InstantiateCompressablePrefab(compressableCubeButtonPrefabPath);

            InteractionState pressedNearState = compressableButton.GetState(pressedNearStateName);
            InteractionState touchState = compressableButton.GetState(touchStateName);

            var pressedNearEvents = compressableButton.GetStateEvents<PressedNearEvents>("PressedNear");

            bool buttonPressed = false;
            bool buttonHold = false;
            bool buttonReleased = false;

            // Add PressedNear state event listeners
            pressedNearEvents.OnButtonPressed.AddListener(() => { buttonPressed = true; });
            pressedNearEvents.OnButtonPressHold.AddListener(() => { buttonHold = true; });
            pressedNearEvents.OnButtonPressReleased.AddListener(() => { buttonReleased = true; });

            // Create a new hand and initialize it with an object in focus
            var leftHand = new TestHand(Handedness.Left);
            yield return ShowHandWithObjectInFocus(leftHand);

            // Move hand to press compressable button, which will set the PressedNear state to on
            yield return MoveHandPressObject(leftHand);

            // Make sure the event listeners were triggered and that the PressedNear and Touch state are on
            Assert.True(buttonPressed);
            Assert.True(buttonHold);
            Assert.AreEqual(pressedNearState.Value, 1);
            Assert.AreEqual(touchState.Value, 1);

            yield return MoveHandOutOfFocus(leftHand);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            // Make sure button released event was triggered and states are set to off 
            Assert.True(buttonReleased);
            Assert.AreEqual(pressedNearState.Value, 0);
            Assert.AreEqual(touchState.Value, 0);
        }

        #endregion

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
            yield return hand.Move(new Vector3(0, -0.4f, 0));
        }

        private IEnumerator MoveHandTouchObject(TestHand hand)
        {
            yield return hand.Move(new Vector3(0, -0.25f, 0.221f));
        }

        private IEnumerator MoveHandPressObject(TestHand hand)
        {
            yield return hand.Move(new Vector3(0, -0.25f, 0.25f));
        }

        // Instantiate a prefab that has CompressableButton attached
        private CompressableButton InstantiateCompressablePrefab(string path)
        {
            UnityEngine.Object interactablePrefab = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));
            GameObject result = UnityEngine.Object.Instantiate(interactablePrefab) as GameObject;

            return result.GetComponent<CompressableButton>();
        }

        #endregion
#endif
    }
}

