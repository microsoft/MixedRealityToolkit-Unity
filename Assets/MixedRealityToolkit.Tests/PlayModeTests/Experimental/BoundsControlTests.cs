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

using Microsoft.MixedReality.Toolkit.UI;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Assert = UnityEngine.Assertions.Assert;
using Microsoft.MixedReality.Toolkit.Experimental.UI.BoundsControl;

namespace Microsoft.MixedReality.Toolkit.Tests.Experimental
{
    /// <summary>
    /// TODO: This test still needs to be adjusted 
    /// Currently it's just a copy of whatever was tested in Bounding Box
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
        /// <summary>
        /// Instantiates a bounds control at boundsControlStartCenter
        /// box is at scale boundsControlStartScale
        /// </summary>
        private BoundsControl InstantiateSceneAndDefaultBbox()
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = boundsControlStartCenter;
            BoundsControl bbox = cube.AddComponent<BoundsControl>();

            MixedRealityPlayspace.PerformTransformation(
            p =>
            {
                p.position = Vector3.zero;
                p.LookAt(boundsControlStartCenter);
            });

            bbox.transform.localScale = boundsControlStartScale;
            bbox.Active = true;

            return bbox;
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
            BoundsControl bbox = InstantiateSceneAndDefaultBbox();
            yield return VerifyInitialBoundsCorrect(bbox);
            Assert.IsNotNull(bbox);

            GameObject.Destroy(bbox.gameObject);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        /// <summary>
        /// Test that if we update the bounds of a box collider, that the corners will move correctly
        /// </summary>
        [UnityTest]
        public IEnumerator BBoxOverride()
        {
            BoundsControl bbox = InstantiateSceneAndDefaultBbox();
            yield return VerifyInitialBoundsCorrect(bbox);
            bbox.BoundsControlActivation = Toolkit.Experimental.UI.BoundsControlTypes.BoundsControlActivationType.ActivateOnStart;
            bbox.HideElementsInInspector = false;
            yield return null;

            var newObject = new GameObject();
            var bc = newObject.AddComponent<BoxCollider>();
            bc.center = new Vector3(.25f, 0, 0);
            bc.size = new Vector3(0.162f, 0.1f, 1);
            bbox.BoundsOverride = bc;
            yield return null;

            Bounds b = GetBoundsControlRigBounds(bbox);

            Debug.Assert(b.center == bc.center, $"bounds center should be {bc.center} but they are {b.center}");
            Debug.Assert(b.size == bc.size, $"bounds size should be {bc.size} but they are {b.size}");

            GameObject.Destroy(bbox.gameObject);
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
            BoundsControl bbox = InstantiateSceneAndDefaultBbox();
            yield return VerifyInitialBoundsCorrect(bbox);
            var inputSimulationService = PlayModeTestUtilities.GetInputSimulationService();

            // front right corner is corner 3
            var frontRightCornerPos = bbox.gameObject.transform.Find("rigRoot/corner_3").position;


            Vector3 initialHandPosition = new Vector3(0, 0, 0.5f);
            // This particular test is sensitive to the number of test frames, and is run at at slower pace.
            int numSteps = 30;
            var delta = new Vector3(0.1f, 0.1f, 0f);
            yield return PlayModeTestUtilities.ShowHand(Handedness.Right, inputSimulationService, ArticulatedHandPose.GestureId.OpenSteadyGrabPoint, initialHandPosition);
            yield return PlayModeTestUtilities.MoveHand(initialHandPosition, frontRightCornerPos, ArticulatedHandPose.GestureId.OpenSteadyGrabPoint, Handedness.Right, inputSimulationService, numSteps);
            yield return PlayModeTestUtilities.MoveHand(frontRightCornerPos, frontRightCornerPos + delta, ArticulatedHandPose.GestureId.Pinch, Handedness.Right, inputSimulationService, numSteps);

            var endBounds = bbox.GetComponent<BoxCollider>().bounds;
            Vector3 expectedCenter = new Vector3(0.033f, 0.033f, 1.467f);
            Vector3 expectedSize = Vector3.one * .567f;
            TestUtilities.AssertAboutEqual(endBounds.center, expectedCenter, "endBounds incorrect center");
            TestUtilities.AssertAboutEqual(endBounds.size, expectedSize, "endBounds incorrect size");

            GameObject.Destroy(bbox.gameObject);
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

            BoundsControl bbox = InstantiateSceneAndDefaultBbox();
            yield return VerifyInitialBoundsCorrect(bbox);
            var scaleHandler = bbox.EnsureComponent<MinMaxScaleConstraint>();
            scaleHandler.ScaleMinimum = minScale;
            scaleHandler.ScaleMaximum = maxScale;
            bbox.RegisterTransformScaleHandler(scaleHandler);

            Vector3 initialScale = bbox.transform.localScale;

            const int numHandSteps = 1;

            Vector3 initialHandPosition = new Vector3(0, 0, 0.5f);
            var frontRightCornerPos = bbox.gameObject.transform.Find("rigRoot/corner_3").position; // front right corner is corner 3
            TestHand hand = new TestHand(Handedness.Right);

            // Hands grab object at initial position
            yield return hand.Show(initialHandPosition);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.OpenSteadyGrabPoint);
            yield return hand.MoveTo(frontRightCornerPos, numHandSteps);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);

            // No change to scale yet
            Assert.AreEqual(initialScale, bbox.transform.localScale);

            // Move hands beyond max scale limit
            yield return hand.MoveTo(new Vector3(scaleHandler.ScaleMaximum * 2, scaleHandler.ScaleMaximum * 2, 0) + frontRightCornerPos, numHandSteps);

            // Assert scale at max
            Assert.AreEqual(Vector3.one * scaleHandler.ScaleMaximum, bbox.transform.localScale);

            // Move hands beyond min scale limit
            yield return hand.MoveTo(new Vector3(-scaleHandler.ScaleMinimum * 2, -scaleHandler.ScaleMinimum * 2, 0) + frontRightCornerPos, numHandSteps);

            // Assert scale at min
            Assert.AreEqual(Vector3.one * scaleHandler.ScaleMinimum, bbox.transform.localScale);

            GameObject.Destroy(bbox.gameObject);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;

        }

        /// <summary>
        /// Uses far interaction (HoloLens 1 style) to scale the bounds control
        /// </summary>
        [UnityTest]
        public IEnumerator ScaleViaHoloLens1Interaction()
        {
            BoundsControl bbox = InstantiateSceneAndDefaultBbox();
            yield return VerifyInitialBoundsCorrect(bbox);
            BoxCollider boxCollider = bbox.GetComponent<BoxCollider>();           
            PlayModeTestUtilities.PushHandSimulationProfile();
            PlayModeTestUtilities.SetHandSimulationMode(HandSimulationMode.Gestures);

            CameraCache.Main.transform.LookAt(bbox.gameObject.transform.Find("rigRoot/corner_3").transform);

            var startHandPos = CameraCache.Main.transform.TransformPoint(new Vector3( 0.1f, 0f, 1.5f));
            TestHand rightHand = new TestHand(Handedness.Right);
            yield return rightHand.Show(startHandPos);
            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            // After pinching, center should remain the same
            var afterPinchbounds = boxCollider.bounds;
            TestUtilities.AssertAboutEqual(afterPinchbounds.center, boundsControlStartCenter, "bbox incorrect center after pinch");
            TestUtilities.AssertAboutEqual(afterPinchbounds.size, boundsControlStartScale, "bbox incorrect size after pinch");

            var delta = new Vector3(0.1f, 0.1f, 0f);
            yield return rightHand.Move(delta);

            var endBounds = boxCollider.bounds;
            Vector3 expectedCenter = new Vector3(0.033f, 0.033f, 1.467f);
            Vector3 expectedSize = Vector3.one * .567f;
            TestUtilities.AssertAboutEqual(endBounds.center, expectedCenter, "endBounds incorrect center");
            TestUtilities.AssertAboutEqual(endBounds.size, expectedSize, "endBounds incorrect size", 0.02f);

            GameObject.Destroy(bbox.gameObject);
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
            BoundsControl bbox = InstantiateSceneAndDefaultBbox();
            yield return VerifyInitialBoundsCorrect(bbox);
            bbox.HideElementsInInspector = false;
            yield return null;

            var startBounds = GetBoundsControlRigBounds(bbox);
            TestUtilities.AssertAboutEqual(startBounds.center, boundsControlStartCenter, "bbox incorrect center at start");
            TestUtilities.AssertAboutEqual(startBounds.size, boundsControlStartScale, "bbox incorrect size at start");

            bbox.gameObject.transform.localScale *= 2;
            yield return null;

            var afterScaleBounds = GetBoundsControlRigBounds(bbox);
            var scaledSize = boundsControlStartScale * 2;
            TestUtilities.AssertAboutEqual(afterScaleBounds.center, boundsControlStartCenter, "bbox incorrect center after scale");
            TestUtilities.AssertAboutEqual(afterScaleBounds.size, scaledSize, "bbox incorrect size after scale");

            bbox.gameObject.transform.position += Vector3.one;
            yield return null;
            var afterTranslateBounds = GetBoundsControlRigBounds(bbox);
            var afterTranslateCenter = Vector3.one + boundsControlStartCenter;

            TestUtilities.AssertAboutEqual(afterTranslateBounds.center, afterTranslateCenter, "bbox incorrect center after translate");
            TestUtilities.AssertAboutEqual(afterTranslateBounds.size, scaledSize, "bbox incorrect size after translate");

            var c0 = bbox.gameObject.transform.Find("rigRoot/corner_0");
            var bboxBottomCenter = afterTranslateBounds.center - Vector3.up * afterTranslateBounds.extents.y;
            Vector3 cc0 = c0.position - bboxBottomCenter;
            float rotateAmount = 30;
            bbox.gameObject.transform.Rotate(new Vector3(0, rotateAmount, 0));
            yield return null;
            Vector3 cc0_rotated = c0.position - bboxBottomCenter;
            Assert.AreApproximatelyEqual(Vector3.Angle(cc0, cc0_rotated), 30, $"rotated angle is not correct. expected {rotateAmount} but got {Vector3.Angle(cc0, cc0_rotated)}");

            GameObject.Destroy(bbox.gameObject);
        }

        /// <summary>
        /// Ensure that while using BoundingBox, if that object gets
        /// deactivated, that BoundingBox no longer transforms that object.
        /// </summary>
        [UnityTest]
        public IEnumerator DisableObject()
        {
            var bbox = InstantiateSceneAndDefaultBbox();
            yield return VerifyInitialBoundsCorrect(bbox);

            Vector3 initialScale = bbox.transform.localScale;

            const int numHandSteps = 1;

            Vector3 initialHandPosition = new Vector3(0, 0, 0.5f);
            var frontRightCornerPos = bbox.gameObject.transform.Find("rigRoot/corner_3").position; // front right corner is corner 3
            TestHand hand = new TestHand(Handedness.Right);

            // Hands grab object at initial position
            yield return hand.Show(initialHandPosition);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.OpenSteadyGrabPoint);
            yield return hand.MoveTo(frontRightCornerPos, numHandSteps);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);

            // Verify that scale works before deactivating
            yield return hand.Move(Vector3.right * 0.2f, numHandSteps);
            Vector3 afterTransformScale = bbox.transform.localScale;
            Assert.AreNotEqual(initialScale, afterTransformScale);

            // Deactivate object and ensure that we don't scale
            bbox.gameObject.SetActive(false);
            yield return null;
            bbox.gameObject.SetActive(true);
            yield return hand.Move(Vector3.right * 0.2f, numHandSteps);
            Assert.AreEqual(afterTransformScale, bbox.transform.localScale);
        }

        /// <summary>
        /// Tests proximity scaling on scale handles of bounds control
        /// Verifies default behavior of handles with effect enabled / disabled as well as custom runtime configured scaling / distance values
        /// </summary>
        [UnityTest]
        public IEnumerator ProximityOnScaleHandles()
        {
            var bbox = InstantiateSceneAndDefaultBbox();
            yield return VerifyInitialBoundsCorrect(bbox);

            // 1. test no proximity scaling active per default
            ScaleHandlesConfiguration scaleHandleConfig = bbox.ScaleHandlesConfiguration;
            Vector3 defaultHandleSize = Vector3.one * scaleHandleConfig.HandleSize;

            Vector3 initialHandPosition = new Vector3(0, 0, 0f);
            // this is specific to scale handles
            Transform scaleHandle = bbox.gameObject.transform.Find("rigRoot/corner_3");
            Transform proximityScaledVisual = scaleHandle.GetChild(0)?.GetChild(0);
            var frontRightCornerPos = scaleHandle.position; // front right corner is corner 
            Assert.IsNotNull(proximityScaledVisual, "Couldn't get visual gameobject for scale handle");
            Assert.IsTrue(proximityScaledVisual.name == "visuals", "scale visual has unexpected name");

            yield return null;
            // verify no proximity scaling applied per default
            Assert.AreEqual(proximityScaledVisual.localScale, defaultHandleSize, "Handle was scaled even though proximity effect wasn't active");
            TestHand hand = new TestHand(Handedness.Left);
            Vector3 initialScale = bbox.transform.localScale;

            // Hands grab object at initial position
            yield return hand.Show(initialHandPosition);
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.OpenSteadyGrabPoint);
            yield return hand.MoveTo(frontRightCornerPos);
            yield return null;

            // we're in poximity scaling range - check if proximity scaling wasn't applied
            Assert.AreEqual(proximityScaledVisual.localScale, defaultHandleSize, "Handle was scaled even though proximity effect wasn't active");

            //// reset hand
            yield return hand.MoveTo(initialHandPosition);

            // 2. enable proximity scaling and test defaults
            ProximityEffectConfiguration proximityConfig = bbox.HandleProximityEffectConfiguration;
            proximityConfig.ProximityEffectActive = true;
            proximityConfig.CloseGrowRate = 1.0f;
            proximityConfig.MediumGrowRate = 1.0f;
            proximityConfig.FarGrowRate = 1.0f;
            bbox.CreateRig();
            yield return null; // wait so rig gameobjects get recreated
            yield return TestCurrentProximityConfiguration(bbox, hand, "Defaults");

            // reset hand
            yield return hand.MoveTo(initialHandPosition);

            // 3. now test custom configuration is applied during runtime
            proximityConfig.CloseScale = 4.0f;
            proximityConfig.MediumScale = 3.0f;
            proximityConfig.FarScale = 2.0f;

            proximityConfig.ObjectMediumProximity = 0.2f;
            proximityConfig.ObjectCloseProximity = 0.1f;

            bbox.CreateRig();
            yield return null; // wait so rig gameobjects get recreated
            yield return TestCurrentProximityConfiguration(bbox, hand, "Custom runtime config max");
        }

        /// <summary>
        /// This tests far, medium and close proximity scaling on scale handles by moving the test hand in the corresponding distance ranges
        /// </summary>
        /// <param name="bbox">Bounds Control to test on</param>
        /// <param name="hand">Test hand to use for testing proximity to handle</param>
        private IEnumerator TestCurrentProximityConfiguration(BoundsControl bbox, TestHand hand, string testDescription)
        {
            // get config and scaling handle
            ScaleHandlesConfiguration scaleHandleConfig = bbox.ScaleHandlesConfiguration;
            Vector3 defaultHandleSize = Vector3.one * scaleHandleConfig.HandleSize;
            Transform scaleHandle = bbox.gameObject.transform.Find("rigRoot/corner_3");
            Transform proximityScaledVisual = scaleHandle.GetChild(0)?.GetChild(0);
            var frontRightCornerPos = scaleHandle.position;
            // check far scale applied
            ProximityEffectConfiguration proximityConfig = bbox.HandleProximityEffectConfiguration;
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
            BoundsControl bbox = emptyGameObject.AddComponent<BoundsControl>();

            // set target to cube
            bbox.Target = cube;
            bbox.Active = true;

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
        /// Returns the AABB of the bounds control rig (corners, edges)
        /// that make up the bounds control by using the positions of the corners
        /// </summary>
        private Bounds GetBoundsControlRigBounds(BoundsControl bbox)
        {
            Bounds b = new Bounds();
            b.center = bbox.transform.Find("rigRoot/corner_0").position;
            for (int i = 1; i < 8; ++i)
            {
                Transform corner = bbox.transform.Find("rigRoot/corner_" + i.ToString());
                b.Encapsulate(corner.position);
            }
            return b;
        }
    }
}
#endif