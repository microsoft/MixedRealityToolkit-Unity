// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Core.Utilities;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Inspectors
{
    [CustomEditor(typeof(ClippingPlane))]
    public class ClippingPlaneEditor : Editor
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
