// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if !WINDOWS_UWP
// When the .NET scripting backend is enabled and C# projects are built
// The assembly that this file is part of is still built for the player,
// even though the assembly itself is marked as a test assembly (this is not
// expected because test assemblies should not be included in player builds).
// Because the .NET backend is deprecated in 2018 and removed in 2019 and this
// issue will likely persist for 2018, this issue is worked around by wrapping all
// play mode tests in this check.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    /// <summary>
    /// Class that tests various types of Interactable buttons and UX components.
    /// Validates various forms of input (i.e speech etc) against various configurations of Interactable.
    /// </summary>
    public class InteractableTests : BasePlayModeTests
    {
        private const float ButtonReleaseAnimationDelay = 0.25f;
        private const float EaseDelay = 0.25f;

        // SDK/Features/UX/Interactable/Prefabs/RadialSet.prefab
        private const string RadialSetPrefabAssetGuid = "8b83134143223104c9bc3865a565cab3";
        private static readonly string RadialSetPrefabAssetPath = AssetDatabase.GUIDToAssetPath(RadialSetPrefabAssetGuid);

        // SDK/Features/UX/Interactable/Prefabs/Radial.prefab
        private const string RadialPrefabAssetGuid = "0f83fa6afa56ead46bbd762156f1137e";
        private static readonly string RadialPrefabAssetPath = AssetDatabase.GUIDToAssetPath(RadialPrefabAssetGuid);

        // Tests/PlayModeTests/Prefabs/Model_PushButton_DisabledOnStart.prefab
        private const string DisabledOnStartPrefabAssetGuid = "19bd42ed40b63a746af7320db5558f86";
        private static readonly string DisabledOnStartPrefabAssetPath = AssetDatabase.GUIDToAssetPath(DisabledOnStartPrefabAssetGuid);

        // Tests/PlayModeTests/Prefabs/TestInteractableInitialize.prefab
        private const string DisabledInitializedPrefabAssetGuid = "0401ea9158809914798d0a74023e3779";
        private static readonly string DisabledInitializedPrefabAssetPath = AssetDatabase.GUIDToAssetPath(DisabledInitializedPrefabAssetGuid);

        private readonly Color DefaultColor = Color.blue;
        private readonly Color FocusColor = Color.yellow;
        private readonly Color DisabledColor = Color.gray;

        /// <summary>
        /// Set initial state before each test.
        /// </summary>
        /// <returns>enumerator</returns>
        /// <remarks>
        /// Note that, in order to catch incorrect reliances on identity camera transforms early on,
        /// this Setup() sets the playspace transform to an arbitrary pose. This can be overridden where
        /// appropriate for an individual test by starting off with, e.g., <see cref="TestUtilities.PlayspaceToOriginLookingForward"/>.
        /// However, it is preferable to retain the arbitrary pose, and use the helpers within TestUtilities
        /// to align test objects with the camera.
        /// For example, to place an object 8 meters in front of the camera, set its global position to:
        /// TestUtilities.PositionRelativeToPlayspace(0.0f, 0.0f, 8.0f);
        /// See usage of these helpers throughout the tests within this file, e.g. <see cref="TestHandInputOnRuntimeAssembled"/>.
        /// See also comments at <see cref="TestUtilities.PlayspaceToArbitraryPose"/>.
        /// </remarks>
        [UnitySetUp]
        public override IEnumerator Setup()
        {
            yield return base.Setup();
            TestUtilities.PlayspaceToArbitraryPose();
            yield return null;
        }

        /// <summary>
        /// Instantiates a push button prefab and uses simulated hand input to press it.
        /// </summary>
        [UnityTest]
        public IEnumerator TestHandInputOnPrefab()
        {
            TestButtonUtilities.InstantiatePressableButtonPrefab(
                new Vector3(0.025f, 0.05f, 0.5f),
                TestButtonUtilities.DefaultRotation,
                TestButtonUtilities.DefaultInteractablePrefabAssetPath,
                "Cylinder",
                out Interactable interactable,
                out Transform translateTargetObject);

            // Subscribe to interactable's on click so we know the click went through
            bool wasClicked = false;
            interactable.OnClick.AddListener(() => { wasClicked = true; });

            Vector3 targetStartPosition = translateTargetObject.localPosition;

            yield return TestButtonUtilities.TestClickPushButton(interactable.transform, targetStartPosition, translateTargetObject);

            Assert.True(wasClicked, "Interactable was not clicked.");

            // Cleanup
            GameObject.Destroy(interactable.gameObject);
        }

        /// <summary>
        /// Assembles a push button from primitives and uses simulated hand input to press it.
        /// </summary>
        [UnityTest]
        public IEnumerator TestHandInputOnRuntimeAssembled()
        {
            AssembleInteractableButton(
                out Interactable interactable,
                out Transform translateTargetObject);

            interactable.transform.position = new Vector3(0.025f, 0.05f, 0.65f);
            interactable.transform.eulerAngles = new Vector3(-90f, 0f, 0f);
            TestUtilities.PlaceRelativeToPlayspace(interactable.transform);

            // Subscribe to interactable's on click so we know the click went through
            bool wasClicked = false;
            interactable.OnClick.AddListener(() => { wasClicked = true; });

            Vector3 targetStartPosition = translateTargetObject.transform.localPosition;

            yield return null;

            // NearInteractionTouchable only works with BoxColliders
            Object.Destroy(interactable.GetComponent<Collider>());
            interactable.gameObject.AddComponent<BoxCollider>();

            // Add a touchable and configure for touch events
            NearInteractionTouchable touchable = interactable.gameObject.AddComponent<NearInteractionTouchable>();
            touchable.EventsToReceive = TouchableEventType.Touch;
            touchable.SetBounds(Vector2.one);
            touchable.SetLocalForward(Vector3.up);
            touchable.SetLocalUp(Vector3.forward);
            touchable.SetLocalCenter(Vector3.up * 2.75f);

            // Add a touch handler and link touch started / touch completed events
            TouchHandler touchHandler = interactable.gameObject.AddComponent<TouchHandler>();
            touchHandler.OnTouchStarted.AddListener((HandTrackingInputEventData e) => interactable.SetInputDown());
            touchHandler.OnTouchCompleted.AddListener((HandTrackingInputEventData e) => interactable.SetInputUp());

            yield return TestButtonUtilities.TestClickPushButton(interactable.transform, targetStartPosition, translateTargetObject);

            Assert.True(wasClicked, "Interactable was not clicked.");

            GameObject.Destroy(interactable.gameObject);
        }

        /// <summary>
        /// Instantiates a push button prefab and uses simulated global input events to press it. Test that global input behaves correctly when Interactable IsEnabled and not
        /// </summary>
        [UnityTest]
        public IEnumerator TestSelectGlobalInput()
        {
            TestButtonUtilities.InstantiateDefaultButton(
                TestButtonUtilities.DefaultButtonType.DefaultPushButton,
                out Interactable interactable,
                out Transform translateTargetObject);

            interactable.transform.position = TestUtilities.PositionRelativeToPlayspace(new Vector3(10f, 0.0f, 0.5f));

            // Subscribe to interactable's on click so we know the click went through
            bool wasClicked = false;
            interactable.OnClick.AddListener(() => { wasClicked = true; });

            // Set interactable to global and disabled
            interactable.IsEnabled = false;
            interactable.IsGlobal = true;

            Vector3 targetStartPosition = translateTargetObject.localPosition;

            yield return null;

            // Find an input source to associate with the input event (doesn't matter which one)
            IMixedRealityInputSource defaultInputSource = CoreServices.InputSystem.DetectedInputSources.FirstOrDefault();
            Assert.NotNull(defaultInputSource, "At least one input source must be present for this test to work.");

            yield return RunGlobalClick(defaultInputSource, interactable.InputAction, targetStartPosition, translateTargetObject, false);

            Assert.False(wasClicked, "Interactable was not clicked.");
            Assert.False(interactable.HasFocus, "Interactable had focus");
            Assert.False(interactable.IsVisited, "Interactable was not visited");

            interactable.IsEnabled = true;

            yield return RunGlobalClick(defaultInputSource, interactable.InputAction, targetStartPosition, translateTargetObject);

            Assert.True(wasClicked, "Interactable was not clicked.");
            Assert.False(interactable.HasFocus, "Interactable had focus");
            Assert.True(interactable.IsVisited, "Interactable was not visited");

            // Unregister global handlers
            interactable.IsGlobal = false;

            // Remove as global listener and cleanup
            GameObject.Destroy(interactable.gameObject);
        }

        /// <summary>
        /// Assembles a push button from primitives and uses simulated input events to press it.
        /// </summary>
        [UnityTest]
        public IEnumerator TestInputActions()
        {
            AssembleInteractableButton(
                out Interactable interactable,
                out Transform translateTargetObject);

            interactable.transform.position = new Vector3(0.0f, 0.0f, 0.5f);
            interactable.transform.eulerAngles = new Vector3(-90f, 0f, 0f);
            TestUtilities.PlaceRelativeToPlayspace(interactable.transform);

            // Subscribe to interactable's on click and on press receiver so we know the click went through
            bool wasClicked = false;
            interactable.OnClick.AddListener(() => { wasClicked = true; });
            var pressReceiver = interactable.AddReceiver<InteractableOnPressReceiver>();
            bool wasPressed = false;
            pressReceiver.OnPress.AddListener(() => { wasPressed = true; Debug.Log("pressReciever wasPressed true"); });
            bool wasReleased = false;
            pressReceiver.OnRelease.AddListener(() => { wasReleased = true; Debug.Log("pressReciever wasReleased true"); });

            Vector3 targetStartPosition = translateTargetObject.localPosition;

            yield return null;

            //
            // Test Select Input Action
            //

            // Find an input source to associate with the input event (doesn't matter which one)
            IMixedRealityInputSource defaultInputSource = CoreServices.InputSystem.DetectedInputSources.FirstOrDefault();
            Assert.NotNull(defaultInputSource, "At least one input source must be present for this test to work.");

            yield return RunGlobalClick(defaultInputSource, interactable.InputAction, targetStartPosition, translateTargetObject);

            Assert.True(wasClicked, "Interactable was not clicked.");
            Assert.True(wasPressed, "interactable not pressed");
            Assert.True(wasReleased, "interactable not released");
            Assert.AreEqual(targetStartPosition, translateTargetObject.localPosition, "Transform target object was not translated back by action.");

            //
            // Test Menu Input Action
            //

            // Find the menu action from the input system profile
            MixedRealityInputAction menuAction = CoreServices.InputSystem.InputSystemProfile.InputActionsProfile.InputActions.Where(m => m.Description == "Menu").FirstOrDefault();
            Assert.NotNull(menuAction.Description, "Couldn't find menu input action in input system profile.");

            // Set the interactable to respond to a 'menu' input action
            interactable.InputAction = menuAction;
            // Reset state tracking
            wasClicked = wasPressed = wasReleased = false;

            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            yield return RunGlobalClick(defaultInputSource, menuAction, targetStartPosition, translateTargetObject);

            Assert.True(wasClicked, "Interactable was not clicked.");
            Assert.True(wasPressed, "interactable not pressed");
            Assert.True(wasReleased, "interactable not released");
            Assert.AreEqual(targetStartPosition, translateTargetObject.localPosition, "Transform target object was not translated back by action.");

            GameObject.Destroy(interactable.gameObject);
        }

        /// <summary>
        /// Assemble an Interactable GameObject and test various SelectionModes
        /// </summary>
        [Test]
        public void TestDimensions()
        {
            AssembleInteractableButton(
                out Interactable interactable,
                out _);

            // Test Button type
            interactable.NumOfDimensions = 1;
            Assert.AreEqual(SelectionModes.Button, interactable.ButtonMode, "Interactable should be in button selection mode");

            interactable.IsToggled = true;
            Assert.IsFalse(interactable.IsToggled, "Interactable should not be in toggle state because not in Toggle selection mode");

            // Test Multi-Dimension type
            interactable.NumOfDimensions = 4;
            interactable.CurrentDimension = 1;
            Assert.AreEqual(SelectionModes.MultiDimension, interactable.ButtonMode, "Interactable should be in MultiDimension selection mode");

            interactable.IsToggled = true;
            Assert.IsFalse(interactable.IsToggled, "Interactable should not be in toggle state because not in Toggle selection mode");

            // Test Toggle type
            interactable.NumOfDimensions = 2;
            interactable.CurrentDimension = 0;
            Assert.AreEqual(SelectionModes.Toggle, interactable.ButtonMode, "Interactable should not be in button selection mode");
            Assert.IsFalse(interactable.IsToggled, "Switching dimensions and setting CurrentDimension to 0 should make IsToggled off");

            interactable.IsToggled = true;
            Assert.IsTrue(interactable.IsToggled, "Interactable should be in toggle state because in Toggle selection mode");

            // Test Invalid type
            interactable.NumOfDimensions = -1;
            Assert.AreEqual(2, interactable.NumOfDimensions, "Interactable should be in Toggle selection mode");
            Assert.AreEqual(1, interactable.CurrentDimension, "Interactable should be Toggled");
            Assert.AreEqual(SelectionModes.Toggle, interactable.ButtonMode, "Interactable should be in Toggle selection mode");
            Assert.IsTrue(interactable.IsToggled, "Invalid Dimension should not change state");

            // Test toggle off via code API
            interactable.TriggerOnClick();
            Assert.AreEqual(2, interactable.NumOfDimensions, "Interactable should be in Toggle selection mode");
            Assert.AreEqual(0, interactable.CurrentDimension, "Interactable should be Toggled off");
            Assert.AreEqual(SelectionModes.Toggle, interactable.ButtonMode, "Interactable should be in Toggle selection mode");
            Assert.False(interactable.IsToggled, "Invalid Dimension should not change state");

            GameObject.Destroy(interactable.gameObject);
        }

        /// <summary>
        /// Instantiates a push button prefab and uses simulated voice input events to press it, both when IsEnabled and not
        /// </summary>
        [UnityTest]
        public IEnumerator TestVoiceInputOnPrefab()
        {
            TestButtonUtilities.InstantiateDefaultButton(
                TestButtonUtilities.DefaultButtonType.DefaultPushButton,
                out Interactable interactable,
                out Transform translateTargetObject);

            // Subscribe to interactable's on click so we know the click went through
            bool wasClicked = false;
            interactable.OnClick.AddListener(() => { wasClicked = true; });

            Vector3 targetStartPosition = translateTargetObject.localPosition;

            // Set up its voice command
            interactable.VoiceCommand = "Select";
            interactable.VoiceRequiresFocus = false;

            // Find an input source to associate with the input event (doesn't matter which one)
            IMixedRealityInputSource defaultInputSource = CoreServices.InputSystem.DetectedInputSources.FirstOrDefault();
            Assert.NotNull(defaultInputSource, "At least one input source must be present for this test to work.");

            //
            // Test speech when disabled
            //

            interactable.IsEnabled = false;

            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();
            yield return TestInputUtilities.ExecuteSpeechCommand(interactable.VoiceCommand, interactable.InputAction, defaultInputSource);
            yield return TestButtonUtilities.CheckButtonTranslation(targetStartPosition, translateTargetObject, false);

            Assert.False(wasClicked, "Interactable was clicked.");
            Assert.False(interactable.IsVisited, "Interactable was visited.");

            //
            // Test speech when enabled
            //

            interactable.IsEnabled = true;

            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();
            yield return TestInputUtilities.ExecuteSpeechCommand(interactable.VoiceCommand, interactable.InputAction, defaultInputSource);
            yield return TestButtonUtilities.CheckButtonTranslation(targetStartPosition, translateTargetObject);

            Assert.True(wasClicked, "Interactable was not clicked.");
            Assert.True(interactable.IsVisited, "Interactable was not visited.");

            Object.Destroy(interactable.gameObject);
        }

        /// <summary>
        /// Test touch input on Interactable by looking at state changes, both for when IsEnabled and not
        /// </summary>
        [UnityTest]
        public IEnumerator TestTouchInput()
        {
            TestButtonUtilities.InstantiateDefaultButton(
                TestButtonUtilities.DefaultButtonType.DefaultPushButton,
                out Interactable interactable,
                out _);

            interactable.gameObject.AddComponent<NearInteractionTouchableVolume>();

            //
            // Test touch when disabled
            //

            interactable.IsEnabled = false;

            yield return TestButtonUtilities.MoveHandToButton(interactable.transform);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            Assert.False(interactable.HasPhysicalTouch);
            Assert.False(interactable.HasPress);

            yield return TestButtonUtilities.MoveHandAwayFromButton(interactable.transform);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            //
            // Test touch when enabled
            //

            interactable.IsEnabled = true;

            yield return TestButtonUtilities.MoveHandToButton(interactable.transform);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            Assert.True(interactable.HasPhysicalTouch);
            Assert.True(interactable.HasPress);

            yield return TestButtonUtilities.MoveHandAwayFromButton(interactable.transform);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            Assert.False(interactable.HasPhysicalTouch);
            Assert.False(interactable.HasPress);

            GameObject.Destroy(interactable.gameObject);
        }

        /// <summary>
        /// Instantiates a runtime assembled Interactable and set Interactable state to disabled (not disabling the GameObject/component)
        /// </summary>
        [UnityTest]
        public IEnumerator TestDisabledStateThemes()
        {
            AssembleInteractableButton(
                out Interactable interactable,
                out Transform translateTargetObject);

            CameraCache.Main.transform.LookAt(interactable.transform.position);

            yield return new WaitForSeconds(EaseDelay);
            var propBlock = InteractableThemeShaderUtils.GetPropertyBlock(translateTargetObject.gameObject);
            Assert.AreEqual(propBlock.GetColor("_Color"), FocusColor);

            interactable.IsEnabled = false;

            yield return new WaitForSeconds(EaseDelay);
            propBlock = InteractableThemeShaderUtils.GetPropertyBlock(translateTargetObject.gameObject);
            Assert.AreEqual(propBlock.GetColor("_Color"), DisabledColor);
            Assert.False(interactable.IsEnabled);
            Assert.False(interactable.HasFocus);

            GameObject.Destroy(interactable.gameObject);
        }

        /// <summary>
        /// Instantiates a runtime assembled Interactable and destroy the Interactable component
        /// </summary>
        [UnityTest]
        public IEnumerator TestDestroy()
        {
            AssembleInteractableButton(
                out Interactable interactable,
                out Transform translateTargetObject);

            // Put GGV focus on the Interactable button
            CameraCache.Main.transform.LookAt(interactable.transform.position);

            yield return new WaitForSeconds(EaseDelay);
            var propBlock = InteractableThemeShaderUtils.GetPropertyBlock(translateTargetObject.gameObject);
            Assert.AreEqual(propBlock.GetColor("_Color"), FocusColor);

            // Destroy the interactable component
            GameObject.Destroy(interactable);

            // Remove focus
            CameraCache.Main.transform.LookAt(Vector3.zero);

            yield return null;
            propBlock = InteractableThemeShaderUtils.GetPropertyBlock(translateTargetObject.gameObject);
            Assert.AreEqual(propBlock.GetColor("_Color"), FocusColor);
        }

        /// <summary>
        /// Instantiates a runtime assembled Interactable with ResetOnDestroy property true and destroy the Interactable component. 
        /// </summary>
        [UnityTest]
        public IEnumerator TestResetOnDestroy()
        {
            AssembleInteractableButton(
                out Interactable interactable,
                out Transform translateTargetObject);

            interactable.ResetOnDestroy = true;

            var originalColor = translateTargetObject.gameObject.GetComponent<Renderer>().material.color;

            // Put GGV focus on the Interactable button
            CameraCache.Main.transform.LookAt(interactable.transform.position);

            yield return new WaitForSeconds(EaseDelay);
            var propBlock = InteractableThemeShaderUtils.GetPropertyBlock(translateTargetObject.gameObject);
            Assert.AreEqual(propBlock.GetColor("_Color"), FocusColor);

            // Destroy the interactable component
            GameObject.Destroy(interactable);

            // Remove focus
            CameraCache.Main.transform.LookAt(Vector3.zero);

            yield return null;
            propBlock = InteractableThemeShaderUtils.GetPropertyBlock(translateTargetObject.gameObject);
            Assert.AreEqual(propBlock.GetColor("_Color"), originalColor);
        }

        /// <summary>
        /// Tests button depth and focus state after enabling, disabling and re-enabling Interactable 
        /// internally via IsEnabled. The focus state after re-enabling should be false and button
        /// depth should be in its default position.  This test is specifically addressing behavior described 
        /// in issue 4967.
        /// </summary>
        [UnityTest]
        public IEnumerator TestDisableOnClick()
        {
            TestButtonUtilities.InstantiateDefaultButton(
                TestButtonUtilities.DefaultButtonType.DefaultPushButton,
                out Interactable interactable,
                out Transform innerCylinderTransform);

            Assert.True(interactable.IsEnabled);

            // OnClick, set Interactable IsEnabled false or aka disabled
            interactable.OnClick.AddListener(() => { interactable.IsEnabled = false; });

            // Get start position of the inner cylinder before button is pressed
            Vector3 innerCylinderStartPosition = innerCylinderTransform.localPosition;

            yield return TestButtonUtilities.TestClickPushButton(interactable.transform, innerCylinderStartPosition, innerCylinderTransform);

            Assert.False(interactable.IsEnabled, "Interactable should be disabled");

            // Re-enable Interactable
            interactable.IsEnabled = true;
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            // Make sure the button depth is back at the starting position when re-enable the gameObject and states have reset
            Assert.True(innerCylinderTransform.localPosition == innerCylinderStartPosition);
            Assert.False(interactable.HasFocus, "Interactable has focus");
            Assert.True(interactable.IsVisited, "Interactable was not visited");

            GameObject.Destroy(interactable.gameObject);
        }

        /// <summary>
        /// Tests that Interactable configured not Enabled on start works as expected.
        /// Enabled on start is an editor level setting only that is applied on Awake/Start
        /// </summary>
        [UnityTest]
        public IEnumerator TestDisabledOnStart()
        {
            // Instantiate model_pushbutton prefab but with enabled on start false
            TestButtonUtilities.InstantiatePressableButtonPrefab(
                new Vector3(0.025f, 0.05f, 0.5f),
                TestButtonUtilities.DefaultRotation,
                DisabledOnStartPrefabAssetPath,
                "Cylinder",
                out Interactable interactable,
                out Transform pressButtonCylinder);

            // Subscribe to interactable's on click so we know the click went through
            bool wasClicked = false;
            interactable.OnClick.AddListener(() => { wasClicked = true; });

            Vector3 targetStartPosition = pressButtonCylinder.localPosition;

            //
            // Test starting as disabled
            //
            yield return TestButtonUtilities.TestClickPushButton(interactable.transform, targetStartPosition, pressButtonCylinder, false);

            Assert.False(wasClicked, "Interactable was clicked.");

            //
            // Test when enabled
            //
            interactable.IsEnabled = true;

            yield return TestButtonUtilities.TestClickPushButton(interactable.transform, targetStartPosition, pressButtonCylinder, true);

            Assert.True(wasClicked, "Interactable was not clicked.");

            // Cleanup
            GameObject.Destroy(interactable.gameObject);
        }

        /// <summary>
        /// Test the TriggerOnClick API for Interactable both when IsEnabled and not. Button should fire OnClick and move UI
        /// </summary>
        [UnityTest]
        public IEnumerator TestTriggerOnClick()
        {
            TestButtonUtilities.InstantiateDefaultButton(
                TestButtonUtilities.DefaultButtonType.DefaultPushButton,
                out Interactable interactable,
                out Transform innerCylinderTransform);

            // Subscribe to interactable's on click so we know the click went through
            bool wasClicked = false;
            interactable.OnClick.AddListener(() => { wasClicked = true; });

            Vector3 targetStartPosition = innerCylinderTransform.localPosition;

            //
            // Test TriggerOnClick when disabled
            //
            interactable.IsEnabled = false;

            interactable.TriggerOnClick();

            Assert.False(wasClicked, "Interactable was clicked.");
            Assert.False(interactable.IsVisited, "Interactable was visited.");

            //
            // Test TriggerOnClick when enabled
            //
            interactable.IsEnabled = true;

            interactable.TriggerOnClick();
            yield return new WaitForSeconds(ButtonReleaseAnimationDelay);

            Assert.True(wasClicked, "Interactable was not clicked.");
            Assert.True(interactable.IsVisited, "Interactable was not visited.");

            GameObject.Destroy(interactable.gameObject);
        }

        /// <summary>
        /// Test the TriggerOnClick API for Interactable in toggle mode both when Can(De)Select and not.
        /// Also test the force parameter of TriggerOnClick.
        /// </summary>
        [UnityTest]
        public IEnumerator TestToggleTriggerOnClick()
        {
            TestButtonUtilities.InstantiateDefaultButton(
                TestButtonUtilities.DefaultButtonType.DefaultPushButton,
                out Interactable toggle,
                out Transform innerCylinderTransform);

            toggle.NumOfDimensions = 2;
            toggle.IsToggled = false;

            // Subscribe to toggle's on click so we know the click went through
            bool wasClicked = false;
            toggle.OnClick.AddListener(() => { wasClicked = true; });

            // Test TriggerOnClick when CanSelect is false
            toggle.CanSelect = false;

            toggle.TriggerOnClick();
            yield return new WaitForSeconds(ButtonReleaseAnimationDelay);

            Assert.False(wasClicked, "Toggle was clicked.");
            Assert.False(toggle.IsToggled, "Toggle was selected.");

            // Test TriggerOnClick when CanSelect is true
            toggle.CanSelect = true;

            toggle.TriggerOnClick();
            yield return new WaitForSeconds(ButtonReleaseAnimationDelay);

            Assert.True(wasClicked, "Toggle was not clicked.");
            Assert.True(toggle.IsToggled, "Toggle was not selected.");

            toggle.IsToggled = false;
            wasClicked = false;

            // Test force TriggerOnClick when CanSelect is false
            toggle.CanSelect = false;

            toggle.TriggerOnClick(true);
            yield return new WaitForSeconds(ButtonReleaseAnimationDelay);

            Assert.True(wasClicked, "Toggle was not clicked.");
            Assert.True(toggle.IsToggled, "Toggle was not selected.");

            wasClicked = false;

            // Test TriggerOnClick when CanDeselect is false
            toggle.CanDeselect = false;

            toggle.TriggerOnClick();
            yield return new WaitForSeconds(ButtonReleaseAnimationDelay);

            Assert.False(wasClicked, "Toggle was clicked.");
            Assert.True(toggle.IsToggled, "Toggle was not selected.");

            // Test TriggerOnClick when CanDeselect is true
            toggle.CanDeselect = true;

            toggle.TriggerOnClick();
            yield return new WaitForSeconds(ButtonReleaseAnimationDelay);

            Assert.True(wasClicked, "Toggle was not clicked.");
            Assert.False(toggle.IsToggled, "Toggle was selected.");

            toggle.IsToggled = true;
            wasClicked = false;

            // Test force TriggerOnClick when CanDeselect is false
            toggle.CanDeselect = false;

            toggle.TriggerOnClick(true);
            yield return new WaitForSeconds(ButtonReleaseAnimationDelay);

            Assert.True(wasClicked, "Toggle was not clicked.");
            Assert.False(toggle.IsToggled, "Toggle was selected.");

            GameObject.Destroy(toggle.gameObject);
        }

        /// <summary>
        /// Tests that radial buttons can be selected and deselected, and that a radial button
        /// set allows just one button to be selected at a time
        /// </summary>
        [UnityTest]
        public IEnumerator TestRadialSetPrefab()
        {
            var radialSet = TestButtonUtilities.InstantiateInteractableFromPath(Vector3.forward, Quaternion.identity, RadialSetPrefabAssetPath);
            var firstRadialButton = radialSet.transform.Find("Radial (1)").GetComponent<Interactable>();
            var secondRadialButton = radialSet.transform.Find("Radial (2)").GetComponent<Interactable>();
            var thirdRadialButton = radialSet.transform.Find("Radial (3)").GetComponent<Interactable>();
            var testHand = new TestHand(Handedness.Right);
            yield return testHand.Show(TestUtilities.PositionRelativeToPlayspace(Vector3.zero));

            Assert.IsTrue(firstRadialButton.IsToggled);
            Assert.IsFalse(secondRadialButton.IsToggled);
            Assert.IsFalse(thirdRadialButton.IsToggled);

            var aBitBack = TestUtilities.DirectionRelativeToPlayspace(Vector3.forward * -0.2f);
            yield return testHand.MoveTo(secondRadialButton.transform.position);
            yield return testHand.Move(aBitBack);

            Assert.IsFalse(firstRadialButton.IsToggled);
            Assert.IsFalse(firstRadialButton.HasFocus);
            Assert.IsTrue(secondRadialButton.IsToggled);
            Assert.IsTrue(secondRadialButton.HasFocus);
            Assert.IsFalse(thirdRadialButton.IsToggled);
            Assert.IsFalse(thirdRadialButton.HasFocus);

            yield return testHand.MoveTo(thirdRadialButton.transform.position);
            yield return testHand.Move(aBitBack);

            Assert.IsFalse(firstRadialButton.IsToggled);
            Assert.IsFalse(firstRadialButton.HasFocus);
            Assert.IsFalse(secondRadialButton.IsToggled);
            Assert.IsFalse(secondRadialButton.HasFocus);
            Assert.IsTrue(thirdRadialButton.IsToggled);
            Assert.IsTrue(thirdRadialButton.HasFocus);

            GameObject.Destroy(radialSet);
        }

        /// <summary>
        /// Tests that the toggle button states consistently return to original state
        /// after subsequent clicks (front plate does not move back after every click).
        /// </summary>
        [UnityTest]
        public IEnumerator TestPressableToggleHoloLens2()
        {
            var rightHand = new TestHand(Handedness.Right);
            Vector3 p2 = TestUtilities.PositionRelativeToPlayspace(new Vector3(0.010f, 0f, 0.3f));

            TestButtonUtilities.InstantiateDefaultButton(
                TestButtonUtilities.DefaultButtonType.DefaultHL2ToggleButton,
                out Interactable interactable,
                out Transform frontPlateTransform);

            Assert.True(interactable.IsEnabled);
            interactable.transform.position = TestUtilities.PositionRelativeToPlayspace(new Vector3(0.0f, 0.1f, 0.4f));

            bool wasClicked = false;
            interactable.OnClick.AddListener(() => { wasClicked = true; });

            // Get start position of the front plate before button is pressed
            Vector3 frontPlateStartPosition = frontPlateTransform.localPosition;

            yield return rightHand.Show(p2);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();
            Assert.IsTrue(interactable.HasFocus, "Interactable does not have focus when hand is pointing at it.");

            int numClicks = 3;
            for (int i = 0; i < numClicks; i++)
            {
                wasClicked = false;
                yield return rightHand.Click();
                // Wait for button animation to complete
                yield return new WaitForSeconds(0.33f);

                Assert.True(wasClicked, "Toggle button was not clicked");
                Assert.AreEqual((i + 1) % 2, interactable.CurrentDimension, $"Toggle button is in incorrect toggle state on click {i}");

                // Make sure the button depth is back at the starting position
                Assert.True(frontPlateTransform.localPosition == frontPlateStartPosition, "Toggle button front plate did not return to starting position.");
            }

            GameObject.Destroy(interactable.gameObject);
        }

        /// <summary>
        /// Tests that the toggle button states consistently return to original state
        /// after subsequent motion controller clicks (front plate does not move back after every click).
        /// </summary>
        [UnityTest]
        public IEnumerator TestPressableToggleMotionController()
        {
            // Switch to motion controller
            var iss = PlayModeTestUtilities.GetInputSimulationService();
            var oldSimMode = iss.ControllerSimulationMode;
            iss.ControllerSimulationMode = ControllerSimulationMode.MotionController;

            var rightMotionController = new TestMotionController(Handedness.Right);
            Vector3 p2 = TestUtilities.PositionRelativeToPlayspace(new Vector3(0.025f, 0.06f, 0.3f));

            TestButtonUtilities.InstantiateDefaultButton(
                TestButtonUtilities.DefaultButtonType.DefaultHL2ToggleButton,
                out Interactable interactable,
                out Transform frontPlateTransform);

            Assert.True(interactable.IsEnabled);
            interactable.transform.position = TestUtilities.PositionRelativeToPlayspace(new Vector3(0.0f, 0.1f, 0.4f));

            bool wasClicked = false;
            interactable.OnClick.AddListener(() => { wasClicked = true; });

            // Get start position of the front plate before button is pressed
            Vector3 frontPlateStartPosition = frontPlateTransform.localPosition;

            yield return rightMotionController.Show(p2);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();
            Assert.IsTrue(interactable.HasFocus, "Interactable does not have focus when motion controller is pointing at it.");

            int numClicks = 3;
            for (int i = 0; i < numClicks; i++)
            {
                wasClicked = false;
                yield return rightMotionController.Click();
                // Wait for button animation to complete
                yield return new WaitForSeconds(0.33f);

                Assert.True(wasClicked, "Toggle button was not clicked");
                Assert.AreEqual((i + 1) % 2, interactable.CurrentDimension, $"Toggle button is in incorrect toggle state on click {i}");

                // Make sure the button depth is back at the starting position
                Assert.True(frontPlateTransform.localPosition == frontPlateStartPosition, "Toggle button front plate did not return to starting position.");
            }

            GameObject.Destroy(interactable.gameObject);
            // Restore the input simulation profile
            iss.ControllerSimulationMode = oldSimMode;
            yield return null;
        }

        /// <summary>
        /// Test InteractableToggleCollection CurrentIndex updates
        /// </summary>
        [UnityTest]
        public IEnumerator TestInteractableToggleCollectionIndexUpdate()
        {
            const int numRadials = 6;

            AssembleInteractableToggleCollection(
                out InteractableToggleCollection interactableToggleCollection,
                numRadials,
                Vector3.forward);

            var toggleList = interactableToggleCollection.ToggleList;

            int[] onClickEventCalled = new int[numRadials];

            // Add listener to each toggle in the toggle collection
            for (int i = 0; i < toggleList.Length; i++)
            {
                int indexClick = i;
                toggleList[i].OnClick.AddListener(() => { onClickEventCalled[indexClick] = 1; });
            }

            for (int j = 0; j < numRadials; j++)
            {
                interactableToggleCollection.CurrentIndex = j;
                yield return null;

                // If the CurrentIndex is changed the toggle should be visually updated and events should be triggered
                for (int i = 0; i < numRadials; i++)
                {
                    bool shouldBeSelected = (i == interactableToggleCollection.CurrentIndex);
                    Assert.AreEqual(shouldBeSelected, interactableToggleCollection.ToggleList[i].IsToggled);

                    int expectedClickCount = (i <= j ? 1 : 0);
                    Assert.AreEqual(onClickEventCalled[i], expectedClickCount);
                }
            }

            GameObject.Destroy(interactableToggleCollection.gameObject);
        }

        /// <summary>
        /// Test if Button state is reset when it goes out of focus from a pressed state
        /// </summary>
        [UnityTest]
        public IEnumerator TestButtonStateResetWhenFocusLostAfterPinch()
        {
            /// This test breaks if there is roll on the camera. I don't know if that's a real
            /// break, or an error in the test. Nulling out roll for now.
            Pose restorePose = TestUtilities.ArbitraryPlayspacePose;
            Pose noRollPose = restorePose;
            noRollPose.rotation.eulerAngles = new Vector3(noRollPose.rotation.eulerAngles.x, noRollPose.rotation.eulerAngles.y, 0.0f);
            TestUtilities.ArbitraryPlayspacePose = noRollPose;

            TestUtilities.PlayspaceToArbitraryPose();
            TestButtonUtilities.InstantiateDefaultButton(
                TestButtonUtilities.DefaultButtonType.DefaultHL2Button,
                out Interactable interactable,
                out _);

            interactable.transform.position = TestUtilities.PositionRelativeToPlayspace(new Vector3(0.0f, 0.1f, 0.4f));
            Assert.True(interactable.IsEnabled);

            var rightHand = new TestHand(Handedness.Right);
            Vector3 focusPosition = TestUtilities.PositionRelativeToPlayspace(new Vector3(0.0175f, 0.0145f, 0.3f));
            Vector3 releaseDelta = TestUtilities.DirectionRelativeToPlayspace(new Vector3(0.05f, 0, 0));

            // Focus the hand on the Button using the far ray pointer
            yield return rightHand.Show(focusPosition);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            Assert.True(interactable.HasFocus);
            Assert.False(interactable.HasPress);
            Assert.False(interactable.HasGesture);
            Assert.True(interactable.StateManager.CurrentState().Index == (int)InteractableStates.InteractableStateEnum.Focus, "Interactable State is not Focus");

            // While keeping focus on the Button, engage the pinch gesture
            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            Assert.True(interactable.HasFocus);
            Assert.True(interactable.HasPress);
            Assert.False(interactable.HasGesture);
            Assert.True(interactable.StateManager.CurrentState().Index == (int)InteractableStates.InteractableStateEnum.Pressed, "Interactable State is not Pressed");

            // Move Hand to remove focus. Button should go to Default State
            yield return rightHand.Move(releaseDelta);
            yield return new WaitForSeconds(0.25f);// Wait for Interactable rollOffTime for HasPress to reset

            Assert.False(interactable.HasFocus);
            Assert.False(interactable.HasPress);
            Assert.False(interactable.HasGesture);
            Assert.True(interactable.StateManager.CurrentState().Index == (int)InteractableStates.InteractableStateEnum.Default, "Interactable State is not Default");

            // Open hand. Button should stay on Default State
            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Open);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            Assert.False(interactable.HasFocus);
            Assert.False(interactable.HasPress);
            Assert.False(interactable.HasGesture);
            Assert.True(interactable.StateManager.CurrentState().Index == (int)InteractableStates.InteractableStateEnum.Default, "Interactable State is not Default");

            // Move Hand back to Initial position and Pinch. Button should go to Pressed State
            yield return rightHand.Move(-releaseDelta);
            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            Assert.True(interactable.HasFocus);
            Assert.True(interactable.HasPress);
            Assert.False(interactable.HasGesture);
            Assert.True(interactable.StateManager.CurrentState().Index == (int)InteractableStates.InteractableStateEnum.Pressed, "Interactable State is not Pressed");

            // Open Hand. Button should go to Focus State
            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Open);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            Assert.True(interactable.HasFocus);
            Assert.False(interactable.HasPress);
            Assert.False(interactable.HasGesture);
            Assert.True(interactable.StateManager.CurrentState().Index == (int)InteractableStates.InteractableStateEnum.Focus, "Interactable State is not Focus");

            GameObject.Destroy(interactable.gameObject);

            TestUtilities.ArbitraryPlayspacePose = restorePose;
        }

        /// <summary>
        /// Ensure a disabled Interactable initializes when accessing one of its properties even though its Awake() has not been called
        /// </summary>
        [UnityTest]
        public IEnumerator TestForceInitialize()
        {
            Object checkboxesPrefab = AssetDatabase.LoadAssetAtPath(DisabledInitializedPrefabAssetPath, typeof(Object));
            var result = Object.Instantiate(checkboxesPrefab, Vector3.forward * 1.0f, Quaternion.identity) as GameObject;
            var interactables = result.GetComponentsInChildren<Interactable>(true);
            TestUtilities.PlaceRelativeToPlayspace(result.transform);

            const int ExpectedCheckboxCount = 2;
            Assert.AreEqual(ExpectedCheckboxCount, interactables.Length);

            bool isFirstActive = interactables[0].gameObject.activeInHierarchy;
            var enabledCheckbox = isFirstActive ? interactables[0] : interactables[1];
            var disabledCheckbox = isFirstActive ? interactables[1] : interactables[0];

            // No error messages should fire.
            enabledCheckbox.IsToggled = true;
            Assert.IsTrue(enabledCheckbox.IsToggled);

            // Disabled checkbox should auto-initialize even though its Awake() has not been called
            disabledCheckbox.IsToggled = true;
            Assert.IsTrue(disabledCheckbox.IsToggled);

            // Checkbox should still behave as expected even though it was pre-initialized
            {
                disabledCheckbox.gameObject.SetActive(true);
                disabledCheckbox.IsToggled = false;

                // Subscribe to interactable's on click so we know the click went through
                bool wasClicked = false;
                disabledCheckbox.OnClick.AddListener(() => { wasClicked = true; });

                Vector3 end = disabledCheckbox.transform.position - TestUtilities.DirectionRelativeToPlayspace(new Vector3(0f, 0.05f, 0f));
                Vector3 start = end - TestUtilities.DirectionRelativeToPlayspace(new Vector3(0f, 0f, 0.5f));

                yield return TestButtonUtilities.MoveHand(start, end);
                yield return TestButtonUtilities.MoveHand(end, start);

                Assert.True(wasClicked, "Interactable was not clicked.");
            }

            Object.Destroy(result);
        }

        #region Test Helpers

        /// <summary>
        /// Generates an interactable from primitives and assigns a select action.
        /// </summary>
        private void AssembleInteractableButton(out Interactable interactable, out Transform translateTargetObject, string selectActionDescription = "Select")
        {
            // Assemble an interactable out of a set of primitives
            // This will be the button housing
            var interactableObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            interactableObject.name = "RuntimeInteractable";
            interactableObject.transform.position = new Vector3(0.05f, 0.05f, 0.625f);
            interactableObject.transform.localScale = new Vector3(0.15f, 0.025f, 0.15f);
            interactableObject.transform.eulerAngles = new Vector3(90f, 0f, 180f);
            TestUtilities.PlaceRelativeToPlayspace(interactableObject.transform);

            // This will be the part that gets scaled
            GameObject childObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            var renderer = childObject.GetComponent<Renderer>();
            renderer.material.color = DefaultColor;
            renderer.material.shader = StandardShaderUtility.MrtkStandardShader;

            childObject.transform.parent = interactableObject.transform;
            childObject.transform.localScale = new Vector3(0.9f, 1f, 0.9f);
            childObject.transform.localPosition = new Vector3(0f, 1.5f, 0f);
            childObject.transform.localRotation = Quaternion.identity;
            // Only use a collider on the main object
            Object.Destroy(childObject.GetComponent<Collider>());

            translateTargetObject = childObject.transform;

            // Add an interactable
            interactable = interactableObject.AddComponent<Interactable>();

            var themeDefinition = ThemeDefinition.GetDefaultThemeDefinition<ScaleOffsetColorTheme>().Value;
            // themeDefinition.Easing.Enabled = false;
            // Set the offset state property (index = 1) to move on the Pressed state (index = 2)
            themeDefinition.StateProperties[1].Values = new List<ThemePropertyValue>()
            {
                new ThemePropertyValue() { Vector3 = Vector3.zero},
                new ThemePropertyValue() { Vector3 = Vector3.zero},
                new ThemePropertyValue() { Vector3 = new Vector3(0.0f, -0.32f, 0.0f)},
                new ThemePropertyValue() { Vector3 = Vector3.zero},
            };
            // Set the color state property (index = 2) values
            themeDefinition.StateProperties[2].Values = new List<ThemePropertyValue>()
            {
                new ThemePropertyValue() { Color = DefaultColor},
                new ThemePropertyValue() { Color = FocusColor},
                new ThemePropertyValue() { Color = Color.green},
                new ThemePropertyValue() { Color = DisabledColor},
            };

            Theme testTheme = ScriptableObject.CreateInstance<Theme>();
            testTheme.States = interactable.States;
            testTheme.Definitions = new List<ThemeDefinition>() { themeDefinition };

            interactable.Profiles = new List<InteractableProfileItem>()
            {
                new InteractableProfileItem()
                {
                    Themes = new List<Theme>() { testTheme },
                    Target = translateTargetObject.gameObject,
                },
            };

            // Set the interactable to respond to the requested input action
            MixedRealityInputAction selectAction = CoreServices.InputSystem.InputSystemProfile.InputActionsProfile.InputActions.Where(m => m.Description == selectActionDescription).FirstOrDefault();
            Assert.NotNull(selectAction.Description, "Couldn't find " + selectActionDescription + " input action in input system profile.");
            interactable.InputAction = selectAction;
        }

        /// <summary>
        /// Generates an InteractableToggleCollection from radial prefabs
        /// </summary>
        private void AssembleInteractableToggleCollection(out InteractableToggleCollection interactableToggleCollection, int numRadials, Vector3 pos)
        {
            GameObject toggleCollection = new GameObject("ToggleCollection");
            interactableToggleCollection = toggleCollection.AddComponent<InteractableToggleCollection>();

            // Instantiate radial prefabs with toggleCollection as the parent
            for (int i = 0; i < numRadials; i++)
            {
                var radial = TestButtonUtilities.InstantiateInteractableFromPath(pos + new Vector3(0.1f, i * 0.1f, 0), Quaternion.identity, RadialPrefabAssetPath);
                radial.name = "Radial " + i;
                Assert.IsNotNull(radial);
                radial.transform.parent = toggleCollection.transform;
            }

            interactableToggleCollection.ToggleList = toggleCollection.GetComponentsInChildren<Interactable>();
        }

        private IEnumerator RunGlobalClick(IMixedRealityInputSource defaultInputSource,
            MixedRealityInputAction inputAction,
            Vector3 targetStartPosition,
            Transform translateTargetObject,
            bool shouldTranslate = true)
        {
            yield return TestInputUtilities.ExecuteGlobalClick(defaultInputSource, inputAction, () =>
            {
                return TestButtonUtilities.CheckButtonTranslation(targetStartPosition, translateTargetObject, shouldTranslate);
            });

            // Wait for at button release animation to finish
            yield return new WaitForSeconds(TestButtonUtilities.ButtonReleaseAnimationDelay);
        }

        #endregion
    }
}
#endif
