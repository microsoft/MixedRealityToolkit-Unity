// Copyright (c) Microsoft Corporation.
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
    [CanEditMultipleObjects]
    public abstract class ClippingPrimitiveEditor : UnityEditor.Editor
    {
        /// <summary>
        /// Notifies the Unity editor if this object has custom frame bounds.
        /// </summary>
        /// <returns>True if custom frame bounds can be used from OnGetFrameBounds.</returns>
        protected abstract bool HasFrameBounds();

        /// <summary>
        /// Returns the bounds the editor should focus on.
        /// </summary>
        /// <returns>The bounds of the clipping primitive.</returns>
        protected abstract Bounds OnGetFrameBounds();

        private ClippingPrimitive clippingPrimitive;

        private void OnEnable()
        {
            clippingPrimitive = (ClippingPrimitive)target;
        }

        /// <summary>
        /// Looks for changes to the list of renderers and gracefully adds and removes them.
        /// </summary>
        public override void OnInspectorGUI()
        {
            var previousRenderers = clippingPrimitive.GetRenderersCopy();

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                DrawDefaultInspector();

                if (check.changed)
                {
                    // Flagging changes other than renderers
                    clippingPrimitive.IsDirty = true;
                }
            }

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
