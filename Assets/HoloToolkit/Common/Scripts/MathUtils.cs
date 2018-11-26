// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Math Utilities class.
    /// </summary>
    public static class MathUtils
    {
        /// <summary>
        /// Get the horizontal FOV from the stereo camera
        /// </summary>
        /// <returns></returns>
        [System.Obsolete("Use CameraExtensions.GetHorizontalFieldOfViewRadians(Camera camera) instead.")]
        public static float GetHorizontalFieldOfViewRadians()
        {
            return CameraCache.Main.GetHorizontalFieldOfViewRadians();
        }

        /// <summary>
        /// Returns if a point will be rendered on the screen in either eye
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        [System.Obsolete("Use CameraExtensions.IsInFOV(Camera camera, Vector3 position) instead.")]
        public static bool IsInFOV(Vector3 position)
        {
            return CameraCache.Main.IsInFOV(position);
        }

        /// <summary>
        /// Takes a point in the coordinate space specified by the "from" transform and transforms it to be the correct point in the coordinate space specified by the "to" transform
        /// applies rotation, scale and translation
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="ptInFrom"></param>
        /// <returns></returns>
        public static Vector3 TransformPointFromTo(Transform from, Transform to, Vector3 ptInFrom)
        {
            Vector3 ptInWorld = (from == null) ? ptInFrom : from.TransformPoint(ptInFrom);
            Vector3 ptInTo = (to == null) ? ptInWorld : to.InverseTransformPoint(ptInWorld);
            return ptInTo;
        }

        /// <summary>
        /// Takes a direction in the coordinate space specified by the "from" transform and transforms it to be the correct direction in the coordinate space specified by the "to" transform
        /// applies rotation only, no translation or scale
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="dirInFrom"></param>
        /// <returns></returns>
        public static Vector3 TransformDirectionFromTo(Transform from, Transform to, Vector3 dirInFrom)
        {
            Vector3 dirInWorld = (from == null) ? dirInFrom : from.TransformDirection(dirInFrom);
            Vector3 dirInTo = (to == null) ? dirInWorld : to.InverseTransformDirection(dirInWorld);
            return dirInTo;
        }

        /// <summary>
        /// Takes a vectpr in the coordinate space specified by the "from" transform and transforms it to be the correct direction in the coordinate space specified by the "to" transform
        /// applies rotation and scale, no translation
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="vecInFrom"></param>
        /// <returns></returns>
        public static Vector3 TransformVectorFromTo(Transform from, Transform to, Vector3 vecInFrom)
        {
            Vector3 vecInWorld = (from == null) ? vecInFrom : from.TransformVector(vecInFrom);
            Vector3 vecInTo = (to == null) ? vecInWorld : to.InverseTransformVector(vecInWorld);
            return vecInTo;
        }

        /// <summary>
        /// Takes a ray in the coordinate space specified by the "from" transform and transforms it to be the correct ray in the coordinate space specified by the "to" transform
        /// </summary>
        public static Ray TransformRayFromTo(Transform from, Transform to, Ray rayToConvert)
        {
            Ray outputRay = new Ray
            {
                origin = TransformPointFromTo(from, to, rayToConvert.origin),
                direction = TransformDirectionFromTo(from, to, rayToConvert.direction)
            };

            return outputRay;
        }

        /// <summary>
        /// Creates a quaternion containing the rotation from the input matrix.
        /// </summary>
        /// <param name="m">Input matrix to convert to quaternion</param>
        /// <returns></returns>
        public static Quaternion QuaternionFromMatrix(Matrix4x4 m)
        {
            // TODO: test and replace with this simpler, more unity-friendly code
            //       Quaternion q = Quaternion.LookRotation(m.GetColumn(2),m.GetColumn(1));

            Quaternion q = new Quaternion();
            q.w = Mathf.Sqrt(Mathf.Max(0, 1 + m[0, 0] + m[1, 1] + m[2, 2])) / 2;
            q.x = Mathf.Sqrt(Mathf.Max(0, 1 + m[0, 0] - m[1, 1] - m[2, 2])) / 2;
            q.y = Mathf.Sqrt(Mathf.Max(0, 1 - m[0, 0] + m[1, 1] - m[2, 2])) / 2;
            q.z = Mathf.Sqrt(Mathf.Max(0, 1 - m[0, 0] - m[1, 1] + m[2, 2])) / 2;
            q.x *= Mathf.Sign(q.x * (m[2, 1] - m[1, 2]));
            q.y *= Mathf.Sign(q.y * (m[0, 2] - m[2, 0]));
            q.z *= Mathf.Sign(q.z * (m[1, 0] - m[0, 1]));
            return q;
        }

        /// <summary>
        /// Extract the translation and rotation components of a Unity matrix
        /// </summary>
        public static void ToTranslationRotation(Matrix4x4 unityMtx, out Vector3 translation, out Quaternion rotation)
        {
            Vector3 upwards = new Vector3(unityMtx.m01, unityMtx.m11, unityMtx.m21);
            Vector3 forward = new Vector3(unityMtx.m02, unityMtx.m12, unityMtx.m22);
            translation = new Vector3(unityMtx.m03, unityMtx.m13, unityMtx.m23);
            rotation = Quaternion.LookRotation(forward, upwards);
        }

        /// <summary>
        /// Project vector onto XZ plane
        /// </summary>
        /// <param name="v"></param>
        /// <returns>result of projecting v onto XZ plane</returns>
        public static Vector3 XZProject(Vector3 v)
        {
            return new Vector3(v.x, 0.0f, v.z);
        }

        /// <summary>
        /// Project vector onto YZ plane
        /// </summary>
        /// <param name="v"></param>
        /// <returns>result of projecting v onto YZ plane</returns>
        public static Vector3 YZProject(Vector3 v)
        {
            return new Vector3(0.0f, v.y, v.z);
        }

        /// <summary>
        /// Project vector onto XY plane
        /// </summary>
        /// <param name="v"></param>
        /// <returns>result of projecting v onto XY plane</returns>
        public static Vector3 XYProject(Vector3 v)
        {
            return new Vector3(v.x, v.y, 0.0f);
        }

        /// <summary>
        /// Returns the distance between a point and an infinite line defined by two points; linePointA and linePointB
        /// </summary>
        /// <param name="point"></param>
        /// <param name="linePointA"></param>
        /// <param name="linePointB"></param>
        /// <returns></returns>
        public static float DistanceOfPointToLine(Vector3 point, Vector3 linePointA, Vector3 linePointB)
        {
            Vector3 closestPoint = ClosestPointOnLineToPoint(point, linePointA, linePointB);
            return (point - closestPoint).magnitude;
        }

        public static Vector3 ClosestPointOnLineToPoint(Vector3 point, Vector3 linePointA, Vector3 linePointB)
        {
            Vector3 v = linePointB - linePointA;
            Vector3 w = point - linePointA;

            float c1 = Vector3.Dot(w, v);
            float c2 = Vector3.Dot(v, v);
            float b = c1 / c2;

            Vector3 pointB = linePointA + (v * b);

            return pointB;
        }

        public static float DistanceOfPointToLineSegment(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
        {
            Vector3 closestPoint = ClosestPointOnLineSegmentToPoint(point, lineStart, lineEnd);
            return (point - closestPoint).magnitude;
        }

        public static Vector3 ClosestPointOnLineSegmentToPoint(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
        {
            Vector3 v = lineEnd - lineStart;
            Vector3 w = point - lineStart;

            float c1 = Vector3.Dot(w, v);
            if (c1 <= 0)
            {
                return lineStart;
            }

            float c2 = Vector3.Dot(v, v);
            if (c2 <= c1)
            {
                return lineEnd;
            }

            float b = c1 / c2;

            Vector3 pointB = lineStart + (v * b);

            return pointB;
        }

        public static bool TestPlanesAABB(Plane[] planes, int planeMask, Bounds bounds, out bool entirelyInside)
        {
            int planeIndex = 0;
            int entirelyInsideCount = 0;
            Vector3 boundsCenter = bounds.center;  // center of bounds
            Vector3 boundsExtent = bounds.extents; // half diagonal
                                                   // do intersection test for each active frame
            int mask = 1;

            // while active frames
            while (mask <= planeMask)
            {
                // if active
                if ((uint)(planeMask & mask) != 0)
                {
                    Plane p = planes[planeIndex];
                    Vector3 n = p.normal;
                    n.x = Mathf.Abs(n.x);
                    n.y = Mathf.Abs(n.y);
                    n.z = Mathf.Abs(n.z);

                    float distance = p.GetDistanceToPoint(boundsCenter);
                    float radius = Vector3.Dot(boundsExtent, n);

                    if (distance + radius < 0)
                    {
                        // behind clip plane
                        entirelyInside = false;
                        return false;
                    }

                    if (distance > radius)
                    {
                        entirelyInsideCount++;
                    }
                }

                mask += mask;
                planeIndex++;
            }

            entirelyInside = entirelyInsideCount == planes.Length;
            return true;
        }

        /// <summary>
        /// Tests component-wise if a Vector2 is in a given range
        /// </summary>
        /// <param name="vec">The vector to test</param>
        /// <param name="lower">The lower bounds</param>
        /// <param name="upper">The upper bounds</param>
        /// <returns>true if in range, otherwise false</returns>
        public static bool InRange(Vector2 vec, Vector2 lower, Vector2 upper)
        {
            return vec.x >= lower.x && vec.x <= upper.x && vec.y >= lower.y && vec.y <= upper.y;
        }

        /// <summary>
        /// Tests component-wise if a Vector3 is in a given range
        /// </summary>
        /// <param name="vec">The vector to test</param>
        /// <param name="lower">The lower bounds</param>
        /// <param name="upper">The upper bounds</param>
        /// <returns>true if in range, otherwise false</returns>
        public static bool InRange(Vector3 vec, Vector3 lower, Vector3 upper)
        {
            return vec.x >= lower.x && vec.x <= upper.x && vec.y >= lower.y && vec.y <= upper.y && vec.z >= lower.z && vec.z <= upper.z;
        }

        /// <summary>
        /// Element-wise addition of two Matrix4x4s - extension method
        /// </summary>
        /// <param name="a">matrix</param>
        /// <param name="b">matrix</param>
        /// <returns>element-wise (a+b)</returns>
        public static Matrix4x4 Add(Matrix4x4 a, Matrix4x4 b)
        {
            Matrix4x4 result = new Matrix4x4();
            result.SetColumn(0, a.GetColumn(0) + b.GetColumn(0));
            result.SetColumn(1, a.GetColumn(1) + b.GetColumn(1));
            result.SetColumn(2, a.GetColumn(2) + b.GetColumn(2));
            result.SetColumn(3, a.GetColumn(3) + b.GetColumn(3));
            return result;
        }

        /// <summary>
        /// Element-wise subtraction of two Matrix4x4s - extension method
        /// </summary>
        /// <param name="a">matrix</param>
        /// <param name="b">matrix</param>
        /// <returns>element-wise (a-b)</returns>
        public static Matrix4x4 Subtract(Matrix4x4 a, Matrix4x4 b)
        {
            Matrix4x4 result = new Matrix4x4();
            result.SetColumn(0, a.GetColumn(0) - b.GetColumn(0));
            result.SetColumn(1, a.GetColumn(1) - b.GetColumn(1));
            result.SetColumn(2, a.GetColumn(2) - b.GetColumn(2));
            result.SetColumn(3, a.GetColumn(3) - b.GetColumn(3));
            return result;
        }

        /// <summary>
        /// find unsigned distance of 3D point to an infinite line
        /// </summary>
        /// <param name="ray">ray that specifies an infinite line</param>
        /// <param name="point">3D point</param>
        /// <returns>unsigned perpendicular distance from point to line</returns>
        public static float DistanceOfPointToLine(Ray ray, Vector3 point)
        {
            return Vector3.Cross(ray.direction, point - ray.origin).magnitude;
        }

        /// <summary>
        /// Find 3D point that minimizes distance to 2 lines, midpoint of the shortest perpendicular line segment between them
        /// </summary>
        /// <param name="p">ray that specifies a line</param>
        /// <param name="q">ray that specifies a line</param>
        /// <returns>point nearest to the lines</returns>
        public static Vector3 NearestPointToLines(Ray p, Ray q)
        {
            float a = Vector3.Dot(p.direction, p.direction);
            float b = Vector3.Dot(p.direction, q.direction);
            float c = Vector3.Dot(q.direction, q.direction);
            Vector3 w0 = p.origin - q.origin;
            float den = a * c - b * b;
            float epsilon = 0.00001f;
            if (den < epsilon)
            {
                // parallel, so just average origins
                return 0.5f * (p.origin + q.origin);
            }
            float d = Vector3.Dot(p.direction, w0);
            float e = Vector3.Dot(q.direction, w0);
            float sc = (b * e - c * d) / den;
            float tc = (a * e - b * d) / den;
            Vector3 point = 0.5f * (p.origin + sc * p.direction + q.origin + tc * q.direction);
            return point;
        }

        /// <summary>
        /// Find 3D point that minimizes distance to a set of 2 or more lines, ignoring outliers
        /// </summary>
        /// <param name="rays">list of rays, each specifying a line, must have at least 1</param>
        /// <param name="ransac_iterations">number of iterations:  log(1-p)/log(1-(1-E)^s)
        ///      where p is probability of at least one sample containing s points is all inliers
        ///      E is proportion of outliers (1-ransac_ratio)
        ///      e.g. p=0.999, ransac_ratio=0.54, s=2 ==>  log(0.001)/(log(1-0.54^2) = 20
        /// </param>
        /// <param name="ransac_threshold">minimum distance from point to line for a line to be considered an inlier</param>
        /// <param name="numActualInliers">return number of inliers: lines that are within ransac_threshold of nearest point</param>
        /// <returns>point nearest to the set of lines, ignoring outliers</returns>
        public static Vector3 NearestPointToLinesRANSAC(List<Ray> rays, int ransac_iterations, float ransac_threshold, out int numActualInliers)
        {
            // start with something, just in case no inliers - this works for case of 1 or 2 rays
            Vector3 nearestPoint = NearestPointToLines(rays[0], rays[rays.Count - 1]);
            numActualInliers = 0;
            if (rays.Count > 2)
            {
                for (int it = 0; it < ransac_iterations; it++)
                {
                    Vector3 testPoint = NearestPointToLines(rays[Random.Range(0, rays.Count)], rays[Random.Range(0, rays.Count)]);

                    // count inliers
                    int numInliersForIteration = 0;
                    for (int ind = 0; ind < rays.Count; ++ind)
                    {
                        if (DistanceOfPointToLine(rays[ind], testPoint) < ransac_threshold)
                            ++numInliersForIteration;
                    }

                    // remember best
                    if (numInliersForIteration > numActualInliers)
                    {
                        numActualInliers = numInliersForIteration;
                        nearestPoint = testPoint;
                    }
                }
            }

            // now find and count actual inliers and do least-squares to find best fit
            var inlierList = rays.Where(r => DistanceOfPointToLine(r, nearestPoint) < ransac_threshold);
            numActualInliers = inlierList.Count();
            if (numActualInliers >= 2)
            {
                nearestPoint = NearestPointToLinesLeastSquares(inlierList);
            }
            return nearestPoint;
        }

        /// <summary>
        /// Find 3D point that minimizes distance to a set of 2 or more lines
        /// </summary>
        /// <param name="rays">each ray specifies an infinite line</param>
        /// <returns>point nearest to the set of lines</returns>
        public static Vector3 NearestPointToLinesLeastSquares(IEnumerable<Ray> rays)
        {
            // finding the point nearest to the set of lines specified by rays
            // Use the following formula, where u_i are normalized direction
            // vectors along each ray and p_i is a point along each ray.

            //                      -1
            //  / =====             \   =====
            //  | \     /        T\ |   \     /        T\
            //  |  >    |I - u  u | |    >    |I - u  u | p
            //  | /     \     i  i/ |   /     \     i  i/  i
            //  | =====             |   =====
            //  \   i               /     i

            Matrix4x4 sumOfProduct = Matrix4x4.zero;
            Vector4 sumOfProductTimesDirection = Vector4.zero;

            foreach (Ray r in rays)
            {
                Vector4 point = r.origin;
                Matrix4x4 directionColumnMatrix = new Matrix4x4();
                Vector3 rNormal = r.direction.normalized;
                directionColumnMatrix.SetColumn(0, rNormal);
                Matrix4x4 directionRowMatrix = directionColumnMatrix.transpose;
                Matrix4x4 product = directionColumnMatrix * directionRowMatrix;
                Matrix4x4 identityMinusDirectionProduct = Subtract(Matrix4x4.identity, product);
                sumOfProduct = Add(sumOfProduct, identityMinusDirectionProduct);
                Vector4 vectorProduct = identityMinusDirectionProduct * point;
                sumOfProductTimesDirection += vectorProduct;
            }

            Matrix4x4 sumOfProductInverse = sumOfProduct.inverse;
            Vector3 nearestPoint = sumOfProductInverse * sumOfProductTimesDirection;
            return nearestPoint;
        }
    }
}