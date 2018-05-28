// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
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

            var initialValue = inputDef.GetRaw();

            Assert.IsNull(initialValue);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue(testValue1);

            Assert.IsTrue(inputDef.Changed);

            var setValue1 = inputDef.GetRaw();

            Assert.IsNotNull(setValue1);
            Assert.AreEqual(setValue1, testValue1);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue(testValue2);

            Assert.IsTrue(inputDef.Changed);

            var setValue2 = inputDef.GetRaw();

            Assert.IsNotNull(setValue2);
            Assert.AreEqual(setValue2, testValue2);
            Assert.IsFalse(inputDef.Changed);
        }

        [Test]
        public void Test02_TestObjectNoChange()
        {
            var inputDef = new InteractionMapping(1, AxisType.Raw, DeviceInputType.None, new InputAction(1, "None"));
            var testValue = new object();

            var initialValue = inputDef.GetRaw();

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

        [Test]
        public void Test03_TestObjectGenericChanged()
        {
            var inputDef = new InteractionMapping(1, AxisType.Raw, DeviceInputType.None, new InputAction(1, "None"));
            var testValue1 = (object)1f;
            var testValue2 = (object)false;

            var initialValue = inputDef.GetValue<object>();

            Assert.IsNull(initialValue);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue<object>(testValue1);

            Assert.IsTrue(inputDef.Changed);

            var setValue1 = inputDef.GetValue<object>();

            Assert.IsNotNull(setValue1);
            Assert.AreEqual(setValue1, testValue1);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue<object>(testValue2);

            Assert.IsTrue(inputDef.Changed);

            var setValue2 = inputDef.GetValue<object>();

            Assert.IsNotNull(setValue2);
            Assert.AreEqual(setValue2, testValue2);
            Assert.IsFalse(inputDef.Changed);
        }

        [Test]
        public void Test04_TestObjectGenericNoChange()
        {
            var inputDef = new InteractionMapping(1, AxisType.Raw, DeviceInputType.None, new InputAction(1, "None"));
            var testValue = (object)1f;

            var initialValue = inputDef.GetValue<object>();

            Assert.IsNull(initialValue);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue<object>(testValue);

            Assert.IsTrue(inputDef.Changed);
            // Make sure the second time we query it's false
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue<object>(testValue);

            Assert.IsFalse(inputDef.Changed);
            // Make sure if we set the same value it's false
            Assert.IsFalse(inputDef.Changed);
        }

        [Test]
        public void Test05_TestObjectDirectVsGenericSpeed()
        {
            var inputDef = new InteractionMapping(1, AxisType.Raw, DeviceInputType.None, new InputAction(1, "None"));
            var testValue1 = (object)1f;
            var testValue2 = (object)false;

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int i = 0; i < SpeedTestIterations; i++)
            {
                var testValue = i % 2 == 0 ? testValue1 : testValue2;
                inputDef.SetValue<object>(testValue);
                inputDef.GetValue<object>();
            }

            var genericTime = stopwatch.ElapsedMilliseconds;

            stopwatch.Restart();

            for (int i = 0; i < SpeedTestIterations; i++)
            {
                var testValue = i % 2 == 0 ? testValue1 : testValue2;
                inputDef.SetValue(testValue);
                inputDef.GetRaw();
            }

            var directTime = stopwatch.ElapsedMilliseconds;

            stopwatch.Stop();

            UnityEngine.Debug.Log($"Object Speed Test Results | Generic Time: {genericTime} | Direct Time: {directTime}");
            Assert.Greater(genericTime, directTime);
        }

        #endregion objects

        #region bools

        [Test]
        public void Test06_TestBoolChanged()
        {
            var inputDef = new InteractionMapping(1, AxisType.Digital, DeviceInputType.None, new InputAction(1, "None"));
            var testValue1 = true;
            var testValue2 = false;

            var initialValue = inputDef.GetBool();

            Assert.IsFalse(initialValue);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue(testValue1);

            Assert.IsTrue(inputDef.Changed);

            var setValue1 = inputDef.GetBool();

            Assert.IsTrue(setValue1);
            Assert.True(setValue1 == testValue1);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue(testValue2);

            Assert.IsTrue(inputDef.Changed);

            var setValue2 = inputDef.GetBool();

            Assert.IsFalse(setValue2);
            Assert.True(setValue2 == testValue2);
            Assert.IsFalse(inputDef.Changed);
        }

        [Test]
        public void Test07_TestBoolNoChange()
        {
            var inputDef = new InteractionMapping(1, AxisType.Digital, DeviceInputType.None, new InputAction(1, "None"));
            var testValue = true;

            var initialValue = inputDef.GetBool();

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

        [Test]
        public void Test08_TestBoolGenericChanged()
        {
            var inputDef = new InteractionMapping(1, AxisType.Digital, DeviceInputType.None, new InputAction(1, "None"));
            var testValue1 = true;
            var testValue2 = false;

            var initialValue = inputDef.GetValue<bool>();

            Assert.IsFalse(initialValue);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue<bool>(testValue1);

            Assert.IsTrue(inputDef.Changed);

            var setValue1 = inputDef.GetValue<bool>();

            Assert.IsTrue(setValue1);
            Assert.True(setValue1 == testValue1);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue<bool>(testValue2);

            Assert.IsTrue(inputDef.Changed);

            var setValue2 = inputDef.GetValue<bool>();

            Assert.IsFalse(setValue2);
            Assert.True(setValue2 == testValue2);
            Assert.IsFalse(inputDef.Changed);
        }

        [Test]
        public void Test09_TestBoolGenericNoChange()
        {
            var inputDef = new InteractionMapping(1, AxisType.Digital, DeviceInputType.None, new InputAction(1, "None"));
            var testValue = true;

            var initialValue = inputDef.GetValue<bool>();

            Assert.IsFalse(initialValue);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue<bool>(testValue);

            Assert.IsTrue(inputDef.Changed);
            // Make sure the second time we query it's false
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue<bool>(testValue);

            Assert.IsFalse(inputDef.Changed);
            // Make sure if we set the same value it's false
            Assert.IsFalse(inputDef.Changed);
        }

        [Test]
        public void Test10_TestBoolDirectVsGenericSpeed()
        {
            var inputDef = new InteractionMapping(1, AxisType.Digital, DeviceInputType.None, new InputAction(1, "None"));
            var testValue1 = true;
            var testValue2 = false;

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int i = 0; i < SpeedTestIterations; i++)
            {
                var testValue = i % 2 == 0 ? testValue1 : testValue2;
                inputDef.SetValue<bool>(testValue);
                inputDef.GetValue<bool>();
            }

            var genericTime = stopwatch.ElapsedMilliseconds;

            stopwatch.Restart();

            for (int i = 0; i < SpeedTestIterations; i++)
            {
                var testValue = i % 2 == 0 ? testValue1 : testValue2;
                inputDef.SetValue(testValue);
                inputDef.GetBool();
            }

            var directTime = stopwatch.ElapsedMilliseconds;

            stopwatch.Stop();

            UnityEngine.Debug.Log($"Bool Speed Test Results | Generic Time: {genericTime} | Direct Time: {directTime}");
            Assert.Greater(genericTime, directTime);
        }

        #endregion bools

        #region float

        [Test]
        public void Test11_TestFloatChanged()
        {
            var inputDef = new InteractionMapping(1, AxisType.SingleAxis, DeviceInputType.None, new InputAction(1, "None"));
            var testValue1 = 1f;
            var testValue2 = 9001f;

            var initialValue = inputDef.GetFloat();

            Assert.AreEqual(initialValue, 0d, double.Epsilon);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue(testValue1);

            Assert.IsTrue(inputDef.Changed);

            var setValue1 = inputDef.GetFloat();

            Assert.AreEqual(setValue1, testValue1, double.Epsilon);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue(testValue2);

            Assert.IsTrue(inputDef.Changed);

            var setValue2 = inputDef.GetFloat();

            Assert.AreEqual(setValue2, testValue2, double.Epsilon);
            Assert.IsFalse(inputDef.Changed);
        }

        [Test]
        public void Test12_TestFloatNoChange()
        {
            var inputDef = new InteractionMapping(1, AxisType.SingleAxis, DeviceInputType.None, new InputAction(1, "None"));
            var testValue = 1f;

            var initialValue = inputDef.GetFloat();

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

        [Test]
        public void Test13_TestFloatGenericChanged()
        {
            var inputDef = new InteractionMapping(1, AxisType.SingleAxis, DeviceInputType.None, new InputAction(1, "None"));
            var testValue1 = 1f;
            var testValue2 = 9001f;

            var initialValue = inputDef.GetValue<float>();

            Assert.AreEqual(initialValue, 0d, double.Epsilon);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue<float>(testValue1);

            Assert.IsTrue(inputDef.Changed);

            var setValue1 = inputDef.GetValue<float>();

            Assert.AreEqual(setValue1, testValue1, double.Epsilon);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue<float>(testValue2);

            Assert.IsTrue(inputDef.Changed);

            var setValue2 = inputDef.GetValue<float>();

            Assert.AreEqual(setValue2, testValue2, double.Epsilon);
            Assert.IsFalse(inputDef.Changed);
        }

        [Test]
        public void Test14_TestFloatGenericNoChange()
        {
            var inputDef = new InteractionMapping(1, AxisType.SingleAxis, DeviceInputType.None, new InputAction(1, "None"));
            var testValue = 1f;

            var initialValue = inputDef.GetValue<float>();

            Assert.AreEqual(initialValue, 0d, double.Epsilon);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue<float>(testValue);

            Assert.IsTrue(inputDef.Changed);
            // Make sure the second time we query it's false
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue<float>(testValue);

            Assert.IsFalse(inputDef.Changed);
            // Make sure if we set the same value it's false
            Assert.IsFalse(inputDef.Changed);
        }

        [Test]
        public void Test15_TestFloatDirectVsGenericSpeed()
        {
            var inputDef = new InteractionMapping(1, AxisType.SingleAxis, DeviceInputType.None, new InputAction(1, "None"));
            var testValue1 = 1f;
            var testValue2 = 9001f;

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int i = 0; i < SpeedTestIterations; i++)
            {
                var testValue = i % 2 == 0 ? testValue1 : testValue2;
                inputDef.SetValue<float>(testValue);
                inputDef.GetValue<float>();
            }

            var genericTime = stopwatch.ElapsedMilliseconds;

            stopwatch.Restart();

            for (int i = 0; i < SpeedTestIterations; i++)
            {
                var testValue = i % 2 == 0 ? testValue1 : testValue2;
                inputDef.SetValue(testValue);
                inputDef.GetFloat();
            }

            var directTime = stopwatch.ElapsedMilliseconds;

            stopwatch.Stop();

            UnityEngine.Debug.Log($"Float Speed Test Results | Generic Time: {genericTime} | Direct Time: {directTime}");
            Assert.Greater(genericTime, directTime);
        }

        #endregion float

        #region Vector2

        [Test]
        public void Test16_TestVector2Changed()
        {
            var inputDef = new InteractionMapping(1, AxisType.DualAxis, DeviceInputType.None, new InputAction(1, "None"));
            var testValue1 = Vector2.one;
            var testValue2 = Vector2.zero;

            var initialValue = inputDef.GetVector2();

            Assert.True(initialValue == Vector2.zero);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue(testValue1);

            Assert.IsTrue(inputDef.Changed);

            var setValue1 = inputDef.GetVector2();

            Assert.True(setValue1 == testValue1);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue(testValue2);

            Assert.IsTrue(inputDef.Changed);

            var setValue2 = inputDef.GetVector2();

            Assert.True(setValue2 == testValue2);
            Assert.IsFalse(inputDef.Changed);
        }

        [Test]
        public void Test17_TestVector2NoChange()
        {
            var inputDef = new InteractionMapping(1, AxisType.DualAxis, DeviceInputType.None, new InputAction(1, "None"));
            var testValue = Vector2.one;

            var initialValue = inputDef.GetVector2();

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

        [Test]
        public void Test18_TestVector2GenericChanged()
        {
            var inputDef = new InteractionMapping(1, AxisType.DualAxis, DeviceInputType.None, new InputAction(1, "None"));
            var testValue1 = Vector2.one;
            var testValue2 = Vector2.zero;

            var initialValue = inputDef.GetValue<Vector2>();

            Assert.True(initialValue == Vector2.zero);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue<Vector2>(testValue1);

            Assert.IsTrue(inputDef.Changed);

            var setValue1 = inputDef.GetValue<Vector2>();

            Assert.True(setValue1 == testValue1);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue<Vector2>(testValue2);

            Assert.IsTrue(inputDef.Changed);

            var setValue2 = inputDef.GetValue<Vector2>();

            Assert.True(setValue2 == testValue2);
            Assert.IsFalse(inputDef.Changed);
        }

        [Test]
        public void Test19_TestVector2GenericNoChange()
        {
            var inputDef = new InteractionMapping(1, AxisType.DualAxis, DeviceInputType.None, new InputAction(1, "None"));
            var testValue = Vector2.one;

            var initialValue = inputDef.GetValue<Vector2>();

            Assert.True(initialValue == Vector2.zero);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue<Vector2>(testValue);

            Assert.IsTrue(inputDef.Changed);
            // Make sure the second time we query it's false
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue<Vector2>(testValue);

            Assert.IsFalse(inputDef.Changed);
            // Make sure if we set the same value it's false
            Assert.IsFalse(inputDef.Changed);
        }

        [Test]
        public void Test20_TestVector2DirectVsGenericSpeed()
        {
            var inputDef = new InteractionMapping(1, AxisType.DualAxis, DeviceInputType.None, new InputAction(1, "None"));
            var testValue1 = Vector2.one;
            var testValue2 = Vector2.zero;

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int i = 0; i < SpeedTestIterations; i++)
            {
                var testValue = i % 2 == 0 ? testValue1 : testValue2;
                inputDef.SetValue<Vector2>(testValue);
                inputDef.GetValue<Vector2>();
            }

            var genericTime = stopwatch.ElapsedMilliseconds;

            stopwatch.Restart();

            for (int i = 0; i < SpeedTestIterations; i++)
            {
                var testValue = i % 2 == 0 ? testValue1 : testValue2;
                inputDef.SetValue(testValue);
                inputDef.GetVector2();
            }

            var directTime = stopwatch.ElapsedMilliseconds;

            stopwatch.Stop();

            UnityEngine.Debug.Log($"Vector2 Speed Test Results | Generic Time: {genericTime} | Direct Time: {directTime}");
            Assert.Greater(genericTime, directTime);
        }

        #endregion Vector2

        #region Vector3

        [Test]
        public void Test21_TestVector3Changed()
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
        public void Test22_TestVector3NoChange()
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

        [Test]
        public void Test23_TestVector3GenericChanged()
        {
            var inputDef = new InteractionMapping(1, AxisType.ThreeDoFPosition, DeviceInputType.None, new InputAction(1, "None"));
            var testValue1 = Vector3.one;
            var testValue2 = Vector3.zero;

            var initialValue = inputDef.GetValue<Vector3>();

            Assert.True(initialValue == Vector3.zero);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue<Vector3>(testValue1);

            Assert.IsTrue(inputDef.Changed);

            var setValue1 = inputDef.GetValue<Vector3>();

            Assert.True(setValue1 == testValue1);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue<Vector3>(testValue2);

            Assert.IsTrue(inputDef.Changed);

            var setValue2 = inputDef.GetValue<Vector3>();

            Assert.True(setValue2 == testValue2);
            Assert.IsFalse(inputDef.Changed);
        }

        [Test]
        public void Test24_TestVector3GenericNoChange()
        {
            var inputDef = new InteractionMapping(1, AxisType.ThreeDoFPosition, DeviceInputType.None, new InputAction(1, "None"));
            var testValue = Vector3.one;

            var initialValue = inputDef.GetValue<Vector3>();

            Assert.True(initialValue == Vector3.zero);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue<Vector3>(testValue);

            Assert.IsTrue(inputDef.Changed);
            // Make sure the second time we query it's false
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue<Vector3>(testValue);

            Assert.IsFalse(inputDef.Changed);
            // Make sure if we set the same value it's false
            Assert.IsFalse(inputDef.Changed);
        }

        [Test]
        public void Test25_TestVector3DirectVsGenericSpeed()
        {
            var inputDef = new InteractionMapping(1, AxisType.ThreeDoFPosition, DeviceInputType.None, new InputAction(1, "None"));
            var testValue1 = Vector3.one;
            var testValue2 = Vector3.zero;

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int i = 0; i < SpeedTestIterations; i++)
            {
                var testValue = i % 2 == 0 ? testValue1 : testValue2;
                inputDef.SetValue<Vector3>(testValue);
                inputDef.GetValue<Vector3>();
            }

            var genericTime = stopwatch.ElapsedMilliseconds;

            stopwatch.Restart();

            for (int i = 0; i < SpeedTestIterations; i++)
            {
                var testValue = i % 2 == 0 ? testValue1 : testValue2;
                inputDef.SetValue(testValue);
                inputDef.GetPosition();
            }

            var directTime = stopwatch.ElapsedMilliseconds;

            stopwatch.Stop();

            UnityEngine.Debug.Log($"Vector3 Speed Test Results | Generic Time: {genericTime} | Direct Time: {directTime}");
            Assert.Greater(genericTime, directTime);
        }

        #endregion Vector3

        #region Quaternion

        [Test]
        public void Test26_TestQuaternionChanged()
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
        public void Test27_TestQuaternionNoChange()
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

        [Test]
        public void Test28_TestQuaternionGenericChanged()
        {
            var inputDef = new InteractionMapping(1, AxisType.ThreeDoFRotation, DeviceInputType.None, new InputAction(1, "None"));
            var testValue1 = Quaternion.Euler(45f, 45f, 45f);
            var testValue2 = Quaternion.identity;

            var initialValue = inputDef.GetValue<Quaternion>();

            Assert.True(initialValue == Quaternion.identity);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue<Quaternion>(testValue1);

            Assert.IsTrue(inputDef.Changed);

            var setValue1 = inputDef.GetValue<Quaternion>();

            Assert.True(setValue1 == testValue1);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue<Quaternion>(testValue2);

            Assert.IsTrue(inputDef.Changed);

            var setValue2 = inputDef.GetValue<Quaternion>();

            Assert.True(setValue2 == testValue2);
            Assert.IsFalse(inputDef.Changed);
        }

        [Test]
        public void Test29_TestQuaternionGenericNoChange()
        {
            var inputDef = new InteractionMapping(1, AxisType.ThreeDoFRotation, DeviceInputType.None, new InputAction(1, "None"));
            var testValue = Quaternion.Euler(45f, 45f, 45f);

            var initialValue = inputDef.GetValue<Quaternion>();

            Assert.True(initialValue == Quaternion.identity);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue<Quaternion>(testValue);

            Assert.IsTrue(inputDef.Changed);
            // Make sure the second time we query it's false
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue<Quaternion>(testValue);

            Assert.IsFalse(inputDef.Changed);
            // Make sure if we set the same value it's false
            Assert.IsFalse(inputDef.Changed);
        }

        [Test]
        public void Test30_TestQuaternionDirectVsGenericSpeed()
        {
            var inputDef = new InteractionMapping(1, AxisType.ThreeDoFRotation, DeviceInputType.None, new InputAction(1, "None"));
            var testValue1 = Quaternion.Euler(45f, 45f, 45f);
            var testValue2 = Quaternion.identity;

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int i = 0; i < SpeedTestIterations; i++)
            {
                var testValue = i % 2 == 0 ? testValue1 : testValue2;
                inputDef.SetValue<Quaternion>(testValue);
                inputDef.GetValue<Quaternion>();
            }

            var genericTime = stopwatch.ElapsedMilliseconds;

            stopwatch.Restart();

            for (int i = 0; i < SpeedTestIterations; i++)
            {
                var testValue = i % 2 == 0 ? testValue1 : testValue2;
                inputDef.SetValue(testValue);
                inputDef.GetRotation();
            }

            var directTime = stopwatch.ElapsedMilliseconds;

            stopwatch.Stop();

            UnityEngine.Debug.Log($"Quaternion Speed Test Results | Generic Time: {genericTime} | Direct Time: {directTime}");
            Assert.Greater(genericTime, directTime);
        }

        #endregion Quaternion

        #region Tuples

        [Test]
        public void Test31_TestTupleChanged()
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
        public void Test32_TestTupleNoChange()
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

        [Test]
        public void Test33_TestTupleGenericChanged()
        {
            var inputDef = new InteractionMapping(1, AxisType.SixDoF, DeviceInputType.None, new InputAction(1, "None"));
            var testValue1 = new Tuple<Vector3, Quaternion>(Vector3.zero, Quaternion.identity);
            var testValue2 = new Tuple<Vector3, Quaternion>(Vector3.one, new Quaternion(45f, 45f, 45f, 45f));

            var initialValue = inputDef.GetTransform();

            Assert.IsNull(initialValue);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue<Tuple<Vector3, Quaternion>>(testValue1);

            Assert.IsTrue(inputDef.Changed);

            var setValue1 = inputDef.GetValue<Tuple<Vector3, Quaternion>>();

            Assert.AreEqual(setValue1, testValue1);
            Assert.AreEqual(setValue1.Item1, testValue1.Item1);
            Assert.AreEqual(setValue1.Item2, testValue1.Item2);
            Assert.AreEqual(setValue1.Item2, testValue1.Item2);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue<Tuple<Vector3, Quaternion>>(testValue2);

            Assert.IsTrue(inputDef.Changed);

            var setValue2 = inputDef.GetValue<Tuple<Vector3, Quaternion>>();

            Assert.AreEqual(setValue2, testValue2);
            Assert.AreEqual(setValue2.Item1, testValue2.Item1);
            Assert.AreEqual(setValue2.Item2, testValue2.Item2);
            Assert.AreEqual(setValue2.Item2, testValue2.Item2);
            Assert.IsFalse(inputDef.Changed);
        }

        [Test]
        public void Test34_TestTupleGenericNoChange()
        {
            var inputDef = new InteractionMapping(1, AxisType.SixDoF, DeviceInputType.None, new InputAction(1, "None"));
            var testValue = new Tuple<Vector3, Quaternion>(Vector3.zero, Quaternion.identity);

            var initialValue = inputDef.GetTransform();

            Assert.IsNull(initialValue);
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue<Tuple<Vector3, Quaternion>>(testValue);

            Assert.IsTrue(inputDef.Changed);
            // Make sure the second time we query it's false
            Assert.IsFalse(inputDef.Changed);

            inputDef.SetValue<Tuple<Vector3, Quaternion>>(testValue);

            Assert.IsFalse(inputDef.Changed);
            // Make sure if we set the same value it's false
            Assert.IsFalse(inputDef.Changed);
        }

        [Test]
        public void Test35_TestTupleSpeed()
        {
            var inputDef = new InteractionMapping(1, AxisType.SixDoF, DeviceInputType.None, new InputAction(1, "None"));
            var testValue1 = new Tuple<Vector3, Quaternion>(Vector3.zero, Quaternion.identity);
            var testValue2 = new Tuple<Vector3, Quaternion>(Vector3.one, new Quaternion(45f, 45f, 45f, 45f));

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int i = 0; i < SpeedTestIterations; i++)
            {
                var testValue = i % 2 == 0 ? testValue1 : testValue2;
                inputDef.SetValue<Tuple<Vector3, Quaternion>>(testValue);
                inputDef.GetValue<Tuple<Vector3, Quaternion>>();
            }

            var genericTime = stopwatch.ElapsedMilliseconds;

            stopwatch.Restart();

            for (int i = 0; i < SpeedTestIterations; i++)
            {
                var testValue = i % 2 == 0 ? testValue1 : testValue2;
                inputDef.SetValue(testValue);
                inputDef.GetTransform();
            }

            var directTime = stopwatch.ElapsedMilliseconds;

            stopwatch.Stop();

            UnityEngine.Debug.Log($"Tuple Speed Test Results | Generic Time: {genericTime} | Direct Time: {directTime}");
            Assert.Greater(genericTime, directTime);
        }

        #endregion Tuples
    }
}