#if UNITY_IOS || UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.XR.iOS
{
    public struct ARCamera
    {
        /**
         The transformation matrix that defines the camera's rotation and translation in world coordinates.
         */

        public Matrix4x4 worldTransform;

        /**
         The camera's orientation defined as Euler angles.
         
         @dicussion The order of components in this vector matches the axes of rotation:
                       1. Pitch (the x component) is the rotation about the node's x-axis (in radians)
                       2. Yaw   (the y component) is the rotation about the node's y-axis (in radians)
                       3. Roll  (the z component) is the rotation about the node's z-axis (in radians)
                    ARKit applies these rotations in the reverse order of the components:
                       1. first roll
                       2. then yaw
                       3. then pitch
         */

        public Vector3 eulerAngles;

        public ARTrackingQuality trackingQuality;

        /**
         The camera intrinsics.
         @discussion The matrix has the following contents:
         fx 0   px
         0  fy  py
         0  0   1
         fx and fy are the focal length in pixels.
         px and py are the coordinates of the principal point in pixels.
         The origin is at the center of the upper-left pixel.
         */

        public Vector3 intrinsics_row1;
        public Vector3 intrinsics_row2;
        public Vector3 intrinsics_row3;

        public ARSize imageResolution;
    
    }
}
#endif
