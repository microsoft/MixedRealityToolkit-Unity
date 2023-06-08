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
                boundsControlGameObject = Object.Instantiate(target);
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
            Assert.IsTrue(bc == null);
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
            Assert.IsTrue(bc == null);
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
            Assert.IsTrue(bc == null);
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
            yield return RuntimeTestUtilities.WaitForUpdates();

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
            Assert.IsTrue(bc == null);
        }

        [UnityTest]
        public IEnumerator TestHandlesToggleWithObjectManipulator([ValueSource(nameof(BoundsVisualsPrefabs))] string visualsPath)
        {
            // Temporarily turn this test off until the synthesized hands are fixed.
            // Currently, this test is broken because a simple pinch and release with the synthesized hand causes the cube to move.
            if (BoundsVisualsPrefabs.Contains(visualsPath))
            {
                yield break;
            }

            InputTestUtilities.SetHandAnchorPoint(Handedness.Right, Input.Simulation.ControllerAnchorPoint.Grab);
            InputTestUtilities.DisableGazeInteractor();

            BoundsControl bc = InstantiateSceneAndDefaultBoundsControl(visualsPath);
            yield return null;

            Assert.IsNotNull(bc);
            Assert.IsFalse(bc.HandlesActive, "Handles should start inactive by default.");

            // Set up cube with object manipulator
            Vector3 initialObjectPosition = bc.transform.position;
            ObjectManipulator objectManipulator = bc.gameObject.AddComponent<ObjectManipulator>();
            objectManipulator.HostTransform = bc.transform;
            objectManipulator.SmoothingFar = false;
            objectManipulator.SmoothingNear = false;
            bc.Interactable = objectManipulator;

            yield return new WaitForFixedUpdate();

            Vector3 initialHandPosition = InputTestUtilities.InFrontOfUser(0.5f);
            Vector3 initialGrabPosition = InputTestUtilities.InFrontOfUser(new Vector3(-0.05f, -0.05f, initialObjectPosition.z)); // grab around the left bottom corner of the cube
            Quaternion initialGrabRotation = Quaternion.identity;
            TestHand hand = new TestHand(Handedness.Right);

            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsFalse(bc.IsManipulated, "BC thought we were already manipulated.");

            yield return hand.Show(initialHandPosition);
            yield return hand.MoveTo(initialGrabPosition);
            yield return hand.RotateTo(initialGrabRotation);
            yield return RuntimeTestUtilities.WaitForUpdates();

            yield return hand.Click();
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsFalse(objectManipulator.IsGrabSelected, "ObjectManipulator should have been released!");
            Assert.IsTrue(bc.HandlesActive, "Handles should have been toggled.");

            TestUtilities.AssertAboutEqual(bc.transform.position, initialObjectPosition, $"Object should not have moved! Actual position: {bc.transform.position:F5}, should be {initialObjectPosition}", 0.00001f);

            Object.Destroy(bc.gameObject);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
            Assert.IsTrue(bc == null);
        }

        [UnityTest]
        public IEnumerator TestNoHandlesToggleWhenMovingWithObjectManipulator([ValueSource(nameof(BoundsVisualsPrefabs))] string visualsPath)
        {
            InputTestUtilities.SetHandAnchorPoint(Handedness.Right, Input.Simulation.ControllerAnchorPoint.Grab);
            InputTestUtilities.DisableGazeInteractor();

            BoundsControl bc = InstantiateSceneAndDefaultBoundsControl(visualsPath);
            yield return null;

            Assert.IsNotNull(bc);
            Assert.IsFalse(bc.HandlesActive, "Handles should start inactive by default.");

            // Set up cube with object manipulator
            Vector3 initialObjectPosition = bc.transform.position;
            ObjectManipulator objectManipulator = bc.gameObject.AddComponent<ObjectManipulator>();
            objectManipulator.HostTransform = bc.transform;
            objectManipulator.SmoothingFar = false;
            objectManipulator.SmoothingNear = false;
            bc.Interactable = objectManipulator;

            yield return new WaitForFixedUpdate();

            Vector3 initialHandPosition = InputTestUtilities.InFrontOfUser(0.5f);
            Vector3 initialGrabPosition = InputTestUtilities.InFrontOfUser(new Vector3(-0.05f, -0.05f, initialObjectPosition.z)); // grab around the left bottom corner of the cube
            Quaternion initialGrabRotation = Quaternion.identity;
            TestHand hand = new TestHand(Handedness.Right);

            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsFalse(bc.IsManipulated, "BC thought we were already manipulated.");

            yield return hand.Show(initialHandPosition);
            yield return RuntimeTestUtilities.WaitForUpdates();
            yield return hand.MoveTo(initialGrabPosition);
            yield return RuntimeTestUtilities.WaitForUpdates();
            yield return hand.RotateTo(initialGrabRotation);
            yield return RuntimeTestUtilities.WaitForUpdates();

            yield return hand.SetHandshape(HandshapeId.Pinch);
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsTrue(objectManipulator.IsGrabSelected, "ObjectManipulator didn't get grabbed on pinch!");

            yield return hand.Move(Vector3.left);
            yield return RuntimeTestUtilities.WaitForUpdates();
            yield return hand.Move(Vector3.right);
            yield return RuntimeTestUtilities.WaitForUpdates();

            yield return hand.SetHandshape(HandshapeId.Open);
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsFalse(objectManipulator.IsGrabSelected, "ObjectManipulator should have been released!");
            Assert.IsFalse(bc.HandlesActive, "Handles should not have been toggled.");

            TestUtilities.AssertAboutEqual(bc.transform.position, initialObjectPosition, $"Object should be placed generally in the same position! Actual position: {bc.transform.position:F5}, should be {initialObjectPosition}", 0.00001f);

            Object.Destroy(bc.gameObject);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
            Assert.IsTrue(bc == null);
        }

        [UnityTest]
        public IEnumerator TestManipulationCursor([ValueSource(nameof(BoundsVisualsPrefabs))] string visualsPath)
        {
            // Known left scale handle 
            yield return HoverCursorRotation("ScaleHandle", new Vector3(0f, 180f, 45f), visualsPath);
            // Known right scale handle
            yield return HoverCursorRotation("ScaleHandle (1)", new Vector3(0f, 180f, 315f), visualsPath);
            // Known bottom rotate handle
            yield return HoverCursorRotation("RotateHandle (5)", new Vector3(0f, 180f, 90f), visualsPath);
            // Known side rotate handle
            yield return HoverCursorRotation("RotateHandle (10)", new Vector3(0f, 180f, 0f), visualsPath);
        }

        private bool ApproximatelyEquals(Vector3 a, Vector3 b, float tolerance = 0.1f)
        {
            return Mathf.Abs(a.x - b.x) <= tolerance
                && Mathf.Abs(a.y - b.y) <= tolerance
                && Mathf.Abs(a.z - b.z) <= tolerance;
        }

        private bool ApproximatelyEquals(Quaternion a, Quaternion b, float tolerance = 0.1f)
        {
            return Mathf.Abs(a.x - b.x) <= tolerance
                && Mathf.Abs(a.y - b.y) <= tolerance
                && Mathf.Abs(a.z - b.z) <= tolerance;
        }

        private IEnumerator HoverCursorRotation(string handleName, Vector3 expectedRotation, string visualsPath)
        {
            InputTestUtilities.SetHandAnchorPoint(Handedness.Right, Input.Simulation.ControllerAnchorPoint.Grab);

            BoundsControl bc = InstantiateSceneAndDefaultBoundsControl(visualsPath);
            yield return null;
            Assert.IsNotNull(bc);

            Assert.IsFalse(bc.HandlesActive, "Handles should start inactive by default");
            bc.HandlesActive = true;

            // Show internal, all should be visible now
            var squeezableVisuals = bc.GetComponentInChildren<SqueezableBoxVisuals>();
            if (squeezableVisuals != null)
            {
                squeezableVisuals.ShowInternalHandles = true;
            }

            // Grab the specified handle
            BoundsHandleInteractable[] allHandles = bc.GetComponentsInChildren<BoundsHandleInteractable>();
            BoundsHandleInteractable handle = null;
            for (int i = 0; i < allHandles.Length && (handle == null); i++)
            {
                BoundsHandleInteractable nextHandle = allHandles[i];
                if (nextHandle.transform.name.Equals(handleName)) handle = nextHandle;
            }

            // Set up test hand
            TestHand hand = new TestHand(Handedness.Right);
            Vector3 initialHandPosition = InputTestUtilities.InFrontOfUser(new Vector3(0.05f, -0.05f, 0.3f)); // orient hand so far interaction ray will hit button
            yield return hand.Show(initialHandPosition);
            yield return RuntimeTestUtilities.WaitForUpdates();

            // Move it so the far ray hovers the handle
            Assert.IsNotNull(handle);
            yield return hand.AimAt(handle.transform.position);
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsTrue(handle.isHovered, $"Handle should be hovered for {handleName}.");
            SpatialManipulationReticle[] reticles = Object.FindObjectsOfType<SpatialManipulationReticle>();
            Assert.AreEqual(reticles.Length, 1, "Cursor should appear.");
            GameObject cursor = reticles[0].gameObject;
            Assert.IsTrue(ApproximatelyEquals(cursor.transform.eulerAngles, expectedRotation), $"Cursor should be rotated for {handleName}.");
            Quaternion worldRotation = cursor.transform.rotation;

            // Select the handle
            yield return hand.SetHandshape(HandshapeId.Pinch);
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsTrue(handle.isSelected, $"Handle should be selected for {handleName}.");
            Assert.IsTrue(handle.isHovered, $"Handle should be hovered for {handleName}.");
            reticles = Object.FindObjectsOfType<SpatialManipulationReticle>();
            Assert.AreEqual(reticles.Length, 1, $"Cursor should stay during select for {handleName}.");
            cursor = reticles[0].gameObject;
            Assert.IsTrue(ApproximatelyEquals(cursor.transform.eulerAngles, expectedRotation), $"Cursor should be rotated for {handleName}.");
            Assert.IsTrue(ApproximatelyEquals(cursor.transform.rotation, worldRotation), $"Rotation should remain after select for {handleName}.");

            // Move the handle
            yield return hand.Move(Vector3.left);
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsTrue(handle.isSelected, $"Handle should be selected for {handleName}.");
            Assert.IsTrue(handle.isHovered, $"Handle should be hovered for {handleName}.");
            reticles = Object.FindObjectsOfType<SpatialManipulationReticle>();
            Assert.AreEqual(reticles.Length, 1, $"Cursor should stay during move for {handleName}.");
            cursor = reticles[0].gameObject;
            Assert.IsTrue(ApproximatelyEquals(cursor.transform.eulerAngles, expectedRotation), $"Cursor should be rotated for {handleName}.");
            Assert.IsTrue(ApproximatelyEquals(cursor.transform.rotation, worldRotation), $"Rotation should remain after select for {handleName}.");

            Object.Destroy(bc.gameObject);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
            Assert.IsTrue(bc == null);
        }
    }
}
