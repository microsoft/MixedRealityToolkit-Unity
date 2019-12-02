// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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