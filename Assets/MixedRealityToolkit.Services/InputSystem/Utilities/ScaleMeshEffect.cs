// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.Input.Utilities
{
    /// <summary>
    /// On Unity UI components the unity_ObjectToWorld matrix is not the transformation matrix of the local 
    /// transform the Graphic component lives on, but that of it's parent Canvas. Many MRTK/Standard shader 
    /// effects require object scale to be known. To solve this issue the ScaleMeshEffect will store scaling 
    /// information into UV channel attributes during UI mesh construction. Ideally we would store the scale 
    /// in one attribute but UGUI only supports two scalers per attribute (even in the tangent attribute).
    /// </summary>
    [RequireComponent(typeof(RectTransform), typeof(Graphic))]
    [HelpURL("https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/README_MRTKStandardShader.html#ugui-support")]
    [AddComponentMenu("Scripts/MRTK/Services/ScaleMeshEffect")]
    public class ScaleMeshEffect : BaseMeshEffect
    {
        /// <summary>
        /// Enforces the parent canvas uses UV channel attributes.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            // Make sure the parent canvas is configured to use UV channel attributes for scaling data.
            var canvas = GetComponentInParent<Canvas>();

            if (canvas != null)
            {
                canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord2;
                canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord3;
            }
        }

        /// <summary>
        /// Stores scaling information into UV channel attributes during UI mesh construction.
        /// </summary>
        /// <param name="vh">The vertex helper to populate with new vertex data.</param>
        public override void ModifyMesh(VertexHelper vh)
        {
            var rectTransform = transform as RectTransform;

            // Pack the 2D xy scale into UV channel 2.
            var scale = new Vector2(rectTransform.rect.width * rectTransform.localScale.x,
                                    rectTransform.rect.height * rectTransform.localScale.y);

            var canvas = GetComponentInParent<Canvas>();

            // Pack the z scale into x and a flag indicating this value comes from a ScaleMeshEffect into y into UV channel 3.
            var depth = new Vector2((canvas ? (1.0f / canvas.transform.lossyScale.z) : 1.0f) * rectTransform.localScale.z, 
                                    -1.0f);

            var vertex = new UIVertex();

            for (var i = 0; i < vh.currentVertCount; ++i)
            {
                vh.PopulateUIVertex(ref vertex, i);
                vertex.uv2 = scale;
                vertex.uv3 = depth;
                vh.SetUIVertex(vertex, i);
            }
        }
    }
}
