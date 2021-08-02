// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.UI;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Defines a comparer to sort TransformConstraints by their
    /// requested execution order, or any other priority
    /// mechanism that a subclass utilizes.
    /// </summary>
    internal class ConstraintExecOrderComparer : IComparer<TransformConstraint>
    {
        /// <returns>
        /// Returns < 0 if x should be executed first.
        /// Returns > 0 if y should be executed first.
        /// Returns = 0 if they are of equivalent execution priority.
        /// </returns>	
        public virtual int Compare(TransformConstraint x, TransformConstraint y)
        {
            return x.ExecutionPriority - y.ExecutionPriority;
        }
    }
}