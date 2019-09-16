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
    [MixedRealityServiceInspector(typeof(IMixedRealitySceneSystem))]
    public class SceneSystemInspector : BaseMixedRealityServiceInspector
    {
        private const float maxLoadButtonWidth = 50;
        private const float tagLoadButtonSetWidth = 120;

        private static readonly Color enabledColor = GUI.backgroundColor;
        private static readonly Color disabledColor = Color.Lerp(enabledColor, Color.clear, 0.5f);
        private static readonly Color errorColor = Color.Lerp(GUI.backgroundColor, Color.red, 0.5f);

        private SceneActivationToken activationToken = new SceneActivationToken();
        private static bool requireActivationToken = false;
        private LightingSceneTransitionType transitionType = LightingSceneTransitionType.None;
        private LoadSceneMode loadSceneMode = LoadSceneMode.Additive;
        private float transitionSpeed = 1f;

        public override bool DrawProfileField { get { return true; } }

        public override void DrawInspectorGUI(object target)
        {
            // Get the scene system itself
            IMixedRealitySceneSystem sceneSystem = target as IMixedRealitySceneSystem;
            // Get the scene system's editor interface
            IMixedRealitySceneSystemEditor sceneSystemEditor = target as IMixedRealitySceneSystemEditor;

            if (sceneSystemEditor == null)
            {
                EditorGUILayout.HelpBox("This scene service implementation does not implement IMixedRealitySceneSystemEditor. Inspector will not be rendered.", MessageType.Info);
                return;
            }

            GUI.color = enabledColor;

            MixedRealitySceneSystemProfile profile = sceneSystem.ConfigurationProfile as MixedRealitySceneSystemProfile;

            if (profile.UseLightingScene)
            {
                EditorGUILayout.LabelField("Lighting Scene", EditorStyles.boldLabel);
                List<SceneInfo> lightingScenes = new List<SceneInfo>(sceneSystemEditor.LightingScenes);
                if (lightingScenes.Count == 0)
                {
                    EditorGUILayout.LabelField("(No lighting scenes found)", EditorStyles.miniLabel);
                }
                else
                {
                    RenderLightingScenes(sceneSystem, sceneSystemEditor, lightingScenes);
                }
                EditorGUILayout.Space();
            }

            GUI.color = enabledColor;

            EditorGUILayout.LabelField("Content Scenes", EditorStyles.boldLabel);
            List<SceneInfo> contentScenes = new List<SceneInfo>(sceneSystemEditor.ContentScenes);
            if (contentScenes.Count == 0)
            {
                EditorGUILayout.LabelField("(No content scenes found)", EditorStyles.miniLabel);
            }
            else
            {
                if (Application.isPlaying)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUILayout.LabelField("Current Scene Operation", EditorStyles.boldLabel);
                    loadSceneMode = (LoadSceneMode)EditorGUILayout.EnumPopup("Load Mode", loadSceneMode);
                    EditorGUILayout.Toggle("Scene Operation In Progress", sceneSystem.SceneOperationInProgress);
                    EditorGUILayout.FloatField("Progress", sceneSystem.SceneOperationProgress);

                    requireActivationToken = EditorGUILayout.Toggle("Require Manual Scene Activation", requireActivationToken);

                    if (requireActivationToken && activationToken.ReadyToProceed)
                    {
                        if (GUILayout.Button("Allow Scene Activation"))
                        {
                            activationToken.AllowSceneActivation = true;
                        }
                    }
                    else
                    {
                        activationToken.AllowSceneActivation = true;
                    }

                    EditorGUILayout.EndVertical();
                }

                EditorGUI.BeginDisabledGroup(sceneSystem.SceneOperationInProgress);
                RenderContentScenes(sceneSystem, sceneSystemEditor, contentScenes);
                EditorGUI.EndDisabledGroup();
            }

            EditorGUILayout.Space();
        }

        private void RenderLightingScenes(IMixedRealitySceneSystem sceneSystem, IMixedRealitySceneSystemEditor sceneSystemEditor, List<SceneInfo> lightingScenes)
        {
            EditorGUILayout.HelpBox("Select the active lighting scene by clicking its name.", MessageType.Info);

            if (Application.isPlaying)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("Current Scene Operation", EditorStyles.boldLabel);
                EditorGUILayout.Toggle("Scene Operation In Progress", sceneSystem.LightingOperationInProgress);
                EditorGUILayout.FloatField("Progress", sceneSystem.LightingOperationProgress);
                transitionType = (LightingSceneTransitionType)EditorGUILayout.EnumPopup("Lighting transition type", transitionType);
                if (transitionType != LightingSceneTransitionType.None)
                {
                    transitionSpeed = EditorGUILayout.Slider("Lighting transition speed", transitionSpeed, 0f, 10f);
                }
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical();
            foreach (SceneInfo lightingScene in lightingScenes)
            {
                if (lightingScene.IsEmpty)
                {
                    GUI.color = errorColor;
                    GUILayout.Button("(Scene Missing)", EditorStyles.toolbarButton);
                    continue;
                }

                bool selected = lightingScene.Name == sceneSystem.ActiveLightingScene;

                GUI.color = selected ? enabledColor : disabledColor;
                if (GUILayout.Button(lightingScene.Name, EditorStyles.toolbarButton) && !selected)
                {
                    sceneSystem.SetLightingScene(lightingScene.Name, transitionType, transitionSpeed);
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void RenderContentScenes(IMixedRealitySceneSystem sceneSystem, IMixedRealitySceneSystemEditor sceneSystemEditor, List<SceneInfo> contentScenes)
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Load / Unload by tag", EditorStyles.miniBoldLabel);
            List<string> contentTags = new List<string>(sceneSystemEditor.ContentTags);

            if (contentTags.Count == 0)
            {
                EditorGUILayout.LabelField("(No scenes with content tags found)", EditorStyles.miniLabel);
            }
            else
            {
                foreach (string tag in contentTags)
                {
                    using (new EditorGUILayout.VerticalScope(GUILayout.MaxWidth(tagLoadButtonSetWidth)))
                    {
                        EditorGUILayout.LabelField(tag, EditorStyles.miniLabel);
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            if (GUILayout.Button("Load", EditorStyles.miniButton, GUILayout.MaxWidth(maxLoadButtonWidth)))
                            {
                                if (Application.isPlaying)
                                {
                                    ServiceContentLoadByTag(sceneSystem, tag);
                                }
                                else
                                {
                                    foreach (SceneInfo contentScene in sceneSystemEditor.ContentScenes)
                                    {
                                        if (contentScene.Tag == tag)
                                        {
                                            EditorSceneManager.OpenScene(contentScene.Path, OpenSceneMode.Additive);
                                        }
                                    }
                                }
                            }

                            if (GUILayout.Button("Unload", EditorStyles.miniButton, GUILayout.MaxWidth(maxLoadButtonWidth)))
                            {
                                if (Application.isPlaying)
                                {
                                    ServiceContentUnloadByTag(sceneSystem, tag);
                                }
                                else
                                {
                                    foreach (SceneInfo contentScene in sceneSystemEditor.ContentScenes)
                                    {
                                        if (contentScene.Tag == tag)
                                        {
                                            Scene scene = EditorSceneManager.GetSceneByName(contentScene.Name);
                                            EditorSceneManager.CloseScene(scene, false);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Load / Unload by build index order", EditorStyles.miniBoldLabel);
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUI.BeginDisabledGroup(!sceneSystem.PrevContentExists);
                if (GUILayout.Button("Load Prev Content", EditorStyles.miniButton))
                {
                    if (Application.isPlaying)
                    {
                        ServiceContentLoadPrev(sceneSystem);
                    }
                    else
                    {
                        sceneSystemEditor.EditorLoadPrevContent();
                    }
                }
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(!sceneSystem.NextContentExists);
                if (GUILayout.Button("Load Next Content", EditorStyles.miniButton))
                {
                    if (Application.isPlaying)
                    {
                        ServiceContentLoadNext(sceneSystem);
                    }
                    else
                    {
                        sceneSystemEditor.EditorLoadNextContent();
                    }
                }
                EditorGUI.EndDisabledGroup();

            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Load / Unload individually", EditorStyles.miniBoldLabel);
            foreach (SceneInfo contentScene in sceneSystemEditor.ContentScenes)
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
                if (GUILayout.Button(contentScene.Name, EditorStyles.toolbarButton))
                {
                    if (Application.isPlaying)
                    {
                        if (loaded)
                        {
                            ServiceContentUnload(sceneSystem, contentScene.Name);
                        }
                        else
                        {
                            ServiceContentLoad(sceneSystem, contentScene.Name);
                        }
                    }
                    else
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

        private async void ServiceContentLoadNext(IMixedRealitySceneSystem sceneSystem)
        {
            await sceneSystem.LoadNextContent(false, LoadSceneMode.Single, activationToken);
        }

        private async void ServiceContentLoadPrev(IMixedRealitySceneSystem sceneSystem)
        {
            await sceneSystem.LoadPrevContent(false, LoadSceneMode.Single, activationToken);
        }

        private async void ServiceContentLoadByTag(IMixedRealitySceneSystem sceneSystem, string tag)
        {
            if (requireActivationToken)
            {
                activationToken.AllowSceneActivation = false;
            }

            await sceneSystem.LoadContentByTag(tag, loadSceneMode, activationToken);
        }

        private async void ServiceContentUnloadByTag(IMixedRealitySceneSystem sceneSystem, string tag)
        {
            await sceneSystem.UnloadContentByTag(tag);
        }

        private async void ServiceContentLoad(IMixedRealitySceneSystem sceneSystem, string sceneName)
        {
            if (requireActivationToken)
            {
                activationToken.AllowSceneActivation = false;
            }

            await sceneSystem.LoadContent(sceneName, loadSceneMode, activationToken);
        }

        private async void ServiceContentUnload(IMixedRealitySceneSystem sceneSystem, string sceneName)
        {
            await sceneSystem.UnloadContent(sceneName);
        }
    }
}
