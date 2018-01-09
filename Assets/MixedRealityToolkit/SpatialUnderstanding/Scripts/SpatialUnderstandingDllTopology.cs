// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Encapsulates the topology queries of the understanding DLL.
    /// These queries will not be valid until after scanning is finalized.
    /// </summary>
    public static class SpatialUnderstandingDllTopology
    {
        /// <summary>
        /// Result of a topology query. Typically results return an array 
        /// of these structures.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct TopologyResult
        {
            public Vector3 position;
            public Vector3 normal;
            public float width;
            public float length;
        };

        // Functions
        /// <summary>
        /// Finds spaces on walls meeting the criteria specified by the parameters.
        /// </summary>
        /// <param name="minHeightOfWallSpace">Minimum height of space to be found by the query</param>
        /// <param name="minWidthOfWallSpace">Minimum width of space to be found by the query</param>
        /// <param name="minHeightAboveFloor">Minimum distance above the floor for the bottom edge of the space</param>
        /// <param name="minFacingClearance">Minimum amount of space in front of the space</param>
        /// <param name="locationCount">Number of location results supplied by the user in locationData</param>
        /// <param name="locationData">Location result array of TopologyResult to be filled with the spaces found by the query</param>
        /// <returns>Number of spaces found by the query. This value is limited by the number of results supplied by the caller (locationCount)</returns>
        // Queries (topology)
        [DllImport("SpatialUnderstanding")]
        public static extern int QueryTopology_FindPositionsOnWalls(
            [In] float minHeightOfWallSpace,
            [In] float minWidthOfWallSpace,
            [In] float minHeightAboveFloor,
            [In] float minFacingClearance,
            [In] int locationCount,             // Pass in the space allocated in locationData
            [In, Out] IntPtr locationData);     // TopologyResult

        /// <summary>
        /// Finds only large spaces on walls meeting the criteria specified by the parameters.
        /// </summary>
        /// <param name="minHeightOfWallSpace">Minimum height of space to be found by the query</param>
        /// <param name="minWidthOfWallSpace">Minimum width of space to be found by the query</param>
        /// <param name="minHeightAboveFloor">Minimum distance above the floor for the bottom edge of the space</param>
        /// <param name="minFacingClearance">Minimum amount of space in front of the space</param>
        /// <param name="locationCount">Number of location results supplied by the user in locationData</param>
        /// <param name="locationData">Location result array of TopologyResult to be filled with the spaces found by the query</param>
        /// <returns>Number of spaces found by the query. This value is limited by the number of results supplied by the caller (locationCount)</returns>
        [DllImport("SpatialUnderstanding")]
        public static extern int QueryTopology_FindLargePositionsOnWalls(
            [In] float minHeightOfWallSpace,
            [In] float minWidthOfWallSpace,
            [In] float minHeightAboveFloor,
            [In] float minFacingClearance,
            [In] int locationCount,             // Pass in the space allocated in locationData
            [In, Out] IntPtr locationData);     // TopologyResult

        /// <summary>
        /// Finds the largest wall
        /// </summary>
        /// <param name="wall">Pointer to a TopologyResult structure, to be filled with the found wall</param>
        /// <returns>Zero if fails, one if success</returns>
        [DllImport("SpatialUnderstanding")]
        public static extern int QueryTopology_FindLargestWall(
            [In, Out] IntPtr wall);             // TopologyResult

        /// <summary>
        /// Finds spaces on the floor meeting the criteria specified by the parameters.
        /// </summary>
        /// <param name="minLengthOfFloorSpace">Minimum length of space to be found by the query</param>
        /// <param name="minWidthOfFloorSpace">Minimum width of space to be found by the query</param>
        /// <param name="locationCount">Number of location results supplied by the user in locationData</param>
        /// <param name="locationData">Location result array of TopologyResult to be filled with the spaces found by the query</param>
        /// <returns>Number of spaces found by the query. This value is limited by the number of results supplied by the caller (locationCount)</returns>
        [DllImport("SpatialUnderstanding")]
        public static extern int QueryTopology_FindPositionsOnFloor(
            [In] float minLengthOfFloorSpace,
            [In] float minWidthOfFloorSpace,
            [In] int locationCount,             // Pass in the space allocated in locationData
            [In, Out] IntPtr locationData);     // TopologyResult

        /// <summary>
        /// Finds the largest spaces on the floor
        /// </summary>
        /// <param name="locationCount">Number of location results supplied by the user in locationData</param>
        /// <param name="locationData">Location result array of TopologyResult to be filled with the spaces found by the query</param>
        /// <returns>Number of spaces found by the query. This value is limited by the number of results supplied by the caller (locationCount)</returns>
        [DllImport("SpatialUnderstanding")]
        public static extern int QueryTopology_FindLargestPositionsOnFloor(
            [In] int locationCount,             // Pass in the space allocated in locationData
            [In, Out] IntPtr locationData);     // TopologyResult

        /// <summary>
        /// Finds good spaces for sitting or placing objects on surfaces.
        /// </summary>
        /// <param name="minHeight">Minimum height above the floor for a space</param>
        /// <param name="maxHeight">Maximum height above the floor for a space</param>
        /// <param name="minFacingClearance">Minimum clearance for the space above the placement surface (minimum space height)</param>
        /// <param name="locationCount">Number of location results supplied by the user in locationData</param>
        /// <param name="locationData">Location result array of TopologyResult to be filled with the spaces found by the query</param>
        /// <returns>Number of spaces found by the query. This value is limited by the number of results supplied by the caller (locationCount)</returns>
        [DllImport("SpatialUnderstanding")]
        public static extern int QueryTopology_FindPositionsSittable(
            [In] float minHeight,
            [In] float maxHeight,
            [In] float minFacingClearance,
            [In] int locationCount,             // Pass in the space allocated in locationData
            [In, Out] IntPtr locationData);     // TopologyResult
    }

}