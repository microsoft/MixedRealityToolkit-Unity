// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal struct MaterialPropertyKey
    {
        public string ShaderName;
        public string PropertyName;

        public MaterialPropertyKey(string shaderName, string propertyName)
        {
            this.ShaderName = shaderName;
            this.PropertyName = propertyName;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is MaterialPropertyKey))
            {
                return false;
            }

            MaterialPropertyKey other = (MaterialPropertyKey)obj;
            return other.ShaderName == ShaderName && other.PropertyName == PropertyName;
        }

        public override int GetHashCode()
        {
            return ShaderName.GetHashCode() ^ PropertyName.GetHashCode();
        }
    }
}