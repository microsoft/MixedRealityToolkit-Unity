// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema
{
    /// <summary>
    /// The material appearance of a primitive.
    /// https://github.com/KhronosGroup/glTF/tree/master/specification/2.0/schema
    /// </summary>
    [Serializable]
    public class GltfMaterial : GltfChildOfRootProperty
    {
        /// <summary>
        /// A set of parameter values that are used to define the metallic-roughness
        /// material model from Physically-Based Rendering (PBR) methodology.
        /// </summary>
        public GltfPbrMetallicRoughness pbrMetallicRoughness;

        /// <summary>
        /// A set of parameter values used to light flat-shaded materials
        /// </summary>
        public GltfMaterialCommonConstant commonConstant;

        /// <summary>
        /// A tangent space normal map. Each texel represents the XYZ components of a
        /// normal vector in tangent space.
        /// </summary>
        public GltfNormalTextureInfo normalTexture;

        /// <summary>
        /// The occlusion map is a greyscale texture, with white indicating areas that
        /// should receive full indirect lighting and black indicating no indirect
        /// lighting.
        /// </summary>
        public GltfOcclusionTextureInfo occlusionTexture;

        /// <summary>
        /// The emissive map controls the color and intensity of the light being emitted
        /// by the material. This texture contains RGB components in sRGB color space.
        /// If a fourth component (A) is present, it is ignored.
        /// </summary>
        public GltfTextureInfo emissiveTexture;

        /// <summary>
        /// The RGB components of the emissive color of the material.
        /// If an emissiveTexture is specified, this value is multiplied with the texel
        /// values.
        /// <items>
        ///	 <minimum>0.0</minimum>
        ///	 <maximum>1.0</maximum>
        /// </items>
        /// <minItems>3</minItems>
        /// <maxItems>3</maxItems>
        /// </summary>
        public float[] emissiveFactor = { 0f, 0f, 0f, 0f };

        /// <summary>
        /// The material's alpha rendering mode enumeration specifying the interpretation of the
        /// alpha value of the main factor and texture. In `OPAQUE` mode, the alpha value is
        /// ignored and the rendered output is fully opaque. In `MASK` mode, the rendered output
        /// is either fully opaque or fully transparent depending on the alpha value and the
        /// specified alpha cutoff value. In `BLEND` mode, the alpha value is used to composite
        /// the source and destination areas. The rendered output is combined with the background
        /// using the normal painting operation (i.e. the Porter and Duff over operator).
        /// </summary>
        public string alphaMode;

        /// <summary>
        /// Specifies the cutoff threshold when in `MASK` mode. If the alpha value is greater than
        /// or equal to this value then it is rendered as fully opaque, otherwise, it is rendered
        /// as fully transparent. This value is ignored for other modes.
        /// </summary>
        public double alphaCutoff = 0.5f;

        /// <summary>
        /// Specifies whether the material is double sided. When this value is false, back-face
        /// culling is enabled. When this value is true, back-face culling is disabled and double
        /// sided lighting is enabled. The back-face must have its normals reversed before the
        /// lighting equation is evaluated.
        /// </summary>
        public bool doubleSided;

        /// <summary>
        /// Unity Material wrapper for the GltfMaterial
        /// </summary>
        public Material Material { get; internal set; }
    }
}