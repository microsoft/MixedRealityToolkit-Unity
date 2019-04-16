// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Extension methods for Unity's Bounds struct
    /// </summary>
    public static class BoundsExtensions
    {
        // Corners
        public const int LBF = 0;
        public const int LBB = 1;
        public const int LTF = 2;
        public const int LTB = 3;
        public const int RBF = 4;
        public const int RBB = 5;
        public const int RTF = 6;
        public const int RTB = 7;

        // X axis
        public const int LTF_RTF = 8;
        public const int LBF_RBF = 9;
        public const int RTB_LTB = 10;
        public const int RBB_LBB = 11;

        // Y axis
        public const int LTF_LBF = 12;
        public const int RTB_RBB = 13;
        public const int LTB_LBB = 14;
        public const int RTF_RBF = 15;

        // Z axis
        public const int RBF_RBB = 16;
        public const int RTF_RTB = 17;
        public const int LBF_LBB = 18;
        public const int LTF_LTB = 19;

        // 2D corners
        public const int LT = 0;
        public const int LB = 1;
        public const int RT = 2;
        public const int RB = 3;

        // 2D midpoints
        public const int LT_RT = 4;
        public const int RT_RB = 5;
        public const int RB_LB = 6;
        public const int LB_LT = 7;

        // Face points
        public const int TOP = 0;
        public const int BOT = 1;
        public const int LFT = 2;
        public const int RHT = 3;
        public const int FWD = 4;
        public const int BCK = 5;

        public enum Axis
        {
            X,
            Y,
            Z
        }

        private static Vector3[] corners = null;

        private static Vector3[] rectTransformCorners = new Vector3[4];

        #region Public Static Functions
        /// <summary>
        /// Returns an instance of the 'Bounds' class which is invalid. An invalid 'Bounds' instance 
        /// is one which has its size vector set to 'float.MaxValue' for all 3 components. The center
        /// of an invalid bounds instance is the zero vector.
        /// </summary>
        public static Bounds GetInvalidBoundsInstance()
        {
            return new Bounds(Vector3.zero, GetInvalidBoundsSize());
        }

        /// <summary>
        /// Checks if the specified bounds instance is valid. A valid 'Bounds' instance is
        /// one whose size vector does not have all 3 components set to 'float.MaxValue'.
        /// </summary>
        public static bool IsValid(this Bounds bounds)
        {
            return bounds.size != GetInvalidBoundsSize();
        }

        /// <summary>
        /// Gets all the corner points of the bounds in world space
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="positions"></param>
        /// <param name="bounds"></param>
        /// <remarks>
        /// Use BoxColliderExtensions.{Left|Right}{Bottom|Top}{Front|Back} consts to index into the output
        /// corners array.
        /// </remarks>
        public static void GetCornerPositions(this Bounds bounds, Transform transform, ref Vector3[] positions)
        {
            // Calculate the local points to transform.
            Vector3 center = bounds.center;
            Vector3 extents = bounds.extents;
            float leftEdge = center.x - extents.x;
            float rightEdge = center.x + extents.x;
            float bottomEdge = center.y - extents.y;
            float topEdge = center.y + extents.y;
            float frontEdge = center.z - extents.z;
            float backEdge = center.z + extents.z;

            // Allocate the array if needed.
            const int numPoints = 8;
            if (positions == null || positions.Length != numPoints)
            {
                positions = new Vector3[numPoints];
            }

            // Transform all the local points to world space.
            positions[LBF] = transform.TransformPoint(leftEdge, bottomEdge, frontEdge);
            positions[LBB] = transform.TransformPoint(leftEdge, bottomEdge, backEdge);
            positions[LTF] = transform.TransformPoint(leftEdge, topEdge, frontEdge);
            positions[LTB] = transform.TransformPoint(leftEdge, topEdge, backEdge);
            positions[RBF] = transform.TransformPoint(rightEdge, bottomEdge, frontEdge);
            positions[RBB] = transform.TransformPoint(rightEdge, bottomEdge, backEdge);
            positions[RTF] = transform.TransformPoint(rightEdge, topEdge, frontEdge);
            positions[RTB] = transform.TransformPoint(rightEdge, topEdge, backEdge);
        }

        /// <summary>
        /// Gets all the corner points of the bounds in local space
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="positions"></param>
        /// <param name="bounds"></param>
        /// <remarks>
        /// Use BoxColliderExtensions.{Left|Right}{Bottom|Top}{Front|Back} consts to index into the output
        /// corners array.
        /// </remarks>
        public static void GetCornerPositions(this Bounds bounds, ref Vector3[] positions)
        {
            // Calculate the local points to transform.
            Vector3 center = bounds.center;
            Vector3 extents = bounds.extents;
            float leftEdge = center.x - extents.x;
            float rightEdge = center.x + extents.x;
            float bottomEdge = center.y - extents.y;
            float topEdge = center.y + extents.y;
            float frontEdge = center.z - extents.z;
            float backEdge = center.z + extents.z;

            // Allocate the array if needed.
            const int numPoints = 8;
            if (positions == null || positions.Length != numPoints)
            {
                positions = new Vector3[numPoints];
            }

            // Transform all the local points to world space.
            positions[LBF] = new Vector3(leftEdge, bottomEdge, frontEdge);
            positions[LBB] = new Vector3(leftEdge, bottomEdge, backEdge);
            positions[LTF] = new Vector3(leftEdge, topEdge, frontEdge);
            positions[LTB] = new Vector3(leftEdge, topEdge, backEdge);
            positions[RBF] = new Vector3(rightEdge, bottomEdge, frontEdge);
            positions[RBB] = new Vector3(rightEdge, bottomEdge, backEdge);
            positions[RTF] = new Vector3(rightEdge, topEdge, frontEdge);
            positions[RTB] = new Vector3(rightEdge, topEdge, backEdge);
        }

        /// <summary>
        /// Gets all the corner points from Renderer's Bounds
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="positions"></param>
        public static void GetCornerPositionsFromRendererBounds(this Bounds bounds, ref Vector3[] positions)
        {
            Vector3 center = bounds.center;
            Vector3 extents = bounds.extents;
            float leftEdge = center.x - extents.x;
            float rightEdge = center.x + extents.x;
            float bottomEdge = center.y - extents.y;
            float topEdge = center.y + extents.y;
            float frontEdge = center.z - extents.z;
            float backEdge = center.z + extents.z;

            const int numPoints = 8;
            if (positions == null || positions.Length != numPoints)
            {
                positions = new Vector3[numPoints];
            }

            positions[LBF] = new Vector3(leftEdge, bottomEdge, frontEdge);
            positions[LBB] = new Vector3(leftEdge, bottomEdge, backEdge);
            positions[LTF] = new Vector3(leftEdge, topEdge, frontEdge);
            positions[LTB] = new Vector3(leftEdge, topEdge, backEdge);
            positions[RBF] = new Vector3(rightEdge, bottomEdge, frontEdge);
            positions[RBB] = new Vector3(rightEdge, bottomEdge, backEdge);
            positions[RTF] = new Vector3(rightEdge, topEdge, frontEdge);
            positions[RTB] = new Vector3(rightEdge, topEdge, backEdge);
        }

        public static void GetFacePositions(this Bounds bounds, Transform transform, ref Vector3[] positions)
        {
            Vector3 center = bounds.center;
            Vector3 extents = bounds.extents;

            const int numPoints = 6;
            if (positions == null || positions.Length != numPoints)
            {
                positions = new Vector3[numPoints];
            }

            positions[TOP] = transform.TransformPoint(center + Vector3.up * extents.y);
            positions[BOT] = transform.TransformPoint(center + Vector3.down * extents.y);
            positions[LFT] = transform.TransformPoint(center + Vector3.left * extents.x);
            positions[RHT] = transform.TransformPoint(center + Vector3.right * extents.x);
            positions[FWD] = transform.TransformPoint(center + Vector3.forward * extents.z);
            positions[BCK] = transform.TransformPoint(center + Vector3.back * extents.z);
        }

        /// <summary>
        /// Gets all the corner points and mid points from Renderer's Bounds
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="transform"></param>
        /// <param name="positions"></param>
        public static void GetCornerAndMidPointPositions(this Bounds bounds, Transform transform, ref Vector3[] positions)
        {
            // Calculate the local points to transform.
            Vector3 center = bounds.center;
            Vector3 extents = bounds.extents;
            float leftEdge = center.x - extents.x;
            float rightEdge = center.x + extents.x;
            float bottomEdge = center.y - extents.y;
            float topEdge = center.y + extents.y;
            float frontEdge = center.z - extents.z;
            float backEdge = center.z + extents.z;

            // Allocate the array if needed.
            const int numPoints = LTF_LTB + 1;
            if (positions == null || positions.Length != numPoints)
            {
                positions = new Vector3[numPoints];
            }

            // Transform all the local points to world space.
            positions[LBF] = transform.TransformPoint(leftEdge, bottomEdge, frontEdge);
            positions[LBB] = transform.TransformPoint(leftEdge, bottomEdge, backEdge);
            positions[LTF] = transform.TransformPoint(leftEdge, topEdge, frontEdge);
            positions[LTB] = transform.TransformPoint(leftEdge, topEdge, backEdge);
            positions[RBF] = transform.TransformPoint(rightEdge, bottomEdge, frontEdge);
            positions[RBB] = transform.TransformPoint(rightEdge, bottomEdge, backEdge);
            positions[RTF] = transform.TransformPoint(rightEdge, topEdge, frontEdge);
            positions[RTB] = transform.TransformPoint(rightEdge, topEdge, backEdge);

            positions[LTF_RTF] = Vector3.Lerp(positions[LTF], positions[RTF], 0.5f);
            positions[LBF_RBF] = Vector3.Lerp(positions[LBF], positions[RBF], 0.5f);
            positions[RTB_LTB] = Vector3.Lerp(positions[RTB], positions[LTB], 0.5f);
            positions[RBB_LBB] = Vector3.Lerp(positions[RBB], positions[LBB], 0.5f);

            positions[LTF_LBF] = Vector3.Lerp(positions[LTF], positions[LBF], 0.5f);
            positions[RTB_RBB] = Vector3.Lerp(positions[RTB], positions[RBB], 0.5f);
            positions[LTB_LBB] = Vector3.Lerp(positions[LTB], positions[LBB], 0.5f);
            positions[RTF_RBF] = Vector3.Lerp(positions[RTF], positions[RBF], 0.5f);

            positions[RBF_RBB] = Vector3.Lerp(positions[RBF], positions[RBB], 0.5f);
            positions[RTF_RTB] = Vector3.Lerp(positions[RTF], positions[RTB], 0.5f);
            positions[LBF_LBB] = Vector3.Lerp(positions[LBF], positions[LBB], 0.5f);
            positions[LTF_LTB] = Vector3.Lerp(positions[LTF], positions[LTB], 0.5f);
        }

        /// <summary>
        /// Gets all the corner points and mid points from Renderer's Bounds, ignoring the z axis
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="transform"></param>
        /// <param name="positions"></param>
        /// <param name="flattenAxis"></param>
        public static void GetCornerAndMidPointPositions2D(this Bounds bounds, Transform transform, ref Vector3[] positions, Axis flattenAxis)
        {
            // Calculate the local points to transform.
            Vector3 center = bounds.center;
            Vector3 extents = bounds.extents;

            float leftEdge = 0;
            float rightEdge = 0;
            float bottomEdge = 0;
            float topEdge = 0;

            // Allocate the array if needed.
            const int numPoints = LB_LT + 1;
            if (positions == null || positions.Length != numPoints)
            {
                positions = new Vector3[numPoints];
            }

            switch (flattenAxis)
            {
                case Axis.X:
                default:
                    leftEdge = center.z - extents.z;
                    rightEdge = center.z + extents.z;
                    bottomEdge = center.y - extents.y;
                    topEdge = center.y + extents.y;
                    // Transform all the local points to world space.
                    positions[LT] = transform.TransformPoint(0, topEdge, leftEdge);
                    positions[LB] = transform.TransformPoint(0, bottomEdge, leftEdge);
                    positions[RT] = transform.TransformPoint(0, topEdge, rightEdge);
                    positions[RB] = transform.TransformPoint(0, bottomEdge, rightEdge);
                    break;

                case Axis.Y:
                    leftEdge = center.z - extents.z;
                    rightEdge = center.z + extents.z;
                    bottomEdge = center.x - extents.x;
                    topEdge = center.x + extents.x;
                    // Transform all the local points to world space.
                    positions[LT] = transform.TransformPoint(topEdge, 0, leftEdge);
                    positions[LB] = transform.TransformPoint(bottomEdge, 0, leftEdge);
                    positions[RT] = transform.TransformPoint(topEdge, 0, rightEdge);
                    positions[RB] = transform.TransformPoint(bottomEdge, 0, rightEdge);
                    break;

                case Axis.Z:
                    leftEdge = center.x - extents.x;
                    rightEdge = center.x + extents.x;
                    bottomEdge = center.y - extents.y;
                    topEdge = center.y + extents.y;
                    // Transform all the local points to world space.
                    positions[LT] = transform.TransformPoint(leftEdge, topEdge, 0);
                    positions[LB] = transform.TransformPoint(leftEdge, bottomEdge, 0);
                    positions[RT] = transform.TransformPoint(rightEdge, topEdge, 0);
                    positions[RB] = transform.TransformPoint(rightEdge, bottomEdge, 0);
                    break;
            }

            positions[LT_RT] = Vector3.Lerp(positions[LT], positions[RT], 0.5f);
            positions[RT_RB] = Vector3.Lerp(positions[RT], positions[RB], 0.5f);
            positions[RB_LB] = Vector3.Lerp(positions[RB], positions[LB], 0.5f);
            positions[LB_LT] = Vector3.Lerp(positions[LB], positions[LT], 0.5f);
        }

        /// <summary>
        /// Method to get bounding box points using Collider method.
        /// </summary>
        /// <param name="target">gameObject that boundingBox bounds.</param>
        /// <param name="boundsPoints">array reference that gets filled with points</param>
        /// <param name="ignoreLayers">layerMask to simplify search</param>
        public static void GetColliderBoundsPoints(GameObject target, List<Vector3> boundsPoints, LayerMask ignoreLayers)
        {
            Collider[] colliders = target.GetComponentsInChildren<Collider>();
            for (int i = 0; i < colliders.Length; i++)
            {
                if (ignoreLayers == (1 << colliders[i].gameObject.layer | ignoreLayers))
                {
                    continue;
                }

                if(colliders[i] is SphereCollider)
                {
                    SphereCollider sc = colliders[i] as SphereCollider;
                    Bounds sphereBounds = new Bounds(sc.center, Vector3.one * sc.radius * 2);
                    sphereBounds.GetFacePositions(sc.transform, ref corners);
                    boundsPoints.AddRange(corners);
                }
                else if(colliders[i] is BoxCollider)
                {
                    BoxCollider bc = colliders[i] as BoxCollider;
                    Bounds boxBounds = new Bounds(bc.center, bc.size);
                    boxBounds.GetCornerPositions(bc.transform, ref corners);
                    boundsPoints.AddRange(corners);

                }
                else if(colliders[i] is MeshCollider)
                {
                    MeshCollider mc = colliders[i] as MeshCollider;
                    Bounds meshBounds = mc.sharedMesh.bounds;
                    meshBounds.GetCornerPositions(mc.transform, ref corners);
                    boundsPoints.AddRange(corners);
                }
                else if(colliders[i] is CapsuleCollider)
                {
                    CapsuleCollider cc = colliders[i] as CapsuleCollider;
                    Bounds capsuleBounds = new Bounds(cc.center, Vector3.zero);
                    switch (cc.direction)
                    {
                        case 0:
                            capsuleBounds.size = new Vector3(cc.height, cc.radius * 2, cc.radius * 2);
                            break;

                        case 1:
                            capsuleBounds.size = new Vector3(cc.radius * 2, cc.height, cc.radius * 2);
                            break;

                        case 2:
                            capsuleBounds.size = new Vector3(cc.radius * 2, cc.radius * 2, cc.height);
                            break;
                    }
                    capsuleBounds.GetFacePositions(cc.transform, ref corners);
                    boundsPoints.AddRange(corners);
                }             
            }
        }

        /// <summary>
        /// GetRenderBoundsPoints gets bounding box points using Render method.
        /// </summary>
        /// <param name="target">gameObject that boundingbox bounds</param>
        /// <param name="boundsPoints">array reference that gets filled with points</param>
        /// <param name="ignoreLayers">layerMask to simplify search</param>
        public static void GetRenderBoundsPoints(GameObject target, List<Vector3> boundsPoints, LayerMask ignoreLayers)
        {
            Renderer[] renderers = target.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; ++i)
            {
                Renderer rendererObj = renderers[i];
                if (ignoreLayers == (1 << rendererObj.gameObject.layer | ignoreLayers))
                {
                    continue;
                }

                rendererObj.bounds.GetCornerPositionsFromRendererBounds(ref corners);
                boundsPoints.AddRange(corners);
            }
        }

        /// <summary>
        /// GetMeshFilterBoundsPoints - gets bounding box points using MeshFilter method.
        /// </summary>
        /// <param name="target">gameObject that boundingbox bounds</param>
        /// <param name="boundsPoints">array reference that gets filled with points</param>
        /// <param name="ignoreLayers">layerMask to simplify search</param>
        public static void GetMeshFilterBoundsPoints(GameObject target, List<Vector3> boundsPoints, LayerMask ignoreLayers)
        {
            MeshFilter[] meshFilters = target.GetComponentsInChildren<MeshFilter>();
            for (int i = 0; i < meshFilters.Length; i++)
            {
                MeshFilter meshFilterObj = meshFilters[i];
                if (ignoreLayers == (1 << meshFilterObj.gameObject.layer | ignoreLayers))
                {
                    continue;
                }

                Bounds meshBounds = meshFilterObj.sharedMesh.bounds;
                meshBounds.GetCornerPositions(meshFilterObj.transform, ref corners);
                boundsPoints.AddRange(corners);
            }
            RectTransform[] rectTransforms = target.GetComponentsInChildren<RectTransform>();
            for (int i = 0; i < rectTransforms.Length; i++)
            {
                rectTransforms[i].GetWorldCorners(rectTransformCorners);
                boundsPoints.AddRange(rectTransformCorners);
            }
        }

        /// <summary>
        /// Transforms 'bounds' using the specified transform matrix.
        /// </summary>
        /// <remarks>
        /// Transforming a 'Bounds' instance means that the function will construct a new 'Bounds' 
        /// instance which has its center translated using the translation information stored in
        /// the specified matrix and its size adjusted to account for rotation and scale. The size
        /// of the new 'Bounds' instance will be calculated in such a way that it will contain the
        /// old 'Bounds'.
        /// </remarks>
        /// <param name="bounds">
        /// The 'Bounds' instance which must be transformed.
        /// </param>
        /// <param name="transformMatrix">
        /// The specified 'Bounds' instance will be transformed using this transform matrix. The function
        /// assumes that the matrix doesn't contain any projection or skew transformation.
        /// </param>
        /// <returns>
        /// The transformed 'Bounds' instance.
        /// </returns>
        public static Bounds Transform(this Bounds bounds, Matrix4x4 transformMatrix)
        {
            // We will need access to the right, up and look vector which are encoded inside the transform matrix
            Vector3 rightAxis = transformMatrix.GetColumn(0);
            Vector3 upAxis = transformMatrix.GetColumn(1);
            Vector3 lookAxis = transformMatrix.GetColumn(2);

            // We will 'imagine' that we want to rotate the bounds' extents vector using the rotation information
            // stored inside the specified transform matrix. We will need these when calculating the new size if
            // the transformed bounds.
            Vector3 rotatedExtentsRight = rightAxis * bounds.extents.x;
            Vector3 rotatedExtentsUp = upAxis * bounds.extents.y;
            Vector3 rotatedExtentsLook = lookAxis * bounds.extents.z;

            // Calculate the new bounds size along each axis. The size on each axis is calculated by summing up the 
            // corresponding vector component values of the rotated extents vectors. We multiply by 2 because we want
            // to get a size and currently we are working with extents which represent half the size.
            float newSizeX = (Mathf.Abs(rotatedExtentsRight.x) + Mathf.Abs(rotatedExtentsUp.x) + Mathf.Abs(rotatedExtentsLook.x)) * 2.0f;
            float newSizeY = (Mathf.Abs(rotatedExtentsRight.y) + Mathf.Abs(rotatedExtentsUp.y) + Mathf.Abs(rotatedExtentsLook.y)) * 2.0f;
            float newSizeZ = (Mathf.Abs(rotatedExtentsRight.z) + Mathf.Abs(rotatedExtentsUp.z) + Mathf.Abs(rotatedExtentsLook.z)) * 2.0f;

            // Construct the transformed 'Bounds' instance
            var transformedBounds = new Bounds();
            transformedBounds.center = transformMatrix.MultiplyPoint(bounds.center);
            transformedBounds.size = new Vector3(newSizeX, newSizeY, newSizeZ);

            // Return the instance to the caller
            return transformedBounds;
        }

        /// <summary>
        /// Returns the screen space corner points of the specified 'Bounds' instance.
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="camera">
        /// The camera used for rendering to the screen. This is needed to perform the
        /// transformation to screen space.
        /// </param>
        public static Vector2[] GetScreenSpaceCornerPoints(this Bounds bounds, Camera camera)
        {
            Vector3 aabbCenter = bounds.center;
            Vector3 aabbExtents = bounds.extents;

            //  Return the screen space point array
            return new Vector2[]
            {
            camera.WorldToScreenPoint(new Vector3(aabbCenter.x - aabbExtents.x, aabbCenter.y - aabbExtents.y, aabbCenter.z - aabbExtents.z)),
            camera.WorldToScreenPoint(new Vector3(aabbCenter.x + aabbExtents.x, aabbCenter.y - aabbExtents.y, aabbCenter.z - aabbExtents.z)),
            camera.WorldToScreenPoint(new Vector3(aabbCenter.x + aabbExtents.x, aabbCenter.y + aabbExtents.y, aabbCenter.z - aabbExtents.z)),
            camera.WorldToScreenPoint(new Vector3(aabbCenter.x - aabbExtents.x, aabbCenter.y + aabbExtents.y, aabbCenter.z - aabbExtents.z)),

            camera.WorldToScreenPoint(new Vector3(aabbCenter.x - aabbExtents.x, aabbCenter.y - aabbExtents.y, aabbCenter.z + aabbExtents.z)),
            camera.WorldToScreenPoint(new Vector3(aabbCenter.x + aabbExtents.x, aabbCenter.y - aabbExtents.y, aabbCenter.z + aabbExtents.z)),
            camera.WorldToScreenPoint(new Vector3(aabbCenter.x + aabbExtents.x, aabbCenter.y + aabbExtents.y, aabbCenter.z + aabbExtents.z)),
            camera.WorldToScreenPoint(new Vector3(aabbCenter.x - aabbExtents.x, aabbCenter.y + aabbExtents.y, aabbCenter.z + aabbExtents.z))
            };
        }

        /// <summary>
        /// Returns the rectangle which encloses the specifies 'Bounds' instance in screen space.
        /// </summary>
        public static Rect GetScreenRectangle(this Bounds bounds, Camera camera)
        {
            // Retrieve the bounds' corner points in screen space
            Vector2[] screenSpaceCornerPoints = bounds.GetScreenSpaceCornerPoints(camera);

            // Identify the minimum and maximum points in the array
            Vector3 minScreenPoint = screenSpaceCornerPoints[0], maxScreenPoint = screenSpaceCornerPoints[0];
            for (int screenPointIndex = 1; screenPointIndex < screenSpaceCornerPoints.Length; ++screenPointIndex)
            {
                minScreenPoint = Vector3.Min(minScreenPoint, screenSpaceCornerPoints[screenPointIndex]);
                maxScreenPoint = Vector3.Max(maxScreenPoint, screenSpaceCornerPoints[screenPointIndex]);
            }

            // Return the screen space rectangle
            return new Rect(minScreenPoint.x, minScreenPoint.y, maxScreenPoint.x - minScreenPoint.x, maxScreenPoint.y - minScreenPoint.y);
        }

        /// <summary>
        /// Returns the volume of the bounds.
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public static float Volume(this Bounds bounds)
        {
            return bounds.size.x * bounds.size.y * bounds.size.z;
        }

        /// <summary>
        /// Returns bounds that contain both this bounds and the bounds passed in.
        /// </summary>
        /// <param name="originalBounds"></param>
        /// <param name="otherBounds"></param>
        /// <returns></returns>
        public static Bounds ExpandToContain(this Bounds originalBounds, Bounds otherBounds)
        {
            Bounds tmpBounds = originalBounds;

            tmpBounds.Encapsulate(otherBounds);

            return tmpBounds;
        }

        /// <summary>
        /// Checks to see if bounds contains the other bounds completely.
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="otherBounds"></param>
        /// <returns></returns>
        public static bool ContainsBounds(this Bounds bounds, Bounds otherBounds)
        {
            return bounds.Contains(otherBounds.min) && bounds.Contains(otherBounds.max);
        }

        /// <summary>
        /// Checks to see whether point is closer to bounds or otherBounds
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="point"></param>
        /// <param name="otherBounds"></param>
        /// <returns></returns>
        public static bool CloserToPoint(this Bounds bounds, Vector3 point, Bounds otherBounds)
        {
            Vector3 distToClosestPoint1 = bounds.ClosestPoint(point) - point;
            Vector3 distToClosestPoint2 = otherBounds.ClosestPoint(point) - point;

            if (distToClosestPoint1.magnitude == distToClosestPoint2.magnitude)
            {
                Vector3 toCenter1 = point - bounds.center;
                Vector3 toCenter2 = point - otherBounds.center;
                return (toCenter1.magnitude <= toCenter2.magnitude);

            }

            return (distToClosestPoint1.magnitude <= distToClosestPoint2.magnitude);
        }

        #endregion

        #region Private Static Functions
        /// <summary>
        /// Returns the vector which is used to represent and invalid bounds size.
        /// </summary>
        private static Vector3 GetInvalidBoundsSize()
        {
            return new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        }
        #endregion
    }
}