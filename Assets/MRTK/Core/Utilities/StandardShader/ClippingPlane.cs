// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Component to animate and visualize a plane that can be used with 
    /// per pixel based clipping.
    /// </summary>
    [ExecuteInEditMode]
    [AddComponentMenu("Scripts/MRTK/Core/ClippingPlane")]
    public class ClippingPlane : ClippingPrimitive
    {
        /// <summary>
        /// The property name of the clip plane data within the shader.
        /// </summary>
        protected int clipPlaneID;

        /// <inheritdoc />
        protected override string Keyword
        {
            get { return "_CLIPPING_PLANE"; }
        }

        /// <inheritdoc />
        protected override string ClippingSideProperty
        {
            get { return "_ClipPlaneSide"; }
        }

        /// <summary>
        /// Renders a visual representation of the clipping primitive when selected.
        /// </summary>
        protected void OnDrawGizmosSelected()
        {
            if (enabled)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawWireCube(Vector3.zero, new Vector3(1.0f, 0.0f, 1.0f));
                Gizmos.DrawLine(Vector3.zero, Vector3.up * -0.5f);
            }
        }

        /// <inheritdoc />
        protected override void Initialize()
        {
            base.Initialize();

            clipPlaneID = Shader.PropertyToID("_ClipPlane");
        }

        /// <inheritdoc />
        protected override void UpdateShaderProperties(MaterialPropertyBlock materialPropertyBlock)
        {
            Vector3 up = transform.up;
            Vector4 plane = new Vector4(up.x, up.y, up.z, Vector3.Dot(up, transform.position));

            materialPropertyBlock.SetVector(clipPlaneID, plane);
        }
    }
}
