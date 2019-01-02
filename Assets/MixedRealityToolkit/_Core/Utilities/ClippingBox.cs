// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Utilities
{
    /// <summary>
    /// Component to animate and visualize a box that can be used with 
    /// per pixel based clipping.
    /// </summary>
    [ExecuteInEditMode]
    public class ClippingBox : ClippingPrimitive
    {
        private int clipBoxSizeID;
        private int clipBoxInverseTransformID;

        protected override string Keyword
        {
            get { return "_CLIPPING_BOX"; }
        }

        protected override string KeywordProperty
        {
            get { return "_ClippingBox"; }
        }

        protected override string ClippingSideProperty
        {
            get { return "_ClipBoxSide"; }
        }

        private void OnDrawGizmosSelected()
        {
            if (enabled)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
            }
        }

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
