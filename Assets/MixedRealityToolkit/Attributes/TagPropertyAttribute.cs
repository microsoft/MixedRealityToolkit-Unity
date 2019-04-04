// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
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
