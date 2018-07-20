// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity.Boundary;
using NUnit.Framework;
using System;
using System.Text;
using UnityEngine;

namespace HoloToolkit.Unity.Tests
{
    /// <summary>
    /// Some smoke tests to make sure the algorithm at least functioning
    /// </summary>
    public class InscribedRectangleTest
    {
        private class LargestRectangleTestCase
        {
            public string Name { get; set; }
            public Vector3[] Polygon { get; set; }
            public Vector2 ExpectedCenter { get; set; }
            public float ExpectedAngle { get; set; }
            public float ExpectedWidth { get; set; }
            public float ExpectedHeight { get; set; }
        }

        private LargestRectangleTestCase[] cases = new LargestRectangleTestCase[]
        {
            new LargestRectangleTestCase
            {
                Name = "Trivial 1",
                Polygon = new Vector3[]
                {
                    new Vector3(1, 0, 1),
                    new Vector3(1, 0, -1),
                    new Vector3(-1, 0, -1),
                    new Vector3(1, 0, 1),
                },
                ExpectedCenter = new Vector2(0.2895508f, -0.2895508f),
                ExpectedAngle = 45,
                ExpectedWidth = 1.203304f,
                ExpectedHeight = 0.8022028f,
            },

            new LargestRectangleTestCase
            {
                Name = "Actual 1",
                Polygon = new Vector3[]
                {
                    new Vector3(-0.5519378f, -1.206148f, -1.19394f),
                    new Vector3(0.650895f, -1.20466f, -1.449473f),
                    new Vector3(0.613879f, -1.205253f, -0.9079847f),
                    new Vector3(0.7181485f, -1.205405f, -0.6554281f),
                    new Vector3(0.8714309f, -1.205195f, -0.7076129f),
                    new Vector3(1.004714f, -1.206064f, 0.2727617f),
                    new Vector3(0.5489904f, -1.206536f, 0.2807388f),
                    new Vector3(0.5284376f, -1.206964f, 0.6781269f),
                    new Vector3(0.07208231f, -1.207793f, 1.033721f),
                    new Vector3(-0.3179387f, -1.207906f, 0.7551652f)
                },
                ExpectedCenter = new Vector2(0.2181969f, -0.4680347f),
                ExpectedAngle = 75,
                ExpectedWidth = 1.486504f,
                ExpectedHeight = 0.9910026f
            },

            new LargestRectangleTestCase
            {
                Name = "Actual 2",
                Polygon = new Vector3[]
                {
                    new Vector3(-0.001732647f, -0.926618f, 1.239084f),
                    new Vector3(-1.178016f, -0.9283609f, 2.398043f),
                    new Vector3(-2.164025f, -0.9293305f, 2.676547f),
                    new Vector3(-2.908203f, -0.9298851f, 2.636722f),
                    new Vector3(-2.423435f, -0.929828f, 3.091747f),
                    new Vector3(-1.226971f, -0.9292948f, 3.661167f),
                    new Vector3(0.2079207f, -0.9281015f, 3.563209f),
                    new Vector3(0.1217785f, -0.927461f, 2.564639f),
                    new Vector3(0.5864475f, -0.9265049f, 1.729308f),
                    new Vector3(0.4944292f, -0.9261689f, 1.15375f),
                },
                ExpectedCenter = new Vector2(-0.5928315f, 2.338437f),
                ExpectedAngle = 135,
                ExpectedWidth = 2.40228f,
                ExpectedHeight = 0.6863657f
            },
        };

        /// <summary>
        /// Helper to write the code for a polygon.
        /// </summary>
        /// <param name="polygon"></param>
        public static void DumpPolygon(Vector3[] polygon)
        {
            var sb = new StringBuilder();
            foreach (var v in polygon)
            {
                sb.AppendFormat("new Vector3({0}f, {1}f, {2}f),\r\n", v.x, v.y, v.z);
            }
            Debug.Log(sb.ToString());
        }

        /// <summary>
        /// Helper to check if two floating point numbers are close enough.
        /// </summary>
        private bool FloatsEqual(float expected, float actual)
        {
            return Math.Abs((double)(expected - actual)) < 0.0001;
        }

        /// <summary>
        /// Run the tests. Outputs the results to the Unity log.
        /// </summary>
        [Test]
        public void RunTestCases()
        {
            foreach (var testCase in cases)
            {
                RunCase(testCase);
            }
        }

        private void RunCase(LargestRectangleTestCase testCase)
        {
            InscribedRectangle inscribedRectangle = new InscribedRectangle(testCase.Polygon, 20);
            Vector2 actualCenter;
            float actualAngle;
            float actualWidth;
            float actualHeight;
            inscribedRectangle.GetRectangleParams(out actualCenter, out actualAngle, out actualWidth, out actualHeight);

            bool passed =
                FloatsEqual(testCase.ExpectedCenter.x, actualCenter.x) &&
                FloatsEqual(testCase.ExpectedCenter.y, actualCenter.y) &&
                FloatsEqual(testCase.ExpectedAngle, actualAngle) &&
                FloatsEqual(testCase.ExpectedWidth, actualWidth) &&
                FloatsEqual(testCase.ExpectedHeight, actualHeight);

            if (!passed)
            {
                var message = string.Format("Test Case \"{0}\" failed\r\nExpected: ({1},{2})  {3}  {4}  {5}\r\nActual: ({6},{7})  {8}  {9}  {10}",
                    testCase.Name,
                    testCase.ExpectedCenter.x,
                    testCase.ExpectedCenter.y,
                    testCase.ExpectedAngle,
                    testCase.ExpectedWidth,
                    testCase.ExpectedHeight,
                    actualCenter.x,
                    actualCenter.y,
                    actualAngle,
                    actualWidth,
                    actualHeight
                    );
                Assert.Fail(message);
            }
        }
    }
}
