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
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Linq;
using System;
using UnityEditor;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class BoundingBoxTests
    {
        private readonly float distanceEpsilon = 0.001f;
        #region Utilities
        [TearDown]
        public void ShutdownMrtk()
        {
            TestUtilities.ShutdownMixedRealityToolkit();
        }

        /// <summary>
        /// Instantiates a bounding box at 0, 0, -1.5f
        /// box is at scale 1,1,1
        /// </summary>
        /// <returns></returns>
        private BoundingBox InstantiateSceneAndDefaultBbox()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes(true);
            TestUtilities.InitializePlayspace();

            RenderSettings.skybox = null;

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

            // Get the dimensions of the corners
            List<GameObject> corners = FindDescendantsContainingName(bbox.gameObject, "corner_");

            Bounds b = new Bounds();
            b.center = corners[0].transform.position;
            foreach (var c in corners.Skip(1))
            {
                b.Encapsulate(c.transform.position);
            }

            Debug.Assert(b.center == bc.center, $"bounds center should be {bc.center} but they are {b.center}");
            Debug.Assert(b.size == bc.size, $"bounds size should be {bc.size} but they are {b.size}");

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

            List<GameObject> corners = FindDescendantsContainingName(bbox.gameObject, "corner_");
            var inputSimulationService = PlayModeTestUtilities.GetInputSimulationService();
            // front right corner is corner 3
            var frontRightCornerPos = corners[3].transform.position;

            yield return PlayModeTestUtilities.ShowHand(Handedness.Right, inputSimulationService, ArticulatedHandPose.GestureId.Open, frontRightCornerPos);
            yield return PlayModeTestUtilities.SetHandState(frontRightCornerPos, ArticulatedHandPose.GestureId.Pinch, Handedness.Right, inputSimulationService);
            var delta = new Vector3(0.1f, 0.1f, 0f);
            yield return PlayModeTestUtilities.MoveHandFromTo(frontRightCornerPos, frontRightCornerPos + delta, 10, ArticulatedHandPose.GestureId.Pinch, Handedness.Right, inputSimulationService);
            var endBounds = bbox.GetComponent<BoxCollider>().bounds;
            AssertAboutEqual(endBounds.center, new Vector3(0.033f, 0.033f, 1.467f), "endBounds incorrect center");
            AssertAboutEqual(endBounds.size, Vector3.one * .567f, "endBounds incorrect size");
        }

        private void AssertAboutEqual(Vector3 actual, Vector3 expected, string message)
        {
            var dist = (actual - expected).magnitude;
            Debug.Assert(dist < distanceEpsilon, $"{message}, expected {expected.ToString("0.000")}, was {actual.ToString("0.000")}, distance {dist}");
        }

        //public IEnumerator ScaleViaFarInteraction()
        //{
        //    var bbox = InstantiateSceneAndDefaultBbox();
        //    // make the box smaller so we can see it

        //}

        /// <summary>
        /// Waits for the user to press the enter key before a test continues.
        /// Not actually used by any test, but it is useful when debugging since you can 
        /// pause the state of the test and inspect the scene.
        /// </summary>
        private IEnumerator WaitForEnterKey()
        {
            Debug.Log(Time.time + "Press Enter...");
            while (!UnityEngine.Input.GetKeyDown(KeyCode.Return))
            {
                yield return null;
            }
        }


        private List<GameObject> FindDescendantsContainingName(GameObject rigRoot, string v)
        {
            Queue<Transform> toExplore = new Queue<Transform>();
            toExplore.Enqueue(rigRoot.transform);
            List<GameObject> result = new List<GameObject>();
            while (toExplore.Count > 0)
            {
                var cur = toExplore.Dequeue();
                if (cur.name.Contains(v))
                {
                    result.Add(cur.gameObject);
                }
                for (int i = 0; i < cur.childCount; i++)
                {
                    toExplore.Enqueue(cur.GetChild(i));
                }
            }
            return result;
        }
    }
}
#endif