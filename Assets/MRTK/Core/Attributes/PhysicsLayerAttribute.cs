// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Physics
{
    /// <summary>
    /// Attribute used to make an <see cref="int"/> field render a dropdown generated from the current layers defined in the Tag Manager.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class PhysicsLayerAttribute : PropertyAttribute
    {
        public PhysicsLayerAttribute() { }
    }
}