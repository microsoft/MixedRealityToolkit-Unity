// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if UNITY_EDITOR
using Microsoft.MixedReality.Toolkit.SceneSystem;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Utilities for loading / saving scenes in editor via SceneInfo.
    /// Because SceneInfo is defined in MixedRealityToolkit, this can't be kept in Editor utilities.
    /// </summary>
    public static class EditorSceneUtils
    {
        /// <summary>
        /// Enum used by this class to specify build settings order
        /// </summary>
        public enum BuildIndexTarget
        {
            First,
            None,
            Last,
        }

        /// <summary>
        /// Creates a new scene with sceneName and saves to path.
        /// </summary>
        public static SceneInfo CreateAndSaveScene(string sceneName, string path = null)
        {
            SceneInfo sceneInfo = default(SceneInfo);

            if (!EditorSceneManager.EnsureUntitledSceneHasBeenSaved("Save untitled scene before proceeding?"))
            {
                return sceneInfo;
            }

            Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);

            if (string.IsNullOrEmpty(path))
            {
                path = "Assets/" + sceneName + ".unity";
            }

            if (!EditorSceneManager.SaveScene(newScene, path))
            {
                Debug.LogError("Couldn't create and save scene " + sceneName + " at path " + path);
                return sceneInfo;
            }

            SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
            sceneInfo.Asset = sceneAsset;
            sceneInfo.Name = sceneAsset.name;
            sceneInfo.Path = path;

            return sceneInfo;
        }

        /// <summary>
        /// Adds scene to build settings.
        /// </summary>
        /// <param name="sceneObject">Scene object reference.</param>
        /// <param name="setAsFirst">Sets as first scene to be loaded.</param>
        public static bool AddSceneToBuildSettings(
            SceneInfo scene,
            EditorBuildSettingsScene[] scenes,
            BuildIndexTarget buildIndexTarget = BuildIndexTarget.None)
        {
            if (scene.IsEmpty)
            {   // Can't add a null scene to build settings
                return false;
            }

            long localID;
            string managerGuidString;
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(scene.Asset, out managerGuidString, out localID);
            GUID sceneGuid = new GUID(managerGuidString);

            List<EditorBuildSettingsScene> newScenes = new List<EditorBuildSettingsScene>(scenes);
            // See if / where the scene exists in build settings
            int buildIndex = EditorSceneUtils.GetSceneBuildIndex(sceneGuid, newScenes);

            if (buildIndex < 0)
            {
                // It doesn't exist in the build settings, add it now
                switch (buildIndexTarget)
                {
                    case BuildIndexTarget.First:
                        // Add it to index 0
                        newScenes.Insert(0, new EditorBuildSettingsScene(sceneGuid, true));
                        break;

                    case BuildIndexTarget.None:
                    default:
                        // Just add it to the end
                        newScenes.Add(new EditorBuildSettingsScene(sceneGuid, true));
                        break;
                }

                EditorBuildSettings.scenes = newScenes.ToArray();
                return true;
            }
            else
            {
                switch (buildIndexTarget)
                {
                    // If it does exist, but isn't in the right spot, move it now
                    case BuildIndexTarget.First:
                        if (buildIndex != 0)
                        {
                            Debug.LogWarning("Scene '" + scene.Name + "' was not first in build order. Changing build settings now.");

                            newScenes.RemoveAt(buildIndex);
                            newScenes.Insert(0, new EditorBuildSettingsScene(sceneGuid, true));
                            EditorBuildSettings.scenes = newScenes.ToArray();
                        }
                        return true;

                    case BuildIndexTarget.Last:
                        if (buildIndex != EditorSceneManager.sceneCountInBuildSettings - 1)
                        {
                            newScenes.RemoveAt(buildIndex);
                            newScenes.Insert(newScenes.Count - 1, new EditorBuildSettingsScene(sceneGuid, true));
                            EditorBuildSettings.scenes = newScenes.ToArray();
                        }
                        return true;

                    case BuildIndexTarget.None:
                    default:
                        // Do nothing
                        return false;

                }
            }
        }

        /// <summary>
        /// Gets the build index for a scene GUID.
        /// There are many ways to do this in Unity but this is the only 100% reliable method I know of.
        /// </summary>
        public static int GetSceneBuildIndex(GUID sceneGUID, List<EditorBuildSettingsScene> scenes)
        {
            int buildIndex = -1;
            int sceneCount = 0;
            for (int i = 0; i < scenes.Count; i++)
            {
                if (scenes[i].guid == sceneGUID)
                {
                    buildIndex = sceneCount;
                    break;
                }

                if (scenes[i].enabled)
                {
                    sceneCount++;
                }
            }

            return buildIndex;
        }

        /// <summary>
        /// Attempts to load scene in editor using a scene object reference.
        /// </summary>
        /// <param name="sceneObject">Scene object reference.</param>
        /// <param name="setAsFirst">Whether to set first in the hierarchy window.</param>
        /// <param name="editorScene">The loaded scene.</param>
        /// <returns>True if successful.</returns>
        public static bool LoadScene(SceneInfo sceneInfo, bool setAsFirst, out Scene editorScene)
        {
            editorScene = default(Scene);

            try
            {
                editorScene = EditorSceneManager.GetSceneByName(sceneInfo.Name);

                if (editorScene.isLoaded)
                {   // Already open - no need to do anything!
                    return true;
                }

                string scenePath = AssetDatabase.GetAssetOrScenePath(sceneInfo.Asset);
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);

                if (setAsFirst && EditorSceneManager.loadedSceneCount >= 1)
                {   // Move the scene to first in order in the hierarchy
                    Scene nextScene = EditorSceneManager.GetSceneAt(0);
                    EditorSceneManager.MoveSceneBefore(editorScene, nextScene);
                }
            }
            catch (InvalidOperationException)
            {
                // This can happen if we're trying to load immediately upon recompilation.
                return false;
            }
            catch (ArgumentException)
            {
                // This can happen if the scene is an invalid scene and we try to SetActive.
                return false;
            }
            catch (NullReferenceException)
            {
                // This can happen if the scene object is null.
                return false;
            }
            catch (MissingReferenceException)
            {
                // This can happen if the scene object is null.
                return false;
            }

            return true;
        }

        /// <summary>
        /// Finds the scene if loaded.
        /// </summary>
        /// <returns>True if scene is loaded</returns>
        public static bool GetSceneIfLoaded(SceneInfo sceneInfo, out Scene editorScene)
        {
            editorScene = default(Scene);
            editorScene = EditorSceneManager.GetSceneByName(sceneInfo.Name);
            return editorScene.IsValid() && editorScene.isLoaded;
        }

        /// <summary>
        /// Returns all root GameObjects in all open scenes.
        /// </summary>
        public static IEnumerable<GameObject> GetRootGameObjectsInLoadedScenes()
        {
            for (int i = 0; i < EditorSceneManager.sceneCount; i++)
            {
                Scene openScene = EditorSceneManager.GetSceneAt(i);
                if (!openScene.isLoaded)
                {   // Oh, Unity.
                    continue;
                }

                foreach (GameObject rootGameObject in openScene.GetRootGameObjects())
                    yield return rootGameObject;
            }
            yield break;
        }

        /// <summary>
        /// Returns true if user is currently editing a prefab.
        /// </summary>
        public static bool IsEditingPrefab()
        {
#if UNITY_2021_2_OR_NEWER
            var prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
#else
            var prefabStage = UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
#endif
            return prefabStage != null;
        }

        /// <summary>
        /// Unloads a scene in the editor and catches any errors that can happen along the way.
        /// </summary>
        public static bool UnloadScene(SceneInfo sceneInfo, bool removeFromHeirarchy)
        {
            Scene editorScene = default(Scene);

            try
            {
                editorScene = EditorSceneManager.GetSceneByName(sceneInfo.Name);
                if (editorScene.isLoaded)
                {
                    EditorSceneManager.CloseScene(editorScene, removeFromHeirarchy);
                }
            }
            catch (InvalidOperationException)
            {
                // This can happen if we're trying to load immediately upon recompilation.
                return false;
            }
            catch (ArgumentException)
            {
                // This can happen if the scene is an invalid scene and we try to SetActive.
                return false;
            }
            catch (NullReferenceException)
            {
                // This can happen if the scene object is null.
                return false;
            }
            catch (MissingReferenceException)
            {
                // This can happen if the scene object is null.
                return false;
            }

            return true;
        }

        /// <summary>
        /// Attempts to set the active scene and catches all the various ways it can go wrong.
        /// Returns true if successful.
        /// </summary>
        public static bool SetActiveScene(Scene scene)
        {
            try
            {
                EditorSceneManager.SetActiveScene(scene);
            }
            catch (InvalidOperationException)
            {
                // This can happen if we're trying to load immediately upon recompilation.
                return false;
            }
            catch (ArgumentException)
            {
                // This can happen if the scene is an invalid scene and we try to SetActive.
                return false;
            }
            catch (NullReferenceException)
            {
                // This can happen if the scene object is null.
                return false;
            }
            catch (MissingReferenceException)
            {
                // This can happen if the scene object is null.
                return false;
            }

            return true;
        }

        /// <summary>
        /// Copies the lighting settings from the lighting scene to the active scene
        /// </summary>
        public static void CopyLightingSettingsToActiveScene(Scene lightingScene)
        {
            // Store the active scene on entry
            Scene activeSceneOnEnter = EditorSceneManager.GetActiveScene();

            // No need to do anything
            if (activeSceneOnEnter == lightingScene)
                return;

            SerializedObject sourceLightmapSettings;
            SerializedObject sourceRenderSettings;

            // Set the active scene to the lighting scene
            SetActiveScene(lightingScene);
            // If we can't get the source settings for some reason, abort
            if (!GetLightingAndRenderSettings(out sourceLightmapSettings, out sourceRenderSettings))
            {
                return;
            }

            bool madeChanges = false;

            // Set active scene back to the active scene on enter
            if (SetActiveScene(activeSceneOnEnter))
            {
                SerializedObject targetLightmapSettings;
                SerializedObject targetRenderSettings;

                if (GetLightingAndRenderSettings(out targetLightmapSettings, out targetRenderSettings))
                {
                    string[] propsToIgnore = new string[] { "m_IndirectSpecularColor" };

                    madeChanges |= SerializedObjectUtils.CopySerializedObject(sourceLightmapSettings, targetLightmapSettings, propsToIgnore);
                    madeChanges |= SerializedObjectUtils.CopySerializedObject(sourceRenderSettings, targetRenderSettings, propsToIgnore);
                }
            }

            if (madeChanges)
            {
                Debug.LogWarning("Changed lighting settings in scene " + activeSceneOnEnter.name + " to match lighting scene " + lightingScene.name);
                EditorSceneManager.MarkSceneDirty(activeSceneOnEnter);
            }
        }

        /// <summary>
        /// Goes through a scene's objects and checks for components that aren't found in permittedComponentTypes
        /// If any are found, they're added to the violations list.
        /// </summary>
        public static bool EnforceSceneComponents(Scene scene, IEnumerable<Type> permittedComponentTypes, List<Component> violations)
        {
            if (!scene.IsValid() || !scene.isLoaded)
            {
                return false;
            }

            int typesEvaluated = 0;
            bool foundAtLeastOneViolation = false;

            try
            {
                foreach (GameObject rootGameObject in scene.GetRootGameObjects())
                {
                    foreach (Component component in rootGameObject.GetComponentsInChildren<Component>())
                    {
                        bool componentIsPermitted = false;
                        foreach (Type type in permittedComponentTypes)
                        {
                            if (type.IsAssignableFrom(component.GetType()))
                            {
                                componentIsPermitted = true;
                                break;
                            }
                            typesEvaluated++;
                        }

                        if (!componentIsPermitted)
                        {
                            foundAtLeastOneViolation = true;
                            violations.Add(component);
                        }
                    }
                }
            }
            catch (Exception)
            {
                // This can go wrong if GetRootSceneObjects fails.
            }

            if (typesEvaluated == 0)
            {
                Debug.LogError("Permitted component types must include at least one type.");
            }

            return foundAtLeastOneViolation;
        }

        /// <summary>
        /// Gets serialized objects for lightmap and render settings from active scene.
        /// </summary>
        public static bool GetLightingAndRenderSettings(out SerializedObject lightmapSettings, out SerializedObject renderSettings)
        {
            lightmapSettings = null;
            renderSettings = null;

            UnityEngine.Object lightmapSettingsObject = null;
            UnityEngine.Object renderSettingsObject = null;

            try
            {
                // Use reflection to get the serialized objects of lightmap settings and render settings
                Type lightmapSettingsType = typeof(LightmapEditorSettings);
                Type renderSettingsType = typeof(RenderSettings);

                lightmapSettingsObject = (UnityEngine.Object)lightmapSettingsType.GetMethod("GetLightmapSettings", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null);
                renderSettingsObject = (UnityEngine.Object)renderSettingsType.GetMethod("GetRenderSettings", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null);
            }
            catch (Exception)
            {
                Debug.LogWarning("Couldn't get lightmap or render settings. This version of Unity may not support this operation.");
                return false;
            }

            if (lightmapSettingsObject == null)
            {
                Debug.LogWarning("Couldn't get lightmap settings object");
                return false;
            }

            if (renderSettingsObject == null)
            {
                Debug.LogWarning("Couldn't get render settings object");
                return false;
            }

            // Store the settings in serialized objects
            lightmapSettings = new SerializedObject(lightmapSettingsObject);
            renderSettings = new SerializedObject(renderSettingsObject);
            return true;
        }

        /// <summary>
        /// Checks build settings for possible errors and displays warnings.
        /// </summary>
        public static bool CheckBuildSettingsForDuplicates(List<SceneInfo> allScenes, Dictionary<string, List<int>> duplicates)
        {
            duplicates.Clear();
            List<int> indexes = null;
            bool foundDuplicate = false;

            foreach (SceneInfo sceneInfo in allScenes)
            {
                if (duplicates.TryGetValue(sceneInfo.Name, out indexes))
                {
                    indexes.Add(sceneInfo.BuildIndex);
                    foundDuplicate = true;
                }
                else
                {
                    duplicates.Add(sceneInfo.Name, new List<int> { sceneInfo.BuildIndex });
                }
            }

            return foundDuplicate;
        }
    }
}
#endif
