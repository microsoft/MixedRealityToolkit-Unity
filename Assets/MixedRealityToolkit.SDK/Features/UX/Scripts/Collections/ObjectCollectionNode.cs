// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Collection node is a data storage class for individual data about an object in a collection.
    /// </summary>
    [Serializable]
    public struct ObjectCollectionNode
    {
        public string Name;
        public Vector2 Offset;
        public float Radius;
        public Transform Transform;
    }
}