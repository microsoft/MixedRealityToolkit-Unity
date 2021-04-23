// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Abstract component to encapsulate common functionality around outline components. 
    /// </summary>
    [HelpURL("https://docs.microsoft.com/windows/mixed-reality/mrtk-unity/features/rendering/mrtk-standard-shader#mesh-outlines")]
    public abstract class BaseMeshOutline : MonoBehaviour
    {
        /// <summary>
        /// The material used to render the outline. Outline materials should normal have "Depth Write" set to Off and "Vertex Extrusion" enabled.
        /// Most MRTK/Standard features should work as an outline material, but it is recommended to keep the outline material as simple as possible.
        /// </summary>
        public Material OutlineMaterial
        {
            get { return outlineMaterial; }
            set
            {
                if (outlineMaterial != value)
                {
                    outlineMaterial = value;
                    ApplyOutlineMaterial();
                }
            }
        }

        [Tooltip("The material used to render the outline. Outline materials should normal have \"Depth Write\" set to Off and \"Vertex Extrusion\" enabled.")]
        [SerializeField]
        protected Material outlineMaterial = null;

        /// <summary>
        /// How thick (in meters) should the outline be. Overrides the "Extrusion Value" in the MRTK/Standard material.
        /// </summary>
        public float OutlineWidth
        {
            get { return outlineWidth; }
            set
            {
                if (outlineWidth != value)
                {
                    outlineWidth = value;
                    ApplyOutlineWidth();
                }
            }
        }

        [Tooltip("How thick (in meters) should the outline be. Overrides the \"Extrusion Value\" in the MRTK/Standard material.")]
        [SerializeField]
        [Range(0.001f, 1.0f)]
        protected float outlineWidth = 0.01f;

        #region MonoBehaviour Implementation

        /// <summary>
        /// Enables users to modify inspector properties while playing in the editor.
        /// </summary>
        protected virtual void OnValidate()
        {
            ApplyOutlineMaterial();
            ApplyOutlineWidth();
        }

        #endregion MonoBehaviour Implementation

        protected abstract void ApplyOutlineMaterial();
        protected abstract void ApplyOutlineWidth();
    }
}
