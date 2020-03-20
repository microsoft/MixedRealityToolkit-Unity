// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema
{
    /// <summary>
    /// Metadata about the glTF asset.
    /// https://github.com/KhronosGroup/glTF/blob/master/specification/2.0/schema/asset.schema.json
    /// </summary>
    [Serializable]
    public class GltfAssetInfo : GltfProperty
    {
        /// <summary>
        /// A copyright message suitable for display to credit the content creator.
        /// </summary>
        public string copyright;

        /// <summary>
        /// Tool that generated this glTF model. Useful for debugging.
        /// </summary>
        public string generator;

        /// <summary>
        /// The glTF version.
        /// </summary>
        public string version;

        /// <summary>
        /// The minimum glTF version that this asset targets.
        /// </summary>
        public string minVersion;
    }
}