#if UNITY_IOS || UNITY_EDITOR
using System;

namespace UnityEngine.XR.iOS
{
    public struct ARFrame
    {
        /**
         A timestamp identifying the frame.
         */
        public double timestamp;

        /**
         The frame's captured image.
         */
        public IntPtr capturedImage;

        /**
         The camera used to capture the frame's image.
         @discussion The camera provides the device's position and orientation as well as camera parameters.
         */
        public ARCamera camera;

        /**
         A list of anchors in the scene.
         */
        //List<ARAnchor> anchors;

        /**
         A light estimate representing the light in the scene.
         @discussion Returns nil if there is no light estimation.
         */
        ARLightEstimate lightEstimate;

    }
}
#endif
