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
        private static readonly Color enabledColor = GUI.backgroundColor;
        private static readonly Color disabledColor = Color.Lerp(enabledColor, Color.clear, 0.5f);
        private static readonly Color errorColor = Color.Lerp(GUI.backgroundColor, Color.red, 0.5f);

        public override bool DrawProfileField { get { return true; } }

        public override void DrawInspectorGUI(object target)
        {
            MixedRealitySceneSystem sceneSystem = (MixedRealitySceneSystem)target;

            GUI.color = enabledColor;

            EditorGUILayout.LabelField("Lighting Scene", EditorStyles.boldLabel);
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Select the active lighting scene by clicking its name.", MessageType.Info);
                EditorGUILayout.Space();
            }
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical();
            foreach (SceneInfo lightingScene in sceneSystem.LightingScenes)
            {
                if (lightingScene.IsEmpty)
                {
                    GUI.color = errorColor;
                    GUILayout.Button("(Scene Missing)", EditorStyles.toolbarButton);
                    continue;
                }

                bool selected = lightingScene.Name == sceneSystem.LightingSceneName;

                GUI.color = selected ? enabledColor : disabledColor;
                if (GUILayout.Button(lightingScene.Name, EditorStyles.toolbarButton) && !selected && !Application.isPlaying)
                {
                    sceneSystem.SetLightingScene(lightingScene.Name);
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();

            GUI.color = enabledColor;

            EditorGUILayout.LabelField("Content Scenes", EditorStyles.boldLabel);
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Load / unload content scenes by clicking their names.", MessageType.Info);
                EditorGUILayout.Space();
            }
            EditorGUILayout.Space();
            foreach (SceneInfo contentScene in sceneSystem.ContentScenes)
            {
                if (contentScene.IsEmpty)
                {
                    GUI.color = errorColor;
                    GUILayout.Button("(Scene Missing)", EditorStyles.toolbarButton);
                    continue;
                }

                Scene scene = EditorSceneManager.GetSceneByName(contentScene.Name);
                bool loaded = scene.isLoaded;

                GUI.color = loaded ? enabledColor : disabledColor;
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