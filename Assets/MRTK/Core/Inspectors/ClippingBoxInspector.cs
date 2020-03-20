// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// A custom editor for the ClippingBox to allow for specification of the framing bounds.
    /// </summary>
    [CustomEditor(typeof(ClippingBox))]
    [CanEditMultipleObjects]
    public class ClippingBoxEditor : ClippingPrimitiveEditor
    {
        /// <inheritdoc/>
        protected override bool HasFrameBounds()
        {
            return true;
        }

        /// <inheritdoc/>
        protected override Bounds OnGetFrameBounds()
        {
            var primitive = target as ClippingBox;
            Debug.Assert(primitive != null);
            return new Bounds(primitive.transform.position, primitive.transform.lossyScale);
        }
    }
}
