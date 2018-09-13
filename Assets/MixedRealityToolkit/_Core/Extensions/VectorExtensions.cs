// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Extensions
{
    /// <summary>
    /// Extension methods for Unity's Vector struct
    /// </summary>
    public static class VectorExtensions
    {
        public static Vector2 Mul(this Vector2 value, Vector2 scale)
        {
            return new Vector2(value.x * scale.x, value.y * scale.y);
        }

        public static Vector2 Div(this Vector2 value, Vector2 scale)
        {
            return new Vector2(value.x / scale.x, value.y / scale.y);
        }

        public static Vector3 Mul(this Vector3 value, Vector3 scale)
        {
            return new Vector3(value.x * scale.x, value.y * scale.y, value.z * scale.z);
        }

        public static Vector3 Div(this Vector3 value, Vector3 scale)
        {
            return new Vector3(value.x / scale.x, value.y / scale.y, value.z / scale.z);
        }

        public static Vector3 RotateAround(this Vector3 point, Vector3 pivot, Quaternion rotation)
        {
            return rotation * (point - pivot) + pivot;
        }

        public static Vector3 RotateAround(this Vector3 point, Vector3 pivot, Vector3 eulerAngles)
        {
            return RotateAround(point, pivot, Quaternion.Euler(eulerAngles));
        }

        public static Vector3 TransformPoint(this Vector3 point, Vector3 translation, Quaternion rotation, Vector3 lossyScale)
        {
            return rotation * Vector3.Scale(lossyScale, point) + translation;
        }

        public static Vector3 InverseTransformPoint(this Vector3 point, Vector3 translation, Quaternion rotation, Vector3 lossyScale)
        {
            var scaleInv = new Vector3(1 / lossyScale.x, 1 / lossyScale.y, 1 / lossyScale.z);
            return Vector3.Scale(scaleInv, (Quaternion.Inverse(rotation) * (point - translation)));
        }

        public static Vector2 Average(this IEnumerable<Vector2> vectors)
        {
            float x = 0f;
            float y = 0f;
            int count = 0;

            foreach (var pos in vectors)
            {
                x += pos.x;
                y += pos.y;
                count++;
            }

            return new Vector2(x / count, y / count);
        }

        public static Vector3 Average(this IEnumerable<Vector3> vectors)
        {
            float x = 0f;
            float y = 0f;
            float z = 0f;
            int count = 0;

            foreach (var pos in vectors)
            {
                x += pos.x;
                y += pos.y;
                z += pos.z;
                count++;
            }

            return new Vector3(x / count, y / count, z / count);
        }

        public static Vector2 Average(this ICollection<Vector2> vectors)
        {
            int count = vectors.Count;
            if (count == 0)
            {
                return Vector2.zero;
            }

            float x = 0f;
            float y = 0f;

            foreach (var pos in vectors)
            {
                x += pos.x;
                y += pos.y;
            }

            return new Vector2(x / count, y / count);
        }

        public static Vector3 Average(this ICollection<Vector3> vectors)
        {
            int count = vectors.Count;

            if (count == 0)
            {
                return Vector3.zero;
            }

            float x = 0f;
            float y = 0f;
            float z = 0f;

            foreach (var pos in vectors)
            {
                x += pos.x;
                y += pos.y;
                z += pos.z;
            }

            return new Vector3(x / count, y / count, z / count);
        }

        public static Vector2 Median(this IEnumerable<Vector2> vectors)
        {
            var enumerable = vectors as Vector2[] ?? vectors.ToArray();
            int count = enumerable.Length;
            return count == 0 ? Vector2.zero : enumerable.OrderBy(v => v.sqrMagnitude).ElementAt(count / 2);
        }

        public static Vector3 Median(this IEnumerable<Vector3> vectors)
        {
            var enumerable = vectors as Vector3[] ?? vectors.ToArray();
            int count = enumerable.Length;
            return count == 0 ? Vector3.zero : enumerable.OrderBy(v => v.sqrMagnitude).ElementAt(count / 2);
        }

        public static Vector2 Median(this ICollection<Vector2> vectors)
        {
            int count = vectors.Count;
            return count == 0 ? Vector2.zero : vectors.OrderBy(v => v.sqrMagnitude).ElementAt(count / 2);
        }

        public static Vector3 Median(this ICollection<Vector3> vectors)
        {
            int count = vectors.Count;
            return count == 0 ? Vector3.zero : vectors.OrderBy(v => v.sqrMagnitude).ElementAt(count / 2);
        }

        public static bool IsValidVector(this Vector3 vector)
        {
            return !float.IsNaN(vector.x) && !float.IsNaN(vector.y) && !float.IsNaN(vector.z) &&
                   !float.IsInfinity(vector.x) && !float.IsInfinity(vector.y) && !float.IsInfinity(vector.z);
        }

        /// <summary>
        /// Get the relative mapping based on a source Vec3 and a radius for spherical mapping.
        /// </summary>
        /// <param name="source">The source <see cref="Vector3"/> to be mapped to sphere</param>
        /// <param name="radius">This is a <see cref="float"/> for the radius of the sphere</param>
        /// <returns></returns>
        public static Vector3 SphericalMapping(Vector3 source, float radius)
        {
            float circ = 2f * Mathf.PI * radius;

            float xAngle = (source.x / circ) * 360f;
            float yAngle = -(source.y / circ) * 360f;

            source.Set(0.0f, 0.0f, radius);

            Quaternion rot = Quaternion.Euler(yAngle, xAngle, 0.0f);
            source = rot * source;

            return source;
        }

        /// <summary>
        /// Get the relative mapping based on a source Vec3 and a radius for cylinder mapping.
        /// </summary>
        /// <param name="source">The source <see cref="Vector3"/> to be mapped to cylinder</param>
        /// <param name="radius">This is a <see cref="float"/> for the radius of the cylinder</param>
        /// <returns></returns>
        public static Vector3 CylindricalMapping(Vector3 source, float radius)
        {
            float circ  = 2f * Mathf.PI * radius;

            float xAngle = (source.x / circ) * 360f;

            source.Set(0.0f, source.y, radius);

            Quaternion rot = Quaternion.Euler(0.0f, xAngle, 0.0f);
            source = rot * source;

            return source;
        }

        /// <summary>
        /// Get the relative mapping based on a source Vec3 and a radius for radial mapping.
        /// </summary>
        /// <param name="source">The source <see cref="Vector3"/> to be mapped to cylinder</param>
        /// <param name="radialRange">The total range of the radial in degrees as a <see cref="float"/></param>
        /// <param name="radius">This is a <see cref="float"/> for the radius of the radial</param>
        /// <param name="row">The current row as a <see cref="int"/> for the radial calculation</param>
        /// <param name="totalRow">The total rows as a <see cref="int"/> for the radial calculation</param>
        /// <param name="column">The current column as a <see cref="int"/> for the radial calculation</param>
        /// <param name="totalColumn">The total columns as a <see cref="int"/> for the radial calculation</param>
        /// <returns></returns>
        public static Vector3 RadialMapping(Vector3 source, float radialRange, float radius, int row, int totalRows, int column, int totalColumns)
        {
            float radialCellAngle = radialRange / totalColumns;

            source.x = 0f;
            source.y = 0f;
            source.z = (radius / totalRows) * row;

            float yAngle = radialCellAngle * (column - (totalColumns * 0.5f)) + (radialCellAngle * .5f);

            Quaternion rot = Quaternion.Euler(0.0f, yAngle, 0.0f);
            source = rot * source;

            return source;
        }

        /// <summary>
        /// Randomized mapping based on a source Vec3 and a radius for randomization distance.
        /// </summary>
        /// <param name="source">The source <see cref="Vector3"/> to be mapped to cylinder</param>
        /// <param name="radius">This is a <see cref="float"/> for the radius of the cylinder</param>
        /// <returns></returns>
        public static Vector3 ScatterMapping(Vector3 source, float radius)
        {
            source.x = UnityEngine.Random.Range(-radius, radius);
            source.y = UnityEngine.Random.Range(-radius, radius);
            return source;
        }

    }
}
