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
        // SDK/Features/UX/Prefabs/Pointers/DefaultControllerPointer.prefab
        private const string LinePointerGuid = "d5b94136462644c9873bb3347169ae7e";
        private static readonly string LinePointerPrefab = AssetDatabase.GUIDToAssetPath(LinePointerGuid);

        // SDK/Features/UX/Prefabs/Pointers/ParabolicPointer.prefab
        private const string CurvePointerGuid = "c4fd3c6fc7ff484eb434775066e7f327";
        private static readonly string CurvePointerPrefab = AssetDatabase.GUIDToAssetPath(CurvePointerGuid);

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
        /// See usage of these helpers throughout the tests within this file, e.g. <see cref="TestSpherePointerInsideGrabbable"/>.
        /// See also comments at <see cref="TestUtilities.PlayspaceToArbitraryPose"/>.
        /// </remarks>
        [UnitySetUp]
        public IEnumerator Setup()
        {
            PlayModeTestUtilities.Setup();
            TestUtilities.PlayspaceToArbitraryPose();
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            PlayModeTestUtilities.TearDown();
            yield return null;
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

            // Confirm the teleport pointer is colliding with the cube which is in front but down
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
        /// Test pointers are correctly enabled when interacting with colliders that are visible, but whose
        /// bounds are outside the camera FOV.
        /// </summary>
        [UnityTest]
        public IEnumerator TestPointerFOVLargeCollider()
        {
            var rightHand = new TestHand(Handedness.Right);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.AddComponent<NearInteractionGrabbable>();
            cube.AddComponent<NearInteractionTouchableVolume>();
            yield return rightHand.Show(TestUtilities.PositionRelativeToPlayspace(Vector3.zero));
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            var spherePointer = PointerUtils.GetPointer<SpherePointer>(Handedness.Right);
            var pokePointer = PointerUtils.GetPointer<PokePointer>(Handedness.Right);

            yield return TestPointerFOVLargeColliderHelper(spherePointer, cube, rightHand);
            yield return TestPointerFOVLargeColliderHelper(pokePointer, cube, rightHand);

            rightHand.Hide();
            GameObject.Destroy(cube);
        }

        /// <summary>
        /// Tests that pointers behave correctly when interacting with objects inside and outside
        /// its field of view
        /// </summary>
        [UnityTest]
        public IEnumerator TestPointerFOV()
        {
            var rightHand = new TestHand(Handedness.Right);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.AddComponent<NearInteractionGrabbable>();
            cube.AddComponent<NearInteractionTouchableVolume>();
            yield return rightHand.Show(TestUtilities.PositionRelativeToPlayspace(Vector3.zero));
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            var spherePointer = PointerUtils.GetPointer<SpherePointer>(Handedness.Right);
            var pokePointer = PointerUtils.GetPointer<PokePointer>(Handedness.Right);

            yield return TestPointerFOVHelper(spherePointer, cube, rightHand);
            yield return TestPointerFOVHelper(pokePointer, cube, rightHand);

            rightHand.Hide();
            GameObject.Destroy(cube);
        }

        /// <summary>
        /// Tests that sphere pointer grabs object when hand is inside a giant grabbable
        /// </summary>
        [UnityTest]
        public IEnumerator TestSpherePointerInsideGrabbable()
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            TestUtilities.PlaceRelativeToPlayspace(cube.transform);
            cube.AddComponent<NearInteractionGrabbable>();
            var rightHand = new TestHand(Handedness.Right);
            yield return rightHand.Show(TestUtilities.PositionRelativeToPlayspace(Vector3.zero));
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            var spherePointer = PointerUtils.GetPointer<SpherePointer>(Handedness.Right);
            Assert.IsNotNull(spherePointer, "Right hand does not have a sphere pointer");
            Assert.IsTrue(spherePointer.IsInteractionEnabled, "Sphere pointer should be enabled because it is near grabbable cube and visible, even if inside a giant cube.");
            GameObject.Destroy(cube);
        }

        /// <summary>
        /// Tests that right after hand being instantiated, the pointer's direction 
        /// is in the same general direction as the forward direction of the camera
        /// </summary>
        [UnityTest]
        public IEnumerator TestHandPointerDirectionToCameraDirection()
        {
            var inputSystem = PlayModeTestUtilities.GetInputSystem();

            // Raise the hand
            var rightHand = new TestHand(Handedness.Right);

            // Set initial position and show hand
            Vector3 initialPos = TestUtilities.PositionRelativeToPlayspace(new Vector3(0.01f, 0.1f, 0.5f));
            yield return rightHand.Show(initialPos);

            // Return first hand controller that is right and source type hand
            var handController = inputSystem.DetectedControllers.First(x => x.ControllerHandedness == Handedness.Right && x.InputSource.SourceType == InputSourceType.Hand);
            Assert.IsNotNull(handController);

            // Get the line pointer from the hand controller
            var linePointer = handController.InputSource.Pointers.OfType<LinePointer>().First();
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
        /// Tests that right after motion controller being instantiated, the pointer's direction 
        /// is in the same general direction as the forward direction of the camera
        /// </summary>
        [UnityTest]
        public IEnumerator TestMotionControllerPointerDirectionToCameraDirection()
        {
            var inputSystem = PlayModeTestUtilities.GetInputSystem();

            // Switch to motion controller
            var iss = PlayModeTestUtilities.GetInputSimulationService();
            var oldSimMode = iss.ControllerSimulationMode;
            iss.ControllerSimulationMode = ControllerSimulationMode.MotionController;

            // Raise the motion controller
            var rightMotionController = new TestMotionController(Handedness.Right);

            // Set initial position and show motion controller
            Vector3 initialPos = TestUtilities.PositionRelativeToPlayspace(new Vector3(0.01f, 0.1f, 0.5f));
            yield return rightMotionController.Show(initialPos);

            // Return first motion controller that is right and source type controller
            var motionController = inputSystem.DetectedControllers.First(x => x.ControllerHandedness == Handedness.Right && x.InputSource.SourceType == InputSourceType.Controller);
            Assert.IsNotNull(motionController);

            // Get the line pointer from the motion controller
            var linePointer = motionController.InputSource.Pointers.OfType<ShellHandRayPointer>().First();
            Assert.IsNotNull(linePointer);

            Vector3 linePointerOrigin = linePointer.Position;

            // Check that the line pointer origin is within half a centimeter of the initial position of the motion controller
            var distance = Vector3.Distance(initialPos, linePointerOrigin);
            Assert.LessOrEqual(distance, 0.005f);

            // Check that the angle between the line pointer ray and camera forward does not exceed 40 degrees
            float angle = Vector3.Angle(linePointer.Rays[0].Direction, CameraCache.Main.transform.forward);
            Assert.LessOrEqual(angle, 40.0f);

            // Restore the input simulation profile
            iss.ControllerSimulationMode = oldSimMode;
            yield return null;
        }


        /// <summary>
        /// Test that the same PokePointer
        /// 1) is not destroyed
        /// 2) retrieved and re-used from the pointer cache
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
            yield return rightHand.Show(TestUtilities.PositionRelativeToPlayspace(Vector3.right));

            var rightPokePointer = PlayModeTestUtilities.GetPointer<PokePointer>(Handedness.Right);
            Assert.IsNotNull(rightPokePointer);
            Assert.IsFalse(rightPokePointer.DestroyOnSourceLost);

            yield return TestButtonUtilities.TestClickPushButton(interactable.transform, targetStartPosition, translateTargetObject);

            Assert.IsTrue(wasClicked);
            Assert.IsNotNull(rightPokePointer);
            Assert.IsNull(PlayModeTestUtilities.GetPointer<PokePointer>(Handedness.Right));

            wasClicked = false;

            yield return rightHand.Show(TestUtilities.PositionRelativeToPlayspace(Vector3.right));

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
            yield return rightHand.Show(TestUtilities.PositionRelativeToPlayspace(Vector3.right));

            var rightPokePointer = PlayModeTestUtilities.GetPointer<PokePointer>(Handedness.Right);
            rightPokePointer.DestroyOnSourceLost = true;

            yield return TestButtonUtilities.TestClickPushButton(interactable.transform, targetStartPosition, translateTargetObject);

            Assert.IsTrue(wasClicked);
            Assert.IsTrue(rightPokePointer == null);
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
            yield return rightHand.Show(TestUtilities.PositionRelativeToPlayspace(Vector3.right));

            var rightPokePointer = PlayModeTestUtilities.GetPointer<PokePointer>(Handedness.Right);
            Assert.IsNotNull(rightPokePointer);
            Assert.IsFalse(rightPokePointer.DestroyOnSourceLost);

            yield return TestButtonUtilities.TestClickPushButton(interactable.transform, targetStartPosition, translateTargetObject);

            Assert.IsTrue(wasClicked);
            Assert.IsTrue(rightPokePointer == null);
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
            where T : MonoBehaviour, IMixedRealityPointer
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

        private IEnumerator TestPointerFOVHelper(IMixedRealityPointer myPointer, GameObject cube, TestHand testHand)
        {
            // Cube in front of camera
            cube.transform.SetPositionAndRotation(Vector3.forward * 1f, Quaternion.identity);
            TestUtilities.PlaceRelativeToPlayspace(cube.transform);
            cube.transform.localScale = Vector3.one * 0.1f;
            yield return testHand.MoveTo(cube.transform.position);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();
            Assert.IsTrue(myPointer.IsInteractionEnabled, $"Pointer {myPointer.PointerName} should be enabled, cube in front camera. Cube size {cube.transform.localScale} location {cube.transform.position}.");

            Vector3 playspaceUp = TestUtilities.DirectionRelativeToPlayspace(Vector3.up);

            // Cube above camera
            cube.transform.Translate(playspaceUp * 10);
            yield return testHand.MoveTo(cube.transform.position);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();
            Assert.IsFalse(myPointer.IsInteractionEnabled, $"Pointer {myPointer.PointerName} should NOT be enabled, cube behind camera. Cube size {cube.transform.localScale} location {cube.transform.position}.");

            // For sphere and poke pointers, test that setting IgnoreCollidersNotInFOV works
            if (myPointer is SpherePointer spherePointer)
            {
                spherePointer.IgnoreCollidersNotInFOV = false;
                yield return PlayModeTestUtilities.WaitForInputSystemUpdate();
                Assert.IsTrue(myPointer.IsInteractionEnabled, $"Pointer {myPointer.PointerName} should be enabled because IgnoreCollidersNotInFOV is false.");
                spherePointer.IgnoreCollidersNotInFOV = true;
            }
            else if (myPointer is PokePointer pokePointer)
            {
                pokePointer.IgnoreCollidersNotInFOV = false;
                yield return PlayModeTestUtilities.WaitForInputSystemUpdate();
                Assert.IsTrue(myPointer.IsInteractionEnabled, $"Pointer {myPointer.PointerName} should be enabled because IgnoreCollidersNotInFOV is false.");
                pokePointer.IgnoreCollidersNotInFOV = true;
            }

            // Move it back to be visible again
            cube.transform.Translate(playspaceUp * -10f);
            yield return testHand.MoveTo(cube.transform.position);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();
            Assert.IsTrue(myPointer.IsInteractionEnabled, $"Pointer {myPointer.PointerName} should be enabled because it is near object inside of FOV. Cube size {cube.transform.localScale} location {cube.transform.position}.");
        }

        private IEnumerator TestPointerFOVLargeColliderHelper(IMixedRealityPointer myPointer, GameObject cube, TestHand testHand)
        {
            Pose cubePose = new Pose(cube.transform.position, cube.transform.rotation);
            Pose worldPose = TestUtilities.PlaceRelativeToPlayspace(cubePose.position, cubePose.rotation);
            cube.transform.SetPositionAndRotation(worldPose.position, worldPose.rotation);
            cube.transform.localScale = new Vector3(3, 3, 0.05f);
            float[] yOffsets = new float[] { -1f, 0f, 1f };
            float[] xOffsets = new float[] { -1f, 0f, 1f };
            float[] zOffsets = new float[] { 1f, -1f };
            var collider = cube.GetComponent<BoxCollider>();
            foreach (var zOffset in zOffsets)
            {
                foreach (var yOffset in yOffsets)
                {
                    foreach (var xOffset in xOffsets)
                    {
                        Vector3 worldOffset = TestUtilities.DirectionRelativeToPlayspace(new Vector3(xOffset, yOffset, zOffset));
                        var cameraPos = CameraCache.Main.transform.position;
                        var pos = cameraPos + worldOffset;
                        cube.transform.position = pos;
                        yield return testHand.MoveTo(cube.transform.position);
                        yield return PlayModeTestUtilities.WaitForInputSystemUpdate();
                        bool isInFov = CameraCache.Main.IsInFOVCached(collider);
                        Assert.IsTrue(zOffset == 1f ? myPointer.IsInteractionEnabled : !myPointer.IsInteractionEnabled,
                            $"Pointer {myPointer.PointerName} in incorrect state. IsInFOV {isInFov} Cube size {cube.transform.localScale} offset {new Vector3(xOffset, yOffset, zOffset)} location {cube.transform.position}.");
                    }
                }
            }
            cube.transform.SetPositionAndRotation(cubePose.position, cubePose.rotation);
        }
        #endregion
    }
}
#endif
