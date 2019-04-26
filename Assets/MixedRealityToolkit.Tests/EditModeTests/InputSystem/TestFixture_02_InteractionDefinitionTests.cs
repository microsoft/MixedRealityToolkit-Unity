// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Tests.InputSystem
{
    public class TestFixture_02_InteractionDefinitionTests
    {
        #region objects

        [Test]
        public void Test01_TestObjectChanged()
        {
            var interaction = new MixedRealityInteractionMapping(1, string.Empty, AxisType.Raw, DeviceInputType.None, MixedRealityInputAction.None);
            var testValue1 = (object)1f;
            var testValue2 = (object)false;

            var initialValue = interaction.RawData;

            Assert.IsNull(initialValue);
            Assert.IsFalse(interaction.Changed);

            interaction.RawData = testValue1;

            Assert.IsTrue(interaction.Changed);

            var setValue1 = interaction.RawData;

            Assert.IsNotNull(setValue1);
            Assert.AreEqual(setValue1, testValue1);
            Assert.IsFalse(interaction.Changed);

            interaction.RawData = testValue2;

            Assert.IsTrue(interaction.Changed);

            var setValue2 = interaction.RawData;

            Assert.IsNotNull(setValue2);
            Assert.AreEqual(setValue2, testValue2);
            Assert.IsFalse(interaction.Changed);
        }

        [Test]
        public void Test02_TestObjectNoChange()
        {
            var interaction = new MixedRealityInteractionMapping(1, string.Empty, AxisType.Raw, DeviceInputType.None, MixedRealityInputAction.None);
            var testValue = new object();

            var initialValue = interaction.RawData;

            Assert.IsNull(initialValue);
            Assert.IsFalse(interaction.Changed);

            interaction.RawData = testValue;

            Assert.IsTrue(interaction.Changed);

            // Make sure the second time we query it's false
            Assert.IsFalse(interaction.Changed);

            interaction.RawData = testValue;

            // Make sure if we set the same value it's false
            Assert.IsFalse(interaction.Changed);

            //Check setting the value twice with the same value produces no change
            var newValue = interaction.RawData;

            // Make sure if we set the same value it's false
            Assert.IsFalse(interaction.Changed, newValue.ToString());

            // Make sure setting again after query, we query again it's false
            interaction.RawData = testValue;

            // Make sure if we set the same value it's false
            Assert.IsFalse(interaction.Changed);
        }

        #endregion objects

        #region bools

        [Test]
        public void Test03_TestBoolChanged()
        {
            var interaction = new MixedRealityInteractionMapping(1, string.Empty, AxisType.Digital, DeviceInputType.None, MixedRealityInputAction.None);
            var testValue1 = true;
            var testValue2 = false;

            var initialValue = interaction.BoolData;

            Assert.IsFalse(initialValue);
            Assert.IsFalse(interaction.Changed);

            interaction.BoolData = testValue1;

            Assert.IsTrue(interaction.Changed);

            var setValue1 = interaction.BoolData;

            Assert.IsTrue(setValue1);
            Assert.True(setValue1 == testValue1);
            Assert.IsFalse(interaction.Changed);

            interaction.BoolData = testValue2;

            Assert.IsTrue(interaction.Changed);

            var setValue2 = interaction.BoolData;

            Assert.IsFalse(setValue2);
            Assert.True(setValue2 == testValue2);
            Assert.IsFalse(interaction.Changed);
        }

        [Test]
        public void Test04_TestBoolNoChange()
        {
            var interaction = new MixedRealityInteractionMapping(1, string.Empty, AxisType.Digital, DeviceInputType.None, MixedRealityInputAction.None);
            var testValue = true;

            var initialValue = interaction.BoolData;

            Assert.IsFalse(initialValue);
            Assert.IsFalse(interaction.Changed);

            interaction.BoolData = testValue;

            Assert.IsTrue(interaction.Changed);

            // Make sure the second time we query it's false
            Assert.IsFalse(interaction.Changed);

            interaction.BoolData = testValue;

            // Make sure if we set the same value it's false
            Assert.IsFalse(interaction.Changed);
        }

        #endregion bools

        #region float

        [Test]
        public void Test05_TestFloatChanged()
        {
            var interaction = new MixedRealityInteractionMapping(1, string.Empty, AxisType.SingleAxis, DeviceInputType.None, MixedRealityInputAction.None);
            var testValue1 = 1f;
            var testValue2 = 9001f;

            var initialValue = interaction.FloatData;

            Assert.AreEqual(initialValue, 0d, double.Epsilon);
            Assert.IsFalse(interaction.Changed);

            interaction.FloatData = testValue1;

            Assert.IsTrue(interaction.Changed);

            var setValue1 = interaction.FloatData;

            Assert.AreEqual(setValue1, testValue1, double.Epsilon);
            Assert.IsFalse(interaction.Changed);

            interaction.FloatData = testValue2;

            Assert.IsTrue(interaction.Changed);

            var setValue2 = interaction.FloatData;

            Assert.AreEqual(setValue2, testValue2, double.Epsilon);
            Assert.IsFalse(interaction.Changed);
        }

        [Test]
        public void Test06_TestFloatNoChange()
        {
            var interaction = new MixedRealityInteractionMapping(1, string.Empty, AxisType.SingleAxis, DeviceInputType.None, MixedRealityInputAction.None);
            var testValue = 1f;

            var initialValue = interaction.FloatData;

            Assert.AreEqual(initialValue, 0d, double.Epsilon);
            Assert.IsFalse(interaction.Changed);

            interaction.FloatData = testValue;

            Assert.IsTrue(interaction.Changed);

            // Make sure the second time we query it's false
            Assert.IsFalse(interaction.Changed);

            interaction.FloatData = testValue;

            // Make sure if we set the same value it's false
            Assert.IsFalse(interaction.Changed);
        }

        #endregion float

        #region Vector2

        [Test]
        public void Test07_TestVector2Changed()
        {
            var interaction = new MixedRealityInteractionMapping(1, string.Empty, AxisType.DualAxis, DeviceInputType.None, MixedRealityInputAction.None);
            var testValue1 = Vector2.one;
            var testValue2 = Vector2.zero;

            var initialValue = interaction.Vector2Data;

            Assert.True(initialValue == Vector2.zero);
            Assert.IsFalse(interaction.Changed);

            interaction.Vector2Data = testValue1;

            Assert.IsTrue(interaction.Changed);

            var setValue1 = interaction.Vector2Data;

            Assert.True(setValue1 == testValue1);
            Assert.IsFalse(interaction.Changed);

            interaction.Vector2Data = testValue2;

            Assert.IsTrue(interaction.Changed);

            var setValue2 = interaction.Vector2Data;

            Assert.True(setValue2 == testValue2);
            Assert.IsFalse(interaction.Changed);
        }

        [Test]
        public void Test08_TestVector2NoChange()
        {
            var interaction = new MixedRealityInteractionMapping(1, string.Empty, AxisType.DualAxis, DeviceInputType.None, MixedRealityInputAction.None);
            var testValue = Vector2.one;

            var initialValue = interaction.Vector2Data;

            Assert.True(initialValue == Vector2.zero);
            Assert.IsFalse(interaction.Changed);

            interaction.Vector2Data = testValue;

            Assert.IsTrue(interaction.Changed);

            // Make sure the second time we query it's false
            Assert.IsFalse(interaction.Changed);

            interaction.Vector2Data = testValue;

            // Make sure if we set the same value it's false
            Assert.IsFalse(interaction.Changed);
        }

        #endregion Vector2

        #region Vector3

        [Test]
        public void Test09_TestVector3Changed()
        {
            var interaction = new MixedRealityInteractionMapping(1, string.Empty, AxisType.ThreeDofPosition, DeviceInputType.None, MixedRealityInputAction.None);
            var testValue1 = Vector3.one;
            var testValue2 = Vector3.zero;

            var initialValue = interaction.PositionData;

            Assert.True(initialValue == Vector3.zero);
            Assert.IsFalse(interaction.Changed);

            interaction.PositionData = testValue1;

            Assert.IsTrue(interaction.Changed);

            var setValue1 = interaction.PositionData;

            Assert.True(setValue1 == testValue1);
            Assert.IsFalse(interaction.Changed);

            interaction.PositionData = testValue2;

            Assert.IsTrue(interaction.Changed);

            var setValue2 = interaction.PositionData;

            Assert.True(setValue2 == testValue2);
            Assert.IsFalse(interaction.Changed);
        }

        [Test]
        public void Test10_TestVector3NoChange()
        {
            var interaction = new MixedRealityInteractionMapping(1, string.Empty, AxisType.ThreeDofPosition, DeviceInputType.None, MixedRealityInputAction.None);
            var testValue = Vector3.one;

            var initialValue = interaction.PositionData;

            Assert.True(initialValue == Vector3.zero);
            Assert.IsFalse(interaction.Changed);

            interaction.PositionData = testValue;

            Assert.IsTrue(interaction.Changed);

            // Make sure the second time we query it's false
            Assert.IsFalse(interaction.Changed);

            interaction.PositionData = testValue;

            // Make sure if we set the same value it's false
            Assert.IsFalse(interaction.Changed);
        }

        #endregion Vector3

        #region Quaternion

        [Test]
        public void Test11_TestQuaternionChanged()
        {
            var interaction = new MixedRealityInteractionMapping(1, string.Empty, AxisType.ThreeDofRotation, DeviceInputType.None, MixedRealityInputAction.None);
            var testValue1 = Quaternion.Euler(45f, 45f, 45f);
            var testValue2 = Quaternion.identity;

            var initialValue = interaction.RotationData;

            Assert.IsTrue(initialValue == Quaternion.identity);
            Assert.IsFalse(interaction.Changed);

            interaction.RotationData = testValue1;

            Assert.IsTrue(interaction.Changed);

            var setValue1 = interaction.RotationData;

            Assert.True(setValue1 == testValue1);
            Assert.IsFalse(interaction.Changed);

            interaction.RotationData = testValue2;

            Assert.IsTrue(interaction.Changed);

            var setValue2 = interaction.RotationData;

            Assert.True(setValue2 == testValue2);
            Assert.IsFalse(interaction.Changed);
        }

        [Test]
        public void Test12_TestQuaternionNoChange()
        {
            var interaction = new MixedRealityInteractionMapping(1, string.Empty, AxisType.ThreeDofRotation, DeviceInputType.None, MixedRealityInputAction.None);
            var testValue = Quaternion.Euler(45f, 45f, 45f);

            var initialValue = interaction.RotationData;

            Assert.IsTrue(initialValue == Quaternion.identity);
            Assert.IsFalse(interaction.Changed);

            interaction.RotationData = testValue;

            Assert.IsTrue(interaction.Changed);

            // Make sure the second time we query it's false
            Assert.IsFalse(interaction.Changed);

            interaction.RotationData = testValue;

            // Make sure if we set the same value it's false
            Assert.IsFalse(interaction.Changed);
        }

        #endregion Quaternion

        #region MixedRealityPose

        [Test]
        public void Test13_TestMixedRealityPoseChanged()
        {
            var interaction = new MixedRealityInteractionMapping(1, string.Empty, AxisType.SixDof, DeviceInputType.None, MixedRealityInputAction.None);
            var testValue1 = new MixedRealityPose(Vector3.up, Quaternion.identity);
            var testValue2 = new MixedRealityPose(Vector3.one, new Quaternion(45f, 45f, 45f, 45f));

            var initialValue = interaction.PoseData;

            Assert.IsTrue(initialValue == MixedRealityPose.ZeroIdentity);
            Assert.IsFalse(interaction.Changed);

            interaction.PoseData = testValue1;

            Assert.IsTrue(interaction.Changed);

            var setValue1 = interaction.PoseData;

            Assert.IsTrue(setValue1 == testValue1);
            Assert.IsFalse(interaction.Changed);

            interaction.PoseData = testValue2;

            Assert.IsTrue(interaction.Changed);

            var setValue2 = interaction.PoseData;

            Assert.IsTrue(setValue2 == testValue2);
            Assert.IsFalse(interaction.Changed);
        }

        [Test]
        public void Test14_TesMixedRealityPoseNoChange()
        {
            var interaction = new MixedRealityInteractionMapping(1, string.Empty, AxisType.SixDof, DeviceInputType.None, MixedRealityInputAction.None);
            var testValue = new MixedRealityPose(Vector3.up, Quaternion.identity);

            var initialValue = interaction.PoseData;

            Assert.IsTrue(initialValue == MixedRealityPose.ZeroIdentity);
            Assert.IsFalse(interaction.Changed);

            interaction.PoseData = testValue;

            Assert.IsTrue(interaction.Changed);

            // Make sure the second time we query it's false
            Assert.IsFalse(interaction.Changed);

            interaction.PoseData = testValue;

            // Make sure if we set the same value it's false
            Assert.IsFalse(interaction.Changed);
        }

        #endregion MixedRealityPose

        #region Interaction Array Tests

        [Test]
        public void Test15_InteractionArrayObject()
        {
            var interactions = new[]
            {
                new MixedRealityInteractionMapping(1, string.Empty, AxisType.Raw, DeviceInputType.None, MixedRealityInputAction.None)
            };

            var testValue1 = (object)1f;
            var testValue2 = (object)false;

            var initialValue = interactions[0];

            Assert.IsNotNull(initialValue);
            Assert.IsNull(initialValue.RawData);
            Assert.IsFalse(initialValue.Changed);

            initialValue.RawData = testValue1;

            Assert.IsTrue(initialValue.Changed);

            var setValue1 = interactions[0];

            Assert.IsNotNull(setValue1);
            Assert.IsNotNull(setValue1.RawData);
            Assert.AreEqual(setValue1.RawData, testValue1);
            Assert.IsFalse(setValue1.Changed);

            setValue1.RawData = testValue2;

            Assert.IsTrue(setValue1.Changed);

            var setValue2 = interactions[0];

            Assert.IsNotNull(setValue2);
            Assert.IsNotNull(setValue2.RawData);
            Assert.AreEqual(setValue2.RawData, testValue2);
            Assert.IsFalse(setValue2.Changed);
        }

        [Test]
        public void Test16_InteractionArrayBool()
        {
            var interactions = new[]
            {
                new MixedRealityInteractionMapping(1, string.Empty, AxisType.Digital, DeviceInputType.None, MixedRealityInputAction.None)
            };

            var testValue1 = true;
            var testValue2 = false;

            var initialValue = interactions[0];

            Assert.IsNotNull(initialValue);
            Assert.IsFalse(initialValue.BoolData);
            Assert.IsFalse(initialValue.Changed);

            initialValue.BoolData = testValue1;

            Assert.IsTrue(initialValue.Changed);

            var setValue1 = interactions[0];

            Assert.IsNotNull(setValue1);
            Assert.IsTrue(setValue1.BoolData);
            Assert.IsTrue(setValue1.BoolData == testValue1);
            Assert.IsFalse(setValue1.Changed);

            setValue1.BoolData = testValue2;

            Assert.IsTrue(setValue1.Changed);

            var setValue2 = interactions[0];

            Assert.IsNotNull(setValue2);
            Assert.IsFalse(setValue2.BoolData);
            Assert.IsTrue(setValue2.BoolData == testValue2);
            Assert.IsFalse(setValue2.Changed);
        }

        [Test]
        public void Test17_InteractionArrayFloat()
        {
            var interactions = new[]
            {
                new MixedRealityInteractionMapping(1, string.Empty, AxisType.SingleAxis, DeviceInputType.None, MixedRealityInputAction.None)
            };

            var testValue1 = 1f;
            var testValue2 = 9001f;

            var initialValue = interactions[0];

            Assert.IsNotNull(initialValue);
            Assert.AreEqual(initialValue.FloatData, 0d, double.Epsilon);
            Assert.IsFalse(initialValue.Changed);

            initialValue.FloatData = testValue1;

            Assert.IsTrue(initialValue.Changed);

            var setValue1 = interactions[0];

            Assert.IsNotNull(setValue1);
            Assert.AreEqual(setValue1.FloatData, testValue1, double.Epsilon);
            Assert.IsFalse(setValue1.Changed);

            setValue1.FloatData = testValue2;

            Assert.IsTrue(setValue1.Changed);

            var setValue2 = interactions[0];

            Assert.IsNotNull(setValue2);
            Assert.AreEqual(setValue2.FloatData, testValue2, double.Epsilon);
            Assert.IsFalse(setValue2.Changed);
        }

        [Test]
        public void Test18_InteractionArrayVector2()
        {
            var interactions = new[]
            {
                new MixedRealityInteractionMapping(1, string.Empty, AxisType.DualAxis, DeviceInputType.None, MixedRealityInputAction.None)
            };

            var testValue1 = Vector2.one;
            var testValue2 = Vector2.zero;

            var initialValue = interactions[0];

            Assert.IsNotNull(initialValue);
            Assert.True(initialValue.Vector2Data == Vector2.zero);
            Assert.IsFalse(initialValue.Changed);

            initialValue.Vector2Data = testValue1;

            Assert.IsTrue(initialValue.Changed);

            var setValue1 = interactions[0];

            Assert.IsNotNull(setValue1);
            Assert.True(setValue1.Vector2Data == testValue1);
            Assert.IsFalse(setValue1.Changed);

            setValue1.Vector2Data = testValue2;

            Assert.IsTrue(setValue1.Changed);

            var setValue2 = interactions[0];

            Assert.IsNotNull(setValue2);
            Assert.True(setValue2.Vector2Data == testValue2);
            Assert.IsFalse(setValue2.Changed);
        }

        [Test]
        public void Test19_InteractionArrayVector3()
        {
            var interactions = new[]
            {
                new MixedRealityInteractionMapping(1, string.Empty, AxisType.ThreeDofPosition, DeviceInputType.None, MixedRealityInputAction.None)
            };

            var testValue1 = Vector3.one;
            var testValue2 = Vector3.zero;

            var initialValue = interactions[0];

            Assert.IsNotNull(initialValue);
            Assert.True(initialValue.PositionData == Vector3.zero);
            Assert.IsFalse(initialValue.Changed);

            initialValue.PositionData = testValue1;

            Assert.IsTrue(initialValue.Changed);

            var setValue1 = interactions[0];

            Assert.IsNotNull(setValue1);
            Assert.True(setValue1.PositionData == testValue1);
            Assert.IsFalse(setValue1.Changed);

            setValue1.PositionData = testValue2;

            Assert.IsTrue(setValue1.Changed);

            var setValue2 = interactions[0];

            Assert.IsNotNull(setValue2);
            Assert.True(setValue2.PositionData == testValue2);
            Assert.IsFalse(setValue2.Changed);
        }

        [Test]
        public void Test20_InteractionArrayQuaternion()
        {
            var interactions = new[]
            {
                new MixedRealityInteractionMapping(1, string.Empty, AxisType.ThreeDofRotation, DeviceInputType.None, MixedRealityInputAction.None)
            };

            var testValue1 = Quaternion.Euler(45f, 45f, 45f);
            var testValue2 = Quaternion.identity;

            var initialValue = interactions[0];

            Assert.IsNotNull(initialValue);
            Assert.True(initialValue.RotationData.eulerAngles == Quaternion.identity.eulerAngles);
            Assert.IsFalse(initialValue.Changed);

            initialValue.RotationData = testValue1;

            Assert.IsTrue(initialValue.Changed);

            var setValue1 = interactions[0];

            Assert.IsNotNull(setValue1);
            Assert.True(setValue1.RotationData == testValue1);
            Assert.IsFalse(setValue1.Changed);

            setValue1.RotationData = testValue2;

            Assert.IsTrue(setValue1.Changed);

            var setValue2 = interactions[0];

            Assert.IsNotNull(setValue2);
            Assert.True(setValue2.RotationData == testValue2);
            Assert.IsFalse(setValue2.Changed);
        }

        [Test]
        public void Test21_InteractionArrayMixedRealityPose()
        {
            var interactions = new[]
            {
                new MixedRealityInteractionMapping(1, string.Empty, AxisType.SixDof, DeviceInputType.None, MixedRealityInputAction.None)
            };

            var testValue1 = new MixedRealityPose(Vector3.one, new Quaternion(45f, 45f, 45f, 45f));
            var testValue2 = MixedRealityPose.ZeroIdentity;

            var initialValue = interactions[0];

            Assert.IsNotNull(initialValue);
            MixedRealityPose initialSixDofValue = initialValue.PoseData;

            Assert.IsTrue(initialSixDofValue.Position == Vector3.zero);
            Assert.IsTrue(initialSixDofValue == MixedRealityPose.ZeroIdentity);
            Assert.IsFalse(initialValue.Changed);

            initialValue.PoseData = testValue1;

            Assert.IsTrue(initialValue.Changed);

            var setValue1 = interactions[0];

            Assert.IsNotNull(setValue1);
            Assert.IsTrue(setValue1.PoseData == testValue1);
            Assert.IsTrue(setValue1.PoseData.Position == testValue1.Position);
            Assert.IsTrue(setValue1.PoseData.Rotation == testValue1.Rotation);
            Assert.IsFalse(setValue1.Changed);

            setValue1.PoseData = testValue2;

            Assert.IsTrue(setValue1.Changed);

            var setValue2 = interactions[0];

            Assert.IsNotNull(setValue2);
            Assert.IsTrue(setValue2.PoseData == testValue2);
            Assert.IsTrue(setValue2.PoseData.Position == testValue2.Position);
            Assert.IsTrue(setValue2.PoseData.Rotation == testValue2.Rotation);
            Assert.IsFalse(setValue1.Changed);
        }
        #endregion Interaction Array Tests

    }
}