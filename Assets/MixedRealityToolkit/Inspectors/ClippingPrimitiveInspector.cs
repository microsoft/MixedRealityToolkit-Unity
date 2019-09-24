// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// An abstract editor component to improve the editor experience with ClippingPrimitives.
    /// </summary>
    [CustomEditor(typeof(ClippingPrimitive))]
    public abstract class ClippingPrimitiveEditor : UnityEditor.Editor
    {
        /// <summary>
        /// Notifies the editor that this object has custom frame bounds.
        /// </summary>
        /// <returns>True for all clipping primitives.</returns>
        private bool HasFrameBounds()
        {
            return true;
        }

        /// <summary>
        /// Returns the bounds the editor should focus on.
        /// </summary>
        /// <returns>The bounds of the clipping primitive.</returns>
        protected abstract Bounds OnGetFrameBounds();

        /// <summary>
        /// Looks for changes to the list of renderers and gracefully adds and removes them.
        /// </summary>
        public override void OnInspectorGUI()
        {
            var clippingPrimitive = (ClippingPrimitive)target;

            var previousRenderers = clippingPrimitive.GetRenderersCopy();
            DrawDefaultInspector();
            var currentRenderers = clippingPrimitive.GetRenderersCopy();

            // Add or remove and renderers that were added or removed via the inspector.
            foreach (var renderer in previousRenderers.Except(currentRenderers))
            {
                clippingPrimitive.RemoveRenderer(renderer);
            }

            foreach (var renderer in currentRenderers.Except(previousRenderers))
            {
                clippingPrimitive.AddRenderer(renderer);
            }
        }
    }
}
