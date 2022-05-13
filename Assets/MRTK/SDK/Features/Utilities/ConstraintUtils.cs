// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.UI;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Utilities for the management of constraints.
    /// </summary>
    internal static class ConstraintUtils
    {
        /// <summary>
        /// Adds a constraint to the specified already-sorted list of constraints, maintaining
        /// execution priority order. SortedSet is not used, as equal priorities
        /// break duplicate-checking with SortedSet, as well as SortedSet not being
        /// able to handle runtime-changing exec priorities.
        /// </summary>
        /// <param name="constraintList">Sorted list of existing priorites</param>
        /// <param name="constraint">Constraint to add</param>
        /// <param name="comparer">ConstraintExecOrderComparer for comparing two constraint priorities</param>
        internal static void AddWithPriority(ref List<TransformConstraint> constraintList, TransformConstraint constraint, ConstraintExecOrderComparer comparer)
        {
            if (constraintList.Contains(constraint))
            {
                return;
            }

            if (constraintList.Count == 0 || comparer.Compare(constraintList[constraintList.Count - 1], constraint) < 0)
            {
                constraintList.Add(constraint);
                return;
            }
            else if (comparer.Compare(constraintList[0], constraint) > 0)
            {
                constraintList.Insert(0, constraint);
                return;
            }
            else
            {
                int idx = constraintList.BinarySearch(constraint, comparer);
                if (idx < 0)
                {
                    // idx will be the two's complement of the index of the
                    // next element that is "larger" than the given constraint.
                    idx = ~idx;
                }
                constraintList.Insert(idx, constraint);
            }
        }
    }
}
