using System;

namespace UnityEngine.XR.iOS
{
    public class ARPlaneAnchorGameObject
    {
        public GameObject gameObject;
#if UNITY_IOS || UNITY_EDITOR 
        public ARPlaneAnchor planeAnchor;
#endif
    }
}

