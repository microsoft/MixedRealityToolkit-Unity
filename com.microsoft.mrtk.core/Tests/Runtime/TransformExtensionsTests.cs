// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Core.Tests
{
    public class TransformExtensionsTests : BaseRuntimeTests
    {
        [UnityTest]
        public IEnumerator GetColliderBoundsTest()
        {
            // Create a root object with colliders attached to it
            GameObject rootObject = new GameObject("Root");
            BoxCollider rootCollider = rootObject.AddComponent<BoxCollider>();
            rootCollider.center = new Vector3(1f, 2f, 3f);
            rootCollider.size = new Vector3(2f, 3f, 4f);

            // Create two child objects with colliders attached to them
            GameObject childObject1 = new GameObject("Child1");
            childObject1.transform.parent = rootObject.transform;
            SphereCollider childCollider1 = childObject1.AddComponent<SphereCollider>();
            childCollider1.center = new Vector3(2f, 3f, 4f);
            childCollider1.radius = 1f;

            GameObject childObject2 = new GameObject("Child2");
            childObject2.transform.parent = rootObject.transform;
            CapsuleCollider childCollider2 = childObject2.AddComponent<CapsuleCollider>();
            childCollider2.center = new Vector3(1f, 1f, 1f);
            childCollider2.radius = 0.5f;
            childCollider2.height = 2f;

            // Get encapsulated bounds
            Bounds bounds = rootObject.transform.GetColliderBounds();

            Assert.AreEqual(new Vector3(1.5f, 2f, 2.75f), bounds.center);
            Assert.AreEqual(new Vector3(3f, 4f, 4.50f), bounds.size);
            yield return null;
        }



        [UnityTest]
        public IEnumerator GetFullPathTest()
        {
            var parent = new GameObject("Parent").transform;
            var child1 = new GameObject("Child1").transform;
            var child2 = new GameObject("Child2").transform;
            var grandchild = new GameObject("Grandchild").transform;
            child1.SetParent(parent);
            child2.SetParent(parent);
            grandchild.SetParent(child1);

            var fullPath = child1.GetFullPath(prefix: "/", delimiter: ".");

            Assert.AreEqual("/Parent.Child1", fullPath);

            fullPath = grandchild.GetFullPath(prefix: "/", delimiter: "/");
            Assert.AreEqual("/Parent/Child1/Grandchild", fullPath);

            fullPath = child2.GetFullPath(prefix: "", delimiter: "-");
            Assert.AreEqual("Parent-Child2", fullPath);

            yield return null;
        }

        [UnityTest]
        public IEnumerator EnumerateHierarchyTest()
        {
            var root = new GameObject("Root").transform;
            var child1 = new GameObject("Child1").transform;
            var child2 = new GameObject("Child2").transform;
            child1.SetParent(root);
            child2.SetParent(root);

            var children = root.EnumerateHierarchy().ToList();

            // 3 because root is also included
            Assert.AreEqual(3, children.Count());
            Assert.Contains(root, children);
            Assert.Contains(child1, children);
            Assert.Contains(child2, children);
            yield return null;
        }

        [UnityTest]
        public IEnumerator EnumerateHierarchyIgnoresTransformTest()
        {
            var root = new GameObject("Root").transform;
            var child1 = new GameObject("Child1").transform;
            var child2 = new GameObject("Child2").transform;
            var ignore = new List<Transform> { child2 };
            child1.SetParent(root);
            child2.SetParent(root);

            var children = root.EnumerateHierarchy(ignore).ToList();

            // 2 because root is also included, but not child2
            Assert.AreEqual(2, children.Count()); 
            Assert.Contains(root, children);
            Assert.Contains(child1, children);
            Assert.IsFalse(children.Contains(child2));
            yield return null;
        }


        [UnityTest]
        public IEnumerator IsParentOrChildOfTest()
        {
            var parent = new GameObject("Parent").transform;
            var child = new GameObject("Child").transform;
            child.SetParent(parent);

            var result = child.IsParentOrChildOf(parent);
            Assert.IsTrue(result);

            result = parent.IsParentOrChildOf(child);
            Assert.IsTrue(result);

            yield return null;
        }

        [UnityTest]
        public IEnumerator FindAncestorComponentNullTest()
        {
            var gameObject = new GameObject();
            var transform = gameObject.transform;

            var component = transform.FindAncestorComponent<BoxCollider>();

            Assert.IsNull(component);
            yield return null;
        }

        [UnityTest]
        public IEnumerator FindAncestorComponentTest()
        {
            var parentGameObject = new GameObject();
            var childGameObject = new GameObject();
            var component = childGameObject.AddComponent<Rigidbody>();
            childGameObject.transform.SetParent(parentGameObject.transform);

            var foundComponent = childGameObject.transform.FindAncestorComponent<Rigidbody>();

            Assert.AreEqual(component, foundComponent);
            yield return null;
        }

        [UnityTest]
        public IEnumerator EnumerateAncestorsTest()
        {
            var grandparentGameObject = new GameObject();
            var parentGameObject = new GameObject();
            var childGameObject = new GameObject();
            childGameObject.transform.SetParent(parentGameObject.transform);
            parentGameObject.transform.SetParent(grandparentGameObject.transform);

            var ancestors = childGameObject.transform.EnumerateAncestors(false).ToList();

            Assert.AreEqual(2, ancestors.Count);
            Assert.AreEqual(parentGameObject.transform, ancestors[0]);
            Assert.AreEqual(grandparentGameObject.transform, ancestors[1]);
            yield return null;
        }

        [UnityTest]
        public IEnumerator TransformSizeTest()
        {
            var parentGameObject = new GameObject();
            parentGameObject.transform.localScale = new Vector3(2, 2, 2);
            var childGameObject = new GameObject();
            childGameObject.transform.SetParent(parentGameObject.transform);
            childGameObject.transform.localScale = new Vector3(2, 2, 2);

            var worldSize = childGameObject.transform.TransformSize(new Vector3(2, 3, 4));

            Assert.AreEqual(new Vector3(8, 12, 16), worldSize);
            yield return null;
        }

        [UnityTest]
        public IEnumerator InverseTransformSizeTest()
        {
            var parentGameObject = new GameObject();
            var childGameObject = new GameObject();
            childGameObject.transform.SetParent(parentGameObject.transform);
            parentGameObject.transform.localScale = new Vector3(2, 2, 2);

            var localSize = parentGameObject.transform.InverseTransformSize(new Vector3(2, 2, 2));

            Assert.AreEqual(new Vector3(1, 1, 1), localSize);
            yield return null;
        }


        [UnityTest]
        public IEnumerator GetDepthTest()
        {
            var grandparentGameObject = new GameObject("Grandparent");
            var parentGameObject = new GameObject("Parent");
            var childGameObject = new GameObject("Child");
            childGameObject.transform.SetParent(parentGameObject.transform);
            parentGameObject.transform.SetParent(grandparentGameObject.transform);

            var depth = childGameObject.transform.GetDepth();

            Assert.AreEqual(1, depth);

            depth = grandparentGameObject.transform.GetDepth();
            Assert.AreEqual(-1, depth);

            depth = parentGameObject.transform.GetDepth();
            Assert.AreEqual(0, depth);

            yield return null;
        }

        [UnityTest]
        public IEnumerator GetChildRecursiveTest()
        {
            var greatGrandparentGameObject = new GameObject("Greatgrandparent");
            var grandparentGameObject = new GameObject("Grandparent");
            var parentGameObject = new GameObject("Parent");
            var uncleGameObject = new GameObject("Uncle");
            var childGameObject = new GameObject("Child");
            var cousinGameObject = new GameObject("Cousin");
            childGameObject.transform.SetParent(parentGameObject.transform);
            cousinGameObject.transform.SetParent(uncleGameObject.transform);
            parentGameObject.transform.SetParent(grandparentGameObject.transform);
            uncleGameObject.transform.SetParent(grandparentGameObject.transform);
            grandparentGameObject.transform.SetParent(greatGrandparentGameObject.transform);

            var childResult = TransformExtensions.GetChildRecursive(greatGrandparentGameObject.transform, "Child");
            Assert.AreSame(childGameObject.transform, childResult);
            var cousinResult = TransformExtensions.GetChildRecursive(greatGrandparentGameObject.transform, "Cousin");
            Assert.AreSame(cousinGameObject.transform, cousinResult);
            var nullResult = TransformExtensions.GetChildRecursive(greatGrandparentGameObject.transform, "Sister");
            Assert.IsNull(nullResult);

            yield return null;
        }
    }
}
