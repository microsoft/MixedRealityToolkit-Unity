// Copyright(c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit
{
    public static class SystemNumericsExtensions
    {
        /// <summary>
        /// Converts this System.Numerics Vector3 to a UnityEngine Vector3.
        /// </summary>
        /// <param name="vector">The vector to convert.</param>
        /// <returns>The converted vector.</returns>
        public static UnityEngine.Vector3 ToUnityVector3(this System.Numerics.Vector3 vector)
        {
            return new UnityEngine.Vector3(vector.X, vector.Y, -vector.Z);
        }

        /// <summary>
        /// Converts this System.Numerics Quaternion to a UnityEngine Quaternion.
        /// </summary>
        /// <param name="quaternion">The quaternion to convert.</param>
        /// <returns>The converted quaternion.</returns>
        public static UnityEngine.Quaternion ToUnityQuaternion(this System.Numerics.Quaternion quaternion)
        {
            return new UnityEngine.Quaternion(-quaternion.X, -quaternion.Y, quaternion.Z, quaternion.W);
        }

        public static System.Numerics.Matrix4x4 ToSystemNumerics(this UnityEngine.Matrix4x4 m) => new System.Numerics.Matrix4x4(
             m.m00, m.m10, -m.m20, m.m30,
             m.m01, m.m11, -m.m21, m.m31,
            -m.m02, -m.m12, m.m22, -m.m32,
             m.m03, m.m13, -m.m23, m.m33);
    }
}
