// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// An attribute that allows a particular field to be constrained to a range dependent on other variables.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class VariableRangeAttribute : PropertyAttribute
    {
        /// <summary>
        /// The name of the minimum variable.
        /// </summary>
        public string MinVariableName { get; private set; }

        /// <summary>
        /// The name of the maximum variable.
        /// </summary>
        public string MaxVariableName { get; private set; }


        /// <summary>
        /// Constructor for a VariableRangeAttribute.
        /// </summary>
        /// <param name="minVariableName">The name of the minimum variable.</param>
        /// <param name="maxVariableName">The name of the maximum variable.</param>
        public VariableRangeAttribute(string minVariableName, string maxVariableName)
        {
            MinVariableName = minVariableName;
            MaxVariableName = maxVariableName;
        }
    }
}
