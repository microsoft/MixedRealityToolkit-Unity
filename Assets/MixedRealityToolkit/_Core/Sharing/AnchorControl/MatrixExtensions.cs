using UnityEngine;

namespace Pixie.AnchorControl
{
    public static class MatrixExtensions
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

        public static Quaternion ExtractRotation(this Matrix4x4 matrix)
        {
            Vector3 forward;
            forward.x = matrix.m02;
            forward.y = matrix.m12;
            forward.z = matrix.m22;

            Vector3 upwards;
            upwards.x = matrix.m01;
            upwards.y = matrix.m11;
            upwards.z = matrix.m21;

            return Quaternion.LookRotation(forward, upwards);
        }

        public static Vector3 ExtractPosition(this Matrix4x4 matrix)
        {
            Vector3 position;
            position.x = matrix.m03;
            position.y = matrix.m13;
            position.z = matrix.m23;
            return position;
        }

        public static Vector3 ExtractScale(this Matrix4x4 matrix)
        {
            Vector3 scale;
            scale.x = new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30).magnitude;
            scale.y = new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31).magnitude;
            scale.z = new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32).magnitude;
            return scale;
        }
    }
}