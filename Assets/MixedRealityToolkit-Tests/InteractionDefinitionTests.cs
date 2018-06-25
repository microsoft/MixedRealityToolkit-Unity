// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
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
            var interaction = new MixedRealityInteractionMapping(1, AxisType.Raw, DeviceInputType.None, InputAction.None);
            var testValue1 = (object)1f;
            var testValue2 = (object)false;

            var initialValue = interaction.GetRawValue();

            Assert.IsNull(initialValue);
            Assert.IsFalse(interaction.Changed);

            interaction.SetRawValue(testValue1);

            Assert.IsTrue(interaction.Changed);

            var setValue1 = interaction.GetRawValue();

            Assert.IsNotNull(setValue1);
            Assert.AreEqual(setValue1, testValue1);
            Assert.IsFalse(interaction.Changed);

            interaction.SetRawValue(testValue2);

            Assert.IsTrue(interaction.Changed);

            var setValue2 = interaction.GetRawValue();

            Assert.IsNotNull(setValue2);
            Assert.AreEqual(setValue2, testValue2);
            Assert.IsFalse(interaction.Changed);
        }

        [Test]
        public void Test02_TestObjectNoChange()
        {
            var interaction = new MixedRealityInteractionMapping(1, AxisType.Raw, DeviceInputType.None, InputAction.None);
            var testValue = new object();

            var initialValue = interaction.GetRawValue();

            Assert.IsNull(initialValue);
            Assert.IsFalse(interaction.Changed);

            interaction.SetRawValue(testValue);

            Assert.IsTrue(interaction.Changed);

            // Make sure the second time we query it's false
            Assert.IsFalse(interaction.Changed);

            interaction.SetRawValue(testValue);

            // Make sure if we set the same value it's false
            Assert.IsFalse(interaction.Changed);

            //Check setting the value twice with the same value produces no change
            var newValue = interaction.GetRawValue();

            // Make sure if we set the same value it's false
            Assert.IsFalse(interaction.Changed, newValue.ToString());

            // Make sure setting again after query, we query again it's false
            interaction.SetRawValue(testValue);

            // Make sure if we set the same value it's false
            Assert.IsFalse(interaction.Changed);
        }

        #endregion objects

        #region bools

        [Test]
        public void Test03_TestBoolChanged()
        {
            var interaction = new MixedRealityInteractionMapping(1, AxisType.Digital, DeviceInputType.None, InputAction.None);
            var testValue1 = true;
            var testValue2 = false;

            var initialValue = interaction.GetBooleanValue();

            Assert.IsFalse(initialValue);
            Assert.IsFalse(interaction.Changed);

            interaction.SetBoolValue(testValue1);

            Assert.IsTrue(interaction.Changed);

            var setValue1 = interaction.GetBooleanValue();

            Assert.IsTrue(setValue1);
            Assert.True(setValue1 == testValue1);
            Assert.IsFalse(interaction.Changed);

            interaction.SetBoolValue(testValue2);

            Assert.IsTrue(interaction.Changed);

            var setValue2 = interaction.GetBooleanValue();

            Assert.IsFalse(setValue2);
            Assert.True(setValue2 == testValue2);
            Assert.IsFalse(interaction.Changed);
        }

        [Test]
        public void Test04_TestBoolNoChange()
        {
            var interaction = new MixedRealityInteractionMapping(1, AxisType.Digital, DeviceInputType.None, InputAction.None);
            var testValue = true;

            var initialValue = interaction.GetBooleanValue();

            Assert.IsFalse(initialValue);
            Assert.IsFalse(interaction.Changed);

            interaction.SetBoolValue(testValue);

            Assert.IsTrue(interaction.Changed);

            // Make sure the second time we query it's false
            Assert.IsFalse(interaction.Changed);

            interaction.SetBoolValue(testValue);

            // Make sure if we set the same value it's false
            Assert.IsFalse(interaction.Changed);
        }

        #endregion bools

        #region float

        [Test]
        public void Test05_TestFloatChanged()
        {
            var interaction = new MixedRealityInteractionMapping(1, AxisType.SingleAxis, DeviceInputType.None, InputAction.None);
            var testValue1 = 1f;
            var testValue2 = 9001f;

            var initialValue = interaction.GetFloatValue();

            Assert.AreEqual(initialValue, 0d, double.Epsilon);
            Assert.IsFalse(interaction.Changed);

            interaction.SetFloatValue(testValue1);

            Assert.IsTrue(interaction.Changed);

            var setValue1 = interaction.GetFloatValue();

            Assert.AreEqual(setValue1, testValue1, double.Epsilon);
            Assert.IsFalse(interaction.Changed);

            interaction.SetFloatValue(testValue2);

            Assert.IsTrue(interaction.Changed);

            var setValue2 = interaction.GetFloatValue();

            Assert.AreEqual(setValue2, testValue2, double.Epsilon);
            Assert.IsFalse(interaction.Changed);
        }

        [Test]
        public void Test06_TestFloatNoChange()
        {
            var interaction = new MixedRealityInteractionMapping(1, AxisType.SingleAxis, DeviceInputType.None, InputAction.None);
            var testValue = 1f;

            var initialValue = interaction.GetFloatValue();

            Assert.AreEqual(initialValue, 0d, double.Epsilon);
            Assert.IsFalse(interaction.Changed);

            interaction.SetFloatValue(testValue);

            Assert.IsTrue(interaction.Changed);

            // Make sure the second time we query it's false
            Assert.IsFalse(interaction.Changed);

            interaction.SetFloatValue(testValue);

            // Make sure if we set the same value it's false
            Assert.IsFalse(interaction.Changed);
        }

        #endregion float

        #region Vector2

        [Test]
        public void Test07_TestVector2Changed()
        {
            var interaction = new MixedRealityInteractionMapping(1, AxisType.DualAxis, DeviceInputType.None, InputAction.None);
            var testValue1 = Vector2.one;
            var testValue2 = Vector2.zero;

            var initialValue = interaction.GetVector2Value();

            Assert.True(initialValue == Vector2.zero);
            Assert.IsFalse(interaction.Changed);

            interaction.SetVector2Value(testValue1);

            Assert.IsTrue(interaction.Changed);

            var setValue1 = interaction.GetVector2Value();

            Assert.True(setValue1 == testValue1);
            Assert.IsFalse(interaction.Changed);

            interaction.SetVector2Value(testValue2);

            Assert.IsTrue(interaction.Changed);

            var setValue2 = interaction.GetVector2Value();

            Assert.True(setValue2 == testValue2);
            Assert.IsFalse(interaction.Changed);
        }

        [Test]
        public void Test08_TestVector2NoChange()
        {
            var interaction = new MixedRealityInteractionMapping(1, AxisType.DualAxis, DeviceInputType.None, InputAction.None);
            var testValue = Vector2.one;

            var initialValue = interaction.GetVector2Value();

            Assert.True(initialValue == Vector2.zero);
            Assert.IsFalse(interaction.Changed);

            interaction.SetVector2Value(testValue);

            Assert.IsTrue(interaction.Changed);

            // Make sure the second time we query it's false
            Assert.IsFalse(interaction.Changed);

            interaction.SetVector2Value(testValue);

            // Make sure if we set the same value it's false
            Assert.IsFalse(interaction.Changed);
        }

        #endregion Vector2

        #region Vector3

        [Test]
        public void Test09_TestVector3Changed()
        {
            var interaction = new MixedRealityInteractionMapping(1, AxisType.ThreeDofPosition, DeviceInputType.None, InputAction.None);
            var testValue1 = Vector3.one;
            var testValue2 = Vector3.zero;

            var initialValue = interaction.GetPositionValue();

            Assert.True(initialValue == Vector3.zero);
            Assert.IsFalse(interaction.Changed);

            interaction.SetPositionValue(testValue1);

            Assert.IsTrue(interaction.Changed);

            var setValue1 = interaction.GetPositionValue();

            Assert.True(setValue1 == testValue1);
            Assert.IsFalse(interaction.Changed);

            interaction.SetPositionValue(testValue2);

            Assert.IsTrue(interaction.Changed);

            var setValue2 = interaction.GetPositionValue();

            Assert.True(setValue2 == testValue2);
            Assert.IsFalse(interaction.Changed);
        }

        [Test]
        public void Test10_TestVector3NoChange()
        {
            var interaction = new MixedRealityInteractionMapping(1, AxisType.ThreeDofPosition, DeviceInputType.None, InputAction.None);
            var testValue = Vector3.one;

            var initialValue = interaction.GetPositionValue();

            Assert.True(initialValue == Vector3.zero);
            Assert.IsFalse(interaction.Changed);

            interaction.SetPositionValue(testValue);

            Assert.IsTrue(interaction.Changed);

            // Make sure the second time we query it's false
            Assert.IsFalse(interaction.Changed);

            interaction.SetPositionValue(testValue);

            // Make sure if we set the same value it's false
            Assert.IsFalse(interaction.Changed);
        }

        #endregion Vector3

        #region Quaternion

        [Test]
        public void Test11_TestQuaternionChanged()
        {
            var interaction = new MixedRealityInteractionMapping(1, AxisType.ThreeDofRotation, DeviceInputType.None, InputAction.None);
            var testValue1 = Quaternion.Euler(45f, 45f, 45f);
            var testValue2 = Quaternion.identity;

            var initialValue = interaction.GetRotationValue();

            Assert.IsTrue(initialValue == Quaternion.identity);
            Assert.IsFalse(interaction.Changed);

            interaction.SetRotationValue(testValue1);

            Assert.IsTrue(interaction.Changed);

            var setValue1 = interaction.GetRotationValue();

            Assert.True(setValue1 == testValue1);
            Assert.IsFalse(interaction.Changed);

            interaction.SetRotationValue(testValue2);

            Assert.IsTrue(interaction.Changed);

            var setValue2 = interaction.GetRotationValue();

            Assert.True(setValue2 == testValue2);
            Assert.IsFalse(interaction.Changed);
        }

        [Test]
        public void Test12_TestQuaternionNoChange()
        {
            var interaction = new MixedRealityInteractionMapping(1, AxisType.ThreeDofRotation, DeviceInputType.None, InputAction.None);
            var testValue = Quaternion.Euler(45f, 45f, 45f);

            var initialValue = interaction.GetRotationValue();

            Assert.IsTrue(initialValue == Quaternion.identity);
            Assert.IsFalse(interaction.Changed);

            interaction.SetRotationValue(testValue);

            Assert.IsTrue(interaction.Changed);

            // Make sure the second time we query it's false
            Assert.IsFalse(interaction.Changed);

            interaction.SetRotationValue(testValue);

            // Make sure if we set the same value it's false
            Assert.IsFalse(interaction.Changed);
        }

        #endregion Quaternion

        #region SixDof

        [Test]
        public void Test13_TestSixDofChanged()
        {
            var interaction = new MixedRealityInteractionMapping(1, AxisType.SixDof, DeviceInputType.None, InputAction.None);
            var testValue1 = new SixDof(Vector3.up, Quaternion.identity);
            var testValue2 = new SixDof(Vector3.one, new Quaternion(45f, 45f, 45f, 45f));

            var initialValue = interaction.GetSixDofValue();

            Assert.IsTrue(initialValue == SixDof.ZeroIdentity);
            Assert.IsFalse(interaction.Changed);

            interaction.SetSixDofValue(testValue1);

            Assert.IsTrue(interaction.Changed);

            var setValue1 = interaction.GetSixDofValue();

            Assert.IsTrue(setValue1 == testValue1);
            Assert.IsFalse(interaction.Changed);

            interaction.SetSixDofValue(testValue2);

            Assert.IsTrue(interaction.Changed);

            var setValue2 = interaction.GetSixDofValue();

            Assert.IsTrue(setValue2 == testValue2);
            Assert.IsFalse(interaction.Changed);
        }

        [Test]
        public void Test14_TestSixDofNoChange()
        {
            var interaction = new MixedRealityInteractionMapping(1, AxisType.SixDof, DeviceInputType.None, InputAction.None);
            var testValue = new SixDof(Vector3.up, Quaternion.identity);

            var initialValue = interaction.GetSixDofValue();

            Assert.IsTrue(initialValue == SixDof.ZeroIdentity);
            Assert.IsFalse(interaction.Changed);

            interaction.SetSixDofValue(testValue);

            Assert.IsTrue(interaction.Changed);

            // Make sure the second time we query it's false
            Assert.IsFalse(interaction.Changed);

            interaction.SetSixDofValue(testValue);

            // Make sure if we set the same value it's false
            Assert.IsFalse(interaction.Changed);
        }

        #endregion SixDof
    }
}