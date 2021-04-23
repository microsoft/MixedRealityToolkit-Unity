// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Boundary;
using NUnit.Framework;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Tests.EditMode.Boundary
{
    public class InscribedRectangleTests
    {
        // When doing double/expected value comparison, we give a slightly larger
        // than usual tolerance because of the nature of how the inscribed rectangle
        // works. Note that the tolerance is percent based because there are larger
        // errors that accumulate when the edges span a larger space.
        private const double TolerancePercent = 0.02;

        [Test]
        public void TestSimpleSquare()
        {
            // A set of edges that represents a 10x10 square.
            Edge[] edges = new Edge[]
            {
                new Edge(new Vector2(0, 0), new Vector2(0, 10)),
                new Edge(new Vector2(0, 10), new Vector2(10, 10)),
                new Edge(new Vector2(10, 10), new Vector2(10, 0)),
                new Edge(new Vector2(10, 0), new Vector2(0, 0))
            };

            InscribedRectangle rectangle = new InscribedRectangle(edges, 0 /*randomSeed*/);

            AssertWithinTolerance(10.0, rectangle.Height);
            AssertWithinTolerance(10.0, rectangle.Width);
        }

        [Test]
        public void TestSimpleRectangle()
        {
            // A set of edges that represents a 10x10 square.
            Edge[] edges = new Edge[]
            {
                new Edge(new Vector2(0, 0), new Vector2(0, 10)),
                new Edge(new Vector2(0, 10), new Vector2(100, 10)),
                new Edge(new Vector2(100, 10), new Vector2(100, 0)),
                new Edge(new Vector2(100, 0), new Vector2(0, 0))
            };

            InscribedRectangle rectangle = new InscribedRectangle(edges, 0 /*randomSeed*/);

            AssertWithinTolerance(10.0, rectangle.Height);
            AssertWithinTolerance(100.0, rectangle.Width);
        }

        /// <summary>
        /// This test case exists to validate the fix for https://github.com/microsoft/MixedRealityToolkit-Unity/issues/7171
        /// </summary>
        /// <remarks>
        /// In particular, this issue used an incorrect Math.min where Math.max was needed, cutting
        /// down the candidate search space for rectangles. This rough ASCII art shows how the issue comes about:
        ///
        /// #
        /// ###
        /// #####
        /// #######
        /// #########
        /// ###########
        /// #############
        /// ###############
        /// #################
        /// ###################
        /// #####################
        /// #######################
        /// #########################
        /// ###########################
        /// #############################
        /// #
        /// #############################
        ///
        /// In this 'room', notice that the largest inscribed rectangle is
        /// actually the one on top. However, because of the issue in #7171, the candidate search space
        /// was being reduced to only the bottom space. As a result, before the fix for that, we would
        /// return the size of the bottom rectangle (which is incorrect - it's the one on top).
        /// </remarks>
        [Test]
        public void TestDegenerateSailboat()
        {
            // A set of edges that represents the fancy art above. Note that in this test case
            // the width/height is a bit lengthened to really ensure that the top rectangle is
            // much bigger.
            Edge[] edges = new Edge[]
            {
                new Edge(new Vector2(0, 0), new Vector2(100, 0)),
                new Edge(new Vector2(100, 0), new Vector2(100, 1)),
                new Edge(new Vector2(100, 1), new Vector2(1, 1)),
                new Edge(new Vector2(1, 1), new Vector2(1, 2)),
                new Edge(new Vector2(1, 2), new Vector2(100, 2)),
                new Edge(new Vector2(100, 2), new Vector2(0, 100)),
                new Edge(new Vector2(0, 100), new Vector2(0, 0)),
            };

            InscribedRectangle rectangle = new InscribedRectangle(edges, 0 /*randomSeed*/);

            // The area should be greater than 100 because:
            // 1) The area of the rectangle below would roughly be less than or equal to 100, and this
            //    rectangle shouldn't get chosen by the algorithm because it's the smaller one.
            Assert.GreaterOrEqual(rectangle.Height * rectangle.Width, 100);

            // The center should be above y = 2 (i.e. we shouldn't be choosing the tiny sliver
            // under the sailboat).
            Assert.GreaterOrEqual(rectangle.Center.y, 2);
        }

        /// <summary>
        /// Asserts that the expected and actual values are within 'tolerance' percent.
        /// </summary>
        /// <remarks>
        /// Assumes that expected isn't zero (none of our test cases should lead to an empty sized
        /// rectangle).
        /// </remarks>
        private static void AssertWithinTolerance(double expected, double actual)
        {
            Assert.IsTrue(Math.Abs((expected - actual) / expected) < TolerancePercent);
        }
    }
}