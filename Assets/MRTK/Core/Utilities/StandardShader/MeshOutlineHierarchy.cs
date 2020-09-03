// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Component which can be used to render an outline around a hierarchy of mesh renderers using
    /// the <see cref="Microsoft.MixedReality.Toolkit.Utilities.MeshOutline"/> component.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Core/MeshOutlineHierarchy")]
    public class MeshOutlineHierarchy : BaseMeshOutline
    {
        private MeshOutline[] meshOutlines = null;

        #region MonoBehaviour Implementation

        /// <summary>
        /// Creates a <see cref="Microsoft.MixedReality.Toolkit.Utilities.MeshOutline"/> component on each child MeshRenderer.
        /// </summary>
        private void Awake()
        {
            MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
            meshOutlines = new MeshOutline[meshRenderers.Length];

            for (int i = 0; i < meshRenderers.Length; ++i)
            {
                var meshOutline = meshRenderers[i].gameObject.AddComponent<MeshOutline>();
                meshOutline.OutlineMaterial = outlineMaterial;
                meshOutline.OutlineWidth = outlineWidth;
                meshOutlines[i] = meshOutline;
            }
        }

        /// <summary>
        /// Removes any components this component has created.
        /// </summary>
        private void OnDestroy()
        {
            foreach (var meshOutline in meshOutlines)
            {
                Destroy(meshOutline);
            }
        }

        #endregion MonoBehaviour Implementation

        #region BaseMeshOutline Implementation

        /// <summary>
        /// Forwards the outlineMaterial to all children <see cref="Microsoft.MixedReality.Toolkit.Utilities.MeshOutline"/>s.
        /// </summary>
        protected override void ApplyOutlineMaterial()
        {
            if (meshOutlines != null)
            {
                foreach (var meshOutline in meshOutlines)
                {
                    if (meshOutline != null)
                    {
                        meshOutline.OutlineMaterial = outlineMaterial;
                    }
                }
            }
        }

        /// <summary>
        /// Forwards the outlineWidth to all children <see cref="Microsoft.MixedReality.Toolkit.Utilities.MeshOutline"/>s.
        /// </summary>
        protected override void ApplyOutlineWidth()
        {
            if (meshOutlines != null)
            {
                foreach (var meshOutline in meshOutlines)
                {
                    if (meshOutline != null)
                    {
                        meshOutline.OutlineWidth = outlineWidth;
                    }
                }
            }
        }

        #endregion BaseMeshOutline Implementation
    }
}
