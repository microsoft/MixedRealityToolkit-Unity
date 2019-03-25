// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema.Extensions
{
    [Serializable]
    public class KHR_Materials_PbrSpecularGlossiness : GltfExtension
    {
        public float[] diffuseFactor;
        public GltfTextureInfo diffuseTexture;
        public float[] specularFactor;
        public float glossinessFactor;
        public GltfTextureInfo specularGlossinessTexture;
    }
}
