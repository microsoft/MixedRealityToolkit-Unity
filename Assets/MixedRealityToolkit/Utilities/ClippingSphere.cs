// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Utilities
{
    /// <summary>
    /// Component to animate and visualize a sphere that can be used with 
    /// per pixel based clipping.
    /// </summary>
    [ExecuteInEditMode]
    public class ClippingSphere : ClippingPrimitive
    {
        [Tooltip("The radius of the clipping sphere.")]
        [SerializeField]
        protected float radius = 0.5f;

        /// <summary>
        /// The radius of the clipping sphere.
        /// </summary>
        public float Radius => radius;

        private int clipSphereID;

        protected override string Keyword
        {
            get { return "_CLIPPING_SPHERE"; }
        }

        protected override string KeywordProperty
        {
            get { return "_ClippingSphere"; }
        }

        protected override string ClippingSideProperty
        {
            get { return "_ClipSphereSide"; }
        }

        private void OnDrawGizmosSelected()
        {
            if (enabled)
            {
                Gizmos.DrawWireSphere(transform.position, radius);
            }
        }

        protected override void Initialize()
        {
            base.Initialize();

            clipSphereID = Shader.PropertyToID("_ClipSphere");
        }

        protected override void UpdateShaderProperties(MaterialPropertyBlock materialPropertyBlock)
        {
            Vector3 position = transform.position;
            Vector4 sphere = new Vector4(position.x, position.y, position.z, radius);
            materialPropertyBlock.SetVector(clipSphereID, sphere);
        }
    }
}
