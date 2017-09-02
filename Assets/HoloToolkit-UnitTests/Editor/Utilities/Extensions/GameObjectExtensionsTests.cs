// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace HoloToolkit.Unity.Tests
{
    public class GameObjectExtensionsTests
    {
        private GameObject empty;
        private LayerMask waterLayer;
        private LayerMask uiLayer;
        private LayerMask transparentFxLayer;

        /// <summary>
        /// Create empty game object for easy cloning
        /// </summary>
        [SetUp]
        public void SetupTests()
        {
            TestUtils.ClearScene();
            empty = new GameObject();
            waterLayer = LayerMask.NameToLayer("Water");
            uiLayer = LayerMask.NameToLayer("UI");
            transparentFxLayer = LayerMask.NameToLayer("TransparentFX");
        }

        [Test]
        public void SetLayerRecursivelyNull()
        {

            Assert.Throws(typeof(System.ArgumentNullException), () =>
            {
                GameObjectExtensions.SetLayerRecursively(null, waterLayer);
            });
        }

        [Test]
        public void SetLayerRecursivelyCacheNull()
        {
            Assert.Throws(typeof(System.ArgumentNullException), () =>
            {
                Dictionary<GameObject, int> layerCache;
                GameObjectExtensions.SetLayerRecursively(null, waterLayer, out layerCache);
            });
        }

        [Test]
        public void ApplyLayerCacheRecursivelyNull()
        {
            Assert.Throws(typeof(System.ArgumentNullException), () =>
            {
                Dictionary<GameObject, int> layerCache = new Dictionary<GameObject, int>();
                GameObjectExtensions.ApplyLayerCacheRecursively(null, layerCache);
            });
        }

        [Test]
        public void ApplyLayerCacheRecursivelyNullCache()
        {
            var parent = Object.Instantiate(empty);
            Assert.Throws(typeof(System.ArgumentNullException), () =>
            {
                parent.ApplyLayerCacheRecursively(null);
            });
        }

        [Test]
        public void SetLayerRecursivelyOne()
        {
            var parent = Object.Instantiate(empty);
            parent.SetLayerRecursively(waterLayer);

            Assert.That(parent.layer, Is.EqualTo(waterLayer.value));
        }

        [Test]
        public void SetLayerRecursivelyOneChild()
        {
            var parent = Object.Instantiate(empty);
            var child = Object.Instantiate(empty, parent.transform);
            parent.SetLayerRecursively(waterLayer);

            Assert.That(child.layer, Is.EqualTo(waterLayer.value));
        }

        [Test]
        public void SetLayerRecursivelyDeeplyNestedBranch()
        {
            var root = Object.Instantiate(empty);
            var parent = Object.Instantiate(empty, root.transform);
            for (var i = 0; i < 10; i++)
            {
                parent = Object.Instantiate(empty, parent.transform);
            }
            root.SetLayerRecursively(waterLayer);

            foreach (var transform in root.GetComponentsInChildren<Transform>())
            {
                Assert.That(transform.gameObject.layer, Is.EqualTo(waterLayer.value));
            }
        }

        [Test]
        public void SetLayerRecursivelyCacheOne()
        {
            var parent = Object.Instantiate(empty);
            parent.layer = uiLayer;
            Dictionary<GameObject, int> layerCache;
            parent.SetLayerRecursively(waterLayer, out layerCache);

            Assert.That(parent.layer, Is.EqualTo(waterLayer.value), "New parent layer is not correct");
            Assert.That(layerCache[parent], Is.EqualTo(uiLayer.value), "Old parent layer is not correct");
        }

        [Test]
        public void SetLayerRecursivelyCacheOneChild()
        {
            var parent = Object.Instantiate(empty);
            var child = Object.Instantiate(empty, parent.transform);
            parent.layer = uiLayer;
            child.layer = transparentFxLayer;

            Dictionary<GameObject, int> layerCache;
            parent.SetLayerRecursively(waterLayer, out layerCache);

            Assert.That(child.layer, Is.EqualTo(waterLayer.value), "New child layer is not correct");
            Assert.That(layerCache[parent], Is.EqualTo(uiLayer.value), "Old parent layer is not correct");
            Assert.That(layerCache[child], Is.EqualTo(transparentFxLayer.value), "Old child layer is not correct");
        }

        [Test]
        public void SetLayerRecursivelyCacheDeeplyNestedBranch()
        {
            var root = Object.Instantiate(empty);
            var parent = Object.Instantiate(empty, root.transform);

            for (var i = 0; i < 10; i++)
            {
                parent = Object.Instantiate(empty, parent.transform);
            }

            Dictionary<GameObject, int> layerCache;
            root.SetLayerRecursively(waterLayer, out layerCache);

            foreach (var transform in root.GetComponentsInChildren<Transform>())
            {
                Assert.That(transform.gameObject.layer, Is.EqualTo(waterLayer.value), "New layer is not correct.");
                Assert.That(layerCache[transform.gameObject], Is.Not.EqualTo(waterLayer.value), "Old layer is not correct");
            }
        }

        [Test]
        public void ApplyLayerCacheRecursivelyOne()
        {
            var parent = Object.Instantiate(empty);
            parent.layer = uiLayer;

            Dictionary<GameObject, int> layerCache;
            parent.SetLayerRecursively(waterLayer, out layerCache);
            parent.ApplyLayerCacheRecursively(layerCache);

            Assert.That(parent.layer, Is.EqualTo(uiLayer.value));
        }

        [Test]
        public void ApplyLayerCacheRecursivelyOneChild()
        {
            var parent = Object.Instantiate(empty);
            var child = Object.Instantiate(empty, parent.transform);
            parent.layer = uiLayer;
            child.layer = transparentFxLayer;

            Dictionary<GameObject, int> layerCache;
            parent.SetLayerRecursively(waterLayer, out layerCache);
            parent.ApplyLayerCacheRecursively(layerCache);

            Assert.That(parent.layer, Is.EqualTo(uiLayer.value), "Reapplied parent layer is not correct");
            Assert.That(child.layer, Is.EqualTo(transparentFxLayer.value), "Reapplied child layer is not correct");
        }

        [Test]
        public void ApplyLayerCacheRecursivelyNestedBranch()
        {
            var root = Object.Instantiate(empty);
            var parent = Object.Instantiate(empty, root.transform);

            for (var i = 0; i < 10; i++)
            {
                parent = Object.Instantiate(empty, parent.transform);
            }

            Dictionary<GameObject, int> layerCache;
            root.SetLayerRecursively(waterLayer, out layerCache);
            root.ApplyLayerCacheRecursively(layerCache);

            foreach (var transform in root.GetComponentsInChildren<Transform>())
            {
                Assert.That(transform.gameObject.layer, Is.Not.EqualTo(waterLayer.value));
            }
        }

        [TestCase("1234", "1234")]
        [TestCase("Foo", "Foo")]
        [TestCase("Foo/Bar", "Foo", "Bar")]
        [TestCase("Foo/Bar/Baz", "Foo", "Bar", "Baz")]
        [TestCase("Foo/Bar/?__/ä/124", "Foo", "Bar", "?__", "ä", "124")]
        public void GetFullPathTests(string result, params string[] names)
        {
            //TODO: Delete with GameObjectExtensions.GetFullPath
            if (names.Length == 0) { Assert.IsFalse(false, "Invalid test case"); }
            var parent = Object.Instantiate(empty);
            parent.name = names[0];
            for (var i = 1; i < names.Length; i++)
            {
                parent = Object.Instantiate(empty, parent.transform);
                parent.name = names[i];
            }

#pragma warning disable 618
            Assert.That(parent.GetFullPath(), Is.EqualTo(result));
#pragma warning restore 618
        }
    }
}
