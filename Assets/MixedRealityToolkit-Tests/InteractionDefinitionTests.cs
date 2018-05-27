using System;
using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using NUnit.Framework;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class InteractionDefinitionTests
    {
        [Test]
        public void Test01_TestTuple()
        {
            var inputDef = new InteractionDefinition(1, AxisType.SixDoF, DeviceInputType.SpatialPointer, new InputAction(1, "Pointer"));
            var testValue = new Tuple<Vector3, Quaternion>(Vector3.one, new Quaternion(45f, 45f, 45f, 45f));

            inputDef.SetValue(testValue);
            var setValue = inputDef.GetTransform();

            Assert.AreEqual(setValue, testValue);
            Assert.AreEqual(setValue.Item1, testValue.Item1);
            Assert.AreEqual(setValue.Item2, testValue.Item2);
        }
    }
}