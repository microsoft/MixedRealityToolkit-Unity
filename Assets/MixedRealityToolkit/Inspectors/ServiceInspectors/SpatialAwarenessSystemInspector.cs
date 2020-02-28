// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    [MixedRealityServiceInspector(typeof(IMixedRealitySpatialAwarenessSystem))]
    public class SpatialAwarenessSystemInspector : BaseMixedRealityServiceInspector
    {
        private const string ShowObserverBoundaryKey = "MRTK_SpatialAwarenessSystemInspector_ShowObserverBoundaryKey";
        private const string ShowObserverOriginKey = "MRTK_SpatialAwarenessSystemInspector_ShowObserverOriginKey";

        private static bool ShowObserverBoundary = false;
        private static bool ShowObserverOrigin = false;

        private static readonly Color[] observerColors = new Color[] { Color.blue, Color.cyan, Color.green, Color.magenta, Color.red, Color.yellow };
        private static readonly Color originColor = new Color(0.75f, 0.1f, 0.75f, 0.75f);
        private static readonly Color enabledColor = GUI.backgroundColor;
        private static readonly Color disabledColor = Color.Lerp(enabledColor, Color.clear, 0.5f);
        
        public override bool AlwaysDrawSceneGUI { get { return false; } }
        
        public override void DrawInspectorGUI(object target)
        {
            IMixedRealitySpatialAwarenessSystem spatial = (IMixedRealitySpatialAwarenessSystem)target;
            IMixedRealityDataProviderAccess dataProviderAccess = (IMixedRealityDataProviderAccess)spatial;

            EditorGUILayout.LabelField("Observers", EditorStyles.boldLabel);
            int observerIndex = 0;

            var dataProviders = dataProviderAccess?.GetDataProviders();
            if (dataProviders != null)
            {
                foreach (IMixedRealitySpatialAwarenessObserver observer in dataProviders)
                {
                    GUI.color = observer.IsRunning ? enabledColor : disabledColor;

                    using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        GUI.color = GetObserverColor(observerIndex);
                        GUILayout.Button(observer.Name);
                        GUI.color = observer.IsRunning ? enabledColor : disabledColor;

                        EditorGUILayout.Toggle("Running", observer.IsRunning);
                        EditorGUILayout.LabelField("Source", observer.SourceName);
                        EditorGUILayout.Toggle("Is Stationary", observer.IsStationaryObserver);
                        EditorGUILayout.FloatField("Update Interval", observer.UpdateInterval);

                        EditorGUILayout.Space();
                        EditorGUILayout.LabelField("Volume Properties", EditorStyles.boldLabel);
                        EditorGUILayout.EnumPopup("Volume Type", observer.ObserverVolumeType);
                        EditorGUILayout.Vector3Field("Origin", observer.ObserverOrigin);
                        EditorGUILayout.Vector3Field("Rotation", observer.ObserverRotation.eulerAngles);
                        EditorGUILayout.Vector3Field("Extents", observer.ObservationExtents);
                    }
                    observerIndex++;
                }
            }

            GUI.color = enabledColor;

            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Observers will be populated once you enter play mode.", MessageType.Info);
            }
            else if (observerIndex == 0)
            {
                EditorGUILayout.LabelField("(None found)", EditorStyles.miniLabel);
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Editor Options", EditorStyles.boldLabel);
            ShowObserverBoundary = SessionState.GetBool(ShowObserverBoundaryKey, false);
            ShowObserverBoundary = EditorGUILayout.Toggle("Show Observer Boundaries", ShowObserverBoundary);
            SessionState.SetBool(ShowObserverBoundaryKey, ShowObserverBoundary);

            ShowObserverOrigin = SessionState.GetBool(ShowObserverOriginKey, false);
            ShowObserverOrigin = EditorGUILayout.Toggle("Show Observer Origins", ShowObserverOrigin);
            SessionState.SetBool(ShowObserverOriginKey, ShowObserverOrigin);
        }

        public override void DrawSceneGUI(object target, SceneView sceneView) { }

        public override void DrawGizmos(object target)
        {
            if (!(ShowObserverBoundary || ShowObserverOrigin))
            {
                return;
            }

            IMixedRealitySpatialAwarenessSystem spatial = (IMixedRealitySpatialAwarenessSystem)target;
            IMixedRealityDataProviderAccess dataProviderAccess = (IMixedRealityDataProviderAccess)spatial;

            var dataProviders = dataProviderAccess?.GetDataProviders();
            if (dataProviders != null)
            {
                int observerIndex = 0;

                foreach (IMixedRealitySpatialAwarenessObserver observer in dataProviders)
                {
                    Gizmos.color = GetObserverColor(observerIndex);

                    if (ShowObserverBoundary)
                    {
                        switch (observer.ObserverVolumeType)
                        {
                            case VolumeType.None:
                                break;

                            case VolumeType.AxisAlignedCube:
                                Gizmos.DrawWireCube(observer.ObserverOrigin, observer.ObservationExtents);
                                break;

                            case VolumeType.Sphere:
                                Gizmos.DrawWireSphere(observer.ObserverOrigin, observer.ObservationExtents.x);
                                break;

                            case VolumeType.UserAlignedCube:
                                Gizmos.DrawWireCube(observer.ObserverOrigin, observer.ObservationExtents);
                                break;
                        }
                    }

                    Gizmos.matrix = Matrix4x4.identity;

                    if (ShowObserverOrigin)
                    {
                        Gizmos.DrawSphere(observer.ObserverOrigin, 0.1f);
                    }

                    observerIndex++;
                }
            }
        }

        private Color GetObserverColor(int observerIndex)
        {
            if (observerIndex >= observerColors.Length)
            {
                observerIndex = 0;
            }

            return Color.Lerp(Color.white, observerColors[observerIndex], 0.35f);
        }
    }
}