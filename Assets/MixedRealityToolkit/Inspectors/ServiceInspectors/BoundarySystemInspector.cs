// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Boundary;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    [MixedRealityServiceInspector(typeof(MixedRealityBoundarySystem))]
    public class BoundarySystemInspector : BaseMixedRealityServiceInspector
    {
        private static readonly Color enabledColor = GUI.backgroundColor;
        private static readonly Color disabledColor = Color.Lerp(enabledColor, Color.clear, 0.5f);

        private const string ShowCeilingInSceneViewKey = "MRTK_BoundarySystemInspector_ShowCeilingInSceneView";
        private const string ShowWallsInSceneViewKey = "MRTK_BoundarySystemInspector_ShowWallsInSceneView";
        private const string ShowFloorInSceneViewKey = "MRTK_BoundarySystemInspector_ShowFloorInSceneVieww";
        private const string ShowPlayAreaInSceneViewKey = "MRTK_BoundarySystemInspector_ShowPlayAreaInSceneView";
        private const string ShowTrackedAreaInSceneViewKey = "MRTK_BoundarySystemInspector_ShowTrackedAreaInSceneView";

        private static bool ShowCeilingInSceneView = false;
        private static bool ShowWallsInSceneView = false;
        private static bool ShowFloorInSceneView = false;
        private static bool ShowPlayAreaInSceneView = false;
        private static bool ShowTrackedAreaInSceneView = false;

        private static readonly Color BoundaryPlaneColor = Color.Lerp(Color.magenta, Color.clear, 0.65f);
        private static readonly Color BoundaryEdgeColor = Color.cyan;

        private static Vector3[] boundaryPlane = new Vector3[4];
        private static Vector3 P1;
        private static Vector3 P2;
        private static Vector3 P3;
        private static Vector3 P4;
        
        // When playing, the profile is no longer relevant
        // Show the options below instead
        public override bool DrawProfileField { get { return !Application.isPlaying; } }

        public override bool AlwaysDrawSceneGUI { get { return true; } }

        public override void DrawInspectorGUI(object target)
        {
            MixedRealityBoundarySystem boundary = (MixedRealityBoundarySystem)target;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Boundary Settings", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Source", boundary.SourceName);

            Edge[] edge = boundary.Bounds;
            if (edge.Length == 0)
            {
                EditorGUILayout.HelpBox("No boundary edges found. You may not have a mixed reality device plugged in, or the device is not configured to use a boundary.", MessageType.Info);
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Editor Settings", EditorStyles.boldLabel);

            ShowCeilingInSceneView = SessionState.GetBool(ShowCeilingInSceneViewKey, false);
            ShowWallsInSceneView = SessionState.GetBool(ShowWallsInSceneViewKey, false);
            ShowFloorInSceneView = SessionState.GetBool(ShowFloorInSceneViewKey, false);
            ShowPlayAreaInSceneView = SessionState.GetBool(ShowPlayAreaInSceneViewKey, false);
            ShowTrackedAreaInSceneView = SessionState.GetBool(ShowTrackedAreaInSceneViewKey, false);

            ShowCeilingInSceneView = EditorGUILayout.Toggle("Show Ceiling", ShowCeilingInSceneView);
            ShowWallsInSceneView = EditorGUILayout.Toggle("Show Walls", ShowWallsInSceneView);
            ShowFloorInSceneView = EditorGUILayout.Toggle("Show Floor", ShowFloorInSceneView);
            ShowPlayAreaInSceneView = EditorGUILayout.Toggle("Show Play Area", ShowPlayAreaInSceneView);
            ShowTrackedAreaInSceneView = EditorGUILayout.Toggle("Show Tracked Area", ShowTrackedAreaInSceneView);

            SessionState.SetBool(ShowCeilingInSceneViewKey, ShowCeilingInSceneView);
            SessionState.SetBool(ShowWallsInSceneViewKey, ShowWallsInSceneView);
            SessionState.SetBool(ShowFloorInSceneViewKey, ShowFloorInSceneView);
            SessionState.SetBool(ShowPlayAreaInSceneViewKey, ShowPlayAreaInSceneView);
            SessionState.SetBool(ShowTrackedAreaInSceneViewKey, ShowTrackedAreaInSceneView);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Runtime Settings", EditorStyles.boldLabel);

            if (Application.isPlaying)
            {
                GUI.color = enabledColor;
                boundary.ShowBoundaryCeiling = EditorGUILayout.Toggle("Show Ceiling", boundary.ShowBoundaryCeiling);
                boundary.ShowBoundaryWalls = EditorGUILayout.Toggle("Show Walls", boundary.ShowBoundaryWalls);
                boundary.ShowFloor = EditorGUILayout.Toggle("Show Floor", boundary.ShowFloor);
                boundary.ShowPlayArea = EditorGUILayout.Toggle("Show Play Area", boundary.ShowPlayArea);
                boundary.ShowTrackedArea = EditorGUILayout.Toggle("Show Tracked Area", boundary.ShowTrackedArea);
            }
            else
            {
                GUI.color = disabledColor;
                EditorGUILayout.Toggle("Show Ceiling", false);
                EditorGUILayout.Toggle("Show Walls", false);
                EditorGUILayout.Toggle("Show Floor", false);
                EditorGUILayout.Toggle("Show Play Area", false);
                EditorGUILayout.Toggle("Show Tracked Area", false);
            }
        }

        public override void DrawSceneGUI(object target, SceneView sceneView)
        {
            IMixedRealityBoundarySystem boundary = (IMixedRealityBoundarySystem)target;
                        
            Edge[] edge = boundary.Bounds;

            if (edge.Length == 0)
                return;

            float boundaryFloorHeight = (boundary.FloorHeight ?? 0);

            if (ShowWallsInSceneView)
            {
                for (int i = 0; i < edge.Length; i++)
                {
                    Vector2 pointA = edge[i].PointA;
                    Vector2 pointB = edge[i].PointB;
                    boundaryPlane[0] = new Vector3(pointA.x, boundaryFloorHeight, pointA.y);
                    boundaryPlane[1] = new Vector3(pointB.x, boundaryFloorHeight, pointB.y);
                    boundaryPlane[2] = new Vector3(pointB.x, boundary.BoundaryHeight, pointB.y);
                    boundaryPlane[3] = new Vector3(pointA.x, boundary.BoundaryHeight, pointA.y);

                    Handles.DrawSolidRectangleWithOutline(boundaryPlane, BoundaryPlaneColor, BoundaryEdgeColor);
                }
            }

            if (ShowTrackedAreaInSceneView)
            {
                Vector2 center;
                float angle;
                float width;
                float height;

                if (boundary.TryGetRectangularBoundsParams(out center, out angle, out width, out height))
                {
                    Quaternion rotation = Quaternion.Euler(0, -angle, 0);
                    Vector3 boundsCenter = new Vector3(center.x, 0, center.y);
                    float xExtents = width * 0.5f;
                    float zExtents = height * 0.5f;

                    P1 = RotatePointAroundPivot(new Vector3(center.x + xExtents, 0, center.y + zExtents), boundsCenter, rotation);
                    P2 = RotatePointAroundPivot(new Vector3(center.x + xExtents, 0, center.y - zExtents), boundsCenter, rotation);
                    P3 = RotatePointAroundPivot(new Vector3(center.x - xExtents, 0, center.y - zExtents), boundsCenter, rotation);
                    P4 = RotatePointAroundPivot(new Vector3(center.x - xExtents, 0, center.y + zExtents), boundsCenter, rotation);

                    P1.y = boundaryFloorHeight;
                    P2.y = boundaryFloorHeight;
                    P3.y = boundaryFloorHeight;
                    P4.y = boundaryFloorHeight;

                    boundaryPlane[0] = P1;
                    boundaryPlane[1] = P2;
                    boundaryPlane[2] = P3;
                    boundaryPlane[3] = P4;
                    Handles.DrawSolidRectangleWithOutline(boundaryPlane, BoundaryPlaneColor, BoundaryEdgeColor);
                }
            }
        }

        public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation)
        {
            return rotation * (point - pivot) + pivot;
        }
    }
}