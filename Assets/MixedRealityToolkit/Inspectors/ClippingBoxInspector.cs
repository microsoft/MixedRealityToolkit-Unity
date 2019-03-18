// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using MRTKPrefix.Utilities;
using UnityEditor;
using UnityEngine;

namespace MRTKPrefix.Editor
{
    [CustomEditor(typeof(ClippingBox))]
    public class ClippingBoxEditor : UnityEditor.Editor
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
