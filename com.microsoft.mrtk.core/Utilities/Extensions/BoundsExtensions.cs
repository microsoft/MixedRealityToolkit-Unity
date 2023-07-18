﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Extensions for Unity's Bounds struct.
    /// </summary>
    public static class BoundsExtensions
    {
        //
        // Corners
        //

        /// <summary>
        /// A value representing the bottom-left corner on the 3D object's front.
        /// </summary>
        public const int LBF = 0;

        /// <summary>
        /// A value representing the bottom-left corner on the 3D object's back.
        /// </summary>
        public const int LBB = 1;

        /// <summary>
        /// A value representing the top-left corner on the 3D object's front.
        /// </summary>
        public const int LTF = 2;

        /// <summary>
        /// A value representing the top-left corner on the 3D object's back.
        /// </summary>
        public const int LTB = 3;

        /// <summary>
        /// A value representing the bottom-right corner on the 3D object's front.
        /// </summary>
        public const int RBF = 4;

        /// <summary>
        /// A value representing the bottom-right corner on the 3D object's back.
        /// </summary>
        public const int RBB = 5;

        /// <summary>
        /// A value representing the top-right corner on the 3D object's front.
        /// </summary>
        public const int RTF = 6;

        /// <summary>
        /// A value representing the top-right corner on the 3D object's back.
        /// </summary>
        public const int RTB = 7;

        //
        // X axis
        //

        /// <summary>
        /// A value representing the top edge on's midpoint the 3D object's front.
        /// </summary>
        public const int LTF_RTF = 8;

        /// <summary>
        /// A value representing the bottom edge's midpoint on the 3D object's front.
        /// </summary>
        public const int LBF_RBF = 9;

        /// <summary>
        /// A value representing the top edge's midpoint on the 3D object's back.
        /// </summary>
        public const int RTB_LTB = 10;

        /// <summary>
        /// A value representing the bottom edge's midpoint on the 3D object's back.
        /// </summary>
        public const int RBB_LBB = 11;

        //
        // Y axis
        //

        /// <summary>
        /// A value representing the left edge's midpoint on the 3D object's front.
        /// </summary>
        public const int LTF_LBF = 12;

        /// <summary>
        /// A value representing the right edge's midpoint on the 3D object's back.
        /// </summary>
        public const int RTB_RBB = 13;

        /// <summary>
        /// A value representing the left edge's midpoint on the 3D object's back.
        /// </summary>
        public const int LTB_LBB = 14;
        
        /// <summary>
        /// A value representing the right edge's midpoint on the 3D object's front.
        /// </summary>
        public const int RTF_RBF = 15;

        //
        // Z axis
        //
        
        /// <summary>
        /// A value representing the bottom-right edge's midpoint on the 3D object's side.
        /// </summary>
        public const int RBF_RBB = 16;
        
        /// <summary>
        /// A value representing the top-right edge's midpoint on the 3D object's side.
        /// </summary>
        public const int RTF_RTB = 17;

        /// <summary>
        /// A value representing the bottom-left edge's midpoint on the 3D object's side.
        /// </summary>
        public const int LBF_LBB = 18;

        /// <summary>
        /// A value representing the top-left edge's midpoint on the 3D object's side.
        /// </summary>
        public const int LTF_LTB = 19;

        //
        // 2D corners
        //
        
        /// <summary>
        /// A value representing the top-left corner of a 2D object.
        /// </summary>
        public const int LT = 0;

        /// <summary>
        /// A value representing the bottom-left corner of a 2D object.
        /// </summary>
        public const int LB = 1;

        /// <summary>
        /// A value representing the top-right corner of a 2D object.
        /// </summary>
        public const int RT = 2;

        /// <summary>
        /// A value representing the bottom-right corner of a 2D object.
        /// </summary>
        public const int RB = 3;

        //
        // 2D midpoints
        //
        
        /// <summary>
        /// A value representing the top edge's midpoint on a 2D object.
        /// </summary>
        public const int LT_RT = 4;

        /// <summary>
        /// A value representing the right edge's midpoint on a 2D object.
        /// </summary>
        public const int RT_RB = 5;

        /// <summary>
        /// A value representing the bottom edge's midpoint on a 2D object.
        /// </summary>
        public const int RB_LB = 6;

        /// <summary>
        /// A value representing the left edge's midpoint on a 2D object.
        /// </summary>
        public const int LB_LT = 7;

        //
        // Face points
        //

        /// <summary>
        /// A value representing the top face on a 3D object.
        /// </summary>
        public const int TOP = 0;

        /// <summary>
        /// A value representing the bottom face on a 3D object.
        /// </summary>
        public const int BOT = 1;

        /// <summary>
        /// A value representing the left face on a 3D object.
        /// </summary>
        public const int LFT = 2;

        /// <summary>
        /// A value representing the right face on a 3D object.
        /// </summary>
        public const int RHT = 3;

        /// <summary>
        /// A value representing the front face on a 3D object.
        /// </summary>
        public const int FWD = 4;

        /// <summary>
        /// A value representing the back face on a 3D object.
        /// </summary>
        public const int BCK = 5;

        // Axis of the capsule’s lengthwise orientation in the object’s local space
        private const int CAPSULE_X_AXIS = 0;
        private const int CAPSULE_Y_AXIS = 1;
        private const int CAPSULE_Z_AXIS = 2;

        // Edges used to render the bounds.
        private static readonly int[] boundsEdges = new int[]
        {
             LBF, LBB,
             LBB, LTB,
             LTB, LTF,
             LTF, LBF,
             LBF, RTB,
             RTB, RTF,
             RTF, RBF,
             RBF, RBB,
             RBB, RTB,
             RTF, LBB,
             RBF, LTB,
             RBB, LTF
        };

        private static Vector3[] corners = null;

        private static Vector3[] rectTransformCorners = new Vector3[4];

        #region Public Static Functions

        /// <summary>
        /// Returns a <see cref="Bounds"/> structure that is invalid.
        /// </summary>
        /// <remarks>
        /// An invalid <see cref="Bounds"/> is one which has its size vector set to <see cref="float.MaxValue"/> for 
        /// all <see cref="Bounds"/> components. The center of an invalid bounds instance is the zero vector.
        /// </remarks>
        public static Bounds GetInvalidBoundsInstance()
        {
            return new Bounds(Vector3.zero, GetInvalidBoundsSize());
        }

        /// <summary>
        /// Checks if the specified <see cref="Bounds"/> is valid. 
        /// </summary>
        /// <remarks>
        /// A valid <see cref="Bounds"/> instance is one whose size vector does not have all <see cref="Bounds"/>
        /// components set to <see cref="float.MaxValue"/>.
        /// </remarks>
        public static bool IsValid(this Bounds bounds)
        {
            return bounds.size != GetInvalidBoundsSize();
        }

        /// <summary>
        /// Gets all the corner points of a <see cref="Bounds"/> structure.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The resulting 3D positions will be in world space by transforming input bounds using the given transform.
        /// </para>
        /// <para>
        /// Use <see cref="LBF"/>, <see cref="LBB"/>, <see cref="LTF"/>, <see cref="LTB"/>, <see cref="RBF"/>, 
        /// <see cref="RBB"/>, <see cref="RTF"/>, and <see cref="RTB"/> values to index into the resulting array.
        /// </para>
        /// </remarks>
        /// <param name="transform">Local to world transform</param>
        /// <param name="positions">Output corner positions</param>
        /// <param name="bounds">Input bounds, in local space</param>
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
        /// Gets all the corner points  a <see cref="Bounds"/> structure.
        /// </summary>        
        /// <remarks>
        /// Use <see cref="LBF"/>, <see cref="LBB"/>, <see cref="LTF"/>, <see cref="LTB"/>, <see cref="RBF"/>, 
        /// <see cref="RBB"/>, <see cref="RTF"/>, and <see cref="RTB"/> values to index into the resulting array.
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
        /// Gets all the corner points from a <see cref="Bounds"/> structure.
        /// </summary>
        /// <remarks>
        /// Use <see cref="LBF"/>, <see cref="LBB"/>, <see cref="LTF"/>, <see cref="LTB"/>, <see cref="RBF"/>, 
        /// <see cref="RBB"/>, <see cref="RTF"/>, and <see cref="RTB"/> values to index into the resulting array.
        /// </remarks>
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

        /// <summary>
        /// Get the 3D positions of faces from a <see cref="Bounds"/> structure.
        /// </summary>
        /// <remarks>
        /// The 3D positions are stored in <see cref="Vector3"/> array at the <see cref="TOP"/>, <see cref="BOT"/>,
        /// <see cref="LFT"/>, <see cref="RHT"/>, <see cref="FWD"/>, and <see cref="BCK"/> indices.
        /// </remarks>
        /// <param name="bounds">The bounds structure in local space.</param>
        /// <param name="transform">The transform belonging to the bounds.</param>
        /// <param name="positions">The array will hold the 3D positions of faces on the <see cref="Bounds"/> object.</param>
        public static void GetFacePositions(this Bounds bounds, Transform transform, ref Vector3[] positions)
        {
            Vector3 center = bounds.center;
            Vector3 extents = bounds.extents;

            const int NumPoints = 6;
            if (positions == null || positions.Length != NumPoints)
            {
                positions = new Vector3[NumPoints];
            }

            positions[TOP] = transform.TransformPoint(center + Vector3.up * extents.y);
            positions[BOT] = transform.TransformPoint(center + Vector3.down * extents.y);
            positions[LFT] = transform.TransformPoint(center + Vector3.left * extents.x);
            positions[RHT] = transform.TransformPoint(center + Vector3.right * extents.x);
            positions[FWD] = transform.TransformPoint(center + Vector3.forward * extents.z);
            positions[BCK] = transform.TransformPoint(center + Vector3.back * extents.z);
        }

        /// <summary>
        /// Gets all the corner points and mid points from a <see cref="Bounds"/> structure.
        /// </summary>
        /// <remarks>
        /// Use <see cref="LBF"/>, <see cref="LBB"/>, <see cref="LTF"/>, <see cref="LTB"/>, <see cref="RBF"/>, 
        /// <see cref="RBB"/>, <see cref="RTF"/>, <see cref="RTB"/>, <see cref="LTF_RTF"/>, <see cref="LBF_RBF"/>,
        /// <see cref="RTB_LTB"/>, <see cref="RBB_LBB"/>, <see cref="LTF_LBF"/>, <see cref="RTB_RBB"/>, 
        /// <see cref="LTB_LBB"/>, <see cref="RTF_RBF"/>, <see cref="RBF_RBB"/>, <see cref="RTF_RTB"/>,
        /// <see cref="LBF_LBB"/>, <see cref="LTF_LTB"/> values to index into the resulting array.
        /// </remarks>
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
        /// Gets all the corner points and mid points from a <see cref="Bounds"/> structure, ignoring the z axis.
        /// </summary>
        /// <remarks>
        /// Use <see cref="LT"/>, <see cref="LB"/>, <see cref="RT"/>, <see cref="RB"/>, <see cref="LT_RT"/>, 
        /// <see cref="RT_RB"/>, <see cref="RB_LB"/>, and <see cref="LB_LT"/> values to index into the resulting array.
        /// </remarks>
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
        /// Method to get bounds from a collection of points.
        /// </summary>
        /// <remarks>
        /// If zero points are present, bounds will not be calculated.
        /// </remarks>
        /// <param name="points">The points to construct a bounds around.</param>
        /// <param name="bounds">A bounds in world space around all the points.</param>
        /// <returns><see langword="true"/> if bounds were calculated, otherwise <see langword="false"/>.</returns>
        public static bool GetPointsBounds(List<Vector3> points, out Bounds bounds)
        {
            if (points.Count != 0)
            {
                bounds = new Bounds(points[0], Vector3.zero);

                for (var i = 1; i < points.Count; ++i)
                {
                    bounds.Encapsulate(points[i]);
                }

                return true;
            }

            bounds = new Bounds();
            return false;
        }

        /// <summary>
        /// Method to get bounds using <see cref="Collider"/> methods.
        /// </summary>
        /// <remarks>
        /// If zero colliders are present, bounds will not be calculated.
        /// </remarks>
        /// <param name="target">GameObject to generate the bounds around.</param>
        /// <param name="bounds">A bounds in world space around all the colliders in a gameObject hierarchy.</param>
        /// <param name="ignoreLayers">A layer mask to restrict the colliders selected.</param>
        /// <returns><see langword="true"/> if bounds were calculated, otherwise <see langword="false"/>.</returns>
        public static bool GetColliderBounds(GameObject target, out Bounds bounds, LayerMask ignoreLayers)
        {
            var boundsPoints = new List<Vector3>();
            GetColliderBoundsPoints(target, boundsPoints, ignoreLayers);

            return GetPointsBounds(boundsPoints, out bounds);
        }

        /// <summary>
        /// Calculates how much scale is required for this <see cref="Bounds"/> to match another <see cref="Bounds"/>.
        /// </summary>
        /// <param name="bounds">The bounds structure to examine.</param>
        /// <param name="otherBounds">Object representation to be scaled to</param>
        /// <param name="padding">padding multiplied into another bounds</param>
        /// <returns>Scale represented as a 3D position.</returns>
        public static Vector3 GetScaleToMatchBounds(this Bounds bounds, Bounds otherBounds, Vector3 padding = default(Vector3))
        {
            Vector3 szA = otherBounds.size + new Vector3(otherBounds.size.x * padding.x, otherBounds.size.y * padding.y, otherBounds.size.z * padding.z);
            Vector3 szB = bounds.size;
            Assert.IsTrue(szB.x != 0 && szB.y != 0 && szB.z != 0, "The bounds of the object must not be zero.");
            return new Vector3(szA.x / szB.x, szA.y / szB.y, szA.z / szB.z);
        }

        /// <summary>
        /// Calculates how much scale is required for this <see cref="Bounds"/> to fit inside another bounds without stretching.
        /// </summary>
        /// <param name="bounds">The bounds structure to examine.</param>
        /// <param name="containerBounds">The bounds of the container attempting to be fit into this object.</param>
        /// <returns>A single scale factor that can be applied to this object to fit inside the container.</returns>
        public static float GetScaleToFitInside(this Bounds bounds, Bounds containerBounds)
        {
            var objectSize = bounds.size;
            var containerSize = containerBounds.size;
            Assert.IsTrue(objectSize.x != 0 && objectSize.y != 0 && objectSize.z != 0, "The bounds of the container must not be zero.");
            return Mathf.Min(containerSize.x / objectSize.x, containerSize.y / objectSize.y, containerSize.z / objectSize.z);
        }

        /// <summary>
        /// Method to get bounding box points using <see cref="Collider"/> methods.
        /// </summary>
        /// <param name="target">A game object that the bounding box bounds.</param>
        /// <param name="boundsPoints">An array reference that gets filled with 3D positions.</param>
        /// <param name="ignoreLayers">A layer mask used to simplify the search.</param>
        /// <param name="relativeTo">Compute bounds relative to this transform.</param>
        public static void GetColliderBoundsPoints(GameObject target, List<Vector3> boundsPoints, LayerMask ignoreLayers, Transform relativeTo = null)
        {
            Collider[] colliders = target.GetComponentsInChildren<Collider>();
            for (int i = 0; i < colliders.Length; i++)
            {
                GetColliderBoundsPoints(colliders[i], boundsPoints, ignoreLayers, relativeTo);
            }
        }

        private static void InverseTransformPoints(ref Vector3[] positions, Transform relativeTo)
        {
            if (relativeTo)
            {
                for (var i = 0; i < positions.Length; ++i)
                {
                    positions[i] = relativeTo.InverseTransformPoint(positions[i]);
                }
            }
        }

        /// <summary>
        /// Method to get bounds from a single <see cref="Collider"/> structure.
        /// </summary>
        /// <param name="collider">Target collider structure to examine.</param>
        /// <param name="boundsPoints">The array reference that gets filled with 3D positions.</param>
        /// <param name="ignoreLayers">The layer mask to simplify search.</param>
        /// <param name="relativeTo">Compute bounds relative to this transform.</param>
        public static void GetColliderBoundsPoints(Collider collider, List<Vector3> boundsPoints, LayerMask ignoreLayers, Transform relativeTo = null)
        {
            if (ignoreLayers == (1 << collider.gameObject.layer | ignoreLayers)) { return; }

            if (collider is SphereCollider)
            {
                SphereCollider sc = collider as SphereCollider;
                Bounds sphereBounds = new Bounds(sc.center, 2 * sc.radius * Vector3.one);
                sphereBounds.GetFacePositions(sc.transform, ref corners);
                InverseTransformPoints(ref corners, relativeTo);
                boundsPoints.AddRange(corners);
            }
            else if (collider is BoxCollider)
            {
                BoxCollider bc = collider as BoxCollider;
                Bounds boxBounds = new Bounds(bc.center, bc.size);
                boxBounds.GetCornerPositions(bc.transform, ref corners);
                InverseTransformPoints(ref corners, relativeTo);
                boundsPoints.AddRange(corners);

            }
            else if (collider is MeshCollider)
            {
                MeshCollider mc = collider as MeshCollider;
                Bounds meshBounds = mc.sharedMesh.bounds;
                meshBounds.GetCornerPositions(mc.transform, ref corners);
                InverseTransformPoints(ref corners, relativeTo);
                boundsPoints.AddRange(corners);
            }
            else if (collider is CapsuleCollider)
            {
                CapsuleCollider cc = collider as CapsuleCollider;
                Bounds capsuleBounds = new Bounds(cc.center, Vector3.zero);
                switch (cc.direction)
                {
                    case CAPSULE_X_AXIS:
                        capsuleBounds.size = new Vector3(cc.height, cc.radius * 2, cc.radius * 2);
                        break;

                    case CAPSULE_Y_AXIS:
                        capsuleBounds.size = new Vector3(cc.radius * 2, cc.height, cc.radius * 2);
                        break;

                    case CAPSULE_Z_AXIS:
                        capsuleBounds.size = new Vector3(cc.radius * 2, cc.radius * 2, cc.height);
                        break;
                }
                capsuleBounds.GetFacePositions(cc.transform, ref corners);
                InverseTransformPoints(ref corners, relativeTo);
                boundsPoints.AddRange(corners);
            }
        }

        /// <summary>
        /// Get the bounds using <see cref="Renderer"/> methods.
        /// </summary>
        /// <remarks>
        /// If zero <see cref="Renderer"/> objects are present, that bounds will not be calculated.
        /// </remarks>
        /// <param name="target">A game object to generate the bounds around.</param>
        /// <param name="bounds">A bounds in world space around all the renderers in a game object hierarchy.</param>
        /// <param name="ignoreLayers">A LayerMask to restrict the colliders selected.</param>
        /// <returns><see langword="true"/> if bounds were calculated, otherwise <see langword="false"/>.</returns>
        public static bool GetRenderBounds(GameObject target, out Bounds bounds, LayerMask ignoreLayers)
        {
            var boundsPoints = new List<Vector3>();
            GetRenderBoundsPoints(target, boundsPoints, ignoreLayers);

            return GetPointsBounds(boundsPoints, out bounds);
        }

        /// <summary>
        /// Gets the bounding box points using <see cref="Renderer"/> methods.
        /// </summary>
        /// <param name="target">The game object that the bounding box bounds.</param>
        /// <param name="boundsPoints">An array reference that gets filled with 3D positions.</param>
        /// <param name="ignoreLayers">A layer mask used to simplify search.</param>
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
        /// Method to get bounds using <see cref="MeshFilter"/> methods.
        /// </summary>
        /// <remarks>
        /// If zero <see cref="MeshFilter"/> objects are present, a bounds will not be calculated.
        /// </remarks>
        /// <param name="target">The game object to generate the bounds around.</param>
        /// <param name="bounds">A bounds in world space around all the mesh filters in a <see cref="GameObject"/> hierarchy.</param>
        /// <param name="ignoreLayers">A layer mask used to restrict the colliders selected.</param>
        /// <returns><see langword="true"/> if bounds were calculated, otherwise <see langword="false"/>.</returns>
        public static bool GetMeshFilterBounds(GameObject target, out Bounds bounds, LayerMask ignoreLayers)
        {
            var boundsPoints = new List<Vector3>();
            GetMeshFilterBoundsPoints(target, boundsPoints, ignoreLayers);

            return GetPointsBounds(boundsPoints, out bounds);
        }

        /// <summary>
        /// Get the bounding box points using <see cref="MeshFilter"/> methods.
        /// </summary>
        /// <param name="target">The game object that bounding box bounds.</param>
        /// <param name="boundsPoints">An array reference that gets filled with points.</param>
        /// <param name="ignoreLayers">A layer mask to simplify search.</param>
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
        /// Transforms a <see cref="Bounds"/> using the specified transform matrix.
        /// </summary>
        /// <remarks>
        /// <para>Transforming a <see cref="Bounds"/> means that the function will construct a new <see cref="Bounds"/> 
        /// which has its center translated using the translation information stored in
        /// the specified matrix and its size adjusted to account for rotation and scale. The size
        /// of the new <see cref="Bounds"/> structure will be calculated in such a way that it will contain the
        /// old <see cref="Bounds"/>.</para>
        /// </remarks>
        /// <param name="bounds">
        /// The <see cref="Bounds"/> which must be transformed.
        /// </param>
        /// <param name="transformMatrix">
        /// The specified <see cref="Bounds"/> will be transformed using this transform matrix. The function
        /// assumes that the matrix doesn't contain any projection or skew transformation.
        /// </param>
        /// <returns>
        /// A transformed <see cref="Bounds"/> structure.
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

            // Construct the transformed <see cref="Bounds"/> instance
            var transformedBounds = new Bounds();
            transformedBounds.center = transformMatrix.MultiplyPoint(bounds.center);
            transformedBounds.size = new Vector3(newSizeX, newSizeY, newSizeZ);

            // Return the instance to the caller
            return transformedBounds;
        }

        /// <summary>
        /// Returns the screen space corner points of the specified <see cref="Bounds"/> instance.
        /// </summary>
        /// <param name="bounds">The bounds structure to examine.</param>
        /// <param name="camera">
        /// The camera used for rendering to the screen. This is needed to perform the
        /// transformation to screen space.
        /// </param>
        public static Vector2[] GetScreenSpaceCornerPoints(this Bounds bounds, Camera camera)
        {
            Vector3 aabbCenter = bounds.center;
            Vector3 aabbExtents = bounds.extents;

            // Return the screen space point array
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
        /// Returns the rectangle which encloses the specifies <see cref="Bounds"/> instance in screen space.
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
        public static float Volume(this Bounds bounds)
        {
            return bounds.size.x * bounds.size.y * bounds.size.z;
        }

        /// <summary>
        /// Returns bounds that contain both this bounds and the bounds passed in.
        /// </summary>
        public static Bounds ExpandToContain(this Bounds originalBounds, Bounds otherBounds)
        {
            Bounds tmpBounds = originalBounds;

            tmpBounds.Encapsulate(otherBounds);

            return tmpBounds;
        }

        /// <summary>
        /// Checks to see if bounds contains the other bounds completely.
        /// </summary>
        public static bool ContainsBounds(this Bounds bounds, Bounds otherBounds)
        {
            return bounds.Contains(otherBounds.min) && bounds.Contains(otherBounds.max);
        }

        /// <summary>
        /// Checks to see whether point is closer to bounds or otherBounds
        /// </summary>
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

        /// <summary>
        /// Draws a wire frame <see href="https://docs.unity3d.com/ScriptReference/Bounds.html">Bounds</see> object using <see href="https://docs.unity3d.com/ScriptReference/Debug.DrawLine.html">Debug.DrawLine</see>.
        /// </summary>
        /// <param name="bounds">The <see href="https://docs.unity3d.com/ScriptReference/Bounds.html">Bounds</see> to draw.</param>
        /// <param name="color">Color of the line.</param>
        /// <param name="duration">How long the line should be visible for in seconds.</param>
        /// <param name="depthTest">Should the line be obscured by objects closer to the camera?</param>
        public static void DebugDraw(this Bounds bounds, Color color, float duration = 0.0f, bool depthTest = true)
        {
            var center = bounds.center;
            var x = bounds.extents.x;
            var y = bounds.extents.y;
            var z = bounds.extents.z;
            var a = new Vector3(-x, y, -z);
            var b = new Vector3(x, -y, -z);
            var c = new Vector3(x, y, -z);

            var vertices = new Vector3[]
            {
                bounds.min, center + a, center + b, center + c,
                bounds.max, center - a, center - b, center - c
            };

            for (var i = 0; i < boundsEdges.Length; i += 2)
            {
                Debug.DrawLine(vertices[boundsEdges[i]], vertices[boundsEdges[i + 1]], color, duration, depthTest);
            }
        }

        /// <summary>
        /// Calculate the intersection area between the rectangle and another.
        /// </summary>
        public static bool Intersects(this Rect thisRect, Rect rect, out Rect area)
        {
            // https://forum.unity.com/threads/getting-the-area-rect-of-intersection-between-two-rectangles.299140/
            area = new Rect();

            if (rect.Overlaps(thisRect))
            {
                float x1 = Mathf.Min(thisRect.xMax, rect.xMax);
                float x2 = Mathf.Max(thisRect.xMin, rect.xMin);
                float y1 = Mathf.Min(thisRect.yMax, rect.yMax);
                float y2 = Mathf.Max(thisRect.yMin, rect.yMin);
                area.x = Mathf.Min(x1, x2);
                area.y = Mathf.Min(y1, y2);
                area.width = Mathf.Max(0.0f, x1 - x2);
                area.height = Mathf.Max(0.0f, y1 - y2);

                return true;
            }

            return false;
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
