using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.AnchorControl
{
    [Serializable]
    public struct AnchorDefinition
    {
        public string ID;
        public Vector3 Position;
        public Vector3 Rotation;
    }
}