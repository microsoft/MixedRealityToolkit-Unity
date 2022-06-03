// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.Input
{
    [Serializable]
    public struct InteractionMode
    {
        public string name;
        [FormerlySerializedAs("id")]
        public int priority;

        public override string ToString()
        {
            return name;
        }
    }
}
