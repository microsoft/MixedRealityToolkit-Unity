// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input.UnityInput;
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
            MouseController controller = new MouseController(TrackingState.NotApplicable, Utilities.Handedness.Any);

            // Tests
            Assert.That(() => controller.Update(), Throws.Nothing);
        }

        [Test]
        public void XboxControllerUpdateTest()
        {
            XboxController controller = new XboxController(TrackingState.NotApplicable, Utilities.Handedness.None);

            TestGenericJoystickControllerUpdate(controller);
        }

#if !UNITY_2020_1_OR_NEWER
        [Test]
        public void GenericOpenVRControllerUpdateTest()
        {
            OpenVR.Input.GenericOpenVRController leftController = new OpenVR.Input.GenericOpenVRController(TrackingState.NotTracked, Utilities.Handedness.Left);
            OpenVR.Input.GenericOpenVRController rightController = new OpenVR.Input.GenericOpenVRController(TrackingState.NotTracked, Utilities.Handedness.Right);

            TestGenericJoystickControllerUpdate(leftController);
            TestGenericJoystickControllerUpdate(rightController);
        }

        [Test]
        public void OculusRemoteControllerUpdateTest()
        {
            OpenVR.Input.OculusRemoteController controller = new OpenVR.Input.OculusRemoteController(TrackingState.NotTracked, Utilities.Handedness.None);

            TestGenericJoystickControllerUpdate(controller);
        }

        [Test]
        public void OculusTouchControllerUpdateTest()
        {
            OpenVR.Input.OculusTouchController leftController = new OpenVR.Input.OculusTouchController(TrackingState.NotTracked, Utilities.Handedness.Left);
            OpenVR.Input.OculusTouchController rightController = new OpenVR.Input.OculusTouchController(TrackingState.NotTracked, Utilities.Handedness.Right);

            TestGenericJoystickControllerUpdate(leftController);
            TestGenericJoystickControllerUpdate(rightController);
        }

        [Test]
        public void ViveKnucklesControllerUpdateTest()
        {
            OpenVR.Input.ViveKnucklesController leftController = new OpenVR.Input.ViveKnucklesController(TrackingState.NotTracked, Utilities.Handedness.Left);
            OpenVR.Input.ViveKnucklesController rightController = new OpenVR.Input.ViveKnucklesController(TrackingState.NotTracked, Utilities.Handedness.Right);

            TestGenericJoystickControllerUpdate(leftController);
            TestGenericJoystickControllerUpdate(rightController);
        }

        [Test]
        public void ViveWandControllerUpdateTest()
        {
            OpenVR.Input.ViveWandController leftController = new OpenVR.Input.ViveWandController(TrackingState.NotTracked, Utilities.Handedness.Left);
            OpenVR.Input.ViveWandController rightController = new OpenVR.Input.ViveWandController(TrackingState.NotTracked, Utilities.Handedness.Right);

            TestGenericJoystickControllerUpdate(leftController);
            TestGenericJoystickControllerUpdate(rightController);
        }

        [Test]
        public void WindowsMixedRealityOpenVRMotionControllerUpdateTest()
        {
            OpenVR.Input.WindowsMixedRealityOpenVRMotionController leftController = new OpenVR.Input.WindowsMixedRealityOpenVRMotionController(TrackingState.NotTracked, Utilities.Handedness.Left);
            OpenVR.Input.WindowsMixedRealityOpenVRMotionController rightController = new OpenVR.Input.WindowsMixedRealityOpenVRMotionController(TrackingState.NotTracked, Utilities.Handedness.Right);

            TestGenericJoystickControllerUpdate(leftController);
            TestGenericJoystickControllerUpdate(rightController);
        }
#endif // !UNITY_2020_1_OR_NEWER

        private void TestGenericJoystickControllerUpdate(GenericJoystickController controller)
        {
            // Test
            Assert.That(() => controller.UpdateController(), Throws.Nothing);
        }
    }
}