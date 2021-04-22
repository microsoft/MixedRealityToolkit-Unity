// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Tests.EditMode.Extensions
{
    public class BoundsExtensionsTests
    {
        List<Vector3> boundsPoints = new List<Vector3>();
        GameObject cube;
        Vector3[] expectedPoints;
        string path;

        [SetUp]
        public void SetUp()
        {
            cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = Vector3.zero;
            cube.transform.rotation = Quaternion.identity;
            boundsPoints.Clear();
            var timestr = DateTime.UtcNow.ToString("yyMMdd-HHmmss");
            path = Path.Combine(Application.persistentDataPath, $"BoundsExtensionsTests-{timestr}.txt");
        }

        private void WriteBoundsPointsToFile()
        {
            using (var writer = new StreamWriter(path, true))
            {
                writer.WriteLine();
                writer.WriteLine("expectedPoints = new Vector3[] {");
                boundsPoints.ForEach((pt) => writer.WriteLine($"\tnew Vector3({pt.x}f, {pt.y}f, {pt.z}f),"));
                writer.WriteLine("};");
            }
        }

        [Test]
        public void GetColliderBoundsPoints()
        {
            // SetUp
            expectedPoints = new Vector3[] {
                new Vector3(-0.5f, -0.5f, -0.5f),
                new Vector3(-0.5f, -0.5f, 0.5f),
                new Vector3(-0.5f, 0.5f, -0.5f),
                new Vector3(-0.5f, 0.5f, 0.5f),
                new Vector3(0.5f, -0.5f, -0.5f),
                new Vector3(0.5f, -0.5f, 0.5f),
                new Vector3(0.5f, 0.5f, -0.5f),
                new Vector3(0.5f, 0.5f, 0.5f),
            };

            BoundsExtensions.GetColliderBoundsPoints(cube, boundsPoints, 0);

            Assert.AreEqual(expectedPoints, boundsPoints.ToArray());

            boundsPoints.Clear();
            cube.transform.localScale = Vector3.one * 2f;
            BoundsExtensions.GetColliderBoundsPoints(cube, boundsPoints, 0);
            expectedPoints = new Vector3[] {
                new Vector3(-1f, -1f, -1f),
                new Vector3(-1f, -1f, 1f),
                new Vector3(-1f, 1f, -1f),
                new Vector3(-1f, 1f, 1f),
                new Vector3(1f, -1f, -1f),
                new Vector3(1f, -1f, 1f),
                new Vector3(1f, 1f, -1f),
                new Vector3(1f, 1f, 1f),
            };
            Assert.AreEqual(expectedPoints, boundsPoints.ToArray());

            boundsPoints.Clear();
            cube.transform.localScale = new Vector3(10, 1, 1);
            BoundsExtensions.GetColliderBoundsPoints(cube, boundsPoints, 0);

            expectedPoints = new Vector3[] {
                new Vector3(-5f, -0.5f, -0.5f),
                new Vector3(-5f, -0.5f, 0.5f),
                new Vector3(-5f, 0.5f, -0.5f),
                new Vector3(-5f, 0.5f, 0.5f),
                new Vector3(5f, -0.5f, -0.5f),
                new Vector3(5f, -0.5f, 0.5f),
                new Vector3(5f, 0.5f, -0.5f),
                new Vector3(5f, 0.5f, 0.5f),
            };

            Assert.AreEqual(expectedPoints, boundsPoints.ToArray());
        }

        [Test]
        public void GetColliderBoundsPointsRelativeTo()
        {
            var relativeTo = new GameObject();
            relativeTo.transform.position = Vector3.forward;
            relativeTo.transform.rotation = Quaternion.identity;

            BoundsExtensions.GetColliderBoundsPoints(cube, boundsPoints, 0, relativeTo.transform);
            expectedPoints = new Vector3[] {
                new Vector3(-0.5f, -0.5f, -1.5f),
                new Vector3(-0.5f, -0.5f, -0.5f),
                new Vector3(-0.5f, 0.5f, -1.5f),
                new Vector3(-0.5f, 0.5f, -0.5f),
                new Vector3(0.5f, -0.5f, -1.5f),
                new Vector3(0.5f, -0.5f, -0.5f),
                new Vector3(0.5f, 0.5f, -1.5f),
                new Vector3(0.5f, 0.5f, -0.5f),
            };
            Assert.AreEqual(expectedPoints, boundsPoints.ToArray());

            boundsPoints.Clear();
            relativeTo.transform.localScale = 0.5f * Vector3.one;
            BoundsExtensions.GetColliderBoundsPoints(cube, boundsPoints, 0, relativeTo.transform);
            expectedPoints = new Vector3[] {
                new Vector3(-1f, -1f, -3f),
                new Vector3(-1f, -1f, -1f),
                new Vector3(-1f, 1f, -3f),
                new Vector3(-1f, 1f, -1f),
                new Vector3(1f, -1f, -3f),
                new Vector3(1f, -1f, -1f),
                new Vector3(1f, 1f, -3f),
                new Vector3(1f, 1f, -1f),
            };
            Assert.AreEqual(expectedPoints, boundsPoints.ToArray());
        }
    }
}