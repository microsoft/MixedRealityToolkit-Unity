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

using Microsoft.MixedReality.Toolkit.Editor;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    class InteractableTests
    {
        const float buttonPressAnimationDelay = 0.25f;
        const float buttonReleaseAnimationDelay = 0.25f;
        const string defaultInteractablePrefabAssetPath = "Assets/MixedRealityToolkit.Examples/Demos/UX/Interactables/Prefabs/Model_PushButton.prefab";
        const string radialSetPrefabAssetPath = "Assets/MixedRealityToolkit.SDK/Features/UX/Interactable/Prefabs/RadialSet.prefab";

        [SetUp]
        public void Setup()
        {
            PlayModeTestUtilities.Setup();
            TestUtilities.PlayspaceToOriginLookingForward();
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

            float pressStartTime = Time.time;
            bool wasTranslated = false;
            while (Time.time < pressStartTime + buttonPressAnimationDelay)
            {   // If the transform is moved at any point during this interval, we were successful
                yield return new WaitForFixedUpdate();
                wasTranslated |= targetStartPosition != translateTargetObject.localPosition;
            }

            // Move the hand back
            yield return PlayModeTestUtilities.MoveHandFromTo(p2, p3, numSteps, ArticulatedHandPose.GestureId.Poke, Handedness.Right, inputSimulationService);
            yield return PlayModeTestUtilities.HideHand(Handedness.Right, inputSimulationService);
            yield return new WaitForSeconds(buttonReleaseAnimationDelay);

            Assert.True(wasClicked, "Interactable was not clicked.");
            Assert.True(wasTranslated, "Transform target object was not translated by action.");
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
            IMixedRealityInputSource defaultInputSource = CoreServices.InputSystem.DetectedInputSources.FirstOrDefault();
            Assert.NotNull(defaultInputSource, "At least one input source must be present for this test to work.");

            // Add interactable as a global listener
            // This is only necessary if IsGlobal is being set manually. If it's set in the inspector, interactable will register itself in OnEnable automatically.
            CoreServices.InputSystem.PushModalInputHandler(interactableObject);

            // Raise a select down input event, then wait for transition to take place
            CoreServices.InputSystem.RaiseOnInputDown(defaultInputSource, Handedness.None, interactable.InputAction);
            // Wait for at least one frame explicitly to ensure the input goes through
            yield return new WaitForFixedUpdate();

            float pressStartTime = Time.time;
            bool wasTranslated = false;
            while (Time.time < pressStartTime + buttonPressAnimationDelay)
            {   // If the transform is moved at any point during this interval, we were successful
                yield return new WaitForFixedUpdate();
                wasTranslated |= targetStartPosition != translateTargetObject.localPosition;
            }

            // Raise a select up input event, then wait for transition to take place
            CoreServices.InputSystem.RaiseOnInputUp(defaultInputSource, Handedness.Right, interactable.InputAction);
            // Wait for at least one frame explicitly to ensure the input goes through
            yield return new WaitForFixedUpdate();
            yield return new WaitForSeconds(buttonReleaseAnimationDelay);

            Assert.True(wasClicked, "Interactable was not clicked.");
            Assert.True(wasTranslated, "Transform target object was not translated by action.");
            Assert.False(interactable.HasFocus, "Interactable had focus");

            // Remove as global listener
            CoreServices.InputSystem.PopModalInputHandler();
        }

        /// <summary>
        /// Assembles a push button from primitives and uses simulated hand input to press it.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
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

            float pressStartTime = Time.time;
            bool wasTranslated = false;
            while (Time.time < pressStartTime + buttonPressAnimationDelay)
            {   // If the transform is moved at any point during this interval, we were successful
                yield return new WaitForFixedUpdate();
                wasTranslated |= targetStartPosition != translateTargetObject.localPosition;
            }

            // Move the hand back
            yield return PlayModeTestUtilities.MoveHandFromTo(p2, p3, numSteps, ArticulatedHandPose.GestureId.Poke, Handedness.Right, inputSimulationService);
            yield return PlayModeTestUtilities.HideHand(Handedness.Right, inputSimulationService);
            yield return new WaitForSeconds(buttonReleaseAnimationDelay);

            Assert.True(wasClicked, "Interactable was not clicked.");
            Assert.True(wasTranslated, "Transform target object was not translated by action.");
        }

        /// <summary>
        /// Assembles a push button from primitives and uses simulated input events to press it.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
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
            IMixedRealityInputSource defaultInputSource = CoreServices.InputSystem.DetectedInputSources.FirstOrDefault();
            Assert.NotNull(defaultInputSource, "At least one input source must be present for this test to work.");

            // Raise an input down event, then wait for transition to take place
            CoreServices.InputSystem.RaiseOnInputDown(defaultInputSource, Handedness.None, interactable.InputAction);
            // Wait for at least one frame explicitly to ensure the input goes through
            yield return new WaitForFixedUpdate();

            float pressStartTime = Time.time;
            bool wasTranslated = false;
            while (Time.time < pressStartTime + buttonPressAnimationDelay)
            {   // If the transform is moved at any point during this interval, we were successful
                yield return new WaitForFixedUpdate();
                wasTranslated |= targetStartPosition != translateTargetObject.localPosition;
            }

            // Raise an input up event, then wait for transition to take place
            CoreServices.InputSystem.RaiseOnInputUp(defaultInputSource, Handedness.None, interactable.InputAction);
            // Wait for at least one frame explicitly to ensure the input goes through
            yield return new WaitForFixedUpdate();
            yield return new WaitForSeconds(buttonReleaseAnimationDelay);

            Assert.True(wasClicked, "Interactable was not clicked.");
            Assert.True(wasTranslated, "Transform target object was not translated by action.");
            Assert.AreEqual(targetStartPosition, translateTargetObject.localPosition, "Transform target object was not translated back by action.");
        }

        /// <summary>
        /// Tests that radial buttons can be selected and deselected, and that a radial button
        /// set allows just one button to be selected at a time
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator TestRadialButtons()
        {
            var radialSet = InstantiateInteractableFromPath(Vector3.forward, Vector3.zero, radialSetPrefabAssetPath);
            var firstRadialButton = radialSet.transform.Find("Radial (1)");
            var secondRadialButton = radialSet.transform.Find("Radial (2)");
            var thirdRadialButton = radialSet.transform.Find("Radial (3)");
            var testHand = new TestHand(Handedness.Right);
            yield return testHand.Show(Vector3.zero);
            Assert.IsTrue(firstRadialButton.GetComponent<Interactable>().IsToggled);
            Assert.IsFalse(secondRadialButton.GetComponent<Interactable>().IsToggled);
            Assert.IsFalse(thirdRadialButton.GetComponent<Interactable>().IsToggled);

            yield return testHand.Show(Vector3.zero);

            var aBitBack = Vector3.forward * -0.2f;
            yield return testHand.MoveTo(firstRadialButton.position);
            yield return testHand.Move(aBitBack);

            yield return testHand.MoveTo(secondRadialButton.transform.position);
            yield return testHand.Move(aBitBack);

            Assert.IsFalse(firstRadialButton.GetComponent<Interactable>().IsToggled);
            Assert.IsTrue(secondRadialButton.GetComponent<Interactable>().IsToggled);
            Assert.IsFalse(thirdRadialButton.GetComponent<Interactable>().IsToggled);
        }

        /// <summary>
        /// Instantiates a push button prefab and uses simulated input events to press it.
        /// </summary>
        /// <returns></returns>
        /// https://github.com/microsoft/MixedRealityToolkit-Unity/issues/5153
        // [UnityTest]
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
            MixedRealityInputAction menuAction = CoreServices.InputSystem.InputSystemProfile.InputActionsProfile.InputActions.Where(m => m.Description == "Menu").FirstOrDefault();
            Assert.NotNull(menuAction.Description, "Couldn't find menu input action in input system profile.");

            // Set the interactable to respond to a 'menu' input action
            interactable.InputAction = menuAction;

            // Find an input source to associate with the input event (doesn't matter which one)
            IMixedRealityInputSource defaultInputSource = CoreServices.InputSystem.DetectedInputSources.FirstOrDefault();
            Assert.NotNull(defaultInputSource, "At least one input source must be present for this test to work.");

            // Raise a menu down input event, then wait for transition to take place
            CoreServices.InputSystem.RaiseOnInputDown(defaultInputSource, Handedness.Right, menuAction);
            // Wait for at least one frame explicitly to ensure the input goes through
            yield return new WaitForFixedUpdate();

            float pressStartTime = Time.time;
            bool wasTranslated = false;
            while (Time.time < pressStartTime + buttonPressAnimationDelay)
            {   // If the transform is moved at any point during this interval, we were successful
                yield return new WaitForFixedUpdate();
                wasTranslated |= targetStartPosition != translateTargetObject.localPosition;
            }

            // Raise a menu up input event, then wait for transition to take place
            CoreServices.InputSystem.RaiseOnInputUp(defaultInputSource, Handedness.Right, menuAction);
            // Wait for at least one frame explicitly to ensure the input goes through
            yield return new WaitForFixedUpdate();
            yield return new WaitForSeconds(buttonReleaseAnimationDelay);

            Assert.True(wasClicked, "Interactable was not clicked.");
            Assert.True(wasTranslated, "Transform target object was not translated by action.");
        }

        /// <summary>
        /// Instantiates a push button prefab and uses simulated voice input events to press it.
        /// </summary>
        /// <returns></returns>
        /// https://github.com/microsoft/MixedRealityToolkit-Unity/issues/5153
        // [UnityTest]
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
            IMixedRealityInputSource defaultInputSource = CoreServices.InputSystem.DetectedInputSources.FirstOrDefault();
            Assert.NotNull(defaultInputSource, "At least one input source must be present for this test to work.");

            // Raise a voice select input event, then wait for transition to take place
            SpeechCommands commands = new SpeechCommands("Select", KeyCode.None, interactable.InputAction);
            CoreServices.InputSystem.RaiseSpeechCommandRecognized(defaultInputSource, RecognitionConfidenceLevel.High, new System.TimeSpan(100), System.DateTime.Now, commands);
            // Wait for at least one frame explicitly to ensure the input goes through
            yield return new WaitForFixedUpdate();

            float pressStartTime = Time.time;
            bool wasTranslated = false;
            while (Time.time < pressStartTime + buttonPressAnimationDelay)
            {   // If the transform is moved at any point during this interval, we were successful
                yield return new WaitForFixedUpdate();
                wasTranslated |= targetStartPosition != translateTargetObject.localPosition;
            }

            // Wait for button press to expire
            yield return new WaitForSeconds(buttonReleaseAnimationDelay);

            Assert.True(wasClicked, "Interactable was not clicked.");
            Assert.True(wasTranslated, "Transform target object was not translated by action.");
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

#if UNITY_EDITOR
            // Find our states and themes via the asset database
            Theme cylinderTheme = ScriptableObjectExtensions.GetAllInstances<Theme>().FirstOrDefault(profile => profile.name.Equals($"CylinderTheme"));
            States defaultStates = ScriptableObjectExtensions.GetAllInstances<States>().FirstOrDefault(profile => profile.name.Equals($"DefaultInteractableStates"));

            interactable.States = defaultStates;
            InteractableProfileItem profileItem = new InteractableProfileItem();
            profileItem.Themes = new System.Collections.Generic.List<Theme>() { cylinderTheme };
            profileItem.HadDefaultTheme = true;
            profileItem.Target = translateTargetObject.gameObject;

            interactable.Profiles = new System.Collections.Generic.List<InteractableProfileItem>() { profileItem };
            interactable.ForceUpdateThemes();
#endif

            // Set the interactable to respond to the requested input action
            MixedRealityInputAction selectAction = CoreServices.InputSystem.InputSystemProfile.InputActionsProfile.InputActions.Where(m => m.Description == selectActionDescription).FirstOrDefault();
            Assert.NotNull(selectAction.Description, "Couldn't find " + selectActionDescription + " input action in input system profile.");
            interactable.InputAction = selectAction;
        }

        private GameObject InstantiateInteractableFromPath(Vector3 position, Vector3 eulerAngles, string path)
        {
            // Load interactable prefab
            Object interactablePrefab = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
            GameObject result = Object.Instantiate(interactablePrefab) as GameObject;
            Assert.IsNotNull(result);

            // Move the object into position
            result.transform.position = position;
            result.transform.eulerAngles = eulerAngles;
            return result;
        }

        /// <summary>
        /// Instantiates the default interactable button.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="interactableObject"></param>
        /// <param name="interactable"></param>
        /// <param name="translateTargetObject"></param>
        private void InstantiateDefaultInteractablePrefab(Vector3 position, Vector3 rotation, out GameObject interactableObject, out Interactable interactable, out Transform translateTargetObject)
        {
            // Load interactable prefab
            interactableObject = InstantiateInteractableFromPath(position, rotation, defaultInteractablePrefabAssetPath);
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
