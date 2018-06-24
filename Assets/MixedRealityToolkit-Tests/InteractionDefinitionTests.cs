// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Extensions;
using NUnit.Framework;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class InteractionDefinitionTests
    {
        #region objects

        [Test]
        public void Test01_TestObjectChanged()
        {
            var inputDef = new InteractionMapping(1, AxisType.Raw, DeviceInputType.None, InputAction.None);
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
            var inputDef = new InteractionMapping(1, AxisType.Raw, DeviceInputType.None, InputAction.None);
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
            var inputDef = new InteractionMapping(1, AxisType.Digital, DeviceInputType.None, InputAction.None);
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
            var inputDef = new InteractionMapping(1, AxisType.Digital, DeviceInputType.None, InputAction.None);
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
            var inputDef = new InteractionMapping(1, AxisType.SingleAxis, DeviceInputType.None, InputAction.None);
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
            var inputDef = new InteractionMapping(1, AxisType.SingleAxis, DeviceInputType.None, InputAction.None);
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
            var inputDef = new InteractionMapping(1, AxisType.DualAxis, DeviceInputType.None, InputAction.None);
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
            var inputDef = new InteractionMapping(1, AxisType.DualAxis, DeviceInputType.None, InputAction.None);
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
            var inputDef = new InteractionMapping(1, AxisType.ThreeDofPosition, DeviceInputType.None, InputAction.None);
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
            var inputDef = new InteractionMapping(1, AxisType.ThreeDofPosition, DeviceInputType.None, InputAction.None);
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
            var inputDef = new InteractionMapping(1, AxisType.ThreeDofRotation, DeviceInputType.None, InputAction.None);
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
            var inputDef = new InteractionMapping(1, AxisType.ThreeDofRotation, DeviceInputType.None, InputAction.None);
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

        #region SixDof

        [Test]
        public void Test13_TestSixDofChanged()
        {
            var inputDef = new InteractionMapping(1, AxisType.SixDof, DeviceInputType.None, InputAction.None);
            var testValue1 = new SixDof(Vector3.up, Quaternion.identity);
            var testValue2 = new SixDof(Vector3.down, new Quaternion(180f, 180f, 180f, 1f));

            var initialValue = inputDef.GetTransform();

            Assert.IsTrue(initialValue.Position == Vector3.zero && initialValue.Rotation == Quaternion.identity);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue(testValue1);

            Assert.IsTrue(inputDef.Changed);

            var setValue1 = inputDef.GetTransform();

            Assert.IsTrue(setValue1 == testValue1);
            Assert.IsTrue(setValue1.Position == testValue1.Position);
            Assert.IsTrue(setValue1.Rotation == testValue1.Rotation);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue(testValue2);

            Assert.IsTrue(inputDef.Changed);

            var setValue2 = inputDef.GetTransform();

            Assert.IsTrue(setValue2 == testValue2);
            Assert.IsTrue(setValue2.Position == testValue2.Position);
            Assert.IsTrue(setValue2.Rotation == testValue2.Rotation);
            Assert.IsFalse(inputDef.Changed);
        }

        [Test]
        public void Test14_TestSixDofNoChange()
        {
            var inputDef = new InteractionMapping(1, AxisType.SixDof, DeviceInputType.None, InputAction.None);
            var testValue = new SixDof(Vector3.up, Quaternion.identity);

            var initialValue = inputDef.GetTransform();

            Assert.IsTrue(initialValue.Position == Vector3.zero && initialValue.Rotation == Quaternion.identity);
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
            var interactions = new System.Collections.Generic.Dictionary<DeviceInputType, InteractionMapping>
            {
                {
                    DeviceInputType.None,
                    new InteractionMapping(1, AxisType.Raw, DeviceInputType.None, InputAction.None)
                }
            };
            var testValue1 = (object)1f;
            var testValue2 = (object)false;

            var initialValue = interactions[DeviceInputType.None].GetRawValue();

            Assert.IsNull(initialValue);
            Assert.IsFalse(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            interactions.SetDictionaryValue(DeviceInputType.None, testValue1);

            Assert.IsTrue(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            var setValue1 = interactions[DeviceInputType.None].GetRawValue();

            Assert.IsNotNull(setValue1);
            Assert.AreEqual(setValue1, testValue1);
            Assert.IsFalse(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            interactions.SetDictionaryValue(DeviceInputType.None, testValue2);

            Assert.IsTrue(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            var setValue2 = interactions[DeviceInputType.None].GetRawValue();

            Assert.IsNotNull(setValue2);
            Assert.AreEqual(setValue2, testValue2);
            Assert.IsFalse(interactions.GetDictionaryValueChanged(DeviceInputType.None));
        }

        [Test]
        public void Test16_InteractionDictionaryBool()
        {
            var interactions = new System.Collections.Generic.Dictionary<DeviceInputType, InteractionMapping>();
            interactions.Add(DeviceInputType.None, new InteractionMapping(1, AxisType.Digital, DeviceInputType.None, InputAction.None));
            const bool testValue1 = true;
            const bool testValue2 = false;

            var initialValue = interactions[DeviceInputType.None].GetBooleanValue();

            Assert.IsFalse(initialValue);
            Assert.IsFalse(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            interactions.SetDictionaryValue(DeviceInputType.None, testValue1);

            Assert.IsTrue(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            var setValue1 = interactions[DeviceInputType.None].GetBooleanValue();

            Assert.IsTrue(setValue1);
            Assert.IsTrue(setValue1 == testValue1);
            Assert.IsFalse(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            interactions.SetDictionaryValue(DeviceInputType.None, testValue2);

            Assert.IsTrue(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            var setValue2 = interactions[DeviceInputType.None].GetBooleanValue();

            Assert.IsFalse(setValue2);
            Assert.IsTrue(setValue2 == testValue2);
            Assert.IsFalse(interactions.GetDictionaryValueChanged(DeviceInputType.None));
        }

        [Test]
        public void Test17_InteractionDictionaryFloat()
        {
            var interactions = new System.Collections.Generic.Dictionary<DeviceInputType, InteractionMapping>();
            interactions.Add(DeviceInputType.None, new InteractionMapping(1, AxisType.SingleAxis, DeviceInputType.None, InputAction.None));
            var testValue1 = 1f;
            var testValue2 = 9001f;

            var initialValue = interactions[DeviceInputType.None].GetFloatValue();

            Assert.AreEqual(initialValue, 0d, double.Epsilon);
            Assert.IsFalse(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            interactions.SetDictionaryValue(DeviceInputType.None, testValue1);

            Assert.IsTrue(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            var setValue1 = interactions[DeviceInputType.None].GetFloatValue();

            Assert.AreEqual(setValue1, testValue1, double.Epsilon);
            Assert.IsFalse(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            interactions.SetDictionaryValue(DeviceInputType.None, testValue2);

            Assert.IsTrue(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            var setValue2 = interactions[DeviceInputType.None].GetFloatValue();

            Assert.AreEqual(setValue2, testValue2, double.Epsilon);
            Assert.IsFalse(interactions.GetDictionaryValueChanged(DeviceInputType.None));
        }

        [Test]
        public void Test18_InteractionDictionaryVector2()
        {
            var interactions = new System.Collections.Generic.Dictionary<DeviceInputType, InteractionMapping>();
            interactions.Add(DeviceInputType.None, new InteractionMapping(1, AxisType.DualAxis, DeviceInputType.None, InputAction.None));
            var testValue1 = Vector2.one;
            var testValue2 = Vector2.zero;

            var initialValue = interactions[DeviceInputType.None].GetVector2Value();

            Assert.True(initialValue == Vector2.zero);
            Assert.IsFalse(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            interactions.SetDictionaryValue(DeviceInputType.None, testValue1);

            Assert.IsTrue(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            var setValue1 = interactions[DeviceInputType.None].GetVector2Value();

            Assert.True(setValue1 == testValue1);
            Assert.IsFalse(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            interactions.SetDictionaryValue(DeviceInputType.None, testValue2);

            Assert.IsTrue(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            var setValue2 = interactions[DeviceInputType.None].GetVector2Value();

            Assert.True(setValue2 == testValue2);
            Assert.IsFalse(interactions.GetDictionaryValueChanged(DeviceInputType.None));
        }

        [Test]
        public void Test19_InteractionDictionaryVector3()
        {
            var interactions = new System.Collections.Generic.Dictionary<DeviceInputType, InteractionMapping>();
            interactions.Add(DeviceInputType.None, new InteractionMapping(1, AxisType.ThreeDofPosition, DeviceInputType.None, InputAction.None));
            var testValue1 = Vector3.one;
            var testValue2 = Vector3.zero;

            var initialValue = interactions[DeviceInputType.None].GetPosition();

            Assert.True(initialValue == Vector3.zero);
            Assert.IsFalse(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            interactions.SetDictionaryValue(DeviceInputType.None, testValue1);

            Assert.IsTrue(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            var setValue1 = interactions[DeviceInputType.None].GetPosition();

            Assert.True(setValue1 == testValue1);
            Assert.IsFalse(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            interactions.SetDictionaryValue(DeviceInputType.None, testValue2);

            Assert.IsTrue(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            var setValue2 = interactions[DeviceInputType.None].GetPosition();

            Assert.True(setValue2 == testValue2);
            Assert.IsFalse(interactions.GetDictionaryValueChanged(DeviceInputType.None));
        }

        [Test]
        public void Test20_InteractionDictionaryQuaternion()
        {
            var interactions = new System.Collections.Generic.Dictionary<DeviceInputType, InteractionMapping>();
            interactions.Add(DeviceInputType.None, new InteractionMapping(1, AxisType.ThreeDofRotation, DeviceInputType.None, InputAction.None));
            var testValue1 = Quaternion.Euler(45f, 45f, 45f);
            var testValue2 = Quaternion.identity;

            var initialValue = interactions[DeviceInputType.None].GetRotation();

            Assert.True(initialValue == Quaternion.identity);
            Assert.IsFalse(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            interactions.SetDictionaryValue(DeviceInputType.None, testValue1);

            Assert.IsTrue(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            var setValue1 = interactions[DeviceInputType.None].GetRotation();

            Assert.True(setValue1 == testValue1);
            Assert.IsFalse(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            interactions.SetDictionaryValue(DeviceInputType.None, testValue2);

            Assert.IsTrue(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            var setValue2 = interactions[DeviceInputType.None].GetRotation();

            Assert.True(setValue2 == testValue2);
            Assert.IsFalse(interactions.GetDictionaryValueChanged(DeviceInputType.None));
        }

        [Test]
        public void Test21_InteractionDictionarySixDof()
        {
            var interactions = new System.Collections.Generic.Dictionary<DeviceInputType, InteractionMapping>();
            interactions.Add(DeviceInputType.None, new InteractionMapping(1, AxisType.SixDof, DeviceInputType.None, InputAction.None));
            var testValue1 = new SixDof(Vector3.up, Quaternion.identity);
            var testValue2 = new SixDof(Vector3.down, new Quaternion(180f, 180f, 180f, 1f));

            var initialValue = interactions[DeviceInputType.None].GetTransform();

            Assert.IsTrue(initialValue.Position == Vector3.zero && initialValue.Rotation == Quaternion.identity);
            Assert.IsFalse(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            interactions.SetDictionaryValue(DeviceInputType.None, testValue1);

            Assert.IsTrue(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            var setValue1 = interactions[DeviceInputType.None].GetTransform();

            Assert.IsTrue(setValue1 == testValue1);
            Assert.IsTrue(setValue1.Position == testValue1.Position);
            Assert.IsTrue(setValue1.Rotation == testValue1.Rotation);
            Assert.IsFalse(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            interactions.SetDictionaryValue(DeviceInputType.None, testValue2);

            Assert.IsTrue(interactions.GetDictionaryValueChanged(DeviceInputType.None));

            var setValue2 = interactions[DeviceInputType.None].GetTransform();

            Assert.IsTrue(setValue2 == testValue2);
            Assert.IsTrue(setValue2.Position == testValue2.Position);
            Assert.IsTrue(setValue2.Rotation == testValue2.Rotation);
            Assert.IsFalse(interactions.GetDictionaryValueChanged(DeviceInputType.None));
        }

        #endregion Interaction Dictionary
    }
}