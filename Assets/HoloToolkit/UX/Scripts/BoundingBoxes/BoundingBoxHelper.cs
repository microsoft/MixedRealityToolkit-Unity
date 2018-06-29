// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Unity.UX
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

        private List<Vector3> rawBoundingCorners = new List<Vector3>();
        private List<Vector3> worldBoundingCorners = new List<Vector3>();
        private GameObject targetObject;
        private bool rawBoundingCornersObtained = false;

        /// <summary>
        /// Objects that align to an target's bounding box can call this function in the object's UpdateLoop
        /// to get current bound points;
        /// </summary>
        /// <param name="target"></param>
        /// <param name="boundsPoints"></param>
        /// <param name="ignoreLayers"></param>
        public void UpdateNonAABoundingBoxCornerPositions(GameObject target, List<Vector3> boundsPoints, LayerMask ignoreLayers)
        {
            if (target != targetObject || rawBoundingCornersObtained == false)
            {
                GetRawBBCorners(target, ignoreLayers);
            }

            if (target == targetObject && rawBoundingCornersObtained)
            {
                boundsPoints.Clear();
                for (int i = 0; i < rawBoundingCorners.Count; ++i)
                {
                    boundsPoints.Add(target.transform.localToWorldMatrix.MultiplyPoint(rawBoundingCorners[i]));
                }

                worldBoundingCorners.Clear();
                worldBoundingCorners.AddRange(boundsPoints);
            }
        }

        /// <summary>
        /// This function gets the untransformed bounding box corner points of a GameObject.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="ignoreLayers"></param>
        public void GetRawBBCorners(GameObject target, LayerMask ignoreLayers)
        {
            targetObject = target;
            rawBoundingCorners.Clear();
            rawBoundingCornersObtained = false;

            GetUntransformedCornersFromObject(target, rawBoundingCorners, ignoreLayers);

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
            Vector3[] midpoints = new Vector3[4];
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
        /// <returns></returns>
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
        /// This function returns the four couners of a face of a bounding cube specified by index.
        /// </summary>
        /// <param name="index">the index of the face of the bounding cube. 0-5</param>
        /// <returns>an array of 4 vectors</returns>
        public Vector3[] GetFaceCorners(int index)
        {
            int[] faceIndices = GetFaceIndices(index);

            if (faceIndices.Length == 4)
            {
                Vector3[] face = new Vector3[4];
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
        /// This is the static function to call to get the non-Axis-aligned bounding box corners one time only.
        /// Use this function if the calling object only needs the info once. To get an updated boundingbox use the
        /// function above---UpdateNonAABoundingBoxCornerPositions(...);
        /// </summary>
        /// <param name="target">The gameObject whose bounding box is desired</param>
        /// <param name="boundsPoints">the array of 8 points that will be filled</param>
        /// <param name="ignoreLayers">a LayerMask variable</param>
        public static void GetNonAABoundingBoxCornerPositions(GameObject target, List<Vector3> boundsPoints, LayerMask ignoreLayers)
        {
            //get untransformed points
            GetUntransformedCornersFromObject(target, boundsPoints, ignoreLayers);

            //transform the points
            for (int i = 0; i < boundsPoints.Count; ++i)
            {
                boundsPoints[i] = target.transform.localToWorldMatrix.MultiplyPoint(boundsPoints[i]);
            }
        }

        /// <summary>
        /// static function that performs one-time non-persistent calculation of boundingbox of object without transformation.
        /// </summary>
        /// <param name="target">The gameObject whose bounding box is desired</param>
        /// <param name="boundsPoints">the array of 8 points that will be filled</param>
        /// <param name="ignoreLayers">a LayerMask variable</param>
        public static void GetUntransformedCornersFromObject(GameObject target, List<Vector3> boundsPoints, LayerMask ignoreLayers)
        {
            GameObject clone = GameObject.Instantiate(target);
            clone.transform.localRotation = Quaternion.identity;
            clone.transform.position = Vector3.zero;
            clone.transform.localScale = Vector3.one;
            Renderer[] renderers = clone.GetComponentsInChildren<Renderer>();

            for (int i = 0; i < renderers.Length; ++i)
            {
                var rendererObj = renderers[i];
                if (ignoreLayers == (1 << rendererObj.gameObject.layer | ignoreLayers))
                {
                    continue;
                }
                Vector3[] corners = null;
                rendererObj.bounds.GetCornerPositionsFromRendererBounds(ref corners);
                AddAABoundingBoxes(boundsPoints, corners);
            }

            GameObject.Destroy(clone);
        }
        /// <summary>
        /// This function expands the box defined by the first param 'points' to include the second bounding box 'pointsToAdd'. The
        /// result is found in the points variable.
        /// </summary>
        /// <param name="points">the bounding box points representing box A</param>
        /// <param name="pointsToAdd">the bounding box points representing box B</param>
        public static void AddAABoundingBoxes(List<Vector3> points, Vector3[] pointsToAdd)
        {
            if (points.Count < 8)
            {
                points.Clear();
                points.AddRange(pointsToAdd);
                return;
            }

            for (int i = 0; i < pointsToAdd.Length; ++i)
            {
                if (pointsToAdd[i].x < points[0].x)
                {
                    points[0].Set(pointsToAdd[i].x, points[0].y, points[0].z);
                    points[1].Set(pointsToAdd[i].x, points[1].y, points[1].z);
                    points[2].Set(pointsToAdd[i].x, points[2].y, points[2].z);
                    points[3].Set(pointsToAdd[i].x, points[3].y, points[3].z);
                }
                if (pointsToAdd[i].x > points[4].x)
                {
                    points[4].Set(pointsToAdd[i].x, points[4].y, points[4].z);
                    points[5].Set(pointsToAdd[i].x, points[5].y, points[5].z);
                    points[6].Set(pointsToAdd[i].x, points[6].y, points[6].z);
                    points[7].Set(pointsToAdd[i].x, points[7].y, points[7].z);
                }

                if (pointsToAdd[i].y < points[0].y)
                {
                    points[0].Set(points[0].x, pointsToAdd[i].y, points[0].z);
                    points[1].Set(points[1].x, pointsToAdd[i].y, points[1].z);
                    points[4].Set(points[4].x, pointsToAdd[i].y, points[4].z);
                    points[5].Set(points[5].x, pointsToAdd[i].y, points[5].z);
                }
                if (pointsToAdd[i].y > points[2].y)
                {
                    points[2].Set(points[2].x, pointsToAdd[i].y, points[2].z);
                    points[3].Set(points[3].x, pointsToAdd[i].y, points[3].z);
                    points[6].Set(points[6].x, pointsToAdd[i].y, points[6].z);
                    points[7].Set(points[7].x, pointsToAdd[i].y, points[7].z);
                }

                if (pointsToAdd[i].z < points[0].z)
                {
                    points[0].Set(points[0].x, points[0].y, pointsToAdd[i].z);
                    points[2].Set(points[2].x, points[2].y, pointsToAdd[i].z);
                    points[6].Set(points[6].x, points[6].y, pointsToAdd[i].z);
                    points[4].Set(points[4].x, points[4].y, pointsToAdd[i].z);
                }
                if (pointsToAdd[i].z > points[1].z)
                {
                    points[1].Set(points[1].x, points[1].y, pointsToAdd[i].z);
                    points[5].Set(points[5].x, points[5].y, pointsToAdd[i].z);
                    points[7].Set(points[7].x, points[7].y, pointsToAdd[i].z);
                    points[3].Set(points[3].x, points[3].y, pointsToAdd[i].z);
                }
            }
        }
    }
}