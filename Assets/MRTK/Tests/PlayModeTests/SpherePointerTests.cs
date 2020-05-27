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

using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using UnityEditor;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Input;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    // Tests to verify sphere pointer distances
    public class SpherePointerTests : BasePlayModeTests
    {
        // Keeping this low by default so the test runs fast. Increase it to be able to see hand movements in the editor.
        private const int numFramesPerMove = 3;

        private float colliderSurfaceZ;

        // Grabbable cube that we want to be manipulating
        private GameObject cube;

        // Grabbable cube that we want to be manipulating to tests overlaps
        private GameObject overlapRect;

        // Initializes MRTK, instantiates the test content prefab and adds a pointer handler to the test collider
        [SetUp]
        public override void Setup()
        {
            base.Setup();

            float centerZ = 2.0f;
            float scale = 0.2f;
            colliderSurfaceZ = centerZ - scale * 0.5f;
            cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.localPosition = new Vector3(0, 0, centerZ);
            cube.transform.localScale = Vector3.one * scale;

            var collider = cube.GetComponentInChildren<Collider>();
            Assert.IsNotNull(collider);

            var grabbable = cube.AddComponent<NearInteractionGrabbable>();
            Assert.IsNotNull(grabbable);
            
            float overlapCenterZ = centerZ;
            overlapRect = GameObject.CreatePrimitive(PrimitiveType.Cube);
            overlapRect.transform.localPosition = new Vector3(0, 0, overlapCenterZ);
            overlapRect.transform.localScale = new Vector3(1.5f,1.5f,1f) * scale;
            overlapRect.SetActive(false);

            var overlapCollider = overlapRect.GetComponentInChildren<Collider>();
            Assert.IsNotNull(overlapCollider);

            var overlapGrabbable = overlapRect.AddComponent<NearInteractionGrabbable>();
            Assert.IsNotNull(overlapGrabbable);
        }

        [TearDown]
        public override void TearDown()
        {
            GameObject.Destroy(cube);
            base.TearDown();
        }

        /// <summary>
        /// Verifies that SpherePointer correctly returns IsNearObject and IsInteractionEnabled
        /// only when it is near a grabbable object, on the correct grabbable layer.
        /// </summary>
        [UnityTest]
        public IEnumerator GrabLayerMasks()
        {
            // Initialize hand
            var rightHand = new TestHand(Handedness.Right);
            yield return rightHand.Show(Vector3.zero);
            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.OpenSteadyGrabPoint);

            var pointer = rightHand.GetPointer<SpherePointer>();
            Assert.IsNotNull(pointer, "Expected to find SpherePointer in the hand controller");
            Vector3 interactionEnabledPos = new Vector3(0.05f, 0, colliderSurfaceZ - pointer.SphereCastRadius);

            Assert.False(pointer.IsNearObject);
            Assert.False(pointer.IsInteractionEnabled);

            // Move hand to object, IsNearObject, IsInteractionEnabled should be true
            yield return rightHand.MoveTo(interactionEnabledPos);
            Assert.True(pointer.IsNearObject);
            Assert.True(pointer.IsInteractionEnabled);

            // Set layer to spatial mesh, which sphere pointer should be ignoring
            // assumption: layer 31 is the spatial mesh layer
            cube.SetLayerRecursively(31);
            yield return null;
            Assert.False(pointer.IsNearObject);
            Assert.False(pointer.IsInteractionEnabled);

            // Set layer back to default
            cube.SetLayerRecursively(0);
            yield return null;
            Assert.True(pointer.IsNearObject);
            Assert.True(pointer.IsInteractionEnabled);

            // Remove the grabbable component, ray should turn on
            GameObject.Destroy(cube.GetComponent<NearInteractionGrabbable>());
            yield return null;
            Assert.False(pointer.IsNearObject);
            Assert.False(pointer.IsInteractionEnabled);

            // Add back the grabbable, ray should turn off
            cube.AddComponent<NearInteractionGrabbable>();
            yield return null;
            Assert.True(pointer.IsNearObject);
            Assert.True(pointer.IsInteractionEnabled);
        }

        /// <summary>
        /// Verifies that SpherePointer correctly returns IsNearObject and IsInteractionEnabled
        /// and only objects on the correct grabbable layer are in focus
        /// </summary>
        [UnityTest]
        public IEnumerator GrabLayerMasksWithOverlap()
        {
            // Initialize hand
            var rightHand = new TestHand(Handedness.Right);
            yield return rightHand.Show(Vector3.zero);
            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.OpenSteadyGrabPoint);

            var pointer = rightHand.GetPointer<SpherePointer>();
            Assert.IsNotNull(pointer, "Expected to find SpherePointer in the hand controller");
            Vector3 interactionEnabledPos = new Vector3(0.05f, 0, colliderSurfaceZ - pointer.SphereCastRadius);

            Assert.False(pointer.IsNearObject);
            Assert.False(pointer.IsInteractionEnabled);

            // Initialize overlapRect
            overlapRect.SetActive(true);

            // Set the cube's layer to spatial mesh, which sphere pointer should be ignoring
            // assumption: layer 31 is the spatial mesh layer
            cube.SetLayerRecursively(31);
            yield return null;
            Assert.False(pointer.IsNearObject);
            Assert.False(pointer.IsInteractionEnabled);

            // Move hand to object, IsNearObject, IsInteractionEnabled should be true
            yield return rightHand.MoveTo(interactionEnabledPos);
            Assert.True(CoreServices.InputSystem.FocusProvider.GetFocusedObject(pointer) == overlapRect, " the overlapping rectangle was not in focus");
            Assert.True(pointer.IsNearObject);
            Assert.True(pointer.IsInteractionEnabled);

            // Set cube's layer back to default
            // Set overlapRect's layer to spatial mesh, which sphere pointer should be ignoring
            // assumption: layer 31 is the spatial mesh layer
            overlapRect.SetLayerRecursively(31);
            cube.SetLayerRecursively(0);
            yield return null;
            Assert.True(CoreServices.InputSystem.FocusProvider.GetFocusedObject(pointer) == cube, " the inner cube was not in focus");

            // Reinitialize the overlapRect
            overlapRect.SetLayerRecursively(0);
            overlapRect.SetActive(false);
        }

        /// <summary>
        /// Verifies that the IsNearObject and IsInteractionEnabled get set 
        /// at the correct times as a hand approaches a grabbable object
        /// </summary>
        [UnityTest]
        public IEnumerator SpherePointerDistances()
        {
            Vector3 margin = new Vector3(0, 0, 0.001f);

            var rightHand = new TestHand(Handedness.Right);

            // Show hand far enough from the test collider
            Vector3 idlePos = new Vector3(0.05f, 0, 1.0f);
            yield return rightHand.Show(idlePos);
            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.OpenSteadyGrabPoint);

            var pointer = rightHand.GetPointer<SpherePointer>();
            Assert.IsNotNull(pointer, "Expected to find SpherePointer in the hand controller");
            Vector3 nearObjectPos = new Vector3(0.05f, 0, colliderSurfaceZ - pointer.NearObjectRadius);
            Vector3 interactionEnabledPos = new Vector3(0.05f, 0, colliderSurfaceZ - pointer.SphereCastRadius);

            Assert.False(pointer.IsNearObject);
            Assert.False(pointer.IsInteractionEnabled);

            // Move hand closer to the collider to enable IsNearObject
            yield return rightHand.MoveTo(nearObjectPos - margin, numFramesPerMove);
            Assert.False(pointer.IsNearObject);
            Assert.False(pointer.IsInteractionEnabled);
            yield return rightHand.MoveTo(nearObjectPos + margin, numFramesPerMove);
            Assert.True(pointer.IsNearObject);
            Assert.False(pointer.IsInteractionEnabled);

            // Move hand closer to the collider to enable IsInteractionEnabled
            yield return rightHand.MoveTo(interactionEnabledPos - margin, numFramesPerMove);
            Assert.True(pointer.IsNearObject);
            Assert.False(pointer.IsInteractionEnabled);
            yield return rightHand.MoveTo(interactionEnabledPos + margin, numFramesPerMove);
            Assert.True(pointer.IsNearObject);
            Assert.True(pointer.IsInteractionEnabled);
            // Move hand back out to disable IsInteractionEnabled
            yield return rightHand.MoveTo(interactionEnabledPos - margin, numFramesPerMove);
            Assert.True(pointer.IsNearObject);
            Assert.False(pointer.IsInteractionEnabled);

            // Move hand back out to disable IsNearObject
            yield return rightHand.MoveTo(nearObjectPos + margin, numFramesPerMove);
            Assert.True(pointer.IsNearObject);
            Assert.False(pointer.IsInteractionEnabled);
            yield return rightHand.MoveTo(nearObjectPos - margin, numFramesPerMove);
            Assert.False(pointer.IsNearObject);
            Assert.False(pointer.IsInteractionEnabled);

            yield return rightHand.MoveTo(idlePos, numFramesPerMove);
            Assert.False(pointer.IsNearObject);
            Assert.False(pointer.IsInteractionEnabled);
        }
    }
}

#endif
