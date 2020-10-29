// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// A <see cref=" Microsoft.MixedReality.Toolkit.Utilities.ObjectCollectionNode"/> specific for <see cref="ScrollingObjectCollection"/>.
    /// </summary>
    [Serializable]
    public class ScrollingObjectCollectionNode : ObjectCollectionNode
    {
        public bool isClipped;

        public ScrollingObjectCollectionNode() { }

        public ScrollingObjectCollectionNode(ObjectCollectionNode baseCollection)
        {
            Name = baseCollection.Name;
            Offset = baseCollection.Offset;
            Radius = baseCollection.Radius;
            Transform = baseCollection.Transform;
            Colliders = baseCollection.Colliders;
        }
    }
}
