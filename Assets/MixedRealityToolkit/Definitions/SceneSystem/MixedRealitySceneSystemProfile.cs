// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SceneSystem
{
    /// <summary>
    /// Configuration profile settings for setting up scene system.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Scene System Profile", fileName = "MixedRealitySceneSystemProfile", order = (int)CreateProfileMenuItemIndices.SceneSystem)]
    [MixedRealityServiceProfile(typeof(IMixedRealitySceneSystem))]
    public class MixedRealitySceneSystemProfile : BaseMixedRealityProfile
    {
        public bool UseManagerScene { get { return useManagerScene && !managerScene.IsEmpty; } }

        public bool UseLightingScene { get { return useLightingScene && lightingScenes.Count > 0; } }

        public SceneInfo ManagerScene => managerScene;

        public SceneInfo DefaultLightingScene { get { return lightingScenes[defaultLightingSceneIndex]; } }

        public IEnumerable<SceneInfo> LightingScenes { get { return lightingScenes; } }

        public IEnumerable<SceneInfo> ContentScenes { get { return contentScenes; } }

        public IEnumerable<string> ContentTags { get { return contentTags; } }

        public bool ManageBuildSettings => manageBuildSettings;

        [SerializeField]
        [Tooltip("Using a manager scene ensures a MixedRealityToolkit instance will always be loaded first in your application. It also ensures that this scene and instance will never be unloaded.")]
        private bool useManagerScene = true;

        [SerializeField]
        private SceneInfo managerScene = default(SceneInfo);

        [SerializeField]
        [Tooltip("Using a lighting scene ensures that the same lighting settings will be enforced across all loaded scenes.")]
        private bool useLightingScene = true;

        [SerializeField]
        private int defaultLightingSceneIndex = 0;

        [SerializeField]
        [Tooltip("Scenes used to control lighting settings. Only one lighting scene can be loaded at any given time.")]
        private List<SceneInfo> lightingScenes = new List<SceneInfo>();

        [SerializeField]
        [Tooltip("Scenes in your build settings which aren't a lighting or manager scene. These can be loaded and unloaded in any combination.")]
        private List<SceneInfo> contentScenes = new List<SceneInfo>();

        [SerializeField]
        [Tooltip("Cached content tags found in your content scenes")]
        private List<string> contentTags = new List<string>();

        [SerializeField]
        [Tooltip("If true, scene service will manage which scenes are in the build settings and in what order.")]
        private bool manageBuildSettings = true;

        public bool GetLightingSceneObject(string lightingSceneName, out SceneInfo lightingScene)
        {
            // TODO generate a lookup
            lightingScene = SceneInfo.Empty;
            foreach (SceneInfo lso in lightingScenes)
            {
                if (lso.Name == lightingSceneName)
                {
                    lightingScene = lso;
                    break;
                }
            }
            return !lightingScene.IsEmpty;
        }

        public IEnumerable<string> GetContentSceneNamesByTag(string tag)
        {
            foreach (SceneInfo contentScene in contentScenes)
            {
                if (contentScene.Tag == tag)
                    yield return contentScene.Name;
            }
        }

#if UNITY_EDITOR
        #region validation
        private void OnValidate()
        {
            if (Application.isPlaying || EditorApplication.isCompiling)
            {
                return;
            }

            // Remove any duplicate entries from our lighting and content scene lists
            if (RemoveOrClearDuplicateEntries(lightingScenes) || RemoveOrClearDuplicateEntries(contentScenes))
            {
                EditorUtility.SetDirty(this);
            }

            // Ensure that manager scenes are not contained in content or lighting scenes
            if (UseManagerScene && (RemoveScene(lightingScenes, managerScene) || RemoveScene(contentScenes, managerScene)))
            {
                EditorUtility.SetDirty(this);
            }

            // Ensure that content scenes are not included in lighting scenes
            if (UseLightingScene && RemoveScenes(lightingScenes, contentScenes))
            {
                EditorUtility.SetDirty(this);
            }

            // Build our content tags
            contentTags.Clear();
            foreach (SceneInfo contentScene in contentScenes)
            {
                if (string.IsNullOrEmpty(contentScene.Tag))
                {
                    continue;
                }

                if (contentScene.Tag == "Untagged")
                {
                    continue;
                }

                if (!contentTags.Contains(contentScene.Tag))
                {
                    contentTags.Add(contentScene.Tag);
                }
            }

            defaultLightingSceneIndex = Mathf.Clamp(defaultLightingSceneIndex, 0, lightingScenes.Count - 1);
        }

        private static bool RemoveScenes(List<SceneInfo> sceneList, List<SceneInfo> scenesToRemove)
        {
            bool changed = false;

            for (int i = sceneList.Count - 1; i >= 0; i--)
            {
                if (sceneList[i].IsEmpty)
                {
                    continue;
                }

                foreach (SceneInfo sceneToRemove in scenesToRemove)
                {
                    if (sceneToRemove.IsEmpty)
                    {
                        continue;
                    }

                    if (sceneList[i].Asset == sceneToRemove.Asset)
                    {
                        Debug.LogWarning("Removing scene " + sceneToRemove.Name + " from scene list.");
                        sceneList[i] = SceneInfo.Empty;
                        changed = true;
                        break;
                    }
                }
            }

            return changed;
        }

        private static bool RemoveScene(List<SceneInfo> sceneList, SceneInfo sceneToRemove)
        {
            bool changed = false;

            for (int i = sceneList.Count -1; i >= 0; i--)
            {
                if (sceneList[i].IsEmpty)
                {
                    continue;
                }

                if (sceneList[i].Asset == sceneToRemove.Asset)
                {
                    Debug.LogWarning("Removing manager scene " + sceneToRemove.Name + " from scene list.");
                    sceneList[i] = SceneInfo.Empty;
                    changed = true;
                }
            }

            return changed;
        }

        private static bool RemoveOrClearDuplicateEntries(List<SceneInfo> sceneList)
        {
            HashSet<int> buildIndexes = new HashSet<int>();
            bool changed = false;

            for (int i = 0; i < sceneList.Count; i++)
            {
                if (!sceneList[i].IsEmpty && sceneList[i].IsInBuildSettings && !buildIndexes.Add(sceneList[i].BuildIndex))
                {   // If we encounter a duplicate, just set it to entry.
                    // This will ensure we don't get duplicates when we add new elements to the array.
                    Debug.LogWarning("Found duplicate entry in scene list at " + i + ", removing");
                    sceneList[i] = SceneInfo.Empty;
                    changed = true;
                }
            }
           
            return changed;
        }
        #endregion
#endif
    }
}