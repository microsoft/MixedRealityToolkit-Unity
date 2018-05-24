#if UNITY_IOS || UNITY_EDITOR
using System;

namespace UnityEngine.XR.iOS
{
    public struct ARPlaneAnchor 
    {

        public string identifier;

        /**
         The transformation matrix that defines the anchor's rotation, translation and scale in world coordinates.
         */
        public Matrix4x4 transform;

        /**
         The alignment of the plane.
         */

        public ARPlaneAnchorAlignment alignment;

        /**
        The center of the plane in the anchor’s coordinate space.
        */

        public Vector3 center;

        /**
        The extent of the plane in the anchor’s coordinate space.
         */
        public Vector3 extent;


    }
}
#endif
