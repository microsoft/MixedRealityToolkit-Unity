// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Component which can be used to render an outline around a mesh renderer. Enabling this component introduces an additional render pass 
    /// of the object being outlined, but is designed to run performantly on mobile Mixed Reality devices and does not utilize any post processes.
    /// This behavior is designed to be used in conjunction with the MRTK/Standard shader. Limitations of this effect include it not working well 
    /// on objects which are non-uniformly scaled and depth sorting issues on overlapping objects.
    /// </summary>
    [RequireComponent(typeof(MeshRenderer))]
    public class MeshOutline : MonoBehaviour
    {
        private const string smoothNormalsKeyword = "_SMOOTH_NORMALS";
        private const string vertexExtrusionValueName = "_VertexExtrusionValue";

        /// <summary>
        /// The material used to render the outline. Outline materials should normal have "Depth Write" set to Off and "Vertex Extrusion" enabled.
        /// Most MRTK/Standard features should work as an outline material, but it is recommended to use a simple unlit material.
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

        [Tooltip("The material used to render the outline. Outline materials should normal have \"Depth Write\" set to Off and \"Vertex Extrusion\" enabled.")]
        [SerializeField]
        private Material outlineMaterial = null;

        /// <summary>
        /// How thick (in meters) should the outline be. Overrides the "Extrusion Value" in the MRTK/Standard material.
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

        [Tooltip("How thick (in meters) should the outline be. Overrides the \"Extrusion Value\" in the MRTK/Standard material.")]
        [SerializeField]
        [Range(0.001f, 1.0f)]
        private float outlineWidth = 0.01f;

        private MeshRenderer meshRenderer = null;
        private MaterialPropertyBlock propertyBlock = null;
        private int vertexExtrusionValueID = 0;
        private Material[] defaultMaterials = null;

        #region MonoBehaviour Implementation

        /// <summary>
        /// Used to update the outline width when modifying the property in the inspector.
        /// </summary>
        private void OnValidate()
        {
            ApplyOutlineMaterial();
        }

        /// <summary>
        /// Gathers initial render state.
        /// </summary>
        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            propertyBlock = new MaterialPropertyBlock();
            vertexExtrusionValueID = Shader.PropertyToID(vertexExtrusionValueName);
            defaultMaterials = meshRenderer.sharedMaterials;
        }

        /// <summary>
        /// Enables the outline.
        /// </summary>
        private void OnEnable()
        {
            ApplyOutlineMaterial();
        }

        /// <summary>
        /// Resets the renderer materials to the default settings.
        /// </summary>
        private void OnDisable()
        {
            meshRenderer.materials = defaultMaterials;
        }

        #endregion MonoBehaviour Implementation

        /// <summary>
        /// Prepares and applies the current outline material to the renderer.
        /// </summary>
        private void ApplyOutlineMaterial()
        {
            if (outlineMaterial != null && meshRenderer != null)
            {
                // Ensure that the outline material always renders before the default materials. 
                outlineMaterial.renderQueue = GetMinRenderQueue(defaultMaterials) - 1;

                // If smooth normals are requested, make sure the mesh has smooth normals.
                if (outlineMaterial.IsKeywordEnabled(smoothNormalsKeyword))
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

        /// <summary>
        /// Updates the current vertex extrusion value used by the shader. 
        /// </summary>
        private void ApplyOutlineWidth()
        {
            if (meshRenderer != null && propertyBlock != null)
            {
                meshRenderer.GetPropertyBlock(propertyBlock);
                propertyBlock.SetFloat(vertexExtrusionValueName, outlineWidth);
                meshRenderer.SetPropertyBlock(propertyBlock);
            }
        }

        /// <summary>
        /// Searches for the minimum render queue value in a list of materials.
        /// </summary>
        /// <param name="materials">The list of materials to search.</param>
        /// <returns>The minimum render queue value.</returns>
        private static int GetMinRenderQueue(Material[] materials)
        {
            var min = int.MaxValue;

            foreach (var material in materials)
            {
                min = Mathf.Min(min, material.renderQueue);
            }

            if (min == int.MaxValue)
            {
                min = (int)RenderQueue.Background;
            }

            return min;
        }
    }
}
