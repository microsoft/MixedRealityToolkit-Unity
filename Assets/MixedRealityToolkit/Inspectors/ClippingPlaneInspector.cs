// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using MRTKPrefix.Utilities;
using UnityEditor;
using UnityEngine;

namespace MRTKPrefix.Editor
{
    [CustomEditor(typeof(ClippingPlane))]
    public class ClippingPlaneEditor : UnityEditor.Editor
    {
        private bool HasFrameBounds() { return true; }

        private Bounds OnGetFrameBounds()
        {
            var primitive = target as ClippingPlane;
            Debug.Assert(primitive != null);
            return new Bounds(primitive.transform.position, Vector3.one);
        }
    }
}
