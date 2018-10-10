using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Patterns.Proximity
{
    [Serializable]
    public struct TargetIndicatorInfo
    {
        public Transform Target;
        public float Distance;
        public Vector3 LocalDirection;
        public bool Active;
    }
}
