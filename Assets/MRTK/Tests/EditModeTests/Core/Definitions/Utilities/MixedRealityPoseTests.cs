// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools.Utils;

namespace Microsoft.MixedReality.Toolkit.Tests.EditMode
{
    public class MixedRealityPoseTests
    {
        /// <summary>
        /// Verifies that the multiplication of MixedRealityPose was successfully performed
        /// </summary>
        [Test]
        public void TestMixedRealityPoseMultiplication()
        {
            var positionA = new Vector3(13, 28, 11);
            var rotationA = Quaternion.Euler(40, 50, 60);
            var poseA = new MixedRealityPose(positionA, rotationA);
            var matrixA = Matrix4x4.TRS(positionA, rotationA, Vector3.one);

            var positionB = new Vector3(23, 51, 2);
            var rotationB = Quaternion.Euler(112, 31, 53);
            var poseB = new MixedRealityPose(positionB, rotationB);
            var matrixB = Matrix4x4.TRS(positionB, rotationB, Vector3.one);

            // Perform multiplication for both MixedRealityPose and Matrix4x4.
            var multipliedPose = poseA * poseB;
            var multipliedMatrix = matrixA * matrixB;

            // Get expected position and rotation from multiplied TRS matrix.
            var expectedPosition = new Vector3(multipliedMatrix.m03, multipliedMatrix.m13, multipliedMatrix.m23);
            var expectedRotation = multipliedMatrix.rotation;

            Assert.That(multipliedPose.Position, Is.EqualTo(expectedPosition).Using(new Vector3EqualityComparer(10e-6f)));
            Assert.That(multipliedPose.Rotation, Is.EqualTo(expectedRotation).Using(new QuaternionEqualityComparer(10e-6f)));
        }
    }
}