// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Tests
{
    public class TestUtilities : MonoBehaviour
    {
        public static void AssertAboutEqual(Vector3 actual, Vector3 expected, string message, float tolerance = 0.01f)
        {
            var dist = (actual - expected).magnitude;
            Debug.Assert(dist < tolerance, $"{message}, expected {expected.ToString("0.000")}, was {actual.ToString("0.000")}");
        }

        public static void AssertAboutEqual(Quaternion actual, Quaternion expected, string message, float tolerance = 0.01f)
        {
            var angle = Quaternion.Angle(actual, expected);
            Debug.Assert(angle < tolerance, $"{message}, expected {expected.ToString("0.000")}, was {actual.ToString("0.000")}");
        }

        public static void AssertNotAboutEqual(Vector3 val1, Vector3 val2, string message, float tolerance = 0.01f)
        {
            var dist = (val1 - val2).magnitude;
            Debug.Assert(dist >= tolerance, $"{message}, val1 {val1.ToString("0.000")} almost equals val2 {val2.ToString("0.000")}");
        }

        public static void AssertNotAboutEqual(Quaternion val1, Quaternion val2, string message, float tolerance = 0.01f)
        {
            var angle = Quaternion.Angle(val1, val2);
            Debug.Assert(angle >= tolerance, $"{message}, val1 {val1.ToString("0.000")} almost equals val2 {val2.ToString("0.000")}");
        }

        /// <summary>
        /// Equivalent to NUnit.Framework.Assert.LessOrEqual, except this also
        /// applies a slight tolerance on the equality check.
        /// </summary>
        /// <remarks>
        /// This allows for things like LessThanOrEqual(2.00000024, 2.0) to still pass.
        /// </remarks>
        public static void AssertLessOrEqual(float observed, float expected, float tolerance = 0.01f)
        {
            Debug.Assert((Mathf.Abs(observed - expected) <= tolerance) || (observed < expected));
        }

        /// <summary>
        /// Equivalent to NUnit.Framework.Assert.LessOrEqual, except this also
        /// applies a slight tolerance on the equality check.
        /// </summary>
        /// <remarks>
        /// This allows for things like LessThanOrEqual(2.00000024, 2.0) to still pass.
        /// </remarks>
        public static void AssertLessOrEqual(float observed, float expected, string message, float tolerance = 0.01f)
        {
            Debug.Assert((Mathf.Abs(observed - expected) <= tolerance) || (observed < expected), message);
        }

        /// <summary>
        /// Equivalent to NUnit.Framework.Assert.GreaterOrEqual, except this also
        /// applies a slight tolerance on the equality check.
        /// </summary>
        /// <remarks>
        /// This allows for things like GreaterThanOrEqual(1.999999999, 2.0) to still pass.
        /// </remarks>
        public static void AssertGreaterOrEqual(float observed, float expected, float tolerance = 0.01f)
        {
            Debug.Assert((Mathf.Abs(observed - expected) <= tolerance) || (observed > expected));
        }
        /// <summary>
        /// Equivalent to NUnit.Framework.Assert.GreaterOrEqual, except this also
        /// applies a slight tolerance on the equality check.
        /// </summary>
        /// <remarks>
        /// This allows for things like GreaterThanOrEqual(1.999999999, 2.0) to still pass.
        /// </remarks>
        public static void AssertGreaterOrEqual(float observed, float expected, string message, float tolerance = 0.01f)
        {
            Debug.Assert((Mathf.Abs(observed - expected) <= tolerance) || (observed > expected), message);
        }
    }
}