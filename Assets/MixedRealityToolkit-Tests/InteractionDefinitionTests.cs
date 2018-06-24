// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Extensions;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.Devices;
using NUnit.Framework;
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
            var inputDef = new MixedRealityInteractionMapping<object>(1, AxisType.Raw, DeviceInputType.None, new InputAction(1, "None"));
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
            var inputDef = new MixedRealityInteractionMapping<object>(1, AxisType.Raw, DeviceInputType.None, new InputAction(1, "None"));
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
            var inputDef = new MixedRealityInteractionMapping<bool>(1, AxisType.Digital, DeviceInputType.None, new InputAction(1, "None"));
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
            var inputDef = new MixedRealityInteractionMapping<bool>(1, AxisType.Digital, DeviceInputType.None, new InputAction(1, "None"));
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
            var inputDef = new MixedRealityInteractionMapping<float>(1, AxisType.SingleAxis, DeviceInputType.None, new InputAction(1, "None"));
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
            var inputDef = new MixedRealityInteractionMapping<float>(1, AxisType.SingleAxis, DeviceInputType.None, new InputAction(1, "None"));
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
            var inputDef = new MixedRealityInteractionMapping<Vector2>(1, AxisType.DualAxis, DeviceInputType.None, new InputAction(1, "None"));
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
            var inputDef = new MixedRealityInteractionMapping<Vector2>(1, AxisType.DualAxis, DeviceInputType.None, new InputAction(1, "None"));
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
            var inputDef = new MixedRealityInteractionMapping<Vector3>(1, AxisType.ThreeDofPosition, DeviceInputType.None, new InputAction(1, "None"));
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
            var inputDef = new MixedRealityInteractionMapping<Vector3>(1, AxisType.ThreeDofPosition, DeviceInputType.None, new InputAction(1, "None"));
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
            var inputDef = new MixedRealityInteractionMapping<Quaternion>(1, AxisType.ThreeDofRotation, DeviceInputType.None, new InputAction(1, "None"));
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
            var inputDef = new MixedRealityInteractionMapping<Quaternion>(1, AxisType.ThreeDofRotation, DeviceInputType.None, new InputAction(1, "None"));
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
            var inputDef = new MixedRealityInteractionMapping<Tuple<Vector3, Quaternion>>(1, AxisType.SixDof, DeviceInputType.None, new InputAction(1, "None"));
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
            var inputDef = new MixedRealityInteractionMapping<Tuple<Vector3, Quaternion>>(1, AxisType.SixDof, DeviceInputType.None, new InputAction(1, "None"));
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
            var interactions = new Dictionary<DeviceInputType, IMixedRealityInteractionMapping>();
            interactions.Add(DeviceInputType.None, new MixedRealityInteractionMapping<object>(1, AxisType.Raw, DeviceInputType.None, new InputAction(0, "None")));
            var testValue1 = (object)1f;
            var testValue2 = (object)false;

            var initialValue = interactions[DeviceInputType.None] as MixedRealityInteractionMapping<object>;

            Assert.IsNotNull(initialValue);
            Assert.IsNull(initialValue.GetValue());
            Assert.IsFalse(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            interactions.SetDictionaryValue(DeviceInputType.None, testValue1);

            Assert.IsTrue(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            var setValue1 = interactions[DeviceInputType.None] as MixedRealityInteractionMapping<object>;

            Assert.IsNotNull(setValue1);
            Assert.IsNotNull(setValue1.GetValue());
            Assert.AreEqual(setValue1.GetValue(), testValue1);
            Assert.IsFalse(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            interactions.SetDictionaryValue(DeviceInputType.None, testValue2);

            Assert.IsTrue(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            var setValue2 = interactions[DeviceInputType.None] as MixedRealityInteractionMapping<object>;

            Assert.IsNotNull(setValue2);
            Assert.IsNotNull(setValue2.GetValue());
            Assert.AreEqual(setValue2.GetValue(), testValue2);
            Assert.IsFalse(interactions.GetDictionaryValueChanged(DeviceInputType.None));
        }

        [Test]
        public void Test16_InteractionDictionaryBool()
        {
            var interactions = new Dictionary<DeviceInputType, IMixedRealityInteractionMapping>();
            interactions.Add(DeviceInputType.None, new MixedRealityInteractionMapping<bool>(1, AxisType.Digital, DeviceInputType.None, new InputAction(0, "None")));
            var testValue1 = true;
            var testValue2 = false;

            var initialValue = interactions[DeviceInputType.None] as MixedRealityInteractionMapping<bool>;

            Assert.IsNotNull(initialValue);
            Assert.IsFalse(initialValue.GetValue());
            Assert.IsFalse(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            interactions.SetDictionaryValue(DeviceInputType.None, testValue1);

            Assert.IsTrue(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            var setValue1 = interactions[DeviceInputType.None] as MixedRealityInteractionMapping<bool>;

            Assert.IsNotNull(setValue1);
            Assert.IsTrue(setValue1.GetValue());
            Assert.IsTrue(setValue1.GetValue() == testValue1);
            Assert.IsFalse(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            interactions.SetDictionaryValue(DeviceInputType.None, testValue2);

            Assert.IsTrue(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            var setValue2 = interactions[DeviceInputType.None] as MixedRealityInteractionMapping<bool>;

            Assert.IsNotNull(setValue2);
            Assert.IsFalse(setValue2.GetValue());
            Assert.IsTrue(setValue2.GetValue() == testValue2);
            Assert.IsFalse(interactions.GetDictionaryValueChanged(DeviceInputType.None));
        }

        [Test]
        public void Test17_InteractionDictionaryFloat()
        {
            var interactions = new Dictionary<DeviceInputType, IMixedRealityInteractionMapping>();
            interactions.Add(DeviceInputType.None, new MixedRealityInteractionMapping<float>(1, AxisType.SingleAxis, DeviceInputType.None, new InputAction(0, "None")));
            var testValue1 = 1f;
            var testValue2 = 9001f;

            var initialValue = interactions[DeviceInputType.None] as MixedRealityInteractionMapping<float>;

            Assert.IsNotNull(initialValue);
            Assert.AreEqual(initialValue.GetValue(), 0d, double.Epsilon);
            Assert.IsFalse(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            interactions.SetDictionaryValue(DeviceInputType.None, testValue1);

            Assert.IsTrue(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            var setValue1 = interactions[DeviceInputType.None] as MixedRealityInteractionMapping<float>;

            Assert.IsNotNull(setValue1);
            Assert.AreEqual(setValue1.GetValue(), testValue1, double.Epsilon);
            Assert.IsFalse(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            interactions.SetDictionaryValue(DeviceInputType.None, testValue2);

            Assert.IsTrue(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            var setValue2 = interactions[DeviceInputType.None] as MixedRealityInteractionMapping<float>;

            Assert.IsNotNull(setValue2);
            Assert.AreEqual(setValue2.GetValue(), testValue2, double.Epsilon);
            Assert.IsFalse(interactions.GetDictionaryValueChanged(DeviceInputType.None));
        }

        [Test]
        public void Test18_InteractionDictionaryVector2()
        {
            var interactions = new Dictionary<DeviceInputType, IMixedRealityInteractionMapping>();
            interactions.Add(DeviceInputType.None, new MixedRealityInteractionMapping<Vector2>(1, AxisType.DualAxis, DeviceInputType.None, new InputAction(0, "None")));
            var testValue1 = Vector2.one;
            var testValue2 = Vector2.zero;

            var initialValue = interactions[DeviceInputType.None] as MixedRealityInteractionMapping<Vector2>;

            Assert.IsNotNull(initialValue);
            Assert.True(initialValue.GetValue() == Vector2.zero);
            Assert.IsFalse(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            interactions.SetDictionaryValue(DeviceInputType.None, testValue1);

            Assert.IsTrue(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            var setValue1 = interactions[DeviceInputType.None] as MixedRealityInteractionMapping<Vector2>;

            Assert.IsNotNull(setValue1);
            Assert.True(setValue1.GetValue() == testValue1);
            Assert.IsFalse(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            interactions.SetDictionaryValue(DeviceInputType.None, testValue2);

            Assert.IsTrue(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            var setValue2 = interactions[DeviceInputType.None] as MixedRealityInteractionMapping<Vector2>;

            Assert.IsNotNull(setValue2);
            Assert.True(setValue2.GetValue() == testValue2);
            Assert.IsFalse(interactions.GetDictionaryValueChanged(DeviceInputType.None));
        }

        [Test]
        public void Test19_InteractionDictionaryVector3()
        {
            var interactions = new Dictionary<DeviceInputType, IMixedRealityInteractionMapping>();
            interactions.Add(DeviceInputType.None, new MixedRealityInteractionMapping<Vector3>(1, AxisType.ThreeDofPosition, DeviceInputType.None, new InputAction(0, "None")));
            var testValue1 = Vector3.one;
            var testValue2 = Vector3.zero;

            var initialValue = interactions[DeviceInputType.None] as MixedRealityInteractionMapping<Vector3>;

            Assert.IsNotNull(initialValue);
            Assert.True(initialValue.GetValue() == Vector3.zero);
            Assert.IsFalse(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            interactions.SetDictionaryValue(DeviceInputType.None, testValue1);

            Assert.IsTrue(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            var setValue1 = interactions[DeviceInputType.None] as MixedRealityInteractionMapping<Vector3>;

            Assert.IsNotNull(setValue1);
            Assert.True(setValue1.GetValue() == testValue1);
            Assert.IsFalse(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            interactions.SetDictionaryValue(DeviceInputType.None, testValue2);

            Assert.IsTrue(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            var setValue2 = interactions[DeviceInputType.None] as MixedRealityInteractionMapping<Vector3>;

            Assert.IsNotNull(setValue2);
            Assert.True(setValue2.GetValue() == testValue2);
            Assert.IsFalse(interactions.GetDictionaryValueChanged(DeviceInputType.None));
        }

        [Test]
        public void Test20_InteractionDictionaryQuaternion()
        {
            var interactions = new Dictionary<DeviceInputType, IMixedRealityInteractionMapping>();
            interactions.Add(DeviceInputType.None, new MixedRealityInteractionMapping<Quaternion>(1, AxisType.ThreeDofRotation, DeviceInputType.None, new InputAction(0, "None")));
            var testValue1 = Quaternion.Euler(45f, 45f, 45f);
            var testValue2 = Quaternion.identity;

            var initialValue = interactions[DeviceInputType.None] as MixedRealityInteractionMapping<Quaternion>;

            Assert.IsNotNull(initialValue);
            Assert.True(initialValue.GetValue().eulerAngles == Quaternion.identity.eulerAngles);
            Assert.IsFalse(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            interactions.SetDictionaryValue(DeviceInputType.None, testValue1);

            Assert.IsTrue(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            var setValue1 = interactions[DeviceInputType.None] as MixedRealityInteractionMapping<Quaternion>;

            Assert.IsNotNull(setValue1);
            Assert.True(setValue1.GetValue() == testValue1);
            Assert.IsFalse(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            interactions.SetDictionaryValue(DeviceInputType.None, testValue2);

            Assert.IsTrue(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            var setValue2 = interactions[DeviceInputType.None] as MixedRealityInteractionMapping<Quaternion>;

            Assert.IsNotNull(setValue2);
            Assert.True(setValue2.GetValue() == testValue2);
            Assert.IsFalse(interactions.GetDictionaryValueChanged(DeviceInputType.None));
        }

        [Test]
        public void Test21_InteractionDictionarySixDof()
        {
            var interactions = new Dictionary<DeviceInputType, IMixedRealityInteractionMapping>();
            interactions.Add(DeviceInputType.None, new MixedRealityInteractionMapping<Tuple<Vector3, Quaternion>>(1, AxisType.SixDof, DeviceInputType.None, new InputAction(0, "None")));
            var testValue1 = new Tuple<Vector3, Quaternion>(Vector3.zero, Quaternion.identity);
            var testValue2 = new Tuple<Vector3, Quaternion>(Vector3.one, new Quaternion(45f, 45f, 45f, 45f));

            var initialValue = interactions[DeviceInputType.None] as MixedRealityInteractionMapping<Tuple<Vector3, Quaternion>>;

            Assert.IsNotNull(initialValue);
            Assert.IsNull(initialValue.GetValue());
            Assert.IsFalse(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            interactions.SetDictionaryValue(DeviceInputType.None, testValue1);

            Assert.IsTrue(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            var setValue1 = interactions[DeviceInputType.None] as MixedRealityInteractionMapping<Tuple<Vector3, Quaternion>>;

            Assert.IsNotNull(setValue1);
            Assert.AreEqual(setValue1.GetValue(), testValue1);
            Assert.IsTrue(setValue1.GetValue().Item1 == testValue1.Item1);
            Assert.IsTrue(setValue1.GetValue().Item2 == testValue1.Item2);
            Assert.IsFalse(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            interactions.SetDictionaryValue(DeviceInputType.None, testValue2);

            Assert.IsTrue(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            var setValue2 = interactions[DeviceInputType.None] as MixedRealityInteractionMapping<Tuple<Vector3, Quaternion>>;

            Assert.IsNotNull(setValue2);
            Assert.AreEqual(setValue2.GetValue(), testValue2);
            Assert.IsTrue(setValue2.GetValue().Item1 == testValue2.Item1);
            Assert.IsTrue(setValue2.GetValue().Item2 == testValue2.Item2);
            Assert.IsFalse(interactions.GetDictionaryValueChanged(DeviceInputType.None));
        }
        #endregion Interaction Dictionary

        #region Interaction Array Tests

        [Test]
        public void Test15_InteractionArrayObject()
        {
            var interactions = new IMixedRealityInteractionMapping[1];
            interactions[0] = new MixedRealityInteractionMapping<object>(1, AxisType.Raw, DeviceInputType.None, new InputAction(0, "None"));
            var testValue1 = (object)1f;
            var testValue2 = (object)false;

            var initialValue = (MixedRealityInteractionMapping<object>)interactions[0];

            Assert.IsNotNull(initialValue);
            Assert.IsNull(initialValue.GetValue());
            Assert.IsFalse(initialValue.Changed);

            initialValue.SetValue(testValue1);

            Assert.IsTrue(initialValue.Changed);

            var setValue1 = (MixedRealityInteractionMapping<object>)interactions[0];

            Assert.IsNotNull(setValue1);
            Assert.IsNotNull(setValue1.GetValue());
            Assert.AreEqual(setValue1.GetValue(), testValue1);
            Assert.IsFalse(setValue1.Changed);

            setValue1.SetValue(testValue2);

            Assert.IsTrue(setValue1.Changed);

            var setValue2 = interactions[0] as MixedRealityInteractionMapping<object>;

            Assert.IsNotNull(setValue2);
            Assert.IsNotNull(setValue2.GetValue());
            Assert.AreEqual(setValue2.GetValue(), testValue2);
            Assert.IsFalse(setValue2.Changed);
        }

        [Test]
        public void Test16_InteractionArrayBool()
        {
            var interactions = new IMixedRealityInteractionMapping[1];
            interactions[0] = new MixedRealityInteractionMapping<bool>(1, AxisType.Digital, DeviceInputType.None, new InputAction(0, "None"));
            var testValue1 = true;
            var testValue2 = false;

            var initialValue = interactions[0] as MixedRealityInteractionMapping<bool>;

            Assert.IsNotNull(initialValue);
            Assert.IsFalse(initialValue.GetValue());
            Assert.IsFalse(initialValue.Changed);

            initialValue.SetValue(testValue1);

            Assert.IsTrue(initialValue.Changed);

            var setValue1 = interactions[0] as MixedRealityInteractionMapping<bool>;

            Assert.IsNotNull(setValue1);
            Assert.IsTrue(setValue1.GetValue());
            Assert.IsTrue(setValue1.GetValue() == testValue1);
            Assert.IsFalse(setValue1.Changed);

            setValue1.SetValue(testValue2);

            Assert.IsTrue(setValue1.Changed);

            var setValue2 = interactions[0] as MixedRealityInteractionMapping<bool>;

            Assert.IsNotNull(setValue2);
            Assert.IsFalse(setValue2.GetValue());
            Assert.IsTrue(setValue2.GetValue() == testValue2);
            Assert.IsFalse(setValue2.Changed);
        }

        [Test]
        public void Test17_InteractionArrayFloat()
        {
            var interactions = new IMixedRealityInteractionMapping[1];
            interactions[0] = new MixedRealityInteractionMapping<float>(1, AxisType.SingleAxis, DeviceInputType.None, new InputAction(0, "None"));
            var testValue1 = 1f;
            var testValue2 = 9001f;

            var initialValue = interactions[0] as MixedRealityInteractionMapping<float>;

            Assert.IsNotNull(initialValue);
            Assert.AreEqual(initialValue.GetValue(), 0d, double.Epsilon);
            Assert.IsFalse(initialValue.Changed);

            initialValue.SetValue(testValue1);

            Assert.IsTrue(initialValue.Changed);

            var setValue1 = interactions[0] as MixedRealityInteractionMapping<float>;

            Assert.IsNotNull(setValue1);
            Assert.AreEqual(setValue1.GetValue(), testValue1, double.Epsilon);
            Assert.IsFalse(setValue1.Changed);

            setValue1.SetValue(testValue2);

            Assert.IsTrue(setValue1.Changed);

            var setValue2 = interactions[0] as MixedRealityInteractionMapping<float>;

            Assert.IsNotNull(setValue2);
            Assert.AreEqual(setValue2.GetValue(), testValue2, double.Epsilon);
            Assert.IsFalse(setValue2.Changed);
        }

        [Test]
        public void Test18_InteractionArrayVector2()
        {
            var interactions = new IMixedRealityInteractionMapping[1];
            interactions[0] = new MixedRealityInteractionMapping<Vector2>(1, AxisType.DualAxis, DeviceInputType.None, new InputAction(0, "None"));
            var testValue1 = Vector2.one;
            var testValue2 = Vector2.zero;

            var initialValue = interactions[0] as MixedRealityInteractionMapping<Vector2>;

            Assert.IsNotNull(initialValue);
            Assert.True(initialValue.GetValue() == Vector2.zero);
            Assert.IsFalse(initialValue.Changed);

            initialValue.SetValue(testValue1);

            Assert.IsTrue(initialValue.Changed);

            var setValue1 = interactions[0] as MixedRealityInteractionMapping<Vector2>;

            Assert.IsNotNull(setValue1);
            Assert.True(setValue1.GetValue() == testValue1);
            Assert.IsFalse(setValue1.Changed);

            setValue1.SetValue(testValue2);

            Assert.IsTrue(setValue1.Changed);

            var setValue2 = interactions[0] as MixedRealityInteractionMapping<Vector2>;

            Assert.IsNotNull(setValue2);
            Assert.True(setValue2.GetValue() == testValue2);
            Assert.IsFalse(setValue2.Changed);
        }

        [Test]
        public void Test19_InteractionArrayVector3()
        {
            var interactions = new IMixedRealityInteractionMapping[1];
            interactions[0] = new MixedRealityInteractionMapping<Vector3>(1, AxisType.ThreeDofPosition, DeviceInputType.None, new InputAction(0, "None"));
            var testValue1 = Vector3.one;
            var testValue2 = Vector3.zero;

            var initialValue = interactions[0] as MixedRealityInteractionMapping<Vector3>;

            Assert.IsNotNull(initialValue);
            Assert.True(initialValue.GetValue() == Vector3.zero);
            Assert.IsFalse(initialValue.Changed);

            initialValue.SetValue(testValue1);

            Assert.IsTrue(initialValue.Changed);

            var setValue1 = interactions[0] as MixedRealityInteractionMapping<Vector3>;

            Assert.IsNotNull(setValue1);
            Assert.True(setValue1.GetValue() == testValue1);
            Assert.IsFalse(setValue1.Changed);

            setValue1.SetValue(testValue2);

            Assert.IsTrue(setValue1.Changed);

            var setValue2 = interactions[0] as MixedRealityInteractionMapping<Vector3>;

            Assert.IsNotNull(setValue2);
            Assert.True(setValue2.GetValue() == testValue2);
            Assert.IsFalse(setValue2.Changed);
        }

        [Test]
        public void Test20_InteractionArrayQuaternion()
        {
            var interactions = new IMixedRealityInteractionMapping[1];
            interactions[0] = new MixedRealityInteractionMapping<Quaternion>(1, AxisType.ThreeDofRotation, DeviceInputType.None, new InputAction(0, "None"));
            var testValue1 = Quaternion.Euler(45f, 45f, 45f);
            var testValue2 = Quaternion.identity;

            var initialValue = interactions[0] as MixedRealityInteractionMapping<Quaternion>;

            Assert.IsNotNull(initialValue);
            Assert.True(initialValue.GetValue().eulerAngles == Quaternion.identity.eulerAngles);
            Assert.IsFalse(initialValue.Changed);

            initialValue.SetValue(testValue1);

            Assert.IsTrue(initialValue.Changed);

            var setValue1 = interactions[0] as MixedRealityInteractionMapping<Quaternion>;

            Assert.IsNotNull(setValue1);
            Assert.True(setValue1.GetValue() == testValue1);
            Assert.IsFalse(setValue1.Changed);

            setValue1.SetValue(testValue2);

            Assert.IsTrue(setValue1.Changed);

            var setValue2 = interactions[0] as MixedRealityInteractionMapping<Quaternion>;

            Assert.IsNotNull(setValue2);
            Assert.True(setValue2.GetValue() == testValue2);
            Assert.IsFalse(setValue2.Changed);
        }

        [Test]
        public void Test21_InteractionArraySixDof()
        {
            var interactions = new IMixedRealityInteractionMapping[1];
            interactions[0] = new MixedRealityInteractionMapping<Tuple<Vector3, Quaternion>>(1, AxisType.SixDof, DeviceInputType.None, new InputAction(0, "None"));
            var testValue1 = new Tuple<Vector3, Quaternion>(Vector3.zero, Quaternion.identity);
            var testValue2 = new Tuple<Vector3, Quaternion>(Vector3.one, new Quaternion(45f, 45f, 45f, 45f));

            var initialValue = interactions[0] as MixedRealityInteractionMapping<Tuple<Vector3, Quaternion>>;

            Assert.IsNotNull(initialValue);
            Assert.IsNull(initialValue.GetValue());
            Assert.IsFalse(initialValue.Changed);

            initialValue.SetValue(testValue1);

            Assert.IsTrue(initialValue.Changed);

            var setValue1 = interactions[0] as MixedRealityInteractionMapping<Tuple<Vector3, Quaternion>>;

            Assert.IsNotNull(setValue1);
            Assert.AreEqual(setValue1.GetValue(), testValue1);
            Assert.IsTrue(setValue1.GetValue().Item1 == testValue1.Item1);
            Assert.IsTrue(setValue1.GetValue().Item2 == testValue1.Item2);
            Assert.IsFalse(setValue1.Changed);

            setValue1.SetValue(testValue2);

            Assert.IsTrue(setValue1.Changed);

            var setValue2 = interactions[0] as MixedRealityInteractionMapping<Tuple<Vector3, Quaternion>>;

            Assert.IsNotNull(setValue2);
            Assert.AreEqual(setValue2.GetValue(), testValue2);
            Assert.IsTrue(setValue2.GetValue().Item1 == testValue2.Item1);
            Assert.IsTrue(setValue2.GetValue().Item2 == testValue2.Item2);
            Assert.IsFalse(setValue1.Changed);
        }
        #endregion Interaction Array Tests

    }
}