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
        /// Converts this System.Numerics Vector3 to a UnityEngine Vector3 format, storing values directly in referenced parameter
        /// </summary>
        public static void ConvertToUnityVector3(this System.Numerics.Vector3 source, ref UnityEngine.Vector3 target)
        {
            target.x = source.X;
            target.y = source.Y;
            target.z = -source.Z;
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

        /// <summary>
        /// Converts this System.Numerics Quaternion to a UnityEngine Quaternion, storing values directly in referenced parameter
        /// </summary>
        public static void ConvertToUnityQuaternion(this System.Numerics.Quaternion source, ref UnityEngine.Quaternion target)
        {
            target.x = -source.X;
            target.y = -source.Y;
            target.z = source.Z;
            target.w = source.W;
        }

    }
}
