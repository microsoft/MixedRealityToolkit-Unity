// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Tests.Extensions
{
    public class CameraExtensionTests
    {
        private static Camera testCamera = null;
        private const float MarginTolerance = 0.005f;

        private class TestPoint
        {
            public bool ShouldBeInFOV;
            public Vector3 Point;
            public TestPoint(Vector3 point, bool isInFOV)
            {
                ShouldBeInFOV = isInFOV;
                Point = point;
            }
        }

        private static List<TestPoint> TestPoints = new List<TestPoint>();

        [SetUp]
        public void SetUp()
        {
            var obj = new GameObject("TestCamera");
            testCamera = obj.AddComponent<Camera>();
            testCamera.nearClipPlane = 0.3f;
            testCamera.farClipPlane = 1000.0f;
            testCamera.fieldOfView = 60.0f;
            testCamera.orthographic = false;
            testCamera.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

            // Create test data with pre-determined points.
            // This data expects the same results for both IsInFOVCone and IsInFOV
            TestPoints = new List<TestPoint>()
            {
                new TestPoint(-Vector3.forward,false),
                new TestPoint(-Vector3.forward - Vector3.right, false),
                new TestPoint(Vector3.forward, true),
                new TestPoint(Vector3.zero, false),

                new TestPoint(new Vector3(0.0f, 0.0f, testCamera.nearClipPlane), true),
                new TestPoint(new Vector3(0.0f, 0.0f, testCamera.nearClipPlane - MarginTolerance), false),

                new TestPoint(new Vector3(0.0f, 0.0f, testCamera.farClipPlane), true),
                new TestPoint(new Vector3(0.0f, 0.0f, testCamera.farClipPlane + MarginTolerance), false),

                new TestPoint(2.0f * Vector3.right, false),
                new TestPoint(2.0f * Vector3.up, false),
            };
        }

        [TearDown]
        public void TearDown()
        {
            GameObjectExtensions.DestroyGameObject(testCamera.gameObject);
        }

        /// <summary>
        /// Test that the Camera extension method IsInFov returns valid points that would be renderable on the camera
        /// </summary>
        [Test]
        public void TestIsInFOV()
        {
            for (int i = 0; i < TestPoints.Count; i++)
            { 
                var test = TestPoints[i];
                Assert.AreEqual(test.ShouldBeInFOV, testCamera.IsInFOV(test.Point), $"TestPoint[{i}] at {test.Point} did not match");
            }

            var far = testCamera.farClipPlane;
            var frustrumSize = testCamera.GetFrustumSizeForDistance(far / 2.0f);
            Assert.IsTrue(testCamera.IsInFOV(new Vector3(frustrumSize.x / 2.0f, frustrumSize.y / 2.0f, far / 2.0f)));
        }

        /// <summary>
        /// Test that the Camera extension method IsInFOVCone returns valid points that would be renderable on the camera
        /// </summary>
        [Test]
        public void TestIsInFOVCone()
        {
            for (int i = 0; i < TestPoints.Count; i++)
            {
                var test = TestPoints[i];
                Assert.AreEqual(test.ShouldBeInFOV, testCamera.IsInFOVCone(test.Point), $"TestPoint[{i}] at {test.Point} did not match");
            }
        }
    }
}
