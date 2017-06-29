// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using NUnit.Framework;
using UnityEngine;

namespace HoloToolkit.Unity.Tests
{
    public class VectorExtensionsTests
    {
        [Test]
        public void Vector2_Mul()
        {
            Vector2 value = new Vector2(2f, 3f);
            Vector2 scale = new Vector2(2f, 3f);
            Vector2 expected = new Vector2(4f, 9f);
            Assert.That(value.Mul(scale), Is.EqualTo(expected));
        }

        [Test]
        public void Vector2_Div()
        {
            Vector2 value = new Vector2(2f, 3f);
            Vector2 scale = new Vector2(2f, 3f);
            Vector2 expected = new Vector2(1f, 1f);
            Assert.That(value.Div(scale), Is.EqualTo(expected));
        }

        [Test]
        public void Vector3_Mul()
        {
            Vector3 value = new Vector3(2f, 3f, 4f);
            Vector3 scale = new Vector3(2f, 3f, 4f);
            Vector3 expected = new Vector3(4f, 9f, 16f);
            Assert.That(value.Mul(scale), Is.EqualTo(expected));
        }

        [Test]
        public void Vector3_Div()
        {
            Vector3 value = new Vector3(2f, 3f, 4f);
            Vector3 scale = new Vector3(2f, 3f, 4f);
            Vector3 expected = new Vector3(1f, 1f, 1f);
            Assert.That(value.Div(scale), Is.EqualTo(expected));
        }

        [Test]
        public void Vector3_RotateAround_Quaternion()
        {
            Vector3 point = new Vector3(0f, 0f, 0f);
            Vector3 pivot = new Vector3(1f, 1f, 1f);
            Quaternion rotation = Quaternion.AngleAxis(180f, new Vector3(0f, 0f, 1f));
            Vector3 expected = new Vector3(2f, 2f, 0f);
            Assert.That(point.RotateAround(pivot, rotation), Is.EqualTo(expected).Within(1f).Ulps);
        }

        [Test]
        public void Vector3_RotateAround_Euler()
        {
            Vector3 point = new Vector3(0f, 0f, 0f);
            Vector3 pivot = new Vector3(1f, 1f, 1f);
            Vector3 rotation = new Vector3(0f, 0f, 180f);
            Vector3 expected = new Vector3(2f, 2f, 0f);
            Assert.That(point.RotateAround(pivot, rotation), Is.EqualTo(expected).Within(1f).Ulps);
        }

        [Test]
        public void Vector3_TransformPoint()
        {
            Vector3 point = new Vector3(1f, 2f, 3f);
            Vector3 scale = new Vector3(2f, 3f, 4f); // scaled point: (2, 6, 12)
            Quaternion rotation = Quaternion.AngleAxis(180f, Vector3.up); // scaled rotated point: (-2, 6, -12)
            Vector3 translation = new Vector3(3f, 4f, 5f); // translated scaled rotated point: (1, 10, -7)
            Vector3 expected = new Vector3(1, 10, -7);
            Vector3 result = point.TransformPoint(translation, rotation, scale);
            Assert.That(Vector3.Distance(result, expected), Is.LessThan(0.00001f));
        }

        [Test]
        public void Vector3_InverseTransformPoint()
        {
            // Perform the same transformation as Vector3_TransformPoint in reverse.
            Vector3 point = new Vector3(1, 10, -7);
            Vector3 scale = new Vector3(2f, 3f, 4f);
            Quaternion rotation = Quaternion.AngleAxis(180f, Vector3.up);
            Vector3 translation = new Vector3(3f, 4f, 5f);
            Vector3 expected = new Vector3(1f, 2f, 3f);
            Vector3 result = point.InverseTransformPoint(translation, rotation, scale);
            Assert.That(Vector3.Distance(result, expected), Is.LessThan(0.00001f));
        }

        [Test]
        public void Vector2Collection_Average_Empty()
        {
            var vectors = new Vector2[] { };
            Assert.That(VectorExtensions.Average(vectors), Is.EqualTo(Vector2.zero));
        }

        [Test]
        public void Vector3Collection_Average_Empty()
        {
            var vectors = new Vector3[] { };
            Assert.That(VectorExtensions.Average(vectors), Is.EqualTo(Vector3.zero));
        }

        [Test]
        public void Vector2Collection_Average()
        {
            var vectors = new Vector2[] { new Vector2(1f, 2f), new Vector2(2f, 3f) };
            Assert.That(VectorExtensions.Average(vectors), Is.EqualTo(new Vector2(1.5f, 2.5f)));
        }

        [Test]
        public void Vector3Collection_Average()
        {
            var vectors = new Vector3[] { new Vector3(1f, 2f, 3f), new Vector3(2f, 3f, 4f) };
            Assert.That(VectorExtensions.Average(vectors), Is.EqualTo(new Vector3(1.5f, 2.5f, 3.5f)));
        }

        [Test]
        public void Vector2Collection_Median_Empty()
        {
            var vectors = new Vector2[] { };
            Assert.That(VectorExtensions.Median(vectors), Is.EqualTo(Vector2.zero));
        }

        [Test]
        public void Vector3Collection_Median_Empty()
        {
            var vectors = new Vector3[] { };
            Assert.That(VectorExtensions.Median(vectors), Is.EqualTo(Vector3.zero));
        }

        [Test]
        public void Vector2Collection_Median()
        {
            var vectors = new Vector2[] { new Vector3(10f, 10f), new Vector2(1f, 1f), new Vector2(5f, 5f) };
            Assert.That(VectorExtensions.Median(vectors), Is.EqualTo(new Vector2(5f, 5f)));
        }

        [Test]
        public void Vector3Collection_Median()
        {
            var vectors = new Vector3[] { new Vector3(10f, 10f, 10f), new Vector3(1f, 1f, 1f), new Vector3(5f, 5f, 5f) };
            Assert.That(VectorExtensions.Median(vectors), Is.EqualTo(new Vector3(5f, 5f, 5f)));
        }

    }
}
