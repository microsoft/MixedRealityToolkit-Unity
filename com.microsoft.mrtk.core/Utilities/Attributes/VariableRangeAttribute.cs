// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// An attribute that allows a particular field to be constrained to a range dependent on other variables
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class VariableRangeAttribute : PropertyAttribute
    {
        public string MinVariableName { get; private set; }

        public string MaxVariableName { get; private set; }


        public VariableRangeAttribute(string minVariableName, string maxVariableName)
        {
            MinVariableName = minVariableName;
            MaxVariableName = maxVariableName;
        }
    }
}
