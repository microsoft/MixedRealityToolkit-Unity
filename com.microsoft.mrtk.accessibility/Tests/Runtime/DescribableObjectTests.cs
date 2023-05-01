// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Core.Tests;
using Microsoft.MixedReality.Toolkit.Input.Tests;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Accessibility.Tests
{
    /// <summary>
    /// Tests for verifying the behavior of the accessible objects.
    /// </summary>
    public class AccessibleObjectTests : BaseRuntimeInputTests
    {
        #region Test cases

        private static string testCubeGuid = "d10f05ae3a6402045b70860918544ed9";
        private static string testCubeAssetPath = AssetDatabase.GUIDToAssetPath(testCubeGuid);

        private readonly Vector3[] inViewPositions =
        {
            // Top row
            new Vector3(-0.5f,  0.5f,   4f),
            new Vector3(0f,     0.5f,   4f),
            new Vector3(0.5f,   0.5f,   4f),
            // Center row
            new Vector3(-0.5f,  0f,     4f),
            new Vector3(0f,     0f,     4f),
            new Vector3(0.5f,   0f,     4f),
            // Bottom row
            new Vector3(-0.5f,  -0.5f,  4f),
            new Vector3(0f,     -0.5f,  4f),
            new Vector3(0.5f,   -0.5f,  4f)
        };

        private readonly Vector3[] outOfViewPositions =
        {
            // Top row, behind
            new Vector3(-0.5f,  0.5f,   -4f),
            new Vector3(0f,     0.5f,   -4f),
            new Vector3(0.5f,   0.5f,   -4f),
            // Center row, behind
            new Vector3(-0.5f,  0f,     -4f),
            new Vector3(0f,     0f,     -4f),
            new Vector3(0.5f,   0f,     -4f),
            // Bottom row, behind
            new Vector3(-0.5f,  -0.5f,  -4f),
            new Vector3(0f,     -0.5f,  -4f),
            new Vector3(0.5f,   -0.5f,  -4f)
        };

        /// <summary>
        /// Creates a test object and places it at the specified location.
        /// </summary>
        /// <param name="location">The position at which to place the created object.</param>
        /// <param name="isAccessible">Should the object have the AccessibleObject script attached?</param>
        private void CreateTestObject(
            Vector3 location,
            bool isAccessible)
        {
            GameObject gameObj;

            if (isAccessible)
            {
                gameObj = Object.Instantiate(
                    AssetDatabase.LoadAssetAtPath<GameObject>(testCubeAssetPath));
            }
            else
            {
                gameObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                gameObj.transform.localScale = Vector3.one * 0.1f;
            }
            gameObj.transform.position = location;
        }

        /// <summary>
        /// Test case to verify that an empty scene is properly handled.
        /// </summary>
        [UnityTest]
        public IEnumerator EmptyScene()
        {
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsTrue(AccessibilityHelpers.Subsystem != null, "Accessibility subsystem not found.");

            if (AccessibilityHelpers.Subsystem != null)
            {
                List<AccessibleObjectClassification> classifications = new List<AccessibleObjectClassification>();
                bool success = AccessibilityHelpers.Subsystem.TryGetAccessibleObjectClassifications(classifications);
                Assert.IsTrue(success, "Failed to get the collection of accessible object classifications.");

                List<GameObject> accessibleObjects = new List<GameObject>();
                success = AccessibilityHelpers.Subsystem.TryGetAccessibleObjects(
                    classifications,
                    AccessibleObjectVisibility.Surround,
                    float.MaxValue,
                    accessibleObjects);
                Assert.IsTrue(success, "Failed to get the collection of accessible objects.");
                Assert.IsTrue(accessibleObjects.Count == 0, "Should not have found any accessible objects an empty scene.");
            }
        }

        /// <summary>
        /// Test case to verify that a scene with only objects that are not accessible is properly handled.
        /// </summary>
        [UnityTest]
        public IEnumerator ObjectsNotAccessible()
        {
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsTrue(AccessibilityHelpers.Subsystem != null, "Accessibility subsystem not found.");

            if (AccessibilityHelpers.Subsystem != null)
            {
                foreach (Vector3 pos in inViewPositions)
                {
                    // Create the objects without adding AccessibleObject.
                    CreateTestObject(pos, false);
                }

                yield return RuntimeTestUtilities.WaitForUpdates();

                List<AccessibleObjectClassification> classifications = new List<AccessibleObjectClassification>();
                bool success = AccessibilityHelpers.Subsystem.TryGetAccessibleObjectClassifications(classifications);
                Assert.IsTrue(success, "Failed to get the collection of accessible object classifications.");

                List<GameObject> accessibleObjects = new List<GameObject>();
                success = AccessibilityHelpers.Subsystem.TryGetAccessibleObjects(
                    classifications,
                    AccessibleObjectVisibility.FieldOfView,
                    float.MaxValue,
                    accessibleObjects);
                Assert.IsTrue(success, "Failed to get the collection of accessible objects.");
                Assert.IsTrue(accessibleObjects.Count == 0, "Should not have found any accessible objects in the scene.");
            }
        }

        /// <summary>
        /// Test case to verify that only accessible objects in the camera view are reported.
        /// </summary>
        [UnityTest]
        public IEnumerator CameraViewObjects()
        {
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsTrue(AccessibilityHelpers.Subsystem != null, "Accessibility subsystem not found.");

            if (AccessibilityHelpers.Subsystem != null)
            {
                // Create objects in the field of view.
                foreach (Vector3 pos in inViewPositions)
                {
                    CreateTestObject(pos, true);
                }

                // Create objects outside of the field of view.
                foreach (Vector3 pos in outOfViewPositions)
                {
                    CreateTestObject(pos, true);
                }

                yield return RuntimeTestUtilities.WaitForUpdates();

                List<AccessibleObjectClassification> classifications = new List<AccessibleObjectClassification>();
                bool success = AccessibilityHelpers.Subsystem.TryGetAccessibleObjectClassifications(classifications);
                Assert.IsTrue(success, "Failed to get the collection of accessible object classifications.");

                List<GameObject> accessibleObjects = new List<GameObject>();
                success = AccessibilityHelpers.Subsystem.TryGetAccessibleObjects(
                    classifications,
                    AccessibleObjectVisibility.FieldOfView,
                    float.MaxValue,
                    accessibleObjects);
                Assert.IsTrue(success, "Failed to get the collection of accessible objects.");
                Assert.AreEqual(
                    inViewPositions.Length,
                    accessibleObjects.Count,
                    "Failed to find the correct number of accessible objects in the scene.");
            }

            yield return null;
        }

        /// <summary>
        /// Test case to verify that all accessible objects in the scene are reported.
        /// </summary>
        [UnityTest]
        public IEnumerator SurroundObjects()
        {
            yield return RuntimeTestUtilities.WaitForUpdates();

            Assert.IsTrue(AccessibilityHelpers.Subsystem != null, "Accessibility subsystem not found.");

            if (AccessibilityHelpers.Subsystem != null)
            {
                // Create objects in the field of view.
                foreach (Vector3 pos in inViewPositions)
                {
                    CreateTestObject(pos, true);
                }

                // Create objects outside of the field of view.
                foreach (Vector3 pos in outOfViewPositions)
                {
                    CreateTestObject(pos, true);
                }

                yield return RuntimeTestUtilities.WaitForUpdates();

                List<AccessibleObjectClassification> classifications = new List<AccessibleObjectClassification>();
                bool success = AccessibilityHelpers.Subsystem.TryGetAccessibleObjectClassifications(classifications);
                Assert.IsTrue(success, "Failed to get the collection of accessible object classifications.");

                List<GameObject> accessibleObjects = new List<GameObject>();
                success = AccessibilityHelpers.Subsystem.TryGetAccessibleObjects(
                    classifications,
                    AccessibleObjectVisibility.Surround,
                    float.MaxValue,
                    accessibleObjects);
                Assert.IsTrue(success, "Failed to get the collection of accessible objects.");
                Assert.AreEqual(
                    inViewPositions.Length + outOfViewPositions.Length,
                    accessibleObjects.Count,
                    "Failed to find the correct number of accessible objects in the scene.");
            }

            yield return null;
        }

        #endregion Test cases
    }
}
