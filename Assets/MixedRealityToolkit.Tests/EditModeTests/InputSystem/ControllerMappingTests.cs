// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Input.UnityInput;
using Microsoft.MixedReality.Toolkit.OpenVR.Input;
using NUnit.Framework;

namespace Microsoft.MixedReality.Toolkit.Tests.EditMode.InputSystem
{
    public class ControllerMappingTests
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes(true);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            TestUtilities.ShutdownMixedRealityToolkit();
            TestUtilities.EditorTearDownScenes();
        }

        [Test]
        public void MouseControllerUpdateTest()
        {
            IMixedRealityInputSource inputSource = CoreServices.InputSystem?.RequestNewGenericInputSource($"Mouse Controller");
            MouseController controller = new MouseController(TrackingState.NotApplicable, Utilities.Handedness.Any, inputSource);

            // Tests
            Assert.That(() => controller.Update(), Throws.Nothing);
        }

        [Test]
        public void XboxControllerUpdateTest()
        {
            IMixedRealityInputSource inputSource = CoreServices.InputSystem?.RequestNewGenericInputSource($"Xbox Controller");
            XboxController controller = new XboxController(TrackingState.NotApplicable, Utilities.Handedness.None, inputSource);

            TestGenericJoystickControllerUpdate(controller);
        }

        [Test]
        public void GenericOpenVRControllerUpdateTest()
        {
            IMixedRealityInputSource leftInputSource = CoreServices.InputSystem?.RequestNewGenericInputSource($"Left OpenVR Controller");
            GenericOpenVRController leftController = new GenericOpenVRController(TrackingState.NotTracked, Utilities.Handedness.Left, leftInputSource);
            IMixedRealityInputSource rightInputSource = CoreServices.InputSystem?.RequestNewGenericInputSource($"Right OpenVR Controller");
            GenericOpenVRController rightController = new GenericOpenVRController(TrackingState.NotTracked, Utilities.Handedness.Right, rightInputSource);

            TestGenericJoystickControllerUpdate(leftController);
            TestGenericJoystickControllerUpdate(rightController);
        }

        [Test]
        public void OculusRemoteControllerUpdateTest()
        {
            IMixedRealityInputSource inputSource = CoreServices.InputSystem?.RequestNewGenericInputSource($"Oculus Remote Controller");
            OculusRemoteController controller = new OculusRemoteController(TrackingState.NotTracked, Utilities.Handedness.None, inputSource);

            TestGenericJoystickControllerUpdate(controller);
        }

        [Test]
        public void OculusTouchControllerUpdateTest()
        {
            IMixedRealityInputSource leftInputSource = CoreServices.InputSystem?.RequestNewGenericInputSource($"Left Oculus Touch Controller");
            OculusTouchController leftController = new OculusTouchController(TrackingState.NotTracked, Utilities.Handedness.Left, leftInputSource);
            IMixedRealityInputSource rightInputSource = CoreServices.InputSystem?.RequestNewGenericInputSource($"Right Oculus Touch Controller");
            OculusTouchController rightController = new OculusTouchController(TrackingState.NotTracked, Utilities.Handedness.Right, rightInputSource);

            TestGenericJoystickControllerUpdate(leftController);
            TestGenericJoystickControllerUpdate(rightController);
        }

        [Test]
        public void ViveKnucklesControllerUpdateTest()
        {
            IMixedRealityInputSource leftInputSource = CoreServices.InputSystem?.RequestNewGenericInputSource($"Left Knuckles Controller");
            ViveKnucklesController leftController = new ViveKnucklesController(TrackingState.NotTracked, Utilities.Handedness.Left, leftInputSource);
            IMixedRealityInputSource rightInputSource = CoreServices.InputSystem?.RequestNewGenericInputSource($"Right Knuckles Controller");
            ViveKnucklesController rightController = new ViveKnucklesController(TrackingState.NotTracked, Utilities.Handedness.Right, rightInputSource);

            TestGenericJoystickControllerUpdate(leftController);
            TestGenericJoystickControllerUpdate(rightController);
        }

        [Test]
        public void ViveWandControllerUpdateTest()
        {
            IMixedRealityInputSource leftInputSource = CoreServices.InputSystem?.RequestNewGenericInputSource($"Left Vive Wand Controller");
            ViveWandController leftController = new ViveWandController(TrackingState.NotTracked, Utilities.Handedness.Left, leftInputSource);
            IMixedRealityInputSource rightInputSource = CoreServices.InputSystem?.RequestNewGenericInputSource($"Right Vive Wand Controller");
            ViveWandController rightController = new ViveWandController(TrackingState.NotTracked, Utilities.Handedness.Right, rightInputSource);

            TestGenericJoystickControllerUpdate(leftController);
            TestGenericJoystickControllerUpdate(rightController);
        }

        [Test]
        public void WindowsMixedRealityOpenVRMotionControllerUpdateTest()
        {
            IMixedRealityInputSource leftInputSource = CoreServices.InputSystem?.RequestNewGenericInputSource($"Left Motion Controller");
            WindowsMixedRealityOpenVRMotionController leftController = new WindowsMixedRealityOpenVRMotionController(TrackingState.NotTracked, Utilities.Handedness.Left, leftInputSource);
            IMixedRealityInputSource rightInputSource = CoreServices.InputSystem?.RequestNewGenericInputSource($"Right Motion Controller");
            WindowsMixedRealityOpenVRMotionController rightController = new WindowsMixedRealityOpenVRMotionController(TrackingState.NotTracked, Utilities.Handedness.Right, rightInputSource);

            TestGenericJoystickControllerUpdate(leftController);
            TestGenericJoystickControllerUpdate(rightController);
        }

        private void TestGenericJoystickControllerUpdate(GenericJoystickController controller)
        {
            // Test
            Assert.That(() => controller.UpdateController(), Throws.Nothing);
        }
    }
}