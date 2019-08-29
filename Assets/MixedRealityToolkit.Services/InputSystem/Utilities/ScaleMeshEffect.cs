// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.Input.Utilities
{
    /// <summary>
    /// On Unity UI components the unity_ObjectToWorld matrix is not the transformation matrix of the local 
    /// transform the Image component lives on, but that of it's parent Canvas. Many MRTK/Standard shader 
    /// effects require scale to be known. To solve this issue the ScaleMeshEffect will store scaling 
    /// information into UV channel 3 during UI mesh construction.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class ScaleMeshEffect : BaseMeshEffect
    {
        /// <summary>
        /// Enforces the parent canvas uses UV channel 3.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            // Make sure the parent canvas is configured to use UV channel 3 because 
            // scale will be written to that channel.
            var canvas = GetComponentInParent<Canvas>();

            if (canvas != null)
            {
                canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord3;
            }
        }

        /// <summary>
        /// Stores scaling information into UV channel 3 during UI mesh construction.
        /// </summary>
        /// <param name="vh">The vertex helper to populate with new vertex data.</param>
        public override void ModifyMesh(VertexHelper vh)
        {
            var rectTransform = transform as RectTransform;
            var scale = new Vector2(-rectTransform.rect.width * rectTransform.localScale.x, 
                                    -rectTransform.rect.height * rectTransform.localScale.y);
            var vertex = new UIVertex();

            for (int i = 0; i < vh.currentVertCount; ++i)
            {
                vh.PopulateUIVertex(ref vertex, i);
                vertex.uv3 = scale;
                vh.SetUIVertex(vertex, i);
            }
        }
    }
}
