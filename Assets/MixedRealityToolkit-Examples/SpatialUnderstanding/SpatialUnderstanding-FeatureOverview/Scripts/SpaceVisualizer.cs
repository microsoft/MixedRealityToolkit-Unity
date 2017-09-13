// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Examples.SpatialUnderstandingFeatureOverview
{
    public class SpaceVisualizer : LineDrawer
    {
        // Singleton
        public static SpaceVisualizer Instance;

        // Consts
        const int QueryResultMaxCount = 512;
        const int DisplayResultMaxCount = 32;

        // Privates
        private List<AnimatedBox> lineBoxList = new List<AnimatedBox>();
        private SpatialUnderstandingDllTopology.TopologyResult[] resultsTopology = new SpatialUnderstandingDllTopology.TopologyResult[QueryResultMaxCount];
        private SpatialUnderstandingDllShapes.ShapeResult[] resultsShape = new SpatialUnderstandingDllShapes.ShapeResult[QueryResultMaxCount];

        // Functions
        private void Awake()
        {
            Instance = this;
        }

        public void ClearGeometry(bool clearAll = true)
        {
            lineBoxList = new List<AnimatedBox>();
            AppState.Instance.SpaceQueryDescription = "";

            if (clearAll && (LevelSolver.Instance != null))
            {
                LevelSolver.Instance.ClearGeometry(false);
            }
        }

        public void Query_PlayspaceAlignment()
        {
            // First clear all our geo
            ClearGeometry();

            // Only if we're enabled
            if (!SpatialUnderstanding.Instance.AllowSpatialUnderstanding)
            {
                return;
            }

            // Alignment information
            SpatialUnderstandingDll.Imports.QueryPlayspaceAlignment(SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceAlignmentPtr());
            SpatialUnderstandingDll.Imports.PlayspaceAlignment alignment = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceAlignment();

            // Box for the space
            float timeDelay = (float)lineBoxList.Count * AnimatedBox.DelayPerItem;
            lineBoxList.Add(
                new AnimatedBox(
                    timeDelay,
                    new Vector3(alignment.Center.x, (alignment.CeilingYValue + alignment.FloorYValue) * 0.5f, alignment.Center.z),
                    Quaternion.LookRotation(alignment.BasisZ, alignment.BasisY),
                    Color.magenta,
                    new Vector3(alignment.HalfDims.x, (alignment.CeilingYValue - alignment.FloorYValue) * 0.5f, alignment.HalfDims.z))
            );
            AppState.Instance.SpaceQueryDescription = "Playspace Alignment OBB";
        }

        public void Query_Topology_FindPositionOnWall()
        {
            ClearGeometry();

            // Only if we're enabled
            if (!SpatialUnderstanding.Instance.AllowSpatialUnderstanding)
            {
                return;
            }

            // Setup
            float minHeightOfWallSpace = 0.5f;
            float minWidthOfWallSpace = 0.75f;
            float minHeightAboveFloor = 1.25f;
            float minFacingClearance = 1.5f;

            // Query
            IntPtr resultsTopologyPtr = SpatialUnderstanding.Instance.UnderstandingDLL.PinObject(resultsTopology);
            int locationCount = SpatialUnderstandingDllTopology.QueryTopology_FindPositionsOnWalls(
                minHeightOfWallSpace, minWidthOfWallSpace, minHeightAboveFloor, minFacingClearance,
                resultsTopology.Length, resultsTopologyPtr);

            // Output
            HandleResults_Topology("Find Position On Wall", locationCount, new Vector3(minWidthOfWallSpace, minHeightOfWallSpace, 0.025f), Color.blue);
        }

        public void Query_Topology_FindLargePositionsOnWalls()
        {
            // Setup
            float minHeightOfWallSpace = 1.0f;
            float minWidthOfWallSpace = 1.5f;
            float minHeightAboveFloor = 1.5f;
            float minFacingClearance = 0.5f;

            // Only if we're enabled
            if (!SpatialUnderstanding.Instance.AllowSpatialUnderstanding)
            {
                return;
            }

            // Query
            IntPtr resultsTopologyPtr = SpatialUnderstanding.Instance.UnderstandingDLL.PinObject(resultsTopology);
            int locationCount = SpatialUnderstandingDllTopology.QueryTopology_FindLargePositionsOnWalls(
                minHeightOfWallSpace, minWidthOfWallSpace, minHeightAboveFloor, minFacingClearance,
                resultsTopology.Length, resultsTopologyPtr);

            // Output
            HandleResults_Topology("Find Large Positions On Walls", locationCount, new Vector3(minWidthOfWallSpace, minHeightOfWallSpace, 0.025f), Color.yellow);
        }

        public void Query_Topology_FindLargeWall()
        {
            ClearGeometry();

            // Only if we're enabled
            if (!SpatialUnderstanding.Instance.AllowSpatialUnderstanding)
            {
                return;
            }

            // Query
            IntPtr wallPtr = SpatialUnderstanding.Instance.UnderstandingDLL.PinObject(resultsTopology);
            int wallCount = SpatialUnderstandingDllTopology.QueryTopology_FindLargestWall(
                wallPtr);
            if (wallCount == 0)
            {
                AppState.Instance.SpaceQueryDescription = "Find Largest Wall (0)";
                return;
            }

            // Add the line boxes
            float timeDelay = (float)lineBoxList.Count * AnimatedBox.DelayPerItem;
            lineBoxList.Add(
                new AnimatedBox(
                    timeDelay,
                    resultsTopology[0].position,
                    Quaternion.LookRotation(resultsTopology[0].normal, Vector3.up),
                    Color.magenta,
                    new Vector3(resultsTopology[0].width, resultsTopology[0].length, 0.05f) * 0.5f)
            );
            AppState.Instance.SpaceQueryDescription = "Find Largest Wall (1)";
        }

        public void Query_Topology_FindPositionsOnFloor()
        {
            // Setup
            float minWidthOfWallSpace = 1.0f;
            float minHeightAboveFloor = 1.0f;

            // Only if we're enabled
            if (!SpatialUnderstanding.Instance.AllowSpatialUnderstanding)
            {
                return;
            }

            // Query
            IntPtr resultsTopologyPtr = SpatialUnderstanding.Instance.UnderstandingDLL.PinObject(resultsTopology);
            int locationCount = SpatialUnderstandingDllTopology.QueryTopology_FindPositionsOnFloor(
                minWidthOfWallSpace, minHeightAboveFloor,
                resultsTopology.Length, resultsTopologyPtr);

            // Output
            HandleResults_Topology("Find Positions On Floor", locationCount, new Vector3(minWidthOfWallSpace, 0.025f, minHeightAboveFloor), Color.red);
        }

        public void Query_Topology_FindLargestPositionsOnFloor()
        {
            // Query
            IntPtr resultsTopologyPtr = SpatialUnderstanding.Instance.UnderstandingDLL.PinObject(resultsTopology);
            int locationCount = SpatialUnderstandingDllTopology.QueryTopology_FindLargestPositionsOnFloor(
                resultsTopology.Length, resultsTopologyPtr);

            // Output
            HandleResults_Topology("Find Largest Positions On Floor", locationCount, new Vector3(1.0f, 1.0f, 0.025f), Color.yellow);
        }

        public void Query_Topology_FindPositionsPlaceable()
        {
            // Setup
            float minHeight = 0.125f;
            float maxHeight = 1.0f;
            float minFacingClearance = 1.0f;

            // Only if we're enabled
            if (!SpatialUnderstanding.Instance.AllowSpatialUnderstanding)
            {
                return;
            }

            // Query
            IntPtr resultsTopologyPtr = SpatialUnderstanding.Instance.UnderstandingDLL.PinObject(resultsTopology);
            int locationCount = SpatialUnderstandingDllTopology.QueryTopology_FindPositionsSittable(
                minHeight, maxHeight, minFacingClearance,
                resultsTopology.Length, resultsTopologyPtr);

            // Output
            HandleResults_Topology("Find Placeable Positions", locationCount, new Vector3(0.25f, 0.025f, 0.25f), Color.cyan);
        }

        public void Query_Shape_FindPositionsOnShape(string shapeName)
        {
            // Setup
            float minRadius = 0.1f;

            // Only if we're enabled
            if (!SpatialUnderstanding.Instance.AllowSpatialUnderstanding)
            {
                return;
            }

            // Query
            IntPtr resultsShapePtr = SpatialUnderstanding.Instance.UnderstandingDLL.PinObject(resultsShape);
            int shapeCount = SpatialUnderstandingDllShapes.QueryShape_FindPositionsOnShape(
                shapeName, minRadius,
                resultsShape.Length, resultsShapePtr);

            // Output
            HandleResults_Shape("Find Positions on Shape '" + shapeName + "'", shapeCount, Color.cyan, new Vector3(0.1f, 0.025f, 0.1f));
        }

        public void Query_Shape_FindShapeHalfDims(string shapeName)
        {
            // Only if we're enabled
            if (!SpatialUnderstanding.Instance.AllowSpatialUnderstanding)
            {
                return;
            }

            // Query
            IntPtr resultsShapePtr = SpatialUnderstanding.Instance.UnderstandingDLL.PinObject(resultsShape);
            int shapeCount = SpatialUnderstandingDllShapes.QueryShape_FindShapeHalfDims(
                shapeName,
                resultsShape.Length, resultsShapePtr);

            // Output
            HandleResults_Shape("Find Shape Min/Max '" + shapeName + "'", shapeCount, Color.blue, new Vector3(0.25f, 0.025f, 0.25f));
        }

        private void HandleResults_Topology(string visDesc, int locationCount, Vector3 boxFullDims, Color color)
        {
            // First clear all our geo
            ClearGeometry();

            // Only if we're enabled
            if (!SpatialUnderstanding.Instance.AllowSpatialUnderstanding)
            {
                return;
            }

            // Add the line boxes (we may have more results than boxes - pick evenly across the results in that case)
            int lineInc = Mathf.CeilToInt((float)locationCount / (float)DisplayResultMaxCount);
            int boxesDisplayed = 0;
            for (int i = 0; i < locationCount; i += lineInc)
            {
                float timeDelay = (float)lineBoxList.Count * AnimatedBox.DelayPerItem;
                lineBoxList.Add(
                    new AnimatedBox(
                        timeDelay,
                        resultsTopology[i].position,
                        Quaternion.LookRotation(resultsTopology[i].normal, Vector3.up),
                        Color.blue,
                        boxFullDims * 0.5f)
                );
                ++boxesDisplayed;
            }

            // Vis description
            if (locationCount == boxesDisplayed)
            {
                AppState.Instance.SpaceQueryDescription = string.Format("{0} ({1})", visDesc, locationCount);
            }
            else
            {
                AppState.Instance.SpaceQueryDescription = string.Format("{0} (found={1}, displayed={2})", visDesc, locationCount, boxesDisplayed);
            }
        }

        private void HandleResults_Shape(string visDesc, int shapeCount, Color color, Vector3 defaultHalfDims)
        {
            // First clear all our geo
            ClearGeometry();

            // Only if we're enabled
            if (!SpatialUnderstanding.Instance.AllowSpatialUnderstanding)
            {
                return;
            }

            // Alignment information
            SpatialUnderstandingDll.Imports.QueryPlayspaceAlignment(SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceAlignmentPtr());
            SpatialUnderstandingDll.Imports.PlayspaceAlignment alignment = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceAlignment();

            // Add the line boxes (we may have more results than boxes - pick evenly across the results in that case)
            int lineInc = Mathf.CeilToInt((float)shapeCount / (float)DisplayResultMaxCount);
            int boxesDisplayed = 0;
            for (int i = 0; i < shapeCount; i += lineInc)
            {
                float timeDelay = (float)lineBoxList.Count * AnimatedBox.DelayPerItem;
                lineBoxList.Add(
                    new AnimatedBox(
                        timeDelay,
                        resultsShape[i].position,
                        Quaternion.LookRotation(alignment.BasisZ, alignment.BasisY),
                        Color.blue,
                        (resultsShape[i].halfDims.sqrMagnitude < 0.01f) ? defaultHalfDims : resultsShape[i].halfDims)
                );
                ++boxesDisplayed;
            }

            // Vis description
            if (shapeCount == boxesDisplayed)
            {
                AppState.Instance.SpaceQueryDescription = string.Format("{0} ({1})", visDesc, shapeCount);
            }
            else
            {
                AppState.Instance.SpaceQueryDescription = string.Format("{0} (found={1}, displayed={2})", visDesc, shapeCount, boxesDisplayed);
            }
        }

        private bool Draw_LineBoxList()
        {
            bool needsUpdate = false;
            for (int i = 0; i < lineBoxList.Count; ++i)
            {
                needsUpdate |= Draw_AnimatedBox(lineBoxList[i]);
            }
            return needsUpdate;
        }

        private void Update_Queries()
        {
            // Queries - basics
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ClearGeometry();
            }
            if (Input.GetKeyDown(KeyCode.F))
            {
                Query_PlayspaceAlignment();
            }

            // Queries - topology
            if (Input.GetKeyDown(KeyCode.G))
            {
                Query_Topology_FindPositionOnWall();
            }
            if (Input.GetKeyDown(KeyCode.H))
            {
                Query_Topology_FindLargePositionsOnWalls();
            }
            if (Input.GetKeyDown(KeyCode.J))
            {
                Query_Topology_FindLargeWall();
            }
            if (Input.GetKeyDown(KeyCode.K))
            {
                Query_Topology_FindPositionsOnFloor();
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                Query_Topology_FindLargestPositionsOnFloor();
            }
            if (Input.GetKeyDown(KeyCode.Semicolon))
            {
                Query_Topology_FindPositionsPlaceable();
            }

            // Queries - shapes
            if (Input.GetKeyDown(KeyCode.Quote))
            {
                Query_Shape_FindShapeHalfDims("All Surfaces");
            }
            if (Input.GetKeyDown(KeyCode.Z))
            {
                Query_Shape_FindPositionsOnShape("Sittable");
            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                Query_Shape_FindShapeHalfDims("Chair");
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                Query_Shape_FindShapeHalfDims("Large Surface");
            }
            if (Input.GetKeyDown(KeyCode.V))
            {
                Query_Shape_FindShapeHalfDims("Large Empty Surface");
            }
            if (Input.GetKeyDown(KeyCode.B))
            {
                Query_Shape_FindShapeHalfDims("Couch");
            }
        }

        private void Update()
        {
            // Queries
            if (SpatialUnderstanding.Instance.ScanState == SpatialUnderstanding.ScanStates.Done)
            {
                Update_Queries();
            }

            // Lines: Begin
            LineDraw_Begin();

            // Drawers
            bool needsUpdate = false;
            needsUpdate |= Draw_LineBoxList();

            // Lines: Finish up
            LineDraw_End(needsUpdate);
        }
    }
}
