// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Core.Utilities.Gltf.Schema
{
    public class GltfProperty
    {
        public readonly Dictionary<string, string> Extensions = new Dictionary<string, string>();
        public readonly Dictionary<string, string> Extras = new Dictionary<string, string>();
    }
}
