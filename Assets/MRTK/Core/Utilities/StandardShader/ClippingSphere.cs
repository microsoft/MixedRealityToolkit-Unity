// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
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
        /// The radius of the clipping sphere, determined by the largest axis of the transform's scale.
        /// </summary>
        [Obsolete("Use Radii instead as ellipsoids are supported. ")]
        public float Radius
        {
            get
            {
                Vector3 radii = Radii;
                return Mathf.Max(radii.x, radii.y, radii.z);
            }
        }

        /// <summary>
        /// The radius of the clipping sphere on each axis, determined by the transform's scale.
        /// </summary>
        public Vector3 Radii => transform.lossyScale * 0.5f;

        /// <summary>
        /// The property name of the clip sphere data within the shader.
        /// </summary>
        protected int clipSphereInverseTransformID;
        private Matrix4x4 clipSphereInverseTransform;

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

        /// <summary>
        /// Renders a visual representation of the clipping primitive when selected.
        /// </summary>
        protected void OnDrawGizmosSelected()
        {
            if (enabled)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawWireSphere(Vector3.zero, 0.5f);
            }
        }

        /// <inheritdoc />
        protected override void Initialize()
        {
            base.Initialize();

            clipSphereInverseTransformID = Shader.PropertyToID("_ClipSphereInverseTransform");
        }

        protected override void BeginUpdateShaderProperties()
        {
            clipSphereInverseTransform = transform.worldToLocalMatrix;
            base.BeginUpdateShaderProperties();
        }

        /// <inheritdoc />
        protected override void UpdateShaderProperties(MaterialPropertyBlock materialPropertyBlock)
        {
            materialPropertyBlock.SetMatrix(clipSphereInverseTransformID, clipSphereInverseTransform);
        }
    }
}
