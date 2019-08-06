// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// TODO
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class MeshOutline : MonoBehaviour
    {
        /// <summary>
        /// TODO Make sure material has "Depth Write" set to Off. And "Vertex Extrusion" enabled.
        /// </summary>
        public Material OutlineMaterial
        {
            get { return outlineMaterial; }
            set
            {
                outlineMaterial = value;
                ApplyOutlineMaterial();
            }
        }

        [Tooltip("TODO")]
        [SerializeField]
        private Material outlineMaterial = null;

        /// <summary>
        /// TODO
        /// </summary>
        public float OutlineWidth
        {
            get { return outlineWidth; }
            set
            {
                outlineWidth = value;
                ApplyOutlineWidth();
            }
        }

        [Tooltip("TODO")]
        [SerializeField]
        [Range(0.001f, 1.0f)]
        private float outlineWidth = 0.01f;

        private MaterialPropertyBlock propertyBlock = null;
        private MeshRenderer meshRenderer = null;
        private Material[] defaultMaterials = null;

        private void OnValidate()
        {
            ApplyOutlineMaterial();
        }

        private void Awake()
        {
            propertyBlock = new MaterialPropertyBlock();
            meshRenderer = GetComponent<MeshRenderer>();
            defaultMaterials = meshRenderer.sharedMaterials;
        }

        private void OnEnable()
        {
            ApplyOutlineMaterial();
        }

        private void OnDisable()
        {
            meshRenderer.materials = defaultMaterials;
        }

        private void ApplyOutlineMaterial()
        {
            if (outlineMaterial != null && meshRenderer != null)
            {
                // Ensure that the outline material always renders before the default materials. 
                outlineMaterial.renderQueue = GetMinRenderQueue(defaultMaterials) - 1;

                // If smooth normals are requested, make sure the mesh has smooth normals.
                if (outlineMaterial.IsKeywordEnabled("_SMOOTH_NORMALS"))
                {
                    gameObject.EnsureComponent<MeshSmoother>().SmoothNormalsAsync();
                }

                ApplyOutlineWidth();

                // Add the outline material as another material pass.
                var materials = new List<Material>(defaultMaterials);
                materials.Add(outlineMaterial);
                meshRenderer.materials = materials.ToArray();
            }
        }

        private void ApplyOutlineWidth()
        {
            if (meshRenderer != null && propertyBlock != null)
            {
                meshRenderer.GetPropertyBlock(propertyBlock);
                propertyBlock.SetFloat("_VertexExtrusionValue", outlineWidth);
                meshRenderer.SetPropertyBlock(propertyBlock);
            }
        }

        private static int GetMinRenderQueue(Material[] materials)
        {
            var min = int.MaxValue;

            foreach (var material in materials)
            {
                min = Mathf.Min(min, material.renderQueue);
            }

            return min;
        }
    }
}
