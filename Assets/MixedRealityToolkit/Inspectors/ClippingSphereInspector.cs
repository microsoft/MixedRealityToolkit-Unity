// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    [CustomEditor(typeof(ClippingSphere))]
    public class ClippingSphereEditor : UnityEditor.Editor
    {
        private bool HasFrameBounds() { return true; }

        private Bounds OnGetFrameBounds()
        {
            var primitive = target as ClippingSphere;
            Debug.Assert(primitive != null);
            return new Bounds(primitive.transform.position, Vector3.one * primitive.Radius);
        }
    }
}
