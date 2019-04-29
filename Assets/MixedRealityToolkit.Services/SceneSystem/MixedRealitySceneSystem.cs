// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Microsoft.MixedReality.Toolkit.SceneSystem
{
    /// <summary>
    /// The default implementation of the <see cref="Microsoft.MixedReality.Toolkit.SceneSystem.IMixedRealitySceneSystem"/>
    /// </summary>
    public class MixedRealitySceneSystem : BaseCoreSystem, IMixedRealitySceneSystem
    {
        public MixedRealitySceneSystem(
            IMixedRealityServiceRegistrar registrar,
            MixedRealitySceneSystemProfile profile) : base(registrar, profile)
        {
            this.profile = profile;
        }

        private MixedRealitySceneSystemProfile profile;

        /// <inheritdoc />
        bool IEqualityComparer.Equals(object x, object y)
        {
            // There shouldn't be other Boundary Managers to compare to.
            return false;
        }

        /// <inheritdoc />
        int IEqualityComparer.GetHashCode(object obj)
        {
            return Mathf.Abs(SourceName.GetHashCode());
        }

        /// <inheritdoc />
        public uint SourceId { get; } = 0;

        /// <inheritdoc />
        public string SourceName { get; } = "Mixed Reality Scene System";

        public override void Initialize()
        {
#if UNITY_EDITOR
            // Subscribe to editor events
            EditorApplication.playModeStateChanged += playModeStateChanged;
            EditorApplication.projectChanged += projectChanged;
            EditorApplication.hierarchyChanged += heirarcyChanged;
            EditorSceneManager.newSceneCreated += newSceneCreated;
            EditorSceneManager.sceneOpened += sceneOpened;
            EditorSceneManager.sceneClosed += sceneClosed;
            EditorSceneManager.activeSceneChangedInEditMode += activeSceneChangedInEditMode;

            UpdateBuildSettings();
#endif
        }

        public override void Update()
        {
            Debug.Log("Updating MixedRealitySceneSystem");
#if UNITY_EDITOR
            UpdateBuildSettings();
#endif
        }


#if UNITY_EDITOR
        private bool updatingSettingsOnEditorChanged = false;

        #region update triggers from editor events

        private void playModeStateChanged(PlayModeStateChange change)
        {
            switch (change)
            {
                case PlayModeStateChange.EnteredEditMode:
                case PlayModeStateChange.ExitingPlayMode:
                    UpdateSettingsOnEditorChanged(false);
                    break;
            }
        }

        private void activeSceneChangedInEditMode(Scene prevActiveScene, Scene newActiveScene)
        {
            // Don't change the active scene in this case.
            // Otherwise we may usurp the creation of a new scene
            // and steal its newly created camera and directional light
            UpdateSettingsOnEditorChanged(false);
        }

        private void heirarcyChanged()
        {
            UpdateSettingsOnEditorChanged(true);
        }

        private void projectChanged()
        {
            UpdateSettingsOnEditorChanged(true);
        }

        private void sceneClosed(Scene scene)
        {
            UpdateSettingsOnEditorChanged(true);
        }

        private void sceneOpened(Scene scene, OpenSceneMode mode)
        {
            UpdateSettingsOnEditorChanged(true);
        }

        private void newSceneCreated(Scene scene, NewSceneSetup setup, NewSceneMode mode)
        {
            UpdateSettingsOnEditorChanged(true);
        }

        #endregion

        private void UpdateSettingsOnEditorChanged(bool updateActiveScene = true)
        {
            if (updatingSettingsOnEditorChanged || EditorApplication.isPlaying || EditorApplication.isCompiling)
            {
                return;
            }

            updatingSettingsOnEditorChanged = true;

            UpdateBuildSettings();
            UpdateManagerScene(updateActiveScene);
            UpdateLightingScene(updateActiveScene);

            updatingSettingsOnEditorChanged = false;
        }

        private void UpdateManagerScene(bool updateActiveScene = true)
        {
            if (profile.ManageActiveScene)
            {
                LoadSceneInEditor(profile.ManagerSceneObject, true, updateActiveScene && profile.ManageActiveScene && profile.ActiveSceneObject == profile.ManagerSceneObject, out Scene scene);
            }
        }

        private void UpdateLightingScene(bool updateActiveScene = true)
        {
            if (profile.UseLightingScene)
            {
                Scene lightingScene;
                if (LoadSceneInEditor(profile.LightingSceneObject, false, updateActiveScene && profile.ManageActiveScene && profile.ActiveSceneObject == profile.LightingSceneObject, out lightingScene))
                {
                    CopyLightingSettings(lightingScene);
                }
            }
        }

        private void UpdateBuildSettings()
        {
            if (profile.UseManagerScene)
            {
                AddSceneToBuildSettings(profile.ManagerSceneObject, true);
            }

            if (profile.UseLightingScene)
            {
                AddSceneToBuildSettings(profile.LightingSceneObject, false);
            }
        }

        private static void AddSceneToBuildSettings(UnityEngine.Object sceneObject, bool setAsFirst = false)
        {
            long localID;
            string managerGuidString;
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(sceneObject, out managerGuidString, out localID);
            GUID sceneGuid = new GUID(managerGuidString);

            // See if / where the scene exists in build settings
            int buildIndex = -1;
            for (int i = 0; i < EditorSceneManager.sceneCountInBuildSettings; i++)
            {
                if (EditorBuildSettings.scenes[i].guid == sceneGuid)
                {
                    buildIndex = i;
                    break;
                }
            }

            if (buildIndex < 0)
            {
                // It doesn't exist in the build settings, add it now
                Debug.LogWarning("Adding " + sceneObject.name + " to build settings now.");

                List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
                if (setAsFirst)
                {   // Add it to index 0
                    scenes.Insert(0, new EditorBuildSettingsScene(sceneGuid, true));
                }
                else
                {   // Just add it to the end
                    scenes.Add(new EditorBuildSettingsScene(sceneGuid, true));
                }

                EditorBuildSettings.scenes = scenes.ToArray();
            }
            else if (setAsFirst && buildIndex != 0)
            {
                // If it does exist, but isn't in the right spot, move it now
                Debug.LogWarning("Scene '" + sceneObject.name + "' was not first in build order. Changing build settings now.");

                List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
                scenes.RemoveAt(buildIndex);
                scenes.Insert(0, new EditorBuildSettingsScene(sceneGuid, true));

                EditorBuildSettings.scenes = scenes.ToArray();
            }
        }

        private static bool LoadSceneInEditor(UnityEngine.Object sceneObject, bool setAsFirst, bool setActive, out Scene editorScene)
        {
            editorScene = default(Scene);

            try
            {
                editorScene = EditorSceneManager.GetSceneByName(sceneObject.name);
                if (!editorScene.isLoaded)
                {
                    string scenePath = AssetDatabase.GetAssetOrScenePath(sceneObject);
                    EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
                }

                if (setAsFirst && EditorSceneManager.sceneCount >= 1)
                {   // Move the scene to first in order in the heirarchy
                    Scene nextScene = EditorSceneManager.GetSceneAt(0);
                    EditorSceneManager.MoveSceneBefore(editorScene, nextScene);
                }

                if (setActive)
                {   // Set this scene to the active scene
                    EditorSceneManager.SetActiveScene(editorScene);
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

            return true;
        }

        private static void CopyLightingSettings(Scene lightingScene)
        {
            // Store the active scene on entry
            Scene activeSceneOnEnter = EditorSceneManager.GetActiveScene();

            // For all loaded scenes, copy and paste the lighting settings from the light scene
            // Store the settings in serialized objects
            SerializedObject sourceLightmapSettings;
            SerializedObject sourceRenderSettings;

            // Set the active scene to the lighting scene
            EditorSceneManager.SetActiveScene(lightingScene);
            // If we can't get the source settings for some reason, abort
            if (!GetLightmapAndRenderSettings(out sourceLightmapSettings, out sourceRenderSettings))
            {
                return;
            }

            for (int i = 0; i < EditorSceneManager.sceneCount; i++)
            {
                Scene otherScene = EditorSceneManager.GetSceneAt(i);
                if (otherScene.name == lightingScene.name)
                {
                    continue;
                }

                // Set the scene to active so the static methods now point to this scene, not the lighting scene
                EditorSceneManager.SetActiveScene(otherScene);

                SerializedObject targetLightmapSettings;
                SerializedObject targetRenderSettings;
                bool madeChanges = false;
                if (GetLightmapAndRenderSettings(out targetLightmapSettings, out targetRenderSettings))
                {
                    madeChanges |= CopySerializedObject(sourceLightmapSettings, targetLightmapSettings);
                    madeChanges |= CopySerializedObject(sourceRenderSettings, targetRenderSettings);
                }

                if (madeChanges)
                {
                    Debug.LogWarning("Changed lighting settings in scene " + otherScene.name + " to match lighting scene " + lightingScene.name);
                }
            }

            // Set active scene back to the lighting scene
            EditorSceneManager.SetActiveScene(activeSceneOnEnter);
        }

        private static bool GetLightmapAndRenderSettings(out SerializedObject lightmapSettings, out SerializedObject renderSettings)
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

        private static bool CopySerializedObject(SerializedObject source, SerializedObject target)
        {
            bool madeChanges = false;
            SerializedProperty prop = source.GetIterator();
            while (prop.Next(true))
            {
                madeChanges |= target.CopyFromSerializedPropertyIfDifferent(prop);
            }

            if (madeChanges)
            {
                target.ApplyModifiedPropertiesWithoutUndo();
            }

            return madeChanges;
        }
#endif
    }
}