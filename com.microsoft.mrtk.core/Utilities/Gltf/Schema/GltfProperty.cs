﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema
{
    public class GltfProperty
    {
        /// <summary>
        /// https://github.com/KhronosGroup/glTF/blob/master/specification/2.0/schema/extension.schema.json
        /// </summary>
        public readonly Dictionary<string, string> Extensions = new Dictionary<string, string>();

        /// <summary>
        /// https://github.com/KhronosGroup/glTF/blob/master/specification/2.0/schema/extras.schema.json
        /// </summary>
        public readonly Dictionary<string, string> Extras = new Dictionary<string, string>();
    }
}
