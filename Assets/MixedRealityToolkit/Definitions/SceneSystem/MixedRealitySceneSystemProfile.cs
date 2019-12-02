// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SceneSystem
{
    /// <summary>
    /// Configuration profile settings for setting up scene system.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Profiles/Mixed Reality Scene System Profile", fileName = "MixedRealitySceneSystemProfile", order = (int)CreateProfileMenuItemIndices.SceneSystem)]
    [MixedRealityServiceProfile(typeof(IMixedRealitySceneSystem))]
    [HelpURL("https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/SceneSystem/SceneSystemGettingStarted.html")]
    public class MixedRealitySceneSystemProfile : BaseMixedRealityProfile
    {
        /// <summary>
        /// Internal class used to cache lighting settings associated with a scene.
        /// </summary>
        [Serializable]
        internal sealed class CachedLightingSettings
        {
            public string SceneName;
            public RuntimeRenderSettings RenderSettings;
            public RuntimeLightingSettings LightingSettings;
            public RuntimeSunlightSettings SunlightSettings;
            public DateTime TimeStamp;
        }

        public bool UseManagerScene { get { return useManagerScene && !managerScene.IsEmpty; } }

        public bool UseLightingScene { get { return useLightingScene && lightingScenes.Count > 0; } }

        public SceneInfo ManagerScene => managerScene;

        public SceneInfo DefaultLightingScene { get { return lightingScenes[defaultLightingSceneIndex]; } }

        public IEnumerable<SceneInfo> LightingScenes { get { return lightingScenes; } }

        public IEnumerable<SceneInfo> ContentScenes { get { return contentScenes; } }

        public IEnumerable<string> ContentTags { get { return contentTags; } }

        public int NumLightingScenes { get { return lightingScenes.Count; } }

        public int NumContentScenes { get { return contentScenes.Count; } }

        public IEnumerable<Type> PermittedLightingSceneComponentTypes
        {
            get
            {
                foreach (SystemType systemType in permittedLightingSceneComponentTypes) { yield return systemType.Type; }
            }
        }

#if UNITY_EDITOR
        public bool EditorManageBuildSettings => editorManageBuildSettings;

        public bool EditorManageLoadedScenes => editorManageLoadedScenes;

        public bool EditorEnforceSceneOrder => editorEnforceSceneOrder;

        public bool EditorEnforceLightingSceneTypes => editorEnforceLightingSceneTypes;

        public bool EditorLightingCacheOutOfDate => editorLightingCacheOutOfDate;

        public bool EditorLightingCacheUpdateRequested { get; set; }
#endif

        [SerializeField]
        private bool useManagerScene = true;

        [SerializeField]
        private SceneInfo managerScene = default(SceneInfo);

        [SerializeField]
        private bool useLightingScene = true;

        [SerializeField]
        private int defaultLightingSceneIndex = 0;

        [SerializeField]
        private List<SceneInfo> lightingScenes = new List<SceneInfo>();

        [SerializeField]
        private List<SceneInfo> contentScenes = new List<SceneInfo>();

        [SerializeField]
        private SystemType[] permittedLightingSceneComponentTypes = new SystemType[] {
            new SystemType(typeof(Transform)),
            new SystemType(typeof(GameObject)),
            new SystemType(typeof(Light)),
            new SystemType(typeof(ReflectionProbe)),
            new SystemType(typeof(LightProbeGroup)),
            new SystemType(typeof(LightProbeProxyVolume)),
        };

        // These will be hidden by the default inspector.
        [SerializeField]
        [Tooltip("Cached content tags found in your content scenes")]
        private List<string> contentTags = new List<string>();

        // These will be hidden by the default inspector.
        [SerializeField]
        [Tooltip("Cached lighting settings from your lighting scenes")]
        private List<CachedLightingSettings> cachedLightingSettings = new List<CachedLightingSettings>();

        #region editor settings

        // CS414 is disabled during this section because these properties are being used in the editor
        // scenario - when this file is build for player scenario, these serialized fields still exist
        // but are not used.
        #pragma warning disable 414
        [SerializeField]
        [Tooltip("If true, the service will update your build settings automatically, ensuring that all manager, lighting and content scenes are added. Disable this if you want total control over build settings.")]
        private bool editorManageBuildSettings = true;

        [SerializeField]
        [Tooltip("If true, the service will ensure manager scene is displayed first in scene hierarchy, followed by lighting and then content. Disable this if you want total control over scene hierarchy.")]
        private bool editorEnforceSceneOrder = true;

        [SerializeField]
        [Tooltip("If true, service will ensure that manager scenes and lighting scenes are always loaded. Disable if you want total control over which scenes are loaded in editor.")]
        private bool editorManageLoadedScenes = true;

        [SerializeField]
        [Tooltip("If true, service will ensure that only lighting-related components are allowed in lighting scenes. Disable if you want total control over the content of lighting scenes.")]
        private bool editorEnforceLightingSceneTypes = true;

        [SerializeField]
        private bool editorLightingCacheOutOfDate = false;
        #pragma warning restore 414

        #endregion

        public bool GetLightingSceneSettings(
            string lightingSceneName,
            out SceneInfo lightingScene,
            out RuntimeLightingSettings lightingSettings,
            out RuntimeRenderSettings renderSettings,
            out RuntimeSunlightSettings sunlightSettings)
        {
            lightingSettings = default(RuntimeLightingSettings);
            renderSettings = default(RuntimeRenderSettings);
            sunlightSettings = default(RuntimeSunlightSettings);
            lightingScene = SceneInfo.Empty;

            for (int i = 0; i < lightingScenes.Count; i++)
            {
                if (lightingScenes[i].Name == lightingSceneName)
                {
                    lightingScene = lightingScenes[i];
                    break;
                }
            }

            if (lightingScene.IsEmpty)
            {   // If we didn't find a lighting scene, don't bother looking for a cache
                return false;
            }

            bool foundCache = false;
            for (int i = 0; i < cachedLightingSettings.Count; i++)
            {
                CachedLightingSettings cache = cachedLightingSettings[i];
                if (cache.SceneName == lightingSceneName)
                {
                    lightingSettings = cache.LightingSettings;
                    renderSettings = cache.RenderSettings;
                    sunlightSettings = cache.SunlightSettings;
                    foundCache = true;
                    break;
                }
            }

            return foundCache;
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

            bool saveChanges = false;

            // Remove any duplicate entries from our lighting and content scene lists
            saveChanges |= (RemoveOrClearDuplicateEntries(lightingScenes) || RemoveOrClearDuplicateEntries(contentScenes));

            // Ensure that manager scenes are not contained in content or lighting scenes
            saveChanges |= (UseManagerScene && (RemoveScene(lightingScenes, managerScene) || RemoveScene(contentScenes, managerScene)));

            // Ensure that content scenes are not included in lighting scenes
            saveChanges |= (UseLightingScene && RemoveScenes(lightingScenes, contentScenes));

            // Build our content tags
            List<string> newContentTags = new List<string>();
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

                if (!newContentTags.Contains(contentScene.Tag))
                {
                    newContentTags.Add(contentScene.Tag);
                }
            }
            
            // See if our content tags have changed
            if (!contentTags.SequenceEqual(newContentTags))
            {
                contentTags = newContentTags;
                saveChanges = true;
            }

            defaultLightingSceneIndex = Mathf.Clamp(defaultLightingSceneIndex, 0, lightingScenes.Count - 1);
            
            if (saveChanges)
            {   // We need to tie this directly to lighting scenes somehow
                editorLightingCacheOutOfDate = true;
                // Make sure our changes are saved to disk!
                AssetDatabase.Refresh();
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssets();
            }
        }

        /// <summary>
        /// Clears cached lighting settings. 
        /// Used to ensure we don't end up with 'dead' cached data.
        /// </summary>
        public void ClearLightingCache()
        {
            cachedLightingSettings.Clear();
        }

        /// <summary>
        /// Used to update the cached lighting / render settings.
        /// Since extracting them is complex and requires scene loading, I thought it best to avoid having the profile do it.
        /// </summary>
        /// <param name="sceneInfo">The scene these settings belong to.</param>
        public void SetLightingCache(SceneInfo sceneInfo, RuntimeLightingSettings lightingSettings, RuntimeRenderSettings renderSettings, RuntimeSunlightSettings sunlightSettings)
        {
            CachedLightingSettings settings = new CachedLightingSettings();
            settings.SceneName = sceneInfo.Name;
            settings.LightingSettings = lightingSettings;
            settings.RenderSettings = renderSettings;
            settings.SunlightSettings = sunlightSettings;
            settings.TimeStamp = DateTime.Now;

            cachedLightingSettings.Add(settings);

            editorLightingCacheOutOfDate = false;
        }

        /// <summary>
        /// Sets editorLightingCacheOutOfDate to true and saves the profile. 
        /// </summary>
        public void SetLightingCacheDirty()
        {
            editorLightingCacheOutOfDate = true;

            AssetDatabase.Refresh();
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }

        public DateTime GetEarliestLightingCacheTimestamp()
        {
            if (cachedLightingSettings.Count <= 0)
            {
                return DateTime.MinValue;
            }

            DateTime earliestTimeStamp = DateTime.MaxValue;
            foreach (CachedLightingSettings settings in cachedLightingSettings)
            {
                if (settings.TimeStamp < earliestTimeStamp)
                {
                    earliestTimeStamp = settings.TimeStamp;
                }
            }

            return earliestTimeStamp;
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
            HashSet<string> scenePaths = new HashSet<string>();
            bool changed = false;

            for (int i = 0; i < sceneList.Count; i++)
            {
                if (sceneList[i].IsEmpty)
                {
                    continue;
                }

                if (!scenePaths.Add(sceneList[i].Path))
                {   // If we encounter a duplicate, just set it to empty.
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
