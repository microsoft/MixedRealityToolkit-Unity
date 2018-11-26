// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using NUnit.Framework;
using UnityEngine;

namespace HoloToolkit.Unity.Tests
{
    public class LayerExtensionsTests
    {
        [Test]
        public void TestIsLayerInLayerMask()
        {
            var waterLayer = LayerMask.NameToLayer("Water");
            var mask = Physics.AllLayers;
            Assert.That(waterLayer.IsInLayerMask(mask), Is.True);
        }

        [Test]
        public void TestIsLayerNotInLayerMask()
        {
            var waterLayer = LayerMask.NameToLayer("Water");
            var mask = LayerMask.GetMask("Default");
            Assert.That(waterLayer.IsInLayerMask(mask), Is.False);
        }

        [Test]
        public void TestFindLayerListIndexFirst()
        {
            var waterLayer = LayerMask.NameToLayer("Water");
            var masks = new LayerMask[]
            {
                LayerMask.GetMask("Water"),
                LayerMask.GetMask("Default"),
                LayerMask.GetMask("UI"),
            };
            Assert.That(waterLayer.FindLayerListIndex(masks), Is.EqualTo(0));
        }

        [Test]
        public void TestFindLayerListIndexLast()
        {
            var waterLayer = LayerMask.NameToLayer("Water");
            var masks = new LayerMask[]
            {
                LayerMask.GetMask("Default"),
                LayerMask.GetMask("UI"),
                LayerMask.GetMask("Water")
            };
            Assert.That(waterLayer.FindLayerListIndex(masks), Is.EqualTo(masks.Length - 1));
        }

        [Test]
        public void TestFindLayerListIndexMiddle()
        {
            var waterLayer = LayerMask.NameToLayer("Water");
            var masks = new LayerMask[]
            {
                LayerMask.GetMask("Default"),
                LayerMask.GetMask("Water"),
                LayerMask.GetMask("UI")
            };
            Assert.That(waterLayer.FindLayerListIndex(masks), Is.EqualTo(1));
        }

        [Test]
        public void TestFindLayerListIndexNone()
        {
            var waterLayer = LayerMask.NameToLayer("Water");
            var masks = new LayerMask[]
            {
                LayerMask.GetMask("Default"),
                LayerMask.GetMask("UI")
            };
            Assert.That(waterLayer.FindLayerListIndex(masks), Is.EqualTo(-1));
        }

        [Test]
        public void TestFindLayerListIndexEmpty()
        {
            var waterLayer = LayerMask.NameToLayer("Water");
            var masks = new LayerMask[] { };
            Assert.That(waterLayer.FindLayerListIndex(masks), Is.EqualTo(-1));
        }

        [Test]
        public void TestCombineLayerMasksMultiple()
        {
            var masks = new LayerMask[]
            {
                LayerMask.GetMask("Ignore Raycast"),
                LayerMask.GetMask("TransparentFX"),
                LayerMask.GetMask("UI")
            };
            var combinedMask = LayerMask.GetMask("Ignore Raycast", "TransparentFX", "UI");
            Assert.That(masks.Combine(), Is.EqualTo(combinedMask));
        }

        [Test]
        public void TestCombineLayerMasksOne()
        {
            var masks = new LayerMask[]
            {
                LayerMask.GetMask("UI")
            };
            var combinedMask = LayerMask.GetMask("UI");
            Assert.That(masks.Combine(), Is.EqualTo(combinedMask));
        }

        [Test]
        public void TestCombineLayerMasksEmpty()
        {
            var masks = new LayerMask[] { };
            var combinedMask = LayerMask.GetMask();
            Assert.That(masks.Combine(), Is.EqualTo(combinedMask));
        }
    }
}
