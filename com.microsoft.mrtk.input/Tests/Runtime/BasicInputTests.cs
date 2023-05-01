// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Core.Tests;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Input.Simulation;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Subsystems;

using HandshapeId = Microsoft.MixedReality.Toolkit.Input.HandshapeTypes.HandshapeId;

namespace Microsoft.MixedReality.Toolkit.Input.Tests
{
    /// <summary>
    /// Basic tests for verifying user input and basic interactions.
    /// </summary>
    public class BasicInputTests : BaseRuntimeInputTests
    {
        /// <summary>
        /// Ensure the simulated input devices are registered and present.
        /// </summary>
        [UnityTest]
        public IEnumerator InputDeviceSmoketest()
        {
            foreach (var device in InputSystem.devices)
            {
                Debug.Log(device);
            }
            Assert.That(InputSystem.devices, Has.Exactly(2).TypeOf<MRTKSimulatedController>());
            yield return null;
        }

        /// <summary>
        /// Ensure the simulated input devices bind to the controllers on the rig.
        /// </summary>
        [UnityTest]
        public IEnumerator InputBindingSmoketest()
        {
            var controllers = new[] {
                CachedLookup.LeftHandController,
                CachedLookup.RightHandController,
                CachedLookup.GazeController
            };

            foreach (var controller in controllers)
            {
                Assert.That(controller, Is.Not.Null);
                Assert.That(controller, Is.AssignableTo(typeof(ActionBasedController)));

                ActionBasedController actionBasedController = controller as ActionBasedController;
                Assert.That(actionBasedController.positionAction.action.controls, Has.Count.GreaterThanOrEqualTo(1));
            }

            yield return null;
        }

        /// <summary>
        /// Ensure the simulated input device actually makes the rig's controllers move/actuate.
        /// </summary>
        [UnityTest]
        public IEnumerator HandMovingSmoketest()
        {
            var controller = CachedLookup.RightHandController as ActionBasedController;

            var testHand = new TestHand(Handedness.Right);
            InputTestUtilities.SetHandAnchorPoint(Handedness.Right, ControllerAnchorPoint.Device);

            yield return testHand.Show(Vector3.forward);
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.That(controller.transform.position.x, Is.EqualTo(0.0f).Within(0.01f));

            yield return testHand.Move(Vector3.right * 0.5f, 60);
            yield return RuntimeTestUtilities.WaitForUpdates();
            Debug.Log("Input system update mode: " + InputSystem.settings.updateMode);

            Assert.That(controller.positionAction.action.controls, Has.Count.GreaterThanOrEqualTo(1));
            Assert.That(controller.positionAction.action.activeControl, Is.Not.Null);
            Assert.That(controller.positionAction.action.ReadValue<Vector3>().x, Is.EqualTo(0.5f).Within(0.01f));

            Assert.That(controller.transform.position.x, Is.EqualTo(0.5f).Within(0.01f));

            yield return null;
        }

        /// <summary>
        /// Test that anchoring the test hands on the grab point actually results in the grab interactor
        /// being located where we want it to be.
        /// </summary>
        [UnityTest]
        public IEnumerator GrabAnchorTest()
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var interactable = cube.AddComponent<MRTKBaseInteractable>();

            Vector3 cubePos = InputTestUtilities.InFrontOfUser();
            cube.transform.position = cubePos;
            cube.transform.localScale = Vector3.one * 1.0f;
            
            var testHand = new TestHand(Handedness.Right);
            InputTestUtilities.SetHandAnchorPoint(Handedness.Right, ControllerAnchorPoint.Grab);

            yield return testHand.Show(Vector3.zero);
            yield return RuntimeTestUtilities.WaitForUpdates();
            yield return testHand.MoveTo(cubePos);
            yield return RuntimeTestUtilities.WaitForUpdates();

            var hands = XRSubsystemHelpers.GetFirstRunningSubsystem<HandsAggregatorSubsystem>();

            bool gotJoint = hands.TryGetPinchingPoint(XRNode.RightHand, out HandJointPose jointPose);

            Assert.IsTrue(interactable.IsGrabHovered, "Interactable wasn't grab hovered!");

            Assert.IsTrue((interactable.HoveringGrabInteractors[0] as GrabInteractor).enabled, "Interactor wasn't enabled");

            TestUtilities.AssertAboutEqual(interactable.HoveringGrabInteractors[0].GetAttachTransform(interactable).position,
                                           cubePos, "Grab interactor's attachTransform wasn't where we wanted it to go!", 0.001f);
        }

        /// <summary>
        /// Very basic test of StatefulInteractable's poke hovering, grab selecting,
        /// and toggling mechanics. Does not test rays or gaze interactions.
        /// </summary>
        [UnityTest]
        public IEnumerator StatefulInteractableSmoketest()
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.AddComponent<StatefulInteractable>();
            cube.transform.position = InputTestUtilities.InFrontOfUser(new Vector3(0.2f, 0.2f, 0.5f));
            cube.transform.localScale = Vector3.one * 0.1f;

            // For this test, we won't use poke selection.
            cube.GetComponent<StatefulInteractable>().DisableInteractorType(typeof(PokeInteractor));

            GameObject cube2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube2.AddComponent<StatefulInteractable>();
            cube2.transform.position = InputTestUtilities.InFrontOfUser(new Vector3(-0.2f, -0.2f, 0.5f));
            cube2.transform.localScale = Vector3.one * 0.1f;

            // For this test, we won't use poke selection.
            cube2.GetComponent<StatefulInteractable>().DisableInteractorType(typeof(PokeInteractor));

            var rightHand = new TestHand(Handedness.Right);

            yield return rightHand.Show(InputTestUtilities.InFrontOfUser(0.5f));

            yield return RuntimeTestUtilities.WaitForUpdates();

            bool shouldTestToggle = false;

            StatefulInteractable firstCubeInteractable = cube.GetComponent<StatefulInteractable>();
            StatefulInteractable secondCubeInteractable = cube2.GetComponent<StatefulInteractable>();

            for (int i = 0; i < 5; i++)
            {
                // Flip this back and forth to test both toggleability and un-toggleability
                shouldTestToggle = !shouldTestToggle;

                yield return rightHand.RotateTo(Quaternion.Euler(0, 45, 0));
                yield return RuntimeTestUtilities.WaitForUpdates();

                // Test the first cube.
                firstCubeInteractable.ForceSetToggled(false);
                firstCubeInteractable.ToggleMode = shouldTestToggle ? StatefulInteractable.ToggleType.Toggle : StatefulInteractable.ToggleType.Button;
                firstCubeInteractable.TriggerOnRelease = (i % 2) == 0;

                Assert.IsFalse(firstCubeInteractable.IsGrabHovered,
                               "StatefulInteractable was already hovered.");

                yield return rightHand.MoveTo(cube.transform.position);
                yield return RuntimeTestUtilities.WaitForUpdates();
                Assert.IsTrue(firstCubeInteractable.IsGrabHovered,
                              "StatefulInteractable did not get hovered.");

                yield return rightHand.SetHandshape(HandshapeId.Pinch);
                yield return RuntimeTestUtilities.WaitForUpdates();
                Assert.IsTrue(firstCubeInteractable.IsGrabSelected,
                              "StatefulInteractable did not get GrabSelected.");

                if (shouldTestToggle)
                {
                    if (secondCubeInteractable.TriggerOnRelease)
                    {
                        Assert.IsFalse(secondCubeInteractable.IsToggled, "StatefulInteractable toggled on press, when it was set to be toggled on release.");
                    }
                    else
                    {
                        Assert.IsFalse(secondCubeInteractable.IsToggled, "StatefulInteractable didn't toggled on press, when it was set to be toggled on press.");
                    }
                }

                yield return rightHand.SetHandshape(HandshapeId.Open);
                yield return RuntimeTestUtilities.WaitForUpdates();
                Assert.IsFalse(firstCubeInteractable.IsGrabSelected,
                              "StatefulInteractable did not get un-GrabSelected.");

                if (shouldTestToggle)
                {
                    Assert.IsTrue(firstCubeInteractable.IsToggled, "StatefulInteractable did not get toggled.");
                }
                else
                {
                    Assert.IsFalse(firstCubeInteractable.IsToggled, "StatefulInteractable shouldn't have been toggled, but it was.");
                }

                // Test the second cube.
                secondCubeInteractable.ForceSetToggled(false);
                secondCubeInteractable.ToggleMode = shouldTestToggle ? StatefulInteractable.ToggleType.Toggle : StatefulInteractable.ToggleType.Button;
                secondCubeInteractable.TriggerOnRelease = (i % 2) == 0;

                yield return rightHand.MoveTo(InputTestUtilities.InFrontOfUser(0.5f));
                yield return RuntimeTestUtilities.WaitForUpdates();
                yield return rightHand.RotateTo(Quaternion.Euler(0, -45, 0));
                yield return RuntimeTestUtilities.WaitForUpdates();

                Assert.IsFalse(secondCubeInteractable.IsGrabHovered,
                               "StatefulInteractable was already hovered.");

                yield return rightHand.MoveTo(secondCubeInteractable.transform.position);
                yield return RuntimeTestUtilities.WaitForUpdates();
                Assert.IsTrue(secondCubeInteractable.IsGrabHovered,
                              "StatefulInteractable did not get hovered.");

                yield return rightHand.SetHandshape(HandshapeId.Pinch);
                yield return RuntimeTestUtilities.WaitForUpdates();
                Assert.IsTrue(secondCubeInteractable.IsGrabSelected,
                              "StatefulInteractable did not get GrabSelected.");

                if (shouldTestToggle)
                {
                    if (secondCubeInteractable.TriggerOnRelease)
                    {
                        Assert.IsFalse(secondCubeInteractable.IsToggled, "StatefulInteractable toggled on press, when it was set to be toggled on release.");
                    }
                    else
                    {
                        Assert.IsFalse(secondCubeInteractable.IsToggled, "StatefulInteractable didn't toggled on press, when it was set to be toggled on press.");
                    }
                }

                yield return rightHand.SetHandshape(HandshapeId.Open);
                yield return RuntimeTestUtilities.WaitForUpdates();
                Assert.IsFalse(secondCubeInteractable.IsGrabSelected,
                              "StatefulInteractable did not get un-GrabSelected.");

                if (shouldTestToggle)
                {
                    Assert.IsTrue(secondCubeInteractable.IsToggled, "StatefulInteractable did not get toggled.");
                }
                else
                {
                    Assert.IsFalse(secondCubeInteractable.IsToggled, "StatefulInteractable shouldn't have been toggled, but it was.");
                }

                yield return rightHand.MoveTo(InputTestUtilities.InFrontOfUser(0.5f));
                yield return RuntimeTestUtilities.WaitForUpdates();
            }

            yield return null;
        }

        /// <summary>
        /// Simple smoketest to ensure basic gaze-pinch selection functionality.
        /// </summary>
        [UnityTest]
        public IEnumerator GazePinchSmokeTest()
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            StatefulInteractable interactable = cube.AddComponent<StatefulInteractable>();
            cube.transform.position = InputTestUtilities.InFrontOfUser();
            cube.transform.localScale = Vector3.one * 0.1f;

            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsTrue(interactable.isHovered);
            Assert.IsTrue(interactable.IsGazeHovered);
            Assert.IsFalse(interactable.IsGazePinchHovered);

            var rightHand = new TestHand(Handedness.Right);
            yield return rightHand.Show(InputTestUtilities.InFrontOfUser() + new Vector3(0.2f, 0, 0f));

            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsTrue(interactable.isHovered);
            Assert.IsTrue(interactable.IsGazePinchHovered);

            yield return rightHand.SetHandshape(HandshapeId.Pinch);
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsTrue(interactable.isSelected);
            Assert.IsTrue(interactable.IsGazePinchSelected);

            yield return rightHand.SetHandshape(HandshapeId.Open);
            yield return RuntimeTestUtilities.WaitForUpdates();
            
            Assert.IsFalse(interactable.isSelected);
            Assert.IsFalse(interactable.IsGazePinchSelected);
            Assert.IsTrue(interactable.isHovered);
            Assert.IsTrue(interactable.IsGazePinchHovered);
        }

        /// <summary>
        /// A dummy interactor used to test basic selection/toggle logic.
        /// </summary>
        private class TestInteractor : XRBaseInteractor { }

        /// <summary>
        /// Test that the correct toggle state should be readable after receiving an OnClicked event.
        /// </summary>
        [UnityTest]
        public IEnumerator TestToggleEventOrdering()
        {
            var gameObject = new GameObject();
            var interactable = gameObject.AddComponent<StatefulInteractable>();
            var interactor = gameObject.AddComponent<TestInteractor>();

            bool receivedOnClicked = false;
            bool expectedToggleState = false;

            interactable.OnClicked.AddListener(() =>
            {
                receivedOnClicked = true;
                Assert.IsTrue(interactable.IsToggled == expectedToggleState, "Toggle state had an unexpected value");
            });

            interactor.StartManualInteraction(interactable as IXRSelectInteractable);
            yield return null;
            interactor.EndManualInteraction();
            yield return null;

            Assert.IsTrue(receivedOnClicked, "Didn't receive click event");
            receivedOnClicked = false;

            interactable.ToggleMode = StatefulInteractable.ToggleType.Toggle;
            expectedToggleState = true;

            interactor.StartManualInteraction(interactable as IXRSelectInteractable);
            yield return null;
            interactor.EndManualInteraction();
            yield return null;

            Assert.IsTrue(receivedOnClicked, "Didn't receive click event");
            receivedOnClicked = false;
            expectedToggleState = false;

            interactor.StartManualInteraction(interactable as IXRSelectInteractable);
            yield return null;
            interactor.EndManualInteraction();
            yield return null;

            Assert.IsTrue(receivedOnClicked, "Didn't receive click event");
        }

        /// <summary>
        /// Test whether toggle state can be hydrated without firing events.
        /// </summary>
        [UnityTest]
        public IEnumerator ToggleHydrationTest()
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var interactable = cube.AddComponent<StatefulInteractable>();

            bool didFireEvent = false;

            interactable.IsToggled.OnEntered.AddListener((_) => didFireEvent = true);
            interactable.IsToggled.OnExited.AddListener((_) => didFireEvent = true);
            interactable.ForceSetToggled(true);

            Assert.IsTrue(interactable.IsToggled, "Interactable didn't get toggled.");
            Assert.IsTrue(didFireEvent, "ForceSetToggled(true) should have fired the event.");

            didFireEvent = false;
            interactable.ForceSetToggled(false);

            Assert.IsFalse(interactable.IsToggled, "Interactable didn't get detoggled.");
            Assert.IsTrue(didFireEvent, "ForceSetToggled(false) should have fired the event.");

            didFireEvent = false;
            interactable.ForceSetToggled(true, fireEvents: false);
            
            Assert.IsTrue(interactable.IsToggled, "Interactable didn't get toggled.");
            Assert.IsFalse(didFireEvent, "ForceSetToggled(true, fireEvents:false) should NOT have fired the event.");

            interactable.ForceSetToggled(false, fireEvents: false);

            Assert.IsFalse(interactable.IsToggled, "Interactable didn't get detoggled.");
            Assert.IsFalse(didFireEvent, "ForceSetToggled(false, fireEvents:false) should NOT have fired the event.");

            yield return null;
        }

        /// <summary>
        /// Tests whether disabling an interactable mid-interaction will
        /// break XRDirectInteractor. Repro test for ADO#1582/1581.
        /// </summary>
        [UnityTest]
        public IEnumerator InteractableDisabledDuringInteraction()
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = InputTestUtilities.InFrontOfUser(new Vector3(1.0f, 0.1f, 1.0f));
            cube.transform.localScale = Vector3.one * 0.1f;
            cube.AddComponent<StatefulInteractable>();

            // Otherwise, poke will conflict with grab.
            cube.GetComponent<StatefulInteractable>().selectMode = InteractableSelectMode.Multiple;

            var rightHand = new TestHand(Handedness.Right);
            yield return rightHand.Show(InputTestUtilities.InFrontOfUser());

            yield return RuntimeTestUtilities.WaitForUpdates();
            yield return rightHand.MoveTo(cube.transform.position);
            yield return RuntimeTestUtilities.WaitForUpdates();
            yield return rightHand.SetHandshape(HandshapeId.Pinch);
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsTrue(cube.GetComponent<StatefulInteractable>().IsGrabSelected,
                          "StatefulInteractable did not get GrabSelected.");

            cube.SetActive(false);

            yield return rightHand.SetHandshape(HandshapeId.Open);
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsFalse(cube.GetComponent<StatefulInteractable>().IsGrabSelected,
                           "StatefulInteractable did not get un-GrabSelected.");

            yield return rightHand.MoveTo(Vector3.zero);
            yield return RuntimeTestUtilities.WaitForUpdates();

            cube.SetActive(true);
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsFalse(AnyProximityDetectorsTriggered(),
                           "ProximityInteractor was still hovering after re-enabling faraway object.");

            XRBaseController rightHandController = CachedLookup.RightHandController;
            Assert.IsTrue(rightHandController != null, "No controllers found for right hand.");

            Assert.IsTrue(rightHandController.GetComponentInChildren<MRTKRayInteractor>().enabled, "Ray didn't reactivate");
            Assert.IsTrue(rightHandController.GetComponentInChildren<GazePinchInteractor>().enabled, "GazePinch didn't reactivate");
            Assert.IsFalse(rightHandController.GetComponentInChildren<PokeInteractor>().enabled, "Poke didn't deactivate");
            Assert.IsFalse(rightHandController.GetComponentInChildren<GrabInteractor>().enabled, "Grab didn't deactivate");

            yield return null;
        }

        /// <summary>
        /// Tests whether spawning an interactable on top of a hand will cause problems with the proximity detector.
        /// </summary>
        [UnityTest]
        public IEnumerator SpawnInteractableOnHand()
        {
            // Spawn our hand.
            var rightHand = new TestHand(Handedness.Right);
            yield return rightHand.Show(InputTestUtilities.InFrontOfUser());
            yield return RuntimeTestUtilities.WaitForUpdates();

            // Prox detector should start out un-triggered.
            Assert.IsFalse(AnyProximityDetectorsTriggered(), "Prox detector started out triggered, when it shouldn't be (no cube yet!)");

            // Rays should start enabled
            XRBaseController rightHandController = CachedLookup.RightHandController;
            Assert.IsTrue(rightHandController != null, "No controllers found for right hand.");

            Assert.IsTrue(rightHandController.GetComponentInChildren<MRTKRayInteractor>().enabled, "Ray didn't start active");
            Assert.IsTrue(rightHandController.GetComponentInChildren<GazePinchInteractor>().enabled, "GazePinch didn't start active");
            Assert.IsFalse(rightHandController.GetComponentInChildren<PokeInteractor>().enabled, "Poke started active, when it shouldn't");
            Assert.IsFalse(rightHandController.GetComponentInChildren<GrabInteractor>().enabled, "Grab started active, when it shouldn't");

            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = InputTestUtilities.InFrontOfUser();
            cube.transform.localScale = Vector3.one * 0.1f;
            cube.AddComponent<StatefulInteractable>();
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsTrue(AnyProximityDetectorsTriggered(), "Prox detector should see it!");

            Assert.IsFalse(rightHandController.GetComponentInChildren<MRTKRayInteractor>().enabled, "Ray didn't disable on proximity");
            Assert.IsFalse(rightHandController.GetComponentInChildren<GazePinchInteractor>().enabled, "GazePinch disable on proximity");
            Assert.IsTrue(rightHandController.GetComponentInChildren<PokeInteractor>().enabled, "Poke didn't activate on proximity");
            Assert.IsTrue(rightHandController.GetComponentInChildren<GrabInteractor>().enabled, "Grab didn't activate on proximity");

            // Move hand far away.
            yield return rightHand.MoveTo(new Vector3(2, 2, 2));
            yield return RuntimeTestUtilities.WaitForUpdates(frameCount:240);

            Assert.IsFalse(AnyProximityDetectorsTriggered(), "Prox detectors should no longer be triggered.");

            Assert.IsTrue(rightHandController.GetComponentInChildren<MRTKRayInteractor>().enabled, "Ray didn't reactivate");
            Assert.IsTrue(rightHandController.GetComponentInChildren<GazePinchInteractor>().enabled, "GazePinch didn't reactivate");
            Assert.IsFalse(rightHandController.GetComponentInChildren<PokeInteractor>().enabled, "Poke didn't deactivate");
            Assert.IsFalse(rightHandController.GetComponentInChildren<GrabInteractor>().enabled, "Grab didn't deactivate");

            yield return null;
        }

        /// <summary>
        /// Tests to make sure that untracked controllers can't initiate any new interactions and that their interactors can no longer hover.
        /// However, the interactions should still maintaining any original selected states, as the loss of tracking is usually just temporary
        /// i.e. we don't want to immediately let go of a gripped object due to a momentary loss in tracking
        /// </summary>
        [UnityTest]
        public IEnumerator UntrackedControllerNearInteractions()
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = InputTestUtilities.InFrontOfUser(new Vector3(1.0f, 0.1f, 1.0f));
            cube.transform.localScale = Vector3.one * 0.1f;
            cube.AddComponent<StatefulInteractable>();

            // Otherwise, poke will conflict with grab.
            cube.GetComponent<StatefulInteractable>().selectMode = InteractableSelectMode.Multiple;

            var rightHand = new TestHand(Handedness.Right);
            yield return RuntimeTestUtilities.WaitForUpdates();
            yield return rightHand.Show(InputTestUtilities.InFrontOfUser(0.5f));
            yield return RuntimeTestUtilities.WaitForUpdates();
            // First ensure that the interactor can interact with a cube normally
            yield return rightHand.MoveTo(cube.transform.position);
            yield return RuntimeTestUtilities.WaitForUpdates();
            yield return rightHand.SetHandshape(HandshapeId.Pinch);
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsTrue(cube.GetComponent<StatefulInteractable>().IsGrabSelected,
                          "StatefulInteractable did not get GrabSelected.");

            // Now check that all hovers are disabled while selection is maintained after we "lose tracking", which is done by hiding the hand
            yield return rightHand.Hide();
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsTrue(cube.GetComponent<StatefulInteractable>().IsGrabSelected,
                           "StatefulInteractable is no longer GrabSelected.");

            // Make sure state is maintained even if the hand gameobject moves
            yield return rightHand.Move(Vector3.left);
            yield return RuntimeTestUtilities.WaitForUpdates();
            Assert.IsTrue(cube.GetComponent<StatefulInteractable>().IsGrabSelected,
                           "StatefulInteractable is no longer GrabSelected.");

            yield return rightHand.SetHandshape(HandshapeId.Open);
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsFalse(cube.GetComponent<StatefulInteractable>().IsGrabSelected,
                           "StatefulInteractable did not get un-GrabSelected.");

            // Check that the hand cannot interact with any new interactables
            var newCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            newCube.transform.position = InputTestUtilities.InFrontOfUser(new Vector3(-3.0f, 0.1f, 1.0f));
            newCube.transform.localScale = Vector3.one * 0.1f;
            newCube.AddComponent<StatefulInteractable>();

            yield return rightHand.MoveTo(newCube.transform.position);
            yield return RuntimeTestUtilities.WaitForUpdates();
            yield return rightHand.SetHandshape(HandshapeId.Pinch);
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsFalse(newCube.GetComponent<StatefulInteractable>().IsGrabSelected,
                            "The interactor somehow grabbed the new cube");

            yield return rightHand.SetHandshape(HandshapeId.Open);
            yield return RuntimeTestUtilities.WaitForUpdates();

            // Finish
            yield return RuntimeTestUtilities.WaitForUpdates();
            yield return rightHand.MoveTo(Vector3.zero);
            yield return RuntimeTestUtilities.WaitForUpdates();

            yield return rightHand.Show();
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsFalse(AnyProximityDetectorsTriggered(),
                           "ProximityInteractor was still hovering after re-enabling faraway object.");

            XRBaseController rightHandController = CachedLookup.RightHandController;
            Assert.IsTrue(rightHandController != null, "No controllers found for right hand.");

            Assert.IsTrue(rightHandController.GetComponentInChildren<MRTKRayInteractor>().enabled, "Ray didn't reactivate");
            Assert.IsTrue(rightHandController.GetComponentInChildren<GazePinchInteractor>().enabled, "GazePinch didn't reactivate");
            Assert.IsFalse(rightHandController.GetComponentInChildren<PokeInteractor>().enabled, "Poke didn't deactivate");
            Assert.IsFalse(rightHandController.GetComponentInChildren<GrabInteractor>().enabled, "Grab didn't deactivate");

            yield return null;
        }

        // Returns true iff any of the ProximityDetectors in the scene are currently triggered.
        private bool AnyProximityDetectorsTriggered()
        {
            ProximityDetector[] detectors = Object.FindObjectsOfType<ProximityDetector>();
            foreach (var detector in detectors)
            {
                if (detector.IsModeDetected())
                {
                    return true;
                }
            }

            return false;
        }
    }
}

