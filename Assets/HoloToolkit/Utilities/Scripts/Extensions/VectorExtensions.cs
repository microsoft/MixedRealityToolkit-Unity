// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// A collection of useful extension methods for Unity's Vector structs.
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
            int count = vectors.Count();
            if (count == 0)
            {
                return Vector2.zero;
            }

            return vectors.OrderBy(v => v.sqrMagnitude).ElementAt(count / 2);
        }

        public static Vector3 Median(this IEnumerable<Vector3> vectors)
        {
            int count = vectors.Count();
            if (count == 0)
            {
                return Vector3.zero;
            }

            return vectors.OrderBy(v => v.sqrMagnitude).ElementAt(count / 2);
        }

        public static Vector2 Median(this ICollection<Vector2> vectors)
        {
            int count = vectors.Count;
            if (count == 0)
            {
                return Vector2.zero;
            }

            return vectors.OrderBy(v => v.sqrMagnitude).ElementAt(count / 2);
        }

        public static Vector3 Median(this ICollection<Vector3> vectors)
        {
            int count = vectors.Count;
            if (count == 0)
            {
                return Vector3.zero;
            }

            return vectors.OrderBy(v => v.sqrMagnitude).ElementAt(count / 2);
        }
    }
}
