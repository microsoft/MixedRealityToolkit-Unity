// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.SceneSystem;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    [MixedRealityServiceInspector(typeof(MixedRealitySceneSystem))]
    public class SceneSystemInspector : BaseMixedRealityServiceInspector
    {
        private const float maxLoadButtonWidth = 50;
        private const float tagLoadButtonSetWidth = 120;

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

                bool selected = lightingScene.Name == sceneSystem.ActiveLightingScene;

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
                EditorGUILayout.HelpBox("You can load / unload scenes in the editor by clicking on their tags or names", MessageType.Info);
                EditorGUILayout.Space();
            }

            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Load / Unload by tag", EditorStyles.miniBoldLabel);
            List<string> contentTags = new List<string>(sceneSystem.ContentTags);
            if (contentTags.Count == 0)
            {
                EditorGUILayout.LabelField("(No scenes with content tags found)", EditorStyles.miniLabel);
            }
            else
            {
                foreach (string tag in contentTags)
                {
                    EditorGUILayout.BeginVertical(GUILayout.MaxWidth(tagLoadButtonSetWidth));
                    EditorGUILayout.LabelField(tag, EditorStyles.miniLabel);
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Load", EditorStyles.toolbarButton, GUILayout.MaxWidth(maxLoadButtonWidth)))
                    {
                        foreach (SceneInfo contentScene in sceneSystem.ContentScenes)
                        {
                            if (contentScene.Tag == tag)
                            {
                                EditorSceneManager.OpenScene(contentScene.Path, OpenSceneMode.Additive);
                            }
                        }
                    }
                    if (GUILayout.Button("Unload", EditorStyles.toolbarButton, GUILayout.MaxWidth(maxLoadButtonWidth)))
                    {
                        foreach (SceneInfo contentScene in sceneSystem.ContentScenes)
                        {
                            if (contentScene.Tag == tag)
                            {
                                Scene scene = EditorSceneManager.GetSceneByName(contentScene.Name);
                                EditorSceneManager.CloseScene(scene, false);
                            }
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Load / Unload individually", EditorStyles.miniBoldLabel);
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
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();
        }
    }
}