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
using Microsoft.MixedReality.Toolkit.Input.UnityInput;
using Microsoft.MixedReality.Toolkit.Teleport;
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
    /// <summary>
    /// Tests to verify pointer state and pointer direction
    /// </summary>
    public class PointerTests 
    {
        private const string LinePointerPrefab = "Assets/MixedRealityToolkit.SDK/Features/UX/Prefabs/Pointers/DefaultControllerPointer.prefab";
        private const string CurvePointerPrefab = "Assets/MixedRealityToolkit.SDK/Features/UX/Prefabs/Pointers/ParabolicPointer.prefab";

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

        #region Tests

        /// <summary>
        /// Tests that line pointers and curve pointer work as expected by using default prefab implementations
        /// LinePointer should be a straight line ray while curve pointers should collide along the curve via ray-marching
        /// </summary>
        [UnityTest]
        public IEnumerator TestLinePointers()
        {
            BaseEventSystem.enableDanglingHandlerDiagnostics = false;

            var linePointer = CreatePointerPrefab<LinePointer>(LinePointerPrefab, 
                out IMixedRealityInputSource lineInputSource, out IMixedRealityController lineController);

            var curvePointer = CreatePointerPrefab<TeleportPointer>(CurvePointerPrefab,
                    out IMixedRealityInputSource curveInputSource, out IMixedRealityController curveController);

            Assert.IsNotNull(linePointer);
            Assert.IsNotNull(curvePointer);

            // Simulate pushing "up" on joystick axis to activate teleport pointer lines
            CoreServices.InputSystem?.RaisePositionInputChanged(curveInputSource,
                curveController.ControllerHandedness,
                curvePointer.TeleportInputAction,
                new Vector2(0.0f, 1.0f));

            var hitObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            hitObject.transform.position = Vector3.forward * 3.0f;
            hitObject.transform.localScale = Vector3.one * 0.1f;

            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            // Confirm the line pointer is colliding with the cube which is straight in front
            Assert.IsTrue(hitObject == linePointer.Result.CurrentPointerTarget);
            Assert.IsNull(curvePointer.Result.CurrentPointerTarget);

            hitObject.transform.position = new Vector3(0.0f, -0.8f, 2.0f);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            // Confirm the teleport poitner is collding with the cube which is in front but down
            Assert.IsTrue(hitObject == curvePointer.Result.CurrentPointerTarget);
            Assert.IsNull(linePointer.Result.CurrentPointerTarget);

            // Clean up our dummy controllers and objects from the input & teleport system
            CoreServices.InputSystem.RaiseSourceLost(lineInputSource, lineController);
            CoreServices.InputSystem.RaiseSourceLost(curveInputSource, curveController);
            CoreServices.TeleportSystem.RaiseTeleportCanceled(curvePointer, null);

            GameObjectExtensions.DestroyGameObject(linePointer.gameObject);
            GameObjectExtensions.DestroyGameObject(curvePointer.gameObject);

            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();
            BaseEventSystem.enableDanglingHandlerDiagnostics = true;
        }

        /// <summary>
        /// Tests that sphere pointer grabs object when hand is insize a giant grabbable
        /// </summary>
        [UnityTest]
        public IEnumerator TestSpherePointerInsideGrabbable()
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.AddComponent<NearInteractionGrabbable>();
            var rightHand = new TestHand(Handedness.Right);
            yield return rightHand.Show(Vector3.zero);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            var spherePointer = PointerUtils.GetPointer<SpherePointer>(Handedness.Right);
            Assert.IsNotNull(spherePointer, "Right hand does not have a sphere pointer");
            Assert.IsTrue(spherePointer.IsInteractionEnabled, "Sphere pointer should be enabled because it is near grabbable cube and visible, even if inside a giant cube.");
            GameObject.Destroy(cube);
        }

        /// <summary>
        /// Tests that sphere pointer behaves correctly when hand is near grabbable
        /// </summary>
        [UnityTest]
        public IEnumerator TestSpherePointerNearGrabbable()
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.AddComponent<NearInteractionGrabbable>();
            cube.transform.position = Vector3.forward;
            cube.transform.localScale = Vector3.one * 0.1f;

            var rightHand = new TestHand(Handedness.Right);
            yield return rightHand.Show(Vector3.forward);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            var spherePointer = PointerUtils.GetPointer<SpherePointer>(Handedness.Right);
            Assert.IsNotNull(spherePointer, "Right hand does not have a sphere pointer");
            Assert.IsTrue(spherePointer.IsInteractionEnabled, "Sphere pointer should be enabled because it is near grabbable cube and visible.");
            
            // Move forward so that cube is no longer visible
            CameraCache.Main.transform.Translate(Vector3.up * 10);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();
            Assert.IsFalse(spherePointer.IsInteractionEnabled, "Sphere pointer should NOT be enabled because hand is near grabbable but the grabbable is not visible.");

            // Move camera back so that cube is visible again
            CameraCache.Main.transform.Translate(Vector3.up * -10f);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();
            Assert.IsTrue(spherePointer.IsInteractionEnabled, "Sphere pointer should be enabled because it is near grabbable cube and visible.");
            GameObject.Destroy(cube);
        }

        /// <summary>
        /// Tests that right after being instantiated, the pointer's direction 
        /// is in the same general direction as the forward direction of the camera
        /// </summary>
        [UnityTest]
        public IEnumerator TestPointerDirectionToCameraDirection()
        {
            var inputSystem = PlayModeTestUtilities.GetInputSystem();

            // Raise the hand
            var rightHand = new TestHand(Handedness.Right);

            // Set initial position and show hand
            Vector3 initialPos = new Vector3(0.01f, 0.1f, 0.5f);
            yield return rightHand.Show(initialPos);

            // Return first hand controller that is right and source type hand
            var handController = inputSystem.DetectedControllers.First(x => x.ControllerHandedness == Utilities.Handedness.Right && x.InputSource.SourceType == InputSourceType.Hand);
            Assert.IsNotNull(handController);

            // Get the line pointer from the hand controller
            var linePointer = handController.InputSource.Pointers.First(x => x is LinePointer);
            Assert.IsNotNull(linePointer);

            Vector3 linePointerOrigin = linePointer.Position;

            // Check that the line pointer origin is within half a centimeter of the initial position of the hand
            var distance = Vector3.Distance(initialPos, linePointerOrigin);
            Assert.LessOrEqual(distance, 0.005f);

            // Check that the angle between the line pointer ray and camera forward does not exceed 40 degrees
            float angle = Vector3.Angle(linePointer.Rays[0].Direction, CameraCache.Main.transform.forward);
            Assert.LessOrEqual(angle, 40.0f);
        }


        /// <summary>
        /// Test that the same PokePointer
        /// 1) is not destroyed
        /// 2) retreived and re-used from the pointer cache
        /// 3) still click buttons and provides input after re-use
        /// </summary>
        [UnityTest]
        public IEnumerator TestPointerCaching()
        {
            TestButtonUtilities.InstantiateDefaultButton(TestButtonUtilities.DefaultButtonType.DefaultPushButton,
                out Interactable interactable,
                out Transform translateTargetObject);

            Vector3 targetStartPosition = translateTargetObject.localPosition;

            // Subscribe to interactable's on click so we know the click went through
            bool wasClicked = false;
            interactable.OnClick.AddListener(() => { wasClicked = true; });

            var rightHand = new TestHand(Handedness.Right);
            yield return rightHand.Show(Vector3.right);

            var rightPokePointer = PlayModeTestUtilities.GetPointer<PokePointer>(Handedness.Right);
            Assert.IsNotNull(rightPokePointer);
            Assert.IsFalse(rightPokePointer.DestroyOnSourceLost);

            yield return TestButtonUtilities.TestClickPushButton(interactable.transform, targetStartPosition, translateTargetObject);

            Assert.IsTrue(wasClicked);
            Assert.IsNotNull(rightPokePointer);
            Assert.IsNull(PlayModeTestUtilities.GetPointer<PokePointer>(Handedness.Right));

            wasClicked = false;

            yield return rightHand.Show(Vector3.right);

            // Confirm that we are re-using the same pointer gameobject that was stored in the cache
            Assert.AreEqual(rightPokePointer, PlayModeTestUtilities.GetPointer<PokePointer>(Handedness.Right));

            yield return TestButtonUtilities.TestClickPushButton(interactable.transform, targetStartPosition, translateTargetObject);
            Assert.IsTrue(wasClicked);
            Assert.IsNotNull(rightPokePointer);
            Assert.IsNull(PlayModeTestUtilities.GetPointer<PokePointer>(Handedness.Right));
        }

        /// <summary>
        /// As GameObjects, pointers can be destroyed at any time. 
        /// Utilize BaseControllerPointer.DestroyOnSourceLost property to test pointer cache does not break with null references (aka auto-destroyed pointers).
        /// </summary>
        [UnityTest]
        public IEnumerator TestDestroyOnSourceLostPointer()
        {
            TestButtonUtilities.InstantiateDefaultButton(TestButtonUtilities.DefaultButtonType.DefaultPushButton,
                out Interactable interactable,
                out Transform translateTargetObject);

            Vector3 targetStartPosition = translateTargetObject.localPosition;

            // Subscribe to interactable's on click so we know the click went through
            bool wasClicked = false;
            interactable.OnClick.AddListener(() => { wasClicked = true; });

            var rightHand = new TestHand(Handedness.Right);
            yield return rightHand.Show(Vector3.right);

            var rightPokePointer = PlayModeTestUtilities.GetPointer<PokePointer>(Handedness.Right);
            rightPokePointer.DestroyOnSourceLost = true;

            yield return TestButtonUtilities.TestClickPushButton(interactable.transform, targetStartPosition, translateTargetObject);

            Assert.IsTrue(wasClicked);
            Assert.IsTrue(UnityObjectExtensions.IsNull(rightPokePointer));
            Assert.IsNull(PlayModeTestUtilities.GetPointer<PokePointer>(Handedness.Right));

            wasClicked = false;
            yield return TestButtonUtilities.TestClickPushButton(interactable.transform, targetStartPosition, translateTargetObject);
            Assert.IsTrue(wasClicked);
        }

        /// <summary>
        /// Test that buttons still work when pointer cache is disabled. 
        /// Pointers that do not auto-destroy themselves on source lost should be destroyed by the input device manager creating the pointers
        /// </summary>
        [UnityTest]
        public IEnumerator TestDisabledPointerCache()
        {
            TestButtonUtilities.InstantiateDefaultButton(TestButtonUtilities.DefaultButtonType.DefaultPushButton,
                out Interactable interactable,
                out Transform translateTargetObject);

            Vector3 targetStartPosition = translateTargetObject.localPosition;

            // Subscribe to interactable's on click so we know the click went through
            bool wasClicked = false;
            interactable.OnClick.AddListener(() => { wasClicked = true; });

            PlayModeTestUtilities.GetInputSimulationService().EnablePointerCache = false;

            var rightHand = new TestHand(Handedness.Right);
            yield return rightHand.Show(Vector3.right);

            var rightPokePointer = PlayModeTestUtilities.GetPointer<PokePointer>(Handedness.Right);
            Assert.IsNotNull(rightPokePointer);
            Assert.IsFalse(rightPokePointer.DestroyOnSourceLost);

            yield return TestButtonUtilities.TestClickPushButton(interactable.transform, targetStartPosition, translateTargetObject);

            Assert.IsTrue(wasClicked);
            Assert.IsTrue(UnityObjectExtensions.IsNull(rightPokePointer));
            Assert.IsNull(PlayModeTestUtilities.GetPointer<PokePointer>(Handedness.Right));

            wasClicked = false;
            yield return TestButtonUtilities.TestClickPushButton(interactable.transform, targetStartPosition, translateTargetObject);
            Assert.IsTrue(wasClicked);
        }

        #endregion

        #region Helpers

        private static T CreatePointerPrefab<T>(string prefabPath,
                                                out IMixedRealityInputSource inputSource,
                                                out IMixedRealityController controller) 
            where T : IMixedRealityPointer
        {
            var pointerPrefab = AssetDatabase.LoadAssetAtPath<Object>(prefabPath);
            var result = PrefabUtility.InstantiatePrefab(pointerPrefab) as GameObject;
            T pointer = result.GetComponent<T>();

            inputSource = CoreServices.InputSystem.RequestNewGenericInputSource(
                pointer.PointerName,
                new IMixedRealityPointer[] { pointer });

            // use MouseController as dummy wrapper controller
            controller = new MouseController(TrackingState.Tracked, Handedness.Right, inputSource);

            if (inputSource != null)
            {
                for (int i = 0; i < inputSource.Pointers.Length; i++)
                {
                    inputSource.Pointers[i].Controller = controller;
                }
            }

            CoreServices.InputSystem.RaiseSourceDetected(inputSource, controller);
            CoreServices.InputSystem?.RaiseSourceTrackingStateChanged(inputSource, controller, TrackingState.Tracked);

            return pointer;
        }

        #endregion
    }
}
#endif
