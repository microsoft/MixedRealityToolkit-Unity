// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    /// <summary>
    /// Editor utility class to discern world space sizing of objects in scene
    /// </summary>
    public static class SizeUtilities
    {
        /// <summary>
        /// Finds the first Renderer type component on the selected GameObject in scene and returns its world space bounds size.
        /// </summary>
        [MenuItem("GameObject/MRTK Debug Utilities/Print Renderer Size", false, 40)]
        public static void RendererSize()
        {
            if (Selection.activeGameObject == null)
            {
                Debug.Log("No selected gameobject is available to calculate Renderer size.");
                return;
            }

            var renderer = Selection.activeGameObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                Debug.Log($"Renderer on GameObject \"{renderer.name}\" has world-space bounds size of {renderer.bounds.size}");
            }
            else
            {
                Debug.Log($"No Renderer component found on {Selection.activeGameObject}");
            }
        }

        [MenuItem("GameObject/MRTK Debug Utilities/Print Renderer Size", true, 40)]
        private static bool ValidateRendererSize()
        {
            if (Selection.activeGameObject == null)
            {
                return false;
            }

            var renderers = Selection.activeGameObject.GetComponent<Renderer>();
            return (renderers != null);
        }

        /// <summary>
        /// Finds all Collider type components on the selected GameObject in scene and returns their world space bounds size.
        /// </summary>
        [MenuItem("GameObject/MRTK Debug Utilities/Print Collider Size", false, 41)]
        public static void ColliderSize()
        {
            if (Selection.activeGameObject == null)
            {
                Debug.Log("No selected gameobject is available to calculate Collider size.");
                return;
            }

            var colliders = Selection.activeGameObject.GetComponents<Collider>();
            if (colliders != null && colliders.Length != 0)
            {
                Debug.Log($"Following Collider components found on \"{Selection.activeGameObject}\"");
                foreach (var c in colliders)
                {
                    Debug.Log($"Collider of type {c.GetType()} has world-space bounds size of {c.bounds.size}");
                }
            }
            else
            {
                Debug.Log($"No Collider components found on {Selection.activeGameObject}");
            }
        }

        [MenuItem("GameObject/MRTK Debug Utilities/Print Collider Size", true, 41)]
        private static bool ValidateColliderSize()
        {
            if (Selection.activeGameObject == null)
            {
                return false;
            }

            var colliders = Selection.activeGameObject.GetComponents<Collider>();
            return (colliders != null && colliders.Length != 0);
        }
    }
}