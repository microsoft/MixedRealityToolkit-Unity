// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if !WINDOWS_UWP
// When the .NET scripting backend is enabled and C# projects are built
// Unity doesn't include the required assemblies (i.e. the ones below).
// Given that the .NET backend is deprecated by Unity at this point it's we have
// to work around this on our end.
using Microsoft.MixedReality.Toolkit.UI;
using NUnit.Framework;
using System.Collections;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Linq;
using System;
using UnityEditor;
using System.Collections.Generic;
using Assert = UnityEngine.Assertions.Assert;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class BoundingBoxTests
    {
        #region Utilities
        [TearDown]
        public void ShutdownMrtk()
        {
            TestUtilities.ShutdownMixedRealityToolkit();
        }

        /// <summary>
        /// Instantiates a bounding box at 0, 0, -1.5f
        /// box is at scale .5, .5, .5
        /// </summary>
        /// <returns></returns>
        private BoundingBox InstantiateSceneAndDefaultBbox()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes(true);
            TestUtilities.InitializePlayspace();

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
        /// <returns></returns>
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
        /// <returns></returns>
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

        /// <summary>
        /// Uses near interaction to scale the bounding box by directly grabbing corner
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator ScaleViaNearInteration()
        {
            var bbox = InstantiateSceneAndDefaultBbox();
            yield return null;
            var bounds = bbox.GetComponent<BoxCollider>().bounds;
            Debug.Assert(bounds.center == new Vector3(0, 0, 1.5f));
            Debug.Assert(bounds.size == new Vector3(.5f, .5f, .5f));

            var inputSimulationService = PlayModeTestUtilities.GetInputSimulationService();
            // front right corner is corner 3
            var frontRightCornerPos = bbox.ScaleCorners[3].transform.position;

            yield return PlayModeTestUtilities.ShowHand(Handedness.Right, inputSimulationService, ArticulatedHandPose.GestureId.Open, frontRightCornerPos);
            yield return PlayModeTestUtilities.SetHandState(frontRightCornerPos, ArticulatedHandPose.GestureId.Pinch, Handedness.Right, inputSimulationService);
            var delta = new Vector3(0.1f, 0.1f, 0f);

            yield return PlayModeTestUtilities.MoveHandFromTo(frontRightCornerPos, frontRightCornerPos + delta, 10, ArticulatedHandPose.GestureId.Pinch, Handedness.Right, inputSimulationService);
            var endBounds = bbox.GetComponent<BoxCollider>().bounds;
            TestUtilities.AssertAboutEqual(endBounds.center, new Vector3(0.033f, 0.033f, 1.467f), "endBounds incorrect center");
            TestUtilities.AssertAboutEqual(endBounds.size, Vector3.one * .567f, "endBounds incorrect size");

            GameObject.Destroy(bbox.gameObject);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;

        }

        /// <summary>
        /// Uses far interaction (HoloLens 1 style) to scale the bounding box
        /// </summary>
        /// <returns></returns>
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

            var iss = PlayModeTestUtilities.GetInputSimulationService();
            var oldIsp = iss.InputSimulationProfile;
            var isp = ScriptableObject.CreateInstance<MixedRealityInputSimulationProfile>();
            isp.HandSimulationMode = HandSimulationMode.Gestures;
            iss.InputSimulationProfile = isp;

            CameraCache.Main.transform.LookAt(bbox.ScaleCorners[3].transform);
            var startHandPos = CameraCache.Main.transform.TransformPoint(new Vector3( 0.1f, 0f, isp.DefaultHandDistance));
            yield return PlayModeTestUtilities.ShowHand(Handedness.Right, iss, ArticulatedHandPose.GestureId.Open, startHandPos);
            yield return PlayModeTestUtilities.SetHandState(startHandPos, ArticulatedHandPose.GestureId.Pinch, Handedness.Right, iss);
            var delta = new Vector3(0.1f, 0.1f, 0f);

            // After pinching, center should remain the same
            var afterPinchbounds = bbox.GetComponent<BoxCollider>().bounds;
            TestUtilities.AssertAboutEqual(afterPinchbounds.center, startCenter, "bbox incorrect center after pinch");
            TestUtilities.AssertAboutEqual(afterPinchbounds.size, startSize, "bbox incorrect size after pinch");


            yield return PlayModeTestUtilities.MoveHandFromTo(startHandPos, startHandPos + delta, 10, ArticulatedHandPose.GestureId.Pinch, Handedness.Right, iss);
            var endBounds = bbox.GetComponent<BoxCollider>().bounds;
            TestUtilities.AssertAboutEqual(endBounds.center, new Vector3(0.033f, 0.033f, 1.467f), "endBounds incorrect center");
            TestUtilities.AssertAboutEqual(endBounds.size, Vector3.one * .567f, "endBounds incorrect size");

            GameObject.Destroy(bbox.gameObject);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;

            // Restore the input simulation profile
            iss.InputSimulationProfile = oldIsp;
            yield return null;
        }

        /// <summary>
        /// Test that changing the transform of the bounding box target (rotation, scale, translation)
        /// updates the rig bounds
        /// </summary>
        /// <returns></returns>
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
        /// Returns the AABB of the bounding box rig (corners, edges)
        /// that make up the bounding box by using the positions of the corners
        /// </summary>
        /// <param name="bbox"></param>
        /// <returns></returns>
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