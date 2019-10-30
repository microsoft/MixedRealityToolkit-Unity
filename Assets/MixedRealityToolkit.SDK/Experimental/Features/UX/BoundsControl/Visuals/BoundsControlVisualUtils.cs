using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI.Experimental.BoundsControlTypes;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Experimental
{
    /// <summary>
    /// Helper class providing some static utility functions for <see cref="BoundsControl"/> <see cref="BoundsControlHandlesBase">handles</see>/>
    /// </summary>
    internal class BoundsControlVisualUtils
    {

        internal static void HandleIgnoreCollider(Collider handlesIgnoreCollider, List<Transform> handles)
        {
            if (handlesIgnoreCollider != null)
            {
                foreach (Transform handle in handles)
                {
                    Collider[] colliders = handle.gameObject.GetComponents<Collider>();
                    foreach (Collider collider in colliders)
                    {
                        UnityEngine.Physics.IgnoreCollision(collider, handlesIgnoreCollider);
                    }
                }
            }
        }

        internal static Bounds GetMaxBounds(GameObject g)
        {
            var b = new Bounds();
            Mesh currentMesh;
            foreach (MeshFilter r in g.GetComponentsInChildren<MeshFilter>())
            {
                if ((currentMesh = r.sharedMesh) == null) { continue; }

                if (b.size == Vector3.zero)
                {
                    b = currentMesh.bounds;
                }
                else
                {
                    b.Encapsulate(currentMesh.bounds);
                }
            }
            return b;
        }

        internal static void ApplyMaterialToAllRenderers(GameObject root, Material material)
        {
            if (material != null)
            {
                Renderer[] renderers = root.GetComponentsInChildren<Renderer>();

                for (int i = 0; i < renderers.Length; ++i)
                {
                    renderers[i].material = material;
                }
            }
        }

        /// <summary>
        /// Add all common components to a corner or rotate affordance
        /// </summary>
        internal static void AddComponentsToAffordance(GameObject afford, Bounds bounds, RotationHandlePrefabCollider colliderType, 
            CursorContextInfo.CursorAction cursorType, Vector3 colliderPadding, Transform parent, bool drawTetherWhenManipulating)
        {
            if (colliderType == RotationHandlePrefabCollider.Box)
            {
                BoxCollider collider = afford.AddComponent<BoxCollider>();
                collider.size = bounds.size;
                collider.center = bounds.center;
                collider.size += colliderPadding;
            }
            else
            {
                SphereCollider sphere = afford.AddComponent<SphereCollider>();
                sphere.center = bounds.center;
                sphere.radius = bounds.extents.x;
                sphere.radius += Mathf.Max(Mathf.Max(colliderPadding.x, colliderPadding.y), colliderPadding.z);
            }

            // In order for the affordance to be grabbed using near interaction we need
            // to add NearInteractionGrabbable;
            var g = afford.EnsureComponent<NearInteractionGrabbable>();
            g.ShowTetherWhenManipulating = drawTetherWhenManipulating;

            var contextInfo = afford.EnsureComponent<CursorContextInfo>();
            contextInfo.CurrentCursorAction = cursorType;
            contextInfo.ObjectCenter = parent;
        }

        /// <summary>
        /// Creates the default material for bounds control handles
        /// </summary>
        internal static Material CreateDefaultMaterial()
        {
            return Resources.Load<Material>("BoundsControlHandleDefault");
        }

    }
}
