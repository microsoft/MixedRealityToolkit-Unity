// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Component to animate and visualize a plane that can be used with 
    /// per pixel based clipping.
    /// </summary>
    [ExecuteInEditMode]
    public class ClippingPlane : ClippingPrimitive
    {
        private int clipPlaneID;

        protected override string Keyword
        {
            get { return "_CLIPPING_PLANE"; }
        }

        protected override string KeywordProperty
        {
            get { return "_ClippingPlane"; }
        }

        protected override string ClippingSideProperty
        {
            get { return "_ClipPlaneSide"; }
        }

        private void OnDrawGizmosSelected()
        {
            if (enabled)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawWireCube(Vector3.zero, new Vector3(1.0f, 0.0f, 1.0f));
                Gizmos.DrawLine(Vector3.zero, Vector3.up * -0.5f);
            }
        }

        protected override void Initialize()
        {
            base.Initialize();

            clipPlaneID = Shader.PropertyToID("_ClipPlane");
        }

        protected override void UpdateShaderProperties(MaterialPropertyBlock materialPropertyBlock)
        {
            Vector3 up = transform.up;
            Vector4 plane = new Vector4(up.x, up.y, up.z, Vector3.Dot(up, transform.position));

            materialPropertyBlock.SetVector(clipPlaneID, plane);
        }
    }
}
