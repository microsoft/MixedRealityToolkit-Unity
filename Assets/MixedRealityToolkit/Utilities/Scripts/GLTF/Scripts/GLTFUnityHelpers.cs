using UnityEngine;

namespace UnityGLTF
{
    /// <summary>
    /// Contains methods which help flip a model's coordinate system.
    /// These methods can be used to change a model's coordinate system from
    /// the glTF standard to Unity's coordinate system. Additionally,
    /// methods for flipping the faces of a model and for flipping a
    /// TexCoord's Y axis to match Unity's definition are present.
    /// </summary>
    /// <remarks>
    /// Methods for both UnityEngine.Vector2 and GLTF.Math.Vector2 are provided.
    /// </remarks>
    public static class GLTFUnityHelpers
    {
        /// <summary>
        /// Flips the V component of a TexCoord to convert between glTF and Unity's TexCoord specification.
        /// </summary>
        /// <param name="vector">The TexCoord to be converted.</param>
        /// <returns>The converted TexCoord.</returns>
        public static Vector2 FlipTexCoordV(Vector2 vector)
        {
            return new Vector2(vector.x, 1.0f - vector.y);
        }

        /// <summary>
        /// Flips the V component of a TexCoord to convert between glTF and Unity's TexCoord specification.
        /// </summary>
        /// <param name="vector">The TexCoord to be converted.</param>
        /// <returns>The converted TexCoord.</returns>
        public static GLTF.Math.Vector2 FlipTexCoordV(GLTF.Math.Vector2 vector)
        {
            return new GLTF.Math.Vector2(vector.X, 1.0f - vector.Y);
        }

        /// <summary>
        /// Flips the V component of all TexCoords in an array to convert between glTF and Unity's TexCoord specification.
        /// </summary>
        /// <param name="array">The array of TexCoords to be converted.</param>
        /// <returns>The array of converted TexCoords.</returns>
        public static Vector2[] FlipTexCoordArrayV(Vector2[] array)
        {
            var returnArray = new Vector2[array.Length];

            for (var i = 0; i < array.Length; i++)
            {
                returnArray[i] = FlipTexCoordV(array[i]);
            }

            return returnArray;
        }

        /// <summary>
        /// Flips the V component of all TexCoords in an array to convert between glTF and Unity's TexCoord specification.
        /// </summary>
        /// <param name="array">The array of TexCoords to be converted.</param>
        /// <returns>The array of converted TexCoords.</returns>
        public static GLTF.Math.Vector2[] FlipTexCoordArrayV(GLTF.Math.Vector2[] array)
        {
            var returnArray = new GLTF.Math.Vector2[array.Length];

            for (var i = 0; i < array.Length; i++)
            {
                returnArray[i] = FlipTexCoordV(array[i]);
            }

            return returnArray;
        }

        /// <summary>
        /// Inverts the Z value of a Vector3 to convert between glTF and Unity's coordinate systems.
        /// </summary>
        /// <param name="vector">The Vector3 to be converted.</param>
        /// <returns>The converted Vector3.</returns>
        public static Vector3 FlipVectorHandedness(Vector3 vector)
        {
            return new Vector3(vector.x, vector.y, -vector.z);
        }

        /// <summary>
        /// Inverts the Z value of a Vector3 to convert between glTF and Unity's coordinate systems.
        /// </summary>
        /// <param name="vector">The Vector3 to be converted.</param>
        /// <returns>The converted Vector3.</returns>
        public static GLTF.Math.Vector3 FlipVectorHandedness(GLTF.Math.Vector3 vector)
        {
            return new GLTF.Math.Vector3(vector.X, vector.Y, -vector.Z);
        }

        /// <summary>
        /// Inverts the Z value of all Vector3s in an array to convert between glTF and Unity's coordinate systems.
        /// </summary>
        /// <param name="array">The array of Vector3s to be converted.</param>
        /// <returns>The array of converted Vector3s.</returns>
        public static Vector3[] FlipVectorArrayHandedness(Vector3[] array)
        {
            var returnArray = new Vector3[array.Length];

            for (var i = 0; i < array.Length; i++)
            {
                returnArray[i] = FlipVectorHandedness(array[i]);
            }

            return returnArray;
        }

        /// <summary>
        /// Inverts the Z value of all Vector3s in an array to convert between glTF and Unity's coordinate systems.
        /// </summary>
        /// <param name="array">The array of Vector3s to be converted.</param>
        /// <returns>The array of converted Vector3s.</returns>
        public static GLTF.Math.Vector3[] FlipVectorArrayHandedness(GLTF.Math.Vector3[] array)
        {
            var returnArray = new GLTF.Math.Vector3[array.Length];

            for (var i = 0; i < array.Length; i++)
            {
                returnArray[i] = FlipVectorHandedness(array[i]);
            }

            return returnArray;
        }

        /// <summary>
        /// Inverts the Z and W values of a Vector4 to convert between glTF and Unity's coordinate systems.
        /// </summary>
        /// <param name="vector">The Vector4 to be converted.</param>
        /// <returns>The converted Vector4.</returns>
        public static Vector4 FlipVectorHandedness(Vector4 vector)
        {
            return new Vector4(vector.x, vector.y, -vector.z, -vector.w);
        }

        /// <summary>
        /// Inverts the Z and W values of a Vector4 to convert between glTF and Unity's coordinate systems.
        /// </summary>
        /// <param name="vector">The Vector4 to be converted.</param>
        /// <returns>The converted Vector4.</returns>
        public static GLTF.Math.Vector4 FlipVectorHandedness(GLTF.Math.Vector4 vector)
        {
            return new GLTF.Math.Vector4(vector.X, vector.Y, -vector.Z, -vector.W);
        }

        /// <summary>
        /// Inverts the Z and W values of all Vector4s in an array to convert between glTF and Unity's coordinate systems.
        /// </summary>
        /// <param name="array">The array of Vector4s to be converted.</param>
        /// <returns>The array of converted Vector4s.</returns>
        public static Vector4[] FlipVectorArrayHandedness(Vector4[] array)
        {
            var returnArray = new Vector4[array.Length];

            for (var i = 0; i < array.Length; i++)
            {
                returnArray[i] = FlipVectorHandedness(array[i]);
            }

            return returnArray;
        }

        /// <summary>
        /// Inverts the Z and W values of all Vector4s in an array to convert between glTF and Unity's coordinate systems.
        /// </summary>
        /// <param name="array">The array of Vector4s to be converted.</param>
        /// <returns>The array of converted Vector4s.</returns>
        public static GLTF.Math.Vector4[] FlipVectorArrayHandedness(GLTF.Math.Vector4[] array)
        {
            var returnArray = new GLTF.Math.Vector4[array.Length];

            for (var i = 0; i < array.Length; i++)
            {
                returnArray[i] = FlipVectorHandedness(array[i]);
            }

            return returnArray;
        }

        /// <summary>
        /// Flips the faces of a model by changing the order of the index array.
        /// </summary>
        /// <param name="array">An array of ints, representing the indices to be rotated.</param>
        /// <returns>The flipped array of indices.</returns>
        public static int[] FlipFaces(int[] array)
        {
            var returnArray = new int[array.Length];

            for (int i = 0; i < array.Length; i += 3)
            {
                returnArray[i] = array[i + 2];
                returnArray[i + 1] = array[i + 1];
                returnArray[i + 2] = array[i];
            }

            return returnArray;
        }
    }
}
