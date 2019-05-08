// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
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

        public bool UseLightingScene { get { return useLightingScene && lightingScenes.Length > 0; } }

        public SceneInfo ManagerScene => managerScene;

        public SceneInfo DefaultLightingScene { get { return lightingScenes[defaultLightingSceneIndex]; } }

        public IEnumerable<SceneInfo> LightingScenes { get { return lightingScenes; } }

        public IEnumerable<SceneInfo> ContentScenes { get { return contentScenes; } }

        [SerializeField]
        [Tooltip("Using a manager scene ensures a MixedRealityToolkit instance will always be loaded first in your application. It also ensures that this scene and instance will never be unloaded.")]
        private bool useManagerScene = true;

        [SerializeField]
        private SceneInfo managerScene = default(SceneInfo);

        [SerializeField]
        [Tooltip("Using a content scene ensures that the same lighting settings will be enforced across all loaded scenes.")]
        private bool useLightingScene = true;

        [SerializeField]
        private int defaultLightingSceneIndex = 0;

        [SerializeField]
        private SceneInfo[] lightingScenes = new SceneInfo[0];

        [SerializeField]
        [Tooltip("Scene names in your build settings which aren't a lighting or manager scene. These will be managed by the service.")]
        private SceneInfo[] contentScenes = new SceneInfo[0];

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

#if UNITY_EDITOR
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

            defaultLightingSceneIndex = Mathf.Clamp(defaultLightingSceneIndex, 0, lightingScenes.Length - 1);
        }

        private static bool RemoveOrClearDuplicateEntries(SceneInfo[] sceneInfoArray)
        {
            HashSet<int> buildIndexes = new HashSet<int>();
            bool changed = false;

            for (int i = 0; i < sceneInfoArray.Length; i++)
            {
                if (!sceneInfoArray[i].IsEmpty && !buildIndexes.Add(sceneInfoArray[i].BuildIndex))
                {   // If we encounter a duplicate, just set it to entry.
                    // This will ensure we don't get duplicates when we add new elements to the array.
                    sceneInfoArray[i] = SceneInfo.Empty;
                    changed = true;
                }
            }
           
            return changed;
        }
#endif
    }
}