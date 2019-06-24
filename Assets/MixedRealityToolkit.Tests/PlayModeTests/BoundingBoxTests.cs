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
        #region Utilities
        [TearDown]
        public void ShutdownMrtk()
        {
            TestUtilities.ShutdownMixedRealityToolkit();
        }

        private GameObject InstantiateSceneAndDefaultBbox()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes(true);
            TestUtilities.InitializePlayspace();

            RenderSettings.skybox = null;

            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = Vector3.forward * -1.5f;
            cube.AddComponent<BoundingBox>();

            return cube;
        }
        #endregion

        [UnityTest]
        public IEnumerator BBoxInstantiate()
        {
            GameObject bbox = InstantiateSceneAndDefaultBbox();
            yield return null;
            BoundingBox bboxComponent = bbox.GetComponent<BoundingBox>();
            Assert.IsNotNull(bboxComponent);

            GameObject.Destroy(bbox);
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
            var go = InstantiateSceneAndDefaultBbox();
            yield return null;
            var bbox = go.GetComponent<BoundingBox>();
            bbox.BoundingBoxActivation = BoundingBox.BoundingBoxActivationType.ActivateOnStart;
            bbox.HideElementsInInspector = false;
            yield return null;

            var newObject = new GameObject();
            newObject.name = "newObject";
            var bc = newObject.AddComponent<BoxCollider>();
            bc.center = new Vector3(.25f, 0, 0);
            bc.size = new Vector3(0.162f, 0.1f, 1);
            bbox.BoundsOverride = bc;
            yield return null;

            // Get the dimensions of the corners
            List<GameObject> corners = FindDescendantsContainingName(go, "corner_");

            Bounds b = new Bounds();
            b.center = corners[0].transform.position;
            foreach (var c in corners.Skip(1))
            {
                b.Encapsulate(c.transform.position);
            }

            Debug.Assert(b.center == new Vector3(.25f, 0, 0), $"bounds center should be {bc.center} but they are {b.center}");
            Debug.Assert(b.size == new Vector3(0.162f, 0.1f, 1), $"bounds size should be {bc.size} but they are {b.size}");

            yield return null;
        }

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
            while(toExplore.Count > 0)
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