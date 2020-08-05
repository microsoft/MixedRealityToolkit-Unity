// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Tests.EditMode.Extensions
{
    public class CameraExtensionTests
    {
        private static Camera testCamera = null;
        private static Camera testCamera2 = null;
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

        private class TestCollider
        {
            public bool ShouldBeInFOVCamera1, ShouldBeInFOVCamera2;
            public Vector3 Position { get; private set; }
            public Vector3 Bounds { get; private set; }
            public Collider Collider { get; private set; }
            public TestCollider(Vector3 point, Vector3 bounds, bool isInFOVCamera1, bool isInFOVCamera2)
            {
                ShouldBeInFOVCamera1 = isInFOVCamera1;
                ShouldBeInFOVCamera2 = isInFOVCamera2;
                Position = point;
                Bounds = bounds;
                GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                obj.transform.position = Position;
                obj.transform.localScale = bounds;
                Collider = obj.GetComponent<BoxCollider>();
            }
        }

        private static List<TestPoint> TestPoints = new List<TestPoint>();
        private List<TestCollider> TestColliders = new List<TestCollider>();

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

            var obj2 = new GameObject("TestCamera2");
            testCamera2 = obj2.AddComponent<Camera>();
            testCamera2.nearClipPlane = 0.3f;
            testCamera2.farClipPlane = 1000.0f;
            testCamera2.fieldOfView = 60.0f;
            testCamera2.orthographic = false;
            testCamera2.transform.SetPositionAndRotation(Vector3.zero, Quaternion.Euler(0f, 180f, 0f));

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

            Vector3 smallCubeSize = Vector3.one * 0.01f;
            Vector3 largeCubeSize = Vector3.one;
            Vector3 zeroCubeSize = Vector3.zero;
            TestColliders = new List<TestCollider>()
            {
                new TestCollider(-Vector3.forward, smallCubeSize, false, true),
                new TestCollider(-Vector3.forward - Vector3.right, largeCubeSize, false, true),
                new TestCollider(Vector3.forward, smallCubeSize, true, false),
                new TestCollider(Vector3.forward, smallCubeSize, true, false),
                new TestCollider(Vector3.zero, Vector3.zero, false, false),
                new TestCollider(Vector3.zero, largeCubeSize, true, true),
                new TestCollider(new Vector3(0.0f, 0.0f, testCamera.nearClipPlane), smallCubeSize, true, false),
                new TestCollider(new Vector3(0.0f, 0.0f, testCamera.farClipPlane), smallCubeSize, true, false),
                new TestCollider(new Vector3(0.0f, 0.0f, -testCamera.nearClipPlane), smallCubeSize, false, true),
                new TestCollider(new Vector3(0.0f, 0.0f, -testCamera.farClipPlane), smallCubeSize, false, true),
                new TestCollider(2.0f * Vector3.right, smallCubeSize, false, false),
                new TestCollider(2.0f * Vector3.up, smallCubeSize, false, false),
            };

        }

        [TearDown]
        public void TearDown()
        {
            GameObjectExtensions.DestroyGameObject(testCamera.gameObject);
        }


        /// <summary>
        /// Test that the Camera extension method IsInFOVConeCached returns valid results for colliders whose bounds are renderable to the camera
        /// </summary>
        [Test]
        public void TestIsInFOVCached()
        {
            for (int i = 0; i < TestColliders.Count; i++)
            {
                var test = TestColliders[i];
                Assert.AreEqual(test.ShouldBeInFOVCamera1, testCamera.IsInFOVCached(test.Collider), $"TestCollider[{i}] did not match");
            }
        }

        /// <summary>
        /// Test that extension method IsInFOVConeCached gives expected results when called from multiple cameras
        /// facing different directions.
        /// </summary>
        [Test]
        public void TestIsInFOVCachedSecondCamera()
        {
            for (int i = 0; i < TestColliders.Count; i++)
            {
                var test = TestColliders[i];
                Assert.AreEqual(test.ShouldBeInFOVCamera1, testCamera.IsInFOVCached(test.Collider), $"TestCollider[{i}] did not match");
                Assert.AreEqual(test.ShouldBeInFOVCamera2, testCamera2.IsInFOVCached(test.Collider), $"TestColliderSecondCamera[{i}] did not match");
            }
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
            /// (fSz.x,fSz.y,far) is right on the upper right corner midway up the frustum. Numerical precision can
            /// put it either in or out of frustum. Subtract a small epsilon to bring it inside.
            float eps = 1.0e-5f;
            Assert.IsTrue(testCamera.IsInFOV(new Vector3(frustrumSize.x / 2.0f - eps, frustrumSize.y / 2.0f - eps, far / 2.0f + eps)));
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
