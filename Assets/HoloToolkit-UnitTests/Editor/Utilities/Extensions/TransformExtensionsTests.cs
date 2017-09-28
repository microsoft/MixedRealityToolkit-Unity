// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HoloToolkit.Unity.Tests
{
    public class TransformExtensionsTests
    {
        private GameObject empty;

        [SetUp]
        public void SetupTests()
        {
            TestUtils.ClearScene();
            empty = new GameObject();
        }

        [Test]
        public void IterateNull()
        {
            Assert.Throws(typeof(System.ArgumentNullException), () =>
            {
                TransformExtensions.EnumerateHierarchy(null);
            });
        }

        [Test]
        public void IterateNullIgnore()
        {
            var root = Object.Instantiate(empty);
            Assert.Throws(typeof(System.ArgumentNullException), () =>
            {
                root.transform.EnumerateHierarchy(null);
            });
        }

        [Test]
        public void IterateIgnoreNull()
        {
            Assert.Throws(typeof(System.ArgumentNullException), () =>
            {
                TransformExtensions.EnumerateHierarchy(null, new List<Transform>(0));
            });
        }

        [Test]
        public void IterateOne()
        {
            var root = Object.Instantiate(empty);
            Assert.That(root.transform.EnumerateHierarchy().First(), Is.EqualTo(root.transform));
        }

        [Test]
        public void IterateIgnoreOne()
        {
            var root = Object.Instantiate(empty);
            var ignoreList = new List<Transform> { Object.Instantiate(empty, root.transform).transform };

            foreach (var transform in root.transform.EnumerateHierarchy(ignoreList))
            {
                Assert.That(transform, Is.EqualTo(root.transform));
            }
        }

        [Test]
        public void IterateIgnoreBranch()
        {
            var root = Object.Instantiate(empty);
            var child = Object.Instantiate(empty, root.transform);
            Object.Instantiate(empty, child.transform);
            var ignoreList = new List<Transform> { child.transform };

            foreach (var transform in root.transform.EnumerateHierarchy(ignoreList))
            {
                Assert.That(transform, Is.EqualTo(root.transform));
            }
        }

        [Test]
        public void IterateTwo()
        {
            var root = Object.Instantiate(empty);
            var child = Object.Instantiate(empty, root.transform);
            foreach (var transform in root.transform.EnumerateHierarchy())
            {
                Assert.That(transform, Is.EqualTo(root.transform).Or.EqualTo(child.transform));
            }
        }

        [Test]
        public void IterateMultipleSiblings()
        {
            var root = Object.Instantiate(empty);
            for (var i = 0; i < 10; i++)
            {
                Object.Instantiate(empty, root.transform);
            }

            var hierarchyCount = root.transform.EnumerateHierarchy().Count();
            Assert.That(hierarchyCount, Is.EqualTo(root.transform.hierarchyCount));
        }

        [Test]
        public void IterateDeeplyNestedSiblings()
        {
            var root = Object.Instantiate(empty);
            var parent = Object.Instantiate(empty, root.transform);

            for (var i = 0; i < 10; i++)
            {
                parent = Object.Instantiate(empty, parent.transform);
                Object.Instantiate(empty, parent.transform);
            }

            var hierarchyCount = root.transform.EnumerateHierarchy().Count();
            Assert.That(hierarchyCount, Is.EqualTo(root.transform.hierarchyCount));
        }

        [Test]
        public void IterateIgnoreDeeplyNestedBranch()
        {
            var unignoredHierarchyCount = 3;
            var root = Object.Instantiate(empty);
            var unignoredBranchRoot = Object.Instantiate(empty, root.transform);
            var unignoredBranchParent = Object.Instantiate(empty, unignoredBranchRoot.transform);

            var ignoredBranchRoot = Object.Instantiate(empty, root.transform);
            var ignoredBranchParent = Object.Instantiate(empty, ignoredBranchRoot.transform);
            var ignoreList = new List<Transform> { ignoredBranchRoot.transform };


            for (var i = 0; i < 10; i++)
            {
                unignoredHierarchyCount++;
                unignoredBranchParent = Object.Instantiate(empty, unignoredBranchParent.transform);
                ignoredBranchParent = Object.Instantiate(empty, ignoredBranchParent.transform);
            }

            var hierarchyCount = root.transform.EnumerateHierarchy(ignoreList).Count();
            Assert.That(hierarchyCount, Is.EqualTo(unignoredHierarchyCount));
        }

        [Test]
        public void IterateIgnoreDeeplyNestedSiblings()
        {
            var root = Object.Instantiate(empty);
            var parent = Object.Instantiate(empty, root.transform);
            var ignoreList = new List<Transform>();

            for (var i = 0; i < 10; i++)
            {
                parent = Object.Instantiate(empty, parent.transform);

                //ignore every other sibling
                if (i % 2 == 0)
                {
                    ignoreList.Add(Object.Instantiate(empty, parent.transform).transform);
                }
                else
                {
                    Object.Instantiate(empty, parent.transform);
                }
            }

            var hierarchyCount = root.transform.EnumerateHierarchy(ignoreList).Count();
            Assert.That(hierarchyCount, Is.EqualTo(root.transform.hierarchyCount - ignoreList.Count));
        }

        [Test]
        public void IterateDeleteDuring()
        {
            var root = Object.Instantiate(empty);
            var parent = Object.Instantiate(empty, root.transform);
            Object.Instantiate(empty, parent.transform);

            Assert.DoesNotThrow(() =>
            {

                foreach (var transform in root.transform.EnumerateHierarchy())
                {
                    if (transform == parent.transform)
                    {
                        Object.DestroyImmediate(transform.gameObject);
                    }
                }
            });
        }

        [Test]
        public void IterateDeleteChildDuring()
        {
            var root = Object.Instantiate(empty);
            var parent = Object.Instantiate(empty, root.transform);
            var child = Object.Instantiate(empty, parent.transform);

            Assert.DoesNotThrow(() =>
            {
                foreach (var transform in root.transform.EnumerateHierarchy())
                {
                    if (transform == parent.transform)
                    {
                        Object.DestroyImmediate(child.gameObject);
                    }
                }
            });
        }

        [TestCase("wah1234", "", "wah", "1234")]
        [TestCase("-Foo", "", "-", "Foo")]
        [TestCase("Foo", "", "", "Foo")]
        [TestCase("/-Foo", "", "/-", "Foo")]
        [TestCase("/Foo..Bar", "..", "/", "Foo", "Bar")]
        [TestCase("/Foo...Bar...Baz", "...", "/", "Foo", "Bar", "Baz")]
        [TestCase("/Foo-Bar", "-", "/", "Foo", "Bar")]
        [TestCase("-Foo/Bar", "/", "-", "Foo", "Bar")]
        [TestCase("/Foo.Bar.?__.ä.124", ".", "/", "Foo", "Bar", "?__", "ä", "124")]
        public void GetFullPathTests(string result, string delimiter, string prefix, params string[] names)
        {
            if (names.Length == 0) { Assert.IsFalse(false, "Invalid test case"); }
            var parent = Object.Instantiate(empty);
            parent.name = names[0];
            for (var i = 1; i < names.Length; i++)
            {
                parent = Object.Instantiate(empty, parent.transform);
                parent.name = names[i];
            }

            Assert.That(parent.transform.GetFullPath(prefix: prefix, delimiter: delimiter), Is.EqualTo(result));
        }

        [Test]
        public void BoundsColliderEmpty()
        {
            var sut = new GameObject().transform.GetColliderBounds();
            Assert.That(sut, Is.EqualTo(new Bounds(Vector3.zero, Vector3.zero)));
        }

        [Test]
        public void BoundsColliderSimple()
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

            var sut = cube.transform.GetColliderBounds();
            Assert.That(sut, Is.EqualTo(new Bounds(Vector3.zero, Vector3.one)));
        }

        [Test]
        public void BoundsColliderChild()
        {
            var parent = new GameObject();
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.SetParent(parent.transform);

            var sut = parent.transform.GetColliderBounds();
            Assert.That(sut, Is.EqualTo(new Bounds(Vector3.zero, Vector3.one)));
        }

        [Test]
        public void BoundsColliderGrandChild()
        {
            var parent = new GameObject();
            var child = Object.Instantiate(empty, parent.transform);
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.SetParent(child.transform);

            var sut = parent.transform.GetColliderBounds();
            Assert.That(sut, Is.EqualTo(new Bounds(Vector3.zero, Vector3.one)));
        }

        [Test]
        public void BoundsColliderTwoChildren()
        {
            var parent = new GameObject();
            var cube1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube1.transform.SetParent(parent.transform);
            cube1.transform.localPosition = Vector3.one * 0.5f;
            var cube2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube2.transform.SetParent(parent.transform);
            cube2.transform.localPosition = Vector3.one * -0.5f;

            var sut = parent.transform.GetColliderBounds();
            Assert.That(sut, Is.EqualTo(new Bounds(Vector3.zero, Vector3.one * 2)));
        }

        [Test]
        public void BoundsColliderExcludeOrigin()
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.localPosition = Vector3.one * 2;

            var sut = cube.transform.GetColliderBounds();
            Assert.That(sut, Is.EqualTo(new Bounds(Vector3.one * 2, Vector3.one)));
        }
    }
}
