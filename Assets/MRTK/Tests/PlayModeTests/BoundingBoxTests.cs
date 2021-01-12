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

using System.Collections;
using System.Linq;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Assert = UnityEngine.Assertions.Assert;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class BoundingBoxTests
    {
        #region Utilities
        [UnitySetUp]
        public IEnumerator SetUp()
        {
            PlayModeTestUtilities.Setup();
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            PlayModeTestUtilities.TearDown();
            yield return null;
        }

        private readonly Vector3 boundingBoxStartCenter = Vector3.forward * 1.5f;
        private readonly Vector3 boundingBoxStartScale = Vector3.one * 0.5f;

        /// <summary>
        /// Instantiates a bounding box at 0, 0, -1.5f
        /// box is at scale .5, .5, .5
        /// Target is set to its child if targetIsChild is true
        /// </summary>
        private BoundingBox InstantiateSceneAndDefaultBbox(GameObject target = null)
        {
            GameObject bboxGameObject;
            if (target != null)
            {
                bboxGameObject = new GameObject();
            }
            else
            {
                bboxGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            }
            bboxGameObject.transform.position = boundingBoxStartCenter;
            bboxGameObject.transform.localScale = boundingBoxStartScale;
            BoundingBox bbox = bboxGameObject.AddComponent<BoundingBox>();
            if (target != null)
            {
                target.transform.parent = bboxGameObject.transform;
                target.transform.localScale = Vector3.one;
                target.transform.localPosition = Vector3.zero;
                bbox.Target = target;
            }

            MixedRealityPlayspace.PerformTransformation(
            p =>
            {
                p.position = Vector3.zero;
                p.LookAt(bboxGameObject.transform.position);
            });

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

            Object.Destroy(bbox.gameObject);
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

            Object.Destroy(bbox.gameObject);
            Object.Destroy(newObject);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        /// <summary>
        /// Test that if we toggle the bounding box's active status,
        /// that the size of the boundsOverride is consistent, even
        /// when BoxPadding is set.
        /// </summary>
        [UnityTest]
        public IEnumerator BBoxOverridePaddingReset()
        {
            var bbox = InstantiateSceneAndDefaultBbox();
            yield return null;
            bbox.BoundingBoxActivation = BoundingBox.BoundingBoxActivationType.ActivateOnStart;
            bbox.HideElementsInInspector = false;

            // Set the bounding box to have a large padding.
            bbox.BoxPadding = Vector3.one;
            yield return null;

            var newObject = new GameObject();
            var bc = newObject.AddComponent<BoxCollider>();
            bc.center = new Vector3(1, 2, 3);
            var backupSize = bc.size = new Vector3(1, 2, 3);
            bbox.BoundsOverride = bc;
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
            bbox.BoundsOverride = bc;
            yield return null;

            // Toggles the bounding box and verifies
            // integrity of the measurements.
            VerifyBoundingBox();

            // Private helper function to prevent code copypasta.
            IEnumerator VerifyBoundingBox()
            {
                // Toggle the bounding box active status to check that the boundsOverride
                // will persist, and will not be destructively resized 
                bbox.gameObject.SetActive(false);
                yield return null;
                Debug.Log($"bc.size = {bc.size}");
                bbox.gameObject.SetActive(true);
                yield return null;
                Debug.Log($"bc.size = {bc.size}");

                Bounds b = GetBoundingBoxRigBounds(bbox);

                var expectedSize = backupSize + Vector3.Scale(bbox.BoxPadding, newObject.transform.lossyScale);
                Debug.Assert(b.center == bc.center, $"bounds center should be {bc.center} but they are {b.center}");
                Debug.Assert(b.size == expectedSize, $"bounds size should be {expectedSize} but they are {b.size}");
                Debug.Assert(bc.size == expectedSize, $"boundsOverride's size was corrupted.");
            }

            Object.Destroy(bbox.gameObject);
            Object.Destroy(newObject);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        /// <summary>
        /// Tests to see that the handlers grow in size when a pointer is near them
        /// </summary>
        [UnityTest]
        public IEnumerator BBoxHandlerUI()
        {
            const int numSteps = 2;
            var bbox = InstantiateSceneAndDefaultBbox();
            bbox.ShowRotationHandleForX = true;
            bbox.ShowRotationHandleForY = true;
            bbox.ShowRotationHandleForZ = true;
            bbox.ShowScaleHandles = true;
            bbox.ProximityEffectActive = true;
            bbox.ScaleHandleSize = 0.1f;
            bbox.RotationHandleSize = 0.1f;
            bbox.FarScale = 1.0f;
            bbox.MediumScale = 1.5f;
            bbox.CloseScale = 1.5f;
            yield return null;
            var bounds = bbox.GetComponent<BoxCollider>().bounds;
            Assert.AreEqual(new Vector3(0, 0, 1.5f), bounds.center);
            Assert.AreEqual(new Vector3(.5f, .5f, .5f), bounds.size);

            // Defining the edge and corner handlers that will be used
            Debug.Log(bbox.transform.Find("rigRoot/midpoint_0").GetChild(0));
            var originalCornerHandlerScale = bbox.transform.Find("rigRoot/corner_1/visualsScale/visuals").transform.localScale;
            var cornerHandlerPosition = bbox.transform.Find("rigRoot/corner_1").transform.position;
            var originalEdgeHandlerScale = bbox.transform.Find("rigRoot/midpoint_0/Sphere").transform.localScale;
            var edgeHandlerPosition = bbox.transform.Find("rigRoot/midpoint_0").transform.position;

            // Wait for the scaling/unscaling animation to finish
            yield return new WaitForSeconds(0.2f);

            // Move the hand to a handler on the corner 
            TestHand rightHand = new TestHand(Handedness.Right);
            yield return rightHand.Show(new Vector3(0, 0, 0.5f));
            yield return rightHand.MoveTo(cornerHandlerPosition, numSteps);

            // Wait for the scaling/unscaling animation to finish
            yield return new WaitForSeconds(0.4f);

            TestUtilities.AssertAboutEqual(bbox.transform.Find("rigRoot/midpoint_0/Sphere").transform.localScale, originalEdgeHandlerScale, "The edge handler changed mistakingly");
            TestUtilities.AssertAboutEqual(bbox.transform.Find("rigRoot/corner_1/visualsScale/visuals").transform.localScale.normalized, originalCornerHandlerScale.normalized, "The corner handler scale has changed");
            Assert.AreApproximatelyEqual(bbox.transform.Find("rigRoot/corner_1/visualsScale/visuals").transform.localScale.x / originalCornerHandlerScale.x, bbox.MediumScale, 0.1f, "The corner handler did not grow when a pointer was near it");

            // Move the hand to a handler on the edge
            yield return rightHand.MoveTo(edgeHandlerPosition, numSteps);
            // Wait for the scaling/unscaling animation to finish
            yield return new WaitForSeconds(0.4f);

            TestUtilities.AssertAboutEqual(bbox.transform.Find("rigRoot/corner_1/visualsScale/visuals").transform.localScale, originalCornerHandlerScale, "The corner handler changed mistakingly");
            TestUtilities.AssertAboutEqual(bbox.transform.Find("rigRoot/midpoint_0/Sphere").transform.localScale.normalized, originalEdgeHandlerScale.normalized, "The edge handler scale has changed");
            Assert.AreApproximatelyEqual(bbox.transform.Find("rigRoot/midpoint_0/Sphere").transform.localScale.x / originalEdgeHandlerScale.x, bbox.MediumScale, 0.1f, "The edge handler did not grow when a pointer was near it");

            Object.Destroy(bbox.gameObject);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        /// <summary>
        /// Uses near interaction to scale the bounding box by directly grabbing corner
        /// </summary>
        [UnityTest]
        public IEnumerator ScaleViaNearInteration()
        {
            const int numSteps = 2;
            var bbox = InstantiateSceneAndDefaultBbox();
            bbox.ScaleHandleSize = 0.1f;
            yield return null;
            var bounds = bbox.GetComponent<BoxCollider>().bounds;
            Assert.AreEqual(new Vector3(0, 0, 1.5f), bounds.center);
            Assert.AreEqual(new Vector3(.5f, .5f, .5f), bounds.size);

            // front right corner is corner 3
            var frontRightCornerPos = bbox.ScaleCorners[3].transform.position;

            TestHand rightHand = new TestHand(Handedness.Right);
            yield return rightHand.Show(new Vector3(0, 0, 0.5f));
            var delta = new Vector3(0.1f, 0.1f, 0f);
            yield return rightHand.MoveTo(frontRightCornerPos, numSteps);
            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            yield return rightHand.Move(delta, numSteps);

            var endBounds = bbox.GetComponent<BoxCollider>().bounds;
            TestUtilities.AssertAboutEqual(endBounds.center, new Vector3(0.033f, 0.033f, 1.467f), "endBounds incorrect center");
            TestUtilities.AssertAboutEqual(endBounds.size, Vector3.one * .567f, "endBounds incorrect size");

            Object.Destroy(bbox.gameObject);
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
            var scaleHandler = bbox.EnsureComponent<MinMaxScaleConstraint>();
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

            Object.Destroy(bbox.gameObject);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;

        }

        /// <summary>
        /// This tests the minimum and maximum scaling for the bounding box when target is a child of the box.
        /// </summary>
        [UnityTest]
        public IEnumerator ScaleChildTargetMinMax()
        {
            float minScale = 0.5f;
            float maxScale = 2f;

            var target = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var bbox = InstantiateSceneAndDefaultBbox(target);
            var scaleHandler = bbox.EnsureComponent<MinMaxScaleConstraint>();
            scaleHandler.ScaleMinimum = minScale;
            scaleHandler.ScaleMaximum = maxScale;
            yield return null;

            Vector3 initialScale = bbox.Target.transform.localScale;

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
            Assert.AreEqual(initialScale, bbox.Target.transform.localScale);

            // Move hands beyond max scale limit
            yield return hand.MoveTo(new Vector3(scaleHandler.ScaleMaximum * 2, scaleHandler.ScaleMaximum * 2, 0) + frontRightCornerPos, numHandSteps);
            
            // Assert scale at max
            Assert.AreEqual(Vector3.one * scaleHandler.ScaleMaximum, bbox.Target.transform.localScale);

            // Move hands beyond min scale limit
            yield return hand.MoveTo(new Vector3(-scaleHandler.ScaleMinimum * 2, -scaleHandler.ScaleMinimum * 2, 0) + frontRightCornerPos, numHandSteps);

            // Assert scale at min
            Assert.AreEqual(Vector3.one * scaleHandler.ScaleMinimum, bbox.Target.transform.localScale);
        }

        /// <summary>
        /// Uses far interaction (HoloLens 1 style) to scale the bounding box
        /// </summary>
        [UnityTest]
        public IEnumerator ScaleViaHoloLens1Interaction()
        {
            var bbox = InstantiateSceneAndDefaultBbox();
            yield return null;
            yield return null;

            var bounds = bbox.GetComponent<BoxCollider>().bounds;
            var startCenter = new Vector3(0, 0, 1.5f);
            var startSize = new Vector3(.5f, .5f, .5f);
            TestUtilities.AssertAboutEqual(bounds.center, startCenter, "bbox incorrect center at start");
            TestUtilities.AssertAboutEqual(bounds.size, startSize, "bbox incorrect size at start");

            // Switch to hand gestures
            var iss = PlayModeTestUtilities.GetInputSimulationService();
            var oldSimMode = iss.ControllerSimulationMode;
            iss.ControllerSimulationMode = ControllerSimulationMode.HandGestures;

            CameraCache.Main.transform.LookAt(bbox.ScaleCorners[3].transform);

            var startHandPos = CameraCache.Main.transform.TransformPoint(new Vector3(0.1f, 0f, 1.5f));
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
            TestUtilities.AssertAboutEqual(endBounds.center, new Vector3(0.033f, 0.033f, 1.467f), "endBounds incorrect center", 0.02f);
            TestUtilities.AssertAboutEqual(endBounds.size, Vector3.one * .561f, "endBounds incorrect size", 0.02f);

            Object.Destroy(bbox.gameObject);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;

            // Restore the input simulation profile
            iss.ControllerSimulationMode = oldSimMode;

            yield return null;
        }

        /// <summary>
        /// Uses motion controller to scale the bounding box
        /// </summary>
        [UnityTest]
        public IEnumerator ScaleViaMotionControllerInteraction()
        {
            var bbox = InstantiateSceneAndDefaultBbox();
            yield return null;
            yield return null;

            var bounds = bbox.GetComponent<BoxCollider>().bounds;
            var startCenter = new Vector3(0, 0, 1.5f);
            var startSize = new Vector3(.5f, .5f, .5f);
            TestUtilities.AssertAboutEqual(bounds.center, startCenter, "bbox incorrect center at start");
            TestUtilities.AssertAboutEqual(bounds.size, startSize, "bbox incorrect size at start");

            // Switch to motion controller
            var iss = PlayModeTestUtilities.GetInputSimulationService();
            var oldSimMode = iss.ControllerSimulationMode;
            iss.ControllerSimulationMode = ControllerSimulationMode.MotionController;

            CameraCache.Main.transform.LookAt(bbox.ScaleCorners[3].transform);

            var startPos = CameraCache.Main.transform.TransformPoint(new Vector3(0.21f, -0.35f, 0f));
            TestMotionController rightMotionController = new TestMotionController(Handedness.Right);
            yield return rightMotionController.Show(startPos);
            SimulatedMotionControllerButtonState selectButtonState = new SimulatedMotionControllerButtonState
            {
                IsSelecting = true
            };
            yield return rightMotionController.SetState(selectButtonState);
            yield return null;

            var delta = new Vector3(0.1f, 0.1f, 0f);
            yield return rightMotionController.Move(delta);
            yield return null;

            SimulatedMotionControllerButtonState defaultButtonState = new SimulatedMotionControllerButtonState();
            yield return rightMotionController.SetState(defaultButtonState);
            yield return null;

            var endBounds = bbox.GetComponent<BoxCollider>().bounds;
            TestUtilities.AssertAboutEqual(endBounds.center, new Vector3(0.033f, 0.033f, 1.467f), "endBounds incorrect center", 0.02f);
            TestUtilities.AssertAboutEqual(endBounds.size, Vector3.one * .561f, "endBounds incorrect size", 0.02f);

            Object.Destroy(bbox.gameObject);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;

            // Restore the input simulation profile
            iss.ControllerSimulationMode = oldSimMode;

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

            Object.Destroy(bbox.gameObject);
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
            var scaleHandler = bbox.EnsureComponent<MinMaxScaleConstraint>();
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
        /// Tests setting a target in code that is a different gameobject than the gameobject the boundingbox component is attached to
        /// </summary>
        [UnityTest]
        public IEnumerator SetTarget()
        {
            // create cube without control
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = boundingBoxStartCenter;

            MixedRealityPlayspace.PerformTransformation(
            p =>
            {
                p.position = Vector3.zero;
                p.LookAt(cube.transform.position);
            });

            cube.transform.localScale = boundingBoxStartScale;

            // create another gameobject with boundscontrol attached 
            var emptyGameObject = new GameObject("empty");
            BoundingBox bbox = emptyGameObject.AddComponent<BoundingBox>();

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