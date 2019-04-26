// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema
{
    /// <summary>
    /// https://github.com/KhronosGroup/glTF/blob/master/specification/2.0/schema/glTFChildOfRootProperty.schema.json
    /// </summary>
    [Serializable]
    public class GltfChildOfRootProperty : GltfProperty
    {
        /// <summary>
        /// The user-defined name of this object.
        /// This is not necessarily unique, e.g., an accessor and a buffer could have the same name,
        /// or two accessors could even have the same name.
        /// </summary>
        public string name;
    }
}