// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.


using Microsoft.MixedReality.Toolkit.Input.Experimental;
using Microsoft.MixedReality.Toolkit.Core.Tests;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.TestTools;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Microsoft.MixedReality.Toolkit.Input.Tests
{
    /// <summary>
    /// Basic tests for verifying mouse interactions.
    /// </summary>
    public class SpatialMouseInputTests : BaseRuntimeInputTests
    {
        private const string SpatialMouseControllerPrefabGuid = "dc525621b8522034e867ed2799129315";
        private static readonly string SpatialMouseControllerPrefabPath = AssetDatabase.GUIDToAssetPath(SpatialMouseControllerPrefabGuid);

        private static GameObject controllerReference;

        /// <summary>
        /// Very basic test of SpatialMouseInteractor clicking an Interactable.
        /// </summary>
        [UnityTest]
        public IEnumerator SpatialMouseInteractorSmokeTest()
        {
            var mouse = InputSystem.AddDevice<Mouse>();

            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.AddComponent<StatefulInteractable>();
            cube.transform.position = new Vector3(0, 0, 0.4f);
            cube.transform.localScale = Vector3.one * 0.1f;

            // For this test, we won't use poke selection.
            cube.GetComponent<StatefulInteractable>().DisableInteractorType(typeof(PokeInteractor));

            StatefulInteractable firstCubeInteractable = cube.GetComponent<StatefulInteractable>();

            // Verify that the mouse is hidden by default.
            var notHoveringMouseInteractor = firstCubeInteractable.HoveringRayInteractors.Find(
                (i) => i.GetType() == typeof(SpatialMouseInteractor));

            Assert.IsNull(notHoveringMouseInteractor,
                "SpatialMouseInteractor is hovering without initial input.");

            // Inject mouse deltas.
            using (StateEvent.From(mouse, out var eventPtr))
            {
                ((DeltaControl)mouse["delta"]).WriteValueIntoEvent(new Vector2(1, 1), eventPtr);
                InputSystem.QueueEvent(eventPtr);
                InputSystem.Update();

                ((DeltaControl)mouse["delta"]).WriteValueIntoEvent(new Vector2(-1, -1), eventPtr);
                InputSystem.QueueEvent(eventPtr);
                InputSystem.Update();
            }

            yield return RuntimeTestUtilities.WaitForUpdates();

            // Verify that the mouse is hovering the cube.
            var hoveringMouseInteractor = firstCubeInteractable.HoveringRayInteractors.Find(
                (i) => i.GetType() == typeof(SpatialMouseInteractor));

            Assert.IsNotNull(hoveringMouseInteractor,
                "StatefulInteractable did not get Hovered by SpatialMouseInteractor.");

            // Inject mouse down.
            using (StateEvent.From(mouse, out var eventPtr))
            {
                ((ButtonControl)mouse["press"]).WriteValueIntoEvent(1f, eventPtr);
                InputSystem.QueueEvent(eventPtr);
                InputSystem.Update();
            }

            yield return RuntimeTestUtilities.WaitForUpdates();

            // Verify that the mouse is selecting the cube.
            var selectingMouseInteractor = firstCubeInteractable.interactorsSelecting.Find(
                (i) => i.GetType() == typeof(SpatialMouseInteractor));

            Assert.IsNotNull(selectingMouseInteractor,
                "StatefulInteractable did not get Selected by SpatialMouseInteractor.");

            // Inject mouse up.
            using (StateEvent.From(mouse, out var eventPtr))
            {
                ((ButtonControl)mouse["press"]).WriteValueIntoEvent(0f, eventPtr);
                InputSystem.QueueEvent(eventPtr);
                InputSystem.Update();
            }

            yield return RuntimeTestUtilities.WaitForUpdates();

            // Verify that the mouse is no longer selecting the cube.
            var notSelectingMouseInteractor = firstCubeInteractable.interactorsSelecting.Find(
                (i) => i.GetType() == typeof(SpatialMouseInteractor));

            Assert.IsNull(notSelectingMouseInteractor,
                "StatefulInteractable did not get Unselected by SpatialMouseInteractor.");

            yield return null;
        }

        /// <summary>
        /// Creates and returns the Spatial Mouse Controller.
        /// </summary>
        public static GameObject InstantiateSpatialMouseController()
        {
            Object prefab = AssetDatabase.LoadAssetAtPath(SpatialMouseControllerPrefabPath, typeof(Object));
            controllerReference = Object.Instantiate(prefab) as GameObject;
            return controllerReference;
        }

        /// <summary>
        /// Destroys the Spatial Mouse Controller.
        /// </summary>
        public static void TeardownSpatialMouseController()
        {
            if (Application.isPlaying)
            {
                UnityEngine.Object.Destroy(controllerReference);
            }
        }

        public override IEnumerator Setup()
        {
            yield return base.Setup();

            InstantiateSpatialMouseController();

            yield return null;
        }

        public override IEnumerator TearDown()
        {
            TeardownSpatialMouseController();

            yield return base.TearDown();
        }
    }
}
