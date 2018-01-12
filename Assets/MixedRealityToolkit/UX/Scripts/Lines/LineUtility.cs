// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;

namespace MixedRealityToolkit.UX.Lines
{
    public enum InterpolationEnum
    {
        Bezeir,
        CatmullRom,
        Hermite,
    }

    /// <summary>
    /// Default options for getting a rotation along a line
    /// </summary>
    public enum RotationTypeEnum
    {
        None,                           // Don't rotate
        Velocity,                       // Use velocity
        RelativeToOrigin,               // Rotate relative to direction from origin point
    }

    /// <summary>
    /// Default options for getting an interpolated point along a line
    /// </summary>
    public enum PointDistributionTypeEnum
    {
        None,                       // Don't adjust placement
        Auto,                       // Adjust placement automatically (default)
        DistanceSingleValue,        // Place based on distance
        DistanceCurveValue,         // Place based on curve
    }

    /// <summary>
    /// Default options for how to generate points in a line renderer
    /// </summary>
    public enum StepModeEnum
    {
        Interpolated,   // Draw points based on NumLineSteps
        FromSource,     // Draw only the points available in the source - use this for hard edges
    }

    /// <summary>
    /// Default options for how to distribute interpolated points in a line renderer
    /// </summary>
    public enum InterpolationModeEnum
    {
        FromNumSteps,   // Specify the number of interpolation steps manually
        FromLength,     // Create steps based on total length of line + manually specified length
        FromCurve       // Create steps based on total length of line + animation curve
    }

    public enum DistortionTypeEnum
    {
        NormalizedLength,   // Use the normalized length of the line plus its distortion strength curve to determine distortion strength
        Uniform,            // Use a single value to determine distortion strength
    }

    public static class LineUtils
    {
        #region offset and rotation functions

        public static readonly Vector3 DefaultUpVector = Vector3.up;

        /// <summary>
        /// Returns normalized length for OffsetModeEnum.Manual
        /// </summary>
        /// <param name="step"></param>
        /// <param name="offsetValue"></param>
        /// <param name="repeat"></param>
        /// <returns></returns>
        public static float GetDistanceSingleValue(int step, float offsetValue, bool repeat)
        {
            float value = step * offsetValue;
            return repeat ? Mathf.Repeat(value, 1f) : Mathf.Clamp01(value);
        }

        /// <summary>
        /// Returns normalized length for OffsetModeEnum.CurveNormalized
        /// </summary>
        /// <param name="step"></param>
        /// <param name="numSteps"></param>
        /// <param name="offsetCurve"></param>
        /// <param name="repeat"></param>
        /// <returns></returns>
        public static float GetDistanceCurveValue(int step, int numSteps, AnimationCurve offsetCurve, bool repeat)
        {
            float value = offsetCurve.Evaluate((float)step / numSteps);
            return repeat ? Mathf.Repeat(value, 1f) : Mathf.Clamp01(value);
        }

        /// <summary>
        /// Returns a blended value from a collection of vectors
        /// </summary>
        /// <param name="source"></param>
        /// <param name="normalizedLength"></param>
        /// <returns></returns>
        public static Vector3 GetVectorCollectionBlend(Vector3[] vectorCollection, float normalizedLength, bool repeat)
        {
            if (vectorCollection.Length == 0)
            {
                return Vector3.zero;
            }
            else if (vectorCollection.Length == 1)
            {
                return vectorCollection[0];
            }

            float arrayValueLength = 1f / vectorCollection.Length;
            int indexA = Mathf.FloorToInt(normalizedLength * vectorCollection.Length);
            if (indexA >= vectorCollection.Length)
            {
                indexA = repeat ? 0 : vectorCollection.Length - 1;
            }

            int indexB = indexA + 1;
            if (indexB >= vectorCollection.Length)
            {
                indexB = repeat ? 0 : vectorCollection.Length - 1;
            }

            float blendAmount = (normalizedLength - (arrayValueLength * indexA)) / arrayValueLength;
            Vector3 blendedVector = Vector3.Lerp(vectorCollection[indexA], vectorCollection[indexB], blendAmount);
            return blendedVector;
        }

        public static float ClosestTo(this IEnumerable<float> collection, float target)
        {
            // NB Method will return float.MaxValue for a sequence containing no elements.
            // Apply any defensive coding here as necessary. 
            var closest = float.MaxValue;
            var minDifference = float.MaxValue;
            foreach (var element in collection)
            {
                var difference = Mathf.Abs(element - target);
                if (minDifference > difference)
                {
                    minDifference = difference;
                    closest = element;
                }
            }

            return closest;
        }

        #endregion

        #region line drawing functions

        public static Vector3 GetPointAlongParabola(Vector3 start, Vector3 end, Vector3 up, float height, float normalizedLength)
        {
            float parabolaTime = normalizedLength * 2 - 1;
            Vector3 direction = end - start;
            Vector3 pos = start + normalizedLength * direction;
            pos += ((-parabolaTime * parabolaTime + 1) * height) * up.normalized;
            return pos;
        }

        public static Vector3 GetPointAlongSpline(SplinePoint[] points, float normalizedLength, InterpolationEnum interpolation = InterpolationEnum.Bezeir)
        {
            int pointIndex = (Mathf.RoundToInt(normalizedLength * points.Length));
            float subnormalizedLength = normalizedLength - ((float)pointIndex / points.Length);

            if (pointIndex + 3 >= points.Length)
            {
                return points[points.Length - 1].Point;
            }
            if (pointIndex < 0)
            {
                return points[0].Point;
            }

            Vector3 point1 = points[pointIndex].Point;
            Vector3 point2 = points[pointIndex + 1].Point;
            Vector3 point3 = points[pointIndex + 2].Point;
            Vector3 point4 = points[pointIndex + 3].Point;

            switch (interpolation)
            {
                case InterpolationEnum.Bezeir:
                default:
                    return InterpolateBezeirPoints(point1, point2, point3, point4, subnormalizedLength);

                case InterpolationEnum.CatmullRom:
                    return InterpolateCatmullRomPoints(point1, point2, point3, point4, subnormalizedLength);
            }
        }

        public static Vector3 InterpolateVectorArray(Vector3[] points, float normalizedLength)
        {
            float arrayValueLength = 1f / points.Length;
            int indexA = Mathf.FloorToInt(normalizedLength * points.Length);
            if (indexA >= points.Length)
            {
                indexA = 0;
            }

            int indexB = indexA + 1;
            if (indexB >= points.Length)
            {
                indexB = 0;
            }

            float blendAmount = (normalizedLength - (arrayValueLength * indexA)) / arrayValueLength;

            return Vector3.Lerp(points[indexA], points[indexB], blendAmount);
        }

        public static Vector3 InterpolateCatmullRomPoints(Vector3 point1, Vector3 point2, Vector3 point3, Vector3 point4, float normalizedLength)
        {
            Vector3 p1 = 2f * point2;
            Vector3 p2 = point3 - point1;
            Vector3 p3 = 2f * point1 - 5f * point2 + 4f * point3 - point4;
            Vector3 p4 = -point1 + 3f * point2 - 3f * point3 + point4;

            Vector3 point = 0.5f * (p1 + (p2 * normalizedLength) + (p3 * normalizedLength * normalizedLength) + (p4 * normalizedLength * normalizedLength * normalizedLength));

            return point;
        }

        public static Vector3 InterpolateBezeirPoints(Vector3 point1, Vector3 point2, Vector3 point3, Vector3 point4, float normalizedLength)
        {
            float invertedDistance = 1f - normalizedLength;
            return
                invertedDistance * invertedDistance * invertedDistance * point1 +
                3f * invertedDistance * invertedDistance * normalizedLength * point2 +
                3f * invertedDistance * normalizedLength * normalizedLength * point3 +
                normalizedLength * normalizedLength * normalizedLength * point4;
        }

        public static Vector3 InterpolateHermitePoints(Vector3 point1, Vector3 point2, Vector3 point3, Vector3 point4, float normalizedLength)
        {
            float invertedDistance = 1f - normalizedLength;
            return
                invertedDistance * invertedDistance * invertedDistance * point1 +
                3f * invertedDistance * invertedDistance * normalizedLength * point2 +
                3f * invertedDistance * normalizedLength * normalizedLength * point3 +
                normalizedLength * normalizedLength * normalizedLength * point4;
        }

        public static Vector3 GetEllipsePoint(float radiusX, float radiusY, float angle)
        {
            return new Vector3(radiusX * Mathf.Cos(angle), radiusY * Mathf.Sin(angle), 0.0f);
        }

        #endregion
    }
}