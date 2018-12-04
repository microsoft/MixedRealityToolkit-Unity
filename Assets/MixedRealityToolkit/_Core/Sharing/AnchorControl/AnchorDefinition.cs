using System;
using UnityEngine;

namespace Pixie.AnchorControl
{
    [Serializable]
    public struct AnchorDefinition
    {
        public string ID;
        public Vector3 Position;
        public Vector3 Rotation;
    }
}