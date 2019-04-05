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
        private const string ShowBoundaryInSceneViewKey = "MRTK_BoundarySystemInspector_ShowBoundaryInSceneView";
        private static bool ShowBoundaryInSceneView = false;
        private static Vector3[] boundaryPlane = new Vector3[4];

        public override bool AlwaysDrawSceneGUI { get { return true; } }

        public override void DrawInspectorGUI(object target)
        {
            IMixedRealityBoundarySystem boundary = (IMixedRealityBoundarySystem)target;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Boundary Settings", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Source", boundary.SourceName);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Editor Settings", EditorStyles.boldLabel);
            ShowBoundaryInSceneView = SessionState.GetBool(ShowBoundaryInSceneViewKey, false);
            ShowBoundaryInSceneView = EditorGUILayout.Toggle("Show boundary in scene view", ShowBoundaryInSceneView);
            SessionState.SetBool(ShowBoundaryInSceneViewKey, ShowBoundaryInSceneView);

            Edge[] edge = boundary.Bounds;
            if (edge.Length == 0)
            {
                EditorGUILayout.HelpBox("No boundary edges found. You may not have a mixed reality device plugged in, or it's not configured to use a boundary.", MessageType.Info);
                return;
            }
        }

        public override void DrawSceneGUI(object target, SceneView sceneView)
        {
            if (!ShowBoundaryInSceneView)
                return;

            IMixedRealityBoundarySystem boundary = (IMixedRealityBoundarySystem)target;
                        
            Edge[] edge = boundary.Bounds;

            if (edge.Length == 0)
                return;

            float boundaryFloorHeight = (boundary.FloorHeight ?? 0);

            for (int i = 0; i < edge.Length; i++)
            {
                Vector2 pointA = edge[i].PointA;
                Vector2 pointB = edge[i].PointB;
                Vector3 pointABot = new Vector3(pointA.x, boundaryFloorHeight, pointA.y);
                Vector3 pointATop = new Vector3(pointA.x, boundary.BoundaryHeight, pointA.y);
                Vector3 pointBBot = new Vector3(pointB.x, boundaryFloorHeight, pointB.y);
                Vector3 pointBTop = new Vector3(pointB.x, boundary.BoundaryHeight, pointB.y);

                boundaryPlane[0] = pointABot;
                boundaryPlane[1] = pointATop;
                boundaryPlane[2] = pointBTop;
                boundaryPlane[3] = pointBBot;

                Handles.DrawSolidRectangleWithOutline(boundaryPlane, Color.Lerp(Color.cyan, Color.clear, 0.5f), Color.magenta);
            }
        }
    }
}