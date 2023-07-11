﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Mixed Reality line utility class with helpful math functions for calculation, and other convenience methods.
    /// </summary>
    internal static class LineUtility
    {
        /// <summary>
        /// Inverts the color
        /// </summary>
        public static Color Invert(this Color color)
        {
            color.r = 1.0f - color.r;
            color.g = 1.0f - color.g;
            color.b = 1.0f - color.b;
            return color;
        }

        /// <summary>
        /// Returns a blended value from a collection of vectors
        /// </summary>
        /// <param name="vectorCollection">The collection to use to calculate the blend.</param>
        /// <param name="normalizedLength">the normalized length along the line to calculate the point.</param>
        /// <returns>The calculated point found along the normalized length.</returns>
        public static Vector3 GetVectorCollectionBlend(Vector3[] vectorCollection, float normalizedLength, bool repeat)
        {
            if (vectorCollection.Length == 0)
            {
                return Vector3.zero;
            }

            if (vectorCollection.Length == 1)
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
            return Vector3.Lerp(vectorCollection[indexA], vectorCollection[indexB], blendAmount);
        }

        /// <summary>
        /// Gets the point along a physics based parabola.
        /// </summary>
        /// <param name="origin">The point in space where the parabola starts</param>
        /// <param name="direction">The direction the line is intended to go</param>
        /// <returns>The calculated point.</returns>
        public static Vector3 GetPointAlongPhysicalParabola(Vector3 origin, Vector3 direction, float velocity, Vector3 gravity, float time)
        {
            direction = Vector3.Normalize(direction);

            origin.x += ((direction.x * velocity * time) + (0.5f * gravity.x * (time * time)));
            origin.y += ((direction.y * velocity * time) + (0.5f * gravity.y * (time * time)));
            origin.z += ((direction.z * velocity * time) + (0.5f * gravity.z * (time * time)));

            return origin;
        }

        /// <summary>
        /// Gets the point along a constrained parabola.
        /// </summary>
        /// <param name="origin">The point in space where the parabola starts.</param>
        /// <param name="end">The point in space where the parabola ends.</param>
        /// <param name="upDirection">The up direction of the arc.</param>
        /// <param name="height">The height of the arc.</param>
        /// <param name="normalizedLength">the normalized length along the line to calculate the point.</param>
        /// <returns>The calculated point found along the normalized length.</returns>
        public static Vector3 GetPointAlongConstrainedParabola(Vector3 origin, Vector3 end, Vector3 upDirection, float height, float normalizedLength)
        {
            float parabolaTime = normalizedLength * 2 - 1;
            Vector3 direction = end - origin;
            Vector3 pos = origin + normalizedLength * direction;
            pos += ((-parabolaTime * parabolaTime + 1) * height) * upDirection.normalized;
            return pos;
        }

        /// <summary>
        /// Gets the point along the spline.
        /// </summary>
        /// <param name="points">the points of the whole spline.</param>
        /// <param name="normalizedLength">the normalized length along the line to calculate the point.</param>
        /// <param name="interpolation">Optional Interpolation type to use when calculating the point.</param>
        /// <returns>The calculated point found along the normalized length.</returns>
        public static Vector3 GetPointAlongSpline(Pose[] points, float normalizedLength, InterpolationType interpolation = InterpolationType.Bezier)
        {
            int pointIndex = (Mathf.RoundToInt(normalizedLength * points.Length));
            float length = normalizedLength - ((float)pointIndex / points.Length);

            if (pointIndex + 3 >= points.Length)
            {
                return points[points.Length - 1].position;
            }

            if (pointIndex < 0)
            {
                return points[0].position;
            }

            Vector3 point1 = points[pointIndex].position;
            Vector3 point2 = points[pointIndex + 1].position;
            Vector3 point3 = points[pointIndex + 2].position;
            Vector3 point4 = points[pointIndex + 3].position;

            switch (interpolation)
            {
                case InterpolationType.CatmullRom:
                    return InterpolateCatmullRomPoints(point1, point2, point3, point4, length);
                default:
                    return InterpolateBezierPoints(point1, point2, point3, point4, length);
            }
        }

        /// <summary>
        /// Interpolate a position between the provided points.
        /// </summary>
        /// <param name="points">The points to use in the calculation.</param>
        /// <param name="normalizedLength">the normalized length along the line to calculate the point.</param>
        /// <returns>The calculated point found along the normalized length.</returns>
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

        /// <summary>
        /// Interpolate the provided points using Catmull Rom algorithm.
        /// </summary>
        /// <param name="normalizedLength">the normalized length along the line to calculate the point.</param>
        /// <returns>The calculated point found along the normalized length.</returns>
        public static Vector3 InterpolateCatmullRomPoints(Vector3 point1, Vector3 point2, Vector3 point3, Vector3 point4, float normalizedLength)
        {
            Vector3 p1 = 2f * point2;
            Vector3 p2 = point3 - point1;
            Vector3 p3 = 2f * point1 - 5f * point2 + 4f * point3 - point4;
            Vector3 p4 = -point1 + 3f * point2 - 3f * point3 + point4;
            return 0.5f * (p1 + (normalizedLength * p2) + (normalizedLength * normalizedLength * p3) + (normalizedLength * normalizedLength * normalizedLength * p4));
        }

        /// <summary>
        /// Interpolate the provided points using the standard Bezier algorithm.
        /// </summary>
        /// <param name="normalizedLength">the normalized length along the line to calculate the point.</param>
        /// <returns>The calculated point found along the normalized length.</returns>
        public static Vector3 InterpolateBezierPoints(Vector3 point1, Vector3 point2, Vector3 point3, Vector3 point4, float normalizedLength)
        {
            float invertedDistance = 1f - normalizedLength;
            return invertedDistance * invertedDistance * invertedDistance * point1 +
                   3f * invertedDistance * invertedDistance * normalizedLength * point2 +
                   3f * invertedDistance * normalizedLength * normalizedLength * point3 +
                   normalizedLength * normalizedLength * normalizedLength * point4;
        }

        /// <summary>
        /// Interpolate the provided points using the Hermite algorithm.
        /// </summary>
        /// <param name="normalizedLength">the normalized length along the line to calculate the point.</param>
        /// <returns>The calculated point found along the normalized length.</returns>
        public static Vector3 InterpolateHermitePoints(Vector3 point1, Vector3 point2, Vector3 point3, Vector3 point4, float normalizedLength)
        {
            float invertedDistance = 1f - normalizedLength;
            return invertedDistance * invertedDistance * invertedDistance * point1 +
                   3f * invertedDistance * invertedDistance * normalizedLength * point2 +
                   3f * invertedDistance * normalizedLength * normalizedLength * point3 +
                   normalizedLength * normalizedLength * normalizedLength * point4;
        }

        /// <summary>
        /// Calculate the ellipse point at the angle provided.
        /// </summary>
        /// <param name="radius">The radius of the ellipse.</param>
        /// <param name="angle">Angle along the ellipse to find the point.</param>
        /// <returns>The calculated point at the specified angle.</returns>
        public static Vector3 GetEllipsePoint(Vector2 radius, float angle)
        {
            cachedEllipsePoint.x = radius.x * Mathf.Cos(angle);
            cachedEllipsePoint.y = radius.y * Mathf.Sin(angle);
            cachedEllipsePoint.z = 0.0f;
            return cachedEllipsePoint;
        }

        /// <summary>
        /// Used to calculate the ellipse point in <see cref="GetEllipsePoint"/>
        /// </summary>
        private static Vector3 cachedEllipsePoint = Vector3.zero;

        /// <summary>
        /// Used to locate and lock the raycast hit data on a select.
        /// </summary>
        public static TargetHitInfo LocateTargetHitPoint(this XRRayInteractor rayInteractor, IXRSelectInteractable interactableObject)
        {
            TargetHitInfo hitInfo = new TargetHitInfo();
            bool hitPointAndTransformUpdated = false;
            bool hitNormalUpdated = false;

            // In the case of affordances/handles, we can stick the ray right on to the handle.
            if (interactableObject is ISnapInteractable snappable)
            {
                hitInfo.HitTargetTransform = snappable.HandleTransform;
                hitInfo.TargetLocalHitPoint = Vector3.zero;
                hitInfo.TargetLocalHitNormal = Vector3.up;
                hitPointAndTransformUpdated = true;
                hitNormalUpdated = true;
            }

            // In the case of an IScrollable being selected, ensure that the reticle locks to the
            // scroller and not to the a list item within the scroller, such as a button.
            if (interactableObject is IScrollable scrollable &&
                scrollable.IsScrolling &&
                scrollable.ScrollingInteractor == (IXRInteractor)rayInteractor)
            {
                hitInfo.HitTargetTransform = scrollable.ScrollableTransform;
                hitInfo.TargetLocalHitPoint = scrollable.ScrollingLocalAnchorPosition;
                hitPointAndTransformUpdated = true;
            }

            // If no hit, abort.
            if (!rayInteractor.TryGetCurrentRaycast(
                  out RaycastHit? raycastHit,
                  out _,
                  out UnityEngine.EventSystems.RaycastResult? raycastResult,
                  out _,
                  out bool isUIHitClosest))
            {
                return hitInfo;
            }

            // Align the reticle with a UI hit if applicable
            if (raycastResult.HasValue && isUIHitClosest)
            {
                hitInfo.HitTargetTransform = raycastResult.Value.gameObject.transform;
                hitInfo.TargetLocalHitPoint = hitInfo.HitTargetTransform.InverseTransformPoint(raycastResult.Value.worldPosition);
                hitInfo.TargetLocalHitNormal = hitInfo.HitTargetTransform.InverseTransformDirection(raycastResult.Value.worldNormal);
                hitInfo.HitDistanceReference = raycastResult.Value.worldPosition;
            }
            // Otherwise, calculate the reticle pose based on the raycast hit.
            else if (raycastHit.HasValue)
            {
                if (!hitPointAndTransformUpdated)
                {
                    hitInfo.HitTargetTransform = raycastHit.Value.collider.transform;
                    hitInfo.TargetLocalHitPoint = hitInfo.HitTargetTransform.InverseTransformPoint(raycastHit.Value.point);
                }

                if (!hitNormalUpdated)
                {
                    hitInfo.TargetLocalHitNormal = hitInfo.HitTargetTransform.InverseTransformDirection(raycastHit.Value.normal);
                }

                hitInfo.HitDistanceReference = hitInfo.HitTargetTransform.TransformPoint(hitInfo.TargetLocalHitPoint);
            }
            return hitInfo;
        }

        /// <summary>
        /// A data container for managing the position, normal, and transform of a target hit point. 
        /// </summary>
        public struct TargetHitInfo
        {
            public Vector3 TargetLocalHitPoint;
            public Vector3 TargetLocalHitNormal;
            public Transform HitTargetTransform;
            public Vector3 HitDistanceReference;
        }
    }
}
