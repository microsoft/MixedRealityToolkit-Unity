// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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
