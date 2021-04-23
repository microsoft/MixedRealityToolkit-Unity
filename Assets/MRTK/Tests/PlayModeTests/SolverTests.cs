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

using Microsoft.MixedReality.Toolkit.Experimental.Utilities;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class SolverTests : BasePlayModeTests
    {
        private const float DistanceThreshold = 1.5f;
        private const float HandDistanceThreshold = 0.5f;
        private const float SolverUpdateWaitTime = 1.0f; // in seconds
        private const float RadialUlnarTestActivationPointModifier = .03f;
        private const float AboveFingerTipsTestActivationPointModifier = .06f;
        private const float WristTestActivationPointModifier = .05f;

        /// <summary>
        /// Internal class used to store data for setup
        /// </summary>
        protected class SetupData
        {
            public SolverHandler handler;
            public Solver solver;
            public GameObject target;
        }

        private List<SetupData> setupDataList = new List<SetupData>();

        [UnityTearDown]
        public override IEnumerator TearDown()
        {
            foreach (var setupData in setupDataList)
            {
                Object.Destroy(setupData?.target);
            }

            return base.TearDown();
        }

        /// <summary>
        /// Test adding solver dynamically at runtime to GameObject
        /// </summary>
        [UnityTest]
        public IEnumerator TestRuntimeInstantiation()
        {
            InstantiateTestSolver<Orbital>();

            yield return null;
        }

        /// <summary>
        /// Test solver system's ability to change target types at runtime
        /// </summary>
        [UnityTest]
        public IEnumerator TestTargetTypes()
        {
            Vector3 rightHandPos = Vector3.right * 50.0f;
            Vector3 leftHandPos = -rightHandPos;
            Vector3 customTransformPos = Vector3.up * 50.0f;

            var transformOverride = new GameObject("Override");
            transformOverride.transform.position = customTransformPos;

            var testObjects = InstantiateTestSolver<Orbital>();
            InputSimulationService inputSimulationService = PlayModeTestUtilities.GetInputSimulationService();

            // Test orbital around right hand
            {
                testObjects.handler.TrackedTargetType = TrackedObjectType.HandJoint;
                yield return TestHandSolver(testObjects, inputSimulationService, rightHandPos, Handedness.Right);
            }

            // Test orbital around left hand line pointer
            {
                testObjects.handler.TrackedTargetType = TrackedObjectType.ControllerRay;
                testObjects.handler.TrackedHandness = Handedness.Left;

                yield return TestHandSolver(testObjects, inputSimulationService, leftHandPos, Handedness.Left);
            }

            // Test orbital around head
            {
                testObjects.handler.TrackedTargetType = TrackedObjectType.Head;

                yield return WaitForFrames(2);

                TestUtilities.AssertLessOrEqual(Vector3.Distance(testObjects.target.transform.position, CameraCache.Main.transform.position), DistanceThreshold);
            }

            // Test orbital around custom override
            {
                testObjects.handler.TrackedTargetType = TrackedObjectType.CustomOverride;
                testObjects.handler.TransformOverride = transformOverride.transform;

                yield return WaitForFrames(2);

                TestUtilities.AssertLessOrEqual(Vector3.Distance(testObjects.target.transform.position, customTransformPos), DistanceThreshold);

                yield return WaitForFrames(2);
            }
        }

        /// <summary>
        /// Tests solver handler's ability to switch hands
        /// </summary>
        [UnityTest]
        public IEnumerator TestHandModality()
        {
            var testObjects = InstantiateTestSolver<Orbital>();

            // Set solver handler to track hands
            testObjects.handler.TrackedTargetType = TrackedObjectType.HandJoint;

            // Set and save relevant positions
            Vector3 rightHandPos = Vector3.right * 20.0f;
            Vector3 leftHandPos = Vector3.right * -20.0f;

            yield return WaitForFrames(2);

            InputSimulationService inputSimulationService = PlayModeTestUtilities.GetInputSimulationService();

            // Test orbital around right hand
            yield return TestHandSolver(testObjects, inputSimulationService, rightHandPos, Handedness.Right);

            // Test orbital around left hand
            yield return TestHandSolver(testObjects, inputSimulationService, leftHandPos, Handedness.Left);

            // Test orbital with both hands visible
            yield return PlayModeTestUtilities.ShowHand(Handedness.Left, inputSimulationService, Utilities.ArticulatedHandPose.GestureId.Open, leftHandPos);
            yield return PlayModeTestUtilities.ShowHand(Handedness.Right, inputSimulationService, Utilities.ArticulatedHandPose.GestureId.Open, rightHandPos);

            // Give time for cube to float to hand
            yield return WaitForFrames(2);

            Vector3 handOrbitalPos = testObjects.target.transform.position;
            TestUtilities.AssertLessOrEqual(Vector3.Distance(handOrbitalPos, leftHandPos), DistanceThreshold);
        }

        /// <summary>
        /// Test Surface Magnetism against "wall" and that attached object falls head direction
        /// </summary>
        [UnityTest]
        public IEnumerator TestSurfaceMagnetism()
        {
            // Reset view to origin
            MixedRealityPlayspace.PerformTransformation(p =>
            {
                p.position = Vector3.zero;
                p.LookAt(Vector3.forward);
            });

            // Build wall to collide against
            var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.transform.localScale = new Vector3(25.0f, 25.0f, 0.2f);
            wall.transform.Rotate(Vector3.up, 180.0f); // Rotate wall so forward faces camera
            wall.transform.position = Vector3.forward * 10.0f;

            yield return WaitForFrames(2);

            // Instantiate our test GameObject with solver. 
            // Set layer to ignore raycast so solver doesn't raycast itself (i.e BoxCollider)
            var testObjects = InstantiateTestSolver<SurfaceMagnetism>();
            testObjects.target.layer = LayerMask.NameToLayer("Ignore Raycast");
            SurfaceMagnetism surfaceMag = testObjects.solver as SurfaceMagnetism;

            var targetTransform = testObjects.target.transform;
            var cameraTransform = CameraCache.Main.transform;

            yield return WaitForFrames(2);

            // Confirm that the surfacemagnetic cube is about on the wall straight ahead
            TestUtilities.AssertLessOrEqual(Vector3.Distance(targetTransform.position, wall.transform.position), DistanceThreshold);

            // Rotate the camera
            Vector3 cameraDir = Vector3.forward + Vector3.right;
            MixedRealityPlayspace.PerformTransformation(p =>
            {
                p.position = Vector3.zero;
                p.LookAt(cameraDir);
            });

            // Calculate where our camera hits the wall
            RaycastHit hitInfo;
            Assert.IsTrue(UnityEngine.Physics.Raycast(Vector3.zero, cameraDir, out hitInfo), "Raycast from camera did not hit wall");

            // Let SurfaceMagnetism update
            yield return WaitForFrames(2);

            // Confirm that the surfacemagnetic cube is on the wall with camera rotated
            TestUtilities.AssertLessOrEqual(Vector3.Distance(targetTransform.position, hitInfo.point), DistanceThreshold);

            // Default orientation mode is TrackedTarget, test object should be facing camera
            Assert.IsTrue(Mathf.Approximately(-1.0f, Vector3.Dot(targetTransform.forward.normalized, cameraTransform.forward.normalized)));

            // Change default orientation mode to surface normal
            surfaceMag.CurrentOrientationMode = SurfaceMagnetism.OrientationMode.SurfaceNormal;

            yield return WaitForFrames(2);

            // Test object should now be facing into the wall (i.e Z axis)
            Assert.IsTrue(Mathf.Approximately(1.0f, Vector3.Dot(targetTransform.forward.normalized, Vector3.forward)));
        }

        /// <summary>
        /// Test solver system's ability to change target types at runtime
        /// </summary>
        [UnityTest]
        public IEnumerator TestInBetween()
        {
            // Build "posts" to put solved object between
            var leftPost = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            leftPost.transform.position = Vector3.forward * 10.0f - Vector3.right * 10.0f;

            var rightPost = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            rightPost.transform.position = Vector3.forward * 10.0f + Vector3.right * 10.0f;

            // Instantiate our test GameObject with solver. 
            var testObjects = InstantiateTestSolver<InBetween>();

            testObjects.handler.TrackedTargetType = TrackedObjectType.CustomOverride;
            testObjects.handler.TransformOverride = leftPost.transform;

            InBetween inBetween = testObjects.solver as InBetween;
            Assert.IsNotNull(inBetween, "Solver cast to InBetween is null");

            inBetween.SecondTrackedObjectType = TrackedObjectType.CustomOverride;
            inBetween.SecondTransformOverride = rightPost.transform;

            // Let InBetween update
            yield return WaitForFrames(2);

            TestUtilities.AssertAboutEqual(testObjects.target.transform.position, Vector3.forward * 10.0f, "InBetween solver did not place object in middle of posts");

            inBetween.PartwayOffset = 0.0f;

            // Let InBetween update
            yield return WaitForFrames(2);

            TestUtilities.AssertAboutEqual(testObjects.target.transform.position, rightPost.transform.position, "InBetween solver did not move to the left post");
        }

        /// <summary>
        /// Test the HandConstraint to make sure it tracks hands correctly.
        /// </summary>
        [UnityTest]
        public IEnumerator TestHandConstraint()
        {
            // Instantiate our test GameObject with solver.
            var testObjects = InstantiateTestSolver<HandConstraint>();
            testObjects.handler.TrackedTargetType = TrackedObjectType.HandJoint;
            testObjects.handler.TrackedHandness = Handedness.Both;

            yield return new WaitForSeconds(SolverUpdateWaitTime);

            TestUtilities.AssertAboutEqual(testObjects.target.transform.position, Vector3.zero, "HandConstraint solver did not start at the origin");

            // Add a right hand.
            var rightHand = new TestHand(Handedness.Right);
            yield return rightHand.Show(Vector3.zero);

            // Move the hand to 0, 0, 1 and ensure that the hand constraint followed.
            var handPosition = Vector3.forward;
            yield return rightHand.MoveTo(handPosition);

            yield return new WaitForSeconds(SolverUpdateWaitTime);

            // Make sure the solver is not in the same location as the hand because the solver should move to a hand safe zone.
            TestUtilities.AssertNotAboutEqual(testObjects.target.transform.position, handPosition, "HandConstraint solver is in the same location of the hand when it should be slightly offset from the hand.");

            // Make sure the solver is near the hand.
            TestUtilities.AssertLessOrEqual(Vector3.Distance(testObjects.target.transform.position, handPosition), HandDistanceThreshold, "HandConstraint solver is not within {0} units of the hand", HandDistanceThreshold);

            // Hide the right hand and create a left hand.
            yield return rightHand.Hide();
            var leftHand = new TestHand(Handedness.Left);
            handPosition = Vector3.zero;
            yield return leftHand.Show(handPosition);

            yield return new WaitForSeconds(SolverUpdateWaitTime);

            // Make sure the solver is now near the other hand.
            TestUtilities.AssertLessOrEqual(Vector3.Distance(testObjects.target.transform.position, handPosition), HandDistanceThreshold, "HandConstraint solver is not within {0} units of the hand", HandDistanceThreshold);
        }

        /// <summary>
        /// Test the HandConstraintPalm up to make sure the FollowHandUntilFacingCamera behavior works as expected
        /// </summary>
        [UnityTest]
        public IEnumerator TestHandConstraintPalmUpSolverPlacement()
        {
            // Instantiate our test GameObject with solver.
            var testObjects = InstantiateTestSolver<HandConstraintPalmUp>();
            testObjects.handler.TrackedTargetType = TrackedObjectType.HandJoint;
            testObjects.handler.TrackedHandness = Handedness.Both;

            var handConstraintSolver = (HandConstraintPalmUp)testObjects.solver;
            handConstraintSolver.FollowHandUntilFacingCamera = true;
            handConstraintSolver.UseGazeActivation = false;

            // Ensure that FacingCameraTrackingThreshold is greater than FollowHandCameraFacingThresholdAngle
            Assert.AreEqual(handConstraintSolver.FacingCameraTrackingThreshold - handConstraintSolver.FollowHandCameraFacingThresholdAngle > 0, true);

            yield return new WaitForSeconds(SolverUpdateWaitTime);

            TestUtilities.AssertAboutEqual(testObjects.target.transform.position, Vector3.zero, "HandConstraintPalmUp solver did not start at the origin");

            var cameraTransform = CameraCache.Main.transform;

            // Place hand 1 meter in front of user, 50 cm below eye level
            var handTestPos = cameraTransform.position + cameraTransform.forward - (Vector3.up * 0.5f);

            var cameraLookVector = (handTestPos - cameraTransform.position).normalized;

            // Generate hand rotation with hand palm facing camera
            var handRoation = Quaternion.LookRotation(cameraTransform.up, cameraLookVector);

            // Add a right hand.
            var rightHand = new TestHand(Handedness.Right);
            yield return rightHand.Show(handTestPos);
            yield return rightHand.SetRotation(handRoation);

            yield return new WaitForSeconds(SolverUpdateWaitTime);

            // Ensure Rotation and offset behavior are following camera
            Assert.AreEqual(handConstraintSolver.RotationBehavior, HandConstraint.SolverRotationBehavior.LookAtMainCamera);
            Assert.AreEqual(handConstraintSolver.OffsetBehavior, HandConstraint.SolverOffsetBehavior.LookAtCameraRotation);

            // Rotate hand so palm is no longer within the FollowHandCameraFacingThresholdAngle
            var newHandRot = handRoation * Quaternion.Euler(-(handConstraintSolver.FollowHandCameraFacingThresholdAngle + 1), 0f, 0f);
            yield return rightHand.SetRotation(newHandRot);

            yield return new WaitForSeconds(SolverUpdateWaitTime);

            // Ensure Rotation and offset behavior are following camera
            Assert.AreEqual(handConstraintSolver.RotationBehavior, HandConstraint.SolverRotationBehavior.LookAtTrackedObject);
            Assert.AreEqual(handConstraintSolver.OffsetBehavior, HandConstraint.SolverOffsetBehavior.TrackedObjectRotation);

            yield return rightHand.Hide();

            yield return new WaitForSeconds(SolverUpdateWaitTime);
        }

        /// <summary>
        /// Test the HandConstraintPalm up to make sure the activation methods work as intended for the Ulnar safe zone
        /// </summary>
        [UnityTest]
        public IEnumerator TestHandConstraintPalmUpSolverActivationUlnar()
        {
            yield return TestHandConstraintPalmUpGazeActivationByZoneAndHand(HandConstraint.SolverSafeZone.UlnarSide, Handedness.Left);
            yield return TestHandConstraintPalmUpGazeActivationByZoneAndHand(HandConstraint.SolverSafeZone.UlnarSide, Handedness.Right);
        }

        /// <summary>
        /// Test the HandConstraintPalm up to make sure the activation methods work as intended for the Radial safe zone
        /// </summary>
        [UnityTest]
        public IEnumerator TestHandConstraintPalmUpSolverActivationRadial()
        {
            yield return TestHandConstraintPalmUpGazeActivationByZoneAndHand(HandConstraint.SolverSafeZone.RadialSide, Handedness.Left);
            yield return TestHandConstraintPalmUpGazeActivationByZoneAndHand(HandConstraint.SolverSafeZone.RadialSide, Handedness.Right);
        }

        /// <summary>
        /// Test the HandConstraintPalm up to make sure the activation methods work as intended for the BelowWrist safe zone
        /// </summary>
        [UnityTest]
        public IEnumerator TestHandConstraintPalmUpSolverActivationBelowWrist()
        {
            yield return TestHandConstraintPalmUpGazeActivationByZoneAndHand(HandConstraint.SolverSafeZone.BelowWrist, Handedness.Left);
            yield return TestHandConstraintPalmUpGazeActivationByZoneAndHand(HandConstraint.SolverSafeZone.BelowWrist, Handedness.Right);
        }

        /// <summary>
        /// Test the HandConstraintPalm up to make sure the activation methods work as intended for the AboveFingerTips safe zone
        /// </summary>
        [UnityTest]
        public IEnumerator TestHandConstraintPalmUpSolverActivationAboveFingerTips()
        {
            yield return TestHandConstraintPalmUpGazeActivationByZoneAndHand(HandConstraint.SolverSafeZone.AboveFingerTips, Handedness.Left);
            yield return TestHandConstraintPalmUpGazeActivationByZoneAndHand(HandConstraint.SolverSafeZone.AboveFingerTips, Handedness.Right);
        }

        /// <summary>
        /// Test the HandConstraintPalm up to make sure the activation methods work as intended for the AtopPalm safe zone
        /// </summary>
        [UnityTest]
        public IEnumerator TestHandConstraintPalmUpSolverActivationAtopPalm()
        {
            yield return TestHandConstraintPalmUpGazeActivationByZoneAndHand(HandConstraint.SolverSafeZone.AtopPalm, Handedness.Left);
            yield return TestHandConstraintPalmUpGazeActivationByZoneAndHand(HandConstraint.SolverSafeZone.AtopPalm, Handedness.Right);
        }

        /// <summary>
        /// Test the HandConstraintPalm up to make sure the FollowHandUntilFacingCamera behavior works as expected
        /// </summary>
        [UnityTest]
        public IEnumerator TestHandConstraintPalmUpSolverReattach()
        {

            // Instantiate our test GameObject with solver.
            var testObjects = InstantiateTestSolver<HandConstraintPalmUp>();
            testObjects.handler.TrackedTargetType = TrackedObjectType.HandJoint;
            testObjects.handler.TrackedHandness = Handedness.Both;

            var manipHandler = testObjects.target.AddComponent<ManipulationHandler>();
            manipHandler.HostTransform = testObjects.target.transform;
            manipHandler.SmoothingActive = false;
            manipHandler.OnManipulationStarted.AddListener((eventData) => testObjects.handler.UpdateSolvers = false);
            // add near interaction grabbable to be able to grab the cube with the simulated articulated hand
            testObjects.target.AddComponent<NearInteractionGrabbable>();

            var boxCollider = testObjects.target.AddComponent<BoxCollider>();
            boxCollider.size = new Vector3(.5f, .5f, .5f);

            var handConstraintSolver = (HandConstraintPalmUp)testObjects.solver;
            handConstraintSolver.FollowHandUntilFacingCamera = true;
            handConstraintSolver.UseGazeActivation = true;
            manipHandler.OnManipulationEnded.AddListener((eventData) => handConstraintSolver.StartWorldLockReattachCheckCoroutine());

            // Ensure that FacingCameraTrackingThreshold is greater than FollowHandCameraFacingThresholdAngle
            Assert.AreEqual(handConstraintSolver.FacingCameraTrackingThreshold - handConstraintSolver.FollowHandCameraFacingThresholdAngle > 0, true);

            yield return null;

            TestUtilities.AssertAboutEqual(testObjects.target.transform.position, Vector3.zero, "HandConstraintPalmUp solver did not start at the origin");

            var cameraTransform = CameraCache.Main.transform;

            // Place hand 1 meter in front of user, 50 cm below eye level
            var handTestPos = cameraTransform.position + cameraTransform.forward + DetermineHandOriginPositionOffset(HandConstraint.SolverSafeZone.UlnarSide, Handedness.Left);

            var cameraLookVector = (handTestPos - cameraTransform.position).normalized;

            // Generate hand rotation with hand palm facing camera
            var handRotation = Quaternion.LookRotation(cameraTransform.up, cameraLookVector);

            // Add a left hand, then  a right hand.
            var leftHand = new TestHand(Handedness.Left);
            yield return leftHand.Show(handTestPos);
            yield return null;

            yield return leftHand.SetRotation(handRotation);

            TestHand rightHand = new TestHand(Handedness.Right);
            yield return rightHand.Show(new Vector3(0, 0, 0.5f));
            yield return null;

            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.OpenSteadyGrabPoint);
            yield return rightHand.Move(testObjects.target.transform.position);
            yield return null;

            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Pinch);
            yield return new WaitForFixedUpdate();
            yield return null;


            var delta = new Vector3(0.5f, 0.5f, 0f);
            yield return rightHand.Move(delta);

            // Grab the menu position to compare it later on
            Vector3 menuPosition = testObjects.target.transform.position;
            Vector3 movedLeftHand = handTestPos - Vector3.right;

            // Move the left hand so it doesn't immediately snap
            yield return leftHand.Move(movedLeftHand);

            yield return null;

            yield return rightHand.SetGesture(ArticulatedHandPose.GestureId.Open);

            yield return null;

            // Before the right hand opens, make sure that the transform of the attached menu is farther than it would if attached
            Assert.IsTrue((testObjects.target.transform.position - movedLeftHand).sqrMagnitude > .1f);

            // Then move the left hand back to the point of activation
            yield return leftHand.Move(handTestPos);
            yield return leftHand.SetRotation(handRotation);
            yield return null;

            // Then move the hand back and see if the attached menu follows
            Assert.IsTrue(testObjects.handler.UpdateSolvers, "Did not properly reattach; UpdateSolver has not been updated to true");

            yield return rightHand.Hide();
            yield return leftHand.Hide();

            yield return null;
        }

        /// <summary>
        /// Test the Overlap solver and make sure it tracks the left simulated hand exactly
        /// </summary>
        [UnityTest]
        public IEnumerator TestOverlap()
        {
            // Instantiate our test GameObject with solver.
            var testObjects = InstantiateTestSolver<Overlap>();
            testObjects.handler.TrackedTargetType = TrackedObjectType.HandJoint;
            var targetTransform = testObjects.target.transform;

            TestUtilities.AssertAboutEqual(targetTransform.position, Vector3.zero, "Overlap not at original position");
            TestUtilities.AssertAboutEqual(targetTransform.rotation, Quaternion.identity, "Overlap not at original rotation");

            // Test that the solver flies to the position of the left hand
            var handPosition = Vector3.forward - Vector3.right;
            var handRotation = Quaternion.LookRotation(handPosition);
            var leftHand = new TestHand(Handedness.Left);
            yield return leftHand.Show(handPosition);
            yield return leftHand.SetRotation(handRotation);

            yield return WaitForFrames(2);
            var hand = PlayModeTestUtilities.GetInputSimulationService().GetControllerDevice(Handedness.Left) as SimulatedHand;
            Assert.IsNotNull(hand);
            Assert.IsTrue(hand.TryGetJoint(TrackedHandJoint.Palm, out MixedRealityPose pose));

            TestUtilities.AssertAboutEqual(targetTransform.position, pose.Position, "Overlap solver is not at the same position as the left hand.");
            Assert.IsTrue(Quaternion.Angle(targetTransform.rotation, pose.Rotation) < 2.0f);

            // Make sure the solver did not move when hand was hidden
            yield return leftHand.Hide();
            yield return WaitForFrames(2);
            TestUtilities.AssertAboutEqual(targetTransform.position, pose.Position, "Overlap solver moved when the hand was hidden.");
            Assert.IsTrue(Quaternion.Angle(targetTransform.rotation, pose.Rotation) < 2.0f);
        }

        /// <summary>
        /// Test solver system's ability to add multiple solvers at runtime and switch between them.
        /// </summary>
        [UnityTest]
        public IEnumerator TestSolverSwap()
        {
            // Reset view to origin
            MixedRealityPlayspace.PerformTransformation(p =>
            {
                p.position = Vector3.zero;
                p.LookAt(Vector3.forward);
            });

            // Instantiate and setup RadialView to place object in the view center.
            var testObjects = InstantiateTestSolver<RadialView>();
            RadialView radialViewSolver = (RadialView)testObjects.solver;
            radialViewSolver.MinDistance = 2.0f;
            radialViewSolver.MaxDistance = 2.0f;
            radialViewSolver.MinViewDegrees = 0.0f;
            radialViewSolver.MaxViewDegrees = 0.0f;

            // Let RadialView update the target object
            yield return WaitForFrames(2);

            // Make sure Radial View is placing object in center of View, so we can later check that a solver swap actually moved the target object.
            TestUtilities.AssertAboutEqual(testObjects.target.transform.position, Vector3.forward * 2.0f, "RadialView does not place object in center of view");

            // Disable the old solver
            radialViewSolver.enabled = false;

            // Add a another solver during runtime, give him a specific location to check whether the new solver updates the target object.
            Orbital orbitalSolver = AddSolverComponent<Orbital>(testObjects.target);
            orbitalSolver.WorldOffset = Vector3.zero;
            orbitalSolver.LocalOffset = Vector3.down * 2.0f;

            // Let Orbital update the target object
            yield return WaitForFrames(2);

            // Make sure Orbital is now updating the target object
            TestUtilities.AssertAboutEqual(testObjects.target.transform.position, Vector3.down * 2.0f, "Orbital solver did not place object below origin");

            // Swap solvers once again during runtime
            radialViewSolver.enabled = true;
            orbitalSolver.enabled = false;

            // Let RadialView update the target object
            yield return WaitForFrames(2);

            // Make sure Radial View is now updating the target object once again.
            TestUtilities.AssertAboutEqual(testObjects.target.transform.position, Vector3.forward * 2.0f, "RadialView solver did not place object in center of view");
        }

        #region TapToPlace Tests
        /// <summary>
        /// Test the default behavior for Tap to Place.  The default behavior has the target object following the head.
        /// </summary>
        [UnityTest]
        public IEnumerator TestTapToPlaceOnClickHead()
        {
            TestUtilities.PlayspaceToOriginLookingForward();

            // Create a cube with Tap to Place attached and Head (default) as the TrackedTargetType 
            var tapToPlaceObj = InstantiateTestSolver<TapToPlace>();
            tapToPlaceObj.target.transform.position = Vector3.forward;
            TapToPlace tapToPlace = tapToPlaceObj.solver as TapToPlace;

            // Set hand position 
            Vector3 handStartPosition = new Vector3(0, -0.1f, 0.6f);
            var leftHand = new TestHand(Handedness.Left);
            yield return leftHand.Show(handStartPosition);

            // Select Tap to Place Obj
            yield return leftHand.Click();

            // Make sure the object is being placed
            Assert.True(tapToPlace.IsBeingPlaced);

            // Move the playspace to simulate head movement
            MixedRealityPlayspace.PerformTransformation(p =>
            {
                p.position = Vector3.left * 1.5f;
            });

            yield return new WaitForFixedUpdate();
            yield return null;

            // Make sure the target obj has followed the head
            Assert.AreEqual(CameraCache.Main.transform.position.x, tapToPlaceObj.target.transform.position.x, 1.0e-5f, "The tap to place object position.x does not match the camera position.x");

            // Tap to place has a 0.5 sec timer between clicks to make sure a double click does not get registered
            // We need to wait at least 0.5 secs until another click is called or tap to place will ignore the action
            yield return new WaitForSeconds(0.5f);

            // Click object to stop placement
            yield return leftHand.Click();

            // Make sure the object is not being placed after the click
            Assert.False(tapToPlace.IsBeingPlaced);

            // Move the playspace to simulate head movement again
            MixedRealityPlayspace.PerformTransformation(p =>
            {
                p.position = Vector3.right;
            });

            yield return new WaitForFixedUpdate();
            yield return null;

            // Make sure the target obj is NOT following the head
            Assert.AreNotEqual(CameraCache.Main.transform.position.x, tapToPlaceObj.target.transform.position.x, "The tap to place object position.x matches camera position.x, when it should not");
        }

        /// <summary>
        /// Test the controller ray as the Tracked Target Type for an object with tap to place attached.
        /// </summary>
        [UnityTest]
        public IEnumerator TestTapToPlaceOnClickControllerRay()
        {
            TestUtilities.PlayspaceToOriginLookingForward();

            // Create a cube with Tap to Place attached
            var tapToPlaceObj = InstantiateTestSolver<TapToPlace>();
            tapToPlaceObj.target.transform.position = Vector3.forward;
            TapToPlace tapToPlace = tapToPlaceObj.solver as TapToPlace;

            // Switch the TrackedTargetType to Controller Ray
            SolverHandler tapToPlaceSolverHandler = tapToPlaceObj.handler;
            tapToPlaceSolverHandler.TrackedTargetType = TrackedObjectType.ControllerRay;

            // Set hand position
            Vector3 handStartPosition = new Vector3(0, -0.1f, 0.6f);
            var leftHand = new TestHand(Handedness.Left);
            yield return leftHand.Show(handStartPosition);

            Vector3 initialObjPosition = tapToPlaceObj.target.transform.position;

            yield return leftHand.Click();

            // Make sure the object is being placed after selection
            Assert.True(tapToPlace.IsBeingPlaced);

            // Move hand, object should follow
            yield return leftHand.Move(Vector3.forward);
            yield return leftHand.Move(Vector3.up);

            // Make sure the object starting position is different from the current position
            Assert.True(initialObjPosition != tapToPlaceObj.target.transform.position);

            // Tap to place has a 0.5 sec timer between clicks to make sure a double click does not get registered
            // We need to wait at least 0.5 secs until another click is called or tap to place will ignore the action
            yield return new WaitForSeconds(0.5f);

            // Click to stop the placement
            yield return leftHand.Click();

            // Make sure the object is not being placed
            Assert.False(tapToPlace.IsBeingPlaced);

            // Get new position of the object after it is placed
            Vector3 newPosition = tapToPlaceObj.target.transform.position;

            // Move hand, the object should NOT move
            yield return leftHand.Move(Vector3.back, 30);

            Assert.True(newPosition == tapToPlaceObj.target.transform.position);
        }

        /// <summary>
        /// Test code configurability for tap to place. Events for tap to place by default are triggered by 
        /// OnPointerClicked.  Code configurability for tap to place is calling Start/Stop Placement instead of 
        /// clicking to select an object.  This test uses AutoStart to start placing the object and StopPlacement.
        /// </summary>
        [UnityTest]
        public IEnumerator TestTapToPlaceCodeConfigurability()
        {
            TestUtilities.PlayspaceToOriginLookingForward();

            // Create a cube with Tap to Place attached
            var tapToPlaceObj = InstantiateTestSolver<TapToPlace>();
            tapToPlaceObj.target.transform.position = Vector3.forward;
            TapToPlace tapToPlace = tapToPlaceObj.solver as TapToPlace;

            // Start Placing the object immediately
            tapToPlace.AutoStart = true;

            Vector3 handStartPosition = new Vector3(0, -0.1f, 0.6f);
            var leftHand = new TestHand(Handedness.Left);
            yield return leftHand.Show(handStartPosition);

            // Make sure the object is being placed after setting AutoStart
            Assert.True(tapToPlace.IsBeingPlaced);

            // Move the playspace to simulate head movement
            MixedRealityPlayspace.PerformTransformation(p =>
            {
                p.position = Vector3.left * 1.5f;
            });

            yield return new WaitForFixedUpdate();
            yield return null;

            // Make sure the target obj is following the head
            Assert.AreEqual(CameraCache.Main.transform.position.x, tapToPlaceObj.target.transform.position.x, 1.0e-5f, "The tap to place object position.x does not match the camera position.x");

            // Stop the placement via code instead of click from the hand
            tapToPlace.StopPlacement();

            Assert.False(tapToPlace.IsBeingPlaced);

            // Move the playspace to simulate head movement again
            MixedRealityPlayspace.PerformTransformation(p =>
            {
                p.position = Vector3.right;
            });

            yield return new WaitForFixedUpdate();
            yield return null;

            // Make sure the target obj is NOT following the head
            Assert.AreNotEqual(CameraCache.Main.transform.position.x, tapToPlaceObj.target.transform.position.x, "The tap to place object position.x matches the camera position.x, when it should not");
        }

        /// <summary>
        /// Tests tap to place object placement if there is a surface hit on another collider through Start/StopPlacement calls
        /// instead of OnPointerClicked.
        /// </summary>
        [UnityTest]
        public IEnumerator TestTapToPlaceColliderTests()
        {
            TestUtilities.PlayspaceToOriginLookingForward();

            // Create a scene with 2 cubes
            GameObject colliderObj1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            colliderObj1.transform.localScale = new Vector3(0.3f, 0.3f, 0.05f);
            colliderObj1.transform.position = new Vector3(0.3f, 0, 1.5f);

            GameObject colliderObj2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            colliderObj2.transform.localScale = new Vector3(0.3f, 0.3f, 0.05f);
            colliderObj2.transform.position = new Vector3(-0.3f, 0, 1.5f);

            // Create a cube with Tap to Place attached
            var tapToPlaceObj = InstantiateTestSolver<TapToPlace>();
            tapToPlaceObj.target.transform.position = Vector3.forward * 2;
            TapToPlace tapToPlace = tapToPlaceObj.solver as TapToPlace;

            // Switching the TrackedTargetType to Controller Ray
            SolverHandler tapToPlaceSolverHandler = tapToPlaceObj.handler;
            tapToPlaceSolverHandler.TrackedTargetType = TrackedObjectType.ControllerRay;

            Vector3 handStartPosition = new Vector3(0, -0.15f, 0.5f);
            var leftHand = new TestHand(Handedness.Left);
            yield return leftHand.Show(handStartPosition);

            // Start the placement via code instead of click from the hand
            tapToPlace.StartPlacement();
            yield return null;

            Assert.True(tapToPlace.IsBeingPlaced);

            // Move hand, object should follow
            yield return leftHand.Move(new Vector3(-0.15f, 0, 0), 30);
            Assert.True(tapToPlaceObj.target.transform.position.z < colliderObj1.transform.position.z);

            yield return leftHand.Move(new Vector3(0.15f, 0, 0), 30);
            Assert.True(tapToPlaceObj.target.transform.position.z > colliderObj1.transform.position.z);

            yield return leftHand.Move(new Vector3(0.15f, 0, 0), 30);
            Assert.True(tapToPlaceObj.target.transform.position.z < colliderObj1.transform.position.z);

            // Stop the placement via code instead of click from the hand
            tapToPlace.StopPlacement();

            Assert.False(tapToPlace.IsBeingPlaced);
        }

        /// <summary>
        /// Tests the UseDefaultSurfaceNormalOffset property for Tap to Place while the object is in the placing state. If the 
        /// UseDefaultSurfaceNormalOffset is true, the object should appear flat against a collider. If false, the object will
        /// have an offset based on SurfaceNormalOffset.
        /// </summary>
        [UnityTest]
        public IEnumerator TestTapToPlaceSurfaceNormalOffsetSet()
        {
            TestUtilities.PlayspaceToOriginLookingForward();

            // Create a scene with 2 cubes
            GameObject colliderObj1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            colliderObj1.transform.localScale = new Vector3(0.3f, 0.3f, 0.05f);
            colliderObj1.transform.position = new Vector3(0.3f, 0, 1.5f);

            GameObject colliderObj2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            colliderObj2.transform.localScale = new Vector3(0.3f, 0.3f, 0.05f);
            colliderObj2.transform.position = new Vector3(-0.3f, 0, 1.5f);

            // Create a cube with Tap to Place attached
            var tapToPlaceObj = InstantiateTestSolver<TapToPlace>();
            tapToPlaceObj.target.transform.position = Vector3.forward * 2;
            tapToPlaceObj.target.transform.localScale = Vector3.one * 0.2f;
            TapToPlace tapToPlace = tapToPlaceObj.solver as TapToPlace;

            // Switching the TrackedTargetType to Controller Ray
            SolverHandler tapToPlaceSolverHandler = tapToPlaceObj.handler;
            tapToPlaceSolverHandler.TrackedTargetType = TrackedObjectType.ControllerRay;

            Vector3 handStartPosition = new Vector3(0, -0.15f, 0.5f);
            var leftHand = new TestHand(Handedness.Left);
            yield return leftHand.Show(handStartPosition);

            tapToPlace.KeepOrientationVertical = true;
            tapToPlace.RotateAccordingToSurface = true;

            // Switch off UseDefaultSurfaceNormalOffset, this shifts the current surface normal offset value to SurfaceNormalOffset
            tapToPlace.UseDefaultSurfaceNormalOffset = false;

            // Make sure the SurfaceNormalOffset is not the default z extents of the bounds
            Assert.AreNotEqual(tapToPlace.SurfaceNormalOffset, tapToPlaceObj.target.GetComponent<Collider>().bounds.extents.z);

            // Start the placement via code instead of click from the hand
            tapToPlace.StartPlacement();
            yield return null;

            Assert.True(tapToPlace.IsBeingPlaced);

            // Move hand in front of a collider for surface detection, the Tap to Place object should follow
            yield return leftHand.Move(new Vector3(-0.15f, 0, 0), 30);

            // Make sure the depth of the Tap to Place Object is very close to the depth of the wall because the SurfaceNormalOffset is 0
            Assert.AreEqual(tapToPlaceObj.target.transform.position.z, colliderObj1.transform.position.z, 0.05f);

            // Move hand between the colliders, the Tap to Place object should have a greater z position because the raycast did not detect a surface
            yield return leftHand.Move(new Vector3(0.15f, 0, 0), 30);
            Assert.True(tapToPlaceObj.target.transform.position.z > colliderObj1.transform.position.z);

            // Move the hand in front of a collider for a surface detection
            yield return leftHand.Move(new Vector3(0.15f, 0, 0), 30);

            // Set the UseDefaultSurfaceNormalOffset to true while still in the placing state
            tapToPlace.UseDefaultSurfaceNormalOffset = true;
            yield return null;

            // Make sure the depth of the Tap to Place Object is less than the depth of the wall because UseDefaultSurfaceNormalOffset is true
            Assert.True(tapToPlaceObj.target.transform.position.z < colliderObj1.transform.position.z);

            // Stop the placement via code instead of click from the hand
            tapToPlace.StopPlacement();

            Assert.False(tapToPlace.IsBeingPlaced);
        }

        /// <summary>
        /// Tests the functionality of StartPlacement() when called before Start() is called. In this case, StartPlacement should
        /// do its normal job after Start() is called.
        /// </summary>
        [UnityTest]
        public IEnumerator TestTapToPlaceStartPlacementBeforeStart()
        {
            TestUtilities.PlayspaceToOriginLookingForward();

            // Create an inactive cube with Tap to Place attached
            var tapToPlaceObj = InstantiateTestSolver<TapToPlace>(false);
            TapToPlace tapToPlace = tapToPlaceObj.solver as TapToPlace;

            // Call StartPlament() before its Start() is called
            tapToPlace.StartPlacement();

            // Make sure it is not in beingPlace state
            Assert.False(tapToPlace.IsBeingPlaced);

            // Set the cube to active which causes Start() to be called
            tapToPlaceObj.target.SetActive(true);

            // Wait until the next frame
            yield return null;

            // Make sure it is now in beingPlace state
            Assert.True(tapToPlace.IsBeingPlaced);
        }

        /// <summary>
        /// Tests the functionality of StartPlacement() when called after Start() is called. In this case, StartPlacement should
        /// do its normal job.
        /// </summary>
        [UnityTest]
        public IEnumerator TestTapToPlaceStartPlacementAfterStart()
        {
            TestUtilities.PlayspaceToOriginLookingForward();

            // Create an active cube with Tap to Place attached
            var tapToPlaceObj = InstantiateTestSolver<TapToPlace>();
            TapToPlace tapToPlace = tapToPlaceObj.solver as TapToPlace;

            // Wait until the next frame
            yield return null;

            // Call StartPlament() after its Start() is called
            tapToPlace.StartPlacement();

            // Make sure it is now in beingPlace state
            Assert.True(tapToPlace.IsBeingPlaced);
        }

        #endregion

        #region Experimental

        /// <summary>
        /// Tests that the DirectionalIndicator can be instantiated through code.
        /// </summary>
        [UnityTest]
        public IEnumerator TestDirectionalIndicator()
        {
            // Reset view to origin
            TestUtilities.PlayspaceToOriginLookingForward();

            const float ANGLE_THRESHOLD = 30.0f;

            var directionTarget = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            directionTarget.transform.position = 10.0f * Vector3.right;

            // Instantiate our test gameobject with solver.
            var testObjects = InstantiateTestSolver<DirectionalIndicator>();

            var indicatorSolver = testObjects.solver as DirectionalIndicator;
            indicatorSolver.DirectionalTarget = directionTarget.transform;

            var indicatorMesh = indicatorSolver.GetComponent<Renderer>();

            // Test that solver points to the right and is visible
            yield return WaitForFrames(2);
            TestUtilities.AssertLessOrEqual(Vector3.Angle(indicatorSolver.transform.up, directionTarget.transform.position.normalized), ANGLE_THRESHOLD);
            Assert.IsTrue(indicatorMesh.enabled);

            directionTarget.transform.position = -10.0f * Vector3.right;

            // Test that solver points to the left now and is visible
            yield return WaitForFrames(2);
            TestUtilities.AssertLessOrEqual(Vector3.Angle(indicatorSolver.transform.up, directionTarget.transform.position.normalized), ANGLE_THRESHOLD);
            Assert.IsTrue(indicatorMesh.enabled);

            // Test that the solver is invisible
            directionTarget.transform.position = 5.0f * Vector3.forward;

            yield return WaitForFrames(2);
            Assert.IsFalse(indicatorMesh.enabled);

            // Get back to a position where the directional indicator should be visible
            directionTarget.transform.position = -10.0f * Vector3.right;
            yield return WaitForFrames(2);
            TestUtilities.AssertLessOrEqual(Vector3.Angle(indicatorSolver.transform.up, directionTarget.transform.position.normalized), ANGLE_THRESHOLD);
            Assert.IsTrue(indicatorMesh.enabled);

            // Check that the solver is near the max scale when turned away from the target
            directionTarget.transform.position = 10.0f * Vector3.back;
            yield return WaitForFrames(2);
            TestUtilities.AssertAboutEqual(indicatorSolver.transform.lossyScale, indicatorSolver.MaxIndicatorScale * Vector3.one, "Not at max indicator size");


            // Check that the solver is smaller when the target is closer to the cameras FOV
            directionTarget.transform.position = 2.0f * Vector3.right + 1.0f * Vector3.forward;
            yield return WaitForFrames(2);
            TestUtilities.AssertLessOrEqual(indicatorSolver.transform.lossyScale.magnitude, ((indicatorSolver.MinIndicatorScale + indicatorSolver.MaxIndicatorScale * 0.5f) * Vector3.one).magnitude, "Not smaller than the average of the indicator size range");

            // Destroy the object and then validate that the mesh is no longer visible
            Object.Destroy(directionTarget);
            yield return null;
            Assert.IsFalse(indicatorMesh.enabled);
        }

        /// <summary>
        /// Test the Follow solver distance clamp options
        /// </summary>
        [UnityTest]
        public IEnumerator TestFollowDistance()
        {
            const float followWaitTime = 0.1f;

            // Reset view to origin
            TestUtilities.PlayspaceToOriginLookingForward();

            // Instantiate our test GameObject with solver.
            var testObjects = InstantiateTestSolver<Follow>();
            var followSolver = (Follow)testObjects.solver;
            testObjects.handler.TrackedTargetType = TrackedObjectType.Head;
            var targetTransform = testObjects.target.transform;

            yield return new WaitForFixedUpdate();
            yield return new WaitForSeconds(followWaitTime);

            // Test distance remains within min/max bounds
            float distanceToHead = Vector3.Distance(targetTransform.position, CameraCache.Main.transform.position);
            TestUtilities.AssertLessOrEqual(distanceToHead, followSolver.MaxDistance, "Follow exceeded max distance");
            TestUtilities.AssertGreaterOrEqual(distanceToHead, followSolver.MinDistance, "Follow subceeded min distance");

            MixedRealityPlayspace.PerformTransformation(p =>
            {
                p.position = Vector3.back * 2;
            });

            yield return new WaitForFixedUpdate();
            yield return new WaitForSeconds(followWaitTime);

            distanceToHead = Vector3.Distance(targetTransform.position, CameraCache.Main.transform.position);
            TestUtilities.AssertLessOrEqual(distanceToHead, followSolver.MaxDistance, "Follow exceeded max distance");
            TestUtilities.AssertGreaterOrEqual(distanceToHead, followSolver.MinDistance, "Follow subceeded min distance");

            MixedRealityPlayspace.PerformTransformation(p =>
            {
                p.position = Vector3.forward * 4;
            });

            yield return new WaitForFixedUpdate();
            yield return new WaitForSeconds(followWaitTime);

            distanceToHead = Vector3.Distance(targetTransform.position, CameraCache.Main.transform.position);
            TestUtilities.AssertLessOrEqual(distanceToHead, followSolver.MaxDistance, "Follow exceeded max distance");
            TestUtilities.AssertGreaterOrEqual(distanceToHead, followSolver.MinDistance, "Follow subceeded min distance");

            // Test VerticalMaxDistance
            followSolver.VerticalMaxDistance = 0.1f;
            targetTransform.position = Vector3.forward;
            targetTransform.rotation = Quaternion.identity;
            MixedRealityPlayspace.PerformTransformation(p =>
            {
                p.position = Vector3.zero;
                p.LookAt(Vector3.forward);
            });

            yield return new WaitForFixedUpdate();
            yield return new WaitForSeconds(followWaitTime);

            MixedRealityPlayspace.PerformTransformation(p =>
            {
                p.LookAt(Vector3.forward + Vector3.up);
            });

            yield return new WaitForFixedUpdate();
            yield return new WaitForSeconds(followWaitTime);

            float yDistance = targetTransform.position.y - CameraCache.Main.transform.position.y;
            Assert.AreEqual(followSolver.VerticalMaxDistance, yDistance);

            followSolver.VerticalMaxDistance = 0f;
        }

        /// <summary>
        /// Test the Follow solver orientation options
        /// </summary>
        [UnityTest]
        public IEnumerator TestFollowOrientation()
        {
            // Instantiate our test GameObject with solver.
            var testObjects = InstantiateTestSolver<Follow>();
            var followSolver = (Follow)testObjects.solver;
            testObjects.handler.TrackedTargetType = TrackedObjectType.Head;
            var targetTransform = testObjects.target.transform;

            // Test orientation deadzone
            followSolver.OrientToControllerDeadzoneDegrees = 70;
            MixedRealityPlayspace.PerformTransformation(p =>
            {
                p.position = Vector3.back;
                p.LookAt(Vector3.forward);
            });

            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.AreEqual(targetTransform.rotation, Quaternion.identity, "Target rotated before we moved beyond the deadzone");

            MixedRealityPlayspace.PerformTransformation(p => p.RotateAround(Vector3.zero, Vector3.up, 90));
            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.AreNotEqual(targetTransform.rotation, Quaternion.identity, "Target did not rotate after we moved beyond the deadzone");

            // Test FaceUserDefinedTargetTransform
            var hand = new TestHand(Handedness.Right);
            yield return hand.Show(Vector3.forward + Vector3.right);
            testObjects.handler.TrackedTargetType = TrackedObjectType.HandJoint;
            followSolver.FaceUserDefinedTargetTransform = true;
            followSolver.TargetToFace = CameraCache.Main.transform;

            TestUtilities.AssertAboutEqual(Quaternion.LookRotation(targetTransform.position - CameraCache.Main.transform.position), targetTransform.rotation, "Target expected to be facing camera.");

            yield return hand.MoveTo(Vector3.forward + Vector3.left, 1);
            yield return null;

            TestUtilities.AssertAboutEqual(Quaternion.LookRotation(targetTransform.position - CameraCache.Main.transform.position), targetTransform.rotation, "Target expected to be facing camera.");
        }

        /// <summary>
        /// Test the Follow solver angular clamp options
        /// </summary>
        [UnityTest]
        public IEnumerator TestFollowDirection()
        {
            // Instantiate our test GameObject with solver.
            var testObjects = InstantiateTestSolver<Follow>();
            var followSolver = (Follow)testObjects.solver;
            testObjects.handler.TrackedTargetType = TrackedObjectType.Head;
            var targetTransform = testObjects.target.transform;

            // variables and lambdas to test direction remains within bounds
            var maxXAngle = followSolver.MaxViewHorizontalDegrees / 2;
            var maxYAngle = followSolver.MaxViewVerticalDegrees / 2;
            Vector3 directionToHead() => CameraCache.Main.transform.position - targetTransform.position;
            float xAngle() => (Mathf.Acos(Vector3.Dot(directionToHead(), targetTransform.right)) * Mathf.Rad2Deg) - 90;
            float yAngle() => 90 - (Mathf.Acos(Vector3.Dot(directionToHead(), targetTransform.up)) * Mathf.Rad2Deg);

            // Test without rotation
            TestUtilities.PlayspaceToOriginLookingForward();

            yield return new WaitForFixedUpdate();
            yield return null;

            TestUtilities.AssertLessOrEqual(Mathf.Abs(xAngle()), maxXAngle, "Follow exceeded the max horizontal angular bounds");
            TestUtilities.AssertLessOrEqual(Mathf.Abs(yAngle()), maxYAngle, "Follow exceeded the max vertical angular bounds");

            // Test y axis rotation
            MixedRealityPlayspace.PerformTransformation(p => p.Rotate(Vector3.up, 45));
            yield return new WaitForFixedUpdate();
            yield return null;

            TestUtilities.AssertLessOrEqual(Mathf.Abs(xAngle()), maxXAngle, "Follow exceeded the max horizontal angular bounds");
            TestUtilities.AssertLessOrEqual(Mathf.Abs(yAngle()), maxYAngle, "Follow exceeded the max vertical angular bounds");

            // Test x axis rotation
            MixedRealityPlayspace.PerformTransformation(p => p.Rotate(Vector3.right, 45));
            yield return new WaitForFixedUpdate();
            yield return null;

            TestUtilities.AssertLessOrEqual(Mathf.Abs(xAngle()), maxXAngle, "Follow exceeded the max horizontal angular bounds");
            TestUtilities.AssertLessOrEqual(Mathf.Abs(yAngle()), maxYAngle, "Follow exceeded the max vertical angular bounds");

            // Test translation
            MixedRealityPlayspace.PerformTransformation(p => p.Translate(Vector3.back, Space.World));
            yield return new WaitForFixedUpdate();
            yield return null;

            TestUtilities.AssertLessOrEqual(Mathf.Abs(xAngle()), maxXAngle, "Follow exceeded the max horizontal angular bounds");
            TestUtilities.AssertLessOrEqual(Mathf.Abs(yAngle()), maxYAngle, "Follow exceeded the max vertical angular bounds");

            // Test renderer bounds clamp mode.
            followSolver.AngularClampMode = Follow.AngularClampType.RendererBounds;
            MixedRealityPlayspace.PerformTransformation(p => p.Rotate(Vector3.up, 180));
            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.Greater(Vector3.Dot(targetTransform.position - CameraCache.Main.transform.position, CameraCache.Main.transform.forward), 0.0f, "Follow did not clamp angle when using AngularClampType.RendererBounds.");

            // Test collider bounds clamp mode.
            followSolver.AngularClampMode = Follow.AngularClampType.ColliderBounds;
            MixedRealityPlayspace.PerformTransformation(p => p.Rotate(Vector3.up, 0.0f));
            yield return new WaitForFixedUpdate();
            yield return null;

            Assert.Greater(Vector3.Dot(targetTransform.position - CameraCache.Main.transform.position, CameraCache.Main.transform.forward), 0.0f, "Follow did not clamp angle when using AngularClampType.ColliderBounds.");
        }

        /// <summary>
        /// Test the Follow solver angular clamp options
        /// </summary>
        [UnityTest]
        public IEnumerator TestFollowStuckBehind()
        {
            const float followWaitTime = 0.1f;

            // Instantiate our test GameObject with solver.
            var testObjects = InstantiateTestSolver<Follow>();
            var followSolver = (Follow)testObjects.solver;
            testObjects.handler.TrackedTargetType = TrackedObjectType.Head;
            var targetTransform = testObjects.target.transform;

            // variables and lambdas to test direction remains within bounds
            Vector3 toTarget() => targetTransform.position - CameraCache.Main.transform.position;

            // Test without rotation
            TestUtilities.PlayspaceToOriginLookingForward();

            yield return new WaitForFixedUpdate();
            yield return new WaitForSeconds(followWaitTime);

            Assert.Greater(Vector3.Dot(CameraCache.Main.transform.forward, toTarget()), 0, "Follow behind the player");

            // Test y axis rotation
            MixedRealityPlayspace.PerformTransformation(p => p.Rotate(Vector3.up, 180));
            yield return new WaitForFixedUpdate();
            yield return new WaitForSeconds(followWaitTime);

            Assert.Greater(Vector3.Dot(CameraCache.Main.transform.forward, toTarget()), 0, "Follow behind the player");
        }

        #endregion

        #region Test Helpers

        private IEnumerator TestHandSolver(SetupData testData, InputSimulationService inputSimulationService, Vector3 handPos, Handedness hand)
        {
            Assert.IsTrue(testData.handler.TrackedTargetType == TrackedObjectType.ControllerRay
                || testData.handler.TrackedTargetType == TrackedObjectType.HandJoint, "TestHandSolver supports on ControllerRay and HandJoint tracked target types");

            yield return PlayModeTestUtilities.ShowHand(hand, inputSimulationService, Utilities.ArticulatedHandPose.GestureId.Open, handPos);

            // Give time for cube to float to hand
            yield return WaitForFrames(2);

            Vector3 handOrbitalPos = testData.target.transform.position;
            TestUtilities.AssertLessOrEqual(Vector3.Distance(handOrbitalPos, handPos), DistanceThreshold);

            Transform expectedTransform = null;
            if (testData.handler.TrackedTargetType == TrackedObjectType.ControllerRay)
            {
                LinePointer pointer = PointerUtils.GetPointer<LinePointer>(hand);
                expectedTransform = (pointer != null) ? pointer.transform : null;
            }
            else
            {
                var handJointService = CoreServices.GetInputSystemDataProvider<IMixedRealityHandJointService>();
                expectedTransform = handJointService.RequestJointTransform(testData.handler.TrackedHandJoint, hand);
            }

            Assert.AreEqual(testData.handler.CurrentTrackedHandedness, hand);
            Assert.IsNotNull(expectedTransform);

            // SolverHandler creates a dummy GameObject to provide a transform for tracking so it can be managed (allocated/deleted)
            // Look at the parent to compare transform equality for what we should be tracking against
            Assert.AreEqual(testData.handler.TransformTarget.parent, expectedTransform);

            yield return PlayModeTestUtilities.HideHand(Handedness.Right, inputSimulationService);

            yield return WaitForFrames(2);
        }

        private SetupData InstantiateTestSolver<T>(bool setGameObjectActive = true) where T : Solver
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = typeof(T).Name;
            cube.transform.localScale = new Vector3(0.1f, 0.2f, 0.1f);
            cube.SetActive(setGameObjectActive);

            Solver solver = AddSolverComponent<T>(cube);

            SolverHandler handler = cube.GetComponent<SolverHandler>();
            Assert.IsNotNull(handler, "GetComponent<SolverHandler>() returned null");

            var setupData = new SetupData()
            {
                handler = handler,
                solver = solver,
                target = cube
            };

            setupDataList.Add(setupData);

            return setupData;
        }

        private T AddSolverComponent<T>(GameObject target) where T : Solver
        {
            T solver = target.AddComponent<T>();
            Assert.IsNotNull(solver, "AddComponent<T>() returned null");

            // Set Solver lerp times to 0 so we can process tests faster instead of waiting for transforms to update/apply
            solver.MoveLerpTime = 0.0f;
            solver.RotateLerpTime = 0.0f;
            solver.ScaleLerpTime = 0.0f;

            return solver;
        }

        private IEnumerator WaitForFrames(int frames)
        {
            for (int i = 0; i < frames; i++)
            {
                yield return null;
            }
        }

        /// <summary>
        /// A generalized testing functionality for the HandConstraintPalmUp script that takes in a safezone and target handedness configuration
        /// and then tests it (using those configurations to generate a target test hand placement for activation)
        /// </summary>
        /// <param name="safeZone"> The safezone tested against for this test</param>
        /// <param name="targetHandedness">The target handedness tested against for these activation tests</param>=
        private IEnumerator TestHandConstraintPalmUpGazeActivationByZoneAndHand(HandConstraint.SolverSafeZone safeZone, Handedness targetHandedness)
        {
            // Instantiate our test GameObject with solver.
            var testObjects = InstantiateTestSolver<HandConstraintPalmUp>();
            testObjects.handler.TrackedTargetType = TrackedObjectType.HandJoint;
            testObjects.handler.TrackedHandness = Handedness.Both;

            var handConstraintSolver = (HandConstraintPalmUp)testObjects.solver;
            handConstraintSolver.FollowHandUntilFacingCamera = true;
            handConstraintSolver.UseGazeActivation = true;

            handConstraintSolver.SafeZone = safeZone;
            testObjects.solver.Smoothing = false;

            // Ensure that FacingCameraTrackingThreshold is greater than FollowHandCameraFacingThresholdAngle
            Assert.AreEqual(handConstraintSolver.FacingCameraTrackingThreshold - handConstraintSolver.FollowHandCameraFacingThresholdAngle > 0, true);

            yield return null;

            TestUtilities.AssertAboutEqual(testObjects.target.transform.position, Vector3.zero, "HandConstraintPalmUp solver did not start at the origin");

            var cameraTransform = CameraCache.Main.transform;
            // Place hand 1 meter in front of user, and near the activation zone
            var handTestPos = cameraTransform.position + cameraTransform.forward + DetermineHandOriginPositionOffset(safeZone, targetHandedness);

            // Generate hand rotation with hand palm facing camera
            var cameraLookVector = (handTestPos - cameraTransform.position).normalized;
            var handRotation = Quaternion.LookRotation(cameraTransform.up, cameraLookVector);

            // Add a hand based on the passed in handedness.
            var hand = new TestHand(targetHandedness);
            yield return hand.Show(handTestPos);
            yield return hand.SetRotation(handRotation);
            yield return null;

            // Ensure Activation occurred by making sure the testObjects position isn't still Vector3.zero
            Assert.AreNotEqual(testObjects.target.transform.position, Vector3.zero);

            var palmConstraint = testObjects.solver as HandConstraint;
            // Test forward offset 
            palmConstraint.ForwardOffset = -0.6f;
            yield return null;
            for (float forwardOffset = -0.5f; forwardOffset < 0; forwardOffset += 0.1f)
            {
                Vector3 prevPosition = testObjects.target.transform.position;
                palmConstraint.ForwardOffset = forwardOffset;
                yield return null;
                Vector3 curPosition = testObjects.target.transform.position;
                Vector3 deltaPos = curPosition - prevPosition;
                float actual = Vector3.Dot(deltaPos, CameraCache.Main.transform.forward);
                string debugStr = $"forwardOffset: {palmConstraint.ForwardOffset} prevPosition: {prevPosition.ToString("0.0000")} curPosition: {curPosition.ToString("0.0000")}, {actual}";
                Assert.True(actual < 0, $"Increasing forward offset is expected to move object toward camera. {debugStr}");
            }

            palmConstraint.ForwardOffset = 0;
            palmConstraint.SafeZoneAngleOffset = 0;
            yield return null;
            int delta = 30;
            for (int angle = delta; angle <= 90; angle += delta)
            {
                Vector3 prevPalmToObj = testObjects.target.transform.position - handTestPos;
                palmConstraint.SafeZoneAngleOffset = angle;
                yield return null;
                Vector3 curPalmToObj = testObjects.target.transform.position - handTestPos;
                Vector3 rotationAxis = -cameraTransform.forward;
                if (safeZone == HandConstraint.SolverSafeZone.AtopPalm)
                {
                    HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, targetHandedness, out MixedRealityPose palmPose);
                    rotationAxis = -palmPose.Forward;
                }
                float signedAngle = Vector3.SignedAngle(prevPalmToObj, curPalmToObj, rotationAxis);

                if (targetHandedness == Handedness.Right)
                {
                    signedAngle *= -1;
                }
                Assert.True(signedAngle < 0, $"Increasing SolverSafeZoneAngleOffset should move menu in clockwise direction in left hand, anti-clockwise in right hand {signedAngle}");
            }


            yield return hand.Hide();
        }

        /// <summary>
        /// Based on the type of handconstraint solver safe zone and handedness, returns the offset that the tested hand should apply initially.
        /// </summary>
        /// <param name="safeZone">The target safezone type that's used to determine the position calculations done</param>
        /// <param name="targetHandedness"> The target handedness that's used to calculate the initial activation position</param>
        /// <returns>The Vector3 representing where the hand should be positioned to during the test to trigger the activation</returns>
        private Vector3 DetermineHandOriginPositionOffset(HandConstraint.SolverSafeZone safeZone, Handedness targetHandedness)
        {
            switch (safeZone)
            {
                case HandConstraint.SolverSafeZone.RadialSide:
                    if (targetHandedness == Handedness.Left)
                    {
                        return Vector3.left * RadialUlnarTestActivationPointModifier;
                    }
                    else
                    {
                        return Vector3.right * RadialUlnarTestActivationPointModifier;
                    }

                case HandConstraint.SolverSafeZone.BelowWrist:
                    return Vector3.up * WristTestActivationPointModifier;

                // AtopPalm uses the same test zone as AboveFingerTips because
                // the hand must move to a similar position to activate.
                case HandConstraint.SolverSafeZone.AtopPalm:
                case HandConstraint.SolverSafeZone.AboveFingerTips:
                    return Vector3.down * AboveFingerTipsTestActivationPointModifier;

                default:
                case HandConstraint.SolverSafeZone.UlnarSide:
                    if (targetHandedness == Handedness.Left)
                    {
                        return Vector3.right * RadialUlnarTestActivationPointModifier;
                    }
                    else
                    {
                        return Vector3.left * RadialUlnarTestActivationPointModifier;
                    }
            }
        }

        #endregion
    }
}
#endif
