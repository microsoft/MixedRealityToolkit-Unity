// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Extensions;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces;
using NUnit.Framework;
using System;
using System.Collections.Generic;
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
            var inputDef = new MixedRealityInteractionMapping<object>(1, AxisType.Raw, DeviceInputType.None, InputAction.None);
            var testValue1 = (object)1f;
            var testValue2 = (object)false;

            var initialValue = inputDef.GetValue();

            Assert.IsNull(initialValue);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue(testValue1);

            Assert.IsTrue(inputDef.Changed);

            var setValue1 = inputDef.GetValue();

            Assert.IsNotNull(setValue1);
            Assert.AreEqual(setValue1, testValue1);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue(testValue2);

            Assert.IsTrue(inputDef.Changed);

            var setValue2 = inputDef.GetValue();

            Assert.IsNotNull(setValue2);
            Assert.AreEqual(setValue2, testValue2);
            Assert.IsFalse(inputDef.Changed);
        }

        [Test]
        public void Test02_TestObjectNoChange()
        {
            var inputDef = new MixedRealityInteractionMapping<object>(1, AxisType.Raw, DeviceInputType.None, InputAction.None);
            var testValue = new object();

            var initialValue = inputDef.GetValue();

            Assert.IsNull(initialValue);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue(testValue);

            Assert.IsTrue(inputDef.Changed);

            // Make sure the second time we query it's false
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue(testValue);

            // Make sure if we set the same value it's false
            Assert.IsFalse(inputDef.Changed);

            //Check setting the value twice with the same value produces no change
            var newValue = inputDef.GetValue();

            // Make sure if we set the same value it's false
            Assert.IsFalse(inputDef.Changed, newValue.ToString());

            // Make sure setting again after query, we query again it's false
            inputDef.SetValue(testValue);

            // Make sure if we set the same value it's false
            Assert.IsFalse(inputDef.Changed);

        }

        #endregion objects

        #region bools

        [Test]
        public void Test03_TestBoolChanged()
        {
            var inputDef = new MixedRealityInteractionMapping<bool>(1, AxisType.Digital, DeviceInputType.None, InputAction.None);
            var testValue1 = true;
            var testValue2 = false;

            var initialValue = inputDef.GetValue();

            Assert.IsFalse(initialValue);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue(testValue1);

            Assert.IsTrue(inputDef.Changed);

            var setValue1 = inputDef.GetValue();

            Assert.IsTrue(setValue1);
            Assert.True(setValue1 == testValue1);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue(testValue2);

            Assert.IsTrue(inputDef.Changed);

            var setValue2 = inputDef.GetValue();

            Assert.IsFalse(setValue2);
            Assert.True(setValue2 == testValue2);
            Assert.IsFalse(inputDef.Changed);
        }

        [Test]
        public void Test04_TestBoolNoChange()
        {
            var inputDef = new MixedRealityInteractionMapping<bool>(1, AxisType.Digital, DeviceInputType.None, InputAction.None);
            var testValue = true;

            var initialValue = inputDef.GetValue();

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
            var inputDef = new MixedRealityInteractionMapping<float>(1, AxisType.SingleAxis, DeviceInputType.None, InputAction.None);
            var testValue1 = 1f;
            var testValue2 = 9001f;

            var initialValue = inputDef.GetValue();

            Assert.AreEqual(initialValue, 0d, double.Epsilon);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue(testValue1);

            Assert.IsTrue(inputDef.Changed);

            var setValue1 = inputDef.GetValue();

            Assert.AreEqual(setValue1, testValue1, double.Epsilon);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue(testValue2);

            Assert.IsTrue(inputDef.Changed);

            var setValue2 = inputDef.GetValue();

            Assert.AreEqual(setValue2, testValue2, double.Epsilon);
            Assert.IsFalse(inputDef.Changed);
        }

        [Test]
        public void Test06_TestFloatNoChange()
        {
            var inputDef = new MixedRealityInteractionMapping<float>(1, AxisType.SingleAxis, DeviceInputType.None, InputAction.None);
            var testValue = 1f;

            var initialValue = inputDef.GetValue();

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
            var inputDef = new MixedRealityInteractionMapping<Vector2>(1, AxisType.DualAxis, DeviceInputType.None, InputAction.None);
            var testValue1 = Vector2.one;
            var testValue2 = Vector2.zero;

            var initialValue = inputDef.GetValue();

            Assert.True(initialValue == Vector2.zero);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue(testValue1);

            Assert.IsTrue(inputDef.Changed);

            var setValue1 = inputDef.GetValue();

            Assert.True(setValue1 == testValue1);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue(testValue2);

            Assert.IsTrue(inputDef.Changed);

            var setValue2 = inputDef.GetValue();

            Assert.True(setValue2 == testValue2);
            Assert.IsFalse(inputDef.Changed);
        }

        [Test]
        public void Test08_TestVector2NoChange()
        {
            var inputDef = new MixedRealityInteractionMapping<Vector2>(1, AxisType.DualAxis, DeviceInputType.None, InputAction.None);
            var testValue = Vector2.one;

            var initialValue = inputDef.GetValue();

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
            var inputDef = new MixedRealityInteractionMapping<Vector3>(1, AxisType.ThreeDofPosition, DeviceInputType.None, InputAction.None);
            var testValue1 = Vector3.one;
            var testValue2 = Vector3.zero;

            var initialValue = inputDef.GetValue();

            Assert.True(initialValue == Vector3.zero);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue(testValue1);

            Assert.IsTrue(inputDef.Changed);

            var setValue1 = inputDef.GetValue();

            Assert.True(setValue1 == testValue1);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue(testValue2);

            Assert.IsTrue(inputDef.Changed);

            var setValue2 = inputDef.GetValue();

            Assert.True(setValue2 == testValue2);
            Assert.IsFalse(inputDef.Changed);
        }

        [Test]
        public void Test10_TestVector3NoChange()
        {
            var inputDef = new MixedRealityInteractionMapping<Vector3>(1, AxisType.ThreeDofPosition, DeviceInputType.None, InputAction.None);
            var testValue = Vector3.one;

            var initialValue = inputDef.GetValue();

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
            var inputDef = new MixedRealityInteractionMapping<Quaternion>(1, AxisType.ThreeDofRotation, DeviceInputType.None, InputAction.None);
            var testValue1 = Quaternion.Euler(45f, 45f, 45f);
            var testValue2 = Quaternion.identity;

            var initialValue = inputDef.GetValue();

            Assert.True(initialValue.eulerAngles == Quaternion.identity.eulerAngles);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue(testValue1);

            Assert.IsTrue(inputDef.Changed);

            var setValue1 = inputDef.GetValue();

            Assert.True(setValue1 == testValue1);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue(testValue2);

            Assert.IsTrue(inputDef.Changed);

            var setValue2 = inputDef.GetValue();

            Assert.True(setValue2 == testValue2);
            Assert.IsFalse(inputDef.Changed);
        }

        [Test]
        public void Test12_TestQuaternionNoChange()
        {
            var inputDef = new MixedRealityInteractionMapping<Quaternion>(1, AxisType.ThreeDofRotation, DeviceInputType.None, InputAction.None);
            var testValue = Quaternion.Euler(45f, 45f, 45f);

            var initialValue = inputDef.GetValue();

            Assert.True(initialValue.eulerAngles == Quaternion.identity.eulerAngles);
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

        #region SixDof

        [Test]
        public void Test13_TestSixDofChanged()
        {
            var inputDef = new MixedRealityInteractionMapping<SixDof>(1, AxisType.SixDof, DeviceInputType.None, InputAction.None);
            var testValue1 = new SixDof(Vector3.zero, Quaternion.identity);
            var testValue2 = new SixDof(Vector3.one, new Quaternion(45f, 45f, 45f, 45f));
            var defaultRotation = new Quaternion();

            var initialValue = inputDef.GetValue();

            Assert.IsTrue(initialValue.Position == Vector3.zero);
            Assert.IsTrue(initialValue.Rotation.w.Equals(defaultRotation.w) &&
                          initialValue.Rotation.x.Equals(defaultRotation.x) &&
                          initialValue.Rotation.y.Equals(defaultRotation.y) &&
                          initialValue.Rotation.z.Equals(defaultRotation.z));
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue(testValue1);

            Assert.IsTrue(inputDef.Changed);

            var setValue1 = inputDef.GetValue();

            Assert.IsTrue(setValue1 == testValue1);
            Assert.IsTrue(setValue1.Position == testValue1.Position);
            Assert.IsTrue(setValue1.Rotation == testValue1.Rotation);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue(testValue2);

            Assert.IsTrue(inputDef.Changed);

            var setValue2 = inputDef.GetValue();

            Assert.IsTrue(setValue2 == testValue2);
            Assert.IsTrue(setValue2.Position == testValue2.Position);
            Assert.IsTrue(setValue2.Rotation == testValue2.Rotation);
            Assert.IsFalse(inputDef.Changed);
        }

        [Test]
        public void Test14_TestSixDofNoChange()
        {
            var inputDef = new MixedRealityInteractionMapping<SixDof>(1, AxisType.SixDof, DeviceInputType.None, InputAction.None);
            var testValue = new SixDof(Vector3.zero, Quaternion.identity);
            var defaultRotation = new Quaternion();

            var initialValue = inputDef.GetValue();

            Assert.IsTrue(initialValue.Position == Vector3.zero);
            Assert.IsTrue(initialValue.Rotation.w.Equals(defaultRotation.w) &&
                          initialValue.Rotation.x.Equals(defaultRotation.x) &&
                          initialValue.Rotation.y.Equals(defaultRotation.y) &&
                          initialValue.Rotation.z.Equals(defaultRotation.z));
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue(testValue);

            Assert.IsTrue(inputDef.Changed);

            // Make sure the second time we query it's false
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue(testValue);

            // Make sure if we set the same value it's false
            Assert.IsFalse(inputDef.Changed);
        }

        #endregion SixDof

        #region Interaction Dictionary Tests

        [Test]
        public void Test15_InteractionDictionaryObject()
        {
            var interactions = new System.Collections.Generic.Dictionary<DeviceInputType, IMixedRealityInteractionMapping>();
            interactions.Add(DeviceInputType.None, new MixedRealityInteractionMapping<object>(1, AxisType.Raw, DeviceInputType.None, InputAction.None));
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
            var interactions = new System.Collections.Generic.Dictionary<DeviceInputType, IMixedRealityInteractionMapping>();
            interactions.Add(DeviceInputType.None, new MixedRealityInteractionMapping<bool>(1, AxisType.Digital, DeviceInputType.None, InputAction.None));
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
            var interactions = new System.Collections.Generic.Dictionary<DeviceInputType, IMixedRealityInteractionMapping>();
            interactions.Add(DeviceInputType.None, new MixedRealityInteractionMapping<float>(1, AxisType.SingleAxis, DeviceInputType.None, InputAction.None));
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
            var interactions = new System.Collections.Generic.Dictionary<DeviceInputType, IMixedRealityInteractionMapping>();
            interactions.Add(DeviceInputType.None, new MixedRealityInteractionMapping<Vector2>(1, AxisType.DualAxis, DeviceInputType.None, InputAction.None));
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
            var interactions = new System.Collections.Generic.Dictionary<DeviceInputType, IMixedRealityInteractionMapping>();
            interactions.Add(DeviceInputType.None, new MixedRealityInteractionMapping<Vector3>(1, AxisType.ThreeDofPosition, DeviceInputType.None, InputAction.None));
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
            var interactions = new System.Collections.Generic.Dictionary<DeviceInputType, IMixedRealityInteractionMapping>();
            interactions.Add(DeviceInputType.None, new MixedRealityInteractionMapping<Quaternion>(1, AxisType.ThreeDofRotation, DeviceInputType.None, InputAction.None));
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
            var interactions = new System.Collections.Generic.Dictionary<DeviceInputType, IMixedRealityInteractionMapping>();
            interactions.Add(DeviceInputType.None, new MixedRealityInteractionMapping<SixDof>(1, AxisType.SixDof, DeviceInputType.None, InputAction.None));
            var testValue1 = new SixDof(Vector3.zero, Quaternion.identity);
            var testValue2 = new SixDof(Vector3.one, new Quaternion(45f, 45f, 45f, 45f));
            var defaultRotation = new Quaternion();

            var initialValue = interactions[DeviceInputType.None] as MixedRealityInteractionMapping<SixDof>;

            Assert.IsNotNull(initialValue);
            SixDof initialSixDofValue = initialValue.GetValue();

            Assert.IsTrue(initialSixDofValue.Position == Vector3.zero);
            Assert.IsTrue(initialSixDofValue.Rotation.w.Equals(defaultRotation.w) &&
                          initialSixDofValue.Rotation.x.Equals(defaultRotation.x) &&
                          initialSixDofValue.Rotation.y.Equals(defaultRotation.y) &&
                          initialSixDofValue.Rotation.z.Equals(defaultRotation.z));
            Assert.IsFalse(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            interactions.SetDictionaryValue(DeviceInputType.None, testValue1);

            Assert.IsTrue(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            var setValue1 = interactions[DeviceInputType.None] as MixedRealityInteractionMapping<SixDof>;

            Assert.IsNotNull(setValue1);
            Assert.IsTrue(setValue1.GetValue() == testValue1);
            Assert.IsTrue(setValue1.GetValue().Position == testValue1.Position);
            Assert.IsTrue(setValue1.GetValue().Rotation == testValue1.Rotation);
            Assert.IsFalse(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            interactions.SetDictionaryValue(DeviceInputType.None, testValue2);

            Assert.IsTrue(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            var setValue2 = interactions[DeviceInputType.None] as MixedRealityInteractionMapping<SixDof>;

            Assert.IsNotNull(setValue2);
            Assert.IsTrue(setValue2.GetValue() == testValue2);
            Assert.IsTrue(setValue2.GetValue().Position == testValue2.Position);
            Assert.IsTrue(setValue2.GetValue().Rotation == testValue2.Rotation);
            Assert.IsFalse(interactions.GetDictionaryValueChanged(DeviceInputType.None));
        }
        #endregion Interaction Dictionary

        #region Interaction Array Tests

        [Test]
        public void Test15_InteractionArrayObject()
        {
            var interactions = new IMixedRealityInteractionMapping[1];
            interactions[0] = new MixedRealityInteractionMapping<object>(1, AxisType.Raw, DeviceInputType.None, InputAction.None);
            var testValue1 = (object)1f;
            var testValue2 = (object)false;

            var initialValue = interactions[0] as MixedRealityInteractionMapping<object>;

            Assert.IsNotNull(initialValue);
            Assert.IsNull(initialValue.GetValue());
            Assert.IsFalse(initialValue.Changed);

            initialValue.SetValue(testValue1);

            Assert.IsTrue(initialValue.Changed);

            var setValue1 = interactions[0] as MixedRealityInteractionMapping<object>;

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
            interactions[0] = new MixedRealityInteractionMapping<bool>(1, AxisType.Digital, DeviceInputType.None, InputAction.None);
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
            interactions[0] = new MixedRealityInteractionMapping<float>(1, AxisType.SingleAxis, DeviceInputType.None, InputAction.None);
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
            interactions[0] = new MixedRealityInteractionMapping<Vector2>(1, AxisType.DualAxis, DeviceInputType.None, InputAction.None);
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
            interactions[0] = new MixedRealityInteractionMapping<Vector3>(1, AxisType.ThreeDofPosition, DeviceInputType.None, InputAction.None);
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
            interactions[0] = new MixedRealityInteractionMapping<Quaternion>(1, AxisType.ThreeDofRotation, DeviceInputType.None, InputAction.None);
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
            interactions[0] = new MixedRealityInteractionMapping<SixDof>(1, AxisType.SixDof, DeviceInputType.None, InputAction.None);
            var testValue1 = new SixDof(Vector3.zero, Quaternion.identity);
            var testValue2 = new SixDof(Vector3.one, new Quaternion(45f, 45f, 45f, 45f));
            var defaultRotation = new Quaternion();

            var initialValue = interactions[0] as MixedRealityInteractionMapping<SixDof>;

            Assert.IsNotNull(initialValue);
            SixDof initialSixDofValue = initialValue.GetValue();

            Assert.IsTrue(initialSixDofValue.Position == Vector3.zero);
            Assert.IsTrue(initialSixDofValue.Rotation.w.Equals(defaultRotation.w) &&
                          initialSixDofValue.Rotation.x.Equals(defaultRotation.x) &&
                          initialSixDofValue.Rotation.y.Equals(defaultRotation.y) &&
                          initialSixDofValue.Rotation.z.Equals(defaultRotation.z));
            Assert.IsFalse(initialValue.Changed);

            initialValue.SetValue(testValue1);

            Assert.IsTrue(initialValue.Changed);

            var setValue1 = interactions[0] as MixedRealityInteractionMapping<SixDof>;

            Assert.IsNotNull(setValue1);
            Assert.IsTrue(setValue1.GetValue() == testValue1);
            Assert.IsTrue(setValue1.GetValue().Position == testValue1.Position);
            Assert.IsTrue(setValue1.GetValue().Rotation == testValue1.Rotation);
            Assert.IsFalse(setValue1.Changed);

            setValue1.SetValue(testValue2);

            Assert.IsTrue(setValue1.Changed);

            var setValue2 = interactions[0] as MixedRealityInteractionMapping<SixDof>;

            Assert.IsNotNull(setValue2);
            Assert.IsTrue(setValue2.GetValue() == testValue2);
            Assert.IsTrue(setValue2.GetValue().Position == testValue2.Position);
            Assert.IsTrue(setValue2.GetValue().Rotation == testValue2.Rotation);
            Assert.IsFalse(setValue1.Changed);
        }
        #endregion Interaction Array Tests

        #region Performance Tests
        //Tests show the array lookup is faster.  Keeping here for reference until merge

        [Test]
        public void Test22_InteractionDictionaryPerformance()
        {
            var InteractionsDictionary = new Dictionary<DeviceInputType, IMixedRealityInteractionMapping>();

            //Setup
            InteractionsDictionary.Add(DeviceInputType.SpatialPointer, new MixedRealityInteractionMapping<SixDof>(1, AxisType.SixDof, DeviceInputType.SpatialPointer, new InputAction(1, "Select")));
            InteractionsDictionary.Add(DeviceInputType.Trigger, new MixedRealityInteractionMapping<float>(2, AxisType.SingleAxis, DeviceInputType.Trigger, new InputAction(1, "Select")));
            InteractionsDictionary.Add(DeviceInputType.SpatialGrip, new MixedRealityInteractionMapping<SixDof>(3, AxisType.SixDof, DeviceInputType.SpatialGrip, new InputAction(2, "Grip")));
            InteractionsDictionary.Add(DeviceInputType.GripPress, new MixedRealityInteractionMapping<bool>(4, AxisType.Digital, DeviceInputType.GripPress, new InputAction(3, "Grab")));
            InteractionsDictionary.Add(DeviceInputType.Menu, new MixedRealityInteractionMapping<bool>(5, AxisType.Digital, DeviceInputType.Menu, new InputAction(4, "Menu")));
            InteractionsDictionary.Add(DeviceInputType.ThumbStick, new MixedRealityInteractionMapping<Vector2>(6, AxisType.DualAxis, DeviceInputType.ThumbStick, new InputAction(5, "Walk")));
            InteractionsDictionary.Add(DeviceInputType.ThumbStickPress, new MixedRealityInteractionMapping<bool>(7, AxisType.Digital, DeviceInputType.ThumbStickPress, new InputAction(6, "Interact")));
            InteractionsDictionary.Add(DeviceInputType.Touchpad, new MixedRealityInteractionMapping<Vector2>(8, AxisType.DualAxis, DeviceInputType.Touchpad, new InputAction(7, "Inventory")));
            InteractionsDictionary.Add(DeviceInputType.TouchpadTouch, new MixedRealityInteractionMapping<bool>(9, AxisType.Digital, DeviceInputType.TouchpadTouch, new InputAction(8, "Pickup")));
            InteractionsDictionary.Add(DeviceInputType.TouchpadPress, new MixedRealityInteractionMapping<bool>(10, AxisType.Digital, DeviceInputType.TouchpadPress, new InputAction(8, "Pickup")));

            //Test
            for (int i = 0; i < 500; i++)
            {
                UpdateFromInteractionSource(InteractionsDictionary);
            }
        }

        #region InteractionDictionaryPerformance Update data functions

        /// <summary>
        /// Update the controller data from the provided platform state
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        public void UpdateFromInteractionSource(Dictionary<DeviceInputType, IMixedRealityInteractionMapping> Interactions)
        {
            //Debug.Assert(interactionSourceState.source.id == SourceId, "An UpdateSourceState call happened with mismatched source ID.");
            // TODO - Do we need Kind?
            //Debug.Assert(interactionSourceState.source.kind == sourceData.Source.kind, "An UpdateSourceState call happened with mismatched source kind.");

            // Update Controller
            // TODO - Controller currently not accepted by InputSystem, only InteractionState captured
            // TODO - May need to be more granular with checks if we are allowing user to configure :S  
            // TODO - Need to think of a better way to validate options, multiple Contains aren't good, maybe an extension?
            UpdateControllerDataDictionary();

            // Update Pointer
            if (Interactions.ContainsKey(DeviceInputType.SpatialPointer)) UpdatePointerData(Interactions);

            // Update Grip
            if (Interactions.ContainsKey(DeviceInputType.SpatialGrip)) UpdateGripData(Interactions);

            // Update Touchpad
            if (Interactions.ContainsKey(DeviceInputType.Touchpad) || Interactions.ContainsKey(DeviceInputType.TouchpadTouch)) UpdateTouchPadData(Interactions);

            // Update Thumbstick
            if (Interactions.ContainsKey(DeviceInputType.Thumb)) UpdateThumbStickData(Interactions);

            // Update Trigger
            if (Interactions.ContainsKey(DeviceInputType.Trigger)) UpdateTriggerData(Interactions);
        }


        /// <summary>
        /// Update the "Controller" input from the device
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        private void UpdateControllerDataDictionary()
        {
            // Get Controller start position and tracked state
            var controllerTracked = true;
            var ControllerState = controllerTracked ? Internal.Definitions.Devices.ControllerState.Tracked : Internal.Definitions.Devices.ControllerState.NotTracked;
            Assert.AreNotEqual(ControllerState.None, ControllerState);
        }

        /// <summary>
        /// Update the "Spatial Pointer" input from the device
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        private void UpdatePointerData(Dictionary<DeviceInputType, IMixedRealityInteractionMapping> Interactions)
        {
            Vector3 currentControllerPosition = Vector3.left;
            Quaternion currentControllerRotation = Quaternion.identity;

            Interactions.SetDictionaryValue(DeviceInputType.SpatialPointer, new SixDof(currentControllerPosition, currentControllerRotation));
        }

        /// <summary>
        /// Update the "Spatial Grip" input from the device
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        private void UpdateGripData(Dictionary<DeviceInputType, IMixedRealityInteractionMapping> Interactions)
        {
            Vector3 currentGripPosition = Vector3.left;
            Quaternion currentGripRotation = Quaternion.identity;

            Interactions.SetDictionaryValue(DeviceInputType.SpatialGrip, new SixDof(currentGripPosition, currentGripRotation));
        }

        /// <summary>
        /// Update the Touchpad input from the device
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        private void UpdateTouchPadData(Dictionary<DeviceInputType, IMixedRealityInteractionMapping> Interactions)
        {
            if (Interactions.ContainsKey(DeviceInputType.TouchpadTouch)) Interactions.SetDictionaryValue(DeviceInputType.TouchpadTouch, true);  //Interactions[DeviceInputType.TouchpadTouch].SetValue(interactionSourceState.touchpadTouched);
            if (Interactions.ContainsKey(DeviceInputType.TouchpadPress)) Interactions.SetDictionaryValue(DeviceInputType.TouchpadPress, true);  //Interactions[DeviceInputType.TouchpadPress].SetValue(interactionSourceState.touchpadPressed);
            if (Interactions.ContainsKey(DeviceInputType.Touchpad)) Interactions.SetDictionaryValue(DeviceInputType.Touchpad, Vector2.left);  //Interactions[DeviceInputType.Touchpad].SetValue(interactionSourceState.touchpadPosition);
        }

        /// <summary>
        /// Update the Thumbstick input from the device
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        private void UpdateThumbStickData(Dictionary<DeviceInputType, IMixedRealityInteractionMapping> Interactions)
        {
            if (Interactions.ContainsKey(DeviceInputType.ThumbStickPress)) Interactions.SetDictionaryValue(DeviceInputType.ThumbStickPress, true);  //Interactions[DeviceInputType.ThumbStickPress].SetValue(interactionSourceState.thumbstickPressed);

            Interactions.SetDictionaryValue(DeviceInputType.ThumbStick, Vector2.left);
        }

        /// <summary>
        /// Update the Trigger input from the device
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        private void UpdateTriggerData(Dictionary<DeviceInputType, IMixedRealityInteractionMapping> Interactions)
        {
            if (Interactions.ContainsKey(DeviceInputType.TriggerPress)) Interactions.SetDictionaryValue(DeviceInputType.TriggerPress, true);  //Interactions[DeviceInputType.TriggerPress].SetValue(interactionSourceState.selectPressed);

            Interactions.SetDictionaryValue(DeviceInputType.Trigger, 1f);
        }

        #endregion InteractionDictionaryPerformance Update data functions

        [Test]
        public void Test23_InteractionArrayPerformance()
        {
            IMixedRealityInteractionMapping[] InteractionsArray = new IMixedRealityInteractionMapping[0];

            //Setup
            var interactions = new List<IMixedRealityInteractionMapping>();

            interactions.Add(new MixedRealityInteractionMapping<SixDof>(1, AxisType.SixDof, DeviceInputType.SpatialPointer, new InputAction(1, "Select")));
            interactions.Add(new MixedRealityInteractionMapping<float>(2, AxisType.SingleAxis, DeviceInputType.Trigger, new InputAction(1, "Select")));
            interactions.Add(new MixedRealityInteractionMapping<SixDof>(3, AxisType.SixDof, DeviceInputType.SpatialGrip, new InputAction(2, "Grip")));
            interactions.Add(new MixedRealityInteractionMapping<bool>(4, AxisType.Digital, DeviceInputType.GripPress, new InputAction(3, "Grab")));
            interactions.Add(new MixedRealityInteractionMapping<bool>(5, AxisType.Digital, DeviceInputType.Menu, new InputAction(4, "Menu")));
            interactions.Add(new MixedRealityInteractionMapping<Vector2>(6, AxisType.DualAxis, DeviceInputType.ThumbStick, new InputAction(5, "Walk")));
            interactions.Add(new MixedRealityInteractionMapping<bool>(7, AxisType.Digital, DeviceInputType.ThumbStickPress, new InputAction(6, "Interact")));
            interactions.Add(new MixedRealityInteractionMapping<Vector2>(8, AxisType.DualAxis, DeviceInputType.Touchpad, new InputAction(7, "Inventory")));
            interactions.Add(new MixedRealityInteractionMapping<bool>(9, AxisType.Digital, DeviceInputType.TouchpadTouch, new InputAction(8, "Pickup")));
            interactions.Add(new MixedRealityInteractionMapping<bool>(10, AxisType.Digital, DeviceInputType.TouchpadPress, new InputAction(8, "Pickup")));

            InteractionsArray = interactions.ToArray();

            //Test
            for (int i = 0; i < 500; i++)
            {
                UpdateController(InteractionsArray);
            }
        }

        #region InteractionArrayPerformance Update data functions

        /// <summary>
        /// Update the controller data from the provided platform state
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        public void UpdateController(IMixedRealityInteractionMapping[] Interactions)
        {
            UpdateControllerDataArray();

            for (int i = 0; i < Interactions.Length; i++)
            {
                switch (Interactions[i].InputType)
                {
                    case DeviceInputType.SpatialPointer:
                    case DeviceInputType.PointerPosition:
                    case DeviceInputType.PointerRotation:
                        UpdatePointerData(Interactions[i]);
                        break;
                    case DeviceInputType.Select:
                    case DeviceInputType.Trigger:
                    case DeviceInputType.TriggerPress:
                    case DeviceInputType.PointerClick:
                        UpdateTriggerData(Interactions[i]);
                        break;
                    case DeviceInputType.SpatialGrip:
                    case DeviceInputType.GripPosition:
                    case DeviceInputType.GripRotation:
                    case DeviceInputType.GripPress:
                        UpdateGripData(Interactions[i]);
                        break;
                    case DeviceInputType.ThumbStick:
                    case DeviceInputType.ThumbStickPress:
                        UpdateThumbStickData(Interactions[i]);
                        break;
                    case DeviceInputType.Touchpad:
                    case DeviceInputType.TouchpadTouch:
                    case DeviceInputType.TouchpadPress:
                        UpdateTouchPadData(Interactions[i]);
                        break;
                    case DeviceInputType.Menu:
                    case DeviceInputType.ButtonPress:
                        {
                            var interaction = Interactions[i] as MixedRealityInteractionMapping<bool>;
                            Debug.Assert(interaction != null);
                            interaction.SetValue(true);
                            break;
                        }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Update the "Controller" input from the device
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        private void UpdateControllerDataArray()
        {
            // Get Controller start position and tracked state
            bool isControllerTracked = true;
            var ControllerState = isControllerTracked ? Internal.Definitions.Devices.ControllerState.Tracked : Internal.Definitions.Devices.ControllerState.NotTracked;
            Assert.AreNotEqual(ControllerState.None, ControllerState);
        }

        /// <summary>
        /// Update the "Spatial Pointer" input from the device
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        /// <param name="MixedRealityInteractionMapping"></param>
        private void UpdatePointerData(IMixedRealityInteractionMapping interactionMapping)
        {
            Vector3 currentPointerPosition = Vector3.left;
            Quaternion currentPointerRotation = Quaternion.identity;

            var interaction = interactionMapping as MixedRealityInteractionMapping<SixDof>;
            currentPointerData.Position = currentPointerPosition;
            currentPointerData.Rotation = currentPointerRotation;
            Debug.Assert(interaction != null);
            interaction.SetValue(currentPointerData);
        }

        private SixDof currentPointerData = new SixDof(Vector3.zero, Quaternion.identity);

        /// <summary>
        /// Update the "Spatial Grip" input from the device
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        /// <param name="interactionMapping"></param>
        private void UpdateGripData(IMixedRealityInteractionMapping interactionMapping)
        {
            switch (interactionMapping.InputType)
            {
                case DeviceInputType.SpatialGrip:
                    {
                        Vector3 currentGripPosition = Vector3.left;
                        Quaternion currentGripRotation = Quaternion.identity;

                        var interaction = interactionMapping as MixedRealityInteractionMapping<SixDof>;
                        var value = new SixDof(currentGripPosition, currentGripRotation);
                        Debug.Assert(interaction != null);
                        interaction.SetValue(value);
                        break;
                    }
                case DeviceInputType.GripPress:
                    {
                        var interaction = interactionMapping as MixedRealityInteractionMapping<bool>;
                        var value = true;
                        Debug.Assert(interaction != null);
                        interaction.SetValue(value);
                        break;
                    }
            }
        }

        /// <summary>
        /// Update the Touchpad input from the device
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        /// <param name="interactionMapping"></param>
        private void UpdateTouchPadData(IMixedRealityInteractionMapping interactionMapping)
        {
            switch (interactionMapping.InputType)
            {
                case DeviceInputType.TouchpadTouch:
                    {
                        var interaction = interactionMapping as MixedRealityInteractionMapping<bool>;
                        Debug.Assert(interaction != null);
                        interaction.SetValue(true);
                        break;
                    }
                case DeviceInputType.TouchpadPress:
                    {
                        var interaction = interactionMapping as MixedRealityInteractionMapping<bool>;
                        Debug.Assert(interaction != null);
                        interaction.SetValue(true);
                        break;
                    }
                case DeviceInputType.Touchpad:
                    {
                        var interaction = interactionMapping as MixedRealityInteractionMapping<Vector2>;
                        Debug.Assert(interaction != null);
                        interaction.SetValue(Vector2.left);
                        break;
                    }
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        /// <summary>
        /// Update the Thumbstick input from the device
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        /// <param name="interactionMapping"></param>
        private void UpdateThumbStickData(IMixedRealityInteractionMapping interactionMapping)
        {
            switch (interactionMapping.InputType)
            {
                case DeviceInputType.ThumbStickPress:
                    {
                        var interaction = interactionMapping as MixedRealityInteractionMapping<bool>;
                        Debug.Assert(interaction != null);
                        interaction.SetValue(true);
                        break;
                    }
                case DeviceInputType.ThumbStick:
                    {
                        var interaction = interactionMapping as MixedRealityInteractionMapping<Vector2>;
                        Debug.Assert(interaction != null);
                        interaction.SetValue(Vector2.down);
                        break;
                    }
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        /// <summary>
        /// Update the Trigger input from the device
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        /// <param name="interactionMapping"></param>
        private void UpdateTriggerData(IMixedRealityInteractionMapping interactionMapping)
        {
            switch (interactionMapping.InputType)
            {
                case DeviceInputType.Select:
                case DeviceInputType.TriggerPress:
                    {
                        var interaction = interactionMapping as MixedRealityInteractionMapping<bool>;
                        Debug.Assert(interaction != null);
                        interaction.SetValue(true);
                        break;
                    }
                case DeviceInputType.Trigger:
                    {
                        var interaction = interactionMapping as MixedRealityInteractionMapping<float>;
                        Debug.Assert(interaction != null);
                        interaction.SetValue(1f);
                        break;
                    }
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        #endregion InteractionArrayPerformance Update data functions

        #endregion Performance Tests

    }
}