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
        /// <summary>
        /// Tests that an interactable component can be added to a GameObject
        /// at runtime.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator TestAddInteractableAtRuntime()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes(true);
            TestUtilities.InitializePlayspace();

            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            // This should not throw an exception
            var interactable = cube.AddComponent<Interactable>();

            // clean up
            GameObject.Destroy(cube);
            yield return null;
        }

        /// <summary>
        /// Instantiates a push button prefab and uses simulated hands to press it.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator TestInstantiateAndInteractWithPrefab()
        {
            // instantiate scene
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes(true);
            TestUtilities.InitializePlayspace();

            // Load interactable prefab
            Object interactablePrefab = AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit.Examples/Demos/UX/Interactables/Prefabs/Model_PushButton.prefab", typeof(Object));
            GameObject interactableObject = Object.Instantiate(interactablePrefab) as GameObject;
            Interactable interactable = interactableObject.GetComponent<Interactable>();
            Assert.IsNotNull(interactable);

            // Find the target object for the interactable transformation
            Transform translateTargetObject = interactableObject.transform.Find("Cylinder");
            Assert.IsNotNull(translateTargetObject, "Object 'Cylinder' could not be found under example object Model_PushButton.");

            // Move the interactable into its press position
            interactableObject.transform.localPosition = new Vector3(0.025f, 0.05f, 0.5f);
            interactableObject.transform.eulerAngles = new Vector3(-90f, 0f, 0f);

            // Move the camera to origin looking at +z to more easily see the button.
            MixedRealityPlayspace.PerformTransformation(
            p =>
            {
                p.position = Vector3.zero;
                p.LookAt(Vector3.forward);
            });

            Vector3 targetStartPosition = translateTargetObject.localPosition;
            
            // Move the hand forward to intersect the interactable
            var inputSimulationService = PlayModeTestUtilities.GetInputSimulationService();
            int numSteps = 32;
            Vector3 p1 = new Vector3(0.0f, 0f, 0f);
            Vector3 p2 = new Vector3(0.05f, 0f, 0.51f);
            Vector3 p3 = new Vector3(0.0f, 0f, 0.0f);

            yield return PlayModeTestUtilities.ShowHand(Handedness.Right, inputSimulationService);
            yield return PlayModeTestUtilities.MoveHandFromTo(p1, p2, numSteps, ArticulatedHandPose.GestureId.Poke, Handedness.Right, inputSimulationService);
            
            Assert.AreNotEqual(targetStartPosition, translateTargetObject.localPosition, "Transform target object was not translated by action.");

            // Move the hand back
            yield return PlayModeTestUtilities.MoveHandFromTo(p2, p3, numSteps, ArticulatedHandPose.GestureId.Poke, Handedness.Right, inputSimulationService);
            yield return PlayModeTestUtilities.HideHand(Handedness.Right, inputSimulationService);

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
        public IEnumerator TestConfigureInteractableButton()
        {
            // instantiate scene
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes(true);
            TestUtilities.InitializePlayspace();

            // Load interactable prefab
            Object interactablePrefab = AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit.Examples/Demos/UX/Interactables/Prefabs/Model_PushButton.prefab", typeof(Object));
            GameObject interactableObject = Object.Instantiate(interactablePrefab) as GameObject;
            Interactable interactable = interactableObject.GetComponent<Interactable>();
            Assert.IsNotNull(interactable);

            // Find the target object for the interactable transformation
            Transform translateTargetObject = interactableObject.transform.Find("Cylinder");
            Assert.IsNotNull(translateTargetObject, "Object 'Cylinder' could not be found under example object Model_PushButton.");

            // Move the interactable into position
            interactableObject.transform.localPosition = new Vector3(0.0f, 0.0f, 0.5f);
            interactableObject.transform.eulerAngles = new Vector3(-90f, 0f, 0f);

            // Move the camera to origin looking at +z to more easily see the button.
            MixedRealityPlayspace.PerformTransformation(
            p =>
            {
                p.position = Vector3.zero;
                p.LookAt(Vector3.forward);
            });

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
            yield return new WaitForSeconds(1f);

            Assert.AreNotEqual(targetStartPosition, translateTargetObject.localPosition, "Transform target object was not translated by action.");

            // Raise a menu up input event, then wait for transition to take place
            MixedRealityToolkit.InputSystem.RaiseOnInputUp(defaultInputSource, Handedness.Right, menuAction);
            yield return new WaitForSeconds(1f);

            Assert.AreEqual(targetStartPosition, translateTargetObject.localPosition, "Transform target object was not translated back by action.");

            Object.Destroy(interactableObject);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        /// <summary>
        /// Assembled a push button prefab from primitives and uses simulated hands to press it.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator TestAssemblePushButtonFromScratch()
        {
            // instantiate scene
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes(true);
            TestUtilities.InitializePlayspace();

            // Assemble an interactable out of a set of primitives
            // This will be the button housing
            GameObject mainObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            mainObject.transform.position = new Vector3(0.05f, 0.05f, 0.625f);
            mainObject.transform.localScale = new Vector3(0.15f, 0.025f, 0.15f);
            mainObject.transform.eulerAngles = new Vector3(90f, 0f, 180f);

            // This will be the part that gets scaled
            GameObject childObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            childObject.GetComponent<Renderer>().material.color = Color.blue;
            childObject.transform.parent = mainObject.transform;
            childObject.transform.localScale = new Vector3(0.9f, 1f, 0.9f);
            childObject.transform.localPosition = new Vector3(0f, 1.5f, 0f);
            childObject.transform.localRotation = Quaternion.identity;
            // Only use a collider on the main object
            GameObject.Destroy(childObject.GetComponent<Collider>());

            // Add an interactable
            Interactable interactable = mainObject.AddComponent<Interactable>();

            #region create interactable theme

            // Create a new theme for the profile that translates the child object
            InteractableOffsetTheme offsetTheme = new InteractableOffsetTheme();

            InteractableThemePropertySettings offsetThemeSetting = new InteractableThemePropertySettings();
            offsetThemeSetting.Theme = offsetTheme;

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
            States interactableStates = ScriptableObject.CreateInstance<States>();

            interactableTheme.States = interactableStates;
            interactableTheme.Settings = new System.Collections.Generic.List<InteractableThemePropertySettings>() { offsetThemeSetting };

            InteractableProfileItem profileItem = new InteractableProfileItem();
            profileItem.Themes = new System.Collections.Generic.List<Theme>() { interactableTheme };
            profileItem.Target = childObject;

            interactable.Profiles = new System.Collections.Generic.List<InteractableProfileItem>() { profileItem };

            #endregion

            // Set the interactable to respond to select actions
            MixedRealityInputAction selectAction = MixedRealityToolkit.InputSystem.InputSystemProfile.InputActionsProfile.InputActions.Where(m => m.Description == "Select").FirstOrDefault();
            Assert.NotNull(selectAction.Description, "Couldn't find select input action in input system profile.");
            interactable.InputAction = selectAction;

            // Add a touchable and configure for touch events
            NearInteractionTouchable touchable = mainObject.AddComponent<NearInteractionTouchable>();
            touchable.EventsToReceive = TouchableEventType.Touch;
            touchable.Bounds = Vector2.one;
            touchable.SetLocalForward(Vector3.up);
            touchable.SetLocalUp(Vector3.forward);
            touchable.LocalCenter = Vector3.up * 2.75f;

            // Add a touch handler and link touch started / touch completed events
            TouchHandler touchHandler = mainObject.AddComponent<TouchHandler>();
            touchHandler.OnTouchStarted.AddListener((HandTrackingInputEventData e) => interactable.SetInputDown());
            touchHandler.OnTouchCompleted.AddListener((HandTrackingInputEventData e) => interactable.SetInputDown());

            // Disable the child object, then re-enable to force registration
            mainObject.SetActive(false);
            yield return null;
            mainObject.SetActive(true);
            yield return null;

            // Move the camera to origin looking at +z to more easily see the button.
            MixedRealityPlayspace.PerformTransformation(
            p =>
            {
                p.position = Vector3.zero;
                p.LookAt(Vector3.forward);
            });

            // Move the hand forward to intersect the interactable
            var inputSimulationService = PlayModeTestUtilities.GetInputSimulationService();
            int numSteps = 32;
            Vector3 p1 = new Vector3(0.0f, 0f, 0f);
            Vector3 p2 = new Vector3(0.05f, 0f, 0.51f);
            Vector3 p3 = new Vector3(0.0f, 0f, 0.0f);

            Vector3 targetStartPosition = childObject.transform.localPosition;

            yield return PlayModeTestUtilities.ShowHand(Handedness.Right, inputSimulationService);
            yield return PlayModeTestUtilities.MoveHandFromTo(p1, p2, numSteps, ArticulatedHandPose.GestureId.Poke, Handedness.Right, inputSimulationService);

            yield return new WaitForSeconds(5f);

            Assert.AreNotEqual(targetStartPosition, childObject.transform.localPosition, "Translate target object was not translated by action.");

            yield return new WaitForSeconds(5f);

            // Move the hand back
            yield return PlayModeTestUtilities.MoveHandFromTo(p2, p3, numSteps, ArticulatedHandPose.GestureId.Poke, Handedness.Right, inputSimulationService);
            yield return PlayModeTestUtilities.HideHand(Handedness.Right, inputSimulationService);

            yield return new WaitForSeconds(5f);

            Assert.AreEqual(targetStartPosition, childObject.transform.localPosition, "Translate target object was not translated back by action.");

            Object.Destroy(mainObject);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        [TearDown]
        public void ShutdownMrtk()
        {
            TestUtilities.ShutdownMixedRealityToolkit();
        }
    }
}
#endif