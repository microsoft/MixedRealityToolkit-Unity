// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;

public class LevelSolver : LineDrawer
{
    // Singleton
    public static LevelSolver Instance;

    // Enums
    public enum QueryStates
    {
        None,
        Processing,
        Finished        
    }

    // Structs
    private struct QueryStatus
    {
        public void Reset()
        {
            State = QueryStates.None;
            Name = "";
            CountFail = 0;
            CountSuccess = 0;
            QueryResult = new List<SpatialUnderstandingDllObjectPlacement.ObjectPlacementResult>();
        }

        public QueryStates State;
        public string Name;
        public int CountFail;
        public int CountSuccess;
        public List<SpatialUnderstandingDllObjectPlacement.ObjectPlacementResult> QueryResult;
    }

    private struct PlacementQuery
    {
        public PlacementQuery(
            SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition placementDefinition,
            List<SpatialUnderstandingDllObjectPlacement.ObjectPlacementRule> placementRules = null,
            List<SpatialUnderstandingDllObjectPlacement.ObjectPlacementConstraint> placementConstraints = null)
        {
            PlacementDefinition = placementDefinition;
            PlacementRules = placementRules;
            PlacementConstraints = placementConstraints;
        }

        public SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition PlacementDefinition;
        public List<SpatialUnderstandingDllObjectPlacement.ObjectPlacementRule> PlacementRules;
        public List<SpatialUnderstandingDllObjectPlacement.ObjectPlacementConstraint> PlacementConstraints;
    }

    private class PlacementResult
    {
        public PlacementResult(float timeDelay, SpatialUnderstandingDllObjectPlacement.ObjectPlacementResult result)
        {
            Box = new AnimatedBox(timeDelay, result.Position, Quaternion.LookRotation(result.Forward, result.Up), Color.blue, result.HalfDims);
            Result = result;
        }

        public LineDrawer.AnimatedBox Box;
        public SpatialUnderstandingDllObjectPlacement.ObjectPlacementResult Result;
    }

    // Properties
    public bool IsSolverInitialized { get; private set; }

    // Privates
    private List<PlacementResult> placementResults = new List<PlacementResult>();
    private QueryStatus queryStatus = new QueryStatus();

    // Functions
    private void Awake()
    {
        Instance = this;
    }

    public void ClearGeometry(bool clearAll = true)
    {
        placementResults.Clear();
        if (SpatialUnderstanding.Instance.AllowSpatialUnderstanding)
        {
            SpatialUnderstandingDllObjectPlacement.Solver_RemoveAllObjects();
        }
        AppState.Instance.ObjectPlacementDescription = "";

        if (clearAll && (SpaceVisualizer.Instance != null))
        {
            SpaceVisualizer.Instance.ClearGeometry(false);
        }
    }

    private bool Draw_PlacementResults()
    {
        bool needsUpdate = false;

        for (int i = 0; i < placementResults.Count; ++i)
        {
            needsUpdate |= Draw_AnimatedBox(placementResults[i].Box);
        }

        return needsUpdate;
    }

    private bool PlaceObjectAsync(
        string placementName,
        SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition placementDefinition,
        List<SpatialUnderstandingDllObjectPlacement.ObjectPlacementRule> placementRules = null,
        List<SpatialUnderstandingDllObjectPlacement.ObjectPlacementConstraint> placementConstraints = null,
        bool clearObjectsFirst = true)
    {
        return PlaceObjectAsync(
            placementName,
            new List<PlacementQuery>() { new PlacementQuery(placementDefinition, placementRules, placementConstraints) },
            clearObjectsFirst);
    }

    private bool PlaceObjectAsync(
        string placementName,
        List<PlacementQuery> placementList,
        bool clearObjectsFirst = true)
    {
        // If we already mid-query, reject the request
        if (queryStatus.State != QueryStates.None)
        {
            return false;
        }

        // Clear geo
        if (clearObjectsFirst)
        {
            ClearGeometry();
        }

        // Mark it
        queryStatus.Reset();
        queryStatus.State = QueryStates.Processing;
        queryStatus.Name = placementName;

        // Tell user we are processing
        AppState.Instance.ObjectPlacementDescription = placementName + " (processing)";

#if UNITY_WSA && !UNITY_EDITOR
        // Kick off a thread to do process the queries
        System.Threading.Tasks.Task.Run(() =>
            {
                // Go through the queries in the list
                for (int i = 0; i < placementList.Count; ++i)
                {
                    // Do the query
                    bool success = PlaceObject(
                        placementName,
                        placementList[i].PlacementDefinition,
                        placementList[i].PlacementRules,
                        placementList[i].PlacementConstraints, 
                        clearObjectsFirst, 
                        true);

                    // Mark the result
                    queryStatus.CountSuccess = success ? (queryStatus.CountSuccess + 1) : queryStatus.CountSuccess;
                    queryStatus.CountFail = !success ? (queryStatus.CountFail + 1) : queryStatus.CountFail;
                }

                // Done
                queryStatus.State = QueryStates.Finished;
            }
        );
#endif

        return true;
    }

    private bool PlaceObject(
        string placementName,
        SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition placementDefinition,
        List<SpatialUnderstandingDllObjectPlacement.ObjectPlacementRule> placementRules = null,
        List<SpatialUnderstandingDllObjectPlacement.ObjectPlacementConstraint> placementConstraints = null,
        bool clearObjectsFirst = true,
        bool isASync = false)
    {
        // Clear objects (if requested)
        if (!isASync && clearObjectsFirst)
        {
            ClearGeometry();
        }
        if (!SpatialUnderstanding.Instance.AllowSpatialUnderstanding)
        {
            return false;
        }

        // New query
        if (SpatialUnderstandingDllObjectPlacement.Solver_PlaceObject(
                placementName,
                SpatialUnderstanding.Instance.UnderstandingDLL.PinObject(placementDefinition),
                (placementRules != null) ? placementRules.Count : 0,
                ((placementRules != null) && (placementRules.Count > 0)) ? SpatialUnderstanding.Instance.UnderstandingDLL.PinObject(placementRules.ToArray()) : IntPtr.Zero,
                (placementConstraints != null) ? placementConstraints.Count : 0,
                ((placementConstraints != null) && (placementConstraints.Count > 0)) ? SpatialUnderstanding.Instance.UnderstandingDLL.PinObject(placementConstraints.ToArray()) : IntPtr.Zero,
                SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticObjectPlacementResultPtr()) > 0)
        {
            SpatialUnderstandingDllObjectPlacement.ObjectPlacementResult placementResult = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticObjectPlacementResult();
            if (!isASync)
            {
                // If not running async, we can just add the results to the draw list right now
                AppState.Instance.ObjectPlacementDescription = placementName + " (1)";
                float timeDelay = (float)placementResults.Count * AnimatedBox.DelayPerItem;
                placementResults.Add(new PlacementResult(timeDelay, placementResult.Clone() as SpatialUnderstandingDllObjectPlacement.ObjectPlacementResult));
            }
            else
            {
                queryStatus.QueryResult.Add(placementResult.Clone() as SpatialUnderstandingDllObjectPlacement.ObjectPlacementResult);
            }
            return true;
        }
        if (!isASync)
        {
            AppState.Instance.ObjectPlacementDescription = placementName + " (0)";
        }
        return false;
    }

    private void ProcessPlacementResults()
    {
        // Check it
        if (queryStatus.State != QueryStates.Finished)
        {
            return;
        }
        if (!SpatialUnderstanding.Instance.AllowSpatialUnderstanding)
        {
            return;
        }

        // Clear results
        ClearGeometry();

        // We will reject any above or below the ceiling/floor
        SpatialUnderstandingDll.Imports.QueryPlayspaceAlignment(SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceAlignmentPtr());
        SpatialUnderstandingDll.Imports.PlayspaceAlignment alignment = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceAlignment();

        // Copy over the results
        for (int i = 0; i < queryStatus.QueryResult.Count; ++i)
        {
            if ((queryStatus.QueryResult[i].Position.y < alignment.CeilingYValue) &&
                (queryStatus.QueryResult[i].Position.y > alignment.FloorYValue))
            {
                float timeDelay = (float)placementResults.Count * AnimatedBox.DelayPerItem;
                placementResults.Add(new PlacementResult(timeDelay, queryStatus.QueryResult[i].Clone() as SpatialUnderstandingDllObjectPlacement.ObjectPlacementResult));
            }
        }

        // Text
        AppState.Instance.ObjectPlacementDescription = queryStatus.Name + " (" + placementResults.Count + "/" + (queryStatus.CountSuccess + queryStatus.CountFail) + ")";

        // Mark done
        queryStatus.Reset();
    }

    public void Query_OnFloor()
    {
        List<PlacementQuery> placementQuery = new List<PlacementQuery>();
        for (int i = 0; i < 4; ++i)
        {
            float halfDimSize = UnityEngine.Random.Range(0.15f, 0.35f);
            placementQuery.Add(
                new PlacementQuery(SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition.Create_OnFloor(new Vector3(halfDimSize, halfDimSize, halfDimSize * 2.0f)),
                                    new List<SpatialUnderstandingDllObjectPlacement.ObjectPlacementRule>() {
                                            SpatialUnderstandingDllObjectPlacement.ObjectPlacementRule.Create_AwayFromOtherObjects(halfDimSize * 3.0f),
                                    }));
        }
        PlaceObjectAsync("OnFloor", placementQuery);
    }

    public void Query_OnWall()
    {
        List<PlacementQuery> placementQuery = new List<PlacementQuery>();
        for (int i = 0; i < 6; ++i)
        {
            float halfDimSize = UnityEngine.Random.Range(0.3f, 0.6f);
            placementQuery.Add(
                new PlacementQuery(SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition.Create_OnWall(new Vector3(halfDimSize, halfDimSize * 0.5f, 0.05f), 0.5f, 3.0f),
                                    new List<SpatialUnderstandingDllObjectPlacement.ObjectPlacementRule>() {
                                            SpatialUnderstandingDllObjectPlacement.ObjectPlacementRule.Create_AwayFromOtherObjects(halfDimSize * 4.0f),
                                    }));
        }
        PlaceObjectAsync("OnWall", placementQuery);
    }

    public void Query_OnCeiling()
    {
        List<PlacementQuery> placementQuery = new List<PlacementQuery>();
        for (int i = 0; i < 2; ++i)
        {
            float halfDimSize = UnityEngine.Random.Range(0.3f, 0.4f);
            placementQuery.Add(
                new PlacementQuery(SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition.Create_OnCeiling(new Vector3(halfDimSize, halfDimSize, halfDimSize)),
                                    new List<SpatialUnderstandingDllObjectPlacement.ObjectPlacementRule>() {
                                            SpatialUnderstandingDllObjectPlacement.ObjectPlacementRule.Create_AwayFromOtherObjects(halfDimSize * 3.0f),
                                    }));
        }
        PlaceObjectAsync("OnCeiling", placementQuery);
    }

    public void Query_OnEdge()
    {
        List<PlacementQuery> placementQuery = new List<PlacementQuery>();
        for (int i = 0; i < 8; ++i)
        {
            float halfDimSize = UnityEngine.Random.Range(0.05f, 0.1f);
            placementQuery.Add(
                new PlacementQuery(SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition.Create_OnEdge(new Vector3(halfDimSize, halfDimSize, halfDimSize),
                                                                                                                  new Vector3(halfDimSize, halfDimSize, halfDimSize)),
                                    new List<SpatialUnderstandingDllObjectPlacement.ObjectPlacementRule>() {
                                            SpatialUnderstandingDllObjectPlacement.ObjectPlacementRule.Create_AwayFromOtherObjects(halfDimSize * 3.0f),
                                    }));
        }
        PlaceObjectAsync("OnEdge", placementQuery);
    }

    public void Query_OnFloorAndCeiling()
    {
        SpatialUnderstandingDll.Imports.QueryPlayspaceAlignment(SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceAlignmentPtr());
        SpatialUnderstandingDll.Imports.PlayspaceAlignment alignment = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceAlignment();
        List<PlacementQuery> placementQuery = new List<PlacementQuery>();
        for (int i = 0; i < 4; ++i)
        {
            float halfDimSize = UnityEngine.Random.Range(0.1f, 0.2f);
            placementQuery.Add(
                new PlacementQuery(SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition.Create_OnFloorAndCeiling(new Vector3(halfDimSize, (alignment.CeilingYValue - alignment.FloorYValue) * 0.5f, halfDimSize),
                                                                                                                         new Vector3(halfDimSize, (alignment.CeilingYValue - alignment.FloorYValue) * 0.5f, halfDimSize)),
                                    new List<SpatialUnderstandingDllObjectPlacement.ObjectPlacementRule>() {
                                            SpatialUnderstandingDllObjectPlacement.ObjectPlacementRule.Create_AwayFromOtherObjects(halfDimSize * 3.0f),
                                    }));
        }
        PlaceObjectAsync("OnFloorAndCeiling", placementQuery);
    }

    public void Query_RandomInAir_AwayFromMe()
    {
        List<PlacementQuery> placementQuery = new List<PlacementQuery>();
        for (int i = 0; i < 8; ++i)
        {
            float halfDimSize = UnityEngine.Random.Range(0.1f, 0.2f);
            placementQuery.Add(
                new PlacementQuery(SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition.Create_RandomInAir(new Vector3(halfDimSize, halfDimSize, halfDimSize)),
                                    new List<SpatialUnderstandingDllObjectPlacement.ObjectPlacementRule>() {
                                            SpatialUnderstandingDllObjectPlacement.ObjectPlacementRule.Create_AwayFromOtherObjects(halfDimSize * 3.0f),
                                            SpatialUnderstandingDllObjectPlacement.ObjectPlacementRule.Create_AwayFromPosition(Camera.main.transform.position, 2.5f),
                                    }));
        }
        PlaceObjectAsync("RandomInAir - AwayFromMe", placementQuery);
    }

    public void Query_OnEdge_NearCenter()
    {
        List<PlacementQuery> placementQuery = new List<PlacementQuery>();
        for (int i = 0; i < 4; ++i)
        {
            float halfDimSize = UnityEngine.Random.Range(0.05f, 0.1f);
            placementQuery.Add(
                new PlacementQuery(SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition.Create_OnEdge(new Vector3(halfDimSize, halfDimSize, halfDimSize), new Vector3(halfDimSize, halfDimSize, halfDimSize)),
                                   new List<SpatialUnderstandingDllObjectPlacement.ObjectPlacementRule>() {
                                            SpatialUnderstandingDllObjectPlacement.ObjectPlacementRule.Create_AwayFromOtherObjects(halfDimSize * 2.0f),
                                   },
                                   new List<SpatialUnderstandingDllObjectPlacement.ObjectPlacementConstraint>() {
                                           SpatialUnderstandingDllObjectPlacement.ObjectPlacementConstraint.Create_NearCenter(),
                                   }));
        }
        PlaceObjectAsync("OnEdge - NearCenter", placementQuery);

    }

    public void Query_OnFloor_AwayFromMe()
    {
        List<PlacementQuery> placementQuery = new List<PlacementQuery>();
        for (int i = 0; i < 4; ++i)
        {
            float halfDimSize = UnityEngine.Random.Range(0.05f, 0.15f);
            placementQuery.Add(
                new PlacementQuery(SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition.Create_OnFloor(new Vector3(halfDimSize, halfDimSize, halfDimSize)),
                                   new List<SpatialUnderstandingDllObjectPlacement.ObjectPlacementRule>() {
                                           SpatialUnderstandingDllObjectPlacement.ObjectPlacementRule.Create_AwayFromPosition(Camera.main.transform.position, 2.0f),
                                           SpatialUnderstandingDllObjectPlacement.ObjectPlacementRule.Create_AwayFromOtherObjects(halfDimSize * 3.0f),
                                   }));
        }
        PlaceObjectAsync("OnFloor - AwayFromMe", placementQuery);
    }

    public void Query_OnFloor_NearMe()
    {
        List<PlacementQuery> placementQuery = new List<PlacementQuery>();
        for (int i = 0; i < 4; ++i)
        {
            float halfDimSize = UnityEngine.Random.Range(0.05f, 0.2f);
            placementQuery.Add(
                new PlacementQuery(SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition.Create_OnFloor(new Vector3(halfDimSize, halfDimSize, halfDimSize)),
                                   new List<SpatialUnderstandingDllObjectPlacement.ObjectPlacementRule>() {
                                           SpatialUnderstandingDllObjectPlacement.ObjectPlacementRule.Create_AwayFromOtherObjects(halfDimSize * 3.0f),
                                   },
                                   new List<SpatialUnderstandingDllObjectPlacement.ObjectPlacementConstraint>() {
                                           SpatialUnderstandingDllObjectPlacement.ObjectPlacementConstraint.Create_NearPoint(Camera.main.transform.position, 0.5f, 2.0f)
                                   }));
        }
        PlaceObjectAsync("OnFloor - NearMe", placementQuery);
    }

    private void Update_Queries()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Query_OnFloor();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Query_OnWall();
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            Query_OnCeiling();
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            Query_OnEdge();
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            Query_OnFloorAndCeiling();
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            Query_RandomInAir_AwayFromMe();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            Query_OnEdge_NearCenter();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            Query_OnFloor_AwayFromMe();
        }
        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            Query_OnFloor_NearMe();
        }
    }

    public bool InitializeSolver()
    {
        if (IsSolverInitialized ||
            !SpatialUnderstanding.Instance.AllowSpatialUnderstanding)
        {
            return IsSolverInitialized;
        }

        if (SpatialUnderstandingDllObjectPlacement.Solver_Init() > 1)
        {
            IsSolverInitialized = true;
        }
        return IsSolverInitialized;
    }

    private void Update()
    {
        // Can't do any of this till we're done with the scanning phase
        if (SpatialUnderstanding.Instance.ScanState != SpatialUnderstanding.ScanStates.Done)
        {
            return;
        }

        // Make sure the solver has been initialized
        if (!IsSolverInitialized &&
            SpatialUnderstanding.Instance.AllowSpatialUnderstanding)
        {
            InitializeSolver();
        }

        // Constraint queries
        if (SpatialUnderstanding.Instance.ScanState == SpatialUnderstanding.ScanStates.Done)
        {
            Update_Queries();
        }

        // Handle async query results
        ProcessPlacementResults();

        // Lines: Begin
        LineDraw_Begin();

        // Drawers
        bool needsUpdate = false;
        needsUpdate |= Draw_PlacementResults();

        // Lines: Finish up
        LineDraw_End(needsUpdate);
    }
}
