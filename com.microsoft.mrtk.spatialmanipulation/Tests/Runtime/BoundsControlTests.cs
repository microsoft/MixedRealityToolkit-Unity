// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Core.Tests;
using Microsoft.MixedReality.Toolkit.Input.Tests;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using HandshapeId = Microsoft.MixedReality.Toolkit.Input.HandshapeTypes.HandshapeId;

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation.Runtime.Tests
{
    /// <summary>
    /// Tests for BoundsControl (v3)
    /// </summary>
    public class BoundsControlTests : BaseRuntimeInputTests
    {
        private static readonly string BoundsVisuals3DOcclusionPath = AssetDatabase.GUIDToAssetPath("7b542306e34a62f4c9a822fcb19b7d99");

        private static readonly string BoundsVisualsTraditionalPath = AssetDatabase.GUIDToAssetPath("ecbf05ce2121a744cb893e82377ba3cd");

        private static readonly List<string> BoundsVisualsPrefabs = new List<string>
        {
            BoundsVisuals3DOcclusionPath,
            BoundsVisualsTraditionalPath
        };

        /// <summary>
        /// Instantiates <paramref name="target"/> and adds a BoundsControl to it. Or,
        /// if <paramref name="target"/> is null, a default cube is spawned instead (with
        /// a BoundsControl!)
        /// </summary>
        /// <param name="target">An object to spawn and wrap with a <see cref="BoundsControl"/></param>
        /// <param name="boundsVisual">Prefab to use for bounds visuals.</param>
        private BoundsControl InstantiateSceneAndDefaultBoundsControl(string boundsVisualPath, GameObject target = null)
        {
            GameObject boundsControlGameObject;
            if (target != null)
            {
                boundsControlGameObject = GameObject.Instantiate(target);
            }
            else
            {
                boundsControlGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            }

            boundsControlGameObject.transform.position = InputTestUtilities.InFrontOfUser(0.75f);
            boundsControlGameObject.transform.localScale = Vector3.one * 0.1f;

            BoundsControl boundsControl = boundsControlGameObject.AddComponent<BoundsControl>();

            GameObject boundsVisual = AssetDatabase.LoadAssetAtPath(boundsVisualPath, typeof(GameObject)) as GameObject;
            Assert.IsNotNull(boundsVisual, "Couldn't load bounds visual prefab at path " + boundsVisualPath);
            boundsControl.BoundsVisualsPrefab = boundsVisual;
            Assert.IsNotNull(boundsControl.BoundsVisualsPrefab, "Prefab is null");
            InputTestUtilities.InitializeCameraToOriginAndForward();

            return boundsControl;
        }

        [UnityTest]
        public IEnumerator SmokeTest([ValueSource(nameof(BoundsVisualsPrefabs))] string visualsPath)
        {
            BoundsControl bc = InstantiateSceneAndDefaultBoundsControl(visualsPath);
            yield return null;
            Assert.IsNotNull(bc);

            Object.Destroy(bc.gameObject);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        [UnityTest]
        public IEnumerator TestNumberOfHandles([ValueSource(nameof(BoundsVisualsPrefabs))] string visualsPath)
        {
            BoundsControl bc = InstantiateSceneAndDefaultBoundsControl(visualsPath);
            yield return null;
            Assert.IsNotNull(bc);

            BoundsHandleInteractable[] allHandles = bc.GetComponentsInChildren<BoundsHandleInteractable>();
            var scaleHandles = new List<BoundsHandleInteractable>();
            var rotateHandles = new List<BoundsHandleInteractable>();

            foreach (var handle in allHandles)
            {
                if (handle.HandleType == HandleType.Scale) { scaleHandles.Add(handle); }
                else if (handle.HandleType == HandleType.Rotation) { rotateHandles.Add(handle); }
            }

            Assert.IsTrue(scaleHandles.Count == 8, $"Incorrect number of scale handles spawned ({scaleHandles.Count})");
            Assert.IsTrue(rotateHandles.Count == 12, $"Incorrect number of rotate handles spawned ({rotateHandles.Count})");

            Object.Destroy(bc.gameObject);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        [UnityTest]
        public IEnumerator TestHandlesActive([ValueSource(nameof(BoundsVisualsPrefabs))] string visualsPath)
        {
            BoundsControl bc = InstantiateSceneAndDefaultBoundsControl(visualsPath);
            yield return null;
            Assert.IsNotNull(bc);

            Assert.IsFalse(bc.HandlesActive, "Handles should start inactive by default");

            BoundsHandleInteractable[] allHandles = bc.GetComponentsInChildren<BoundsHandleInteractable>();

            foreach (var handle in allHandles)
            {
                Assert.IsFalse(handle.enabled, "All handles should be disabled when overall HandlesActive = false");
            }

            bc.HandlesActive = true;

            var squeezableVisuals = bc.GetComponentInChildren<SqueezableBoxVisuals>();
            // Show internal, all should be visible now
            if (squeezableVisuals != null)
            {
                squeezableVisuals.ShowInternalHandles = true;
            }

            yield return null;

            foreach (var handle in allHandles)
            {
                Assert.IsTrue(handle.enabled, "All handles should now be enabled, especially now that we set showInternalHandles");
            }

            Object.Destroy(bc.gameObject);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }

        [UnityTest]
        public IEnumerator TestManipulationDetection([ValueSource(nameof(BoundsVisualsPrefabs))] string visualsPath)
        {
            InputTestUtilities.SetHandAnchorPoint(Handedness.Right, Input.Simulation.ControllerAnchorPoint.Grab);

            BoundsControl bc = InstantiateSceneAndDefaultBoundsControl(visualsPath);
            yield return null;
            Assert.IsNotNull(bc);

            Assert.IsFalse(bc.HandlesActive, "Handles should start inactive by default");

            BoundsHandleInteractable[] allHandles = bc.GetComponentsInChildren<BoundsHandleInteractable>();

            BoundsHandleInteractable testHandle = allHandles[0]; // Just grab a random one, we don't care!

            bc.HandlesActive = true;

            var squeezableVisuals = bc.GetComponentInChildren<SqueezableBoxVisuals>();
            // Show internal, all should be visible now
            if (squeezableVisuals != null)
            {
                squeezableVisuals.ShowInternalHandles = true;
            }

            var rightHand = new TestHand(Handedness.Right);
            yield return rightHand.Show(InputTestUtilities.InFrontOfUser(0.5f));

            Assert.IsFalse(bc.IsManipulated, "BC thought we were already manipulated");

            yield return rightHand.MoveTo(testHandle.transform.position);
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsTrue(testHandle.isHovered, "Handle should be hovered.");

            yield return rightHand.SetHandshape(HandshapeId.Pinch);
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsTrue(testHandle.isSelected, "Handle didn't get selected.");
            Assert.IsTrue(bc.IsManipulated, "No manipulation detected when we grabbed a handle.");

            Object.Destroy(bc.gameObject);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }
    }
}
