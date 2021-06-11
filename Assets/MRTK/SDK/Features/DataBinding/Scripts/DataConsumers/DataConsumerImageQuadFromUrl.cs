// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.Data
{
    /// <summary>
    /// A data consumer that will fetch any supported image type from a URL
    /// and associated with a Quad MeshRenderer component  being managed by this
    /// object.
    /// </summary>
    
    [Serializable]
    public class DataConsumerImageQuadFromUrl : DataConsumerImageTextureFromUrl
    {
        [Tooltip("(Optional) Specific mesh renderer to populate with a retrieved image. If not specified, first MeshRenderer found in this or children will be used.")]
        [SerializeField] private MeshRenderer ImageQuadMeshRenderer;


        internal override Type[] GetComponentTypes()
        {

            Type[] types = { typeof(MeshRenderer) };
            return types;
        }


        internal override void InitializeForComponent(Type componentType, Component component)
        {
            if (ImageQuadMeshRenderer == null)
            {
                // No specific MeshRenderer specified, so assign the one that was found for us.

                ImageQuadMeshRenderer = component as MeshRenderer;
            }
            ImageQuadMeshRenderer.material = new Material(Shader.Find("Unlit/Texture")); ;
        }

 

        internal override void PlaceImageTexture(Texture2D imageTexture)
        {
            ImageQuadMeshRenderer.material.mainTexture = imageTexture;

        }

    }
}
