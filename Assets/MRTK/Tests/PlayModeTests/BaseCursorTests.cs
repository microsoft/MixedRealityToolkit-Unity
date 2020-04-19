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

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.UI;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    // Tests to verify that cursor state is updated correctly
    public class BaseCursorTests
    {
        // Keeping this low by default so the test runs fast. Increase it to be able to see hand movements in the editor.
        private const int numFramesPerMove = 1;

        private GameObject cube;

        // Initializes MRTK, instantiates the test content prefab 
        [SetUp]
        public void SetUp()
        {
            PlayModeTestUtilities.Setup();
            TestUtilities.PlayspaceToOriginLookingForward();
            
            // Target frame rate is set to 50 to match the physics
            // tick rate. The rest of the test code needs to wait on a frame to have
            // passed, and this is a rough way of ensuring that each WaitForFixedUpdate()
            // will roughly wait for frame to also pass.
            Application.targetFrameRate = 50;

            // Add a second cube in the background, sometimes physics checks fail with just one collider
            // in scene
            GameObject backgroundCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            backgroundCube.transform.position = Vector3.forward * 30;

            cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.localPosition = new Vector3(0, 0, 2);
            cube.transform.localScale = new Vector3(.2f, .2f, .2f);

            var collider = cube.GetComponentInChildren<Collider>();
            Assert.IsNotNull(collider);
        }

        [TearDown]
        public void TearDown()
        {
            Object.Destroy(cube);
            TestUtilities.ShutdownMixedRealityToolkit();
        }

        private void VerifyCursorStateFromPointers(IEnumerable<IMixedRealityPointer> pointers, CursorStateEnum state)
        {
            Assert.NotZero(pointers.ToReadOnlyCollection().Count);
            foreach(var pointer in pointers)
            {
                VerifyCursorState(pointer.BaseCursor, state);
            }
        }

        private void VerifyCursorState(IMixedRealityCursor cursor, CursorStateEnum state)
        {
            var baseCursor = cursor as BaseCursor;
            Assert.IsNotNull(baseCursor);
            Assert.AreEqual(state, baseCursor.CursorState);
        }

        static internal void VerifyCursorContextFromPointers(IEnumerable<IMixedRealityPointer> pointers, CursorContextEnum context)
        {
            Assert.NotZero(pointers.ToReadOnlyCollection().Count);
            foreach (var pointer in pointers)
            {
                VerifyCursorContext(pointer.BaseCursor, context);
            }
        }

        static private void VerifyCursorContext(IMixedRealityCursor cursor, CursorContextEnum context)
        {
            var baseCursor = cursor as BaseCursor;
            Assert.IsNotNull(baseCursor);
            Assert.AreEqual(context, baseCursor.CursorContext);
        }

        [UnityTest]
        public IEnumerator ArticulatedCursorState()
        {
            var inputSystem = PlayModeTestUtilities.GetInputSystem();

            // Show hand far enough from the test collider so the cursor is not on it
            var hand = new TestHand(Handedness.Right);
            Vector3 offObjectPos = new Vector3(0.05f, 0, 1.0f);
            yield return hand.Show(offObjectPos);
            VerifyCursorStateFromPointers(inputSystem.FocusProvider.GetPointers<ShellHandRayPointer>(), CursorStateEnum.Interact);

            // Move hand closer to the collider so the cursor is on it
            Vector3 onObjectPos = new Vector3(0.05f, 0, 1.5f);
            yield return hand.MoveTo(onObjectPos, numFramesPerMove);
            VerifyCursorStateFromPointers(inputSystem.FocusProvider.GetPointers<ShellHandRayPointer>(), CursorStateEnum.InteractHover);

            // Trigger pinch
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            VerifyCursorStateFromPointers(inputSystem.FocusProvider.GetPointers<ShellHandRayPointer>(), CursorStateEnum.Select);

            // Release pinch
            yield return hand.SetGesture(ArticulatedHandPose.GestureId.Open, false);
            VerifyCursorStateFromPointers(inputSystem.FocusProvider.GetPointers<ShellHandRayPointer>(), CursorStateEnum.Release);

            // Wait to transition back to InteractHover
            yield return null;
            VerifyCursorStateFromPointers(inputSystem.FocusProvider.GetPointers<ShellHandRayPointer>(), CursorStateEnum.InteractHover);

            // Move back so the cursor is no longer on the object
            yield return hand.MoveTo(offObjectPos, numFramesPerMove);
            VerifyCursorStateFromPointers(inputSystem.FocusProvider.GetPointers<ShellHandRayPointer>(), CursorStateEnum.Interact);
        }

        [UnityTest]
        public IEnumerator GestureCursorState()
        {
            var inputSystem = PlayModeTestUtilities.GetInputSystem();

            var iss = PlayModeTestUtilities.GetInputSimulationService();
            var oldHandSimMode = iss.HandSimulationMode;
            iss.HandSimulationMode = HandSimulationMode.Gestures;

            Vector3 underPointerPos = new Vector3(0, 0, 2);
            Vector3 abovePointerPos = new Vector3(0, -2, 2);

            var rightHand = new TestHand(Handedness.Right);
            var leftHand = new TestHand(Handedness.Left);

            // No hands, pointer not on cube, hand open
            cube.transform.localPosition = abovePointerPos;
            yield return new WaitForFixedUpdate();
            yield return null;
            VerifyCursorState(inputSystem.GazeProvider.GazeCursor, CursorStateEnum.Observe);

            // No hands, pointer on cube, hand open
            cube.transform.localPosition = underPointerPos;
            yield return new WaitForFixedUpdate();
            yield return null;
            VerifyCursorState(inputSystem.GazeProvider.GazeCursor, CursorStateEnum.ObserveHover);

            // Right hand, pointer not on cube, hand open
            cube.transform.localPosition = abovePointerPos;
            yield return new WaitForFixedUpdate();
            yield return rightHand.Show(Vector3.zero);
            VerifyCursorState(inputSystem.GazeProvider.GazeCursor, CursorStateEnum.Interact);

            // Both hands, pointer not on cube, hand open
            yield return leftHand.Show(Vector3.zero);
            VerifyCursorState(inputSystem.GazeProvider.GazeCursor, CursorStateEnum.Interact);

            // Left hand, pointer not on cube, hand open
            yield return rightHand.Hide();
            VerifyCursorState(inputSystem.GazeProvider.GazeCursor, CursorStateEnum.Interact);

            // Left hand, pointer on cube, hand open
            cube.transform.localPosition = underPointerPos;
            yield return new WaitForFixedUpdate();
            yield return null;
            VerifyCursorState(inputSystem.GazeProvider.GazeCursor, CursorStateEnum.InteractHover);

            // Both hands, pointer on cube, hand open
            yield return rightHand.Show(Vector3.zero);
            VerifyCursorState(inputSystem.GazeProvider.GazeCursor, CursorStateEnum.InteractHover);

            // Right hand, pointer on cube, hand open
            yield return leftHand.Hide();
            VerifyCursorState(inputSystem.GazeProvider.GazeCursor, CursorStateEnum.InteractHover);

            // Right hand, pointer on cube, hand pinched
            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            VerifyCursorState(inputSystem.GazeProvider.GazeCursor, CursorStateEnum.Select);
            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Open);

            // Left hand, pointer on cube, hand pinched
            yield return rightHand.Hide();
            yield return leftHand.Show(Vector3.zero);
            yield return leftHand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            VerifyCursorState(inputSystem.GazeProvider.GazeCursor, CursorStateEnum.Select);

            // Both hands, pointer on cube, left pinched & right open
            yield return rightHand.Show(Vector3.zero);
            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Open);
            VerifyCursorState(inputSystem.GazeProvider.GazeCursor, CursorStateEnum.Select);

            // Both hands, pointer on cube, both pinched
            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            VerifyCursorState(inputSystem.GazeProvider.GazeCursor, CursorStateEnum.Select);

            // Both hands, pointer on cube, right pinched & left open
            yield return leftHand.SetGesture(ArticulatedHandPose.GestureId.Open);
            VerifyCursorState(inputSystem.GazeProvider.GazeCursor, CursorStateEnum.Select);

            // Restore the input simulation profile
            iss.HandSimulationMode = oldHandSimMode;
            yield return null;
        }

        [UnityTest]
        public IEnumerator CursorContextMove()
        {
            var inputSystem = PlayModeTestUtilities.GetInputSystem();

            // The cube needs to be moved from under the gaze cursor before we add the manipulation handler.
            // Because the cube is under the gaze cursor from the beginning, it gets a focus gained event
            // in Setup(). When we show the right hand, we get a focus lost event from the gaze pointer. 
            // This messes with the CursorContextManipulationHandler hoverCount, as it decrements without
            // ever having incremented. To avoid this, we move the cube out of focus before we add the
            // ManipulationHandler and CursorContextManipulationHandler.
            cube.transform.localPosition = new Vector3(0, -2, 2);
            yield return new WaitForFixedUpdate();
            yield return null;

            ManipulationHandler manipulationHandler = cube.AddComponent<ManipulationHandler>();
            CursorContextManipulationHandler cursorContextManipulationHandler = cube.AddComponent<CursorContextManipulationHandler>();
            yield return new WaitForFixedUpdate();
            yield return null;

            // Move cube back to original position (described above)
            cube.transform.localPosition = new Vector3(0, 0, 2);
            yield return new WaitForFixedUpdate();
            yield return null;

            // Show right hand on object
            var rightHand = new TestHand(Handedness.Right);
            Vector3 rightPos = new Vector3(0.05f, 0, 1.5f);
            yield return rightHand.Show(rightPos);
            yield return new WaitForFixedUpdate();
            yield return null;
            VerifyCursorContextFromPointers(inputSystem.FocusProvider.GetPointers<ShellHandRayPointer>(), CursorContextEnum.None);

            // Pinch right hand
            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            yield return new WaitForFixedUpdate();
            yield return null;
            VerifyCursorContextFromPointers(inputSystem.FocusProvider.GetPointers<ShellHandRayPointer>(), CursorContextEnum.MoveCross);

            // Show left hand on object
            var leftHand = new TestHand(Handedness.Left);
            Vector3 leftPos = new Vector3(-0.05f, 0, 1.5f);
            yield return rightHand.Hide();
            yield return leftHand.Show(leftPos);
            yield return new WaitForFixedUpdate();
            yield return null;
            VerifyCursorContextFromPointers(inputSystem.FocusProvider.GetPointers<ShellHandRayPointer>(), CursorContextEnum.None);

            // Pinch left hand
            yield return leftHand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            yield return new WaitForFixedUpdate();
            yield return null;
            VerifyCursorContextFromPointers(inputSystem.FocusProvider.GetPointers<ShellHandRayPointer>(), CursorContextEnum.MoveCross);

            // Show both hands on object
            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Open);
            yield return rightHand.Show(rightPos);
            yield return leftHand.SetGesture(ArticulatedHandPose.GestureId.Open);
            yield return new WaitForFixedUpdate();
            yield return null;
            VerifyCursorContextFromPointers(inputSystem.FocusProvider.GetPointers<ShellHandRayPointer>(), CursorContextEnum.MoveCross);

            Object.Destroy(cursorContextManipulationHandler);
            Object.Destroy(manipulationHandler);
        }

        [UnityTest]
        public IEnumerator CursorContextScaleRotate()
        {
            var inputSystem = PlayModeTestUtilities.GetInputSystem();

            var empty = new GameObject();
            var info = cube.AddComponent<CursorContextInfo>();
            info.ObjectCenter = empty.transform;

            // Show hand on object
            var hand = new TestHand(Handedness.Right);
            Vector3 handPos = new Vector3(0.05f, 0, 1.5f);
            yield return hand.Show(handPos);
            VerifyCursorContextFromPointers(inputSystem.FocusProvider.GetPointers<ShellHandRayPointer>(), CursorContextEnum.None);

            // rotate, center north
            info.CurrentCursorAction = CursorContextInfo.CursorAction.Rotate;
            empty.transform.SetPositionAndRotation(cube.transform.position + Vector3.up, Quaternion.identity);
            yield return new WaitForFixedUpdate();
            yield return null;
            VerifyCursorContextFromPointers(inputSystem.FocusProvider.GetPointers<ShellHandRayPointer>(), CursorContextEnum.RotateNorthSouth);

            // rotate, center east
            empty.transform.SetPositionAndRotation(cube.transform.position + Vector3.right, Quaternion.identity);
            yield return new WaitForFixedUpdate();
            yield return null;
            VerifyCursorContextFromPointers(inputSystem.FocusProvider.GetPointers<ShellHandRayPointer>(), CursorContextEnum.RotateEastWest);

            // scale, center northeast
            info.CurrentCursorAction = CursorContextInfo.CursorAction.Scale;
            empty.transform.SetPositionAndRotation(cube.transform.position + Vector3.up + Vector3.right, Quaternion.identity);
            yield return new WaitForFixedUpdate();
            yield return null;
            VerifyCursorContextFromPointers(inputSystem.FocusProvider.GetPointers<ShellHandRayPointer>(), CursorContextEnum.MoveNortheastSouthwest);

            // scale, center southeast
            empty.transform.SetPositionAndRotation(cube.transform.position + Vector3.down + Vector3.right, Quaternion.identity);
            yield return new WaitForFixedUpdate();
            yield return null;
            VerifyCursorContextFromPointers(inputSystem.FocusProvider.GetPointers<ShellHandRayPointer>(), CursorContextEnum.MoveNorthwestSoutheast);

            // scale, center northwest
            empty.transform.SetPositionAndRotation(cube.transform.position + Vector3.up + Vector3.left, Quaternion.identity);
            yield return new WaitForFixedUpdate();
            yield return null;
            VerifyCursorContextFromPointers(inputSystem.FocusProvider.GetPointers<ShellHandRayPointer>(), CursorContextEnum.MoveNorthwestSoutheast);

            // scale, center southwest
            empty.transform.SetPositionAndRotation(cube.transform.position + Vector3.down + Vector3.left, Quaternion.identity);
            yield return new WaitForFixedUpdate();
            yield return null;
            VerifyCursorContextFromPointers(inputSystem.FocusProvider.GetPointers<ShellHandRayPointer>(), CursorContextEnum.MoveNortheastSouthwest);

            Object.Destroy(empty);
            Object.Destroy(info);
        }

        [UnityTest]

        public IEnumerator CursorScaling()
        {
            // Finding or initializing necessary objects
            Camera cam = GameObject.FindObjectOfType<Camera>();

            BaseCursor baseCursor = GameObject.FindObjectOfType<BaseCursor>();
            Assert.IsNotNull(baseCursor);

            // Make sure resizing is turned on
            Assert.IsTrue(baseCursor.ResizeCursorWithDistance);

            // Set CursorAngularScale for hardcoded calculations
            baseCursor.CursorAngularSize = 50.0f;

            cube.transform.position = Vector3.forward * 2.0f;

            // Wait for cursor to resize/move
            yield return new WaitForFixedUpdate();
            yield return null;

            // Part one tests cursor size against precalculated values at two different distances

            // FIRST DISTANCE
            float precalculatedScale = 2.0f * Vector3.Distance(cam.transform.position, baseCursor.transform.position) * Mathf.Tan(baseCursor.CursorAngularSize * Mathf.Deg2Rad * 0.5f);
            Assert.IsTrue(Mathf.Approximately(precalculatedScale, baseCursor.transform.localScale.y));

            yield return new WaitForFixedUpdate();
            yield return null;

            cube.transform.position = Vector3.forward;

            // Wait for cursor to resize/move
            yield return new WaitForFixedUpdate();
            yield return null;

            // SECOND DISTANCE
            precalculatedScale = 2.0f * Vector3.Distance(cam.transform.position, baseCursor.transform.position) * Mathf.Tan(baseCursor.CursorAngularSize * Mathf.Deg2Rad * 0.5f);
            Assert.IsTrue(Mathf.Approximately(precalculatedScale, baseCursor.transform.localScale.y));

            yield return new WaitForFixedUpdate();
            yield return null;

            // Part two tests if precalculated angularSize matches what is returned in baseCursor.ComputeScaleWithAngularScale() at two different distances

            // FIRST DISTANCE
            float firstAngularScale = 2 * Mathf.Atan2(baseCursor.LocalScale.y * 0.5f, Vector3.Distance(cam.transform.position, baseCursor.transform.position));

            cube.gameObject.SetActive(false);

            yield return new WaitForFixedUpdate();
            yield return null;

            // SECOND DISTANCE
            float secondAngularScale = 2 * Mathf.Atan2(baseCursor.LocalScale.y * 0.5f, Vector3.Distance(cam.transform.position, baseCursor.transform.position));

            Assert.IsTrue(Mathf.Approximately(firstAngularScale, secondAngularScale));
        }
    }
}

#endif
