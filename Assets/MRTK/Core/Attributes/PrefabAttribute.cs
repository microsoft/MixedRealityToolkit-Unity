// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Attribute used to ensure that a GameObject inspector slot only accepts prefabs.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class PrefabAttribute : PropertyAttribute { }
}