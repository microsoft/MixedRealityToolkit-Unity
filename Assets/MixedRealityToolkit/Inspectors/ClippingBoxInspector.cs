// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Core.Utilities;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Inspectors
{
    [CustomEditor(typeof(ClippingBox))]
    public class ClippingBoxEditor : Editor
    {
        private bool HasFrameBounds() { return true; }

        private Bounds OnGetFrameBounds()
        {
            var primitive = target as ClippingBox;
            Debug.Assert(primitive != null);
            return new Bounds(primitive.transform.position, primitive.transform.lossyScale * 0.5f);
        }
    }
}
