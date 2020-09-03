// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Attribute used to make a float or int variable in a script be restricted to a specific range.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class Vector3RangeAttribute : PropertyAttribute
    {
        /// <summary>
        /// Minimum value.
        /// </summary>
        public readonly float Min;

        /// <summary>
        /// Maximum value.
        /// </summary>
        public readonly float Max;

        /// <summary>
        /// Attribute used to make a float or int variable in a script be restricted to a specific range.
        /// </summary>
        /// <param name="min">The minimum allowed value.</param>
        /// <param name="max">The maximum allowed value.</param>
        public Vector3RangeAttribute(float min, float max)
        {
            Min = min;
            Max = max;
        }
    }
}
