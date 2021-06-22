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
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.UI.BoundsControlTypes;
using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using Assert = UnityEngine.Assertions.Assert;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    /// <summary>
    /// Tests for runtime behavior of bounds control
    /// </summary>
    public class BoundsControlTests : BasePlayModeTests
    {
        private Material testMaterial;
        private Material testMaterialGrabbed;

        #region Utilities

        public override IEnumerator Setup()
        {
            // create shared test materials
            var shader = StandardShaderUtility.MrtkStandardShader;
            testMaterial = new Material(shader);
            testMaterial.color = Color.yellow;

            testMaterialGrabbed = new Material(shader);
            testMaterialGrabbed.color = Color.green;

            yield return base.Setup();
        }

        private readonly Vector3 boundsControlStartCenter = Vector3.forward * 1.5f;
        private readonly Vector3 boundsControlStartScale = Vector3.one * 0.5f;

        // SDK/Features/UX/Prefabs/AppBar/AppBar.prefab
        private const string appBarPrefabGuid = "83c02591e2867124181bcd3bcb65e288";
        private static readonly string appBarPrefabLink = AssetDatabase.GUIDToAssetPath(appBarPrefabGuid);

        public struct HandleTestData
        {
            public HandleTestData(string name, string visualPath, string configName)
            {
                handleName = name;
                handleVisualPath = visualPath;
                configPropertyName = configName;
            }
            public string handleName;
            public string handleVisualPath;
            public string configPropertyName;
        };

        static HandleTestData[] handleTestData = new HandleTestData[]
        {
            new HandleTestData("corner_3", "corner_3/visuals", "ScaleHandlesConfig"),
            new HandleTestData("midpoint_2", "midpoint_2/visuals", "RotationHandlesConfig"),
            new HandleTestData("faceCenter_2", "faceCenter_2/visuals", "TranslationHandlesConfig")
        };

        public struct PerAxisHandleTestData
        {
            public PerAxisHandleTestData(string name, string configName, CardinalAxisType[] axisTypes)
            {
                handleName = name;
                configPropertyName = configName;
                handleAxisTypes = axisTypes;
            }
            public string handleName;
            public string configPropertyName;
            public CardinalAxisType[] handleAxisTypes;
        };

        static PerAxisHandleTestData[] perAxisHandleTestData = new PerAxisHandleTestData[]
        {
            new PerAxisHandleTestData("midpoint", "RotationHandlesConfig", VisualUtils.EdgeAxisType),
            new PerAxisHandleTestData("faceCenter", "TranslationHandlesConfig", VisualUtils.FaceAxisType)
        };

        /// <summary>
        /// Instantiates a bounds control at boundsControlStartCenter
        /// transform is at scale boundsControlStartScale
        /// </summary>
        private BoundsControl InstantiateSceneAndDefaultBoundsControl(GameObject target = null)
        {
            GameObject boundsControlGameObject;
            if (target != null)
            {
                boundsControlGameObject = new GameObject();
            }
            else
            {
                boundsControlGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            }
            boundsControlGameObject.transform.position = boundsControlStartCenter;
            boundsControlGameObject.transform.localScale = boundsControlStartScale;
            BoundsControl boundsControl = boundsControlGameObject.AddComponent<BoundsControl>();
            if (target != null)
            {
                target.transform.parent = boundsControlGameObject.transform;
                target.transform.localScale = Vector3.one;
                target.transform.localPosition = Vector3.zero;
                boundsControl.Target = target;
            }
            TestUtilities.PlayspaceToOriginLookingForward();
            boundsControl.Active = true;

            return boundsControl;
        }

        /// <summary>
        /// Tests if the initial transform setup of bounds control has been propagated to its collider
        /// </summary>
        /// <param name="boundsControl">Bounds control that controls the collider size</param>
        private IEnumerator VerifyInitialBoundsCorrect(BoundsControl boundsControl)
        {
            yield return null;
            yield return new WaitForFixedUpdate();
            BoxCollider boxCollider = boundsControl.Target.GetComponent<BoxCollider>();
            var bounds = boxCollider.bounds;
            TestUtilities.AssertAboutEqual(bounds.center, boundsControlStartCenter, "bounds control incorrect center at start");
            TestUtilities.AssertAboutEqual(bounds.size, boundsControlStartScale, "bounds control incorrect size at start");
            yield return null;
        }
        #endregion

        /// <summary>
        /// Verify that we can instantiate bounds control at runtime
        /// </summary>
        [UnityTest]
        public IEnumerator BoundsControlInstantiate()
        {
            BoundsControl boundsControl = InstantiateSceneAndDefaultBoundsControl();
            Assert.IsNotNull(boundsControl, "Bounds control creation failed!");
            yield return VerifyInitialBoundsCorrect(boundsControl);

            GameObject.Destroy(boundsControl.gameObject);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        /// <summary>
        /// Verify correct collider attachment for handles after bounds control instantiation
        /// </summary>
        [UnityTest]
        public IEnumerator HandleColliderInstantiation([ValueSource("handleTestData")] HandleTestData testData)
        {
            BoundsControl boundsControl = InstantiateSceneAndDefaultBoundsControl();
            Assert.IsNotNull(boundsControl, "Bounds control creation failed!");
            yield return VerifyInitialBoundsCorrect(boundsControl);
            boundsControl.BoundsControlActivation = BoundsControlActivationType.ActivateOnStart;
            yield return null;

            // get handle and their visuals and check if collider is only attached to the handle gameobject
            // but not the handle visuals
            GameObject rigRoot = boundsControl.transform.Find("rigRoot").gameObject;
            Assert.IsNotNull(rigRoot, "rigRoot couldn't be found");
            Transform handle = rigRoot.transform.Find(testData.handleName);
            Assert.IsNotNull(handle, "couldn't find handle");
            Transform handleVisual = rigRoot.transform.Find(testData.handleVisualPath);
            Assert.IsNotNull(handleVisual, "couldn't find handle visual");

            Assert.IsNotNull(handle.GetComponent<Collider>(), "Handle should have collider.");
            Assert.IsNull(handleVisual.GetComponent<Collider>(), "Visual should not have collider.");

            yield return null;
        }

        /// <summary>
        /// Test that if we update the bounds of a box collider, that the corners will move correctly
        /// </summary>
        [UnityTest]
        public IEnumerator BoundsOverrideTest()
        {
            BoundsControl boundsControl = InstantiateSceneAndDefaultBoundsControl();
            yield return VerifyInitialBoundsCorrect(boundsControl);
            boundsControl.BoundsControlActivation = BoundsControlActivationType.ActivateOnStart;
            boundsControl.HideElementsInInspector = false;
            yield return null;

            var newObject = new GameObject();
            var bc = newObject.AddComponent<BoxCollider>();
            bc.center = new Vector3(.25f, 0, 0);
            bc.size = new Vector3(0.162f, 0.1f, 1);
            boundsControl.BoundsOverride = bc;
            yield return null;

            Bounds b = GetBoundsControlRigBounds(boundsControl);

            Debug.Assert(b.center == bc.center, $"bounds center should be {bc.center} but they are {b.center}");
            Debug.Assert(b.size == bc.size, $"bounds size should be {bc.size} but they are {b.size}");

            GameObject.Destroy(boundsControl.gameObject);
            GameObject.Destroy(newObject);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        /// <summary>
        /// Test that if we toggle the bounding box's active status,
        /// that the size of the boundsOverride is consistent, even
        /// when BoxPadding is set.
        /// </summary>
        [UnityTest]
        public IEnumerator BoundsOverridePaddingReset()
        {
            BoundsControl boundsControl = InstantiateSceneAndDefaultBoundsControl();
            yield return VerifyInitialBoundsCorrect(boundsControl);
            boundsControl.BoundsControlActivation = BoundsControlActivationType.ActivateOnStart;
            boundsControl.HideElementsInInspector = false;

            // Set the bounding box to have a large padding.
            boundsControl.BoxPadding = Vector3.one;
            yield return null;

            var newObject = new GameObject();
            var bc = newObject.AddComponent<BoxCollider>();
            bc.center = new Vector3(1, 2, 3);
            var backupSize = bc.size = new Vector3(1, 2, 3);
            boundsControl.BoundsOverride = bc;
            yield return null;

            // Toggles the bounding box and verifies
            // integrity of the measurements.
            VerifyBoundingBox();

            // Change the center and size of the boundsOverride
            // in the middle of execution, to ensure
            // these changes will be correctly reflected
            // in the BoundingBox after toggling.
            bc.center = new Vector3(0.1776f, 0.42f, 0.0f);
            backupSize = bc.size = new Vector3(0.1776f, 0.42f, 1.0f);
            boundsControl.BoundsOverride = bc;
            yield return null;

            // Toggles the bounding box and verifies
            // integrity of the measurements.
            VerifyBoundingBox();

            // Private helper function to prevent code copypasta.
            IEnumerator VerifyBoundingBox()
            {
                // Toggle the bounding box active status to check that the boundsOverride
                // will persist, and will not be destructively resized 
                boundsControl.gameObject.SetActive(false);
                yield return null;
                Debug.Log($"bc.size = {bc.size}");
                boundsControl.gameObject.SetActive(true);
                yield return null;
                Debug.Log($"bc.size = {bc.size}");

                Bounds b = GetBoundsControlRigBounds(boundsControl);

                var expectedSize = backupSize + Vector3.Scale(boundsControl.BoxPadding, newObject.transform.lossyScale);
                Debug.Assert(b.center == bc.center, $"bounds center should be {bc.center} but they are {b.center}");
                Debug.Assert(b.size == expectedSize, $"bounds size should be {expectedSize} but they are {b.size}");
                Debug.Assert(bc.size == expectedSize, $"boundsOverride's size was corrupted.");
            }

            GameObject.Destroy(boundsControl.gameObject);
            GameObject.Destroy(newObject);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        /// <summary>
        /// Uses near interaction to scale the bounds control by directly grabbing corner
        /// </summary>
        [UnityTest]
        public IEnumerator FlickeringBoundsTest()
        {
            BoundsControl boundsControl = InstantiateSceneAndDefaultBoundsControl();
            boundsControl.BoundsControlActivation = BoundsControlActivationType.ActivateByProximityAndPointer;
            yield return VerifyInitialBoundsCorrect(boundsControl);
            var inputSimulationService = PlayModeTestUtilities.GetInputSimulationService();

            boundsControl.gameObject.transform.position = new Vector3(0, 0, 1.386f);
            boundsControl.gameObject.transform.rotation = Quaternion.Euler(0, 45.0f, 0);

            TestHand hand = new TestHand(Handedness.Left);
            yield return hand.Show(new Vector3(0, 0, 1));

            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            // Check for a few loops that the hand is not flickering between states
            // number of iterations is an arbitrary number to check that the box isn't flickering
            int iterations = 15;
            for (int i = 0; i < iterations; i++)
            {
                Assert.IsFalse(hand.GetPointer<SpherePointer>().IsNearObject);
                yield return null;
            }
        }

        /// <summary>
        /// Uses near interaction to scale the bounds control by directly grabbing corner - uniform scaling
        /// </summary>
        [UnityTest]
        public IEnumerator ScaleViaNearInteraction()
        {
            BoundsControl boundsControl = InstantiateSceneAndDefaultBoundsControl();
            yield return VerifyInitialBoundsCorrect(boundsControl);

            // front right corner is corner 3
            var frontRightCornerPos = boundsControl.gameObject.transform.Find("rigRoot/corner_3").position;


            Vector3 initialHandPosition = new Vector3(0, 0, 0.5f);
            var delta = new Vector3(0.1f, 0.1f, 0f);
            TestHand hand = new TestHand(Handedness.Left);
            yield return hand.Show(initialHandPosition);
            yield return hand.MoveTo(frontRightCornerPos);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            yield return hand.MoveTo(frontRightCornerPos + delta);
            yield return null;

            var endBounds = boundsControl.GetComponent<BoxCollider>().bounds;
            Vector3 expectedCenter = new Vector3(0.033f, 0.033f, 1.467f);
            Vector3 expectedSize = Vector3.one * .567f;
            TestUtilities.AssertAboutEqual(endBounds.center, expectedCenter, "endBounds incorrect center");
            TestUtilities.AssertAboutEqual(endBounds.size, expectedSize, "endBounds incorrect size");

            yield return null;
        }

        /// <summary>
        /// Uses near interaction to scale the bounds control by directly grabbing corner - precise scaling 
        /// </summary>
        [UnityTest]
        public IEnumerator ScaleNonUniform()
        {
            BoundsControl boundsControl = InstantiateSceneAndDefaultBoundsControl();
            boundsControl.ScaleHandlesConfig.ScaleBehavior = HandleScaleMode.NonUniform;
            yield return VerifyInitialBoundsCorrect(boundsControl);

            // front right corner is corner 3
            var frontRightCornerPos = boundsControl.gameObject.transform.Find("rigRoot/corner_3").position;

            Vector3 initialHandPosition = new Vector3(0, 0, 0.5f);
            var delta = new Vector3(0.1f, 0.1f, 0f);
            TestHand hand = new TestHand(Handedness.Left);
            yield return hand.Show(initialHandPosition);
            yield return hand.MoveTo(frontRightCornerPos);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            yield return hand.MoveTo(frontRightCornerPos + delta);
            yield return null;

            var endBounds = boundsControl.GetComponent<BoxCollider>().bounds;
            Vector3 expectedCenter = new Vector3(0.05f, 0.05f, 1.5f);
            Vector3 expectedSize = Vector3.one * .6f;
            expectedSize.z = 0.5f;
            TestUtilities.AssertAboutEqual(endBounds.center, expectedCenter, "endBounds incorrect center");
            TestUtilities.AssertAboutEqual(endBounds.size, expectedSize, "endBounds incorrect size");

            yield return null;
        }

        /// <summary>
        /// Test bounds control rotation via far interaction
        /// Verifies gameobject has rotation in one axis only applied and no other transform changes happen during interaction
        /// </summary>
        [UnityTest]
        public IEnumerator RotateViaFarInteraction()
        {
            BoundsControl boundsControl = InstantiateSceneAndDefaultBoundsControl();
            yield return VerifyInitialBoundsCorrect(boundsControl);

            Vector3 pointOnCube = new Vector3(-0.033f, -0.129f, 0.499f); // position where hand ray points on center of the test cube
            Vector3 rightFrontRotationHandlePoint = new Vector3(0.121f, -0.127f, 0.499f); // position of hand for far interacting with front right rotation sphere 
            Vector3 endRotation = new Vector3(-0.18f, -0.109f, 0.504f); // end position for far interaction scaling

            TestHand hand = new TestHand(Handedness.Left);
            yield return hand.Show(pointOnCube); // Initially make sure that hand ray is pointed on cube surface so we won't go behind the cube with our ray
            // grab front right rotation point
            yield return hand.MoveTo(rightFrontRotationHandlePoint);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            // move to left side of cube
            yield return hand.MoveTo(endRotation);

            // make sure rotation is as expected and no other transform values have been modified through this
            Vector3 expectedPosition = new Vector3(0f, 0f, 1.5f);
            Vector3 expectedSize = Vector3.one * 0.5f;
            float angle;
            Vector3 axis = new Vector3();
            boundsControl.transform.rotation.ToAngleAxis(out angle, out axis);
            float expectedAngle = 85f;
            float angleDiff = Mathf.Abs(expectedAngle - angle);
            Vector3 expectedAxis = new Vector3(0f, 1f, 0f);
            TestUtilities.AssertAboutEqual(axis, expectedAxis, "Rotated around wrong axis");
            Assert.IsTrue(angleDiff <= 1f, "cube didn't rotate as expected");
            TestUtilities.AssertAboutEqual(boundsControl.transform.position, expectedPosition, "cube moved while rotating");
            TestUtilities.AssertAboutEqual(boundsControl.transform.localScale, expectedSize, "cube scaled while rotating");

            GameObject.Destroy(boundsControl.gameObject);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        /// <summary>
        /// Test bounds control rotation via motion controller
        /// Verifies gameobject has rotation in one axis only applied and no other transform changes happen during interaction
        /// </summary>
        [UnityTest]
        public IEnumerator RotateViaMotionController()
        {
            // Switch to motion controller
            var iss = PlayModeTestUtilities.GetInputSimulationService();
            var oldSimMode = iss.ControllerSimulationMode;
            iss.ControllerSimulationMode = ControllerSimulationMode.MotionController;

            BoundsControl boundsControl = InstantiateSceneAndDefaultBoundsControl();
            yield return VerifyInitialBoundsCorrect(boundsControl);

            Vector3 pointOnCube = new Vector3(-0.033f, -0.129f, 0.499f); // position where ray points on the test cube
            Vector3 rightFrontRotationHandlePoint = new Vector3(0.122f, -0.201f, 0.518f); // position of motion controller for far interacting with front right rotation sphere 
            Vector3 endRotation = new Vector3(-0.18f, -0.109f, 0.504f); // end position for far interaction scaling

            TestMotionController motionController = new TestMotionController(Handedness.Left);
            yield return motionController.Show(pointOnCube); // Initially make sure that ray is pointed on cube surface so we won't go behind the cube with our ray
            // grab front right rotation point
            yield return motionController.MoveTo(rightFrontRotationHandlePoint);
            SimulatedMotionControllerButtonState buttonState = new SimulatedMotionControllerButtonState()
            {
                IsSelecting = true
            };
            yield return motionController.SetState(buttonState);
            // move to left side of cube
            yield return motionController.MoveTo(endRotation);

            // make sure rotation is as expected and no other transform values have been modified through this
            Vector3 expectedPosition = new Vector3(0f, 0f, 1.5f);
            Vector3 expectedSize = Vector3.one * 0.5f;
            float angle;
            Vector3 axis = new Vector3();
            boundsControl.transform.rotation.ToAngleAxis(out angle, out axis);
            float expectedAngle = 54f;
            float angleDiff = Mathf.Abs(expectedAngle - angle);
            Vector3 expectedAxis = new Vector3(0f, 1f, 0f);
            TestUtilities.AssertAboutEqual(axis, expectedAxis, "Rotated around wrong axis");
            Assert.IsTrue(angleDiff <= 1f, "cube didn't rotate as expected");
            TestUtilities.AssertAboutEqual(boundsControl.transform.position, expectedPosition, "cube moved while rotating");
            TestUtilities.AssertAboutEqual(boundsControl.transform.localScale, expectedSize, "cube scaled while rotating");
            GameObject.Destroy(boundsControl.gameObject);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;

            // Restore the input simulation profile
            iss.ControllerSimulationMode = oldSimMode;
            yield return null;
        }

        /// <summary>
        /// Test bounds control rotation via far interaction, while moving extremely slowly.
        /// Rotation amount should be coherent even with extremely small per-frame motion
        /// </summary>
        [UnityTest]
        public IEnumerator RotateVerySlowlyViaFarInteraction()
        {
            BoundsControl boundsControl = InstantiateSceneAndDefaultBoundsControl();
            yield return VerifyInitialBoundsCorrect(boundsControl);

            Vector3 pointOnCube = new Vector3(-0.033f, -0.129f, 0.499f); // position where hand ray points on center of the test cube
            Vector3 rightFrontRotationHandlePoint = new Vector3(0.121f, -0.127f, 0.499f); // position of hand for far interacting with front right rotation sphere 
            Vector3 endRotation = new Vector3(-0.18f, -0.109f, 0.504f); // end position for far interaction scaling

            TestHand hand = new TestHand(Handedness.Left);
            yield return hand.Show(pointOnCube); // Initially make sure that hand ray is pointed on cube surface so we won't go behind the cube with our ray
            // grab front right rotation point
            yield return hand.MoveTo(rightFrontRotationHandlePoint);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);

            // First, we make a series of very very tiny movements, as if the user
            // is making very precise adjustments to the rotation. If the rotation is
            // being calculated per-frame instead of per-manipulation-event, this should
            // induce drift/error.
            for (int i = 0; i < 50; i++)
            {
                yield return hand.MoveTo(Vector3.Lerp(rightFrontRotationHandlePoint, endRotation, (1 / 1000.0f) * i));
            }

            // Move the rest of the way very quickly.
            yield return hand.MoveTo(endRotation);

            // make sure rotation is as expected and no other transform values have been modified through this
            Vector3 expectedPosition = new Vector3(0f, 0f, 1.5f);
            Vector3 expectedSize = Vector3.one * 0.5f;
            float angle;
            Vector3 axis = new Vector3();
            boundsControl.transform.rotation.ToAngleAxis(out angle, out axis);
            float expectedAngle = 85f;
            float angleDiff = Mathf.Abs(expectedAngle - angle);
            Vector3 expectedAxis = new Vector3(0f, 1f, 0f);
            TestUtilities.AssertAboutEqual(axis, expectedAxis, "Rotated around wrong axis");
            Assert.IsTrue(angleDiff <= 1f, "cube didn't rotate as expected");
            TestUtilities.AssertAboutEqual(boundsControl.transform.position, expectedPosition, "cube moved while rotating");
            TestUtilities.AssertAboutEqual(boundsControl.transform.localScale, expectedSize, "cube scaled while rotating");

            GameObject.Destroy(boundsControl.gameObject);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        /// <summary>
        /// Test bounds control rotation via motion controller, while moving extremely slowly.
        /// Rotation amount should be coherent even with extremely small per-frame motion
        /// </summary>
        [UnityTest]
        public IEnumerator RotateVerySlowlyViaMotionController()
        {
            // Switch to motion controller
            var iss = PlayModeTestUtilities.GetInputSimulationService();
            var oldSimMode = iss.ControllerSimulationMode;
            iss.ControllerSimulationMode = ControllerSimulationMode.MotionController;

            BoundsControl boundsControl = InstantiateSceneAndDefaultBoundsControl();
            yield return VerifyInitialBoundsCorrect(boundsControl);

            Vector3 pointOnCube = new Vector3(-0.033f, -0.129f, 0.499f); // position where ray points on the test cube
            Vector3 rightFrontRotationHandlePoint = new Vector3(0.122f, -0.201f, 0.518f); // position of motion controller for far interacting with front right rotation sphere 
            Vector3 endRotation = new Vector3(-0.18f, -0.109f, 0.504f); // end position for far interaction scaling

            TestMotionController motionController = new TestMotionController(Handedness.Left);
            yield return motionController.Show(pointOnCube); // Initially make sure that ray is pointed on cube surface so we won't go behind the cube with our ray
            // grab front right rotation point
            yield return motionController.MoveTo(rightFrontRotationHandlePoint);
            SimulatedMotionControllerButtonState buttonState = new SimulatedMotionControllerButtonState()
            {
                IsSelecting = true
            };
            yield return motionController.SetState(buttonState);

            // First, we make a series of very very tiny movements, as if the user
            // is making very precise adjustments to the rotation. If the rotation is
            // being calculated per-frame instead of per-manipulation-event, this should
            // induce drift/error.
            for (int i = 0; i < 50; i++)
            {
                yield return motionController.MoveTo(Vector3.Lerp(rightFrontRotationHandlePoint, endRotation, (1 / 1000.0f) * i));
            }

            // Move the rest of the way very quickly.
            yield return motionController.MoveTo(endRotation);

            // make sure rotation is as expected and no other transform values have been modified through this
            Vector3 expectedPosition = new Vector3(0f, 0f, 1.5f);
            Vector3 expectedSize = Vector3.one * 0.5f;
            float angle;
            Vector3 axis = new Vector3();
            boundsControl.transform.rotation.ToAngleAxis(out angle, out axis);
            float expectedAngle = 54f;
            float angleDiff = Mathf.Abs(expectedAngle - angle);
            Vector3 expectedAxis = new Vector3(0f, 1f, 0f);
            TestUtilities.AssertAboutEqual(axis, expectedAxis, "Rotated around wrong axis");
            Assert.IsTrue(angleDiff <= 1f, "cube didn't rotate as expected");
            TestUtilities.AssertAboutEqual(boundsControl.transform.position, expectedPosition, "cube moved while rotating");
            TestUtilities.AssertAboutEqual(boundsControl.transform.localScale, expectedSize, "cube scaled while rotating");

            GameObject.Destroy(boundsControl.gameObject);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
            // Restore the input simulation profile
            iss.ControllerSimulationMode = oldSimMode;
            yield return null;
        }

        /// <summary>
        /// Test bounds control rotation via near interaction
        /// Verifies gameobject has rotation in one axis only applied and no other transform changes happen during interaction
        /// </summary>
        [UnityTest]
        public IEnumerator RotateViaNearInteraction()
        {
            BoundsControl boundsControl = InstantiateSceneAndDefaultBoundsControl();
            yield return VerifyInitialBoundsCorrect(boundsControl);

            Vector3 pointOnCube = new Vector3(-0.033f, -0.129f, 0.499f); // position where hand ray points on center of the test cube
            Vector3 rightFrontRotationHandlePoint = new Vector3(0.248f, 0.001f, 1.226f); // position of hand for far interacting with front right rotation sphere 
            Vector3 endRotation = new Vector3(-0.284f, -0.001f, 1.23f); // end position for far interaction scaling

            TestHand hand = new TestHand(Handedness.Left);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.OpenSteadyGrabPoint);
            yield return hand.Show(pointOnCube);
            // grab front right rotation point
            yield return hand.MoveTo(rightFrontRotationHandlePoint);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            // move to left side of cube
            yield return hand.MoveTo(endRotation);

            // make sure rotation is as expected and no other transform values have been modified through this
            Vector3 expectedPosition = new Vector3(0f, 0f, 1.5f);
            Vector3 expectedSize = Vector3.one * 0.5f;
            float angle;
            Vector3 axis = new Vector3();
            boundsControl.transform.rotation.ToAngleAxis(out angle, out axis);
            float expectedAngle = 90f;
            float angleDiff = Mathf.Abs(expectedAngle - angle);
            Vector3 expectedAxis = new Vector3(0f, 1f, 0f);
            TestUtilities.AssertAboutEqual(axis, expectedAxis, "Rotated around wrong axis");
            Assert.IsTrue(angleDiff <= 1f, "cube didn't rotate as expected");
            TestUtilities.AssertAboutEqual(boundsControl.transform.position, expectedPosition, "cube moved while rotating");
            TestUtilities.AssertAboutEqual(boundsControl.transform.localScale, expectedSize, "cube scaled while rotating");

            GameObject.Destroy(boundsControl.gameObject);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        /// <summary>
        /// Test bounds control rotation constraints via near interaction.
        /// Verifies gameobject won't rotate when rotation constraint is applied.
        /// </summary>
        [UnityTest]
        public IEnumerator RotationConstraintViaNearInteraction()
        {
            BoundsControl boundsControl = InstantiateSceneAndDefaultBoundsControl();
            yield return VerifyInitialBoundsCorrect(boundsControl);

            var rotateConstraint = boundsControl.EnsureComponent<RotationAxisConstraint>();
            rotateConstraint.ConstraintOnRotation = AxisFlags.YAxis; // restrict rotation in Y axis

            Vector3 pointOnCube = new Vector3(-0.033f, -0.129f, 0.499f); // position where hand ray points on center of the test cube
            Vector3 rightFrontRotationHandlePoint = new Vector3(0.248f, 0.001f, 1.226f); // position of hand for far interacting with front right rotation sphere 
            Vector3 endRotation = new Vector3(-0.284f, -0.001f, 1.23f); // end position for far interaction scaling

            TestHand hand = new TestHand(Handedness.Left);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.OpenSteadyGrabPoint);
            yield return hand.Show(pointOnCube);
            // grab front right rotation point
            yield return hand.MoveTo(rightFrontRotationHandlePoint);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            // move to left side of cube
            yield return hand.MoveTo(endRotation);

            // make sure rotation is as expected and no other transform values have been modified through this
            Vector3 expectedPosition = new Vector3(0f, 0f, 1.5f);
            Vector3 expectedSize = Vector3.one * 0.5f;
            float angle;
            Vector3 axis = new Vector3();
            boundsControl.transform.rotation.ToAngleAxis(out angle, out axis);
            Assert.IsTrue(angle == 0f, "cube didn't constraint on rotation");
            TestUtilities.AssertAboutEqual(boundsControl.transform.position, expectedPosition, "cube shouldn't move while rotating");
            TestUtilities.AssertAboutEqual(boundsControl.transform.localScale, expectedSize, "cube shouldn't scale while rotating");

            yield return null;
        }

        /// <summary>
        /// Test bounds control rotation via near interaction, while moving extremely slowly.
        /// Rotation amount should be coherent even with extremely small per-frame motion
        /// </summary>
        [UnityTest]
        public IEnumerator RotateVerySlowlyViaNearInteraction()
        {
            BoundsControl boundsControl = InstantiateSceneAndDefaultBoundsControl();
            yield return VerifyInitialBoundsCorrect(boundsControl);

            Vector3 pointOnCube = new Vector3(-0.033f, -0.129f, 0.499f); // position where hand ray points on center of the test cube
            Vector3 rightFrontRotationHandlePoint = new Vector3(0.248f, 0.001f, 1.226f); // position of hand for far interacting with front right rotation sphere 
            Vector3 endRotation = new Vector3(-0.284f, -0.001f, 1.23f); // end position for far interaction scaling

            TestHand hand = new TestHand(Handedness.Left);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.OpenSteadyGrabPoint);
            yield return hand.Show(pointOnCube);
            // grab front right rotation point
            yield return hand.MoveTo(rightFrontRotationHandlePoint);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);

            // First, we make a series of very very tiny movements, as if the user
            // is making very precise adjustments to the rotation. If the rotation is
            // being calculated per-frame instead of per-manipulation-event, this should
            // induce drift/error.
            for (int i = 0; i < 50; i++)
            {
                yield return hand.MoveTo(Vector3.Lerp(rightFrontRotationHandlePoint, endRotation, (1 / 1000.0f) * i));
            }

            // Move the rest of the way very quickly.
            yield return hand.MoveTo(endRotation);

            // make sure rotation is as expected and no other transform values have been modified through this
            Vector3 expectedPosition = new Vector3(0f, 0f, 1.5f);
            Vector3 expectedSize = Vector3.one * 0.5f;
            float angle;
            Vector3 axis = new Vector3();
            boundsControl.transform.rotation.ToAngleAxis(out angle, out axis);
            float expectedAngle = 90f;
            float angleDiff = Mathf.Abs(expectedAngle - angle);
            Vector3 expectedAxis = new Vector3(0f, 1f, 0f);
            TestUtilities.AssertAboutEqual(axis, expectedAxis, "Rotated around wrong axis");
            Assert.IsTrue(angleDiff <= 1f, $"cube didn't rotate as expected, actual angle: {angle}");
            TestUtilities.AssertAboutEqual(boundsControl.transform.position, expectedPosition, "cube moved while rotating");
            TestUtilities.AssertAboutEqual(boundsControl.transform.localScale, expectedSize, "cube scaled while rotating");

            GameObject.Destroy(boundsControl.gameObject);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        /// <summary>
        /// Test bounds control rotation via HoloLens 1 interaction / GGV
        /// Verifies gameobject has rotation in one axis only applied and no other transform changes happen during interaction
        /// </summary>
        [UnityTest]
        public IEnumerator RotateViaHololens1Interaction()
        {
            BoundsControl control = InstantiateSceneAndDefaultBoundsControl();
            yield return VerifyInitialBoundsCorrect(control);
            var iss = PlayModeTestUtilities.GetInputSimulationService();
            var oldSimMode = iss.ControllerSimulationMode;
            iss.ControllerSimulationMode = ControllerSimulationMode.HandGestures;

            // move camera to look at rotation sphere
            CameraCache.Main.transform.LookAt(new Vector3(0.248f, 0.001f, 1.226f)); // rotation sphere front right

            var startHandPos = new Vector3(0.364f, -0.157f, 0.437f);
            var endPoint = new Vector3(0.141f, -0.163f, 0.485f);

            // perform tab with hand and drag to left 
            TestHand rightHand = new TestHand(Handedness.Right);
            yield return rightHand.Show(startHandPos);
            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            yield return rightHand.MoveTo(endPoint);

            // make sure only Y axis rotation was performed and no other transform values have changed
            Vector3 expectedPosition = new Vector3(0f, 0f, 1.5f);
            Vector3 expectedSize = Vector3.one * 0.5f;
            float angle;
            Vector3 axis = new Vector3();
            control.transform.rotation.ToAngleAxis(out angle, out axis);
            float expectedAngle = 84f;
            float angleDiff = Mathf.Abs(expectedAngle - angle);
            Vector3 expectedAxis = new Vector3(0f, 1f, 0f);
            TestUtilities.AssertAboutEqual(axis, expectedAxis, "Rotated around wrong axis");
            Assert.IsTrue(angleDiff <= 1f, "cube didn't rotate as expected");
            TestUtilities.AssertAboutEqual(control.transform.position, expectedPosition, "cube moved while rotating");
            TestUtilities.AssertAboutEqual(control.transform.localScale, expectedSize, "cube scaled while rotating");

            GameObject.Destroy(control.gameObject);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;

            iss.ControllerSimulationMode = oldSimMode;
            yield return null;
        }

        /// <summary>
        /// Tests scaling of bounds control by grabbing a corner with the far interaction hand ray
        /// </summary>
        [UnityTest]
        public IEnumerator ScaleViaFarInteraction()
        {
            BoundsControl boundsControl = InstantiateSceneAndDefaultBoundsControl();
            yield return VerifyInitialBoundsCorrect(boundsControl);

            Vector3 rightCornerInteractionPoint = new Vector3(0.184f, 0.078f, 0.79f); // position of hand for far interacting with front right corner 
            Vector3 pointOnCube = new Vector3(-0.033f, -0.129f, 0.499f); // position where hand ray points on center of the test cube
            Vector3 scalePoint = new Vector3(0.165f, 0.267f, 0.794f); // end position for far interaction scaling

            TestHand hand = new TestHand(Handedness.Left);
            yield return hand.Show(pointOnCube); // Initially make sure that hand ray is pointed on cube surface so we won't go behind the cube with our ray
            yield return hand.MoveTo(rightCornerInteractionPoint);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            yield return hand.MoveTo(scalePoint);
            var endBounds = boundsControl.GetComponent<BoxCollider>().bounds;
            Vector3 expectedCenter = new Vector3(0.0453f, 0.0453f, 1.455f);
            Vector3 expectedSize = Vector3.one * 0.59f;
            TestUtilities.AssertAboutEqual(endBounds.center, expectedCenter, "endBounds incorrect center");
            TestUtilities.AssertAboutEqual(endBounds.size, expectedSize, "endBounds incorrect size");

            GameObject.Destroy(boundsControl.gameObject);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        /// <summary>
        /// Tests scaling of bounds control by grabbing a corner with the motion controller
        /// </summary>
        [UnityTest]
        public IEnumerator ScaleViaMotionController()
        {
            // Switch to motion controller
            var iss = PlayModeTestUtilities.GetInputSimulationService();
            var oldSimMode = iss.ControllerSimulationMode;
            iss.ControllerSimulationMode = ControllerSimulationMode.MotionController;

            BoundsControl boundsControl = InstantiateSceneAndDefaultBoundsControl();
            yield return VerifyInitialBoundsCorrect(boundsControl);

            Vector3 rightCornerInteractionPoint = new Vector3(0.1673f, 0.121f, 0.7813f); // position of motion controller for interacting with front right corner 
            Vector3 pointOnCube = new Vector3(-0.033f, -0.129f, 0.499f); // position where ray points on the test cube
            Vector3 scalePoint = new Vector3(0.165f, 0.267f, 0.794f); // end position for motion controller scaling

            TestMotionController motionController = new TestMotionController(Handedness.Left);
            yield return motionController.Show(pointOnCube); // Initially make sure that ray is pointed on cube surface so we won't go behind the cube with our ray
            yield return motionController.MoveTo(rightCornerInteractionPoint);
            SimulatedMotionControllerButtonState buttonState = new SimulatedMotionControllerButtonState()
            {
                IsSelecting = true
            };
            yield return motionController.SetState(buttonState);
            yield return motionController.MoveTo(scalePoint);
            var endBounds = boundsControl.GetComponent<BoxCollider>().bounds;
            Vector3 expectedCenter = new Vector3(0.022f, 0.022f, 1.478f);
            Vector3 expectedSize = Vector3.one * 0.543f;
            TestUtilities.AssertAboutEqual(endBounds.center, expectedCenter, "endBounds incorrect center");
            TestUtilities.AssertAboutEqual(endBounds.size, expectedSize, "endBounds incorrect size");

            GameObject.Destroy(boundsControl.gameObject);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;

            // Restore the input simulation profile
            iss.ControllerSimulationMode = oldSimMode;

            yield return null;
        }

        /// <summary>
        /// This tests the minimum and maximum scaling for the bounds control.
        /// </summary>
        [UnityTest]
        public IEnumerator ScaleMinMax()
        {
            float minScale = 0.5f;
            float maxScale = 2f;

            BoundsControl boundsControl = InstantiateSceneAndDefaultBoundsControl();
            yield return VerifyInitialBoundsCorrect(boundsControl);
            var scaleHandler = boundsControl.EnsureComponent<MinMaxScaleConstraint>();
            scaleHandler.ScaleMinimum = minScale;
            scaleHandler.ScaleMaximum = maxScale;

            Vector3 initialScale = boundsControl.transform.localScale;

            const int numHandSteps = 1;

            Vector3 initialHandPosition = new Vector3(0, 0, 0.5f);
            var frontRightCornerPos = boundsControl.gameObject.transform.Find("rigRoot/corner_3").position; // front right corner is corner 3
            TestHand hand = new TestHand(Handedness.Right);

            // Hands grab object at initial position
            yield return hand.Show(initialHandPosition);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.OpenSteadyGrabPoint);
            yield return hand.MoveTo(frontRightCornerPos, numHandSteps);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);

            // No change to scale yet
            Assert.AreEqual(initialScale, boundsControl.transform.localScale);

            // Move hands beyond max scale limit
            yield return hand.MoveTo(new Vector3(scaleHandler.ScaleMaximum * 2, scaleHandler.ScaleMaximum * 2, 0) + frontRightCornerPos, numHandSteps);

            // Assert scale at max
            Assert.AreEqual(Vector3.one * scaleHandler.ScaleMaximum, boundsControl.transform.localScale);

            // Move hands beyond min scale limit
            yield return hand.MoveTo(new Vector3(-scaleHandler.ScaleMinimum * 2, -scaleHandler.ScaleMinimum * 2, 0) + frontRightCornerPos, numHandSteps);

            // Assert scale at min
            Assert.AreEqual(Vector3.one * scaleHandler.ScaleMinimum, boundsControl.transform.localScale);

            GameObject.Destroy(boundsControl.gameObject);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;

        }

        /// <summary>
        /// This tests the minimum and maximum scaling for the bounds control when target is its child.
        /// </summary>
        [UnityTest]
        public IEnumerator ScaleChildTargetMinMax()
        {
            float minScale = 0.5f;
            float maxScale = 2f;

            var target = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var boundsControl = InstantiateSceneAndDefaultBoundsControl(target);
            yield return VerifyInitialBoundsCorrect(boundsControl);
            var scaleHandler = boundsControl.EnsureComponent<MinMaxScaleConstraint>();
            scaleHandler.ScaleMinimum = minScale;
            scaleHandler.ScaleMaximum = maxScale;

            Vector3 initialScale = boundsControl.Target.transform.localScale;

            const int numHandSteps = 1;

            Vector3 initialHandPosition = new Vector3(0, 0, 0.5f);
            var frontRightCornerPos = boundsControl.Target.transform.Find("rigRoot/corner_3").position; // front right corner is corner 3
            TestHand hand = new TestHand(Handedness.Right);

            // Hands grab object at initial position
            yield return hand.Show(initialHandPosition);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.OpenSteadyGrabPoint);
            yield return hand.MoveTo(frontRightCornerPos, numHandSteps);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);

            // No change to scale yet
            Assert.AreEqual(initialScale, boundsControl.Target.transform.localScale);

            // Move hands beyond max scale limit
            yield return hand.MoveTo(new Vector3(scaleHandler.ScaleMaximum * 2, scaleHandler.ScaleMaximum * 2, 0) + frontRightCornerPos, numHandSteps);

            // Assert scale at max
            Assert.AreEqual(Vector3.one * scaleHandler.ScaleMaximum, boundsControl.Target.transform.localScale);

            // Move hands beyond min scale limit
            yield return hand.MoveTo(new Vector3(-scaleHandler.ScaleMinimum * 2, -scaleHandler.ScaleMinimum * 2, 0) + frontRightCornerPos, numHandSteps);

            // Assert scale at min
            Assert.AreEqual(Vector3.one * scaleHandler.ScaleMinimum, boundsControl.Target.transform.localScale);
        }

        /// <summary>
        /// Uses far interaction (HoloLens 1 style) to scale the bounds control
        /// </summary>
        [UnityTest]
        public IEnumerator ScaleViaHoloLens1Interaction()
        {
            BoundsControl boundsControl = InstantiateSceneAndDefaultBoundsControl();
            yield return VerifyInitialBoundsCorrect(boundsControl);
            BoxCollider boxCollider = boundsControl.GetComponent<BoxCollider>();
            var iss = PlayModeTestUtilities.GetInputSimulationService();
            var oldSimMode = iss.ControllerSimulationMode;
            iss.ControllerSimulationMode = ControllerSimulationMode.HandGestures;

            CameraCache.Main.transform.LookAt(boundsControl.gameObject.transform.Find("rigRoot/corner_3").transform);

            var startHandPos = CameraCache.Main.transform.TransformPoint(new Vector3(0.1f, 0f, 1.5f));
            TestHand rightHand = new TestHand(Handedness.Right);
            yield return rightHand.Show(startHandPos);
            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            // After pinching, center should remain the same
            var afterPinchbounds = boxCollider.bounds;
            TestUtilities.AssertAboutEqual(afterPinchbounds.center, boundsControlStartCenter, "boundsControl incorrect center after pinch");
            TestUtilities.AssertAboutEqual(afterPinchbounds.size, boundsControlStartScale, "boundsControl incorrect size after pinch");

            var delta = new Vector3(0.1f, 0.1f, 0f);
            yield return rightHand.Move(delta);

            var endBounds = boxCollider.bounds;
            Vector3 expectedCenter = new Vector3(0.028f, 0.028f, 1.47f);
            Vector3 expectedSize = Vector3.one * .555f;
            TestUtilities.AssertAboutEqual(endBounds.center, expectedCenter, "endBounds incorrect center");
            TestUtilities.AssertAboutEqual(endBounds.size, expectedSize, "endBounds incorrect size", 0.02f);

            GameObject.Destroy(boundsControl.gameObject);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;

            iss.ControllerSimulationMode = oldSimMode;
            yield return null;
        }

        /// <summary>
        /// Test bounds control translation via far interaction
        /// Verifies gameobject has translation in one axis only applied and no other transform changes happen during interaction
        /// </summary>
        [UnityTest]
        public IEnumerator TranslateViaFarInteraction()
        {
            BoundsControl boundsControl = InstantiateSceneAndDefaultBoundsControl();
            yield return VerifyInitialBoundsCorrect(boundsControl);
            boundsControl.TranslationHandlesConfig.ShowHandleForX = true;
            boundsControl.SmoothingActive = false;
            boundsControl.transform.position = new Vector3(-1.0f, 0.0f, 1.5f);

            Vector3 pointOnCube = new Vector3(-0.033f, -0.129f, 0.499f); // position where hand ray points on center of the test cube
            Vector3 transformHandlePosition = new Vector3(-0.324f, -0.141f, 0.499f);
            Vector3 endPosition = new Vector3(0.497f, -0.188f, 0.499f);

            TestHand hand = new TestHand(Handedness.Left);
            yield return hand.Show(pointOnCube); // Initially make sure that hand ray is pointed on cube surface so we won't go behind the cube with our ray
            yield return hand.MoveTo(transformHandlePosition);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            // move to left side of cube
            yield return hand.MoveTo(endPosition);

            // make sure translation is as expected and no other transform values have been modified through this
            Vector3 expectedPosition = new Vector3(1f, 0f, 1.5f);
            Vector3 expectedSize = Vector3.one * 0.5f;
            TestUtilities.AssertAboutEqual(boundsControl.transform.position, expectedPosition, "cube didn't move as expected");
            TestUtilities.AssertAboutEqual(boundsControl.transform.localScale, expectedSize, "cube scaled while translating");
            TestUtilities.AssertAboutEqual(boundsControl.transform.rotation, Quaternion.identity, "cube rotated while translating");

            GameObject.Destroy(boundsControl.gameObject);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        /// <summary>
        /// Test bounds control translation via motion controller
        /// Verifies gameobject has translation in one axis only applied and no other transform changes happen during interaction
        /// </summary>
        [UnityTest]
        public IEnumerator TranslateViaMotionController()
        {
            // Switch to motion controller
            var iss = PlayModeTestUtilities.GetInputSimulationService();
            var oldSimMode = iss.ControllerSimulationMode;
            iss.ControllerSimulationMode = ControllerSimulationMode.MotionController;

            BoundsControl boundsControl = InstantiateSceneAndDefaultBoundsControl();
            yield return VerifyInitialBoundsCorrect(boundsControl);
            boundsControl.TranslationHandlesConfig.ShowHandleForX = true;
            boundsControl.SmoothingActive = false;
            boundsControl.transform.position = new Vector3(-1.0f, 0.0f, 1.5f);

            Vector3 pointOnCube = new Vector3(-0.033f, -0.129f, 0.499f); // position where ray points on the test cube
            Vector3 transformHandlePosition = new Vector3(-0.538f, -0.313f, 0.31f);
            Vector3 endPosition = new Vector3(0.497f, -0.188f, 0.499f);

            TestMotionController motionController = new TestMotionController(Handedness.Right);
            yield return motionController.Show(pointOnCube); // Initially make sure that ray is pointed on cube surface so we won't go behind the cube with our ray
            yield return motionController.MoveTo(transformHandlePosition);
            SimulatedMotionControllerButtonState buttonState = new SimulatedMotionControllerButtonState()
            {
                IsSelecting = true
            };
            yield return motionController.SetState(buttonState);
            // move to left side of cube
            yield return motionController.MoveTo(endPosition);

            // make sure translation is as expected and no other transform values have been modified through this
            Vector3 expectedPosition = new Vector3(0.035f, 0f, 1.5f);
            Vector3 expectedSize = Vector3.one * 0.5f;
            TestUtilities.AssertAboutEqual(boundsControl.transform.position, expectedPosition, "cube didn't move as expected");
            TestUtilities.AssertAboutEqual(boundsControl.transform.localScale, expectedSize, "cube scaled while translating");
            TestUtilities.AssertAboutEqual(boundsControl.transform.rotation, Quaternion.identity, "cube rotated while translating");

            GameObject.Destroy(boundsControl.gameObject);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;

            // Restore the input simulation profile
            iss.ControllerSimulationMode = oldSimMode;

            yield return null;
        }

        /// <summary>
        /// Test bounds control translation via near interaction
        /// Verifies gameobject has translation in one axis only applied and no other transform changes happen during interaction
        /// </summary>
        [UnityTest]
        public IEnumerator TranslateViaNearInteraction()
        {
            BoundsControl boundsControl = InstantiateSceneAndDefaultBoundsControl();
            yield return VerifyInitialBoundsCorrect(boundsControl);
            boundsControl.TranslationHandlesConfig.ShowHandleForX = true;

            Vector3 pointOnCube = new Vector3(-0.033f, -0.129f, 0.499f); // position where hand ray points on center of the test cube
            Transform transformHandle = boundsControl.gameObject.transform.Find("rigRoot/faceCenter_0");
            Vector3 transformHandlePosition = transformHandle.position;
            Vector3 endPosition = transformHandlePosition + new Vector3(10.0f, 0.0f, 0.0f);

            TestHand hand = new TestHand(Handedness.Left);
            yield return hand.Show(pointOnCube); // Initially make sure that hand ray is pointed on cube surface so we won't go behind the cube with our ray
            yield return hand.MoveTo(transformHandlePosition);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            // move to left side of cube
            yield return hand.MoveTo(endPosition);

            // make sure translation is as expected and no other transform values have been modified through this
            Vector3 expectedPosition = new Vector3(10f, 0f, 1.5f);
            Vector3 expectedSize = Vector3.one * 0.5f;

            TestUtilities.AssertAboutEqual(boundsControl.transform.position, expectedPosition, "cube didn't move as expected");
            TestUtilities.AssertAboutEqual(boundsControl.transform.localScale, expectedSize, "cube scaled while translating");
            TestUtilities.AssertAboutEqual(boundsControl.transform.rotation, Quaternion.identity, "cube rotated while translating");

            GameObject.Destroy(boundsControl.gameObject);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        /// <summary>
        /// Test bounds control translation via HoloLens 1 interaction / GGV
        /// Verifies gameobject has translation in one axis only applied and no other transform changes happen during interaction
        /// </summary>
        [UnityTest]
        public IEnumerator TranslateViaHololens1Interaction()
        {
            // todo
            BoundsControl control = InstantiateSceneAndDefaultBoundsControl();
            yield return VerifyInitialBoundsCorrect(control);
            control.TranslationHandlesConfig.ShowHandleForZ = true;
            control.SmoothingActive = false;
            var iss = PlayModeTestUtilities.GetInputSimulationService();
            var oldSimMode = iss.ControllerSimulationMode;
            iss.ControllerSimulationMode = ControllerSimulationMode.HandGestures;

            // move camera to look at translation sphere
            Transform transformHandle = control.gameObject.transform.Find("rigRoot/faceCenter_2");
            CameraCache.Main.transform.LookAt(transformHandle.position);

            var startHandPos = new Vector3(0.191f, -0.07f, 0.499f);
            var endPoint = new Vector3(-0.368f, -0.221f, 0.499f);

            // perform tab with hand and drag to left 
            TestHand rightHand = new TestHand(Handedness.Right);
            yield return rightHand.Show(startHandPos);
            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            yield return rightHand.MoveTo(endPoint);

            // make sure x axis translation was performed and no other transform values have changed
            Vector3 expectedPosition = new Vector3(0f, 0f, 1.0f);
            Vector3 expectedSize = Vector3.one * 0.5f;

            TestUtilities.AssertAboutEqual(control.transform.position, expectedPosition, "cube didn't move as expected");
            TestUtilities.AssertAboutEqual(control.transform.localScale, expectedSize, "cube scaled while translating");
            TestUtilities.AssertAboutEqual(control.transform.rotation, Quaternion.identity, "cube rotated while translating");

            GameObject.Destroy(control.gameObject);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;

            iss.ControllerSimulationMode = oldSimMode;
            yield return null;
        }

        /// <summary>
        /// Test bounds control translate constraints via near interaction.
        /// Verifies gameobject won't translate when MoveAxisConstraint is applied.
        /// </summary>
        [UnityTest]
        public IEnumerator TranslationConstraintViaNearInteraction()
        {
            BoundsControl boundsControl = InstantiateSceneAndDefaultBoundsControl();
            yield return VerifyInitialBoundsCorrect(boundsControl);
            boundsControl.TranslationHandlesConfig.ShowHandleForX = true;

            var moveConstraint = boundsControl.EnsureComponent<MoveAxisConstraint>();
            moveConstraint.ConstraintOnMovement = AxisFlags.XAxis; // restrict translation in X axis
            yield return null;

            Vector3 pointOnCube = new Vector3(-0.033f, -0.129f, 0.499f); // position where hand ray points on center of the test cube
            Transform transformHandle = boundsControl.gameObject.transform.Find("rigRoot/faceCenter_0");
            Vector3 transformHandlePosition = transformHandle.position;
            Vector3 endPosition = transformHandlePosition + new Vector3(10.0f, 0.0f, 0.0f);

            TestHand hand = new TestHand(Handedness.Left);
            yield return hand.Show(pointOnCube); // Initially make sure that hand ray is pointed on cube surface so we won't go behind the cube with our ray
            yield return hand.MoveTo(transformHandlePosition);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            // move to left side of cube
            yield return hand.MoveTo(endPosition);

            // object shouldn't move with constraint attached
            Vector3 expectedPosition = new Vector3(0f, 0f, 1.5f);
            Vector3 expectedSize = Vector3.one * 0.5f;

            TestUtilities.AssertAboutEqual(boundsControl.transform.position, expectedPosition, "cube didn't move as expected");
            TestUtilities.AssertAboutEqual(boundsControl.transform.localScale, expectedSize, "cube scaled while translating");
            TestUtilities.AssertAboutEqual(boundsControl.transform.rotation, Quaternion.identity, "cube rotated while translating");

            GameObject.Destroy(boundsControl.gameObject);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        /// <summary>
        /// Test that changing the transform of the bounds control target (rotation, scale, translation)
        /// updates the rig bounds
        /// </summary>
        [UnityTest]
        public IEnumerator UpdateTransformUpdatesBounds()
        {
            BoundsControl boundsControl = InstantiateSceneAndDefaultBoundsControl();
            yield return VerifyInitialBoundsCorrect(boundsControl);
            boundsControl.HideElementsInInspector = false;
            yield return null;

            var startBounds = GetBoundsControlRigBounds(boundsControl);
            TestUtilities.AssertAboutEqual(startBounds.center, boundsControlStartCenter, "boundsControl incorrect center at start");
            TestUtilities.AssertAboutEqual(startBounds.size, boundsControlStartScale, "boundsControl incorrect size at start");

            boundsControl.gameObject.transform.localScale *= 2;
            yield return null;

            var afterScaleBounds = GetBoundsControlRigBounds(boundsControl);
            var scaledSize = boundsControlStartScale * 2;
            TestUtilities.AssertAboutEqual(afterScaleBounds.center, boundsControlStartCenter, "boundsControl incorrect center after scale");
            TestUtilities.AssertAboutEqual(afterScaleBounds.size, scaledSize, "boundsControl incorrect size after scale");

            boundsControl.gameObject.transform.position += Vector3.one;
            yield return null;
            var afterTranslateBounds = GetBoundsControlRigBounds(boundsControl);
            var afterTranslateCenter = Vector3.one + boundsControlStartCenter;

            TestUtilities.AssertAboutEqual(afterTranslateBounds.center, afterTranslateCenter, "boundsControl incorrect center after translate");
            TestUtilities.AssertAboutEqual(afterTranslateBounds.size, scaledSize, "boundsControl incorrect size after translate");

            var c0 = boundsControl.gameObject.transform.Find("rigRoot/corner_0");
            var boundsControlBottomCenter = afterTranslateBounds.center - Vector3.up * afterTranslateBounds.extents.y;
            Vector3 cc0 = c0.position - boundsControlBottomCenter;
            float rotateAmount = 30;
            boundsControl.gameObject.transform.Rotate(new Vector3(0, rotateAmount, 0));
            yield return null;
            Vector3 cc0_rotated = c0.position - boundsControlBottomCenter;
            Assert.AreApproximatelyEqual(Vector3.Angle(cc0, cc0_rotated), 30, $"rotated angle is not correct. expected {rotateAmount} but got {Vector3.Angle(cc0, cc0_rotated)}");

            GameObject.Destroy(boundsControl.gameObject);
        }

        /// <summary>
        /// Ensure that while using BoundingBox, if that object gets
        /// deactivated, that BoundingBox no longer transforms that object.
        /// </summary>
        [UnityTest]
        public IEnumerator DisableObject()
        {
            var boundsControl = InstantiateSceneAndDefaultBoundsControl();
            yield return VerifyInitialBoundsCorrect(boundsControl);

            Vector3 initialScale = boundsControl.transform.localScale;

            const int numHandSteps = 1;

            Vector3 initialHandPosition = new Vector3(0, 0, 0.5f);
            var frontRightCornerPos = boundsControl.gameObject.transform.Find("rigRoot/corner_3").position; // front right corner is corner 3
            TestHand hand = new TestHand(Handedness.Right);

            // Hands grab object at initial position
            yield return hand.Show(initialHandPosition);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.OpenSteadyGrabPoint);
            yield return hand.MoveTo(frontRightCornerPos, numHandSteps);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);

            // Verify that scale works before deactivating
            yield return hand.Move(Vector3.right * 0.2f, numHandSteps);
            Vector3 afterTransformScale = boundsControl.transform.localScale;
            Assert.AreNotEqual(initialScale, afterTransformScale);

            // Deactivate object and ensure that we don't scale
            boundsControl.gameObject.SetActive(false);
            yield return null;
            boundsControl.gameObject.SetActive(true);
            yield return hand.Move(Vector3.right * 0.2f, numHandSteps);
            Assert.AreEqual(afterTransformScale, boundsControl.transform.localScale);
        }

        /// <summary>
        /// Tests proximity scaling on scale handles of bounds control
        /// Verifies default behavior of handles with effect enabled / disabled as well as custom runtime configured scaling / distance values
        /// </summary>
        [UnityTest]
        public IEnumerator ProximityOnScaleHandles()
        {
            var boundsControl = InstantiateSceneAndDefaultBoundsControl();
            yield return VerifyInitialBoundsCorrect(boundsControl);

            // 1. test no proximity scaling active per default
            ScaleHandlesConfiguration scaleHandleConfig = boundsControl.ScaleHandlesConfig;
            Vector3 defaultHandleSize = Vector3.one * scaleHandleConfig.HandleSize;

            Vector3 initialHandPosition = new Vector3(0, 0, 0f);
            // this is specific to scale handles
            Transform scaleHandle = boundsControl.gameObject.transform.Find("rigRoot/corner_3");
            Transform proximityScaledVisual = scaleHandle.GetChild(0);
            var frontRightCornerPos = scaleHandle.position; // front right corner is corner 
            Assert.IsNotNull(proximityScaledVisual, "Couldn't get visual gameobject for scale handle");
            Assert.IsTrue(proximityScaledVisual.name == "visuals", "scale visual has unexpected name");

            yield return null;
            // verify no proximity scaling applied per default
            Assert.AreEqual(proximityScaledVisual.localScale, defaultHandleSize, "Handle was scaled even though proximity effect wasn't active");
            TestHand hand = new TestHand(Handedness.Left);
            Vector3 initialScale = boundsControl.transform.localScale;

            // Hands grab object at initial position
            yield return hand.Show(initialHandPosition);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.OpenSteadyGrabPoint);
            yield return hand.MoveTo(frontRightCornerPos);
            yield return null;

            // we're in proximity scaling range - check if proximity scaling wasn't applied
            Assert.AreEqual(proximityScaledVisual.localScale, defaultHandleSize, "Handle was scaled even though proximity effect wasn't active");

            //// reset hand
            yield return hand.MoveTo(initialHandPosition);

            // 2. enable proximity scaling and test defaults
            ProximityEffectConfiguration proximityConfig = boundsControl.HandleProximityEffectConfig;
            proximityConfig.ProximityEffectActive = true;
            proximityConfig.CloseGrowRate = 1.0f;
            proximityConfig.MediumGrowRate = 1.0f;
            proximityConfig.FarGrowRate = 1.0f;
            yield return null; // wait so rig gameobjects get recreated
            yield return TestCurrentProximityConfiguration(boundsControl, hand, "Defaults");

            // reset hand
            yield return hand.MoveTo(initialHandPosition);

            // 3. now test custom configuration is applied during runtime
            proximityConfig.CloseScale = 4.0f;
            proximityConfig.MediumScale = 3.0f;
            proximityConfig.FarScale = 2.0f;

            proximityConfig.ObjectMediumProximity = 0.2f;
            proximityConfig.ObjectCloseProximity = 0.1f;

            yield return null; // wait so rig gameobjects get recreated
            yield return TestCurrentProximityConfiguration(boundsControl, hand, "Custom runtime config max");
        }

        /// <summary>
        /// This tests far, medium and close proximity scaling on scale handles by moving the test hand in the corresponding distance ranges
        /// </summary>
        /// <param name="boundsControl">Bounds Control to test on</param>
        /// <param name="hand">Test hand to use for testing proximity to handle</param>
        private IEnumerator TestCurrentProximityConfiguration(BoundsControl boundsControl, TestHand hand, string testDescription)
        {
            // get config and scaling handle
            ScaleHandlesConfiguration scaleHandleConfig = boundsControl.ScaleHandlesConfig;
            Vector3 defaultHandleSize = Vector3.one * scaleHandleConfig.HandleSize;
            Transform scaleHandle = boundsControl.gameObject.transform.Find("rigRoot/corner_3");
            Transform proximityScaledVisual = scaleHandle.GetChild(0);
            var frontRightCornerPos = scaleHandle.position;
            // check far scale applied
            ProximityEffectConfiguration proximityConfig = boundsControl.HandleProximityEffectConfig;
            Vector3 expectedFarScale = defaultHandleSize * proximityConfig.FarScale;
            Assert.AreEqual(proximityScaledVisual.localScale, expectedFarScale, testDescription + " - Proximity far scale wasn't applied to handle");

            // move into medium range and check if scale was applied
            Vector3 mediumProximityTestDist = frontRightCornerPos;
            mediumProximityTestDist.x += proximityConfig.ObjectMediumProximity;
            yield return hand.MoveTo(mediumProximityTestDist);
            Vector3 expectedMediumScale = defaultHandleSize * proximityConfig.MediumScale;
            Assert.AreEqual(proximityScaledVisual.localScale, expectedMediumScale, testDescription + " - Proximity medium scale wasn't applied to handle");

            // move into close scale range and check if scale was applied
            Vector3 closeProximityTestDir = frontRightCornerPos;
            closeProximityTestDir.x += proximityConfig.ObjectCloseProximity;
            yield return hand.MoveTo(closeProximityTestDir);
            Vector3 expectedCloseScale = defaultHandleSize * proximityConfig.CloseScale;
            Assert.AreEqual(proximityScaledVisual.localScale, expectedCloseScale, testDescription + " - Proximity close scale wasn't applied to handle");

            // move out of close scale again - should fall back to medium proximity
            closeProximityTestDir = mediumProximityTestDist;
            yield return hand.MoveTo(closeProximityTestDir);
            Assert.AreEqual(proximityScaledVisual.localScale, expectedMediumScale, testDescription + " - Proximity medium scale wasn't applied to handle");

            // move out of medium proximity and check if far scaling is applied
            mediumProximityTestDist = Vector3.zero;
            yield return hand.MoveTo(mediumProximityTestDist);
            Assert.AreEqual(proximityScaledVisual.localScale, expectedFarScale, testDescription + " - Proximity far scale wasn't applied to handle");

            yield return null;
        }

        /// <summary>
        /// Tests setting a target in code that is a different gameobject than the gameobject the bounds control component is attached to
        /// </summary>
        [UnityTest]
        public IEnumerator SetTarget()
        {
            // create cube without control
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = boundsControlStartCenter;

            MixedRealityPlayspace.PerformTransformation(
            p =>
            {
                p.position = Vector3.zero;
                p.LookAt(cube.transform.position);
            });

            cube.transform.localScale = boundsControlStartScale;

            // create another gameobject with boundscontrol attached 
            var emptyGameObject = new GameObject("empty");
            BoundsControl boundsControl = emptyGameObject.AddComponent<BoundsControl>();
            yield return new WaitForFixedUpdate();

            // fetch root and scale handle
            GameObject rigRoot = boundsControl.transform.Find("rigRoot").gameObject;
            Assert.IsNotNull(rigRoot, "rigRoot couldn't be found");
            var scaleHandle = boundsControl.transform.Find("rigRoot/corner_3");
            Assert.IsNotNull(scaleHandle, "scale handle couldn't be found");

            // verify root is parented to bounds control gameobject
            Assert.AreEqual(boundsControl.gameObject, rigRoot.transform.parent.gameObject);

            // set target to cube
            boundsControl.Target = cube;
            Assert.IsNotNull(rigRoot, "rigRoot was destroyed on setting a new target");
            Assert.IsNotNull(scaleHandle, "scale handle was destroyed on setting a new target");

            // verify root is parented to target gameobject
            Assert.AreEqual(cube, rigRoot.transform.parent.gameObject);

            // grab corner and scale object
            Vector3 initialHandPosition = new Vector3(0, 0, 0.5f);
            int numSteps = 30;
            var delta = new Vector3(0.1f, 0.1f, 0f);
            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.Show(initialHandPosition);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.OpenSteadyGrabPoint);
            var scaleHandlePos = scaleHandle.position;
            yield return hand.MoveTo(scaleHandlePos, numSteps);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            yield return hand.MoveTo(scaleHandlePos + delta, numSteps);

            var endBounds = cube.GetComponent<BoxCollider>().bounds;
            Vector3 expectedCenter = new Vector3(0.033f, 0.033f, 1.467f);
            Vector3 expectedSize = Vector3.one * .567f;
            TestUtilities.AssertAboutEqual(endBounds.center, expectedCenter, "endBounds incorrect center");
            TestUtilities.AssertAboutEqual(endBounds.size, expectedSize, "endBounds incorrect size");

            Object.Destroy(emptyGameObject);
            Object.Destroy(cube);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        /// <summary>
        /// Tests the different activation flags bounding box handles can be activated with
        /// </summary>
        [UnityTest]
        public IEnumerator ActivationTypeTest()
        {
            var boundsControl = InstantiateSceneAndDefaultBoundsControl();
            yield return VerifyInitialBoundsCorrect(boundsControl);
            boundsControl.gameObject.AddComponent<NearInteractionGrabbable>();

            // cache rig root for verifying that we're not recreating the rig on config changes
            GameObject rigRoot = boundsControl.transform.Find("rigRoot").gameObject;
            Assert.IsNotNull(rigRoot, "rigRoot couldn't be found");

            // default case is activation on start
            Assert.IsTrue(boundsControl.Active, "default behavior should be bounds control activation on start");
            Assert.IsFalse(boundsControl.WireframeOnly, "default behavior should be not wireframe only");

            boundsControl.BoundsControlActivation = BoundsControlActivationType.ActivateByProximity;
            // make sure rigroot is still alive
            Assert.IsNotNull(rigRoot, "rigRoot got destroyed while configuring bounds control during runtime");
            // handles should be disabled now 
            Assert.IsTrue(boundsControl.Active, "control should be active");
            Assert.IsTrue(boundsControl.WireframeOnly, "wireframeonly should be enabled");

            // move to bounds control with hand and check if it activates on proximity
            Transform cornerVisual = rigRoot.transform.Find("corner_1/visuals");
            Assert.IsNotNull(cornerVisual, "couldn't find scale handle visual");
            TestHand hand = new TestHand(Handedness.Right);
            Vector3 pointOnCube = new Vector3(-0.033f, -0.129f, 0.499f); // position where hand ray points on center of the test cube
            yield return hand.Show(pointOnCube);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Open);
            Vector3 pointOnCubeNear = boundsControl.transform.position;
            pointOnCubeNear.z = cornerVisual.position.z;
            yield return hand.MoveTo(pointOnCubeNear);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();
            Assert.IsTrue(boundsControl.Active, "control should be active");
            Assert.IsFalse(boundsControl.WireframeOnly, "wireframeonly should be disabled");

            yield return hand.MoveTo(pointOnCube);
            Assert.IsTrue(boundsControl.Active, "control should be active");
            Assert.IsTrue(boundsControl.WireframeOnly, "wireframeonly should be enabled");
            yield return hand.Hide();

            // check far pointer activation
            boundsControl.BoundsControlActivation = BoundsControlActivationType.ActivateByPointer;
            yield return hand.Show(cornerVisual.position);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();
            // shouldn't be enabled on proximity of near pointer
            Assert.IsTrue(boundsControl.Active, "control should be active");
            Assert.IsTrue(boundsControl.WireframeOnly, "wireframeonly should be enabled");
            // enable on far pointer
            yield return hand.MoveTo(pointOnCube);

            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();
            Assert.IsTrue(boundsControl.Active, "control should be active");
            Assert.IsFalse(boundsControl.WireframeOnly, "wireframeonly should be disabled");
            yield return hand.Hide();
            Assert.IsTrue(boundsControl.WireframeOnly, "wireframeonly should be enabled");

            boundsControl.BoundsControlActivation = BoundsControlActivationType.ActivateByProximityAndPointer;
            yield return hand.Show(cornerVisual.position);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();
            // should be enabled on proximity of near pointer
            Assert.IsTrue(boundsControl.Active, "control should be active");
            Assert.IsFalse(boundsControl.WireframeOnly, "wireframeonly should be disabled");
            // enable on far pointer
            yield return hand.MoveTo(pointOnCube);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();
            Assert.IsTrue(boundsControl.Active, "control should be active");
            Assert.IsFalse(boundsControl.WireframeOnly, "wireframeonly should be disabled");
            yield return hand.Hide();

            // check manual activation
            boundsControl.BoundsControlActivation = BoundsControlActivationType.ActivateManually;
            Assert.IsFalse(boundsControl.Active, "control shouldn't be active");

            boundsControl.Active = true;
            Assert.IsTrue(boundsControl.Active, "control should be active");
            Assert.IsFalse(boundsControl.WireframeOnly, "wireframeonly should be disabled");
            yield return null;
        }

        /// <summary>
        /// Tests visibility changes of scale handles.
        /// Makes sure rig isn't recreated and visibility restores as expected when disabling the entire control.
        /// </summary>
        [UnityTest]
        public IEnumerator ScaleHandleVisibilityTest()
        {
            var boundsControl = InstantiateSceneAndDefaultBoundsControl();
            yield return VerifyInitialBoundsCorrect(boundsControl);

            // cache rig root for verifying that we're not recreating the rig on config changes
            GameObject rigRoot = boundsControl.transform.Find("rigRoot").gameObject;
            Assert.IsNotNull(rigRoot, "rigRoot couldn't be found");

            // cache rig root for verifying that we're not recreating the rig on config changes
            Transform scaleHandle = rigRoot.transform.Find("corner_3");
            Assert.IsNotNull(scaleHandle, "couldn't find handle");

            // test scale handle behavior
            Assert.IsTrue(scaleHandle.gameObject.activeSelf, "scale handle not active by default");
            ScaleHandlesConfiguration scaleHandleConfig = boundsControl.ScaleHandlesConfig;
            scaleHandleConfig.ShowScaleHandles = false;
            Assert.IsNotNull(rigRoot, "rigRoot was destroyed on hiding handles");
            Assert.IsNotNull(scaleHandle, "handle was destroyed on hide");
            Assert.IsFalse(scaleHandle.gameObject.activeSelf, "handle wasn't disabled on hide");

            scaleHandleConfig.ShowScaleHandles = true;
            Assert.IsTrue(scaleHandle.gameObject.activeSelf, "handle wasn't enabled on show");

            // make sure handles are disabled and enabled when bounds control is deactivated / activated
            boundsControl.Active = false;
            Assert.IsNotNull(rigRoot, "rigRoot was destroyed on disabling bounds control");
            Assert.IsFalse(scaleHandle.gameObject.activeSelf, "scale handle not disabled");

            // set active again and make sure internal states have been restored
            boundsControl.Active = true;
            Assert.IsNotNull(rigRoot, "rigRoot was destroyed on enabling bounds control");
            Assert.IsTrue(scaleHandle.gameObject.activeSelf, "scale handle not enabled");
            yield return null;
        }

        private int GetFirstHandleIndexForAxis(CardinalAxisType axisType, ref CardinalAxisType[] handleAxisTypes)
        {
            for (int i = 0; i < handleAxisTypes.Length; ++i)
            {
                if (handleAxisTypes[i] == axisType)
                {
                    return i;
                }
            }

            Debug.LogError("Couldn't find index for axis");
            return 0;
        }

        /// <summary>
        /// Tests visibility changes of per axis handle types.
        /// Makes sure rig isn't recreated and visibility restores as expected when disabling the entire control.
        /// </summary>
        [UnityTest]
        public IEnumerator PerAxisHandleVisibilityTest([ValueSource("perAxisHandleTestData")] PerAxisHandleTestData testData)
        {
            var boundsControl = InstantiateSceneAndDefaultBoundsControl();
            yield return VerifyInitialBoundsCorrect(boundsControl);

            // cache rig root for verifying that we're not recreating the rig on config changes
            GameObject rigRoot = boundsControl.transform.Find("rigRoot").gameObject;
            Assert.IsNotNull(rigRoot, "rigRoot couldn't be found");

            // test rotation handle behavior
            string handleAxisXName = testData.handleName + "_" + GetFirstHandleIndexForAxis(CardinalAxisType.X, ref testData.handleAxisTypes);
            string handleAxisYName = testData.handleName + "_" + GetFirstHandleIndexForAxis(CardinalAxisType.Y, ref testData.handleAxisTypes);
            string handleAxisZName = testData.handleName + "_" + GetFirstHandleIndexForAxis(CardinalAxisType.Z, ref testData.handleAxisTypes);
            Transform handleAxisX = rigRoot.transform.Find(handleAxisXName);
            Transform handleAxisY = rigRoot.transform.Find(handleAxisYName);
            Transform handleAxisZ = rigRoot.transform.Find(handleAxisZName);

            System.Reflection.PropertyInfo propName = boundsControl.GetType().GetProperty(testData.configPropertyName);
            PerAxisHandlesConfiguration config = (PerAxisHandlesConfiguration)propName.GetValue(boundsControl);

            Assert.AreEqual(handleAxisX.gameObject.activeSelf, config.ShowHandleForX, "handle x default value not applied");
            Assert.AreEqual(handleAxisY.gameObject.activeSelf, config.ShowHandleForY, "handle y default value not applied");
            Assert.AreEqual(handleAxisZ.gameObject.activeSelf, config.ShowHandleForZ, "handle z default value not applied");

            // disable visibility for each component
            config.ShowHandleForX = false;
            config.ShowHandleForY = true;
            config.ShowHandleForZ = true;
            Assert.IsNotNull(rigRoot, "rigRoot was destroyed on hiding handles");
            Assert.IsNotNull(handleAxisX, "handle was destroyed on hide");
            Assert.IsFalse(handleAxisX.gameObject.activeSelf, "handle x not hidden");
            Assert.IsTrue(handleAxisY.gameObject.activeSelf, "handle y not active");
            Assert.IsTrue(handleAxisZ.gameObject.activeSelf, "handle z not active");

            config.ShowHandleForY = false;
            Assert.IsFalse(handleAxisX.gameObject.activeSelf, "handle x not hidden");
            Assert.IsFalse(handleAxisY.gameObject.activeSelf, "handle y not hidden");
            Assert.IsTrue(handleAxisZ.gameObject.activeSelf, "handle z not active");

            config.ShowHandleForX = true;
            config.ShowHandleForY = true;
            config.ShowHandleForZ = false;
            Assert.IsTrue(handleAxisX.gameObject.activeSelf, "rotation handle x not active");
            Assert.IsTrue(handleAxisY.gameObject.activeSelf, "rotation handle y not active");
            Assert.IsFalse(handleAxisZ.gameObject.activeSelf, "rotation handle z not hidden");

            // make sure handles are disabled and enabled when bounds control is deactivated / activated
            boundsControl.Active = false;
            Assert.IsNotNull(rigRoot, "rigRoot was destroyed on disabling bounds control");
            Assert.IsFalse(handleAxisX.gameObject.activeSelf, "rotation handle x not hidden");
            Assert.IsFalse(handleAxisY.gameObject.activeSelf, "rotation handle y not hidden");
            Assert.IsFalse(handleAxisZ.gameObject.activeSelf, "rotation handle z not hidden");

            // set active again and make sure internal states have been restored
            boundsControl.Active = true;
            Assert.IsNotNull(rigRoot, "rigRoot was destroyed on enabling bounds control");
            Assert.IsTrue(handleAxisX.gameObject.activeSelf, "rotation handle x not active");
            Assert.IsTrue(handleAxisY.gameObject.activeSelf, "rotation handle y not active");
            Assert.IsFalse(handleAxisZ.gameObject.activeSelf, "rotation handle z not hidden");

            // enable z axis again and verify
            config.ShowHandleForZ = true;
            Assert.IsTrue(handleAxisX.gameObject.activeSelf, "rotation handle x not active");
            Assert.IsTrue(handleAxisY.gameObject.activeSelf, "rotation handle y not active");
            Assert.IsTrue(handleAxisZ.gameObject.activeSelf, "rotation handle z not active");

            // test disabling all rotation handles before activating the gameobject
            // verifies bug https://github.com/microsoft/MixedRealityToolkit-Unity/issues/8239
            boundsControl.gameObject.SetActive(false);
            yield return null;
            config.ShowHandleForX = false;
            config.ShowHandleForY = false;
            config.ShowHandleForZ = false;
            boundsControl.BoundsControlActivation = BoundsControlActivationType.ActivateOnStart;
            boundsControl.gameObject.SetActive(true);
            yield return null;

            // refetch transforms
            rigRoot = boundsControl.transform.Find("rigRoot").gameObject;
            Assert.IsNotNull(rigRoot, "rigRoot couldn't be found");
            handleAxisX = rigRoot.transform.Find(handleAxisXName);
            Assert.IsNotNull(handleAxisX, "rotation handle couldn't be found");
            handleAxisY = rigRoot.transform.Find(handleAxisYName);
            Assert.IsNotNull(handleAxisY, "rotation handle couldn't be found");
            handleAxisZ = rigRoot.transform.Find(handleAxisZName);
            Assert.IsNotNull(handleAxisZ, "rotation handle couldn't be found");

            // check handle visibility
            Assert.IsFalse(handleAxisX.gameObject.activeSelf, "rotation handle x active");
            Assert.IsFalse(handleAxisY.gameObject.activeSelf, "rotation handle y active");
            Assert.IsFalse(handleAxisZ.gameObject.activeSelf, "rotation handle z active");
        }

        /// <summary>
        /// Tests that draw tether flag gets propagated to NearInteractionGrabbable on configuration changes.
        /// Makes sure that rig / visuals aren't recreated.
        /// </summary>
        [UnityTest]
        public IEnumerator ManipulationTetherTest()
        {
            var boundsControl = InstantiateSceneAndDefaultBoundsControl();
            yield return VerifyInitialBoundsCorrect(boundsControl);

            // cache rig root for verifying that we're not recreating the rig on config changes
            GameObject rigRoot = boundsControl.transform.Find("rigRoot").gameObject;
            Assert.IsNotNull(rigRoot, "rigRoot couldn't be found");

            // test default and runtime changing draw tether flag of both handle types
            yield return TestDrawManipulationTetherFlag(boundsControl.ScaleHandlesConfig, rigRoot, "corner_3");
            yield return TestDrawManipulationTetherFlag(boundsControl.RotationHandlesConfig, rigRoot, "midpoint_2");
            boundsControl.TranslationHandlesConfig.ShowHandleForZ = true;
            yield return TestDrawManipulationTetherFlag(boundsControl.TranslationHandlesConfig, rigRoot, "faceCenter_2");
            yield return null;
        }

        private IEnumerator TestDrawManipulationTetherFlag(HandlesBaseConfiguration config, GameObject rigRoot, string handleName)
        {
            Assert.IsTrue(config.DrawTetherWhenManipulating, "tether drawing should be on as default");

            // cache rig root for verifying that we're not recreating the rig on config changes
            Transform handle = rigRoot.transform.Find(handleName);
            Assert.IsNotNull(handle, "couldn't find handle");
            var grabbable = handle.gameObject.GetComponent<NearInteractionGrabbable>();
            Assert.AreEqual(config.DrawTetherWhenManipulating, grabbable.ShowTetherWhenManipulating, "draw tether wasn't propagated to handle NearInteractionGrabbable component");

            config.DrawTetherWhenManipulating = false;
            Assert.IsNotNull(rigRoot, "rigRoot got destroyed while configuring bounds control during runtime");
            Assert.IsNotNull(handle, "handle was destroyed when changing tether visibility");
            Assert.IsFalse(grabbable.ShowTetherWhenManipulating, "show tether wasn't applied to NearInteractionGrabbable of handle");

            yield return null;
        }

        /// <summary>
        /// Tests adding padding to the bounds of a bounds control and verifies if handles have moved accordingly.
        /// Also verifies that visuals didn't get recreated during padding value changes.
        /// </summary>
        [UnityTest]
        public IEnumerator BoundsControlPaddingTest()
        {
            var boundsControl = InstantiateSceneAndDefaultBoundsControl();
            yield return VerifyInitialBoundsCorrect(boundsControl);

            // fetch rigroot
            GameObject rigRoot = boundsControl.transform.Find("rigRoot").gameObject;
            Assert.IsNotNull(rigRoot, "rigRoot couldn't be found");
            Transform cornerVisual = rigRoot.transform.Find("corner_3/visuals");
            Assert.IsNotNull(cornerVisual, "couldn't find corner visual");
            var cornerVisualPosition = cornerVisual.position;
            var defaultPadding = boundsControl.BoxPadding;
            var targetBoundsOriginal = boundsControl.TargetBounds; // this has the default padding already applied
            var targetBoundsSize = targetBoundsOriginal.size;
            Vector3 targetBoundsScaleInv = new Vector3(1.0f / targetBoundsOriginal.transform.lossyScale.x, 1.0f / targetBoundsOriginal.transform.lossyScale.y, 1.0f / targetBoundsOriginal.transform.lossyScale.z);

            // set padding
            boundsControl.BoxPadding = Vector3.one * 0.5f;
            var scaledPaddingDelta = Vector3.Scale(boundsControl.BoxPadding - defaultPadding, targetBoundsScaleInv);
            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            // check rig or handle isn't recreated
            Assert.IsNotNull(rigRoot, "rigRoot got destroyed while configuring bounds control during runtime");
            Assert.IsNotNull(cornerVisual, "handle visual was recreated on changing padding");

            // check padding is applied to bounds 
            var newBoundsSize = boundsControl.TargetBounds.size;
            Assert.AreEqual(newBoundsSize, targetBoundsSize + scaledPaddingDelta, "padding wasn't applied to target bounds");

            // check padding is applied to handle position - corners should have moved half the padding distance 
            var cornerPosDiff = cornerVisualPosition - cornerVisual.position;
            var paddingHalf = boundsControl.BoxPadding * 0.5f;
            Assert.AreEqual(cornerPosDiff.sqrMagnitude, paddingHalf.sqrMagnitude, "corner visual didn't move on applying padding to control");

            yield return null;
        }

        /// <summary>
        /// Tests toggling link visibility and verifying visuals are not recreated.
        /// </summary>
        [UnityTest]
        public IEnumerator LinksVisibilityTest()
        {
            var boundsControl = InstantiateSceneAndDefaultBoundsControl();
            yield return VerifyInitialBoundsCorrect(boundsControl);

            // fetch rigroot, corner visual and rotation handle config
            GameObject rigRoot = boundsControl.transform.Find("rigRoot").gameObject;
            Assert.IsNotNull(rigRoot, "rigRoot couldn't be found");

            Transform linkVisual = rigRoot.transform.Find("link_0");
            Assert.IsNotNull(linkVisual, "link visual couldn't be found");
            Assert.IsTrue(linkVisual.gameObject.activeSelf, "links not visible by default");
            yield return new WaitForFixedUpdate();

            // disable wireframe and make sure we're not recreating anything
            LinksConfiguration linkConfiguration = boundsControl.LinksConfig;
            linkConfiguration.ShowWireFrame = false;
            Assert.IsNotNull(rigRoot, "rigRoot got destroyed while configuring bounds control during runtime");
            Assert.IsNotNull(linkVisual, "link visual was recreated on changing visibility");
            Assert.IsFalse(linkVisual.gameObject.activeSelf, "links did not get deactivated on hide");
            yield return new WaitForFixedUpdate();

            // enable links again
            linkConfiguration.ShowWireFrame = true;
            Assert.IsNotNull(rigRoot, "rigRoot got destroyed while configuring bounds control during runtime");
            Assert.IsNotNull(linkVisual, "link visual was recreated on changing visibility");
            Assert.IsTrue(linkVisual.gameObject.activeSelf, "links did not get activated on show");
            yield return new WaitForFixedUpdate();

            yield return null;
        }

        /// <summary>
        /// Verifies links are getting disabled on flattening the bounds control without recreating any visuals
        /// </summary>
        [UnityTest]
        public IEnumerator LinksFlattenTest()
        {
            // test flatten and unflatten for links
            var boundsControl = InstantiateSceneAndDefaultBoundsControl();
            yield return VerifyInitialBoundsCorrect(boundsControl);

            // fetch rigroot, and one of the link visuals
            GameObject rigRoot = boundsControl.transform.Find("rigRoot").gameObject;
            Assert.IsNotNull(rigRoot, "rigRoot couldn't be found");

            Transform linkVisual = rigRoot.transform.Find("link_0");
            Assert.IsNotNull(linkVisual, "link visual couldn't be found");

            Assert.IsTrue(linkVisual.gameObject.activeSelf, "link with index 0 wasn't enabled by default");

            // flatten x axis and make sure link gets deactivated
            boundsControl.FlattenAxis = FlattenModeType.FlattenX;
            Assert.IsFalse(linkVisual.gameObject.activeSelf, "link with index 0 wasn't disabled when control was flattened in X axis");
            Assert.IsNotNull(rigRoot, "rigRoot got destroyed while configuring bounds control during runtime");

            // unflatten the control again and make sure link gets activated accordingly
            boundsControl.FlattenAxis = FlattenModeType.DoNotFlatten;
            Assert.IsTrue(linkVisual.gameObject.activeSelf, "link with index 0 wasn't enabled on unflatten");
            Assert.IsNotNull(rigRoot, "rigRoot got destroyed while configuring bounds control during runtime");
            yield return null;
        }

        /// <summary>
        /// Tests link radius config changes are applied to the link visual without recreating them.
        /// </summary>
        [UnityTest]
        public IEnumerator LinksRadiusTest()
        {
            var boundsControl = InstantiateSceneAndDefaultBoundsControl();
            yield return VerifyInitialBoundsCorrect(boundsControl);

            // fetch rigroot, and one of the link visuals
            GameObject rigRoot = boundsControl.transform.Find("rigRoot").gameObject;
            Assert.IsNotNull(rigRoot, "rigRoot couldn't be found");

            Transform linkVisual = rigRoot.transform.Find("link_0");
            Assert.IsNotNull(linkVisual, "link visual couldn't be found");

            LinksConfiguration linkConfiguration = boundsControl.LinksConfig;
            // verify default radius
            Assert.AreEqual(linkVisual.localScale.x, linkConfiguration.WireframeEdgeRadius, "Wireframe default edge radius wasn't applied to link local scale");
            // change radius
            linkConfiguration.WireframeEdgeRadius = 0.5f;
            Assert.IsNotNull(rigRoot, "rigRoot got destroyed while configuring bounds control during runtime");
            Assert.IsNotNull(linkVisual, "link visual shouldn't be destroyed when changing edge radius");

            // check if radius was applied
            Assert.AreEqual(linkVisual.localScale.x, linkConfiguration.WireframeEdgeRadius, "Wireframe edge radius wasn't applied to link local scale");

            yield return null;
        }


        /// <summary>
        /// Verifies link shapes get applied to link visuals once they are changed in the configuration.
        /// Makes sure links are not destroyed but only mesh filter gets replaced.
        /// </summary>
        [UnityTest]
        public IEnumerator LinksShapeTest()
        {
            var boundsControl = InstantiateSceneAndDefaultBoundsControl();
            yield return VerifyInitialBoundsCorrect(boundsControl);

            // fetch rigroot, and one of the link visuals
            GameObject rigRoot = boundsControl.transform.Find("rigRoot").gameObject;
            Assert.IsNotNull(rigRoot, "rigRoot couldn't be found");

            Transform linkVisual = rigRoot.transform.Find("link_0");
            Assert.IsNotNull(linkVisual, "link visual couldn't be found");

            LinksConfiguration linkConfiguration = boundsControl.LinksConfig;
            // verify default shape
            Assert.AreEqual(linkConfiguration.WireframeShape, WireframeType.Cubic);
            var linkMeshFilter = linkVisual.GetComponent<MeshFilter>();

            Assert.IsTrue(linkMeshFilter.mesh.name == "Cube Instance", "Links weren't created with default cube");

            // change shape - this should only affect the sharedmesh property of the mesh filter
            linkConfiguration.WireframeShape = WireframeType.Cylindrical;
            Assert.IsNotNull(rigRoot, "rigRoot got destroyed while configuring bounds control during runtime");
            Assert.IsNotNull(linkVisual, "link visual shouldn't be destroyed when switching mesh");

            // check if shape was applied
            Assert.IsTrue(linkMeshFilter.mesh.name == "Cylinder Instance", "Link shape wasn't switched to cylinder");

            yield return null;
        }

        /// <summary>
        /// Tests changing the links material during runtime and making sure links and rig are not recreated.
        /// </summary>
        [UnityTest]
        public IEnumerator LinksMaterialTest()
        {
            var boundsControl = InstantiateSceneAndDefaultBoundsControl();
            yield return VerifyInitialBoundsCorrect(boundsControl);

            // fetch rigroot and one of the link visuals
            GameObject rigRoot = boundsControl.transform.Find("rigRoot").gameObject;
            Assert.IsNotNull(rigRoot, "rigRoot couldn't be found");

            Transform linkVisual = rigRoot.transform.Find("link_0");
            Assert.IsNotNull(linkVisual, "link visual couldn't be found");
            LinksConfiguration linkConfiguration = boundsControl.LinksConfig;
            // set material and make sure rig root and link isn't destroyed while doing so
            linkConfiguration.WireframeMaterial = testMaterial;
            Assert.IsNotNull(rigRoot, "rigRoot got destroyed while configuring bounds control during runtime");
            Assert.IsNotNull(linkVisual, "link visual was recreated on setting material");
            // make sure color changed on visual
            Assert.AreEqual(linkVisual.GetComponent<Renderer>().material.color, testMaterial.color, "wireframe material wasn't applied to visual");

            yield return null;
        }

        /// <summary>
        /// Tests changing the box display default and grabbed material during runtime,
        /// making sure neither box display nor rig get recreated.
        /// </summary>
        [UnityTest]
        public IEnumerator BoxDisplayMaterialTest()
        {
            var boundsControl = InstantiateSceneAndDefaultBoundsControl();
            yield return VerifyInitialBoundsCorrect(boundsControl);

            // fetch rigroot, corner visual and rotation handle config
            GameObject rigRoot = boundsControl.transform.Find("rigRoot").gameObject;
            Assert.IsNotNull(rigRoot, "rigRoot couldn't be found");
            // box visual should be disabled per default
            Transform boxVisual = rigRoot.transform.Find("box display");
            Assert.IsNotNull(boxVisual, "box visual couldn't be found");
            Assert.IsFalse(boxVisual.gameObject.activeSelf, "box was active as default - correct behavior is box display being disabled as a default");

            BoxDisplayConfiguration boxConfig = boundsControl.BoxDisplayConfig;
            // set materials and make 1. rig root hasn't been destroyed 2. box hasn't been destroyed 3. box has been activated
            boxConfig.BoxMaterial = testMaterial;
            Assert.IsNotNull(rigRoot, "rigRoot got destroyed while configuring bounds control during runtime");
            Assert.IsNotNull(boxVisual, "box visual was recreated on setting material");
            Assert.IsTrue(boxVisual.gameObject.activeSelf, "box wasn't set active when setting the material");
            // now set grabbed material and make sure we neither destroy rig root nor the box display
            boxConfig.BoxGrabbedMaterial = testMaterialGrabbed;
            Assert.IsNotNull(rigRoot, "rigRoot got destroyed while configuring bounds control during runtime");
            Assert.IsNotNull(boxVisual, "box visual got destroyed when setting grabbed material");
            // make sure color changed on visual
            Assert.AreEqual(boxVisual.GetComponent<Renderer>().material.color, testMaterial.color, "box material wasn't applied to visual");

            // grab one of the scale handles and make sure grabbed material is applied to box
            Transform cornerVisual = rigRoot.transform.Find("corner_3/visuals");
            Assert.IsNotNull(cornerVisual, "couldn't find scale handle visual");
            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.Show(Vector3.zero);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.OpenSteadyGrabPoint);
            yield return hand.MoveTo(cornerVisual.position);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            yield return new WaitForFixedUpdate();
            Assert.AreEqual(boxVisual.GetComponent<Renderer>().material.color, testMaterialGrabbed.color, "box grabbed material wasn't applied to visual");
            // release handle
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.OpenSteadyGrabPoint);

            yield return null;
        }

        /// <summary>
        /// Tests scaling of box display after flattening bounds control during runtime
        /// and making sure neither box display nor rig get recreated.
        /// </summary>
        [UnityTest]
        public IEnumerator BoxDisplayFlattenAxisScaleTest()
        {
            // test flatten mode of rotation handle
            var boundsControl = InstantiateSceneAndDefaultBoundsControl();
            yield return VerifyInitialBoundsCorrect(boundsControl);

            // cache rig root for verifying that we're not recreating the rig on config changes
            GameObject rigRoot = boundsControl.transform.Find("rigRoot").gameObject;
            Assert.IsNotNull(rigRoot, "rigRoot couldn't be found");

            // get box display and activate by setting material
            BoxDisplayConfiguration boxDisplayConfig = boundsControl.BoxDisplayConfig;
            boxDisplayConfig.BoxMaterial = testMaterial;
            boundsControl.FlattenAxis = FlattenModeType.DoNotFlatten;

            Transform boxDisplay = rigRoot.transform.Find("box display");
            Assert.IsNotNull(boxDisplay, "couldn't find box display");
            Assert.IsTrue(boxDisplay.gameObject.activeSelf, "box should be active when material is set");
            Vector3 originalScale = boxDisplay.localScale;

            // flatten x axis and make sure box gets flattened
            boundsControl.FlattenAxis = FlattenModeType.FlattenX;
            Assert.IsNotNull(rigRoot, "rigRoot got destroyed while configuring bounds control during runtime");
            Assert.IsNotNull(boxDisplay, "box display got destroyed while flattening axis");
            Assert.AreEqual(boxDisplay.localScale.x, boxDisplayConfig.FlattenAxisDisplayScale, "Flatten axis scale wasn't applied properly to box display");

            // modify flatten scale
            boxDisplayConfig.FlattenAxisDisplayScale = 5.0f;
            Assert.IsNotNull(rigRoot, "rigRoot got destroyed while configuring bounds control during runtime");
            Assert.IsNotNull(boxDisplay, "box display got destroyed while changing flatten scale");
            Assert.AreEqual(boxDisplay.localScale.x * boundsControl.transform.localScale.x, boxDisplayConfig.FlattenAxisDisplayScale, "Flatten axis scale wasn't applied properly to box display");

            // unflatten the control again and make sure handle gets activated accordingly
            boundsControl.FlattenAxis = FlattenModeType.DoNotFlatten;
            Assert.IsNotNull(rigRoot, "rigRoot got destroyed while configuring bounds control during runtime");
            Assert.IsNotNull(boxDisplay, "box display got destroyed while unflattening control");
            Assert.AreEqual(originalScale, boxDisplay.localScale, "Unflattening axis didn't return original scaling");

            yield return null;
        }

        /// <summary>
        /// Test for verifying that per axis handles are properly switched off/on when flattening/ unflattening the rig.
        /// Makes sure rig and handles are not recreated on changing flattening mode.
        /// </summary>
        [UnityTest]
        public IEnumerator PerAxisHandleFlattenTest([ValueSource("perAxisHandleTestData")] PerAxisHandleTestData testData)
        {
            // test flatten mode of rotation handle
            var boundsControl = InstantiateSceneAndDefaultBoundsControl();
            yield return VerifyInitialBoundsCorrect(boundsControl);
            boundsControl.TranslationHandlesConfig.ShowHandleForX = true; // make sure translation handle test handle is enabled for per axis tests

            // cache rig root for verifying that we're not recreating the rig on config changes
            GameObject rigRoot = boundsControl.transform.Find("rigRoot").gameObject;
            Assert.IsNotNull(rigRoot, "rigRoot couldn't be found");

            // get handle and make sure it's active per default
            Transform handle = rigRoot.transform.Find(testData.handleName + "_0");
            Assert.IsNotNull(handle, "couldn't find rotation handle");
            Assert.IsTrue(handle.gameObject.activeSelf, "handle wasn't enabled by default");

            // flatten x axis and make sure handle gets deactivated
            boundsControl.FlattenAxis = FlattenModeType.FlattenX;
            Assert.IsFalse(handle.gameObject.activeSelf, "handle wasn't disabled when control was flattened in X axis");
            Assert.IsNotNull(rigRoot, "rigRoot got destroyed while configuring bounds control during runtime");

            // unflatten the control again and make sure handle gets activated accordingly
            boundsControl.FlattenAxis = FlattenModeType.DoNotFlatten;
            Assert.IsTrue(handle.gameObject.activeSelf, "handle wasn't enabled on unflatten");
            Assert.IsNotNull(rigRoot, "rigRoot got destroyed while configuring bounds control during runtime");

            yield return null;
        }

        /// <summary>
        /// Test for verifying that per axis handles are properly switched off/on when
        /// FlattenAuto mode is used.
        /// </summary>
        [UnityTest]
        public IEnumerator FlattenAutoTest([ValueSource("perAxisHandleTestData")] PerAxisHandleTestData testData)
        {
            // test flatten mode of per axis handle
            var boundsControl = InstantiateSceneAndDefaultBoundsControl();
            yield return VerifyInitialBoundsCorrect(boundsControl);
            boundsControl.TranslationHandlesConfig.ShowHandleForX = true; // make sure translation handle test handle is enabled for per axis tests

            // Make cube very flat on X axis.
            boundsControl.transform.localScale = new Vector3(0.01f, 1f, 1f);
            boundsControl.HideElementsInInspector = false;

            // cache rig root for verifying that we're not recreating the rig on config changes
            GameObject rigRoot = boundsControl.transform.Find("rigRoot").gameObject;
            Assert.IsNotNull(rigRoot, "rigRoot couldn't be found");

            // get handle and make sure it's active per default
            Transform handle = rigRoot.transform.Find(testData.handleName + "_0");
            Assert.IsNotNull(handle, "couldn't find handle");
            Assert.IsTrue(handle.gameObject.activeSelf, "handle wasn't enabled by default");

            // Set FlattenModeType to FlattenAuto
            boundsControl.FlattenAxis = FlattenModeType.FlattenAuto;

            Assert.IsFalse(handle.gameObject.activeSelf, "handle wasn't disabled when FlattenAuto was used");
            Assert.IsNotNull(rigRoot, "rigRoot got destroyed while configuring bounds control during runtime");

            // unflatten the control again and make sure handle gets activated accordingly
            boundsControl.FlattenAxis = FlattenModeType.DoNotFlatten;
            Assert.IsTrue(handle.gameObject.activeSelf, "handle wasn't enabled on unflatten");
            Assert.IsNotNull(rigRoot, "rigRoot got destroyed while configuring bounds control during runtime");

            yield return null;
        }

        /// <summary>
        /// Test for verifying changing the per axis handle prefabs during runtime 
        /// and making sure the entire rig won't be recreated
        /// </summary>
        [UnityTest]
        public IEnumerator PerAxisHandlePrefabTest([ValueSource("perAxisHandleTestData")] PerAxisHandleTestData testData)
        {
            var boundsControl = InstantiateSceneAndDefaultBoundsControl();
            yield return VerifyInitialBoundsCorrect(boundsControl);
            GameObject childBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var sharedMeshFilter = childBox.GetComponent<MeshFilter>();
            boundsControl.TranslationHandlesConfig.ShowHandleForZ = true; // make sure translation handle test handle is enabled for per axis tests

            // cache rig root for verifying that we're not recreating the rig on config changes
            GameObject rigRoot = boundsControl.transform.Find("rigRoot").gameObject;
            Assert.IsNotNull(rigRoot, "rigRoot couldn't be found");

            // check default mesh filter
            Transform perAxisVisual = rigRoot.transform.Find(testData.handleName + "_2/visuals");
            Transform cornerVisual = rigRoot.transform.Find("corner_3/visuals");
            Assert.IsNotNull(perAxisVisual, "couldn't find axis handle visual");
            Assert.IsNotNull(cornerVisual, "couldn't find scale handle visual");
            var handleVisualMeshFilter = perAxisVisual.GetComponent<MeshFilter>();

            Assert.IsTrue(handleVisualMeshFilter.mesh.name == "Sphere Instance", "Axis handles weren't created with default sphere");

            // change mesh
            System.Reflection.PropertyInfo propName = boundsControl.GetType().GetProperty(testData.configPropertyName);
            PerAxisHandlesConfiguration config = (PerAxisHandlesConfiguration)propName.GetValue(boundsControl);
            config.HandlePrefab = childBox;
            yield return null;
            yield return new WaitForFixedUpdate();

            // make sure only the visuals have been destroyed but not the rig root
            Assert.IsNotNull(rigRoot, "rigRoot got destroyed while configuring bounds control during runtime");
            Assert.IsNotNull(cornerVisual, "scale handle got destroyed while replacing prefab for per axis handle");
            Assert.IsNull(perAxisVisual, "axis handle visual wasn't destroyed when swapping the prefab");

            // fetch new per axis handle visual
            perAxisVisual = rigRoot.transform.Find(testData.handleName + "_2/visuals");
            Assert.IsNotNull(perAxisVisual, "couldn't find handle visual");
            handleVisualMeshFilter = perAxisVisual.GetComponent<MeshFilter>();

            // check if new mesh filter was applied
            Assert.IsTrue(sharedMeshFilter.mesh.name == handleVisualMeshFilter.mesh.name, "box handle wasn't applied");

            yield return null;
        }

        /// <summary>
        /// Test for verifying automatically generated handle colliders are properly aligned to the handle visual
        /// </summary>
        [UnityTest]
        public IEnumerator HandleAlignmentTest([ValueSource("handleTestData")] HandleTestData testData)
        {
            var boundsControl = InstantiateSceneAndDefaultBoundsControl();
            yield return VerifyInitialBoundsCorrect(boundsControl);

            // Create an oblong-shaped handle
            // (cylinder primitive will do, as it is longer than it is wide!)
            // Testing oblong handles will stress the alignment/rotation behavior
            var cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            GameObject.Destroy(cylinder.GetComponent<CapsuleCollider>());

            // Wait for Destroy() to do its thing
            yield return null;

            // Set the handles to be cylinders!
            System.Reflection.PropertyInfo propName = boundsControl.GetType().GetProperty(testData.configPropertyName);
            HandlesBaseConfiguration config = (HandlesBaseConfiguration)propName.GetValue(boundsControl);
            config.HandlePrefab = cylinder;

            // Reflection voodoo to retrieve the ColliderPadding value regardless of which
            // handle configuration subclass we're currently using
            System.Type configType = config.GetType();
            var paddingProperty = configType.GetProperty("ColliderPadding");
            Vector3 padding = (Vector3)paddingProperty.GetValue(config);

            // Wait for BoundsControl to update to new handle prefab/rebuild rig
            yield return null;
            yield return new WaitForFixedUpdate();

            // Iterate over all handle transforms
            foreach(Transform handle in boundsControl.transform.Find("rigRoot"))
            {   
                // Is this the handle type we're currently looking for?
                if(handle.name.StartsWith(testData.handleName))
                {
                    BoxCollider handleCollider = handle.GetComponent<BoxCollider>();

                    Vector3[] colliderPoints = new Vector3[8];
                    Vector3[] globalColliderPoints = new Vector3[8];

                    // Strip the padding off the collider bounds, so that these bounds will match up
                    // correctly with the visual bounds, if the alignment was properly done.
                    VisualUtils.GetCornerPositionsFromBounds(new Bounds(handleCollider.center, handleCollider.size - padding), ref colliderPoints);

                    // Perform a local-global transformation on all corners of the local collider bounds.
                    for(int i = 0; i < colliderPoints.Length; ++i)
                    {
                        globalColliderPoints[i] = handle.TransformPoint(colliderPoints[i]);
                    }

                    Transform visual = handle.GetChild(0);
                    Bounds handleBounds = VisualUtils.GetMaxBounds(visual.gameObject);

                    Vector3[] visualPoints = new Vector3[8];
                    Vector3[] globalVisualPoints = new Vector3[8];

                    VisualUtils.GetCornerPositionsFromBounds(handleBounds, ref visualPoints);

                    // Perform a local-global transformation on all corners of the local visual handle bounds.
                    for(int i = 0; i < visualPoints.Length; ++i)
                    {
                        globalVisualPoints[i] = visual.TransformPoint(visualPoints[i]);
                    }

                    // Make sure all corners/vertices of the bounds are coherent after realignment, in global space
                    bool flag = true;
                    for(int i = 0; i < globalColliderPoints.Length; ++i)
                    {
                        if(globalColliderPoints[i] != globalVisualPoints[i])
                        {
                            flag = false;
                            Debug.LogError($"Bounds mismatch, collider point: {globalColliderPoints[i].ToString("F3")}, visual point: {globalVisualPoints[i].ToString("F3")}");
                        }
                    }

                    Assert.IsTrue(flag, "Handle collider does not match visual bounds, likely incorrect realignment of handle/visual transforms");
                }
            }

            yield return null;
        }

        /// <summary>
        /// Test for verifying changing the handle prefabs during runtime 
        /// in regular and flatten mode and making sure the entire rig won't be recreated
        /// </summary>
        [UnityTest]
        public IEnumerator ScaleHandlePrefabTest()
        {
            var boundsControl = InstantiateSceneAndDefaultBoundsControl();
            yield return VerifyInitialBoundsCorrect(boundsControl);
            GameObject childSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            var sharedMeshFilter = childSphere.GetComponent<MeshFilter>();
            // cache rig root for verifying that we're not recreating the rig on config changes
            GameObject rigRoot = boundsControl.transform.Find("rigRoot").gameObject;
            Assert.IsNotNull(rigRoot, "rigRoot couldn't be found");

            // check default mesh filter
            Transform cornerVisual = rigRoot.transform.Find("corner_3/visuals");
            Assert.IsNotNull(cornerVisual, "couldn't find corner visual");
            Transform rotationHandleVisual = rigRoot.transform.Find("midpoint_2/visuals");
            Assert.IsNotNull(rotationHandleVisual, "couldn't find rotation handle visual");
            var cornerMeshFilter = cornerVisual.GetComponent<MeshFilter>();

            Assert.IsTrue(cornerMeshFilter.mesh.name == "Cube Instance", "Scale handles weren't created with default cube");

            // change mesh
            ScaleHandlesConfiguration scaleHandleConfig = boundsControl.ScaleHandlesConfig;
            scaleHandleConfig.HandlePrefab = childSphere;
            yield return null;
            yield return new WaitForFixedUpdate();

            // make sure only the visuals have been destroyed but not the rig root
            Assert.IsNotNull(rigRoot, "rigRoot got destroyed while configuring bounds control during runtime");
            Assert.IsNotNull(rotationHandleVisual, "rotation handle visual got destroyed while replacing the scale handle");
            Assert.IsNull(cornerVisual, "corner visual wasn't destroyed when swapping the prefab");

            // fetch new corner visual
            cornerVisual = rigRoot.transform.Find("corner_3/visuals");
            Assert.IsNotNull(cornerVisual, "couldn't find corner visual");
            cornerMeshFilter = cornerVisual.GetComponent<MeshFilter>();
            // check if new mesh filter was applied
            Assert.IsTrue(sharedMeshFilter.mesh.name == cornerMeshFilter.mesh.name, "sphere scale handle wasn't applied");

            // set flatten mode
            boundsControl.FlattenAxis = FlattenModeType.FlattenX;
            yield return null;
            yield return new WaitForFixedUpdate();

            // make sure only the visuals have been destroyed but not the rig root
            Assert.IsNotNull(rigRoot, "rigRoot got destroyed while configuring bounds control during runtime");
            Assert.IsNull(cornerVisual, "corner visual wasn't destroyed when swapping the prefab");

            // mesh should be cube again
            cornerVisual = rigRoot.transform.Find("corner_3/visuals");
            Assert.IsNotNull(cornerVisual, "couldn't find corner visual");
            cornerMeshFilter = cornerVisual.GetComponent<MeshFilter>();
            Assert.IsTrue(cornerMeshFilter.mesh.name == "Cube Instance", "Flattened scale handles weren't created with default cube");
            // reset flatten mode
            boundsControl.FlattenAxis = FlattenModeType.DoNotFlatten;

            scaleHandleConfig.HandleSlatePrefab = childSphere;
            yield return null;
            yield return new WaitForFixedUpdate();

            // make sure only the visuals have been destroyed but not the rig root
            Assert.IsNotNull(rigRoot, "rigRoot got destroyed while configuring bounds control during runtime");
            Assert.IsNull(cornerVisual, "corner visual wasn't destroyed when swapping the prefab");

            // fetch new corner visual
            cornerVisual = rigRoot.transform.Find("corner_3/visuals");
            Assert.IsNotNull(cornerVisual, "couldn't find corner visual");
            cornerMeshFilter = cornerVisual.GetComponent<MeshFilter>();

            // check if new mesh filter was applied
            Assert.IsTrue(cornerMeshFilter.mesh.name.StartsWith(sharedMeshFilter.mesh.name), "sphere scale handle wasn't applied");
            yield return null;
        }

        /// <summary>
        /// Tests runtime configuration of handle materials.
        /// Verifies handle default and grabbed material are properly replaced in all visuals when 
        /// setting the material in the config as well as validating that neither the rig nor the visuals get recreated.
        /// </summary>
        [UnityTest]
        public IEnumerator HandleMaterialTest([ValueSource("handleTestData")] HandleTestData testData)
        {
            var boundsControl = InstantiateSceneAndDefaultBoundsControl();
            yield return VerifyInitialBoundsCorrect(boundsControl);
            boundsControl.TranslationHandlesConfig.ShowHandleForZ = true; // make sure translation handle test handle is enabled for per axis tests

            // fetch rigroot, corner visual and rotation handle config
            GameObject rigRoot = boundsControl.transform.Find("rigRoot").gameObject;
            Assert.IsNotNull(rigRoot, "rigRoot couldn't be found");
            Transform cornerVisual = rigRoot.transform.Find(testData.handleVisualPath);
            Assert.IsNotNull(cornerVisual, "couldn't find corner visual");

            System.Reflection.PropertyInfo propName = boundsControl.GetType().GetProperty(testData.configPropertyName);
            HandlesBaseConfiguration handleConfig = (HandlesBaseConfiguration)propName.GetValue(boundsControl);
            // set materials and make sure rig root and visuals haven't been destroyed while doing so
            handleConfig.HandleMaterial = testMaterial;
            Assert.IsNotNull(rigRoot, "rigRoot got destroyed while configuring bounds control during runtime");
            handleConfig.HandleGrabbedMaterial = testMaterialGrabbed;
            Assert.IsNotNull(rigRoot, "rigRoot got destroyed while configuring bounds control during runtime");
            Assert.IsNotNull(cornerVisual, "corner visual got destroyed when setting material");
            // make sure color changed on visual
            Assert.AreEqual(cornerVisual.GetComponent<Renderer>().material.color, testMaterial.color, "handle material wasn't applied to visual");

            // grab handle and make sure grabbed material is applied
            var frontRightCornerPos = cornerVisual.position;
            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.Show(Vector3.zero);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.OpenSteadyGrabPoint);
            yield return hand.MoveTo(frontRightCornerPos);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            yield return new WaitForFixedUpdate();
            Assert.AreEqual(cornerVisual.GetComponent<Renderer>().material.color, testMaterialGrabbed.color, "handle grabbed material wasn't applied to visual");
            // release handle
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.OpenSteadyGrabPoint);
        }

        /// <summary>
        /// Tests runtime configuration of handle size.
        /// Verifies handles are scaled according to new size value without recreating the visual or the rig
        /// </summary>
        [UnityTest]
        public IEnumerator HandleSizeTest([ValueSource("handleTestData")] HandleTestData testData)
        {
            var boundsControl = InstantiateSceneAndDefaultBoundsControl();
            yield return VerifyInitialBoundsCorrect(boundsControl);
            boundsControl.TranslationHandlesConfig.ShowHandleForZ = true;

            // fetch rigroot, corner visual and rotation handle config
            GameObject rigRoot = boundsControl.transform.Find("rigRoot").gameObject;
            Assert.IsNotNull(rigRoot, "rigRoot couldn't be found");
            Transform handleVisual = rigRoot.transform.Find(testData.handleVisualPath);
            Assert.IsNotNull(handleVisual, "couldn't find visual " + testData.handleVisualPath);

            // test hand setup
            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.Show(Vector3.zero);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.OpenSteadyGrabPoint);

            // set test materials so we know if we're interacting with the handle later in the test
            System.Reflection.PropertyInfo propName = boundsControl.GetType().GetProperty(testData.configPropertyName);
            HandlesBaseConfiguration handleConfig = (HandlesBaseConfiguration)propName.GetValue(boundsControl);
            handleConfig.HandleMaterial = testMaterial;
            handleConfig.HandleGrabbedMaterial = testMaterialGrabbed;

            // test runtime handle size configuration
            handleConfig.HandleSize = 0.1f;
            Assert.IsNotNull(rigRoot, "rigRoot got destroyed while configuring bounds control during runtime");
            Assert.IsNotNull(handleVisual, "visual got destroyed when setting material");
            yield return hand.MoveTo(handleVisual.position + new Vector3(1.0f, 0.0f, 0.0f) * handleConfig.HandleSize * 0.5f);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            Assert.AreEqual(handleVisual.GetComponent<Renderer>().material.color, testMaterialGrabbed.color, "handle wasn't grabbed");
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.OpenSteadyGrabPoint);
        }

        /// <summary>
        /// Tests runtime configuration of handle collider padding.
        /// Verifies collider of handles are scaled according to new size value 
        /// without recreating the visual or the rig
        /// </summary>
        [UnityTest]
        public IEnumerator HandleColliderPaddingTest([ValueSource("handleTestData")] HandleTestData testData)
        {
            var boundsControl = InstantiateSceneAndDefaultBoundsControl();
            yield return VerifyInitialBoundsCorrect(boundsControl);
            boundsControl.TranslationHandlesConfig.ShowHandleForZ = true;

            // fetch rigroot, corner visual and rotation handle config
            GameObject rigRoot = boundsControl.transform.Find("rigRoot").gameObject;
            Assert.IsNotNull(rigRoot, "rigRoot couldn't be found");
            Transform cornerVisual = rigRoot.transform.Find(testData.handleVisualPath);
            Assert.IsNotNull(cornerVisual, "couldn't find visual" + testData.handleVisualPath);
            // init test hand
            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.Show(Vector3.zero);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.OpenSteadyGrabPoint);
            // set test materials so we know if we're interacting with the handle later in the test
            System.Reflection.PropertyInfo propName = boundsControl.GetType().GetProperty(testData.configPropertyName);
            HandlesBaseConfiguration handleConfig = (HandlesBaseConfiguration)propName.GetValue(boundsControl);
            handleConfig.HandleMaterial = testMaterial;
            handleConfig.HandleGrabbedMaterial = testMaterialGrabbed;
            yield return new WaitForFixedUpdate();

            // move hand to edge of rotation handle collider
            Transform cornerHandle = rigRoot.transform.Find(testData.handleName);
            var cornerCollider = cornerHandle.GetComponent<BoxCollider>();
            Vector3 originalColliderExtents = cornerCollider.bounds.extents;

            yield return hand.MoveTo(cornerHandle.position + originalColliderExtents);
            // test runtime collider padding configuration
            Vector3 newColliderPadding = handleConfig.ColliderPadding * 5.0f;

            // move hand to new collider bounds edge before setting the new value in the config
            yield return hand.MoveTo(cornerHandle.position + originalColliderExtents + (newColliderPadding * 0.5f));
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            // handle shouldn't be in grabbed state
            Assert.AreEqual(cornerVisual.GetComponent<Renderer>().material.color, testMaterial.color, "handle was grabbed outside collider padding area");

            yield return hand.SetGesture(ArticulatedHandPose.GestureId.OpenSteadyGrabPoint);
            // now adjust collider bounds and try grabbing the handle again
            handleConfig.ColliderPadding += newColliderPadding;
            yield return new WaitForFixedUpdate();
            Assert.IsNotNull(rigRoot, "rigRoot got destroyed while configuring bounds control during runtime");
            Assert.IsNotNull(cornerVisual, "corner visual got destroyed when setting material");
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            // handle should be grabbed now
            Assert.AreEqual(cornerVisual.GetComponent<Renderer>().material.color, testMaterialGrabbed.color, "handle wasn't grabbed");
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.OpenSteadyGrabPoint);
        }

        /// <summary>
        /// Test starting and ending manipulating an object via the app bar
        /// </summary>
        [UnityTest]
        public IEnumerator ManipulateViaAppBarFarInteraction()
        {
            // create cube with bounds control and app bar
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = boundsControlStartCenter;
            BoundsControl boundsControl = cube.AddComponent<BoundsControl>();

            TestUtilities.PlayspaceToOriginLookingForward();

            boundsControl.transform.localScale = boundsControlStartScale;
            Object appBarPrefab = AssetDatabase.LoadAssetAtPath(appBarPrefabLink, typeof(Object));
            Assert.IsNotNull(appBarPrefab, "Couldn't load app bar prefab from assetdatabase");
            GameObject appBarGameObject = Object.Instantiate(appBarPrefab) as GameObject;
            Assert.IsNotNull(appBarGameObject, "Couldn't instantiate appbar prefab");
            appBarGameObject.SetActive(false);
            AppBar appBar = appBarGameObject.GetComponent<AppBar>();
            Assert.IsNotNull(appBar, "Couldn't find AppBar component in prefab");

            appBarGameObject.transform.localScale = Vector3.one * 5.0f;
            appBar.Target = boundsControl;
            appBarGameObject.SetActive(true);

            // manipulation coords
            Vector3 rightCornerInteractionPoint = new Vector3(0.184f, 0.078f, 0.79f); // position of hand for far interacting with front right corner 
            Vector3 pointOnCube = new Vector3(-0.033f, -0.129f, 0.499f); // position where hand ray points on center of the test cube
            Vector3 scalePoint = new Vector3(0.165f, 0.267f, 0.794f); // end position for far interaction scaling
            Vector3 appBarButtonStart = new Vector3(-0.028f, -0.263f, 0.499f); // location of hand for interaction with the app bar manipulation button after scene setup
            Vector3 appBarButtonAfterScale = new Vector3(0.009f, -0.255f, 0.499f); // location of the hand for interaction with the app bar manipulation button after scaling

            // first test to interact with the cube without activating the app bar
            // this shouldn't scale the cube
            TestHand hand = new TestHand(Handedness.Left);
            yield return hand.Show(pointOnCube); // Initially make sure that hand ray is pointed on cube surface so we won't go behind the cube with our ray
            yield return hand.MoveTo(rightCornerInteractionPoint);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            yield return hand.MoveTo(scalePoint);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Open);
            var endBounds = boundsControl.GetComponent<BoxCollider>().bounds;
            TestUtilities.AssertAboutEqual(endBounds.center, boundsControlStartCenter, "endBounds incorrect center");
            TestUtilities.AssertAboutEqual(endBounds.size, boundsControlStartScale, "endBounds incorrect size");

            // now activate the bounds control via app bar
            yield return hand.MoveTo(appBarButtonStart);
            yield return hand.Click();

            // check if we can scale the box now
            yield return hand.MoveTo(pointOnCube); // make sure our hand ray is on the cube again before moving to the scale corner
            yield return hand.MoveTo(rightCornerInteractionPoint); // move to scale corner
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            yield return hand.MoveTo(scalePoint);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Open);
            endBounds = boundsControl.GetComponent<BoxCollider>().bounds;
            Vector3 expectedScaleCenter = new Vector3(0.0453f, 0.0453f, 1.455f);
            Vector3 expectedScaleSize = Vector3.one * 0.59f;
            TestUtilities.AssertAboutEqual(endBounds.center, expectedScaleCenter, "endBounds incorrect center");
            TestUtilities.AssertAboutEqual(endBounds.size, expectedScaleSize, "endBounds incorrect size");

            // deactivate the bounds control via app bar
            yield return hand.MoveTo(appBarButtonAfterScale);
            yield return hand.Click();

            // check if we can scale the box - box shouldn't scale
            Vector3 startLocationScaleToOriginal = new Vector3(0.181f, 0.013f, 0.499f);
            Vector3 endLocationScaleToOriginal = new Vector3(0.121f, -0.052f, 0.499f);
            yield return hand.MoveTo(pointOnCube); // make sure our hand ray is on the cube again before moving to the scale corner
            yield return hand.MoveTo(startLocationScaleToOriginal); // move to scale corner
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            yield return hand.MoveTo(endLocationScaleToOriginal);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Open);
            endBounds = boundsControl.GetComponent<BoxCollider>().bounds;
            TestUtilities.AssertAboutEqual(endBounds.center, expectedScaleCenter, "endBounds incorrect center");
            TestUtilities.AssertAboutEqual(endBounds.size, expectedScaleSize, "endBounds incorrect size");

            // activate the bounds control via app bar
            yield return hand.MoveTo(appBarButtonAfterScale);
            yield return hand.Click();

            // try again to scale the box back
            yield return hand.MoveTo(pointOnCube); // make sure our hand ray is on the cube again before moving to the scale corner
            yield return hand.MoveTo(startLocationScaleToOriginal); // move to scale corner
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            yield return hand.MoveTo(endLocationScaleToOriginal);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Open);
            endBounds = boundsControl.GetComponent<BoxCollider>().bounds;
            TestUtilities.AssertAboutEqual(endBounds.center, boundsControlStartCenter, "endBounds incorrect center");
            TestUtilities.AssertAboutEqual(endBounds.size, boundsControlStartScale, "endBounds incorrect size");

            yield return null;
        }

        /// <summary>
        /// Test starting and ending manipulating an object via the app bar
        /// </summary>
        [UnityTest]
        public IEnumerator ManipulateViaAppBarMotionController()
        {
            // Switch to motion controller
            var iss = PlayModeTestUtilities.GetInputSimulationService();
            var oldSimMode = iss.ControllerSimulationMode;
            iss.ControllerSimulationMode = ControllerSimulationMode.MotionController;

            // create cube with bounds control and app bar
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = boundsControlStartCenter;
            BoundsControl boundsControl = cube.AddComponent<BoundsControl>();

            TestUtilities.PlayspaceToOriginLookingForward();

            boundsControl.transform.localScale = boundsControlStartScale;
            Object appBarPrefab = AssetDatabase.LoadAssetAtPath(appBarPrefabLink, typeof(Object));
            Assert.IsNotNull(appBarPrefab, "Couldn't load app bar prefab from assetdatabase");
            GameObject appBarGameObject = Object.Instantiate(appBarPrefab) as GameObject;
            Assert.IsNotNull(appBarGameObject, "Couldn't instantiate appbar prefab");
            appBarGameObject.SetActive(false);
            AppBar appBar = appBarGameObject.GetComponent<AppBar>();
            Assert.IsNotNull(appBar, "Couldn't find AppBar component in prefab");

            appBarGameObject.transform.localScale = Vector3.one * 5.0f;
            appBar.Target = boundsControl;
            appBarGameObject.SetActive(true);

            // manipulation coords
            Vector3 rightCornerInteractionPoint = new Vector3(0.1659f, 0.123f, 0.79f); // position of motion controller for far interacting with front right corner 
            Vector3 pointOnCube = new Vector3(-0.033f, -0.129f, 0.499f); // position where ray points on the test cube
            Vector3 scalePoint = new Vector3(0.165f, 0.267f, 0.794f); // end position for motion controller scaling
            Vector3 appBarButtonStart = new Vector3(-0.12f, -0.435f, 0.499f); // location of motion controller for interaction with the app bar manipulation button after scene setup
            Vector3 appBarButtonAfterScale = new Vector3(-0.075f, -0.414f, 0.499f); // location of the motion controller for interaction with the app bar manipulation button after scaling

            // first test to interact with the cube without activating the app bar
            // this shouldn't scale the cube
            TestMotionController motionController = new TestMotionController(Handedness.Left);
            yield return motionController.Show(pointOnCube); // Initially make sure that ray is pointed on cube surface so we won't go behind the cube with our ray
            yield return motionController.MoveTo(rightCornerInteractionPoint);
            SimulatedMotionControllerButtonState selectButtonState = new SimulatedMotionControllerButtonState()
            {
                IsSelecting = true
            };
            yield return motionController.SetState(selectButtonState);
            yield return motionController.MoveTo(scalePoint);
            yield return motionController.SetState(new SimulatedMotionControllerButtonState());
            var endBounds = boundsControl.GetComponent<BoxCollider>().bounds;
            TestUtilities.AssertAboutEqual(endBounds.center, boundsControlStartCenter, "endBounds incorrect center");
            TestUtilities.AssertAboutEqual(endBounds.size, boundsControlStartScale, "endBounds incorrect size");

            // now activate the bounds control via app bar
            yield return motionController.MoveTo(appBarButtonStart);
            yield return motionController.Click();

            // check if we can scale the box now
            yield return motionController.MoveTo(pointOnCube); // make sure our ray is on the cube again before moving to the scale corner
            yield return motionController.MoveTo(rightCornerInteractionPoint); // move to scale corner
            yield return motionController.SetState(selectButtonState);
            yield return motionController.MoveTo(scalePoint);
            yield return motionController.SetState(new SimulatedMotionControllerButtonState());
            endBounds = boundsControl.GetComponent<BoxCollider>().bounds;
            Vector3 expectedScaleCenter = new Vector3(0.023f, 0.023f, 1.477f);
            Vector3 expectedScaleSize = Vector3.one * 0.546f;
            TestUtilities.AssertAboutEqual(endBounds.center, expectedScaleCenter, "endBounds incorrect center");
            TestUtilities.AssertAboutEqual(endBounds.size, expectedScaleSize, "endBounds incorrect size");

            // deactivate the bounds control via app bar
            yield return motionController.MoveTo(appBarButtonAfterScale);
            yield return motionController.Click();

            // check if we can scale the box - box shouldn't scale
            Vector3 startScaleLocation = new Vector3(0.173f, 0.104f, 0.499f);
            Vector3 endScaleLocation = new Vector3(0.406f, 0.271f, 0.499f);
            yield return motionController.MoveTo(pointOnCube); // make sure our ray is on the cube again before moving to the scale corner
            yield return motionController.MoveTo(startScaleLocation); // move to scale corner
            yield return motionController.SetState(selectButtonState);
            yield return motionController.MoveTo(endScaleLocation);
            yield return motionController.SetState(new SimulatedMotionControllerButtonState());
            endBounds = boundsControl.GetComponent<BoxCollider>().bounds;
            TestUtilities.AssertAboutEqual(endBounds.center, expectedScaleCenter, "endBounds incorrect center");
            TestUtilities.AssertAboutEqual(endBounds.size, expectedScaleSize, "endBounds incorrect size");

            // activate the bounds control via app bar
            yield return motionController.MoveTo(appBarButtonAfterScale);
            yield return motionController.Click();

            // try again to scale the box back
            yield return motionController.MoveTo(pointOnCube); // make sure our ray is on the cube again before moving to the scale corner
            yield return motionController.MoveTo(startScaleLocation); // move to scale corner
            yield return motionController.SetState(selectButtonState);
            yield return motionController.MoveTo(endScaleLocation);
            yield return motionController.SetState(new SimulatedMotionControllerButtonState());
            endBounds = boundsControl.GetComponent<BoxCollider>().bounds;
            TestUtilities.AssertAboutEqual(endBounds.center, new Vector3(0.089f, 0.089f, 1.411f), "endBounds incorrect center");
            TestUtilities.AssertAboutEqual(endBounds.size, Vector3.one * 0.679f, "endBounds incorrect size");

            yield return null;

            // Restore the input simulation profile
            iss.ControllerSimulationMode = oldSimMode;

            yield return null;
        }

        /// <summary>
        /// Test creating an new instance of a scriptable configuration and setting it.
        /// </summary>
        [UnityTest]
        public IEnumerator SetVisualConfiguration()
        {
            BoundsControl boundsControl = InstantiateSceneAndDefaultBoundsControl();

            // Make sure the material on the object has not been applied 
            Assert.AreNotEqual(GetBoxVisual(boundsControl).GetComponent<Renderer>().material.color, testMaterial.color);

            // Create new scriptable
            BoxDisplayConfiguration boxDisplayConfiguration = ScriptableObject.CreateInstance<BoxDisplayConfiguration>();
            yield return null;

            // Set the material property of the new scriptable
            boxDisplayConfiguration.BoxMaterial = testMaterial;
            yield return null;

            // Set new scriptable
            boundsControl.BoxDisplayConfig = boxDisplayConfiguration;
            yield return null;

            // Make sure the new scriptable visuals have been applied to the object
            Assert.AreEqual(GetBoxVisual(boundsControl).GetComponent<Renderer>().material.color, testMaterial.color);
        }

        // Returns the "box display" transform in the bounds control rig
        private Transform GetBoxVisual(BoundsControl boundsControl)
        {
            GameObject rigRoot = boundsControl.transform.Find("rigRoot").gameObject;
            Assert.IsNotNull(rigRoot, "rigRoot couldn't be found");

            Transform boxVisual = rigRoot.transform.Find("box display");
            Assert.IsNotNull(boxVisual, "box visual couldn't be found");

            return boxVisual;
        }

        /// <summary>
        /// Returns the AABB of the bounds control rig (corners, edges)
        /// that make up the bounds control by using the positions of the corners
        /// </summary>
        private Bounds GetBoundsControlRigBounds(BoundsControl boundsControl)
        {
            Bounds b = new Bounds();
            b.center = boundsControl.transform.Find("rigRoot/corner_0").position;
            for (int i = 1; i < 8; ++i)
            {
                Transform corner = boundsControl.transform.Find("rigRoot/corner_" + i.ToString());
                b.Encapsulate(corner.position);
            }
            return b;
        }
    }
}
#endif
