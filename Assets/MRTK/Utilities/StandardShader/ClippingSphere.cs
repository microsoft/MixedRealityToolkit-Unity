// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Component to animate and visualize a sphere that can be used with 
    /// per pixel based clipping.
    /// </summary>
    [ExecuteInEditMode]
    [AddComponentMenu("Scripts/MRTK/Core/ClippingSphere")]
    public class ClippingSphere : ClippingPrimitive
    {
        /// <summary>
        /// The radius of the clipping sphere, which is determined by the largest axis of the transform's scale.
        /// </summary>
        public float Radius
        {
            get
            {
                Vector3 lossyScale = transform.lossyScale * 0.5f;
                return Mathf.Max(Mathf.Max(lossyScale.x, lossyScale.y), lossyScale.z);
            }
        }

        private int clipSphereID;

        /// <inheritdoc />
        protected override string Keyword
        {
            get { return "_CLIPPING_SPHERE"; }
        }

        /// <inheritdoc />
        protected override string ClippingSideProperty
        {
            get { return "_ClipSphereSide"; }
        }

        private void OnDrawGizmosSelected()
        {
            if (enabled)
            {
                Gizmos.DrawWireSphere(transform.position, Radius);
            }
        }

        /// <inheritdoc />
        protected override void Initialize()
        {
            base.Initialize();

            clipSphereID = Shader.PropertyToID("_ClipSphere");
        }

        /// <inheritdoc />
        protected override void UpdateShaderProperties(MaterialPropertyBlock materialPropertyBlock)
        {
            Vector3 position = transform.position;
            Vector4 sphere = new Vector4(position.x, position.y, position.z, Radius);
            materialPropertyBlock.SetVector(clipSphereID, sphere);
        }
    }
}
