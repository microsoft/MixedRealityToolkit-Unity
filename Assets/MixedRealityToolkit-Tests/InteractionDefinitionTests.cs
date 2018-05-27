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
        public void Test01_TestTupleChanged()
        {
            var inputDef = new InteractionDefinition(1, AxisType.SixDoF, DeviceInputType.SpatialPointer, new InputAction(1, "Pointer"));
            var testValue = new Tuple<Vector3, Quaternion>(Vector3.one, new Quaternion(45f, 45f, 45f, 45f));

            inputDef.SetValue(testValue);
            var setValue = inputDef.GetTransform();

            Assert.AreEqual(setValue, testValue);
            Assert.AreEqual(setValue.Item1, testValue.Item1);
            Assert.AreEqual(setValue.Item2, testValue.Item2);
            Assert.AreEqual(inputDef.Changed, true);
        }

        [Test]
        public void Test02_TestTupleChanged()
        {
            var inputDef = new InteractionDefinition(2, AxisType.SixDoF, DeviceInputType.SpatialPointer, new InputAction(1, "Pointer"));
            var testValue = new Tuple<Vector3, Quaternion>(Vector3.one, new Quaternion(45f, 45f, 45f, 45f));

            inputDef.SetValue(testValue);
            var setValue = inputDef.GetTransform();

            Assert.AreEqual(inputDef.Changed, true);

            inputDef.SetValue(testValue);

            Assert.AreEqual(inputDef.Changed, false);
        }

        [Test]
        public void Test03_TestTupleGeneric()
        {
            var inputDef = new InteractionDefinition(3, AxisType.SixDoF, DeviceInputType.SpatialPointer, new InputAction(1, "Pointer"));
            var testValue = new Tuple<Vector3, Quaternion>(Vector3.one, new Quaternion(45f, 45f, 45f, 45f));

            inputDef.SetValue<Tuple<Vector3, Quaternion>>(testValue);
            var setValue = inputDef.GetValue<Tuple<Vector3,Quaternion>>();

            Assert.AreEqual(setValue, testValue);
            Assert.AreEqual(setValue.Item1, testValue.Item1);
            Assert.AreEqual(setValue.Item2, testValue.Item2);
            Assert.AreEqual(inputDef.Changed, true);
        }

        [Test]
        public void Test04_TestTupleGenericChanged()
        {
            var inputDef = new InteractionDefinition(3, AxisType.SixDoF, DeviceInputType.SpatialPointer, new InputAction(1, "Pointer"));
            var testValue = new Tuple<Vector3, Quaternion>(Vector3.one, new Quaternion(45f, 45f, 45f, 45f));

            inputDef.SetValue<Tuple<Vector3, Quaternion>>(testValue);
            var setValue = inputDef.GetValue<Tuple<Vector3, Quaternion>>();

            Assert.AreEqual(inputDef.Changed, true);

            inputDef.SetValue<Tuple<Vector3, Quaternion>>(testValue);

            Assert.AreEqual(inputDef.Changed, false);


        }
    }
}