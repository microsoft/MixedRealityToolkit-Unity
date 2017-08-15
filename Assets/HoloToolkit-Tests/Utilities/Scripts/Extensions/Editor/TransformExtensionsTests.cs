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

        /// <summary>
        /// Create empty game object for easy cloning
        /// </summary>
        [SetUp]
        public void SetupTests()
        {
            empty = new GameObject();
        }

        /// <summary>
        /// Delete everything between each test
        /// </summary>
        [TearDown]
        public void ClearScene()
        {
            foreach (var gameObject in Object.FindObjectsOfType<GameObject>())
            {
                //only destroy root objects
                if (gameObject.transform.parent == null)
                {
                    Object.DestroyImmediate(gameObject);
                }
            }
        }

        [Test]
        public void IterateNull()
        {
            Assert.Throws(typeof(System.ArgumentNullException), () => {
                TransformExtensions.IterateHierarchy(null);
            });
        }

        [Test]
        public void IterateNullIgnore()
        {
            var root = Object.Instantiate(empty);
            Assert.Throws(typeof(System.ArgumentNullException), () => {
                root.transform.IterateHierarchy(null);
            });
        }

        [Test]
        public void IterateIgnoreNull()
        {
            Assert.Throws(typeof(System.ArgumentNullException), () => {
                TransformExtensions.IterateHierarchy(null, new List<Transform>(0));
            });
        }

        [Test]
        public void IterateOne()
        {
            var root = Object.Instantiate(empty);
            Assert.That(root.transform.IterateHierarchy().First(), Is.EqualTo(root.transform));
        }

        [Test]
        public void IterateIgnoreOne()
        {
            var root = Object.Instantiate(empty);
            var ignoreList = new List<Transform> { Object.Instantiate(empty, root.transform).transform };

            foreach (var transform in root.transform.IterateHierarchy(ignoreList))
            {
                Assert.That(transform, Is.EqualTo(root.transform));
            }
        }

        [Test]
        public void IterateIgnoreBranch()
        {
            var root = Object.Instantiate(empty);
            var child = Object.Instantiate(empty, root.transform);
            var grandChild = Object.Instantiate(empty, child.transform);
            var ignoreList = new List<Transform> { child.transform };

            foreach (var transform in root.transform.IterateHierarchy(ignoreList))
            {
                Assert.That(transform, Is.EqualTo(root.transform));
            }
        }

        [Test]
        public void IterateTwo()
        {
            var root = Object.Instantiate(empty);
            var child = Object.Instantiate(empty, root.transform);
            foreach (var transform in root.transform.IterateHierarchy())
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

            var hierarchyCount = root.transform.IterateHierarchy().Count();
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

            var hierarchyCount = root.transform.IterateHierarchy().Count();
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

            var hierarchyCount = root.transform.IterateHierarchy(ignoreList).Count();
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
                } else
                {
                    Object.Instantiate(empty, parent.transform);
                }
            }

            var hierarchyCount = root.transform.IterateHierarchy(ignoreList).Count();
            Assert.That(hierarchyCount, Is.EqualTo(root.transform.hierarchyCount - ignoreList.Count));
        }
    }
}
