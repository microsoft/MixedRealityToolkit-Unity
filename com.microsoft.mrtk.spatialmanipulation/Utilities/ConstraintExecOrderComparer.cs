// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation
{
    /// <summary>
    /// Defines a comparer to sort TransformConstraints by their
    /// requested execution order, or any other priority
    /// mechanism that a subclass utilizes.
    /// </summary>
    internal class ConstraintExecOrderComparer : IComparer<TransformConstraint>
    {
        /// <summary>
        /// Compare two <see cref="TransformConstraint"/> objects.
        /// </summary>
        /// <param name="x">The first values to compare.</param>
        /// <param name="y">The second value to compare.</param>
        /// <returns>
        /// Returns a negative value if <paramref name="x"/> is less than <paramref name="y"/>, zero
        /// if <paramref name="x"/> and <paramref name="y"/> are equal, and a positive value if
        /// <paramref name="x"/> is greater than <paramref name="y"/>.
        /// </returns>	
        public virtual int Compare(TransformConstraint x, TransformConstraint y)
        {
            return x.ExecutionPriority - y.ExecutionPriority;
        }
    }
}
