// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
#if !WINDOWS_UWP
// When the .NET scripting backend is enabled and C# projects are built
// Unity doesn't include the required assemblies (i.e. the ones below).
// Given that the .NET backend is deprecated by Unity at this point it's we have
// to work around this on our end.
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    class InteractableTests
    {
        const float buttonPressAnimationDelay = 0.15f;
        const float buttonReleaseAnimationDelay = 0.75f;

        [SetUp]
        public void Setup()
        {
            PlayModeTestUtilities.Setup();
            TestUtilities.FaceCameraForward();
        }

        [TearDown]
        public void TearDown()
        {
            PlayModeTestUtilities.TearDown();
        }

        /// <summary>
        /// Tests that an interactable component can be added to a GameObject
        /// at runtime.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator TestAddInteractableAtRuntime()
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            // This should not throw an exception
            var interactable = cube.AddComponent<Interactable>();

            // clean up
            GameObject.Destroy(cube);
            yield return null;
        }

        /// <summary>
        /// Instantiates a push button prefab and uses simulated hand input to press it.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator TestSimulatedHandInputOnPrefab()
        {
            // Load interactable prefab
            GameObject interactableObject;
            Interactable interactable;
            Transform translateTargetObject;

            InstantiateDefaultInteractablePrefab(
                new Vector3(0.025f, 0.05f, 0.5f),
                new Vector3(-90f, 0f, 0f),
                out interactableObject, 
                out interactable,
                out translateTargetObject);

            // Subscribe to interactable's on click so we know the click went through
            bool wasClicked = false;
            interactable.OnClick.AddListener(() => { wasClicked = true; });

            Vector3 targetStartPosition = translateTargetObject.localPosition;
            
            // Move the hand forward to intersect the interactable
            var inputSimulationService = PlayModeTestUtilities.GetInputSimulationService();
            int numSteps = 32;
            Vector3 p1 = new Vector3(0.0f, 0f, 0f);
            Vector3 p2 = new Vector3(0.05f, 0f, 0.51f);
            Vector3 p3 = new Vector3(0.0f, 0f, 0.0f);

            yield return PlayModeTestUtilities.ShowHand(Handedness.Right, inputSimulationService);
            yield return PlayModeTestUtilities.MoveHandFromTo(p1, p2, numSteps, ArticulatedHandPose.GestureId.Poke, Handedness.Right, inputSimulationService);
            yield return new WaitForSeconds(buttonPressAnimationDelay);

            Assert.AreNotEqual(targetStartPosition, translateTargetObject.localPosition, "Transform target object was not translated by action.");

            // Move the hand back
            yield return PlayModeTestUtilities.MoveHandFromTo(p2, p3, numSteps, ArticulatedHandPose.GestureId.Poke, Handedness.Right, inputSimulationService);
            yield return PlayModeTestUtilities.HideHand(Handedness.Right, inputSimulationService);
            yield return new WaitForSeconds(buttonReleaseAnimationDelay);

            Assert.True(wasClicked, "Interactable was not clicked.");
            Assert.AreEqual(targetStartPosition, translateTargetObject.localPosition, "Transform target object was not translated back by action.");

            Object.Destroy(interactableObject);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        /// <summary>
        /// Instantiates a push button prefab and uses simulated input events to press it.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator TestSimulatedMenuInputOnPrefab()
        {
            // Load interactable prefab
            GameObject interactableObject;
            Interactable interactable;
            Transform translateTargetObject;

            InstantiateDefaultInteractablePrefab(
                new Vector3(0.0f, 0.0f, 0.5f),
                new Vector3(-90f, 0f, 0f),
                out interactableObject,
                out interactable,
                out translateTargetObject);

            // Subscribe to interactable's on click so we know the click went through
            bool wasClicked = false;
            interactable.OnClick.AddListener(() => { wasClicked = true; });

            Vector3 targetStartPosition = translateTargetObject.localPosition;

            yield return null;

            // Find the menu action from the input system profile
            MixedRealityInputAction menuAction = MixedRealityToolkit.InputSystem.InputSystemProfile.InputActionsProfile.InputActions.Where(m => m.Description == "Menu").FirstOrDefault();
            Assert.NotNull(menuAction.Description, "Couldn't find menu input action in input system profile.");

            // Set the interactable to respond to a 'menu' input action
            interactable.InputAction = menuAction;

            // Find an input source to associate with the input event (doesn't matter which one)
            IMixedRealityInputSource defaultInputSource = MixedRealityToolkit.InputSystem.DetectedInputSources.FirstOrDefault();
            Assert.NotNull(defaultInputSource, "At least one input source must be present for this test to work.");

            // Raise a menu down input event, then wait for transition to take place
            MixedRealityToolkit.InputSystem.RaiseOnInputDown(defaultInputSource, Handedness.Right, menuAction);
            yield return new WaitForSeconds(buttonPressAnimationDelay);

            Assert.AreNotEqual(targetStartPosition, translateTargetObject.localPosition, "Transform target object was not translated by action.");

            // Raise a menu up input event, then wait for transition to take place
            MixedRealityToolkit.InputSystem.RaiseOnInputUp(defaultInputSource, Handedness.Right, menuAction);
            yield return new WaitForSeconds(buttonReleaseAnimationDelay);

            Assert.True(wasClicked, "Interactable was not clicked.");
            Assert.AreEqual(targetStartPosition, translateTargetObject.localPosition, "Transform target object was not translated back by action.");

            Object.Destroy(interactableObject);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        /// <summary>
        /// Instantiates a push button prefab and uses simulated voice input events to press it.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator TestSimulatedVoiceInputOnPrefab()
        {
            // Load interactable prefab
            GameObject interactableObject;
            Interactable interactable;
            Transform translateTargetObject;

            InstantiateDefaultInteractablePrefab(
                new Vector3(0.0f, 0.0f, 0.5f),
                new Vector3(-90f, 0f, 0f),
                out interactableObject,
                out interactable,
                out translateTargetObject);

            // Subscribe to interactable's on click so we know the click went through
            bool wasClicked = false;
            interactable.OnClick.AddListener(() => { wasClicked = true; });

            // Set up its voice command
            interactable.VoiceCommand = "Select";

            Vector3 targetStartPosition = translateTargetObject.localPosition;

            yield return null;

            // Find an input source to associate with the input event (doesn't matter which one)
            IMixedRealityInputSource defaultInputSource = MixedRealityToolkit.InputSystem.DetectedInputSources.FirstOrDefault();
            Assert.NotNull(defaultInputSource, "At least one input source must be present for this test to work.");

            // Raise a voice select input event, then wait for transition to take place
            SpeechCommands commands = new SpeechCommands("Select", KeyCode.None, interactable.InputAction);
            MixedRealityToolkit.InputSystem.RaiseSpeechCommandRecognized(defaultInputSource, RecognitionConfidenceLevel.High, new System.TimeSpan(1000), System.DateTime.Now, commands);
            yield return new WaitForSeconds(buttonPressAnimationDelay);

            Assert.True(wasClicked, "Interactable was not clicked.");
            Assert.AreNotEqual(targetStartPosition, translateTargetObject.localPosition, "Transform target object was not translated by action.");

            Object.Destroy(interactableObject);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        /// <summary>
        /// Instantiates a push button prefab and uses simulated global input events to press it.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator TestSimulatedGlobalSelectInputOnPrefab()
        {
            // Face the camera in the opposite direction so we don't focus on button
            MixedRealityPlayspace.PerformTransformation(
            p =>
            {
                p.position = Vector3.zero;
                p.LookAt(Vector3.back);
            });

            // Load interactable prefab
            GameObject interactableObject;
            Interactable interactable;
            Transform translateTargetObject;

            // Place out of the way of any pointers
            InstantiateDefaultInteractablePrefab(
                new Vector3(10f, 0.0f, 0.5f),
                new Vector3(-90f, 0f, 0f),
                out interactableObject,
                out interactable,
                out translateTargetObject);

            // Subscribe to interactable's on click so we know the click went through
            bool wasClicked = false;
            interactable.OnClick.AddListener(() => { wasClicked = true; });

            // Set interactable to global
            interactable.IsGlobal = true;

            Vector3 targetStartPosition = translateTargetObject.localPosition;

            yield return null;

            // Find an input source to associate with the input event (doesn't matter which one)
            IMixedRealityInputSource defaultInputSource = MixedRealityToolkit.InputSystem.DetectedInputSources.FirstOrDefault();
            Assert.NotNull(defaultInputSource, "At least one input source must be present for this test to work.");

            // Add interactable as a global listener
            // This is only necessary if IsGlobal is being set manually. If it's set in the inspector, interactable will register itself in OnEnable automatically.
            MixedRealityToolkit.InputSystem.PushModalInputHandler(interactableObject);

            // Raise a select down input event, then wait for transition to take place
            MixedRealityToolkit.InputSystem.RaiseOnInputDown(defaultInputSource, Handedness.None, interactable.InputAction);
            yield return new WaitForSeconds(buttonPressAnimationDelay);

            Assert.AreNotEqual(targetStartPosition, translateTargetObject.localPosition, "Transform target object was not translated by action.");

            // Raise a select up input event, then wait for transition to take place
            MixedRealityToolkit.InputSystem.RaiseOnInputUp(defaultInputSource, Handedness.Right, interactable.InputAction);
            yield return new WaitForSeconds(buttonReleaseAnimationDelay);

            Assert.True(wasClicked, "Interactable was not clicked.");
            Assert.AreEqual(targetStartPosition, translateTargetObject.localPosition, "Transform target object was not translated back by action.");
            Assert.False(interactable.HasFocus, "Interactable had focus");

            // Remove as global listener
            MixedRealityToolkit.InputSystem.PopModalInputHandler();

            Object.Destroy(interactableObject);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        /// <summary>
        /// Assembles a push button from primitives and uses simulated hand input to press it.
        /// </summary>
        /// <returns></returns>
        //[UnityTest] This test is not currently working
        public IEnumerator TestSimulatedHandInputOnRuntimeAssembled()
        {
            // Load interactable prefab
            GameObject interactableObject;
            Interactable interactable;
            Transform translateTargetObject;

            AssembleInteractableButton(
                out interactableObject,
                out interactable,
                out translateTargetObject);

            interactableObject.transform.position = new Vector3(0.025f, 0.05f, 0.65f);
            interactableObject.transform.eulerAngles = new Vector3(-90f, 0f, 0f);

            // Subscribe to interactable's on click so we know the click went through
            bool wasClicked = false;
            interactable.OnClick.AddListener(() => { wasClicked = true; });

            Vector3 targetStartPosition = translateTargetObject.transform.localPosition;

            yield return null;

            // Add a touchable and configure for touch events
            NearInteractionTouchable touchable = interactableObject.AddComponent<NearInteractionTouchable>();
            touchable.EventsToReceive = TouchableEventType.Touch;
            touchable.Bounds = Vector2.one;
            touchable.SetLocalForward(Vector3.up);
            touchable.SetLocalUp(Vector3.forward);
            touchable.LocalCenter = Vector3.up * 2.75f;

            // Add a touch handler and link touch started / touch completed events
            TouchHandler touchHandler = interactableObject.AddComponent<TouchHandler>();
            touchHandler.OnTouchStarted.AddListener((HandTrackingInputEventData e) => interactable.SetInputDown());
            touchHandler.OnTouchCompleted.AddListener((HandTrackingInputEventData e) => interactable.SetInputUp());

            // Move the hand forward to intersect the interactable
            var inputSimulationService = PlayModeTestUtilities.GetInputSimulationService();
            int numSteps = 32;
            Vector3 p1 = new Vector3(0.0f, 0f, 0f);
            Vector3 p2 = new Vector3(0.05f, 0f, 0.51f);
            Vector3 p3 = new Vector3(0.0f, 0f, 0.0f);

            yield return PlayModeTestUtilities.ShowHand(Handedness.Right, inputSimulationService);
            yield return PlayModeTestUtilities.MoveHandFromTo(p1, p2, numSteps, ArticulatedHandPose.GestureId.Poke, Handedness.Right, inputSimulationService);
            yield return new WaitForSeconds(buttonPressAnimationDelay);

            Assert.AreNotEqual(targetStartPosition, translateTargetObject.transform.localPosition, "Translate target object was not translated by action.");

            // Move the hand back
            yield return PlayModeTestUtilities.MoveHandFromTo(p2, p3, numSteps, ArticulatedHandPose.GestureId.Poke, Handedness.Right, inputSimulationService);
            yield return PlayModeTestUtilities.HideHand(Handedness.Right, inputSimulationService);
            yield return new WaitForSeconds(buttonReleaseAnimationDelay);

            Assert.True(wasClicked, "Interactable was not clicked.");
            Assert.AreEqual(targetStartPosition, translateTargetObject.transform.localPosition, "Translate target object was not translated back by action.");

            Object.Destroy(interactableObject);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        /// <summary>
        /// Assembles a push button from primitives and uses simulated input events to press it.
        /// </summary>
        /// <returns></returns>
        //[UnityTest] This test is not currently working
        public IEnumerator TestSimulatedSelectInputOnRuntimeAssembled()
        {
            // Load interactable prefab
            GameObject interactableObject;
            Interactable interactable;
            Transform translateTargetObject;

            AssembleInteractableButton(
                out interactableObject,
                out interactable,
                out translateTargetObject);

            interactableObject.transform.position = new Vector3(0.0f, 0.0f, 0.5f);
            interactableObject.transform.eulerAngles = new Vector3(-90f, 0f, 0f);

            // Subscribe to interactable's on click so we know the click went through
            bool wasClicked = false;
            interactable.OnClick.AddListener(() => { wasClicked = true; });

            Vector3 targetStartPosition = translateTargetObject.localPosition;

            yield return null;

            // Find an input source to associate with the input event (doesn't matter which one)
            IMixedRealityInputSource defaultInputSource = MixedRealityToolkit.InputSystem.DetectedInputSources.FirstOrDefault();
            Assert.NotNull(defaultInputSource, "At least one input source must be present for this test to work.");

            // Raise an input down event, then wait for transition to take place
            MixedRealityToolkit.InputSystem.RaiseOnInputDown(defaultInputSource, Handedness.None, interactable.InputAction);
            yield return new WaitForSeconds(buttonPressAnimationDelay);

            Assert.AreNotEqual(targetStartPosition, translateTargetObject.localPosition, "Transform target object was not translated by action.");

            // Raise an input up event, then wait for transition to take place
            MixedRealityToolkit.InputSystem.RaiseOnInputUp(defaultInputSource, Handedness.None, interactable.InputAction);
            yield return new WaitForSeconds(buttonReleaseAnimationDelay);

            Assert.True(wasClicked, "Interactable was not clicked.");
            Assert.AreEqual(targetStartPosition, translateTargetObject.localPosition, "Transform target object was not translated back by action.");

            Object.Destroy(interactableObject);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        /// <summary>
        /// Generates an interactable from primitives and assigns a select action.
        /// </summary>
        /// <param name="interactableObject"></param>
        /// <param name="interactable"></param>
        /// <param name="translateTargetObject"></param>
        /// <param name="selectActionDescription"></param>
        private void AssembleInteractableButton(out GameObject interactableObject, out Interactable interactable, out Transform translateTargetObject, string selectActionDescription = "Select")
        {
            // Assemble an interactable out of a set of primitives
            // This will be the button housing
            interactableObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            interactableObject.name = "RuntimeInteractable";
            interactableObject.transform.position = new Vector3(0.05f, 0.05f, 0.625f);
            interactableObject.transform.localScale = new Vector3(0.15f, 0.025f, 0.15f);
            interactableObject.transform.eulerAngles = new Vector3(90f, 0f, 180f);

            // This will be the part that gets scaled
            GameObject childObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            childObject.GetComponent<Renderer>().material.color = Color.blue;
            childObject.transform.parent = interactableObject.transform;
            childObject.transform.localScale = new Vector3(0.9f, 1f, 0.9f);
            childObject.transform.localPosition = new Vector3(0f, 1.5f, 0f);
            childObject.transform.localRotation = Quaternion.identity;
            // Only use a collider on the main object
            GameObject.Destroy(childObject.GetComponent<Collider>());

            translateTargetObject = childObject.transform;

            // Add an interactable
            interactable = interactableObject.AddComponent<Interactable>();

            #region create interactable theme

            // Create a new theme for the profile that translates the child object
            InteractableOffsetTheme offsetTheme = new InteractableOffsetTheme();

            InteractableThemePropertySettings offsetThemeSetting = new InteractableThemePropertySettings();
            offsetThemeSetting.Theme = offsetTheme;
            offsetThemeSetting.Type = typeof(InteractableOffsetTheme);
            offsetThemeSetting.AssemblyQualifiedName = typeof(InteractableOffsetTheme).AssemblyQualifiedName;
            offsetThemeSetting.CustomSettings = new System.Collections.Generic.List<InteractableCustomSetting>();
            offsetThemeSetting.CustomHistory = new System.Collections.Generic.List<InteractableCustomSetting>();

            var easing = new Easing();
            easing.Curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
            offsetThemeSetting.Easing = easing;

            InteractableThemeProperty themeProperty = new InteractableThemeProperty();
            InteractableThemePropertyValue defaultPropertyValue = new InteractableThemePropertyValue();
            InteractableThemePropertyValue pressedPropertyValue = new InteractableThemePropertyValue();

            defaultPropertyValue.Vector3 = new Vector3(0f, 1.5f, 0f);
            pressedPropertyValue.Vector3 = new Vector3(0f, 0.3f, 0f);

            themeProperty.StartValue = defaultPropertyValue;
            themeProperty.Values = new System.Collections.Generic.List<InteractableThemePropertyValue>() { defaultPropertyValue, defaultPropertyValue, pressedPropertyValue, defaultPropertyValue };
            themeProperty.Type = InteractableThemePropertyValueTypes.Vector3;

            offsetThemeSetting.Properties = new System.Collections.Generic.List<InteractableThemeProperty>() { themeProperty };

            Theme interactableTheme = ScriptableObject.CreateInstance<Theme>();

            // Assign bit values to states
            int bitCount = 0;
            for (int i = 0; i < interactableTheme.States.StateList.Count; i++)
            {
                if (i == 0)
                {
                    bitCount += 1;
                }
                else
                {
                    bitCount += bitCount;
                }

                interactableTheme.States.StateList[i].Bit = bitCount;
            }
            interactableTheme.Settings = new System.Collections.Generic.List<InteractableThemePropertySettings>() { offsetThemeSetting };

            InteractableProfileItem profileItem = new InteractableProfileItem();
            profileItem.Themes = new System.Collections.Generic.List<Theme>() { interactableTheme };
            profileItem.Target = childObject;

            interactable.Profiles = new System.Collections.Generic.List<InteractableProfileItem>() { profileItem };

            interactable.ForceUpdateThemes();

            #endregion

            // Set the interactable to respond to the requested input action
            MixedRealityInputAction selectAction = MixedRealityToolkit.InputSystem.InputSystemProfile.InputActionsProfile.InputActions.Where(m => m.Description == selectActionDescription).FirstOrDefault();
            Assert.NotNull(selectAction.Description, "Couldn't find " + selectActionDescription + " input action in input system profile.");
            interactable.InputAction = selectAction;
        }

        /// <summary>
        /// Instantiates the default interactable buttom.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="interactableObject"></param>
        /// <param name="interactable"></param>
        /// <param name="translateTargetObject"></param>
        private void InstantiateDefaultInteractablePrefab(Vector3 position, Vector3 rotation, out GameObject interactableObject, out Interactable interactable, out Transform translateTargetObject)
        {
            // Load interactable prefab
            Object interactablePrefab = AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit.Examples/Demos/UX/Interactables/Prefabs/Model_PushButton.prefab", typeof(Object));
            interactableObject = Object.Instantiate(interactablePrefab) as GameObject;
            interactable = interactableObject.GetComponent<Interactable>();
            Assert.IsNotNull(interactable);

            // Find the target object for the interactable transformation
            translateTargetObject = interactableObject.transform.Find("Cylinder");
            Assert.IsNotNull(translateTargetObject, "Object 'Cylinder' could not be found under example object Model_PushButton.");

            // Move the object into position
            interactableObject.transform.position = position;
            interactableObject.transform.eulerAngles = rotation;
        }
    }
}
#endif