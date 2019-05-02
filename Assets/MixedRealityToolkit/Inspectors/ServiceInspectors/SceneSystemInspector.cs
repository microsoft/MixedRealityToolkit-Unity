// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.SceneSystem;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    [MixedRealityServiceInspector(typeof(MixedRealitySceneSystem))]
    public class SceneSystemInspector : BaseMixedRealityServiceInspector
    {
        private const string ShowObserverBoundaryKey = "MRTK_SpatialAwarenessSystemInspector_ShowObserverBoundaryKey";
        private const string ShowObserverOriginKey = "MRTK_SpatialAwarenessSystemInspector_ShowObserverOriginKey";

        private static bool ShowObserverBoundary = false;
        private static bool ShowObserverOrigin = false;

        private static readonly Color[] observerColors = new Color[] { Color.blue, Color.cyan, Color.green, Color.magenta, Color.red, Color.yellow };
        private static readonly Color originColor = new Color(0.75f, 0.1f, 0.75f, 0.75f);
        private static readonly Color enabledColor = GUI.backgroundColor;
        private static readonly Color disabledColor = Color.Lerp(enabledColor, Color.clear, 0.5f);
        
        public override bool DrawProfileField { get { return true; } }

        public override void DrawInspectorGUI(object target)
        {
            MixedRealitySceneSystem sceneSystem = (MixedRealitySceneSystem)target;

            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Select the active lighting scene by clicking its name.", MessageType.Info);
                EditorGUILayout.Space();
            }

            EditorGUILayout.LabelField("Lighting Scene", EditorStyles.boldLabel);

            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical();
            foreach (SceneInfo lightingScene in sceneSystem.LightingScenes)
            {
                bool selected = lightingScene.Name == sceneSystem.LightingSceneName;

                GUI.color = selected ? Color.Lerp(Color.green, Color.white,0.5f) : Color.white;
                if (GUILayout.Button(lightingScene.Name, EditorStyles.toolbarButton) && !selected && !Application.isPlaying)
                {
                    sceneSystem.SetLightingScene(lightingScene.Name);
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Content Scenes", EditorStyles.boldLabel);
            foreach (SceneInfo contentScene in sceneSystem.ContentScenes)
            {
                Scene scene = EditorSceneManager.GetSceneByName(contentScene.Name);
                bool loaded = scene.isLoaded;

                GUI.color = loaded ? Color.Lerp(Color.green, Color.white, 0.5f) : Color.white;
                if (GUILayout.Button(contentScene.Name, EditorStyles.toolbarButton) && !Application.isPlaying)
                {
                    if (loaded)
                    {
                        EditorSceneManager.CloseScene(scene, false);
                    }
                    else
                    {
                        EditorSceneManager.OpenScene(contentScene.Path, OpenSceneMode.Additive);
                    }
                }
            }
        }
    }
}