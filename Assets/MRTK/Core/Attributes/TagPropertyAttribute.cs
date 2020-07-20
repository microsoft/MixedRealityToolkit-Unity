// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using UnityEngine;
using System;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// A PropertyAttribute for Unity tags (a string field).
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class TagPropertyAttribute : PropertyAttribute
    {
        // Do nothing
    }
}
