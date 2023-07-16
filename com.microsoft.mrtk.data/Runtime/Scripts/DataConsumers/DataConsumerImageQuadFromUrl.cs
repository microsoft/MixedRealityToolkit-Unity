// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// Disable "missing XML comment" warning for the experimental package.
// While nice to have, documentation is not required for this experimental package.
#pragma warning disable CS1591

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Data
{
    /// <summary>
    /// A data consumer that will fetch any supported image type from a URL
    /// and associated with a Quad MeshRenderer component being managed by this
    /// object.
    /// </summary>
    [Serializable]
    [AddComponentMenu("MRTK/Data Binding/Consumers/Data Consumer Image Quad From Url")]
    public class DataConsumerImageQuadFromUrl : DataConsumerImageTextureFromUrl
    {
        [Tooltip("(Optional) Specific mesh renderer to populate with a retrieved image. If not specified, first MeshRenderer found in this or children will be used.")]
        [SerializeField]
        private MeshRenderer imageQuadMeshRenderer = null;

        [Tooltip("(Optional) Default material to use for showing image as a texture.")]
        [SerializeField]
        private Material defaultMaterial = null;

        /// <inheritdoc/>
        protected override Type[] GetComponentTypes()
        {
            Type[] types = { typeof(MeshRenderer) };
            return types;
        }

        /// <inheritdoc/>
        protected override void InitializeForComponent(Component component)
        {
            if (imageQuadMeshRenderer == null)
            {
                // No specific MeshRenderer specified, so assign the one that was found for us.

                imageQuadMeshRenderer = component as MeshRenderer;
            }

            if (defaultMaterial != null)
            {
                imageQuadMeshRenderer.material = defaultMaterial;
            }
        }

        /// <inheritdoc/>
        protected override void PlaceImageTexture(Texture2D imageTexture)
        {
            imageQuadMeshRenderer.material.mainTexture = imageTexture;
        }
    }
}
#pragma warning restore CS1591