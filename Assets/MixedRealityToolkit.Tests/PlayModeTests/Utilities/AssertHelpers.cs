using NUnit.Framework;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    /// <summary>
    /// Test helpers that help with assertions of non-trivial object types.
    /// </summary>
    public class AssertHelpers
    {
        /// <summary>
        /// Validates that the given Vector3s are almost equal, in particular that the corresponding
        /// X, Y, and Z values of the two vectors differ by at most the specified tolerance (which
        /// defaults to .02)
        /// </summary>
        /// <remarks>
        /// Note that the tolerance is not based on the total summed delta - if any of the individual
        /// parts exceed the tolerance this assert will fail the test.
        /// </remarks>
        public static void AlmostEquals(Vector3 expected, Vector3 actual, float tolerance = 0.02f)
        {
            if (Math.Abs(expected.x - actual.x) > tolerance ||
                Math.Abs(expected.y - actual.y) > tolerance ||
                Math.Abs(expected.z - actual.z) > tolerance)
            {
                Assert.Fail($"Actual Vector3 value: {actual} differs from expected Vector3 value: {expected}");
            }
        }
    }
}
