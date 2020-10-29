// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema
{
    /// <summary>
    /// Texture sampler properties for filtering and wrapping modes.
    /// https://github.com/KhronosGroup/glTF/blob/master/specification/2.0/schema/sampler.schema.json
    /// </summary>
    [Serializable]
    public class GltfSampler : GltfChildOfRootProperty, ISerializationCallbackReceiver
    {
        /// <summary>
        /// Magnification filter.
        /// Valid values correspond to WebGL enums: `9728` (NEAREST) and `9729` (LINEAR).
        /// </summary>
        [NonSerialized]
        public GltfMagnificationFilterMode MagFilter;

        [SerializeField]
        private string magFilter = null;

        /// <summary>
        /// Minification filter. All valid values correspond to WebGL enums.
        /// </summary>
        [NonSerialized]
        public GltfMinFilterMode MinFilter;

        [SerializeField]
        private string minFilter = null;

        /// <summary>
        /// s wrapping mode.  All valid values correspond to WebGL enums.
        /// </summary>
        [NonSerialized]
        public GltfWrapMode WrapS;

        [SerializeField]
        private string wrapS = null;

        /// <summary>
        /// t wrapping mode.  All valid values correspond to WebGL enums.
        /// </summary>
        [NonSerialized]
        public GltfWrapMode WrapT;

        [SerializeField]
        private string wrapT = null;

        public void OnAfterDeserialize()
        {
            if (Enum.TryParse(magFilter, out GltfMagnificationFilterMode result))
            {
                MagFilter = result;
            }
            else
            {
                MagFilter = GltfMagnificationFilterMode.Linear;
            }
            if (Enum.TryParse(minFilter, out GltfMinFilterMode result2))
            {
                MinFilter = result2;
            }
            else
            {
                MinFilter = GltfMinFilterMode.NearestMipmapLinear;
            }
            if (Enum.TryParse(wrapT, out GltfWrapMode result3))
            {
                WrapT = result3;
            }
            else
            {
                WrapT = GltfWrapMode.Repeat;
            }
            if (Enum.TryParse(wrapS, out GltfWrapMode result4))
            {
                WrapS = result4;
            }
            else
            {
                WrapS = GltfWrapMode.Repeat;
            }
        }

        public void OnBeforeSerialize()
        {

        }
    }
}