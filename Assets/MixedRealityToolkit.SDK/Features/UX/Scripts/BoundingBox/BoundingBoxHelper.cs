// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// The BoundingBoxHelper class contains functions for getting geometric info from the non-axis-aligned 
    /// bounding box of a GameObject. These functions can be used to align another object to the center of
    /// a certain face or the center of an edge of a face... etc.
    /// The BoundingBoxHelper static function can be used for a one time calculation.
    /// The dynamic functions can be used to obtain boundingcube info on an object's Update loop. Operations
    /// are minimized in the dynamic use scenario.
    /// </summary>
    public class BoundingBoxHelper
    {
        readonly int[] face0 = { 0, 1, 3, 2 };
        readonly int[] face1 = { 1, 5, 7, 3 };
        readonly int[] face2 = { 5, 4, 6, 7 };
        readonly int[] face3 = { 4, 0, 2, 6 };
        readonly int[] face4 = { 6, 2, 3, 7 };
        readonly int[] face5 = { 1, 0, 4, 5 };
        readonly int[] noFaceIndices = { };
        readonly Vector3[] noFaceVertices = { };

        private Vector3[] face = new Vector3[4];
        private Vector3[] midpoints = new Vector3[4];
        private List<Vector3> rawBoundingCorners = new List<Vector3>();
        private List<Vector3> worldBoundingCorners = new List<Vector3>();
        private BoxCollider targetBounds;
        private bool rawBoundingCornersObtained = false;


        /// <summary>
        /// Objects that align to an target's bounding box can call this function in the object's UpdateLoop
        /// to get current bound points;
        /// </summary>
        [Obsolete("Use UpdateNonAABoundsCornerPositions and pass in TargetBounds")]
        public void UpdateNonAABoundingBoxCornerPositions(BoundingBox boundingBox, List<Vector3> boundsPoints)
        {
            UpdateNonAABoundsCornerPositions(boundingBox.TargetBounds, boundsPoints);
        }
        /// <summary>
        /// Returns the corner points of the given collider bounds
        /// </summary>
        /// <param name="colliderBounds">The collider bounds the corner points are calculated from</param>
        /// <param name="boundsPoints">The corner points calculated from the collider points</param>
        public void UpdateNonAABoundsCornerPositions(BoxCollider colliderBounds, List<Vector3> boundsPoints)
        {
            if (colliderBounds != targetBounds || rawBoundingCornersObtained == false)
            {
                GetRawBoundsCorners(colliderBounds);
            }

            if (colliderBounds == targetBounds && rawBoundingCornersObtained)
            {
                boundsPoints.Clear();
                for (int i = 0; i < rawBoundingCorners.Count; ++i)
                {
                    boundsPoints.Add(colliderBounds.transform.localToWorldMatrix.MultiplyPoint(rawBoundingCorners[i]));
                }

                worldBoundingCorners.Clear();
                worldBoundingCorners.AddRange(boundsPoints);
            }
        }

        /// <summary>
        /// This function calculates the untransformed bounding box corner points of a GameObject.
        /// </summary>
        [Obsolete("Use GetRawBBCorners and pass in TargetBounds")]
        public void GetRawBBCorners(BoundingBox boundingBox)
        {
            GetRawBoundsCorners(boundingBox.TargetBounds);
        }

        /// <summary>
        /// Calculates the untransformed corner points of the given collider bounds
        /// </summary>
        /// <param name="colliderBounds">The collider bounds the corner points are calculated from.</param>
        internal void GetRawBoundsCorners(BoxCollider colliderBounds)
        {
            targetBounds = colliderBounds;
            rawBoundingCorners.Clear();
            rawBoundingCornersObtained = false;

            GetUntransformedCornersFromObject(colliderBounds, rawBoundingCorners);

            if (rawBoundingCorners != null && rawBoundingCorners.Count >= 4)
            {
                rawBoundingCornersObtained = true;
            }
        }

        /// <summary>
        /// this function gets the indices of the bounding cube corners that make up a face.
        /// </summary>
        /// <param name="index">the face index of the bounding cube 0-5</param>
        /// <returns>an array of four integer indices</returns>
        public int[] GetFaceIndices(int index)
        {
            switch (index)
            {
                case 0:
                    return face0;
                case 1:
                    return face1;
                case 2:
                    return face2;
                case 3:
                    return face3;
                case 4:
                    return face4;
                case 5:
                    return face5;
            }

            return noFaceIndices;
        }

        /// <summary>
        /// This function returns the midpoints of each of the edges of the face of the bounding box
        /// </summary>
        /// <param name="index">the index of the face of the bounding cube- 0-5</param>
        /// <returns>four Vector3 points</returns>
        public Vector3[] GetFaceEdgeMidpoints(int index)
        {
            Vector3[] corners = GetFaceCorners(index);
            midpoints[0] = (corners[0] + corners[1]) * 0.5f;
            midpoints[1] = (corners[1] + corners[2]) * 0.5f;
            midpoints[2] = (corners[2] + corners[3]) * 0.5f;
            midpoints[3] = (corners[3] + corners[0]) * 0.5f;

            return midpoints;
        }

        /// <summary>
        /// Get the normal of the face of the bounding cube specified by index
        /// </summary>
        /// <param name="index">the index of the face of the bounding cube 0-5</param>
        /// <returns>a vector3 representing the face normal</returns>
        public Vector3 GetFaceNormal(int index)
        {
            int[] face = GetFaceIndices(index);

            if (face.Length == 4)
            {
                Vector3 ab = (worldBoundingCorners[face[1]] - worldBoundingCorners[face[0]]).normalized;
                Vector3 ac = (worldBoundingCorners[face[2]] - worldBoundingCorners[face[0]]).normalized;
                return Vector3.Cross(ab, ac).normalized;
            }

            return Vector3.zero;
        }

        /// <summary>
        /// This function returns the centroid of a face of the bounding cube of an object specified
        /// by the index parameter;
        /// </summary>
        /// <param name="index">an index into the list of faces of a boundingcube. 0-5</param>
        public Vector3 GetFaceCentroid(int index)
        {
            int[] faceIndices = GetFaceIndices(index);

            if (faceIndices.Length == 4)
            {
                return (worldBoundingCorners[faceIndices[0]] +
                        worldBoundingCorners[faceIndices[1]] +
                        worldBoundingCorners[faceIndices[2]] +
                        worldBoundingCorners[faceIndices[3]]) * 0.25f;
            }

            return Vector3.zero;
        }

        /// <summary>
        /// Get the center of the bottom edge of a face of the bounding box determined by index
        /// </summary>
        /// <param name="index">parameter indicating which face is used. 0-5</param>
        /// <returns>a vector representing the bottom most edge center of the face</returns>
        public Vector3 GetFaceBottomCentroid(int index)
        {
            Vector3[] edgeCentroids = GetFaceEdgeMidpoints(index);

            Vector3 leastYPoint = edgeCentroids[0];
            for (int i = 1; i < 4; ++i)
            {
                leastYPoint = edgeCentroids[i].y < leastYPoint.y ? edgeCentroids[i] : leastYPoint;
            }
            return leastYPoint;
        }

        /// <summary>
        /// This function returns the four corners of a face of a bounding cube specified by index.
        /// </summary>
        /// <param name="index">the index of the face of the bounding cube. 0-5</param>
        /// <returns>an array of 4 vectors</returns>
        public Vector3[] GetFaceCorners(int index)
        {
            int[] faceIndices = GetFaceIndices(index);

            if (faceIndices.Length == 4)
            {
                face[0] = worldBoundingCorners[faceIndices[0]];
                face[1] = worldBoundingCorners[faceIndices[1]];
                face[2] = worldBoundingCorners[faceIndices[2]];
                face[3] = worldBoundingCorners[faceIndices[3]];
                return face;
            }

            return noFaceVertices;
        }

        /// <summary>
        /// This function gets the index of the face of the bounding cube that is most facing the lookAtPoint.
        /// This could be the headPosition or camera position if the face that was facing the view is desired.
        /// </summary>
        /// <param name="lookAtPoint">the world coordinate to test which face is desired</param>
        /// <returns>an integer representing the index of the bounding box faces</returns>
        public int GetIndexOfForwardFace(Vector3 lookAtPoint)
        {
            int highestDotIndex = -1;
            float hightestDotValue = float.MinValue;
            for (int i = 0; i < 6; ++i)
            {
                Vector3 a = (lookAtPoint - GetFaceCentroid(i)).normalized;
                Vector3 b = GetFaceNormal(i);
                float dot = Vector3.Dot(a, b);
                if (hightestDotValue < dot)
                {
                    hightestDotValue = dot;
                    highestDotIndex = i;
                }
            }
            return highestDotIndex;
        }

        /// <summary>
        /// static function that performs one-time non-persistent calculation of corner points of given bounds 
        /// without taking world transform into account.
        /// </summary>
        /// <param name="targetBounds">the bounds the corner points are to be calculated from</param>
        /// <param name="boundsPoints">the array of 8 corner points that will be filled</param>
        public static void GetUntransformedCornersFromObject(BoxCollider targetBounds, List<Vector3> boundsPoints)
        {
            Bounds cloneBounds = new Bounds(targetBounds.center, targetBounds.size);
            Vector3[] corners = null;
            cloneBounds.GetCornerPositions(ref corners);
            boundsPoints.AddRange(corners);
        }
    }
}
