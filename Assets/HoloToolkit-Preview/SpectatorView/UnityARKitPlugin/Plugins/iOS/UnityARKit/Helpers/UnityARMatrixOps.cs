using System;

namespace UnityEngine.XR.iOS
{
    public class UnityARMatrixOps
    {

        public static Vector3 GetPosition(Matrix4x4 matrix)
        {
            // Convert from ARKit's right-handed coordinate
            // system to Unity's left-handed
            Vector3 position = matrix.GetColumn(3);
            position.z = -position.z;

            return position;
        }

        public static Quaternion GetRotation(Matrix4x4 matrix)
        {
            // Convert from ARKit's right-handed coordinate
            // system to Unity's left-handed
            Quaternion rotation = QuaternionFromMatrix(matrix);
            rotation.z = -rotation.z;
            rotation.w = -rotation.w;

            return rotation;
        }


        static Quaternion QuaternionFromMatrix(Matrix4x4 m) {
            // Adapted from: http://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaternion/index.htm
            Quaternion q = new Quaternion();
            q.w = Mathf.Sqrt( Mathf.Max( 0, 1 + m[0,0] + m[1,1] + m[2,2] ) ) / 2; 
            q.x = Mathf.Sqrt( Mathf.Max( 0, 1 + m[0,0] - m[1,1] - m[2,2] ) ) / 2; 
            q.y = Mathf.Sqrt( Mathf.Max( 0, 1 - m[0,0] + m[1,1] - m[2,2] ) ) / 2; 
            q.z = Mathf.Sqrt( Mathf.Max( 0, 1 - m[0,0] - m[1,1] + m[2,2] ) ) / 2; 
            q.x *= Mathf.Sign( q.x * ( m[2,1] - m[1,2] ) );
            q.y *= Mathf.Sign( q.y * ( m[0,2] - m[2,0] ) );
            q.z *= Mathf.Sign( q.z * ( m[1,0] - m[0,1] ) );
            return q;
        }

    }
}

