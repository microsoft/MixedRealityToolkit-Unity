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

using Assert = UnityEngine.Assertions.Assert;
using Microsoft.MixedReality.Toolkit.Experimental.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests.Experimental
{
    /// <summary>
    /// Tests for runtime behavior of bounds control
    /// </summary>
    public class BoundsControlTests
    {
        #region Utilities

        [SetUp]
        public void Setup()
        {
            PlayModeTestUtilities.Setup();
        }

        [TearDown]
        public void ShutdownMrtk()
        {
            PlayModeTestUtilities.TearDown();
        }

        private readonly Vector3 boundsControlStartCenter = Vector3.forward * 1.5f;
        private readonly Vector3 boundsControlStartScale = Vector3.one * 0.5f;

        // SDK/Features/UX/Prefabs/AppBar/AppBar.prefab
        private const string appBarPrefabGuid = "83c02591e2867124181bcd3bcb65e288";
        private static readonly string appBarPrefabLink = AssetDatabase.GUIDToAssetPath(appBarPrefabGuid);

        /// <summary>
        /// Instantiates a bounds control at boundsControlStartCenter
        /// transform is at scale boundsControlStartScale
        /// </summary>
        private BoundsControl InstantiateSceneAndDefaultBoundsControl()
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = boundsControlStartCenter;
            BoundsControl boundsControl = cube.AddComponent<BoundsControl>();

            TestUtilities.PlayspaceToOriginLookingForward();

            boundsControl.transform.localScale = boundsControlStartScale;
            boundsControl.Active = true;

            return boundsControl;
        }

        /// <summary>
        /// Tests if the initial transform setup of bounds control has been propagated to it's collider
        /// </summary>
        /// <param name="boundsControl">Bounds control that controls the collider size</param>
        private IEnumerator VerifyInitialBoundsCorrect(BoundsControl boundsControl)
        {
            yield return null;
            yield return new WaitForFixedUpdate();
            BoxCollider boxCollider = boundsControl.GetComponent<BoxCollider>();
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
        public IEnumerator BBoxInstantiate()
        {
            BoundsControl boundsControl = InstantiateSceneAndDefaultBoundsControl();
            yield return VerifyInitialBoundsCorrect(boundsControl);
            Assert.IsNotNull(boundsControl);

            GameObject.Destroy(boundsControl.gameObject);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        /// <summary>
        /// Test that if we update the bounds of a box collider, that the corners will move correctly
        /// </summary>
        [UnityTest]
        public IEnumerator BBoxOverride()
        {
            BoundsControl boundsControl = InstantiateSceneAndDefaultBoundsControl();
            yield return VerifyInitialBoundsCorrect(boundsControl);
            boundsControl.BoundsControlActivation = Toolkit.Experimental.UI.BoundsControlTypes.BoundsControlActivationType.ActivateOnStart;
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
        /// Uses near interaction to scale the bounds control by directly grabbing corner
        /// </summary>
        [UnityTest]
        public IEnumerator ScaleViaNearInteration()
        {
            BoundsControl boundsControl = InstantiateSceneAndDefaultBoundsControl();
            yield return VerifyInitialBoundsCorrect(boundsControl);
            var inputSimulationService = PlayModeTestUtilities.GetInputSimulationService();

            // front right corner is corner 3
            var frontRightCornerPos = boundsControl.gameObject.transform.Find("rigRoot/corner_3").position;


            Vector3 initialHandPosition = new Vector3(0, 0, 0.5f);
            // This particular test is sensitive to the number of test frames and is run at a slower pace.
            int numSteps = 30;
            var delta = new Vector3(0.1f, 0.1f, 0f);
            yield return PlayModeTestUtilities.ShowHand(Handedness.Right, inputSimulationService, ArticulatedHandPose.GestureId.OpenSteadyGrabPoint, initialHandPosition);
            yield return PlayModeTestUtilities.MoveHand(initialHandPosition, frontRightCornerPos, ArticulatedHandPose.GestureId.OpenSteadyGrabPoint, Handedness.Right, inputSimulationService, numSteps);
            yield return PlayModeTestUtilities.MoveHand(frontRightCornerPos, frontRightCornerPos + delta, ArticulatedHandPose.GestureId.Pinch, Handedness.Right, inputSimulationService, numSteps);

            var endBounds = boundsControl.GetComponent<BoxCollider>().bounds;
            Vector3 expectedCenter = new Vector3(0.033f, 0.033f, 1.467f);
            Vector3 expectedSize = Vector3.one * .567f;
            TestUtilities.AssertAboutEqual(endBounds.center, expectedCenter, "endBounds incorrect center");
            TestUtilities.AssertAboutEqual(endBounds.size, expectedSize, "endBounds incorrect size");

            GameObject.Destroy(boundsControl.gameObject);
            // Wait for a frame to give Unity a change to actually destroy the object
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
            yield return hand.Show(pointOnCube); //initially make sure that hand ray is pointed on cube surface so we won't go behind the cube with our ray
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
            float expectedAngle = 87f;
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
            float expectedAngle = 92f;
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
        /// Test bounds control rotation via HoloLens 1 interaction / GGV
        /// Verifies gameobject has rotation in one axis only applied and no other transform changes happen during interaction
        /// </summary>
        [UnityTest]
        public IEnumerator RotateViaHololens1Interaction()
        {
            BoundsControl control = InstantiateSceneAndDefaultBoundsControl();
            yield return VerifyInitialBoundsCorrect(control);
            PlayModeTestUtilities.PushHandSimulationProfile();
            PlayModeTestUtilities.SetHandSimulationMode(HandSimulationMode.Gestures);

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
            float expectedAngle = 86f;
            float angleDiff = Mathf.Abs(expectedAngle - angle);
            Vector3 expectedAxis = new Vector3(0f, 1f, 0f);
            TestUtilities.AssertAboutEqual(axis, expectedAxis, "Rotated around wrong axis");
            Assert.IsTrue(angleDiff <= 1f, "cube didn't rotate as expected");
            TestUtilities.AssertAboutEqual(control.transform.position, expectedPosition, "cube moved while rotating");
            TestUtilities.AssertAboutEqual(control.transform.localScale, expectedSize, "cube scaled while rotating");

            GameObject.Destroy(control.gameObject);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;

            // Restore the input simulation profile
            PlayModeTestUtilities.PopHandSimulationProfile();

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
            yield return hand.Show(pointOnCube); //initially make sure that hand ray is pointed on cube surface so we won't go behind the cube with our ray
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
            boundsControl.RegisterTransformScaleHandler(scaleHandler);

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
        /// Uses far interaction (HoloLens 1 style) to scale the bounds control
        /// </summary>
        [UnityTest]
        public IEnumerator ScaleViaHoloLens1Interaction()
        {
            BoundsControl boundsControl = InstantiateSceneAndDefaultBoundsControl();
            yield return VerifyInitialBoundsCorrect(boundsControl);
            BoxCollider boxCollider = boundsControl.GetComponent<BoxCollider>();
            PlayModeTestUtilities.PushHandSimulationProfile();
            PlayModeTestUtilities.SetHandSimulationMode(HandSimulationMode.Gestures);

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
            Vector3 expectedCenter = new Vector3(0.033f, 0.033f, 1.467f);
            Vector3 expectedSize = Vector3.one * .567f;
            TestUtilities.AssertAboutEqual(endBounds.center, expectedCenter, "endBounds incorrect center");
            TestUtilities.AssertAboutEqual(endBounds.size, expectedSize, "endBounds incorrect size", 0.02f);

            GameObject.Destroy(boundsControl.gameObject);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;

            // Restore the input simulation profile
            PlayModeTestUtilities.PopHandSimulationProfile();

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
            ScaleHandlesConfiguration scaleHandleConfig = boundsControl.ScaleHandlesConfiguration;
            Vector3 defaultHandleSize = Vector3.one * scaleHandleConfig.HandleSize;

            Vector3 initialHandPosition = new Vector3(0, 0, 0f);
            // this is specific to scale handles
            Transform scaleHandle = boundsControl.gameObject.transform.Find("rigRoot/corner_3");
            Transform proximityScaledVisual = scaleHandle.GetChild(0)?.GetChild(0);
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
            ProximityEffectConfiguration proximityConfig = boundsControl.HandleProximityEffectConfiguration;
            proximityConfig.ProximityEffectActive = true;
            proximityConfig.CloseGrowRate = 1.0f;
            proximityConfig.MediumGrowRate = 1.0f;
            proximityConfig.FarGrowRate = 1.0f;
            boundsControl.CreateRig();
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

            boundsControl.CreateRig();
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
            ScaleHandlesConfiguration scaleHandleConfig = boundsControl.ScaleHandlesConfiguration;
            Vector3 defaultHandleSize = Vector3.one * scaleHandleConfig.HandleSize;
            Transform scaleHandle = boundsControl.gameObject.transform.Find("rigRoot/corner_3");
            Transform proximityScaledVisual = scaleHandle.GetChild(0)?.GetChild(0);
            var frontRightCornerPos = scaleHandle.position;
            // check far scale applied
            ProximityEffectConfiguration proximityConfig = boundsControl.HandleProximityEffectConfiguration;
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

            // set target to cube
            boundsControl.Target = cube;
            boundsControl.Active = true;

            // front right corner is corner 3
            var frontRightCornerPos = cube.transform.Find("rigRoot/corner_3").position;

            // grab corner and scale object
            Vector3 initialHandPosition = new Vector3(0, 0, 0.5f);
            int numSteps = 30;
            var delta = new Vector3(0.1f, 0.1f, 0f);
            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.Show(initialHandPosition);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.OpenSteadyGrabPoint);
            yield return hand.MoveTo(frontRightCornerPos, numSteps);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            yield return hand.MoveTo(frontRightCornerPos + delta, numSteps);

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
            yield return hand.Show(pointOnCube); //initially make sure that hand ray is pointed on cube surface so we won't go behind the cube with our ray
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