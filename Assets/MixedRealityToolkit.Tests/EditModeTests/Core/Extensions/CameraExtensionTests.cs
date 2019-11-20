// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using NUnit.Framework;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Tests.Extensions
{
    public class CameraExtensionTests
    {
        private static Camera testCamera = null;
        private const float MarginTolerance = 0.005f;

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
            Assert.IsFalse(testCamera.IsInFOV(-Vector3.forward));
            Assert.IsFalse(testCamera.IsInFOV(-Vector3.forward - Vector3.right));
            Assert.IsTrue(testCamera.IsInFOV(Vector3.forward));
            Assert.IsFalse(testCamera.IsInFOV(Vector3.zero));

            Assert.IsTrue(testCamera.IsInFOV(new Vector3(0.0f, 0.0f, testCamera.nearClipPlane)));
            Assert.IsFalse(testCamera.IsInFOV(new Vector3(0.0f, 0.0f, testCamera.nearClipPlane - MarginTolerance)));

            float far = testCamera.farClipPlane;
            Assert.IsTrue(testCamera.IsInFOV(new Vector3(0.0f, 0.0f, far)));
            Assert.IsFalse(testCamera.IsInFOV(new Vector3(0.0f, 0.0f, far + MarginTolerance)));

            var frustrumSize = testCamera.GetFrustumSizeForDistance(far/2.0f);
            Assert.IsTrue(testCamera.IsInFOV(new Vector3(frustrumSize.x / 2.0f, frustrumSize.y / 2.0f, far/2.0f)));

            Assert.IsFalse(testCamera.IsInFOV(2.0f * Vector3.right));
            Assert.IsFalse(testCamera.IsInFOV(2.0f * Vector3.up));
        }
    }
}
