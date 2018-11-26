#if UNITY_IOS || UNITY_EDITOR
using System;

namespace UnityEngine.XR.iOS
{
    public struct ARUserAnchor 
    {

        public string identifier;

        /**
         The transformation matrix that defines the anchor's rotation, translation and scale in world coordinates.
         */
        public Matrix4x4 transform;
    }
}
#endif
