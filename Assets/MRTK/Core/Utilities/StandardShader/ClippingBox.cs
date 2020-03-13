// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Component to animate and visualize a box that can be used with 
    /// per pixel based clipping.
    /// </summary>
    [ExecuteInEditMode]
    [AddComponentMenu("Scripts/MRTK/Core/ClippingBox")]
    public class ClippingBox : ClippingPrimitive
    {
        /// <summary>
        /// The property name of the clip box data within the shader.
        /// </summary>
        protected int clipBoxSizeID;

        /// <summary>
        /// The property name of the clip box inverse transformation matrix within the shader.
        /// </summary>
        protected int clipBoxInverseTransformID;

        /// <inheritdoc />
        protected override string Keyword
        {
            get { return "_CLIPPING_BOX"; }
        }

        /// <inheritdoc />
        protected override string ClippingSideProperty
        {
            get { return "_ClipBoxSide"; }
        }

        /// <summary>
        /// Renders a visual representation of the clipping primitive when selected.
        /// </summary>
        protected void OnDrawGizmosSelected()
        {
            if (enabled)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
            }
        }

        /// <inheritdoc />
        protected override void Initialize()
        {
            base.Initialize();

            clipBoxSizeID = Shader.PropertyToID("_ClipBoxSize");
            clipBoxInverseTransformID = Shader.PropertyToID("_ClipBoxInverseTransform");
        }

        protected override void UpdateShaderProperties(MaterialPropertyBlock materialPropertyBlock)
        {
            Vector3 lossyScale = transform.lossyScale * 0.5f;
            Vector4 boxSize = new Vector4(lossyScale.x, lossyScale.y, lossyScale.z, 0.0f);
            materialPropertyBlock.SetVector(clipBoxSizeID, boxSize);
            Matrix4x4 boxInverseTransform = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one).inverse;
            materialPropertyBlock.SetMatrix(clipBoxInverseTransformID, boxInverseTransform);
        }
    }
}
