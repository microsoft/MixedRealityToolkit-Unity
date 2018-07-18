// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Utilities;
using NUnit.Framework;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class UnityInputManagerHelperTests
    {
        [Test]
        public void Test01_TestAddCustomMappings()
        {
            InputManagerAxis[] OpenVRControllerAxisMappings =
            {
                new InputManagerAxis() { Name = "OPENVR_TOUCHPAD_LEFT_CONTROLLER_HORIZONTAL",  Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 1 },
                new InputManagerAxis() { Name = "OPENVR_TOUCHPAD_LEFT_CONTROLLER_VERTICAL",    Dead = 0.001f, Sensitivity = 1, Invert = false,  Type = InputManagerAxisType.JoystickAxis, Axis = 2 },
                new InputManagerAxis() { Name = "OPENVR_TOUCHPAD_RIGHT_CONTROLLER_HORIZONTAL",  Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 4 },
                new InputManagerAxis() { Name = "OPENVR_TOUCHPAD_RIGHT_CONTROLLER_VERTICAL",    Dead = 0.001f, Sensitivity = 1, Invert = false,  Type = InputManagerAxisType.JoystickAxis, Axis = 5 },
                new InputManagerAxis() { Name = "OPENVR_TRIGGER_LEFT_CONTROLLER",    Dead = 0.001f, Sensitivity = 1, Invert = false,  Type = InputManagerAxisType.JoystickAxis, Axis = 9 },
                new InputManagerAxis() { Name = "OPENVR_TRIGGER_RIGHT_CONTROLLER",    Dead = 0.001f, Sensitivity = 1, Invert = false,  Type = InputManagerAxisType.JoystickAxis, Axis = 10 },
                new InputManagerAxis() { Name = "OPENVR_GRIP_LEFT_CONTROLLER",    Dead = 0.001f, Sensitivity = 1, Invert = false,  Type = InputManagerAxisType.JoystickAxis, Axis = 11 },
                new InputManagerAxis() { Name = "OPENVR_GRIP_RIGHT_CONTROLLER",    Dead = 0.001f, Sensitivity = 1, Invert = false,  Type = InputManagerAxisType.JoystickAxis, Axis = 12 }
            };

            InputMappingAxisUtility.ApplyMappings(OpenVRControllerAxisMappings);

            Assert.IsTrue(InputMappingAxisUtility.DoesAxisNameExist("OPENVR_TOUCHPAD_LEFT_CONTROLLER_HORIZONTAL"));
            Assert.IsTrue(InputMappingAxisUtility.DoesAxisNameExist("OPENVR_TOUCHPAD_LEFT_CONTROLLER_VERTICAL"));
            Assert.IsTrue(InputMappingAxisUtility.DoesAxisNameExist("OPENVR_TOUCHPAD_RIGHT_CONTROLLER_HORIZONTAL"));
            Assert.IsTrue(InputMappingAxisUtility.DoesAxisNameExist("OPENVR_TOUCHPAD_RIGHT_CONTROLLER_VERTICAL"));
            Assert.IsTrue(InputMappingAxisUtility.DoesAxisNameExist("OPENVR_TRIGGER_LEFT_CONTROLLER"));
            Assert.IsTrue(InputMappingAxisUtility.DoesAxisNameExist("OPENVR_TRIGGER_RIGHT_CONTROLLER"));
            Assert.IsTrue(InputMappingAxisUtility.DoesAxisNameExist("OPENVR_GRIP_LEFT_CONTROLLER"));
            Assert.IsTrue(InputMappingAxisUtility.DoesAxisNameExist("OPENVR_GRIP_RIGHT_CONTROLLER"));
        }

        [Test]
        public void Test02_TestRemoveCustomMappings()
        {
            InputManagerAxis[] OpenVRControllerAxisMappings =
            {
                new InputManagerAxis() { Name = "OPENVR_TOUCHPAD_LEFT_CONTROLLER_HORIZONTAL",  Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 1 },
                new InputManagerAxis() { Name = "OPENVR_TOUCHPAD_LEFT_CONTROLLER_VERTICAL",    Dead = 0.001f, Sensitivity = 1, Invert = false,  Type = InputManagerAxisType.JoystickAxis, Axis = 2 },
                new InputManagerAxis() { Name = "OPENVR_TOUCHPAD_RIGHT_CONTROLLER_HORIZONTAL",  Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 4 },
                new InputManagerAxis() { Name = "OPENVR_TOUCHPAD_RIGHT_CONTROLLER_VERTICAL",    Dead = 0.001f, Sensitivity = 1, Invert = false,  Type = InputManagerAxisType.JoystickAxis, Axis = 5 },
                new InputManagerAxis() { Name = "OPENVR_TRIGGER_LEFT_CONTROLLER",    Dead = 0.001f, Sensitivity = 1, Invert = false,  Type = InputManagerAxisType.JoystickAxis, Axis = 9 },
                new InputManagerAxis() { Name = "OPENVR_TRIGGER_RIGHT_CONTROLLER",    Dead = 0.001f, Sensitivity = 1, Invert = false,  Type = InputManagerAxisType.JoystickAxis, Axis = 10 },
                new InputManagerAxis() { Name = "OPENVR_GRIP_LEFT_CONTROLLER",    Dead = 0.001f, Sensitivity = 1, Invert = false,  Type = InputManagerAxisType.JoystickAxis, Axis = 11 },
                new InputManagerAxis() { Name = "OPENVR_GRIP_RIGHT_CONTROLLER",    Dead = 0.001f, Sensitivity = 1, Invert = false,  Type = InputManagerAxisType.JoystickAxis, Axis = 12 }
            };

            InputMappingAxisUtility.RemoveMappings(OpenVRControllerAxisMappings);

            Assert.IsFalse(InputMappingAxisUtility.DoesAxisNameExist("OPENVR_TOUCHPAD_LEFT_CONTROLLER_HORIZONTAL"));
            Assert.IsFalse(InputMappingAxisUtility.DoesAxisNameExist("OPENVR_TOUCHPAD_LEFT_CONTROLLER_VERTICAL"));
            Assert.IsFalse(InputMappingAxisUtility.DoesAxisNameExist("OPENVR_TOUCHPAD_RIGHT_CONTROLLER_HORIZONTAL"));
            Assert.IsFalse(InputMappingAxisUtility.DoesAxisNameExist("OPENVR_TOUCHPAD_RIGHT_CONTROLLER_VERTICAL"));
            Assert.IsFalse(InputMappingAxisUtility.DoesAxisNameExist("OPENVR_TRIGGER_LEFT_CONTROLLER"));
            Assert.IsFalse(InputMappingAxisUtility.DoesAxisNameExist("OPENVR_TRIGGER_RIGHT_CONTROLLER"));
            Assert.IsFalse(InputMappingAxisUtility.DoesAxisNameExist("OPENVR_GRIP_LEFT_CONTROLLER"));
            Assert.IsFalse(InputMappingAxisUtility.DoesAxisNameExist("OPENVR_GRIP_RIGHT_CONTROLLER"));
        }
    }
}