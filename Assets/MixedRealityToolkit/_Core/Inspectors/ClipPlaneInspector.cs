// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Core.Utilities;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Inspectors
{
    [CustomEditor(typeof(ClipPlane))]
    public class ClipPlaneEditor : Editor
    {
        private bool HasFrameBounds() { return true; }

        private Bounds OnGetFrameBounds()
        {
            var clipPlane = target as ClipPlane;
            Debug.Assert(clipPlane != null);
            return new Bounds(clipPlane.transform.position, Vector3.one);
        }
    }
}
