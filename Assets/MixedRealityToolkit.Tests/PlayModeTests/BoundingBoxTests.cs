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
using System.Linq;
using Assert = UnityEngine.Assertions.Assert;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class BoundingBoxTests
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

        /// <summary>
        /// Instantiates a bounding box at 0, 0, -1.5f
        /// box is at scale .5, .5, .5
        /// </summary>
        private BoundingBox InstantiateSceneAndDefaultBbox()
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = Vector3.forward * 1.5f;
            BoundingBox bbox = cube.AddComponent<BoundingBox>();

            MixedRealityPlayspace.PerformTransformation(
            p =>
            {
                p.position = Vector3.zero;
                p.LookAt(cube.transform.position);
            });

            bbox.transform.localScale *= 0.5f;
            bbox.Active = true;

            return bbox;
        }
        #endregion

        /// <summary>
        /// Verify that we can instantiate bounding box at runtime
        /// </summary>
        [UnityTest]
        public IEnumerator BBoxInstantiate()
        {
            var bbox = InstantiateSceneAndDefaultBbox();
            yield return null;
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
            var bbox = InstantiateSceneAndDefaultBbox();
            yield return null;
            bbox.BoundingBoxActivation = BoundingBox.BoundingBoxActivationType.ActivateOnStart;
            bbox.HideElementsInInspector = false;
            yield return null;

            var newObject = new GameObject();
            var bc = newObject.AddComponent<BoxCollider>();
            bc.center = new Vector3(.25f, 0, 0);
            bc.size = new Vector3(0.162f, 0.1f, 1);
            bbox.BoundsOverride = bc;
            yield return null;

            Bounds b = GetBoundingBoxRigBounds(bbox);

            Debug.Assert(b.center == bc.center, $"bounds center should be {bc.center} but they are {b.center}");
            Debug.Assert(b.size == bc.size, $"bounds size should be {bc.size} but they are {b.size}");

            GameObject.Destroy(bbox.gameObject);
            GameObject.Destroy(newObject);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration1() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration2() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration3() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration4() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration5() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration6() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration7() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration8() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration9() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration10() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration11() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration12() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration13() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration14() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration15() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration16() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration17() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration18() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration19() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration20() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration21() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration22() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration23() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration24() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration25() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration26() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration27() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration28() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration29() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration30() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration31() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration32() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration33() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration34() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration35() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration36() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration37() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration38() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration39() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration40() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration41() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration42() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration43() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration44() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration45() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration46() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration47() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration48() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration49() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration50() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration51() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration52() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration53() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration54() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration55() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration56() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration57() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration58() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration59() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration60() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration61() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration62() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration63() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration64() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration65() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration66() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration67() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration68() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration69() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration70() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration71() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration72() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration73() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration74() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration75() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration76() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration77() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration78() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration79() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration80() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration81() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration82() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration83() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration84() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration85() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration86() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration87() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration88() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration89() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration90() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration91() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration92() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration93() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration94() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration95() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration96() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration97() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration98() { yield return ScaleViaNearInteration(); }
        [UnityTest]
        public IEnumerator ScaleViaNearInteration99() { yield return ScaleViaNearInteration(); }

        /// <summary>
        /// Uses near interaction to scale the bounding box by directly grabbing corner
        /// </summary>
        public IEnumerator ScaleViaNearInteration()
        {
            var bbox = InstantiateSceneAndDefaultBbox();
            bbox.ScaleHandleSize = 0.1f;
            yield return null;
            var bounds = bbox.GetComponent<BoxCollider>().bounds;
            Assert.AreEqual(new Vector3(0, 0, 1.5f), bounds.center);
            Assert.AreEqual(new Vector3(.5f, .5f, .5f), bounds.size);

            var inputSimulationService = PlayModeTestUtilities.GetInputSimulationService();

            // front right corner is corner 3
            var frontRightCornerPos = bbox.ScaleCorners[3].transform.position;

            TestHand rightHand = new TestHand(Handedness.Right);
            yield return rightHand.Show(new Vector3(0, 0, 0.5f));
            var delta = new Vector3(0.1f, 0.1f, 0f);
            yield return rightHand.MoveTo(frontRightCornerPos, 2);
            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            yield return rightHand.Move(delta, 2);

            var endBounds = bbox.GetComponent<BoxCollider>().bounds;
            TestUtilities.AssertAboutEqual(endBounds.center, new Vector3(0.033f, 0.033f, 1.467f), "endBounds incorrect center");
            TestUtilities.AssertAboutEqual(endBounds.size, Vector3.one * .567f, "endBounds incorrect size");

            GameObject.Destroy(bbox.gameObject);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;

        }

        /// <summary>
        /// This tests the minimum and maximum scaling for the bounding box.
        /// </summary>
        [UnityTest]
        public IEnumerator ScaleMinMax()
        {
            float minScale = 0.5f;
            float maxScale = 2f;

            var bbox = InstantiateSceneAndDefaultBbox();
            var scaleHandler = bbox.EnsureComponent<TransformScaleHandler>();
            scaleHandler.ScaleMinimum = minScale;
            scaleHandler.ScaleMaximum = maxScale;
            yield return null;

            Vector3 initialScale = bbox.transform.localScale;

            const int numHandSteps = 1;

            Vector3 initialHandPosition = new Vector3(0, 0, 0.5f);
            var frontRightCornerPos = bbox.ScaleCorners[3].transform.position; // front right corner is corner 3
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
        /// Uses far interaction (HoloLens 1 style) to scale the bounding box
        /// </summary>
        [UnityTest]
        public IEnumerator ScaleViaHoloLens1Interaction()
        {
            var bbox = InstantiateSceneAndDefaultBbox();
            yield return null;
            var bounds = bbox.GetComponent<BoxCollider>().bounds;
            var startCenter = new Vector3(0, 0, 1.5f);
            var startSize = new Vector3(.5f, .5f, .5f);
            TestUtilities.AssertAboutEqual(bounds.center, startCenter, "bbox incorrect center at start");
            TestUtilities.AssertAboutEqual(bounds.size, startSize, "bbox incorrect size at start");

            PlayModeTestUtilities.PushHandSimulationProfile();
            PlayModeTestUtilities.SetHandSimulationMode(HandSimulationMode.Gestures);

            CameraCache.Main.transform.LookAt(bbox.ScaleCorners[3].transform);

            var startHandPos = CameraCache.Main.transform.TransformPoint(new Vector3( 0.1f, 0f, 1.5f));
            TestHand rightHand = new TestHand(Handedness.Right);
            yield return rightHand.Show(startHandPos);
            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            // After pinching, center should remain the same
            var afterPinchbounds = bbox.GetComponent<BoxCollider>().bounds;
            TestUtilities.AssertAboutEqual(afterPinchbounds.center, startCenter, "bbox incorrect center after pinch");
            TestUtilities.AssertAboutEqual(afterPinchbounds.size, startSize, "bbox incorrect size after pinch");

            var delta = new Vector3(0.1f, 0.1f, 0f);
            yield return rightHand.Move(delta);

            var endBounds = bbox.GetComponent<BoxCollider>().bounds;
            TestUtilities.AssertAboutEqual(endBounds.center, new Vector3(0.033f, 0.033f, 1.467f), "endBounds incorrect center");
            TestUtilities.AssertAboutEqual(endBounds.size, Vector3.one * .561f, "endBounds incorrect size", 0.02f);

            GameObject.Destroy(bbox.gameObject);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;

            // Restore the input simulation profile
            PlayModeTestUtilities.PopHandSimulationProfile();

            yield return null;
        }

        /// <summary>
        /// Test that changing the transform of the bounding box target (rotation, scale, translation)
        /// updates the rig bounds
        /// </summary>
        [UnityTest]
        public IEnumerator UpdateTransformUpdatesBounds()
        {
            var bbox = InstantiateSceneAndDefaultBbox();
            bbox.HideElementsInInspector = false;
            yield return null;

            var startBounds = GetBoundingBoxRigBounds(bbox);
            var startCenter = new Vector3(0, 0, 1.5f);
            var startSize = new Vector3(.5f, .5f, .5f);
            TestUtilities.AssertAboutEqual(startBounds.center, startCenter, "bbox incorrect center at start");
            TestUtilities.AssertAboutEqual(startBounds.size, startSize, "bbox incorrect size at start");

            bbox.gameObject.transform.localScale *= 2;
            yield return null;

            var afterScaleBounds = GetBoundingBoxRigBounds(bbox);
            var scaledSize = startSize * 2;
            TestUtilities.AssertAboutEqual(afterScaleBounds.center, startCenter, "bbox incorrect center after scale");
            TestUtilities.AssertAboutEqual(afterScaleBounds.size, scaledSize, "bbox incorrect size after scale");

            bbox.gameObject.transform.position += Vector3.one;
            yield return null;
            var afterTranslateBounds = GetBoundingBoxRigBounds(bbox);
            var afterTranslateCenter = Vector3.one + startCenter;

            TestUtilities.AssertAboutEqual(afterTranslateBounds.center, afterTranslateCenter, "bbox incorrect center after translate");
            TestUtilities.AssertAboutEqual(afterTranslateBounds.size, scaledSize, "bbox incorrect size after translate");

            var c0 = bbox.ScaleCorners[0];
            var bboxBottomCenter = afterTranslateBounds.center - Vector3.up * afterTranslateBounds.extents.y;
            Vector3 cc0 = c0.transform.position - bboxBottomCenter;
            float rotateAmount = 30;
            bbox.gameObject.transform.Rotate(new Vector3(0, rotateAmount, 0));
            yield return null;
            Vector3 cc0_rotated = c0.transform.position - bboxBottomCenter;
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
            float minScale = 0.5f;
            float maxScale = 2f;

            var bbox = InstantiateSceneAndDefaultBbox();
            var scaleHandler = bbox.EnsureComponent<TransformScaleHandler>();
            scaleHandler.ScaleMinimum = minScale;
            scaleHandler.ScaleMaximum = maxScale;
            yield return null;

            Vector3 initialScale = bbox.transform.localScale;

            const int numHandSteps = 1;

            Vector3 initialHandPosition = new Vector3(0, 0, 0.5f);
            var frontRightCornerPos = bbox.ScaleCorners[3].transform.position; // front right corner is corner 3
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
        /// Returns the AABB of the bounding box rig (corners, edges)
        /// that make up the bounding box by using the positions of the corners
        /// </summary>
        private Bounds GetBoundingBoxRigBounds(BoundingBox bbox)
        {
            var corners = bbox.ScaleCorners;

            Bounds b = new Bounds();
            b.center = corners[0].position;
            foreach (var c in corners.Skip(1))
            {
                b.Encapsulate(c.transform.position);
            }
            return b;
        }
    }
}
#endif