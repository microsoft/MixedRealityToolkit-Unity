// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Extensions;
using NUnit.Framework;
using System;
using System.Diagnostics;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class InteractionDefinitionTests
    {
        private const int SpeedTestIterations = 1000000;

        #region objects

        [Test]
        public void Test01_TestObjectChanged()
        {
            var inputDef = new InteractionMapping(1, AxisType.Raw, DeviceInputType.None, new InputAction(1, "None"));
            var testValue1 = (object)1f;
            var testValue2 = (object)false;

            var initialValue = inputDef.GetRawValue();

            Assert.IsNull(initialValue);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue(testValue1);

            Assert.IsTrue(inputDef.Changed);

            var setValue1 = inputDef.GetRawValue();

            Assert.IsNotNull(setValue1);
            Assert.AreEqual(setValue1, testValue1);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue(testValue2);

            Assert.IsTrue(inputDef.Changed);

            var setValue2 = inputDef.GetRawValue();

            Assert.IsNotNull(setValue2);
            Assert.AreEqual(setValue2, testValue2);
            Assert.IsFalse(inputDef.Changed);
        }

        [Test]
        public void Test02_TestObjectNoChange()
        {
            var inputDef = new InteractionMapping(1, AxisType.Raw, DeviceInputType.None, new InputAction(1, "None"));
            var testValue = new object();

            var initialValue = inputDef.GetRawValue();

            Assert.IsNull(initialValue);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue(testValue);

            Assert.IsTrue(inputDef.Changed);
            // Make sure the second time we query it's false
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue(testValue);

            // Make sure if we set the same value it's false
            Assert.IsFalse(inputDef.Changed);
        }

        #endregion objects

        #region bools

        [Test]
        public void Test03_TestBoolChanged()
        {
            var inputDef = new InteractionMapping(1, AxisType.Digital, DeviceInputType.None, new InputAction(1, "None"));
            var testValue1 = true;
            var testValue2 = false;

            var initialValue = inputDef.GetBooleanValue();

            Assert.IsFalse(initialValue);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue(testValue1);

            Assert.IsTrue(inputDef.Changed);

            var setValue1 = inputDef.GetBooleanValue();

            Assert.IsTrue(setValue1);
            Assert.True(setValue1 == testValue1);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue(testValue2);

            Assert.IsTrue(inputDef.Changed);

            var setValue2 = inputDef.GetBooleanValue();

            Assert.IsFalse(setValue2);
            Assert.True(setValue2 == testValue2);
            Assert.IsFalse(inputDef.Changed);
        }

        [Test]
        public void Test04_TestBoolNoChange()
        {
            var inputDef = new InteractionMapping(1, AxisType.Digital, DeviceInputType.None, new InputAction(1, "None"));
            var testValue = true;

            var initialValue = inputDef.GetBooleanValue();

            Assert.IsFalse(initialValue);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue(testValue);

            Assert.IsTrue(inputDef.Changed);
            // Make sure the second time we query it's false
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue(testValue);

            // Make sure if we set the same value it's false
            Assert.IsFalse(inputDef.Changed);
        }

        #endregion bools

        #region float

        [Test]
        public void Test05_TestFloatChanged()
        {
            var inputDef = new InteractionMapping(1, AxisType.SingleAxis, DeviceInputType.None, new InputAction(1, "None"));
            var testValue1 = 1f;
            var testValue2 = 9001f;

            var initialValue = inputDef.GetFloatValue();

            Assert.AreEqual(initialValue, 0d, double.Epsilon);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue(testValue1);

            Assert.IsTrue(inputDef.Changed);

            var setValue1 = inputDef.GetFloatValue();

            Assert.AreEqual(setValue1, testValue1, double.Epsilon);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue(testValue2);

            Assert.IsTrue(inputDef.Changed);

            var setValue2 = inputDef.GetFloatValue();

            Assert.AreEqual(setValue2, testValue2, double.Epsilon);
            Assert.IsFalse(inputDef.Changed);
        }

        [Test]
        public void Test06_TestFloatNoChange()
        {
            var inputDef = new InteractionMapping(1, AxisType.SingleAxis, DeviceInputType.None, new InputAction(1, "None"));
            var testValue = 1f;

            var initialValue = inputDef.GetFloatValue();

            Assert.AreEqual(initialValue, 0d, double.Epsilon);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue(testValue);

            Assert.IsTrue(inputDef.Changed);
            // Make sure the second time we query it's false
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue(testValue);

            // Make sure if we set the same value it's false
            Assert.IsFalse(inputDef.Changed);
        }

        #endregion float

        #region Vector2

        [Test]
        public void Test07_TestVector2Changed()
        {
            var inputDef = new InteractionMapping(1, AxisType.DualAxis, DeviceInputType.None, new InputAction(1, "None"));
            var testValue1 = Vector2.one;
            var testValue2 = Vector2.zero;

            var initialValue = inputDef.GetVector2Value();

            Assert.True(initialValue == Vector2.zero);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue(testValue1);

            Assert.IsTrue(inputDef.Changed);

            var setValue1 = inputDef.GetVector2Value();

            Assert.True(setValue1 == testValue1);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue(testValue2);

            Assert.IsTrue(inputDef.Changed);

            var setValue2 = inputDef.GetVector2Value();

            Assert.True(setValue2 == testValue2);
            Assert.IsFalse(inputDef.Changed);
        }

        [Test]
        public void Test08_TestVector2NoChange()
        {
            var inputDef = new InteractionMapping(1, AxisType.DualAxis, DeviceInputType.None, new InputAction(1, "None"));
            var testValue = Vector2.one;

            var initialValue = inputDef.GetVector2Value();

            Assert.True(initialValue == Vector2.zero);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue(testValue);

            Assert.IsTrue(inputDef.Changed);
            // Make sure the second time we query it's false
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue(testValue);

            // Make sure if we set the same value it's false
            Assert.IsFalse(inputDef.Changed);
        }

        #endregion Vector2

        #region Vector3

        [Test]
        public void Test09_TestVector3Changed()
        {
            var inputDef = new InteractionMapping(1, AxisType.ThreeDoFPosition, DeviceInputType.None, new InputAction(1, "None"));
            var testValue1 = Vector3.one;
            var testValue2 = Vector3.zero;

            var initialValue = inputDef.GetPosition();

            Assert.True(initialValue == Vector3.zero);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue(testValue1);

            Assert.IsTrue(inputDef.Changed);

            var setValue1 = inputDef.GetPosition();

            Assert.True(setValue1 == testValue1);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue(testValue2);

            Assert.IsTrue(inputDef.Changed);

            var setValue2 = inputDef.GetPosition();

            Assert.True(setValue2 == testValue2);
            Assert.IsFalse(inputDef.Changed);
        }

        [Test]
        public void Test10_TestVector3NoChange()
        {
            var inputDef = new InteractionMapping(1, AxisType.ThreeDoFPosition, DeviceInputType.None, new InputAction(1, "None"));
            var testValue = Vector3.one;

            var initialValue = inputDef.GetPosition();

            Assert.True(initialValue == Vector3.zero);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue(testValue);

            Assert.IsTrue(inputDef.Changed);
            // Make sure the second time we query it's false
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue(testValue);

            // Make sure if we set the same value it's false
            Assert.IsFalse(inputDef.Changed);
        }

        #endregion Vector3

        #region Quaternion

        [Test]
        public void Test11_TestQuaternionChanged()
        {
            var inputDef = new InteractionMapping(1, AxisType.ThreeDoFRotation, DeviceInputType.None, new InputAction(1, "None"));
            var testValue1 = Quaternion.Euler(45f, 45f, 45f);
            var testValue2 = Quaternion.identity;

            var initialValue = inputDef.GetRotation();

            Assert.True(initialValue == Quaternion.identity);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue(testValue1);

            Assert.IsTrue(inputDef.Changed);

            var setValue1 = inputDef.GetRotation();

            Assert.True(setValue1 == testValue1);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue(testValue2);

            Assert.IsTrue(inputDef.Changed);

            var setValue2 = inputDef.GetRotation();

            Assert.True(setValue2 == testValue2);
            Assert.IsFalse(inputDef.Changed);
        }

        [Test]
        public void Test12_TestQuaternionNoChange()
        {
            var inputDef = new InteractionMapping(1, AxisType.ThreeDoFRotation, DeviceInputType.None, new InputAction(1, "None"));
            var testValue = Quaternion.Euler(45f, 45f, 45f);

            var initialValue = inputDef.GetRotation();

            Assert.True(initialValue == Quaternion.identity);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue(testValue);

            Assert.IsTrue(inputDef.Changed);
            // Make sure the second time we query it's false
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue(testValue);

            // Make sure if we set the same value it's false
            Assert.IsFalse(inputDef.Changed);
        }

        #endregion Quaternion

        #region Tuples

        [Test]
        public void Test13_TestTupleChanged()
        {
            var inputDef = new InteractionMapping(1, AxisType.SixDoF, DeviceInputType.None, new InputAction(1, "None"));
            var testValue1 = new Tuple<Vector3, Quaternion>(Vector3.zero, Quaternion.identity);
            var testValue2 = new Tuple<Vector3, Quaternion>(Vector3.one, new Quaternion(45f, 45f, 45f, 45f));

            var initialValue = inputDef.GetTransform();

            Assert.IsNull(initialValue);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue(testValue1);

            Assert.IsTrue(inputDef.Changed);

            var setValue1 = inputDef.GetTransform();

            Assert.AreEqual(setValue1, testValue1);
            Assert.AreEqual(setValue1.Item1, testValue1.Item1);
            Assert.AreEqual(setValue1.Item2, testValue1.Item2);
            Assert.AreEqual(setValue1.Item2, testValue1.Item2);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue(testValue2);

            Assert.IsTrue(inputDef.Changed);

            var setValue2 = inputDef.GetTransform();

            Assert.AreEqual(setValue2, testValue2);
            Assert.AreEqual(setValue2.Item1, testValue2.Item1);
            Assert.AreEqual(setValue2.Item2, testValue2.Item2);
            Assert.AreEqual(setValue2.Item2, testValue2.Item2);
            Assert.IsFalse(inputDef.Changed);
        }

        [Test]
        public void Test14_TestTupleNoChange()
        {
            var inputDef = new InteractionMapping(1, AxisType.SixDoF, DeviceInputType.None, new InputAction(1, "None"));
            var testValue = new Tuple<Vector3, Quaternion>(Vector3.zero, Quaternion.identity);

            var initialValue = inputDef.GetTransform();

            Assert.IsNull(initialValue);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue(testValue);

            Assert.IsTrue(inputDef.Changed);
            // Make sure the second time we query it's false
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue(testValue);

            // Make sure if we set the same value it's false
            Assert.IsFalse(inputDef.Changed);
        }

        #endregion Tuples

        #region Interaction Dictionary Tests

        [Test]
        public void Test15_InteractionDictionaryObject()
        {
            var Interactions = new System.Collections.Generic.Dictionary<DeviceInputType, InteractionMapping>();
            Interactions.Add(DeviceInputType.None, new InteractionMapping(1, AxisType.Raw, DeviceInputType.None, new InputAction(1, "None")));
            var testValue1 = (object)1f;
            var testValue2 = (object)false;

            var initialValue = Interactions[DeviceInputType.None].GetRawValue();

            Assert.IsNull(initialValue);
            Assert.IsFalse(Interactions.GetDictionaryValueChanged(DeviceInputType.None));

            Interactions.SetDictionaryValue(DeviceInputType.None, testValue1);

            Assert.IsTrue(Interactions.GetDictionaryValueChanged(DeviceInputType.None));

            var setValue1 = Interactions[DeviceInputType.None].GetRawValue();

            Assert.IsNotNull(setValue1);
            Assert.AreEqual(setValue1, testValue1);
            Assert.IsFalse(Interactions.GetDictionaryValueChanged(DeviceInputType.None));

            Interactions.SetDictionaryValue(DeviceInputType.None, testValue2);

            Assert.IsTrue(Interactions.GetDictionaryValueChanged(DeviceInputType.None));

            var setValue2 = Interactions[DeviceInputType.None].GetRawValue();

            Assert.IsNotNull(setValue2);
            Assert.AreEqual(setValue2, testValue2);
            Assert.IsFalse(Interactions.GetDictionaryValueChanged(DeviceInputType.None));
        }

        [Test]
        public void Test16_InteractionDictionaryBool()
        {
            var Interactions = new System.Collections.Generic.Dictionary<DeviceInputType, InteractionMapping>();
            Interactions.Add(DeviceInputType.None, new InteractionMapping(1, AxisType.Digital, DeviceInputType.None, new InputAction(1, "None")));
            var testValue1 = true;
            var testValue2 = false;

            var initialValue = Interactions[DeviceInputType.None].GetBooleanValue();

            Assert.IsFalse(initialValue);
            Assert.IsFalse(Interactions.GetDictionaryValueChanged(DeviceInputType.None));

            Interactions.SetDictionaryValue(DeviceInputType.None, testValue1);

            Assert.IsTrue(Interactions.GetDictionaryValueChanged(DeviceInputType.None));

            var setValue1 = Interactions[DeviceInputType.None].GetBooleanValue();

            Assert.IsTrue(setValue1);
            Assert.AreEqual(setValue1, testValue1);
            Assert.IsFalse(Interactions.GetDictionaryValueChanged(DeviceInputType.None));

            Interactions.SetDictionaryValue(DeviceInputType.None, testValue2);

            Assert.IsTrue(Interactions.GetDictionaryValueChanged(DeviceInputType.None));

            var setValue2 = Interactions[DeviceInputType.None].GetBooleanValue();

            Assert.IsFalse(setValue2);
            Assert.AreEqual(setValue2, testValue2);
            Assert.IsFalse(Interactions.GetDictionaryValueChanged(DeviceInputType.None));
        }

        [Test]
        public void Test17_InteractionDictionaryFloat()
        {
            var Interactions = new System.Collections.Generic.Dictionary<DeviceInputType, InteractionMapping>();
            Interactions.Add(DeviceInputType.None, new InteractionMapping(1, AxisType.SingleAxis, DeviceInputType.None, new InputAction(1, "None")));
            var testValue1 = 1f;
            var testValue2 = 9001f;

            var initialValue = Interactions[DeviceInputType.None].GetFloatValue();

            Assert.AreEqual(initialValue, 0d, double.Epsilon);
            Assert.IsFalse(Interactions.GetDictionaryValueChanged(DeviceInputType.None));

            Interactions.SetDictionaryValue(DeviceInputType.None, testValue1);

            Assert.IsTrue(Interactions.GetDictionaryValueChanged(DeviceInputType.None));

            var setValue1 = Interactions[DeviceInputType.None].GetFloatValue();

            Assert.AreEqual(setValue1, testValue1, double.Epsilon);
            Assert.IsFalse(Interactions.GetDictionaryValueChanged(DeviceInputType.None));

            Interactions.SetDictionaryValue(DeviceInputType.None, testValue2);

            Assert.IsTrue(Interactions.GetDictionaryValueChanged(DeviceInputType.None));

            var setValue2 = Interactions[DeviceInputType.None].GetFloatValue();

            Assert.AreEqual(setValue2, testValue2, double.Epsilon);
            Assert.IsFalse(Interactions.GetDictionaryValueChanged(DeviceInputType.None));
        }

        [Test]
        public void Test18_InteractionDictionaryVector2()
        {
            var Interactions = new System.Collections.Generic.Dictionary<DeviceInputType, InteractionMapping>();
            Interactions.Add(DeviceInputType.None, new InteractionMapping(1, AxisType.DualAxis, DeviceInputType.None, new InputAction(1, "None")));
            var testValue1 = Vector2.one;
            var testValue2 = Vector2.zero;

            var initialValue = Interactions[DeviceInputType.None].GetVector2Value();

            Assert.True(initialValue == Vector2.zero);
            Assert.IsFalse(Interactions.GetDictionaryValueChanged(DeviceInputType.None));

            Interactions.SetDictionaryValue(DeviceInputType.None, testValue1);

            Assert.IsTrue(Interactions.GetDictionaryValueChanged(DeviceInputType.None));

            var setValue1 = Interactions[DeviceInputType.None].GetVector2Value();

            Assert.True(setValue1 == testValue1);
            Assert.IsFalse(Interactions.GetDictionaryValueChanged(DeviceInputType.None));

            Interactions.SetDictionaryValue(DeviceInputType.None, testValue2);

            Assert.IsTrue(Interactions.GetDictionaryValueChanged(DeviceInputType.None));

            var setValue2 = Interactions[DeviceInputType.None].GetVector2Value();

            Assert.True(setValue2 == testValue2);
            Assert.IsFalse(Interactions.GetDictionaryValueChanged(DeviceInputType.None));
        }

        [Test]
        public void Test19_InteractionDictionaryVector3()
        {
            var Interactions = new System.Collections.Generic.Dictionary<DeviceInputType, InteractionMapping>();
            Interactions.Add(DeviceInputType.None, new InteractionMapping(1, AxisType.ThreeDoFPosition, DeviceInputType.None, new InputAction(1, "None")));
            var testValue1 = Vector3.one;
            var testValue2 = Vector3.zero;

            var initialValue = Interactions[DeviceInputType.None].GetPosition();

            Assert.True(initialValue == Vector3.zero);
            Assert.IsFalse(Interactions.GetDictionaryValueChanged(DeviceInputType.None));

            Interactions.SetDictionaryValue(DeviceInputType.None, testValue1);

            Assert.IsTrue(Interactions.GetDictionaryValueChanged(DeviceInputType.None));

            var setValue1 = Interactions[DeviceInputType.None].GetPosition();

            Assert.True(setValue1 == testValue1);
            Assert.IsFalse(Interactions.GetDictionaryValueChanged(DeviceInputType.None));

            Interactions.SetDictionaryValue(DeviceInputType.None, testValue2);

            Assert.IsTrue(Interactions.GetDictionaryValueChanged(DeviceInputType.None));

            var setValue2 = Interactions[DeviceInputType.None].GetPosition();

            Assert.True(setValue2 == testValue2);
            Assert.IsFalse(Interactions.GetDictionaryValueChanged(DeviceInputType.None));
        }

        [Test]
        public void Test20_InteractionDictionaryQuaternion()
        {
            var Interactions = new System.Collections.Generic.Dictionary<DeviceInputType, InteractionMapping>();
            Interactions.Add(DeviceInputType.None, new InteractionMapping(1, AxisType.ThreeDoFRotation, DeviceInputType.None, new InputAction(1, "None")));
            var testValue1 = Quaternion.Euler(45f, 45f, 45f);
            var testValue2 = Quaternion.identity;

            var initialValue = Interactions[DeviceInputType.None].GetRotation();

            Assert.True(initialValue == Quaternion.identity);
            Assert.IsFalse(Interactions.GetDictionaryValueChanged(DeviceInputType.None));

            Interactions.SetDictionaryValue(DeviceInputType.None, testValue1);

            Assert.IsTrue(Interactions.GetDictionaryValueChanged(DeviceInputType.None));

            var setValue1 = Interactions[DeviceInputType.None].GetRotation();

            Assert.True(setValue1 == testValue1);
            Assert.IsFalse(Interactions.GetDictionaryValueChanged(DeviceInputType.None));

            Interactions.SetDictionaryValue(DeviceInputType.None, testValue2);

            Assert.IsTrue(Interactions.GetDictionaryValueChanged(DeviceInputType.None));

            var setValue2 = Interactions[DeviceInputType.None].GetRotation();

            Assert.True(setValue2 == testValue2);
            Assert.IsFalse(Interactions.GetDictionaryValueChanged(DeviceInputType.None));
        }

        [Test]
        public void Test21_InteractionDictionaryTuples()
        {
            var Interactions = new System.Collections.Generic.Dictionary<DeviceInputType, InteractionMapping>();
            Interactions.Add(DeviceInputType.None, new InteractionMapping(1, AxisType.SixDoF, DeviceInputType.None, new InputAction(1, "None")));
            var testValue1 = new Tuple<Vector3, Quaternion>(Vector3.zero, Quaternion.identity);
            var testValue2 = new Tuple<Vector3, Quaternion>(Vector3.one, new Quaternion(45f, 45f, 45f, 45f));

            var initialValue = Interactions[DeviceInputType.None].GetTransform();

            Assert.IsNull(initialValue);
            Assert.IsFalse(Interactions.GetDictionaryValueChanged(DeviceInputType.None));

            Interactions.SetDictionaryValue(DeviceInputType.None, testValue1);

            Assert.IsTrue(Interactions.GetDictionaryValueChanged(DeviceInputType.None));

            var setValue1 = Interactions[DeviceInputType.None].GetTransform();

            Assert.AreEqual(setValue1, testValue1);
            Assert.AreEqual(setValue1.Item1, testValue1.Item1);
            Assert.AreEqual(setValue1.Item2, testValue1.Item2);
            Assert.AreEqual(setValue1.Item2, testValue1.Item2);
            Assert.IsFalse(Interactions.GetDictionaryValueChanged(DeviceInputType.None));

            Interactions.SetDictionaryValue(DeviceInputType.None, testValue2);

            Assert.IsTrue(Interactions.GetDictionaryValueChanged(DeviceInputType.None));

            var setValue2 = Interactions[DeviceInputType.None].GetTransform();

            Assert.AreEqual(setValue2, testValue2);
            Assert.AreEqual(setValue2.Item1, testValue2.Item1);
            Assert.AreEqual(setValue2.Item2, testValue2.Item2);
            Assert.AreEqual(setValue2.Item2, testValue2.Item2);
            Assert.IsFalse(Interactions.GetDictionaryValueChanged(DeviceInputType.None));
        }

        #endregion Interaction Dictionary
    }
}