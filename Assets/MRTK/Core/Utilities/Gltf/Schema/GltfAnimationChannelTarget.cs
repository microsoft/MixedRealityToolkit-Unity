// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema
{
    /// <summary>
    /// The index of the node and TRS property that an animation channel targets.
    /// https://github.com/KhronosGroup/glTF/blob/master/specification/2.0/schema/animation.channel.target.schema.json
    /// </summary>
    [Serializable]
    public class GltfAnimationChannelTarget : GltfProperty, ISerializationCallbackReceiver
    {
        /// <summary>
        /// The index of the node to target.
        /// </summary>
        public int node = -1;

        /// <summary>
        /// The name of the node's TRS property to modify.
        /// </summary>
        [NonSerialized]
        public GltfAnimationChannelPath Path;

        [SerializeField]
        private string path = null;

        public void OnAfterDeserialize()
        {
            if (Enum.TryParse(path, out GltfAnimationChannelPath result))
            {
                Path = result;
            }
            else
            {
                Path = default;
            }
        }

        public void OnBeforeSerialize()
        {

        }
    }
}