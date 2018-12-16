// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Core.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using NUnit.Framework;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Tests.InputSystem
{
    public class TestFixture_02_InteractionDefinitionTests
    {
        #region objects

        public MixedRealityInteractionMapping InitializeRawInteractionMapping()
        {
            return new MixedRealityInteractionMapping(1, string.Empty, AxisType.Raw, DeviceInputType.None, MixedRealityInputAction.None);
        }

        /// <summary>
        /// We test by initializing a new <see cref="MixedRealityInteractionMapping"/>.
        /// We expect that <see cref="MixedRealityInteractionMapping.Changed"/> == false.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.Updated"/> == false.<para/>
        /// </summary>
        [Test]
        public void Test_01_01_InitializedRawData()
        {
            var interaction = InitializeRawInteractionMapping();

            var initialValue = interaction.RawData;

            // Test to make sure the initial values are correct.
            Assert.IsNull(initialValue);
            Assert.IsFalse(interaction.Changed);
            Assert.IsFalse(interaction.Updated);

            interaction.RawData = initialValue;

            // Test to make sure that setting the same initial
            // value doesn't raise changed or updated.
            Assert.IsNull(interaction.RawData);
            Assert.IsFalse(interaction.Changed);
            Assert.IsFalse(interaction.Updated);
        }

        /// <summary>
        /// We test by setting the interaction data to two different values.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.Changed"/> == true, then false after each subsequent check before assigning a new value.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.Updated"/> == true, then false after each subsequent check before assigning a new value.<para/>
        /// </summary>
        [Test]
        public void Test_01_02_ObjectChangedAndUpdated()
        {
            var interaction = InitializeRawInteractionMapping();
            var initialValue = interaction.RawData;
            var testValue1 = (object)1f;
            var testValue2 = (object)false;

            interaction.RawData = testValue1;

            // Make sure the first query after value assignment is true
            Assert.IsTrue(interaction.Changed);
            Assert.IsTrue(interaction.Updated);

            var setValue1 = interaction.RawData;

            // Check the values
            Assert.IsNotNull(setValue1);
            Assert.AreEqual(setValue1, testValue1);
            // Make sure the second time we query it's false
            Assert.IsFalse(interaction.Changed);
            Assert.IsFalse(interaction.Updated);

            interaction.RawData = testValue2;

            // Make sure the first query after value assignment is true
            Assert.IsTrue(interaction.Changed);
            Assert.IsTrue(interaction.Updated);

            var setValue2 = interaction.RawData;

            // Check the values
            Assert.IsNotNull(setValue2);
            Assert.AreEqual(setValue2, testValue2);
            // Make sure the second time we query it's false
            Assert.IsFalse(interaction.Changed);
            Assert.IsFalse(interaction.Updated);

            interaction.RawData = initialValue;

            // Make sure the first query after value assignment is true
            Assert.IsTrue(interaction.Changed);
            Assert.IsTrue(interaction.Updated);

            var setValue3 = interaction.RawData;

            // Check the values
            Assert.IsNull(interaction.RawData);
            Assert.AreEqual(initialValue, setValue3);
            // Make sure the second time we query it's false
            Assert.IsFalse(interaction.Changed);
            Assert.IsFalse(interaction.Updated);
        }

        /// <summary>
        /// We test by setting the interaction data to the same object multiple times.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.Changed"/> == false.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.Updated"/> == true.<para/>
        /// </summary>
        [Test]
        public void Test_01_03_ObjectNoChangeAndUpdated()
        {
            var interaction = InitializeRawInteractionMapping();
            var testValue = new object();

            interaction.RawData = testValue;

            // Make sure the first query after value assignment is true
            Assert.IsTrue(interaction.Changed);
            Assert.IsTrue(interaction.Updated);

            var setValue1 = interaction.RawData;

            // Check the values
            Assert.IsNotNull(setValue1);
            Assert.AreEqual(testValue, setValue1);
            // Make sure the second time we query it's false
            Assert.IsFalse(interaction.Changed);
            Assert.IsFalse(interaction.Updated);

            interaction.RawData = testValue;

            // Make sure if we set the same value changed is false
            Assert.IsFalse(interaction.Changed);

            // Make sure if we set the same value updated is true
            Assert.IsTrue(interaction.Updated);

            var setValue2 = interaction.RawData;

            // Check the values
            Assert.IsNotNull(setValue2);
            Assert.AreEqual(testValue, setValue2);
            // Make sure if we set the same value it's false
            Assert.IsFalse(interaction.Changed);
            Assert.IsFalse(interaction.Updated);
        }

        #endregion objects

        #region bools

        public MixedRealityInteractionMapping InitializeBoolInteractionMapping()
        {
            return new MixedRealityInteractionMapping(1, string.Empty, AxisType.Digital, DeviceInputType.None, MixedRealityInputAction.None);
        }

        /// <summary>
        /// We test by initializing a new <see cref="MixedRealityInteractionMapping"/>.
        /// We expect that <see cref="MixedRealityInteractionMapping.Changed"/> == false.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.Updated"/> == false.<para/>
        /// </summary>
        [Test]
        public void Test_02_01_InitializedBoolData()
        {
            var interaction = InitializeBoolInteractionMapping();
            var initialValue = interaction.BoolData;

            // Test to make sure the initial values are correct.
            Assert.IsFalse(initialValue);
            Assert.IsFalse(interaction.Changed);
            Assert.IsFalse(interaction.Updated);

            interaction.BoolData = initialValue;

            // Test to make sure that setting the same initial
            // value doesn't raise changed or updated.
            Assert.IsFalse(initialValue);
            Assert.IsFalse(interaction.Changed);
            Assert.IsFalse(interaction.Updated);
        }

        /// <summary>
        /// We test by setting the interaction data to two different values.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.Changed"/> == true, then false after each subsequent check before assigning a new value.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.Updated"/> == true, then false after each subsequent check before assigning a new value.<para/>
        /// </summary>
        [Test]
        public void Test_02_02_BoolChangedAndUpdated()
        {
            var interaction = InitializeBoolInteractionMapping();
            const bool testValue1 = true;
            const bool testValue2 = false;

            interaction.BoolData = testValue1;

            // Make sure the first query after value assignment is true
            Assert.IsTrue(interaction.Changed);
            Assert.IsTrue(interaction.Updated);

            var setValue1 = interaction.BoolData;

            // Check the values
            Assert.IsTrue(setValue1);
            Assert.True(setValue1 == testValue1);
            // Make sure the second time we query it's false
            Assert.IsFalse(interaction.Changed);
            Assert.IsFalse(interaction.Updated);

            interaction.BoolData = testValue2;

            // Make sure the first query after value assignment is true
            Assert.IsTrue(interaction.Changed);
            Assert.IsTrue(interaction.Updated);

            var setValue2 = interaction.BoolData;

            // Check the values
            Assert.IsFalse(setValue2);
            Assert.True(setValue2 == testValue2);
            // Make sure the second time we query it's false
            Assert.IsFalse(interaction.Changed);
            Assert.IsFalse(interaction.Updated);
        }

        /// <summary>
        /// We test by setting the interaction data to the same object multiple times.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.Changed"/> == false.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.Updated"/> == true.<para/>
        /// </summary>
        [Test]
        public void Test_02_03_BoolNoChangeAndUpdated()
        {
            var interaction = InitializeBoolInteractionMapping();
            const bool testValue = true;

            interaction.BoolData = testValue;

            // Make sure if we set the value it's true
            Assert.IsTrue(interaction.Changed);
            Assert.IsTrue(interaction.Updated);

            var setValue1 = interaction.BoolData;

            // Check the values
            Assert.IsTrue(testValue);
            Assert.True(setValue1 == testValue);
            // Make sure the second time we query it's false
            Assert.IsFalse(interaction.Changed);
            Assert.IsFalse(interaction.Updated);

            interaction.BoolData = testValue;

            // Make sure if we set the same value it's false
            Assert.IsFalse(interaction.Changed);

            // Make sure if we set the same value it's true
            Assert.IsTrue(interaction.Updated);

            var setValue2 = interaction.BoolData;

            // Check the values
            Assert.IsTrue(testValue);
            Assert.True(setValue2 == testValue);
            // Make sure the second time we query it's false
            Assert.IsFalse(interaction.Changed);
            Assert.IsFalse(interaction.Updated);
        }

        #endregion bools

        #region float

        public MixedRealityInteractionMapping InitializeFloatInteractionMapping()
        {
            return new MixedRealityInteractionMapping(1, string.Empty, AxisType.SingleAxis, DeviceInputType.None, MixedRealityInputAction.None);
        }

        /// <summary>
        /// We test by initializing a new <see cref="MixedRealityInteractionMapping"/>.
        /// We expect that <see cref="MixedRealityInteractionMapping.Changed"/> == false.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.Updated"/> == false.<para/>
        /// </summary>
        [Test]
        public void Test_03_01_InitializedFloatData()
        {
            var interaction = InitializeFloatInteractionMapping();
            var initialValue = interaction.FloatData;

            // Test to make sure the initial values are correct.
            Assert.AreEqual(0d, initialValue, double.Epsilon);
            Assert.IsFalse(interaction.Changed);
            Assert.IsFalse(interaction.Updated);

            interaction.FloatData = initialValue;

            // Test to make sure that setting the same initial
            // value doesn't raise changed or updated.
            Assert.AreEqual(0d, initialValue, double.Epsilon);
            Assert.IsFalse(interaction.Changed);
            Assert.IsFalse(interaction.Updated);
        }

        /// <summary>
        /// We test by setting the interaction data to two different values.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.Changed"/> == true, then false after each subsequent check before assigning a new value.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.Updated"/> == true, then false after each subsequent check before assigning a new value.<para/>
        /// </summary>
        [Test]
        public void Test_03_02_FloatChangedAndUpdated()
        {
            var interaction = InitializeFloatInteractionMapping();
            var initialValue = interaction.FloatData;
            const float testValue1 = 1f;
            const float testValue2 = 9001f;

            interaction.FloatData = testValue1;

            // Make sure the first query after value assignment is true
            Assert.IsTrue(interaction.Changed);
            Assert.IsTrue(interaction.Updated);

            var setValue1 = interaction.FloatData;

            // Check the values
            Assert.AreEqual(testValue1, setValue1, double.Epsilon);
            // Make sure the second time we query it's false
            Assert.IsFalse(interaction.Changed);
            Assert.IsFalse(interaction.Updated);

            interaction.FloatData = testValue2;

            // Make sure the first query after value assignment is true
            Assert.IsTrue(interaction.Changed);
            Assert.IsTrue(interaction.Updated);

            var setValue2 = interaction.FloatData;

            // Check the values
            Assert.AreEqual(testValue2, setValue2, double.Epsilon);
            // Make sure the second time we query it's false
            Assert.IsFalse(interaction.Changed);
            Assert.IsFalse(interaction.Updated);

            interaction.FloatData = initialValue;

            // Make sure the first query after value assignment is true
            Assert.IsTrue(interaction.Changed);
            Assert.IsTrue(interaction.Updated);

            var setValue3 = interaction.FloatData;

            // Check the values
            Assert.AreEqual(initialValue, setValue3, double.Epsilon);
            // Make sure the second time we query it's false
            Assert.IsFalse(interaction.Changed);
            Assert.IsFalse(interaction.Updated);
        }

        /// <summary>
        /// We test by setting the interaction data to the same object multiple times.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.Changed"/> == false.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.Updated"/> == true.<para/>
        /// </summary>
        [Test]
        public void Test_03_03_FloatNoChangeAndUpdated()
        {
            var interaction = InitializeFloatInteractionMapping();
            const float testValue = 1f;

            interaction.FloatData = testValue;

            // Make sure if we set the value it's true
            Assert.IsTrue(interaction.Changed);
            Assert.IsTrue(interaction.Updated);

            var setValue1 = interaction.FloatData;

            // Check the values
            Assert.AreEqual(testValue, setValue1, double.Epsilon);
            // Make sure the second time we query it's false
            Assert.IsFalse(interaction.Changed);
            Assert.IsFalse(interaction.Updated);

            interaction.FloatData = testValue;

            // Make sure if we set the same value it's false
            Assert.IsFalse(interaction.Changed);

            // Make sure if we set the same value it's true
            Assert.IsTrue(interaction.Updated);

            var setValue2 = interaction.FloatData;

            // Check the values
            Assert.AreEqual(testValue, setValue2, double.Epsilon);
            // Make sure the second time we query it's false
            Assert.IsFalse(interaction.Changed);
            Assert.IsFalse(interaction.Updated);
        }

        #endregion float

        #region Vector2

        public MixedRealityInteractionMapping InitializeVector2InteractionMapping()
        {
            return new MixedRealityInteractionMapping(1, string.Empty, AxisType.DualAxis, DeviceInputType.None, MixedRealityInputAction.None);
        }

        /// <summary>
        /// We test by initializing a new <see cref="MixedRealityInteractionMapping"/>.
        /// We expect that <see cref="MixedRealityInteractionMapping.Changed"/> == false.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.Updated"/> == false.<para/>
        /// </summary>
        [Test]
        public void Test_04_01_InitializedVector2()
        {
            var interaction = InitializeVector2InteractionMapping();
            var initialValue = interaction.Vector2Data;

            // Test to make sure the initial values are correct.
            Assert.True(initialValue == Vector2.zero);
            Assert.IsFalse(interaction.Changed);
            Assert.IsFalse(interaction.Updated);

            interaction.Vector2Data = initialValue;

            // Test to make sure that setting the same initial
            // value doesn't raise changed or updated.
            Assert.True(initialValue == Vector2.zero);
            Assert.IsFalse(interaction.Changed);
            Assert.IsFalse(interaction.Updated);
        }

        /// <summary>
        /// We test by setting the interaction data to two different values.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.Changed"/> == true, then false after each subsequent check before assigning a new value.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.Updated"/> == true, then false after each subsequent check before assigning a new value.<para/>
        /// </summary>
        [Test]
        public void Test_04_02_Vector2ChangedAndUpdated()
        {
            var interaction = InitializeVector2InteractionMapping();
            var initialValue = interaction.Vector2Data;
            var testValue1 = Vector2.one;
            var testValue2 = Vector2.one * 0.5f;

            interaction.Vector2Data = testValue1;

            // Make sure the first query after value assignment is true
            Assert.IsTrue(interaction.Changed);
            Assert.IsTrue(interaction.Updated);

            var setValue1 = interaction.Vector2Data;

            // Check the values
            Assert.True(setValue1 == testValue1);
            // Make sure the second time we query it's false
            Assert.IsFalse(interaction.Changed);
            Assert.IsFalse(interaction.Changed);

            interaction.Vector2Data = testValue2;

            // Make sure the first query after value assignment is true
            Assert.IsTrue(interaction.Changed);
            Assert.IsTrue(interaction.Updated);

            var setValue2 = interaction.Vector2Data;

            // Check the values
            Assert.True(setValue2 == testValue2);
            // Make sure the second time we query it's false
            Assert.IsFalse(interaction.Changed);
            Assert.IsFalse(interaction.Changed);

            interaction.Vector2Data = initialValue;

            // Make sure the first query after value assignment is true
            Assert.IsTrue(interaction.Changed);
            Assert.IsTrue(interaction.Updated);

            var setValue3 = interaction.Vector2Data;

            // Check the values
            Assert.True(setValue3 == initialValue);
            // Make sure the second time we query it's false
            Assert.IsFalse(interaction.Changed);
            Assert.IsFalse(interaction.Changed);
        }

        /// <summary>
        /// We test by setting the interaction data to the same object multiple times.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.Changed"/> == false.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.Updated"/> == true.<para/>
        /// </summary>
        [Test]
        public void Test_04_03_Vector2NoChangeAndUpdated()
        {
            var interaction = InitializeVector2InteractionMapping();
            var testValue = Vector2.one;

            interaction.Vector2Data = testValue;

            // Make sure if we set the value it's true
            Assert.IsTrue(interaction.Changed);
            Assert.IsTrue(interaction.Updated);

            var setValue1 = interaction.Vector2Data;

            // Check the values
            Assert.True(setValue1 == testValue);
            // Make sure the second time we query it's false
            Assert.IsFalse(interaction.Changed);
            Assert.IsFalse(interaction.Updated);

            interaction.Vector2Data = testValue;

            // Make sure if we set the same value it's false
            Assert.IsFalse(interaction.Changed);

            // Make sure if we set the same value it's true
            Assert.IsTrue(interaction.Updated);

            var setValue2 = interaction.Vector2Data;

            // Check the values
            Assert.True(setValue2 == testValue);
            // Make sure the second time we query it's false
            Assert.IsFalse(interaction.Changed);
            Assert.IsFalse(interaction.Updated);
        }

        #endregion Vector2

        #region Vector3

        public MixedRealityInteractionMapping InitializeVector3InteractionMapping()
        {
            return new MixedRealityInteractionMapping(1, string.Empty, AxisType.ThreeDofPosition, DeviceInputType.None, MixedRealityInputAction.None);
        }

        /// <summary>
        /// We test by setting the interaction data to two different values.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.Changed"/> == true, then false after each subsequent check before assigning a new value.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.Updated"/> == true, then false after each subsequent check before assigning a new value.<para/>
        /// </summary>
        [Test]
        public void Test_05_01_InitializedVector3()
        {
            var interaction = InitializeVector3InteractionMapping();
            var initialValue = interaction.PositionData;

            // Test to make sure the initial values are correct.
            Assert.True(initialValue == Vector3.zero);
            Assert.IsFalse(interaction.Changed);
            Assert.IsFalse(interaction.Updated);

            interaction.PositionData = initialValue;

            // Test to make sure that setting the same initial
            // value doesn't raise changed or updated.
            Assert.True(initialValue == Vector3.zero);
            Assert.IsFalse(interaction.Changed);
            Assert.IsFalse(interaction.Updated);
        }

        /// <summary>
        /// We test by setting the interaction data to the same object multiple times.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.Changed"/> == false.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.Updated"/> == true.<para/>
        /// </summary>
        [Test]
        public void Test_05_02_Vector3ChangedAndUpdated()
        {
            var interaction = InitializeVector3InteractionMapping();
            var initialValue = interaction.PositionData;
            var testValue1 = Vector3.one;
            var testValue2 = Vector3.one * 0.5f;

            interaction.PositionData = testValue1;

            // Make sure the first query after value assignment is true
            Assert.IsTrue(interaction.Changed);
            Assert.IsTrue(interaction.Updated);

            var setValue1 = interaction.PositionData;

            // Check the values
            Assert.True(setValue1 == testValue1);
            // Make sure the second time we query it's false
            Assert.IsFalse(interaction.Changed);
            Assert.IsFalse(interaction.Changed);

            interaction.PositionData = testValue2;

            // Make sure the first query after value assignment is true
            Assert.IsTrue(interaction.Changed);
            Assert.IsTrue(interaction.Updated);

            var setValue2 = interaction.PositionData;

            // Check the values
            Assert.True(setValue2 == testValue2);
            // Make sure the second time we query it's false
            Assert.IsFalse(interaction.Changed);
            Assert.IsFalse(interaction.Changed);

            interaction.PositionData = initialValue;

            // Make sure the first query after value assignment is true
            Assert.IsTrue(interaction.Changed);
            Assert.IsTrue(interaction.Updated);

            var setValue3 = interaction.PositionData;

            // Check the values
            Assert.True(setValue3 == initialValue);
            // Make sure the second time we query it's false
            Assert.IsFalse(interaction.Changed);
            Assert.IsFalse(interaction.Changed);
        }

        /// <summary>
        /// We test by initializing a new <see cref="MixedRealityInteractionMapping"/>.
        /// We expect that <see cref="MixedRealityInteractionMapping.Changed"/> == false.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.Updated"/> == false.<para/>
        /// </summary>
        [Test]
        public void Test_05_03_Vector3NoChangeAndUpdated()
        {
            var interaction = InitializeVector3InteractionMapping();
            var testValue = Vector3.one;

            interaction.PositionData = testValue;

            // Make sure if we set the value it's true
            Assert.IsTrue(interaction.Changed);
            Assert.IsTrue(interaction.Updated);

            var setValue1 = interaction.PositionData;

            // Check the values
            Assert.True(setValue1 == testValue);
            // Make sure the second time we query it's false
            Assert.IsFalse(interaction.Changed);
            Assert.IsFalse(interaction.Updated);

            interaction.PositionData = testValue;

            // Make sure if we set the same value it's false
            Assert.IsFalse(interaction.Changed);

            // Make sure if we set the same value it's true
            Assert.IsTrue(interaction.Updated);

            var setValue2 = interaction.PositionData;

            // Check the values
            Assert.True(setValue2 == testValue);
            // Make sure the second time we query it's false
            Assert.IsFalse(interaction.Changed);
            Assert.IsFalse(interaction.Updated);
        }

        #endregion Vector3

        #region Quaternion

        public MixedRealityInteractionMapping InitializeQuaternionInteractionMapping()
        {
            return new MixedRealityInteractionMapping(1, string.Empty, AxisType.ThreeDofRotation, DeviceInputType.None, MixedRealityInputAction.None);
        }

        /// <summary>
        /// We test by setting the interaction data to two different values.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.Changed"/> == true, then false after each subsequent check before assigning a new value.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.Updated"/> == true, then false after each subsequent check before assigning a new value.<para/>
        /// </summary>
        [Test]
        public void Test_06_01_InitializeQuaternion()
        {
            var interaction = InitializeQuaternionInteractionMapping();
            var initialValue = interaction.RotationData;

            // Test to make sure the initial values are correct.
            Assert.IsTrue(initialValue == Quaternion.identity);
            Assert.IsFalse(interaction.Changed);
            Assert.IsFalse(interaction.Updated);

            interaction.RotationData = initialValue;

            // Test to make sure that setting the same initial
            // value doesn't raise changed or updated.
            Assert.IsTrue(initialValue == Quaternion.identity);
            Assert.IsFalse(interaction.Changed);
            Assert.IsFalse(interaction.Updated);
        }

        /// <summary>
        /// We test by setting the interaction data to the same object multiple times.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.Changed"/> == false.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.Updated"/> == true.<para/>
        /// </summary>
        [Test]
        public void Test_06_02_QuaternionChangedAndUpdated()
        {
            var interaction = InitializeQuaternionInteractionMapping();
            var initialValue = interaction.RotationData;
            var testValue1 = Quaternion.Euler(45f, 45f, 45f);
            var testValue2 = Quaternion.Euler(270f, 270f, 270f);

            interaction.RotationData = testValue1;

            // Make sure the first query after value assignment is true
            Assert.IsTrue(interaction.Changed);
            Assert.IsTrue(interaction.Updated);

            var setValue1 = interaction.RotationData;

            // Check the values
            Assert.True(setValue1 == testValue1);
            // Make sure the second time we query it's false
            Assert.IsFalse(interaction.Changed);
            Assert.IsFalse(interaction.Changed);

            interaction.RotationData = testValue2;

            // Make sure the first query after value assignment is true
            Assert.IsTrue(interaction.Changed);
            Assert.IsTrue(interaction.Updated);

            var setValue2 = interaction.RotationData;

            // Check the values
            Assert.True(setValue2 == testValue2);
            // Make sure the second time we query it's false
            Assert.IsFalse(interaction.Changed);
            Assert.IsFalse(interaction.Changed);

            interaction.RotationData = initialValue;

            // Make sure the first query after value assignment is true
            Assert.IsTrue(interaction.Changed);
            Assert.IsTrue(interaction.Updated);

            var setValue3 = interaction.RotationData;

            // Check the values
            Assert.True(setValue3 == initialValue);
            // Make sure the second time we query it's false
            Assert.IsFalse(interaction.Changed);
            Assert.IsFalse(interaction.Changed);
        }

        /// <summary>
        /// We test by initializing a new <see cref="MixedRealityInteractionMapping"/>.
        /// We expect that <see cref="MixedRealityInteractionMapping.Changed"/> == false.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.Updated"/> == false.<para/>
        /// </summary>
        [Test]
        public void Test_06_03_QuaternionNoChangeAndUpdated()
        {
            var interaction = InitializeQuaternionInteractionMapping();
            var testValue = Quaternion.Euler(45f, 45f, 45f);

            interaction.RotationData = testValue;

            // Make sure if we set the value it's true
            Assert.IsTrue(interaction.Changed);
            Assert.IsTrue(interaction.Updated);

            var setValue1 = interaction.RotationData;

            // Check the values
            Assert.True(setValue1 == testValue);
            // Make sure the second time we query it's false
            Assert.IsFalse(interaction.Changed);
            Assert.IsFalse(interaction.Updated);

            interaction.RotationData = testValue;

            // Make sure if we set the same value it's false
            Assert.IsFalse(interaction.Changed);

            // Make sure if we set the same value it's true
            Assert.IsTrue(interaction.Updated);

            var setValue2 = interaction.RotationData;

            // Check the values
            Assert.True(setValue2 == testValue);
            // Make sure the second time we query it's false
            Assert.IsFalse(interaction.Changed);
            Assert.IsFalse(interaction.Updated);
        }

        #endregion Quaternion

        #region MixedRealityPose

        public MixedRealityInteractionMapping InitializeMixedRealityPoseInteractionMapping()
        {
            return new MixedRealityInteractionMapping(1, string.Empty, AxisType.SixDof, DeviceInputType.None, MixedRealityInputAction.None);
        }

        /// <summary>
        /// We test by setting the interaction data to two different values.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.Changed"/> == true, then false after each subsequent check before assigning a new value.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.Updated"/> == true, then false after each subsequent check before assigning a new value.<para/>
        /// </summary>
        [Test]
        public void Test_07_01_InitializePoseData()
        {
            var interaction = InitializeMixedRealityPoseInteractionMapping();
            var initialValue = interaction.PoseData;

            // Test to make sure the initial values are correct.
            Assert.IsTrue(initialValue == MixedRealityPose.ZeroIdentity);
            Assert.IsFalse(interaction.Changed);
            Assert.IsFalse(interaction.Updated);

            interaction.PoseData = initialValue;

            // Test to make sure that setting the same initial
            // value doesn't raise changed or updated.
            Assert.IsTrue(initialValue == MixedRealityPose.ZeroIdentity);
            Assert.IsFalse(interaction.Changed);
            Assert.IsFalse(interaction.Updated);
        }

        /// <summary>
        /// We test by setting the interaction data to the same object multiple times.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.Changed"/> == false.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.Updated"/> == true.<para/>
        /// </summary>
        [Test]
        public void Test_07_02_MixedRealityPoseChangedAndUpdated()
        {
            var interaction = InitializeMixedRealityPoseInteractionMapping();
            var initialValue = interaction.PoseData;
            var testValue1 = new MixedRealityPose(Vector3.up, Quaternion.identity);
            var testValue2 = new MixedRealityPose(Vector3.one, new Quaternion(45f, 45f, 45f, 45f));

            interaction.PoseData = testValue1;

            // Make sure the first query after value assignment is true
            Assert.IsTrue(interaction.Changed);
            Assert.IsTrue(interaction.Updated);

            var setValue1 = interaction.PoseData;

            // Check the values
            Assert.True(setValue1 == testValue1);
            // Make sure the second time we query it's false
            Assert.IsFalse(interaction.Changed);
            Assert.IsFalse(interaction.Changed);

            interaction.PoseData = testValue2;

            // Make sure the first query after value assignment is true
            Assert.IsTrue(interaction.Changed);
            Assert.IsTrue(interaction.Updated);

            var setValue2 = interaction.PoseData;

            // Check the values
            Assert.True(setValue2 == testValue2);
            // Make sure the second time we query it's false
            Assert.IsFalse(interaction.Changed);
            Assert.IsFalse(interaction.Changed);

            interaction.PoseData = initialValue;

            // Make sure the first query after value assignment is true
            Assert.IsTrue(interaction.Changed);
            Assert.IsTrue(interaction.Updated);

            var setValue3 = interaction.PoseData;

            // Check the values
            Assert.True(setValue3 == initialValue);
            // Make sure the second time we query it's false
            Assert.IsFalse(interaction.Changed);
            Assert.IsFalse(interaction.Changed);
        }

        /// <summary>
        /// We test by initializing a new <see cref="MixedRealityInteractionMapping"/>.
        /// We expect that <see cref="MixedRealityInteractionMapping.Changed"/> == false.<para/>
        /// We expect that <see cref="MixedRealityInteractionMapping.Updated"/> == false.<para/>
        /// </summary>
        [Test]
        public void Test_07_03_MixedRealityPoseNoChangeAndUpdated()
        {
            var interaction = new MixedRealityInteractionMapping(1, string.Empty, AxisType.SixDof, DeviceInputType.None, MixedRealityInputAction.None);
            var testValue = new MixedRealityPose(Vector3.up, Quaternion.identity);

            interaction.PoseData = testValue;

            // Make sure if we set the value it's true
            Assert.IsTrue(interaction.Changed);
            Assert.IsTrue(interaction.Updated);

            var setValue1 = interaction.PoseData;

            // Check the values
            Assert.True(setValue1 == testValue);
            // Make sure the second time we query it's false
            Assert.IsFalse(interaction.Changed);
            Assert.IsFalse(interaction.Updated);

            interaction.PoseData = testValue;

            // Make sure if we set the same value it's false
            Assert.IsFalse(interaction.Changed);

            // Make sure if we set the same value it's true
            Assert.IsTrue(interaction.Updated);

            var setValue2 = interaction.PoseData;

            // Check the values
            Assert.True(setValue2 == testValue);
            // Make sure the second time we query it's false
            Assert.IsFalse(interaction.Changed);
            Assert.IsFalse(interaction.Updated);
        }

        #endregion MixedRealityPose

        #region Interaction Array Tests

        [Test]
        public void Test_08_01_InteractionArrayObject()
        {
            var interactions = new[]
            {
                InitializeRawInteractionMapping()
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
        public void Test_08_02_InteractionArrayBool()
        {
            var interactions = new[]
            {
                InitializeBoolInteractionMapping()
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
        public void Test_08_03_InteractionArrayFloat()
        {
            var interactions = new[]
            {
                InitializeFloatInteractionMapping()
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
        public void Test_08_04_InteractionArrayVector2()
        {
            var interactions = new[]
            {
                InitializeVector2InteractionMapping()
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
        public void Test_08_05_InteractionArrayVector3()
        {
            var interactions = new[]
            {
                InitializeVector3InteractionMapping()
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
        public void Test_08_06_InteractionArrayQuaternion()
        {
            var interactions = new[]
            {
                InitializeQuaternionInteractionMapping()
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
        public void Test_08_07_InteractionArrayMixedRealityPose()
        {
            var interactions = new[]
            {
                InitializeMixedRealityPoseInteractionMapping()
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