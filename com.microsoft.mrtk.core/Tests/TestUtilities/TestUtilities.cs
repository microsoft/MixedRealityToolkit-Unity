// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using NUnit.Framework;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Tests
{
    public static class TestUtilities
    {
        public static void AssertAboutEqual(Vector3 actual, Vector3 expected, string message, float tolerance = 0.01f)
        {
            Assert.That((actual - expected).magnitude,
                        Is.EqualTo(0.0f).Within(tolerance),
                        $"{message}, expected {expected:0.000}, was {actual:0.000}");
        }

        public static void AssertAboutEqual(Quaternion actual, Quaternion expected, string message, float tolerance = 0.01f)
        {
            Assert.That(Quaternion.Angle(actual, expected),
                        Is.EqualTo(0.0f).Within(tolerance),
                        $"{message}, expected {expected:0.000}, was {actual:0.000}");
        }

        public static void AssertNotAboutEqual(Vector3 val1, Vector3 val2, string message, float tolerance = 0.01f)
        {
            Assert.That((val1 - val2).magnitude,
                        Is.GreaterThan(tolerance),
                        $"{message}, val1 {val1:0.000} almost equals val2 {val2:0.000}");
        }

        public static void AssertNotAboutEqual(Quaternion val1, Quaternion val2, string message, float tolerance = 0.01f)
        {
            Assert.That(Quaternion.Angle(val1, val2),
                        Is.GreaterThan(tolerance),
                        $"{message}, val1 {val1:0.000} almost equals val2 {val2:0.000}");
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
