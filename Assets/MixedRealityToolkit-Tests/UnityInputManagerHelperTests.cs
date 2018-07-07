// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Utilities;
using NUnit.Framework;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class UnityInputManagerHelperTests
    {
        [Test]
        public void Test01_TestAddDefaultMRTKMappings()
        {
            //First apply settings
            InputMappingAxisUtility.ApplyMappings();
            foreach (var mapping in InputMappingAxisUtility.MRTKDefaultInputMappingNames)
            {
                Assert.IsTrue(InputMappingAxisUtility.DoesAxisNameExist(mapping));
            }
        }

        [Test]
        public void Test02_TestRemoveDefaultMRTKMappings()
        {
            //First apply settings
            InputMappingAxisUtility.ApplyMappings();

            InputMappingAxisUtility.RemoveMappings();

            foreach (var mapping in InputMappingAxisUtility.MRTKDefaultInputMappingNames)
            {
                Assert.IsFalse(InputMappingAxisUtility.DoesAxisNameExist(mapping));
            }
        }

        [Test]
        public void Test03_TestAddCustomMappings()
        {
            //Ensure default mappings are removed
            InputMappingAxisUtility.RemoveMappings();

            InputMappingAxisUtility.InputManagerAxis[] OpenVRControllerAxisMappings =
            {
                new InputMappingAxisUtility.InputManagerAxis() { Name = "OPENVR_TOUCHPAD_LEFT_CONTROLLER_HORIZONTAL",  Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputMappingAxisUtility.MappingAxisType.JoystickAxis, Axis = 1 },
                new InputMappingAxisUtility.InputManagerAxis() { Name = "OPENVR_TOUCHPAD_LEFT_CONTROLLER_VERTICAL",    Dead = 0.001f, Sensitivity = 1, Invert = false,  Type = InputMappingAxisUtility.MappingAxisType.JoystickAxis, Axis = 2 },
                new InputMappingAxisUtility.InputManagerAxis() { Name = "OPENVR_TOUCHPAD_RIGHT_CONTROLLER_HORIZONTAL",  Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputMappingAxisUtility.MappingAxisType.JoystickAxis, Axis = 4 },
                new InputMappingAxisUtility.InputManagerAxis() { Name = "OPENVR_TOUCHPAD_RIGHT_CONTROLLER_VERTICAL",    Dead = 0.001f, Sensitivity = 1, Invert = false,  Type = InputMappingAxisUtility.MappingAxisType.JoystickAxis, Axis = 5 },
                new InputMappingAxisUtility.InputManagerAxis() { Name = "OPENVR_TRIGGER_LEFT_CONTROLLER",    Dead = 0.001f, Sensitivity = 1, Invert = false,  Type = InputMappingAxisUtility.MappingAxisType.JoystickAxis, Axis = 9 },
                new InputMappingAxisUtility.InputManagerAxis() { Name = "OPENVR_TRIGGER_RIGHT_CONTROLLER",    Dead = 0.001f, Sensitivity = 1, Invert = false,  Type = InputMappingAxisUtility.MappingAxisType.JoystickAxis, Axis = 10 },
                new InputMappingAxisUtility.InputManagerAxis() { Name = "OPENVR_GRIP_LEFT_CONTROLLER",    Dead = 0.001f, Sensitivity = 1, Invert = false,  Type = InputMappingAxisUtility.MappingAxisType.JoystickAxis, Axis = 11 },
                new InputMappingAxisUtility.InputManagerAxis() { Name = "OPENVR_GRIP_RIGHT_CONTROLLER",    Dead = 0.001f, Sensitivity = 1, Invert = false,  Type = InputMappingAxisUtility.MappingAxisType.JoystickAxis, Axis = 12 }
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
        public void Test04_TestRemoveCustomMappings()
        {
            //Ensure default mappings are removed
            InputMappingAxisUtility.RemoveMappings();

            InputMappingAxisUtility.InputManagerAxis[] OpenVRControllerAxisMappings =
            {
                new InputMappingAxisUtility.InputManagerAxis() { Name = "OPENVR_TOUCHPAD_LEFT_CONTROLLER_HORIZONTAL",  Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputMappingAxisUtility.MappingAxisType.JoystickAxis, Axis = 1 },
                new InputMappingAxisUtility.InputManagerAxis() { Name = "OPENVR_TOUCHPAD_LEFT_CONTROLLER_VERTICAL",    Dead = 0.001f, Sensitivity = 1, Invert = false,  Type = InputMappingAxisUtility.MappingAxisType.JoystickAxis, Axis = 2 },
                new InputMappingAxisUtility.InputManagerAxis() { Name = "OPENVR_TOUCHPAD_RIGHT_CONTROLLER_HORIZONTAL",  Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputMappingAxisUtility.MappingAxisType.JoystickAxis, Axis = 4 },
                new InputMappingAxisUtility.InputManagerAxis() { Name = "OPENVR_TOUCHPAD_RIGHT_CONTROLLER_VERTICAL",    Dead = 0.001f, Sensitivity = 1, Invert = false,  Type = InputMappingAxisUtility.MappingAxisType.JoystickAxis, Axis = 5 },
                new InputMappingAxisUtility.InputManagerAxis() { Name = "OPENVR_TRIGGER_LEFT_CONTROLLER",    Dead = 0.001f, Sensitivity = 1, Invert = false,  Type = InputMappingAxisUtility.MappingAxisType.JoystickAxis, Axis = 9 },
                new InputMappingAxisUtility.InputManagerAxis() { Name = "OPENVR_TRIGGER_RIGHT_CONTROLLER",    Dead = 0.001f, Sensitivity = 1, Invert = false,  Type = InputMappingAxisUtility.MappingAxisType.JoystickAxis, Axis = 10 },
                new InputMappingAxisUtility.InputManagerAxis() { Name = "OPENVR_GRIP_LEFT_CONTROLLER",    Dead = 0.001f, Sensitivity = 1, Invert = false,  Type = InputMappingAxisUtility.MappingAxisType.JoystickAxis, Axis = 11 },
                new InputMappingAxisUtility.InputManagerAxis() { Name = "OPENVR_GRIP_RIGHT_CONTROLLER",    Dead = 0.001f, Sensitivity = 1, Invert = false,  Type = InputMappingAxisUtility.MappingAxisType.JoystickAxis, Axis = 12 }
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