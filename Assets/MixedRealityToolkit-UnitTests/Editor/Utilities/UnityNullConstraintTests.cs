// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using NUnit.Framework;
using NUnit.Framework.Constraints;
using UnityEngine;

namespace HoloToolkit.Unity.Tests
{
    public class UnityNullConstraintTests
    {
        [Test]
        public void TestObjectUnityNullConstraint()
        {
            var obj = CreateDestroyedObject();
            var constraint = new UnityNullConstraint();
            Assert.That(constraint.ApplyTo(obj).IsSuccess, Is.True);
        }

        [Test]
        public void TestObjectUsingUnityIsDirect()
        {
            var gameObject = new GameObject();
            Object.DestroyImmediate(gameObject);
            Assert.That(gameObject, Is.UnityNull);
        }

        [Test]
        public void TestNotUnityNull()
        {
            Assert.That(new GameObject(), Is.Not.UnityNull());
        }

        [Test]
        public void TestActualNull()
        {
            Assert.That(null, Is.UnityNull);
        }

        [Test]
        public void TestNullCheckFailWithNormalObjects()
        {
            Assert.That(new object(), Is.Not.UnityNull());
        }

        [Test]
        public void TestChainedCompare()
        {
            var obj = CreateDestroyedObject();
            Assert.That(obj, Is.UnityNull.And.Not.Null);
        }

        [Test]
        public void TestCompareAgainstNormalNullConstraint()
        {
            var obj = CreateDestroyedObject();
            var result1 = new UnityNullConstraint().ApplyTo(obj).IsSuccess;
            var result2 = new NullConstraint().ApplyTo(obj).IsSuccess;
            Assert.That(result1, Is.Not.EqualTo(result2));
        }

        [Test]
        public void TestTransformUnityNullConstraint()
        {
            var transform = new GameObject().transform;
            Object.DestroyImmediate(transform.gameObject);
            Assert.That(transform, Is.UnityNull);
        }

        private static GameObject CreateDestroyedObject()
        {
            var gameObject = new GameObject();
            Object.DestroyImmediate(gameObject);
            return gameObject;
        }
    }
}
