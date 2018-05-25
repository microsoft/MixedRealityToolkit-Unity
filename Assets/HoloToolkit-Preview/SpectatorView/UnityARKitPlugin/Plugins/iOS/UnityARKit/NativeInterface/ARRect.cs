#if UNITY_IOS || UNITY_EDITOR
using System;

namespace UnityEngine.XR.iOS
{
    public struct ARRect
    {
        public ARPoint origin;
        public ARSize  size;
    }
}
#endif
