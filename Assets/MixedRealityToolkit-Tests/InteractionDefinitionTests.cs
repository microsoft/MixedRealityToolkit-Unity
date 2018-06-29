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
            var interaction = new MixedRealityInteractionMapping(1, AxisType.Raw, DeviceInputType.None, MixedRealityInputAction.None);
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
            var interaction = new MixedRealityInteractionMapping(1, AxisType.Raw, DeviceInputType.None, MixedRealityInputAction.None);
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
            var interaction = new MixedRealityInteractionMapping(1, AxisType.Digital, DeviceInputType.None, MixedRealityInputAction.None);
            var testValue1 = true;
            var testValue2 = false;

            var initialValue = interaction.GetBoolValue();

            Assert.IsFalse(initialValue);
            Assert.IsFalse(interaction.Changed);

            interaction.SetBoolValue(testValue1);

            Assert.IsTrue(interaction.Changed);

            var setValue1 = interaction.GetBoolValue();

            Assert.IsTrue(setValue1);
            Assert.True(setValue1 == testValue1);
            Assert.IsFalse(interaction.Changed);

            interaction.SetBoolValue(testValue2);

            Assert.IsTrue(interaction.Changed);

            var setValue2 = interaction.GetBoolValue();

            Assert.IsFalse(setValue2);
            Assert.True(setValue2 == testValue2);
            Assert.IsFalse(interaction.Changed);
        }

        [Test]
        public void Test04_TestBoolNoChange()
        {
            var interaction = new MixedRealityInteractionMapping(1, AxisType.Digital, DeviceInputType.None, MixedRealityInputAction.None);
            var testValue = true;

            var initialValue = interaction.GetBoolValue();

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
            var interaction = new MixedRealityInteractionMapping(1, AxisType.SingleAxis, DeviceInputType.None, MixedRealityInputAction.None);
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
            var interaction = new MixedRealityInteractionMapping(1, AxisType.SingleAxis, DeviceInputType.None, MixedRealityInputAction.None);
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
            var interaction = new MixedRealityInteractionMapping(1, AxisType.DualAxis, DeviceInputType.None, MixedRealityInputAction.None);
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
            var interaction = new MixedRealityInteractionMapping(1, AxisType.DualAxis, DeviceInputType.None, MixedRealityInputAction.None);
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
            var interaction = new MixedRealityInteractionMapping(1, AxisType.ThreeDofPosition, DeviceInputType.None, MixedRealityInputAction.None);
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
            var interaction = new MixedRealityInteractionMapping(1, AxisType.ThreeDofPosition, DeviceInputType.None, MixedRealityInputAction.None);
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
            var interaction = new MixedRealityInteractionMapping(1, AxisType.ThreeDofRotation, DeviceInputType.None, MixedRealityInputAction.None);
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
            var interaction = new MixedRealityInteractionMapping(1, AxisType.ThreeDofRotation, DeviceInputType.None, MixedRealityInputAction.None);
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

        #region MixedRealityPose

        [Test]
        public void Test13_TestMixedRealityPoseChanged()
        {
            var interaction = new MixedRealityInteractionMapping(1, AxisType.SixDof, DeviceInputType.None, MixedRealityInputAction.None);
            var testValue1 = new MixedRealityPose(Vector3.up, Quaternion.identity);
            var testValue2 = new MixedRealityPose(Vector3.one, new Quaternion(45f, 45f, 45f, 45f));

            var initialValue = interaction.GetSixDofValue();

            Assert.IsTrue(initialValue == MixedRealityPose.ZeroIdentity);
            Assert.IsFalse(interaction.Changed);

            interaction.SetPoseValue(testValue1);

            Assert.IsTrue(interaction.Changed);

            var setValue1 = interaction.GetSixDofValue();

            Assert.IsTrue(setValue1 == testValue1);
            Assert.IsFalse(interaction.Changed);

            interaction.SetPoseValue(testValue2);

            Assert.IsTrue(interaction.Changed);

            var setValue2 = interaction.GetSixDofValue();

            Assert.IsTrue(setValue2 == testValue2);
            Assert.IsFalse(interaction.Changed);
        }

        [Test]
        public void Test14_TesMixedRealityPoseNoChange()
        {
            var interaction = new MixedRealityInteractionMapping(1, AxisType.SixDof, DeviceInputType.None, MixedRealityInputAction.None);
            var testValue = new MixedRealityPose(Vector3.up, Quaternion.identity);

            var initialValue = interaction.GetSixDofValue();

            Assert.IsTrue(initialValue == MixedRealityPose.ZeroIdentity);
            Assert.IsFalse(interaction.Changed);

            interaction.SetPoseValue(testValue);

            Assert.IsTrue(interaction.Changed);

            // Make sure the second time we query it's false
            Assert.IsFalse(interaction.Changed);

            interaction.SetPoseValue(testValue);

            // Make sure if we set the same value it's false
            Assert.IsFalse(interaction.Changed);
        }

        #endregion MixedRealityPose

        #region Interaction Array Tests

        [Test]
        public void Test15_InteractionArrayObject()
        {
            var interactions = new MixedRealityInteractionMapping[1];
            interactions[0] = new MixedRealityInteractionMapping(1, AxisType.Raw, DeviceInputType.None, MixedRealityInputAction.None);
            var testValue1 = (object)1f;
            var testValue2 = (object)false;

            var initialValue = interactions[0];

            Assert.IsNotNull(initialValue);
            Assert.IsNull(initialValue.GetRawValue());
            Assert.IsFalse(initialValue.Changed);

            initialValue.SetRawValue(testValue1);

            Assert.IsTrue(initialValue.Changed);

            var setValue1 = interactions[0];

            Assert.IsNotNull(setValue1);
            Assert.IsNotNull(setValue1.GetRawValue());
            Assert.AreEqual(setValue1.GetRawValue(), testValue1);
            Assert.IsFalse(setValue1.Changed);

            setValue1.SetRawValue(testValue2);

            Assert.IsTrue(setValue1.Changed);

            var setValue2 = interactions[0];

            Assert.IsNotNull(setValue2);
            Assert.IsNotNull(setValue2.GetRawValue());
            Assert.AreEqual(setValue2.GetRawValue(), testValue2);
            Assert.IsFalse(setValue2.Changed);
        }

        [Test]
        public void Test16_InteractionArrayBool()
        {
            var interactions = new MixedRealityInteractionMapping[1];
            interactions[0] = new MixedRealityInteractionMapping(1, AxisType.Digital, DeviceInputType.None, MixedRealityInputAction.None);
            var testValue1 = true;
            var testValue2 = false;

            var initialValue = interactions[0];

            Assert.IsNotNull(initialValue);
            Assert.IsFalse(initialValue.GetBoolValue());
            Assert.IsFalse(initialValue.Changed);

            initialValue.SetBoolValue(testValue1);

            Assert.IsTrue(initialValue.Changed);

            var setValue1 = interactions[0];

            Assert.IsNotNull(setValue1);
            Assert.IsTrue(setValue1.GetBoolValue());
            Assert.IsTrue(setValue1.GetBoolValue() == testValue1);
            Assert.IsFalse(setValue1.Changed);

            setValue1.SetBoolValue(testValue2);

            Assert.IsTrue(setValue1.Changed);

            var setValue2 = interactions[0];

            Assert.IsNotNull(setValue2);
            Assert.IsFalse(setValue2.GetBoolValue());
            Assert.IsTrue(setValue2.GetBoolValue() == testValue2);
            Assert.IsFalse(setValue2.Changed);
        }

        [Test]
        public void Test17_InteractionArrayFloat()
        {
            var interactions = new MixedRealityInteractionMapping[1];
            interactions[0] = new MixedRealityInteractionMapping(1, AxisType.SingleAxis, DeviceInputType.None, MixedRealityInputAction.None);
            var testValue1 = 1f;
            var testValue2 = 9001f;

            var initialValue = interactions[0];

            Assert.IsNotNull(initialValue);
            Assert.AreEqual(initialValue.GetFloatValue(), 0d, double.Epsilon);
            Assert.IsFalse(initialValue.Changed);

            initialValue.SetFloatValue(testValue1);

            Assert.IsTrue(initialValue.Changed);

            var setValue1 = interactions[0];

            Assert.IsNotNull(setValue1);
            Assert.AreEqual(setValue1.GetFloatValue(), testValue1, double.Epsilon);
            Assert.IsFalse(setValue1.Changed);

            setValue1.SetFloatValue(testValue2);

            Assert.IsTrue(setValue1.Changed);

            var setValue2 = interactions[0];

            Assert.IsNotNull(setValue2);
            Assert.AreEqual(setValue2.GetFloatValue(), testValue2, double.Epsilon);
            Assert.IsFalse(setValue2.Changed);
        }

        [Test]
        public void Test18_InteractionArrayVector2()
        {
            var interactions = new MixedRealityInteractionMapping[1];
            interactions[0] = new MixedRealityInteractionMapping(1, AxisType.DualAxis, DeviceInputType.None, MixedRealityInputAction.None);
            var testValue1 = Vector2.one;
            var testValue2 = Vector2.zero;

            var initialValue = interactions[0];

            Assert.IsNotNull(initialValue);
            Assert.True(initialValue.GetVector2Value() == Vector2.zero);
            Assert.IsFalse(initialValue.Changed);

            initialValue.SetVector2Value(testValue1);

            Assert.IsTrue(initialValue.Changed);

            var setValue1 = interactions[0];

            Assert.IsNotNull(setValue1);
            Assert.True(setValue1.GetVector2Value() == testValue1);
            Assert.IsFalse(setValue1.Changed);

            setValue1.SetVector2Value(testValue2);

            Assert.IsTrue(setValue1.Changed);

            var setValue2 = interactions[0];

            Assert.IsNotNull(setValue2);
            Assert.True(setValue2.GetVector2Value() == testValue2);
            Assert.IsFalse(setValue2.Changed);
        }

        [Test]
        public void Test19_InteractionArrayVector3()
        {
            var interactions = new MixedRealityInteractionMapping[1];
            interactions[0] = new MixedRealityInteractionMapping(1, AxisType.ThreeDofPosition, DeviceInputType.None, MixedRealityInputAction.None);
            var testValue1 = Vector3.one;
            var testValue2 = Vector3.zero;

            var initialValue = interactions[0];

            Assert.IsNotNull(initialValue);
            Assert.True(initialValue.GetPositionValue() == Vector3.zero);
            Assert.IsFalse(initialValue.Changed);

            initialValue.SetPositionValue(testValue1);

            Assert.IsTrue(initialValue.Changed);

            var setValue1 = interactions[0];

            Assert.IsNotNull(setValue1);
            Assert.True(setValue1.GetPositionValue() == testValue1);
            Assert.IsFalse(setValue1.Changed);

            setValue1.SetPositionValue(testValue2);

            Assert.IsTrue(setValue1.Changed);

            var setValue2 = interactions[0];

            Assert.IsNotNull(setValue2);
            Assert.True(setValue2.GetPositionValue() == testValue2);
            Assert.IsFalse(setValue2.Changed);
        }

        [Test]
        public void Test20_InteractionArrayQuaternion()
        {
            var interactions = new MixedRealityInteractionMapping[1];
            interactions[0] = new MixedRealityInteractionMapping(1, AxisType.ThreeDofRotation, DeviceInputType.None, MixedRealityInputAction.None);
            var testValue1 = Quaternion.Euler(45f, 45f, 45f);
            var testValue2 = Quaternion.identity;

            var initialValue = interactions[0];

            Assert.IsNotNull(initialValue);
            Assert.True(initialValue.GetRotationValue().eulerAngles == Quaternion.identity.eulerAngles);
            Assert.IsFalse(initialValue.Changed);

            initialValue.SetRotationValue(testValue1);

            Assert.IsTrue(initialValue.Changed);

            var setValue1 = interactions[0];

            Assert.IsNotNull(setValue1);
            Assert.True(setValue1.GetRotationValue() == testValue1);
            Assert.IsFalse(setValue1.Changed);

            setValue1.SetRotationValue(testValue2);

            Assert.IsTrue(setValue1.Changed);

            var setValue2 = interactions[0];

            Assert.IsNotNull(setValue2);
            Assert.True(setValue2.GetRotationValue() == testValue2);
            Assert.IsFalse(setValue2.Changed);
        }

        [Test]
        public void Test21_InteractionArrayMixedRealityPose()
        {
            var interactions = new MixedRealityInteractionMapping[1];
            interactions[0] = new MixedRealityInteractionMapping(1, AxisType.SixDof, DeviceInputType.None, MixedRealityInputAction.None);
            var testValue1 = new MixedRealityPose(Vector3.one, new Quaternion(45f, 45f, 45f, 45f));
            var testValue2 = new MixedRealityPose(Vector3.zero, Quaternion.identity);

            var initialValue = interactions[0];

            Assert.IsNotNull(initialValue);
            MixedRealityPose initialSixDofValue = initialValue.GetSixDofValue();

            Assert.IsTrue(initialSixDofValue.Position == Vector3.zero);
            Assert.IsTrue(initialSixDofValue == MixedRealityPose.ZeroIdentity);
            Assert.IsFalse(initialValue.Changed);

            initialValue.SetPoseValue(testValue1);

            Assert.IsTrue(initialValue.Changed);

            var setValue1 = interactions[0];

            Assert.IsNotNull(setValue1);
            Assert.IsTrue(setValue1.GetSixDofValue() == testValue1);
            Assert.IsTrue(setValue1.GetSixDofValue().Position == testValue1.Position);
            Assert.IsTrue(setValue1.GetSixDofValue().Rotation == testValue1.Rotation);
            Assert.IsFalse(setValue1.Changed);

            setValue1.SetPoseValue(testValue2);

            Assert.IsTrue(setValue1.Changed);

            var setValue2 = interactions[0];

            Assert.IsNotNull(setValue2);
            Assert.IsTrue(setValue2.GetSixDofValue() == testValue2);
            Assert.IsTrue(setValue2.GetSixDofValue().Position == testValue2.Position);
            Assert.IsTrue(setValue2.GetSixDofValue().Rotation == testValue2.Rotation);
            Assert.IsFalse(setValue1.Changed);
        }
        #endregion Interaction Array Tests

    }
}