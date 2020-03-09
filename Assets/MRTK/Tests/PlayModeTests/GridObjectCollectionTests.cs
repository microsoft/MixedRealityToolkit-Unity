// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if !WINDOWS_UWP
// When the .NET scripting backend is enabled and C# projects are built
// The assembly that this file is part of is still built for the player,
// even though the assembly itself is marked as a test assembly (this is not
// expected because test assemblies should not be included in player builds).
// Because the .NET backend is deprecated in 2018 and removed in 2019 and this
// issue will likely persist for 2018, this issue is worked around by wrapping all
// play mode tests in this check.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using System;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using System.Text;
using System.IO;


namespace Microsoft.MixedReality.Toolkit.Tests
{
    /// <summary>
    /// Tests different layout behavior of GridObjectCollection.
    /// You can use GridObjectLayoutControl.cs in the examples package to
    /// quickly generate the expected positions used in these tests.
    /// </summary>
    internal class GridObjectCollectionTests
    {
        [SetUp]
        public void Setup()
        {
            PlayModeTestUtilities.Setup();
            TestUtilities.PlayspaceToOriginLookingForward();
        }

        [TearDown]
        public void TearDown()
        {
            PlayModeTestUtilities.TearDown();
        }

        #region Tests
        /// <summary>
        /// Tests that grid lays out object correctly for all different anchor types
        /// </summary>
        [UnityTest]
        public IEnumerator TestAnchors()
        {
            // Create a grid
            GameObject go = new GameObject();
            go.name = "grid";
            var grid = go.AddComponent<GridObjectCollection>();
            grid.Distance = 0.75f;
            grid.CellWidth = 0.15f;
            grid.CellHeight = 0.15f;

            for(int i = 0; i < 3; i++)
            {
                var child = GameObject.CreatePrimitive(PrimitiveType.Cube);
                child.transform.parent = go.transform;
                child.transform.localScale = Vector3.one * 0.1f;
            }

            grid.Layout = LayoutOrder.Horizontal;

            int expectedIdx = 0;
            foreach(LayoutAnchor et in Enum.GetValues(typeof(LayoutAnchor)))
            {
                grid.Anchor = et;
                grid.UpdateCollection();
                int j = 0;
                foreach(Transform child2 in go.transform)
                {
                    var expected = anchorTestExpected[expectedIdx];
                    var actual = child2.transform.localPosition;
                    TestUtilities.AssertAboutEqual(
                        actual, 
                        expected, 
                        "Child object not in expected position, layout " + et, 
                        0.01f);
                    j++;
                    expectedIdx++;
                }
                yield return null;

            }
            yield return null;
        }

    #region Expected Values
    // You can use GridObjectLayoutControl.cs in the examples package to
    // quickly generate the expected positions used in these tests.

    private Vector3[] anchorTestExpected = new Vector3[] {
        new Vector3(0.08f, -0.08f, 0.75f), // UpperLeft index 0
        new Vector3(0.23f, -0.08f, 0.75f), // UpperLeft index 1
        new Vector3(0.38f, -0.08f, 0.75f), // UpperLeft index 2
        new Vector3(-0.15f, -0.08f, 0.75f), // UpperCenter index 0
        new Vector3(0.00f, -0.08f, 0.75f), // UpperCenter index 1
        new Vector3(0.15f, -0.08f, 0.75f), // UpperCenter index 2
        new Vector3(-0.38f, -0.08f, 0.75f), // UpperRight index 0
        new Vector3(-0.23f, -0.08f, 0.75f), // UpperRight index 1
        new Vector3(-0.08f, -0.08f, 0.75f), // UpperRight index 2
        new Vector3(0.08f, 0.00f, 0.75f), // MiddleLeft index 0
        new Vector3(0.23f, 0.00f, 0.75f), // MiddleLeft index 1
        new Vector3(0.38f, 0.00f, 0.75f), // MiddleLeft index 2
        new Vector3(-0.15f, 0.00f, 0.75f), // MiddleCenter index 0
        new Vector3(0.00f, 0.00f, 0.75f), // MiddleCenter index 1
        new Vector3(0.15f, 0.00f, 0.75f), // MiddleCenter index 2
        new Vector3(-0.38f, 0.00f, 0.75f), // MiddleRight index 0
        new Vector3(-0.23f, 0.00f, 0.75f), // MiddleRight index 1
        new Vector3(-0.08f, 0.00f, 0.75f), // MiddleRight index 2
        new Vector3(0.08f, 0.08f, 0.75f), // BottomLeft index 0
        new Vector3(0.23f, 0.08f, 0.75f), // BottomLeft index 1
        new Vector3(0.38f, 0.08f, 0.75f), // BottomLeft index 2
        new Vector3(-0.15f, 0.08f, 0.75f), // BottomCenter index 0
        new Vector3(0.00f, 0.08f, 0.75f), // BottomCenter index 1
        new Vector3(0.15f, 0.08f, 0.75f), // BottomCenter index 2
        new Vector3(-0.38f, 0.08f, 0.75f), // BottomRight index 0
        new Vector3(-0.23f, 0.08f, 0.75f), // BottomRight index 1
        new Vector3(-0.08f, 0.08f, 0.75f) // BottomRight index 2
    };
    #endregion
    
    #endregion
    }
}
#endif