// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
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
            InputMappingAxisUtility.CheckUnityInputManagerMappings(ControllerMappingLibrary.UnityInputManagerAxes);

            Assert.IsTrue(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.MIXEDREALITY_AXIS1));
            Assert.IsTrue(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.MIXEDREALITY_AXIS2));
            Assert.IsTrue(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.MIXEDREALITY_AXIS4));
            Assert.IsTrue(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.MIXEDREALITY_AXIS5));
            Assert.IsTrue(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.MIXEDREALITY_AXIS9));
            Assert.IsTrue(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.MIXEDREALITY_AXIS10));
            Assert.IsTrue(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.MIXEDREALITY_AXIS11));
            Assert.IsTrue(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.MIXEDREALITY_AXIS12));
            Assert.IsTrue(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.MIXEDREALITY_AXIS13));
            Assert.IsTrue(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.MIXEDREALITY_AXIS14));
            Assert.IsTrue(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.MIXEDREALITY_AXIS15));
            Assert.IsTrue(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.MIXEDREALITY_AXIS16));
            Assert.IsTrue(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.MIXEDREALITY_AXIS17));
            Assert.IsTrue(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.MIXEDREALITY_AXIS18));
            Assert.IsTrue(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.MIXEDREALITY_AXIS19));
            Assert.IsTrue(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.MIXEDREALITY_AXIS20));
            Assert.IsTrue(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.MIXEDREALITY_AXIS21));
            Assert.IsTrue(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.MIXEDREALITY_AXIS22));
            Assert.IsTrue(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.MIXEDREALITY_AXIS23));
            Assert.IsTrue(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.MIXEDREALITY_AXIS24));
            Assert.IsTrue(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.MIXEDREALITY_AXIS25));
            Assert.IsTrue(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.MIXEDREALITY_AXIS26));
            Assert.IsTrue(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.MIXEDREALITY_AXIS27));
        }

        [Test]
        public void Test02_TestRemoveCustomMappings()
        {

            InputMappingAxisUtility.RemoveMappings(ControllerMappingLibrary.UnityInputManagerAxes);

            Assert.IsFalse(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.MIXEDREALITY_AXIS1));
            Assert.IsFalse(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.MIXEDREALITY_AXIS2));
            Assert.IsFalse(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.MIXEDREALITY_AXIS4));
            Assert.IsFalse(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.MIXEDREALITY_AXIS5));
            Assert.IsFalse(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.MIXEDREALITY_AXIS9));
            Assert.IsFalse(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.MIXEDREALITY_AXIS10));
            Assert.IsFalse(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.MIXEDREALITY_AXIS11));
            Assert.IsFalse(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.MIXEDREALITY_AXIS12));
            Assert.IsFalse(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.MIXEDREALITY_AXIS13));
            Assert.IsFalse(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.MIXEDREALITY_AXIS14));
            Assert.IsFalse(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.MIXEDREALITY_AXIS15));
            Assert.IsFalse(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.MIXEDREALITY_AXIS16));
            Assert.IsFalse(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.MIXEDREALITY_AXIS17));
            Assert.IsFalse(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.MIXEDREALITY_AXIS18));
            Assert.IsFalse(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.MIXEDREALITY_AXIS19));
            Assert.IsFalse(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.MIXEDREALITY_AXIS20));
            Assert.IsFalse(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.MIXEDREALITY_AXIS21));
            Assert.IsFalse(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.MIXEDREALITY_AXIS22));
            Assert.IsFalse(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.MIXEDREALITY_AXIS23));
            Assert.IsFalse(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.MIXEDREALITY_AXIS24));
            Assert.IsFalse(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.MIXEDREALITY_AXIS25));
            Assert.IsFalse(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.MIXEDREALITY_AXIS26));
            Assert.IsFalse(InputMappingAxisUtility.DoesAxisNameExist(ControllerMappingLibrary.MIXEDREALITY_AXIS27));
        }
    }
}