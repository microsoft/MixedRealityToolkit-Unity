using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.AnchorControl
{
    public static class FromMatrixExtension
    {
        public static Vector3 GetVector3(this Matrix4x4 matrix)
        {
            return matrix.GetColumn(3);
        }

        public static Quaternion GetQuaternion(this Matrix4x4 matrix)
        {
            if (matrix.GetColumn(2) == matrix.GetColumn(1)) { return Quaternion.identity; }
            return Quaternion.LookRotation(matrix.GetColumn(2), matrix.GetColumn(1));
        }

        public static void FillMatrixFromQuaternion(this Quaternion rotation, ref Vector3[] covariance)
        {
            covariance[0] = rotation * Vector3.right;
            covariance[1] = rotation * Vector3.up;
            covariance[2] = rotation * Vector3.forward;
        }

        public static Matrix4x4 Lerp(Matrix4x4 from, Matrix4x4 to, float amount)
        {
            return Matrix4x4.TRS(Vector3.Lerp(from.GetVector3(), to.GetVector3(), amount), Quaternion.Slerp(from.GetQuaternion(), to.GetQuaternion(), amount), Vector3.one);
        }
    }
}