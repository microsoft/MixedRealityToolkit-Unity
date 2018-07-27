// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class Vector3RangeAttribute : PropertyAttribute
    {
        public readonly float Min;
        public readonly float Max;

        /// <summary>
        ///   <para>Attribute used to make a float or int variable in a script be restricted to a specific range.</para>
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
