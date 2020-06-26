// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input.Editor;
using Microsoft.MixedReality.Toolkit.Input;
using NUnit.Framework;

namespace Microsoft.MixedReality.Toolkit.Tests.EditMode.InputSystem
{
    public class UnityInputManagerHelperTests
    {
        [Test]
        public void TestAddCustomMappings()
        {
            InputMappingAxisUtility.CheckUnityInputManagerMappings(ControllerMappingLibrary.UnityInputManagerAxes);

            Assert.IsTrue(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.AXIS_1));
            Assert.IsTrue(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.AXIS_2));
            Assert.IsTrue(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.AXIS_4));
            Assert.IsTrue(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.AXIS_5));
            Assert.IsTrue(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.AXIS_9));
            Assert.IsTrue(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.AXIS_10));
            Assert.IsTrue(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.AXIS_11));
            Assert.IsTrue(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.AXIS_12));
            Assert.IsTrue(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.AXIS_13));
            Assert.IsTrue(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.AXIS_14));
            Assert.IsTrue(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.AXIS_15));
            Assert.IsTrue(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.AXIS_16));
            Assert.IsTrue(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.AXIS_17));
            Assert.IsTrue(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.AXIS_18));
            Assert.IsTrue(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.AXIS_19));
            Assert.IsTrue(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.AXIS_20));
            Assert.IsTrue(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.AXIS_21));
            Assert.IsTrue(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.AXIS_22));
            Assert.IsTrue(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.AXIS_23));
            Assert.IsTrue(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.AXIS_24));
            Assert.IsTrue(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.AXIS_25));
            Assert.IsTrue(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.AXIS_26));
            Assert.IsTrue(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.AXIS_27));
        }

        [Test]
        public void TestRemoveCustomMappings()
        {
            InputMappingAxisUtility.RemoveMappings(ControllerMappingLibrary.UnityInputManagerAxes);

            Assert.IsFalse(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.AXIS_1));
            Assert.IsFalse(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.AXIS_2));
            Assert.IsFalse(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.AXIS_4));
            Assert.IsFalse(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.AXIS_5));
            Assert.IsFalse(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.AXIS_9));
            Assert.IsFalse(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.AXIS_10));
            Assert.IsFalse(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.AXIS_11));
            Assert.IsFalse(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.AXIS_12));
            Assert.IsFalse(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.AXIS_13));
            Assert.IsFalse(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.AXIS_14));
            Assert.IsFalse(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.AXIS_15));
            Assert.IsFalse(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.AXIS_16));
            Assert.IsFalse(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.AXIS_17));
            Assert.IsFalse(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.AXIS_18));
            Assert.IsFalse(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.AXIS_19));
            Assert.IsFalse(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.AXIS_20));
            Assert.IsFalse(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.AXIS_21));
            Assert.IsFalse(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.AXIS_22));
            Assert.IsFalse(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.AXIS_23));
            Assert.IsFalse(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.AXIS_24));
            Assert.IsFalse(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.AXIS_25));
            Assert.IsFalse(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.AXIS_26));
            Assert.IsFalse(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.AXIS_27));
        }

        [TearDown]
        public void TearDown()
        {
            // Put the mappings back.
            InputMappingAxisUtility.CheckUnityInputManagerMappings(ControllerMappingLibrary.UnityInputManagerAxes);
        }
    }
}