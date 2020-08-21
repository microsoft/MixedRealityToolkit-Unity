// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema
{
    /// <summary>
    /// Common mesh primitive attributes.
    /// https://github.com/KhronosGroup/glTF/blob/master/specification/2.0/schema/mesh.primitive.schema.json
    /// </summary>
    public class GltfMeshPrimitiveAttributes : ReadOnlyDictionary<string, int>
    {
        private const string POSITION_KEY = "POSITION";
        private const string NORMAL_KEY = "NORMAL";
        private const string TANGENT_KEY = "TANGENT";
        private const string TEXCOORD_0_KEY = "TEXCOORD_0";
        private const string TEXCOORD_1_KEY = "TEXCOORD_1";
        private const string TEXCOORD_2_KEY = "TEXCOORD_2";
        private const string TEXCOORD_3_KEY = "TEXCOORD_3";
        private const string COLOR_0_KEY = "COLOR_0";
        private const string JOINTS_0_KEY = "JOINTS_0";
        private const string WEIGHTS_0_KEY = "WEIGHTS_0";

        public GltfMeshPrimitiveAttributes(IDictionary<string, int> dictionary) : base(dictionary)
        {
        }

        private int TryGetDefault(string key, int defaultVal)
        {
            int index;
            return TryGetValue(key, out index) ? index : defaultVal;
        }

        public int POSITION
        {
            get
            {
                return TryGetDefault(POSITION_KEY, -1);
            }
        }

        public int NORMAL
        {
            get
            {
                return TryGetDefault(NORMAL_KEY, -1);
            }
        }

        public int TEXCOORD_0
        {
            get
            {
                return TryGetDefault(TEXCOORD_0_KEY, -1);
            }
        }

        public int TEXCOORD_1
        {
            get
            {
                return TryGetDefault(TEXCOORD_1_KEY, -1);
            }
        }

        public int TEXCOORD_2
        {
            get
            {
                return TryGetDefault(TEXCOORD_2_KEY, -1);
            }
        }

        public int TEXCOORD_3
        {
            get
            {
                return TryGetDefault(TEXCOORD_3_KEY, -1);
            }
        }

        public int COLOR_0
        {
            get
            {
                return TryGetDefault(COLOR_0_KEY, -1);
            }
        }

        public int TANGENT
        {
            get
            {
                return TryGetDefault(TANGENT_KEY, -1);
            }
        }
        public int WEIGHTS_0
        {
            get
            {
                return TryGetDefault(WEIGHTS_0_KEY, -1);
            }
        }

        public int JOINTS_0
        {
            get
            {
                return TryGetDefault(JOINTS_0_KEY, -1);
            }
        }
    }
}