// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema
{
    /// <summary>
    /// Image data used to create a texture. Image can be referenced by URI or
    /// `bufferView` index. `mimeType` is required in the latter case.
    /// https://github.com/KhronosGroup/glTF/blob/master/specification/2.0/schema/image.schema.json
    /// </summary>
    [Serializable]
    public class GltfImage : GltfChildOfRootProperty
    {
        #region Serialized Fields

        /// <summary>
        /// The uri of the image.  Relative paths are relative to the .gltf file.
        /// Instead of referencing an external file, the uri can also be a data-uri.
        /// The image format must be jpg, png, bmp, or gif.
        /// </summary>
        public string uri;

        /// <summary>
        /// The image's MIME type.
        /// <minLength>1</minLength>
        /// </summary>
        public string mimeType;

        /// <summary>
        /// The index of the bufferView that contains the image.
        /// Use this instead of the image's uri property.
        /// </summary>
        public int bufferView;

        #endregion Serialized Fields

        /// <summary>
        /// Unity Texture2D wrapper for the GltfImage
        /// </summary>
        public Texture2D Texture { get; internal set; }
    }
}