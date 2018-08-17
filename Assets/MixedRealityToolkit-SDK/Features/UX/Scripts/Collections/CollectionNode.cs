// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.UX.Collections
{
    /// <summary>
    /// Collection node is a data storage class for individual data about an object in the collection.
    /// </summary>
    [Serializable]
    public class CollectionNode
    {
        public string Name;
        public Vector2 Offset;
        public float Radius;
        public Transform transform;
    }
}

