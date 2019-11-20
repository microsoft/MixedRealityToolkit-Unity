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
    /// on objects which are not watertight (or required to be two sided) and depth sorting issues can occur on overlapping objects.
    /// </summary>
    [RequireComponent(typeof(MeshRenderer))]
    public class MeshOutline : BaseMeshOutline
    {
        private const string vertexExtrusionKeyword = "_VERTEX_EXTRUSION";
        private const string vertexExtrusionSmoothNormalsKeyword = "_VERTEX_EXTRUSION_SMOOTH_NORMALS";
        private const string vertexExtrusionValueName = "_VertexExtrusionValue";

        private MeshRenderer meshRenderer = null;
        private MaterialPropertyBlock propertyBlock = null;
        private int vertexExtrusionValueID = 0;
        private Material[] defaultMaterials = null;
        private MeshSmoother createdMeshSmoother = null;

        #region MonoBehaviour Implementation

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

        /// <summary>
        /// Removes any components this component has created.
        /// </summary>
        private void OnDestroy()
        {
            Destroy(createdMeshSmoother);
        }

        #endregion MonoBehaviour Implementation

        #region BaseMeshOutline Implementation

        /// <summary>
        /// Prepares and applies the current outline material to the renderer.
        /// </summary>
        protected override void ApplyOutlineMaterial()
        {
            if (outlineMaterial != null && meshRenderer != null)
            {
                Debug.AssertFormat(outlineMaterial.IsKeywordEnabled(vertexExtrusionKeyword), 
                                   "The material \"{0}\" does not have vertex extrusion enabled, no outline will be rendered.", outlineMaterial.name);

                // Ensure that the outline material always renders before the default materials.
                outlineMaterial.renderQueue = GetMinRenderQueue(defaultMaterials) - 1;

                // If smooth normals are requested, make sure the mesh has smooth normals.
                if (outlineMaterial.IsKeywordEnabled(vertexExtrusionSmoothNormalsKeyword))
                {
                    var meshSmoother = (createdMeshSmoother == null) ? gameObject.GetComponent<MeshSmoother>() : createdMeshSmoother;

                    if (meshSmoother == null)
                    {
                        createdMeshSmoother = gameObject.AddComponent<MeshSmoother>();
                        meshSmoother = createdMeshSmoother;
                    }

                    meshSmoother.SmoothNormals();
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
        protected override void ApplyOutlineWidth()
        {
            if (meshRenderer != null && propertyBlock != null)
            {
                meshRenderer.GetPropertyBlock(propertyBlock);
                propertyBlock.SetFloat(vertexExtrusionValueName, outlineWidth);
                meshRenderer.SetPropertyBlock(propertyBlock);
            }
        }

        #endregion BaseMeshOutline Implementation

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
                if (material != null)
                {
                    min = Mathf.Min(min, material.renderQueue);
                }
            }

            if (min == int.MaxValue)
            {
                min = (int)RenderQueue.Background;
            }

            return min;
        }
    }
}
