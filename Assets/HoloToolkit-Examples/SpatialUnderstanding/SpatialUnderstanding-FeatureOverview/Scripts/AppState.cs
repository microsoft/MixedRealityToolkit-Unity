// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;
using System;
using UnityEngine.VR.WSA.Input;

public class AppState : Singleton<AppState>
{
    // Consts
    public float kMinAreaForStats = 5.0f;
    public float kMinAreaForComplete = 50.0f;
    public float kMinHorizAreaForComplete = 25.0f;
    public float kMinWallAreaForComplete = 10.0f;

    // Config
    public TextMesh DebugDisplay;
    public TextMesh DebugSubDisplay;
    public Transform Parent_Scene;
    public SpatialMappingObserver MappingObserver;
    public Cursor AppCursor;

    // Properties
    public string SpaceQueryDescription
    {
        get
        {
            return spaceQueryDescription;
        }
        set
        {
            spaceQueryDescription = value;
            objectPlacementDescription = "";
        }
    }

    public string ObjectPlacementDescription
    {
        get
        {
            return objectPlacementDescription;
        }
        set
        {
            objectPlacementDescription = value;
            spaceQueryDescription = "";
        }
    }

    public bool DoesScanMeetMinBarForCompletion
    {
        get
        {
            // Only allow this when we are actually scanning
            if ((SpatialUnderstanding.Instance.ScanState != SpatialUnderstanding.ScanStates.Scanning) ||
                (!SpatialUnderstanding.Instance.AllowSpatialUnderstanding))
            {
                return false;
            }

            // Query the current playspace stats
            IntPtr statsPtr = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceStatsPtr();
            if (SpatialUnderstandingDll.Imports.QueryPlayspaceStats(statsPtr) == 0)
            {
                return false;
            }
            SpatialUnderstandingDll.Imports.PlayspaceStats stats = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceStats();

            // Check our preset requirements
            if ((stats.TotalSurfaceArea > kMinAreaForComplete) ||
                (stats.HorizSurfaceArea > kMinHorizAreaForComplete) ||
                (stats.WallSurfaceArea > kMinWallAreaForComplete))
            {
                return true;
            }
            return false;
        }
    }

    public string PrimaryText
    {
        get
        {
            // Display the space and object query results (has priority)
            if (!string.IsNullOrEmpty(SpaceQueryDescription))
            {
                return SpaceQueryDescription;
            }
            else if (!string.IsNullOrEmpty(ObjectPlacementDescription))
            {
                return ObjectPlacementDescription;
            }

            // Scan state
            if (SpatialUnderstanding.Instance.AllowSpatialUnderstanding)
            {
                switch (SpatialUnderstanding.Instance.ScanState)
                {
                    case SpatialUnderstanding.ScanStates.Scanning:
                        // Get the scan stats
                        IntPtr statsPtr = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceStatsPtr();
                        if (SpatialUnderstandingDll.Imports.QueryPlayspaceStats(statsPtr) == 0)
                        {
                            return "playspace stats query failed";
                        }

                        // The stats tell us if we could potentially finish
                        if (DoesScanMeetMinBarForCompletion)
                        {
                            return "When ready, air tap to finalize your playspace";
                        }
                        return "Walk around and scan in your playspace";
                    case SpatialUnderstanding.ScanStates.Finishing:
                        return "Finalizing scan (please wait)";
                    case SpatialUnderstanding.ScanStates.Done:
                        return "Scan complete - Use the menu to run queries";
                    default:
                        return "ScanState = " + SpatialUnderstanding.Instance.ScanState.ToString();
                }
            }
            return "";
        }
    }

    public Color PrimaryColor
    {
        get
        {
            if (SpatialUnderstanding.Instance.ScanState == SpatialUnderstanding.ScanStates.Scanning)
            {
                if (HandsManager.Instance.HandDetected)
                {
                    return DoesScanMeetMinBarForCompletion ? Color.green : Color.red;
                }
                return DoesScanMeetMinBarForCompletion ? Color.yellow : Color.white;
            }

            // If we're looking at the menu, fade it out
            Vector3 hitPos, hitNormal;
            UnityEngine.UI.Button hitButton;
            float alpha = AppCursor.RayCastUI(out hitPos, out hitNormal, out hitButton) ? 0.15f : 1.0f;

            // Special case processing & 
            return (!string.IsNullOrEmpty(SpaceQueryDescription) || !string.IsNullOrEmpty(ObjectPlacementDescription)) ?
                (PrimaryText.Contains("processing") ? new Color(1.0f, 0.0f, 0.0f, 1.0f) : new Color(1.0f, 0.7f, 0.1f, alpha)) :
                new Color(1.0f, 1.0f, 1.0f, alpha);
        }
    }

    public string DetailsText
    {
        get
        {
            if (SpatialUnderstanding.Instance.ScanState == SpatialUnderstanding.ScanStates.None)
            {
                return "";
            }

            // Scanning stats get second priority
            if ((SpatialUnderstanding.Instance.ScanState == SpatialUnderstanding.ScanStates.Scanning) &&
                (SpatialUnderstanding.Instance.AllowSpatialUnderstanding))
            {
                IntPtr statsPtr = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceStatsPtr();
                if (SpatialUnderstandingDll.Imports.QueryPlayspaceStats(statsPtr) == 0)
                {
                    return "Playspace stats query failed";
                }
                SpatialUnderstandingDll.Imports.PlayspaceStats stats = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceStats();

                // Start showing the stats when they are no longer zero
                if (stats.TotalSurfaceArea > kMinAreaForStats)
                {
                    string subDisplayText = string.Format("totalArea={0:0.0}, horiz={1:0.0}, wall={2:0.0}", stats.TotalSurfaceArea, stats.HorizSurfaceArea, stats.WallSurfaceArea);
                    subDisplayText += string.Format("\nnumFloorCells={0}, numCeilingCells={1}, numPlatformCells={2}", stats.NumFloor, stats.NumCeiling, stats.NumPlatform);
                    subDisplayText += string.Format("\npaintMode={0}, seenCells={1}, notSeen={2}", stats.CellCount_IsPaintMode, stats.CellCount_IsSeenQualtiy_Seen + stats.CellCount_IsSeenQualtiy_Good, stats.CellCount_IsSeenQualtiy_None);
                    return subDisplayText;
                }
                return "";
            }
            return "";
        }
    }

    // Privates
    private string spaceQueryDescription;
    private string objectPlacementDescription;

    // Functions
    private void Start()
    {
        // Default the scene & the HoloToolkit objects to the camera
        Vector3 sceneOrigin = Camera.main.transform.position;
        Parent_Scene.transform.position = sceneOrigin;
        MappingObserver.SetObserverOrigin(sceneOrigin);
    }

    private void OnEnable()
    {
        InteractionManager.SourcePressed += OnAirTap;
    }

    private void OnDisable()
    {
        InteractionManager.SourcePressed -= OnAirTap;
    }

    private void OnAirTap(InteractionSourceState state)
    {
        if ((SpatialUnderstanding.Instance.ScanState == SpatialUnderstanding.ScanStates.Scanning) &&
            !SpatialUnderstanding.Instance.ScanStatsReportStillWorking)
        {
            SpatialUnderstanding.Instance.RequestFinishScan();
        }
    }

    private void Update_DebugDisplay(float deltaTime)
    {
        // Basic checks
        if (DebugDisplay == null)
        {
            return;
        }

        // Update display text
        DebugDisplay.text = PrimaryText;
        DebugDisplay.color = PrimaryColor;
        DebugSubDisplay.text = DetailsText;
    }

    private void Update_KeyboardInput(float deltaTime)
    {
        // Toggle SurfaceMapping & CustomUnderstandingMesh visibility
        if (Input.GetKeyDown(KeyCode.BackQuote) &&
            (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift)))
        {
            SpatialMappingManager.Instance.DrawVisualMeshes = !SpatialMappingManager.Instance.DrawVisualMeshes;
            Debug.Log("SpatialUnderstanding -> ProcessedMap.drawMeshes=" + SpatialMappingManager.Instance.DrawVisualMeshes);
        }
        else if (Input.GetKeyDown(KeyCode.BackQuote) &&
                 (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
        {
            SpatialUnderstanding.Instance.UnderstandingCustomMesh.DrawProcessedMesh = !SpatialUnderstanding.Instance.UnderstandingCustomMesh.DrawProcessedMesh;
            Debug.Log("SpatialUnderstanding -> ProcessedMap.drawMeshes=" + SpatialUnderstanding.Instance.UnderstandingCustomMesh.DrawProcessedMesh);
        }
    }

    private void Update()
    {
        // Updates
        Update_DebugDisplay(Time.deltaTime);
        Update_KeyboardInput(Time.deltaTime);
    }
}