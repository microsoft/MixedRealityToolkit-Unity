// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// An attribute that allows a particular field to be rendered as multi-selectable
    /// set of flags.
    /// </summary>
    /// <remarks>
    /// From https://answers.unity.com/questions/486694/default-editor-enum-as-flags-.html
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class EnumFlagsAttribute : PropertyAttribute
    {
        public EnumFlagsAttribute() { }
    }
}